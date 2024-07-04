using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Admin;
using Hooks;
using System.Text.Json.Serialization;

namespace AdminHooks;
public class PluginConfig : BasePluginConfig
{
    [JsonPropertyName("Values")] public List<AdminHook> Limits { get; set; } = new List<AdminHook>([new AdminHook("@css/root", -1), new AdminHook("@css/generic", 3)]);
}

public class AskConnectIp : BasePlugin, IPluginConfig<PluginConfig>
{
    public PluginConfig Config { get; set; }
    public override string ModuleName => "[Admin] Hooks";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "Nick Fox";

    private IHooksApi? _hooks;
    private PluginCapability<IHooksApi> PluginHooks { get; } = new("hooks:nfcore");

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _hooks = PluginHooks.Get();

        _hooks.AddHook((playerinfo) => {

            var slot = playerinfo.Player().Slot;
            if (hooksCount[slot] > 0)
            {
                hooksCount[slot]--;
                if (hooksCount[slot] == 0)
                    playerinfo.Player().PrintToChat(Localizer["expired"]);
                else
                    playerinfo.Player().PrintToChat(String.Format(Localizer["use_count"], hooksCount[slot]));
                playerinfo.Set(HookState.Enabled);

            }
            else
                if (hooksCount[slot] == -1)
                playerinfo.Set(HookState.Enabled);

        });
    }

    public void OnConfigParsed(PluginConfig config)
    {
        Config = config;
    }
    private int[] hooksCount = new int[65];
    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        for (int i = 0; i < 65; i++)
            hooksCount[i] = 0;
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        for (int i = 0; i < 65; i++)
        {
            var player = Utilities.GetPlayerFromSlot(i);

            if (IsValidPlayer(player)) // TODO: добавить подтягивание из конфига
            {                
                var limit = AdminHookGet(player);
                if(limit != 0)
                    hooksCount[i] = limit;
            }
        }
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        for (int i = 0; i < 65; i++)
        {
            var player = Utilities.GetPlayerFromSlot(i);

            if (IsValidPlayer(player)) // TODO: добавить подтягивание из конфига
            {
                if (hooksCount[i] == -1) continue;
                var limit = AdminHookGet(player);
                if (limit > 0)
                    hooksCount[i] += limit;
                else if (limit == -1)
                    hooksCount[i] = -1;
            }
        }
        return HookResult.Continue;
    }

    private bool IsValidPlayer(CCSPlayerController player)
    {
        return player != null && player.IsValid && player.Connected == PlayerConnectedState.PlayerConnected && !player.IsBot;
    }


    public int AdminHookGet(CCSPlayerController player, bool endRound = false)
    {
        foreach(var limit in Config.Limits)
        {
            if(AdminManager.PlayerHasPermissions(player, limit.Flag) && limit.OnlyEndRound == endRound)
                return limit.Value;
        }

        return 0;
    }
}