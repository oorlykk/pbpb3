using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SnLib;
using Win32;

namespace pbpb
{

    [Flags]
    public enum PubgStatuses {
        [Description("Non")]
        None = 0x0,
        [Description("Неразборчивый")]
        Unknown = 0x1,
        [Description("Лобби")]
        Lobby = 0x2,
        [Description("Подготовка")]
        Prepare = 0x4,
        [Description("Выпрыгнуть")]
        Eject = 0x8,
        [Description("Открыть парашут")]
        Parachute = 0x10,
        [Description("Живой")]
        Alive = 0x20,
        [Description("Выход в Лобби")]
        ExitToLobby = 0x40,
        [Description("Продолжить Матч")]
        MatchCanContinue = 0x80,
        [Description("Системное Меню Выход")]
        MainManuExit = 0x200,
        [Description("Настройки")]
        Settings = 0x400,
        [Description( "fail" )]
        Fail = 0x800,
        [Description( "Reconnect" )]
        Reconnect = 0x1000,
        [Description( "Standup" )]
        PlayerStandup = 0x2000,
    }

    public static class PubgStatus {

        public static int LastGoodTime { get; private set; }

        public static void SetLastGood() => LastGoodTime = Environment.TickCount;

        public static void ResetLastGood() => LastGoodTime = 0;

        
        public static int LastDistance { get; private set; }

        
        public static Bitmap RawScr;

        
        public static PubgStatuses Status { get; private set; }

        public static PubgStatuses Now( PCS Pcs ) {

            PubgStatuses result = PubgStatuses.None;

            if (RawScr != null) { RawScr.Dispose(); RawScr = null; }

            RawScr = SGraph.Scr( "", PubgWindow.Width, PubgWindow.Height, PubgWindow.PosX, PubgWindow.PosY );
             
            if (RawScr == null) goto EXIT;
            
            foreach (var key in Pcs) {

                PubgControl pc = key.Value;
                PubgControls pcname = key.Key;

                if (pc.IsNative) continue;

                pc.ControlImageFromImage( RawScr );          

                LastDistance = pc.CalcDistance( true );

                if (LastDistance < 5) {

                    if (pcname == PubgControls.btnStart) 
                        result |= PubgStatuses.Lobby;

                    else if (pcname == PubgControls.btnExit) 
                        result |= PubgStatuses.ExitToLobby;

                    else if (pcname == PubgControls.labJoined) 
                        result |= PubgStatuses.Prepare;

                    else if (pcname == PubgControls.labEject) 
                        result |= PubgStatuses.Eject;

                    else if (pcname == PubgControls.labReleaseParachute) 
                        result |= PubgStatuses.Parachute;

                    else if (pcname == PubgControls.labAlive) 
                        result |= PubgStatuses.Alive;

                    else if (pcname == PubgControls.btnMatchCanContinue) 
                        result |= PubgStatuses.MatchCanContinue;

                    else if (pcname == PubgControls.labSysManu)
                        result |= PubgStatuses.MainManuExit;

                    else if (pcname == PubgControls.labSettings)
                        result |= PubgStatuses.Settings;

                    else if (pcname == PubgControls.labMatchFailed)
                        result |= PubgStatuses.Fail;

                    else if (pcname == PubgControls.btnReconnect)
                        result |= PubgStatuses.Reconnect;

                    else if (pcname == PubgControls.labPlayerStateStand)
                        result |= PubgStatuses.PlayerStandup;
                };
            }

            if (result.HasFlags( PubgStatuses.Prepare ) ||
                result.HasFlags( PubgStatuses.Alive ) ||
                result.HasFlags( PubgStatuses.Eject ) ||
                result.HasFlags( PubgStatuses.Parachute ) ||
                result.HasFlags( PubgStatuses.ExitToLobby ) ||
                result.HasFlags( PubgStatuses.PlayerStandup )
                ) SetLastGood();

            User32.SendMessage(User32.FindWindow(0, Form1.AppTitle), Form1.WM_SCRUPDATE, 0, 0);
            User32.SendMessage(User32.FindWindow(0, Form1.ViewFormTitle), Form1.WM_SCRUPDATE, 0, 0);
            
            Application.DoEvents();          

            EXIT:

            return Status = result; 

        }

    }
}
