using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Admin;
using HGR;
using System.Text.Json.Serialization;

namespace AdminHGR;

public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("Values")] public List<AdminLimit> Limits { get; set; } = new List<AdminLimit>([new AdminLimit("@css/root", -1, -1, -1), new AdminLimit("@css/generic", 3, 0, 0, true)]);
}

public class AdminHGR: BasePlugin, IPluginConfig<PluginConfig>
{
    public PluginConfig Config { get; set; }
    public override string ModuleName => "[Admin] HGR";
    public override string ModuleVersion => "2.0";
    public override string ModuleAuthor => "Nick Fox";

    private IHGRApi? _hgr;
    private PluginCapability<IHGRApi> PluginHooks { get; } = new("hgr:nfcore");
    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
    }


    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _hgr = PluginHooks.Get();
                
        hgrCount = new int[3][];
        for (int i = 0; i < 3; i++)
            hgrCount[i] = new int[65];

        if (_hgr == null) return;
        _hgr.AddHook(HGRHook, 75);
    }

    public override void Unload(bool hotReload)
    {        
        _hgr.RemHook(HGRHook);
    }


    private int[][] hgrCount; // 0 - Hooks, 1 - Grabs, 2 - Ropes


    private void HGRHook(PlayerHGR info)
    {
        if (info.State() == HGRState.Disabled)
        {
            int i = 0;
            switch (info.Mode())
            {
                case HGRMode.Hook: i = 0; break;
                case HGRMode.Grab: i = 1; break;
                case HGRMode.Rope: i = 2; break;
                default: return;
            }

            if (hgrCount[i][info.Player().Slot] == -1)
                info.Enable();
            else
                if (hgrCount[i][info.Player().Slot] > 0)
            {
                hgrCount[i][info.Player().Slot]--;
                info.Enable();

                if (hgrCount[i][info.Player().Slot] == 0)
                    info.Player().PrintToChat(Localizer["admin_hgr.expired"]);
                else
                    info.Player().PrintToChat(String.Format(Localizer["admin_hgr.use_count"], hgrCount[i][info.Player().Slot]));
            }
        }
    }

    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 65; j++)
                hgrCount[i][j] = 0;
        return HookResult.Continue;
    }


    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        OnRoundEvent(true);
        return HookResult.Continue;
    }


    [GameEventHandler]
    public HookResult OnFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        OnRoundEvent(false);
        return HookResult.Continue;
    }

    private void OnRoundEvent(bool endRound)
    {
        foreach (var player in Utilities.GetPlayers())
        {
            var slot = player.Slot;
            (int hooks, int grabs, int ropes) = GetValues(player);

            AddHGRValues(slot, 0, hooks);
            AddHGRValues(slot, 1, grabs);
            AddHGRValues(slot, 2, ropes);
          
        }
    }

    private (int, int, int) GetValues(CCSPlayerController player, bool endRound = false)
    {
        var values = new int[3];

        foreach(var limit in Config.Limits)
        {
            if (AdminManager.PlayerHasPermissions(player, limit.Flag) && limit.OnlyEndRound == endRound)
            {
                if (limit.HooksValue == -1)
                    values[0] = -1;
                else if(limit.HooksValue > values[0])
                    values[0] = limit.HooksValue;

                if (limit.GrabsValue == -1)
                    values[1] = -1;
                else if (limit.GrabsValue > values[1])
                    values[1] = limit.GrabsValue;

                if (limit.RopesValue == -1)
                    values[2] = -1;
                else if (limit.RopesValue > values[2])
                    values[2] = limit.RopesValue;
            }
        }

        return (values[0], values[1], values[2]);
    }

    private void AddHGRValues(int slot, int index, int value)
    {
        if(value == -1)
            hgrCount[index][slot] = -1;
        else if (hgrCount[index][slot] != -1)
            hgrCount[index][slot] += value;
    }

}