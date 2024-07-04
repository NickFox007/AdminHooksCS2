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
        public string Flag;
        public int Value;
        public bool OnlyEndRound;
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
