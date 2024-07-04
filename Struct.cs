using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminHooks
{
    public struct AdminHook
    {
        public string Flag { get; set; }
        public int Value { get; set; }
        public bool OnlyEndRound { get; set; }
        public AdminHook()
        {
            Flag = "";
            Value = -1;
            OnlyEndRound = false;
        }
        public AdminHook(string Flag, int Value, bool OnlyEndRound = false)
        {
            this.Flag = Flag;
            this.Value = Value;
            this.OnlyEndRound = OnlyEndRound;
        }
    }    
}
