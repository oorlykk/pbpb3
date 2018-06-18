﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnLib;

namespace pbpb
{
    public static class PubgRound {

        public static bool IsLive;

        public static int StartedTime;

        public static int EndedTime;

        public static string EndReason = "none";

        public static bool HelpedOnStart;

        //public static string RewardsFolder = AppDomain.CurrentDomain.BaseDirectory + @"rewards\";
        public static string RewardsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\rewards\";

        public static bool RewardSaved;

        public static string GetRewardName() {

            long t = Environment.TickCount - StartedTime;

            string st = STime.TickToStr( t ).Replace( ":", "." );

            string nt = DateTime.Now.ToString().Replace( ":", "." );

            return nt + " - " + st;
        }

        public static bool SaveReward() {

            try {

                if (!Directory.Exists( RewardsFolder ))
                    Directory.CreateDirectory( RewardsFolder );

                string filename = RewardsFolder + GetRewardName() + " " + HelpedOnStart.ToYesNoString() + ".jpg";

                SGraph.Scr( filename, PubgWindow.Width, PubgWindow.Height, PubgWindow.PosX, PubgWindow.PosY, true );

                Log.Add( "Reward saved " + filename );

                RewardSaved = true;

                return true;

            }
            catch {

                Log.Add( "Can't save reward... error" );

                return false;
            }
        }

        public static void Set() {

            if (IsLive) return;
          
            IsLive = true; 
            StartedTime = Environment.TickCount;                   
            RewardSaved = false;
            HelpedOnStart = false;
            PubgWindow.KillExecuted = false;
            Form1.PubgInput.EjectClickedTime = int.MaxValue;
            Form1.PubgInput.ParachuteClickedTime = int.MaxValue;
            Form1.PubgInput.DownClickedTime = int.MaxValue;
            Form1.Analytics.IncMatchLive();

            Log.Add("New Round Set.");
        }

        public static void End(bool savereward = false, string endreason = "") {         

            IsLive = false;
            EndedTime = Environment.TickCount;

            if (savereward) SaveReward();                    

            if (endreason != "") {

                EndReason = endreason;
                Log.Add( String.Format("Round End ({0})", endreason) );
            }
            
        }

    }

    partial class Form1 {

        //public PubgRound PubgRound = null;
    }
}
