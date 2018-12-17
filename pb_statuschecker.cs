using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SnLib;
using Win32;

namespace pbpb
{
    public static class ____qwejqjiqojwe { }

    partial class Form1
    {
        public const int OneMin = 60000;
        public const int HalfMin = OneMin / 2;

        public const int MAX_NOLASTGOOD = 6 * OneMin;
        public const int MAX_NOLASTGOOD_FOR_INPUT = HalfMin + 10000;
        public const int MAX_NOLASTGOOD_FOR_PASSIVEMODE = 30 * OneMin;
        public const int MAX_NOLASTGOOD_FOR_SHUTDOWN = 40 * OneMin;

        void PubgStatusProc() {        
            
            int threadwait = 2500;          
            PubgStatuses ps = PubgStatuses.None;
            bool pubg_window_visibled = false;
            bool viewclick = false;

            Log.Add("(MAIN) StatusProc enter!");

            while (!BotStopper.WaitOne( threadwait, false )) {

                try {

                    threadwait = ( User32.IsWindowVisible( User32.FindWindow(0, ViewFormTitle ) ) > 0 ) ?
                        100 : 100;

                    if (!PubgWindow.Exists) {

                        if (Pcs != null) {

                            Log.Add( "(PS) Collection of game controlls nulled." );
                            Pcs = null;
                        }

                        Log.Add( "(PS) No way :( Wait..." );
                        continue;
                    } else {

                        if (Pcs == null) {

                            Init_Pcs();
                            Log.Add( "(PS) Collection of game controlls created." );
                        }
                    }

                    //PubgWindow.CheckBEye(false);

                    pubg_window_visibled = PubgWindow.IsWindowVisible;
                    if (!pubg_window_visibled)
                        PubgWindow.Show();
                    

                    if (Setti.HiddenMode)
                        Thread.Sleep( 600 );

                    if (PubgWindow.NeedSetupWindow) {

                        bool res = PubgWindow.SetupWindow();

                        Log.Add( String.Format( "(PS) SetupWindow {0}; scalepart {1} ; result: {2}", 
                            PubgWindow.Handle, PubgWindow.ScalePart, res.ToYesNoString() ) ); 
                        
                    }

                    ps = PubgStatus.Now( Pcs );

                    Log.Add( String.Format( "S: {0}", ps.GetDescription() ) );

                    if (AppIsExp) goto EXIT;  //Magic EXP!

                    if (ps == PubgStatuses.None) { //Unknown

                        Log.Append( " di: " + PubgStatus.LastDistance.ToString() );

                        if (Environment.TickCount - PubgStatus.LastGoodTime > MAX_NOLASTGOOD_FOR_INPUT) {

                            if (Environment.TickCount - Pcs[PubgControls.btnVirtCenter].LastClickedTick > HalfMin
                                && 
                                Environment.TickCount - Pcs[PubgControls.btnStart].LastClickedTick > HalfMin) {

                                Pcs[PubgControls.btnVirtCenter].ClickLeftMouse( "+help input (long lg)" );
                                PubgInput.ViewPerson();
                            }
                        }
                    }

                    else if (ps.HasFlags( PubgStatuses.Reconnect )) {

                        Log.Append( " di: " + Pcs[PubgControls.btnReconnect].LastDistance.ToString() );

                        Pcs[PubgControls.btnReconnect].ClickLeftMouse( "click Reconnect" );
                    }

                    else if (ps.HasFlags( PubgStatuses.Fail )) {

                        Log.Append( " di: " + Pcs[PubgControls.labMatchFailed].LastDistance.ToString() );

                        PubgControl c = Pcs[PubgControls.labMatchFailed];
                        int lc = c.LastClickedTick;

                        if (lc == 0)
                            c.ClickLeftMouse( "fail click", true );
                        else if (Environment.TickCount - lc > 45000) {
                            Log.Add("restart so failed");
                            c.ResetLastClickedTick();
                            PubgStatus.ResetLastGood();
                        }                      
                    }

                    else if (ps.HasFlags(PubgStatuses.MatchCanContinue)) {

                        Log.Append( " di: " + Pcs[PubgControls.btnMatchCanContinue].LastDistance.ToString() );      
                        //if (!Setti.IsShortRoundTime &&
                        //    Environment.TickCount - Pcs[PubgControls.btnMatchCanContinue].LastClickedTick > 45000) 

                        //    Pcs[PubgControls.btnMatchCanContinue].ClickLeftMouse( "click Continue" );             
                        //else

                            Pcs[PubgControls.btnMatchCanContinueCancel].ClickLeftMouse("click Cancel");     
                    }

                    else if (ps.HasFlags( PubgStatuses.Lobby)) {

                        Log.Append( " di: " + Pcs[PubgControls.btnStart].LastDistance.ToString() );

                        PubgRound.End( !PubgRound.RewardSaved && Setti.SaveReward, "" );

                        if (Environment.TickCount - Pcs[PubgControls.btnStart].LastClickedTick > 15000) 

                            Pcs[PubgControls.btnStart].ClickLeftMouse("click Start");
                        
                    }

                    else if (ps.HasFlags( PubgStatuses.Settings)) {

                        Log.Append( " di: " + Pcs[PubgControls.labSettings].LastDistance.ToString() );
                        if (Setti.FixMissStates) PubgInput.KeyPress( Keys.Escape );

                    }

                    else if (ps.HasFlags(PubgStatuses.ExitToLobby)) {

                        Log.Append( " di: " + Pcs[PubgControls.btnExit].LastDistance.ToString() );

                        bool needsavereward = !PubgRound.RewardSaved && Setti.SaveReward;
                        if (needsavereward) {

                            Thread.Sleep( 6000 );
                            PubgRound.End( needsavereward, "ExitToLobby" );
                        }

                        Log.Add( "click ExitToLobby (ESC)" );
                        PubgInput.KeyPress( Keys.Escape );                     
                        Thread.Sleep( 1500 );
                        Pcs[PubgControls.btnConfirmExit].ClickLeftMouse("click ConfirmExit");            

                    }

                    else if (ps.HasFlags( PubgStatuses.MainManuExit)) {

                        Log.Append( " di: " + Pcs[PubgControls.labSysManu].LastDistance.ToString() );
                        
                        if ( Environment.TickCount - PubgRound.StartedTime > Setti.MaxRoundTimeRnd )    
                            Pcs[PubgControls.btnConfirmExit].ClickLeftMouse( "click ConfirmExit" );
                        
                    }
                    
                    //else if (ps.HasFlags( PubgStatuses.Eject)) {

                    //    Log.Append( " di: " + Pcs[PubgControls.labEject].LastDistance.ToString() );

                    //    if (Setti.UseEject) {

                    //        Thread.Sleep( RND.Next( 5, 8 ) * 1000 );

                    //        PubgInput.Eject();
                    //        PubgInput.Forward();
                    //    }
                    //}

                    //else if (ps.HasFlags( PubgStatuses.Parachute)) {

                    //    Log.Append( " di: " + Pcs[PubgControls.labReleaseParachute].LastDistance.ToString() );

                    //    PubgInput.Parachute();
                    //    PubgInput.Forward();
                    //}

                    else if (ps.HasFlags( PubgStatuses.Prepare)) {

                        Log.Append( " di: " + Pcs[PubgControls.labJoined].LastDistance.ToString() );
                    }

                    else if (ps.HasFlags( PubgStatuses.Alive)) {

                        Log.Append( " di: " + Pcs[PubgControls.labAlive].LastDistance.ToString() );

                        PubgRound.Set();

                        //if (!PubgRound.HelpedOnStart) {

                        //    PubgInput.ReleaseKey( Keys.S );
                        //    Thread.Sleep(100);
                        //    PubgRound.HelpedOnStart = true;
                        //    PubgInput.Back();
                        //    PubgInput.MoveCam( true );
                        //}

                        //viewclick = !viewclick;
                        //if (viewclick) PubgInput.ViewPerson(); //circle update view

                        //if (ps.HasFlags( PubgStatuses.PlayerStandup )) {

                        //    Log.Add("Player Down (so Stand)");
                        //    PubgInput.Down();
                        //    PubgInput.Forward();
                        //}

                        if (Environment.TickCount - PubgRound.StartedTime > 1000 * 40 &&
                            Environment.TickCount - PubgRound.StartedTime < 1000 * 45)                      
                            if (Setti.UseEject)
                            {

                                PubgInput.Eject();
                            }
                            

                        if (
                            (Environment.TickCount - PubgRound.StartedTime > OneMin + 5000) &&
                            (Environment.TickCount - PubgRound.StartedTime < OneMin + 15000)
                           ) {

                            int bX = Setti.PubgWindowAbsoluteX;
                            int bY = Setti.PubgWindowAbsoluteY;

                            Setti.PubgWindowAbsoluteX = 0;
                            Setti.PubgWindowAbsoluteY = 0;

                            PubgWindow.SetupWindow();
                            PubgWindow.Window.SetForegorund();
                            PubgWindow.AlertUser();
                            PubgRound.ManualWait = true;

                            //int t = Environment.TickCount;                           
                            while (PubgRound.ManualWait) Thread.Sleep(10);

                            Setti.PubgWindowAbsoluteX = bX;
                            Setti.PubgWindowAbsoluteY = bY;
                            
                        }

                        //End match on MaxRoundTime
                        if (Environment.TickCount - PubgRound.StartedTime > Setti.MaxRoundTimeRnd)
                        {

                            Log.Add("MaxRoundTime trig! try exit =>");
                            PubgInput.KeyPress(Keys.Escape);

                            Thread.Sleep(1500);
                            Pcs[PubgControls.btnConfirmExit].ClickLeftMouse("click ConfirmExit (@==@)");

                            Thread.Sleep(1500);
                            Pcs[PubgControls.btnConfirmExit].ClickLeftMouse("click ConfirmExit");

                            Log.Add(".end");
                        }
                    }

                    EXIT:;
                    if ( pubg_window_visibled && Setti.HiddenMode ) PubgWindow.Hide();
                    if (PubgWindow.IsFocused) {
                        Thread.Sleep(500);
                        User32.SetForegroundWindow( (IntPtr) User32.GetDesktopWindow() );
                    }
                }
                catch (Exception e) {

                    Log.Add( "(PS) exception: " + e.Message );
                }
                finally { 

                    Thread.Sleep(3000);
                }
            }

            Log.Add("(MAIN) StatusProc leave!");
        }
    }
}