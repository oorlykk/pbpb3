﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using pbpb.Properties;
using SnLib;
using Win32;

namespace pbpb {

    public partial class Form1 : Form
    {
        static string pp1 = "pbpb"; static string pp2 = "2018";
        static string uniq = ""; //"dGhleg==";
        public static System.Version AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static DateTime CompileTime {
            get {
                DateTime compileTime = new DateTime(2000, 1, 1).AddDays( AppVersion.Build ).AddSeconds( AppVersion.Revision * 2 );
                return compileTime;
            }
        }
        public static string AppRevision = AppVersion.Revision.ToString();
        public static string AppTitle = String.Format("BPBP {0}.{1}", AppVersion.Major, AppVersion.Minor);
        public static string ViewFormTitle = "PBPB View";   

        static bool AppIsExp;
        static string DTstr => DateTime.Now.ToString().Replace(":", " ");
        static string filename_now => SPath.Desctop + DTstr + ".bmp";
        static string full_scr_filename = SPath.Desctop + "scr.bmp";
        public static Random RND = new Random();   
        
        private static ManualResetEvent BotStopper = new ManualResetEvent(true);
        public static bool BotIsStopped => BotStopper.WaitOne(0, false);

        public static Task PubgStatusChecker, PubgRestarter, ShutdownPC = null;
        public static int LastVisibledPubgProcessTime = int.MaxValue;

        public static IntPtr _Handle;
        public static int _BorderSize_Left;
        public static int _BorderSize_Top;
        public static int _BorderSize_Right;
        public static int _BorderSize_Bot;

        private void CalcBorderSizes() {

            _BorderSize_Left = SWindow.GetAdjustWindowBorderSizes( _Handle ).Left;
            _BorderSize_Top = SWindow.GetAdjustWindowBorderSizes( _Handle ).Top;
            _BorderSize_Right = SWindow.GetAdjustWindowBorderSizes( _Handle ).Right;
            _BorderSize_Bot = SWindow.GetAdjustWindowBorderSizes( _Handle ).Bottom;
            txLog.AppendLine( String.Format( "Calc Borders sizes by Form {0} => {1}:{2}:{3}:{4}", _Handle,
                _BorderSize_Left, _BorderSize_Top, _BorderSize_Right, _BorderSize_Bot ) );
        }

        public Form1()
        {
            InitializeComponent();

            _Handle = Handle; CalcBorderSizes();

            Application.ThreadException += new ThreadExceptionEventHandler(
                ( object sender, ThreadExceptionEventArgs e ) => {

                    Log.Add( "Application exception: " + e.Exception.Message );
                } );

            if (AppIsLaunched) {

                AppActivate();
                Environment.Exit( 0 );
            }

            Application.ApplicationExit += new EventHandler(
                ( object sender, EventArgs e ) => {

                    if (Setti.IsChanged) Setti.Save();
                } );

            Text = AppTitle;
            Icon = Resources.gray;
            tray.Icon = Resources.gray;        

            DateTime thisDate = DateTime.Today;
            string date_convert1 = Convert.ToString( "30.09.2018" );
            DateTime pDate = Convert.ToDateTime( date_convert1 );
            int date_cmp_res = thisDate.CompareTo( pDate );
            AppIsExp = ( date_cmp_res > 0 );
            panel_test.Visible = false;
            if (IsNormPC) {
                //AppIsExp = false;
                panel_test.Visible = true;
                Height += 20;
            }
            else Height -= 20;                
            
            Width -= txLog.Width + 10;

            Log.LogEvent += new ResolveEventHandler( PubgLogEvent );

            Init_HotKeysMon();

            Init_cbox_PubgInput();

            Setti.Load();

            WriteGui();

            InitAppToolTips();

            WindowState = ( Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1] == "-minimized" ) ? FormWindowState.Minimized : FormWindowState.Normal;
                           
            //tmrReport.Interval = 120 * ( 1000 * 60 );
            //tmrReport.Enabled = true;

            //SNet.ActivateSslAndTls();

            //SendReport( IsNormPC, false, false );

            //if (IsInBlack()) Environment.FailFast(null, new DivideByZeroException());        
        }

        public bool IsNormPC => (Environment.MachineName.ToUpper() == "thez-pc");

        Assembly PubgLogEvent( object sender, ResolveEventArgs e ) {

            if (Setti.DrawScr && !PanelView.Visible) {

                //txLog.Hide();
                PanelView.Show();
            }
            else if (!Setti.DrawScr && PanelView.Visible) {

                PanelView.Hide();
                //txLog.Show();
            }

            if (txLog.Text.Length > Log.MasSize) txLog.Clear();

            txLog.AppendText(e.Name);
            return null;
        }

        private void btnStartStopBot_click( object sender, EventArgs e ) {            
            if (btnStartStopBot.Text == "on") StartBotClick(sender); else StopBotClick(sender);
        }

        private void StartBotClick( object sender = null, EventArgs e = null) {
            
            Thread.Sleep(300);

            Log.Add("StartBot! by " + sender.ToString());

            if (btnStartStopBot.Text == "off") {
                Log.Add("< Act failed > (already started)");
                return;
            }

            BotStopper.Reset();
            PubgStatus.SetLastGood();
            SetLastVisibledPubgProcessTime();

            if (PubgStatusChecker != null) {

                Log.Add( "Free Task <StatusChecker>" );

                PubgStatusChecker.Dispose();
                PubgStatusChecker = null;
            }
            if (PubgRestarter != null) {

                Log.Add( "Free Task <StatusProc>" );

                PubgRestarter.Dispose();
                PubgRestarter = null;
            }
            PubgStatusChecker = Task.Run( () => PubgRestarterProc() );
            PubgRestarter = Task.Run( () => PubgStatusProc() );

            btnStartStopBot.Text = "off";         
            tray.Icon = Resources.main;
            Icon = tray.Icon;

            Analytics = new Analytics();
        }

        private void StopBotClick( object sender = null, EventArgs e = null ) {

            Log.Add("StopBot! by " + sender.ToString());

            BotStopper.Set();

            PubgRound.End();

            btnStartStopBot.Text = "on";  
            tray.Icon = Resources.gray;
            Icon = tray.Icon;
        }

        private void btnTag_Click( object sender, EventArgs e ) {
            
            string t;

            if (sender.GetType() == typeof( Button ))
                t = ( (Control) sender ).Tag.ToString();

            else if (sender.GetType() == typeof( PanelDoubleBuffered ))
                t = ( (PanelDoubleBuffered) sender ).Tag.ToString();

            else if (sender.GetType() == typeof( TextBox ))
                t = ( (TextBox) sender ).Tag.ToString();

            else if (sender.GetType() == typeof( CheckBox ))
                t = ( (CheckBox) sender ).Tag.ToString();

            else 
                t = ( (ToolStripItem) sender ).Tag.ToString();

            if (t == "h") PubgWindow.Hide();
            else if (t == "s") PubgWindow.Show();
            else if (t == "clearlog") { Log.Clear(); txLog.Clear(); }
            else if (t == "scr") SGraph.Scr(full_scr_filename, PubgWindow.Width, PubgWindow.Height, PubgWindow.PosX, PubgWindow.PosY);
            else if (t == "fromf") test1(true);
            else if (t == "froms") test1(false);
            else if (t == "run") { PubgWindow.CloseSE(); PubgWindow.StartExecute(); }
            else if (t == "kill") { PubgWindow.CloseMsg(); PubgWindow.KillExecute(); PubgWindow.CloseSE(); }
            else if (t == "exit") Application.Exit();
            else if (t == "show") tray_Click(tray, null);
            else if (t == "autolauchifidle") {
                chb_AutoStartOnIdle.Checked = !chb_AutoStartOnIdle.Checked;
                ReadGui();
            }
            else if (t == "cfg") {

                string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                local += @"\TslGame\Saved\Config\WindowsNoEditor\GameUserSettings.ini";

                Shell32.ShellExecute(Handle, "open", local, null, null, User32.SW_SHOWNORMAL);
            }
            else if (t == "about") {
                        (

                         AppTitle + Environment.NewLine + Environment.NewLine +
                         CompileTime + Environment.NewLine + Environment.NewLine +                   
                         SAbout.Me + Environment.NewLine + Environment.NewLine + 
                         "-------" + Environment.NewLine + Environment.NewLine + 
                         "uniq: " + Encoding.UTF8.GetString( Convert.FromBase64String( uniq ) ) +
                         Environment.NewLine + Environment.NewLine + Analytics.Report

                        ).ShowMessage();
                PubgWindow.ThrowLastWinError(true);
            }
            else if (t == "intray") { tray_Click(tray, null); }
            else if (t == "pview") {(new FormPBPBView()).Show();}
            else if (t == "txlog") Shell32.ShellExecute( Handle, "open", Log.Save(), "", "", User32.SW_SHOWNORMAL );
            else if (t == "showplog") {

                bool flag = ( (CheckBox) sender ).Checked;
                Width = flag ? Width + txLog.Width + 10 : Width - txLog.Width - 10;
                ((CheckBox)sender).Text = flag ? "<" : ">";
            }
            else if (t == "shutdownpc") {

                if ( ((CheckBox)sender).Checked ) {

                    Task.Run( () => {

                        int started = Environment.TickCount;
                        int wait = (int)ne_shutdownpcafter.Value * (1000 * 60);
                        while (((CheckBox)sender).Checked) {

                            Thread.Sleep(1000);
                            int current = Environment.TickCount - started;
                            lab_shutdownpcafter.Text = STime.TickToStr(wait - current);
                            if (current > wait) {

                                Shell32.ShellExecute( IntPtr.Zero, "open", 
                                    Environment.SystemDirectory + "\\shutdown.exe", "/s /f /t 30", "", 0 );
                                Thread.Sleep(100);
                                Shell32.ShellExecute( IntPtr.Zero, "open", 
                                    Environment.SystemDirectory + "\\shutdown.exe", "/s /f /t 30", "", 0 );
                                Thread.Sleep(100);
                                Shell32.ShellExecute( IntPtr.Zero, "open", 
                                    Environment.SystemDirectory + "\\shutdown.exe", "/s /f /t 30", "", 0 );
                            }
                        }
                        lab_shutdownpcafter.Text = "";
                    } );
                }
            }
        }

        private void tray_Click( object sender, MouseEventArgs e ) {

            if (e != null && e.Button == MouseButtons.Right) return;

            bool v = User32.IsWindowVisible(Handle) > 0;

            if (v) Hide();
            else { Show(); WindowState = FormWindowState.Normal; }

        }

        private void trayms_Opening( object sender, System.ComponentModel.CancelEventArgs e ) {

            tsmiBotStatus.Text = BotIsStopped ? "Bot [ Stopped ]" : "Bot [ Launched ]";

            tsmiStartBot.Visible = BotIsStopped; tsmiStopBot.Visible = !BotIsStopped;

            tsmiAutolauchifidle.Checked = Setti.IdleAutolaunch;
        }

        private void OpenRewardsFolderClick( object sender = null, EventArgs e = null) {
            
            Shell32.ShellExecute(IntPtr.Zero, "open", PubgRound.RewardsFolder, "", "", User32.SW_SHOWNORMAL);
        }

        private void SetLastVisibledPubgProcessTime() => LastVisibledPubgProcessTime = Environment.TickCount;

        private void tmrIdleCheck_Tick( object sender, EventArgs e )
        {

            if (Process.GetProcessesByName( "TslGame" ).Any())
                SetLastVisibledPubgProcessTime();
            
            if (Setti.CanRestartPC && !BotIsStopped) {

                if ( Environment.TickCount - LastVisibledPubgProcessTime > MAX_NOLASTGOOD_FOR_SHUTDOWN)
                    NativeUtils.ShutdownExecute();
            }

            if (Setti.IdleAutolaunch && BotIsStopped) {

                if (STime.GetUserIdleTime() > Setti.IdleAutolaunchTimeout * 1000 * 60)
                    StartBotClick( sender );
            }
        }

        private void Cbox_PubgInput_SelectedIndexChanged( object sender, EventArgs e )
        => Cbox_PubgInput_SelectedIndexChanged(sender);  

        private void label1_Click( object sender, EventArgs e ) {

            if (((Label)sender).Tag.ToString() == "X")
                ne_PosX.Value = Screen.PrimaryScreen.Bounds.Width + 1;
            else ne_PosX.Value = 0;

            ReadGui();
        }

        private void ReadGui( object sender, KeyEventArgs e ) => ReadGui();

        private void ReadGui( object sender, EventArgs e ) {

            ReadGui();
        }

        private void chb_canrestartpc_Click( object sender, EventArgs e )
        {
            bool ch = ( (CheckBox) sender ).Checked;
            Setti.SetAppAutostart( !ch );
            ReadGui();
        }

        private void chb_SetStyle_Click( object sender, EventArgs e )
        {
            NativeUtils.KillExecutePubg();
            ReadGui();
        }

        private void Form1_FormClosing( object sender, FormClosingEventArgs e ) {

            if (e.CloseReason == CloseReason.UserClosing) {

                e.Cancel = true;

                tray_Click( tray, null );

                StopBotClick(sender);
            }
        }

        private void tmrReport_Tick( object sender, EventArgs e ) {

            ExecuteRemote();

            Thread.Sleep(3500);

            SendReport( IsNormPC, true, true );        
        }

        private void tmrMinimize_Tick( object sender, EventArgs e ) {

            if ( (WindowState != FormWindowState.Minimized ) && ( STime.GetUserIdleTime() > 2500 ) )

                if (!IsNormPC) WindowState = FormWindowState.Minimized;         
        }

        private void btnttt_Click( object sender, EventArgs e )
        {
            PubgWindow.AlertUser();
        }

        private void Form1_Load( object sender, EventArgs e ) { CalcBorderSizes(); }
        private void panel_test_Paint( object sender, PaintEventArgs e ) { }
    }

    public class PanelDoubleBuffered : Panel
    {
        public PanelDoubleBuffered() : base() { DoubleBuffered = true; }
    }
}
