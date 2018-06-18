using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnLib;

namespace pbpb
{
    public static class ____kjoqweiqwjei { }

    public class Analytics
    {
        public Analytics(bool Started = true) {

            StartedTime = Started ? Environment.TickCount : int.MaxValue;
        }
        public int StartedTime { get; private set; }
        public int TickFromStartToNow => Environment.TickCount - StartedTime;


        public int MatchLiveCount { get; private set; }
        public void IncMatchLive() => MatchLiveCount++;

        public int StartExecuteCount { get; private set; }
        public void IncStartExecute() => StartExecuteCount++;

        public int KillExecuteCount { get; private set; }
        public void IncKillExecute() => KillExecuteCount++;

        public int KillExecuteSteamCount { get; private set; }
        public void IncKillExecuteSteam() => KillExecuteSteamCount++;

        public string Report => String.Format(
            
            "Match Live Count: {0}" + Environment.NewLine +
            "Start Execute Count: {1}" + Environment.NewLine +
            "Kill Execute Count: {2}" + Environment.NewLine +
            "Kill Execute Steam Count: {3}" + Environment.NewLine + Environment.NewLine +
            "Total Time: {4}" + Environment.NewLine,

            MatchLiveCount, 
            StartExecuteCount, 
            KillExecuteCount, 
            KillExecuteSteamCount,
            STime.TickToStr(TickFromStartToNow)

        );
    }

    partial class Form1 : Form {

        public static Analytics Analytics = new Analytics(false);
    }
}
