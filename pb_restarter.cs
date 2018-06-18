using System;
using System.Threading;
using SnLib;

namespace pbpb
{
    public class ___qeqweqweq {}

    partial class Form1
    {  

        void PubgRestarterProc() {

            Log.Add("(MAIN) RestarterProc enter!");

            do {

                try {

                    if (!InputBase.CanInteract) {

                        Log.Add( "(PR) No idle time for actions. Wait..." );

                        continue;
                    }

                    if (!PubgWindow.Exists) {

                        if (PubgWindow.SUExists) {

                            Log.Add( "(PR) wait steam updating..." );

                            continue;
                        }

                        PubgWindow.StartExecute();

                        Thread.Sleep( 90000 );
                    }

                    if (
                        ( ( !Setti.PassiveMode ) && ( Environment.TickCount - PubgStatus.LastGoodTime > MAX_NOLASTGOOD ) )
                        ||
                        ( ( Setti.PassiveMode ) && ( Environment.TickCount - PubgStatus.LastGoodTime > MAX_NOLASTGOOD_FOR_PASSIVEMODE ) )
                       ) {

                        bool executed = PubgWindow.KillExecuted;
                        PubgWindow.KillExecute();                       
                        if (Setti.CanRestartSteam && executed)   
                            PubgWindow.KillExecuteSteam();                    
                        Thread.Sleep( 15000 );

                        PubgStatus.SetLastGood();
                    }


                    if (PubgWindow.SEExists || PubgWindow.CrashExists || PubgWindow.SURExists ||
                        PubgWindow.SCEExitst || NativeWindows.SteamGameOverlayUICrash.Exist ||
                        NativeWindows.SteamClientBootstrapper.Exist ) {

                        string s = String.Format(
                            "(PR) error found ( crash {0}, steam {1}, update {2}, conerror {3}, " +
                            "overlayerror {4}, bootstrappererror {5} )",
                            PubgWindow.CrashExists.ToYesNoString(),
                            PubgWindow.SEExists.ToYesNoString(),
                            PubgWindow.SURExists.ToYesNoString(),
                            PubgWindow.SCEExitst.ToYesNoString(),
                            NativeWindows.SteamGameOverlayUICrash.Exist.ToYesNoString(),
                            NativeWindows.SteamClientBootstrapper.Exist.ToYesNoString());

                        Log.Add( s );

                        PubgWindow.CloseSUR();

                        PubgWindow.CloseSE();

                        PubgWindow.CloseSCE();                  

                        PubgWindow.KillCrash();

                        //PubgWindow.KillExecute();

                        if (NativeWindows.SteamGameOverlayUICrash.Exist)
                            NativeWindows.SteamGameOverlayUICrash.SetClose();

                        if (NativeWindows.SteamClientBootstrapper.Exist)
                            NativeWindows.SteamClientBootstrapper.SetClose();

                    }

                }
                catch (Exception e) {

                    Log.Add( "(PR) exception: " + e.Message );
                }
                finally {

                    Thread.Sleep(1000);
                }
            } while (!BotStopper.WaitOne(30000, false));

            Log.Add("(MAIN) RestarterProc leave!");
        }
    }
}