using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using SnLib;

namespace pbpb
{

    public sealed class Setti {
        private const string appname = "PBPB";
        //
        public static bool IsChanged = false;
        //
        public static bool HiddenMode = false;
        public static bool PassiveMode = false;
        public static bool SaveReward = true;
        public static bool IdleAutolaunch = true;
        public static int IdleAutolaunchTimeout = 5;
        public static int PubgWindowAbsoluteY = 200;
        public static bool CanRestartSteam = true;
        public static bool CanRestartPC = false;
        public static bool DrawScr = true;
        public static int PubgWindowAbsoluteX = 0;      
        public static bool SetStyle = true;
        public static bool FixMissStates = true;
        public static byte Input = 0;
        public static bool UseEject = true;
        public static bool HidePubgFromTaskBar = false;
        public static decimal MaxRoundTime = 4.1m * (1000*60);
        //no saved
        public static int MaxRoundTimeRnd =>  (int)MaxRoundTime +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 ) +
                                    Form1.RND.Next( 1000 );

        public static bool IsShortRoundTime => MaxRoundTime < 5 * ( 1000 * 60 );

        public static void Save()  {

            IsChanged = false;

            IFormatter fo = new BinaryFormatter();

            MemoryStream ms = new MemoryStream();

            object o = SSerialize.StaticClassSave(typeof(Setti));

            fo.Serialize(ms, o);

            var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey( appname );

            key.SetValue( Form1.AppTitle, ms.ToArray() );

            key.Close();

            ms.Close();

        }

        public static bool Load()
        {   
            
            bool result = false;

            try {

                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey( appname );

                byte[] data = (byte[]) key.GetValue( Form1.AppTitle );

                key.Close();

                MemoryStream ms = new MemoryStream();

                ms.Write( data, 0, data.Length );

                ms.Position = 0;

                IFormatter fo = new BinaryFormatter();

                object[,] o = fo.Deserialize( ms ) as object[,];

                ms.Close();

                SSerialize.StaticClassSaveLoad( typeof( Setti ), o );

                result = true;

                Log.Add("Settings loaded.");

            } catch {

                Log.Add("Default settings. " + PubgWindowAbsoluteX.ToString());                
            }

            return result;

        }

        public static bool SetAppAutostart( bool delete = false )
        {
            string appapth = Application.ExecutablePath;

            try {

                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey( "Software\\Microsoft\\Windows\\CurrentVersion\\Run\\" );

                if (!delete)
                    key.SetValue( appname, appapth + " -minimized" );
                else
                    key.DeleteValue( appname );

                key.Close();
            } catch {

                return false;
            }

            return true;
        }

        public static bool AppHasAutostart()
        {
            
            string appapth = Application.ExecutablePath;
            string value;

            try {

                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey( "Software\\Microsoft\\Windows\\CurrentVersion\\Run\\" );

                value = (string) key.GetValue( appname );

                key.Close();

                if (value == null) return false;

                if (value.LastIndexOf( @" -" ) > 0)
                    value = value.Remove(value.LastIndexOf( " -" ), 
                                         value.Length - value.LastIndexOf( " -" ) );

            } catch {

                return false;
            }
            
            return (appapth.ToLower() == value.ToLower());
        }

    }

    partial class Form1
    {

        public void ReadGui() {
            //
            Setti.IsChanged = true;
            //
            Setti.HiddenMode                    =      chb_HiddenMode.Checked;
            Setti.PassiveMode                   =      chb_PassiveMode.Checked;
            Setti.SaveReward                    =      chb_SaveReward.Checked;
            Setti.IdleAutolaunch                =      chb_AutoStartOnIdle.Checked;
            Setti.CanRestartSteam               =      chb_CanKillSteam.Checked;
            Setti.CanRestartPC                  =      chb_AllowRestartPC.Checked;          
            Setti.DrawScr                       =      chb_view.Checked;
            Setti.SetStyle                      =      chb_SetStyle.Checked;
            Setti.FixMissStates                 =      chb_FixMissStates.Checked;
            Setti.UseEject                      =      chb_UseEject.Checked;
            Setti.HidePubgFromTaskBar           =      chb_HidePubgFromTaskBar.Checked;

            Setti.IdleAutolaunchTimeout         =      (int)ne_MaxIdle.Value;
            Setti.PubgWindowAbsoluteX           =      (int)ne_PosX.Value;
            Setti.PubgWindowAbsoluteY           =      (int)ne_PosY.Value;
            Setti.MaxRoundTime                  =      ne_MaxRoundTIme.Value * (1000*60);
            Setti.Input                         =      (byte)cbox_PubgInput.SelectedIndex;
        }

        public void WriteGui( object sender = null, EventArgs e = null ) {

            chb_HiddenMode.Checked          =      Setti.HiddenMode;
            chb_PassiveMode.Checked         =      Setti.PassiveMode;
            chb_SaveReward.Checked          =      Setti.SaveReward;
            chb_AutoStartOnIdle.Checked     =      Setti.IdleAutolaunch;
            chb_CanKillSteam.Checked        =      Setti.CanRestartSteam;
            chb_AllowRestartPC.Checked      =      Setti.CanRestartPC && Setti.AppHasAutostart();
            chb_view.Checked                =      Setti.DrawScr;
            chb_SetStyle.Checked            =      Setti.SetStyle;
            chb_FixMissStates.Checked       =      Setti.FixMissStates;
            chb_UseEject.Checked            =      Setti.UseEject;
            chb_HidePubgFromTaskBar.Checked =      Setti.HidePubgFromTaskBar;

            ne_MaxIdle.Value                =      Setti.IdleAutolaunchTimeout;
            ne_PosX.Value                   =      Setti.PubgWindowAbsoluteX;
            ne_PosY.Value                   =      Setti.PubgWindowAbsoluteY;
            ne_MaxRoundTIme.Value           =      Setti.MaxRoundTime / (1000*60);
            cbox_PubgInput.SelectedIndex    =      Setti.Input;     
        }
    }
}
