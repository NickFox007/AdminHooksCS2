using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminHGR;

public struct AdminLimit
{
    public string Flag { get; set; }
    public int HooksValue { get; set; }
    public int GrabsValue { get; set; }
    public int RopesValue { get; set; }
    public bool OnlyEndRound { get; set; }
    
    public AdminLimit()
    {
        Flag = "@css/admin";
        HooksValue = 0;
        GrabsValue = 0;
        RopesValue = 0;
        OnlyEndRound = false;
    }
    public AdminLimit(string Flag, int HooksValue = 0, int GrabsValue = 0, int RopesValue = 0, bool OnlyEndRound = false)
    {
        this.Flag = Flag;
        this.HooksValue = HooksValue;
        this.GrabsValue = GrabsValue;
        this.RopesValue = RopesValue;
        this.OnlyEndRound = OnlyEndRound;
    }
}
