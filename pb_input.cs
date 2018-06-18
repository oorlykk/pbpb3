using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32;
using SnLib;
using System.Windows.Forms;
using System.Threading;

namespace pbpb
{
    public class PubgInputAuto : InputBase
    {
        public override string ToString() => "Auto";

        public override void KeyDown( Keys key ) => new PubgInputMsg().KeyDown(key);
        
        public override void KeyUp( Keys key ) => new PubgInputMsg().KeyUp(key);
        
        public override void KeyPress( Keys key ) {

            if (key != Keys.LWin)
                new PubgInputMsg().KeyPress( key );
            else
                new PubgInputEvnt().KeyPress( key );
        }

        public override void MouseDown( MouseButtons button, int x, int y ) =>
            new PubgInputEvnt().MouseDown(button, x, y);

        public override void MouseUp( MouseButtons button, int x, int y ) =>
             new PubgInputEvnt().MouseUp(button, x, y);

        public override void MouseClick( MouseButtons button, int x, int y ) =>
             new PubgInputEvnt().MouseClick(button, x, y);

        public override void MouseDClick( MouseButtons button, int x, int y ) =>
             new PubgInputEvnt().MouseDClick( button, x, y );

        public override void MouseMove( int x, int y ) => new PubgInputEvnt().MouseMove(x, y);

        public override void MoveCam( bool toBot ) => new PubgInputEvnt().MoveCam( toBot );      
    }

    public class PubgInputVirtual : InputBase
    {
        public override string ToString() => "Virtual";
    }

    public class PubgInputMsg : InputBaseSend
    {
        public override string ToString() => "Msg";
    }

    public class PubgInputEvnt : InputBaseEvnt
    {
        public override string ToString() => "Evnt";
        
        public override void MoveCam(bool toBot)
        {
            int x = PubgWindow.PosX + ( PubgWindow.Width / 2 );
            int y = PubgWindow.PosY + ( PubgWindow.Height / 2 );

            if (_doinput(x, y, true)) {

                base.MouseDown( MouseButtons.Left, x, y );
                Thread.Sleep(64);
                base.MouseUp( MouseButtons.Left, x, y );
                for (int i = 1; i < 200; i++) {

                    base.MouseMove( 0, toBot? -10 : 10 );
                    Thread.Sleep( 1 );
                }
                new PubgInputMsg().Back();

                _afterinput();
            }            
        }

        public override void KeyDown( Keys key )
        {
            if (_doinput()) {

                base.KeyDown( key );

                _afterinput();
            }
        }

        public override void KeyUp( Keys key )
        {
            if (_doinput()) {

                base.KeyUp( key );

                _afterinput();
            }
        }

        public override void KeyPress( Keys key )
        {
            if (_doinput()) {

                base.KeyPress(key);

                _afterinput();
            }
        }

        public override void MouseDown( MouseButtons button, int x, int y )
        {
            if (_doinput(x, y, true)) {

                base.MouseDown(button, x, y);

                _afterinput();
            }
        }

        public override void MouseUp( MouseButtons button, int x, int y )
        {
            if (_doinput(x, y, true)) {

                base.MouseUp(button, x, y);

                _afterinput();
            }
        }

        public override void MouseClick( MouseButtons button, int x, int y )
        {
            if (_doinput(x, y, true)) {

                Thread.Sleep( 600 );
                base.MouseDown(button, x, y);
                Thread.Sleep( 1 );
                base.MouseUp(button, x, y);

                _afterinput();
            }
        }

        public override void MouseDClick( MouseButtons button, int x, int y )
        {
            if (_doinput( x, y, true )) {

                Thread.Sleep( 2000 );
                base.MouseDown( button, x, y );
                Thread.Sleep( 400 );
                base.MouseUp( button, x, y );
                //Thread.Sleep( User32.GetDoubleClickTime() );
                //base.MouseDown( button, x, y );
                //base.MouseUp( button, x, y );

                _afterinput();
            }
        }

        public override void MouseMove( int x, int y )
        {
            if (_doinput( x, y, true )) {

                base.MouseMove(x, y);
                _afterinput();
            }
        }

        bool _doinput(int x = -1, int y = -1, bool save = false)
        {          
            if (CanInteract) {
                // cur pos
                if (x == -1 && y == -1)
                    InputSetter.SaveCursorPos();
                else {

                    User32.GetCursorPos( out POINT cp );
                    if (cp.x != x && cp.y != y)
                        InputSetter.SetCursorPos( x, y, true );
                }     

                Thread.Sleep(1);

                // fw
                if (User32.ForegroundWindow != Handle) {                  
                    InputSetter.SetForegroundWindow( Handle, true );
                    Thread.Sleep( 100 );
                }
                else {
                    InputSetter.SaveForegroundWindow();
                }
                return true;
            }
            else { Log.Add( "Can't input (no idle)" ); return false; }
        }

        void _afterinput() {

            Thread.Sleep( 300 );

            if (User32.ForegroundWindow != InputSetter.SavedForegoundWindow)
                InputSetter.RestoreSavedForegroundWindow();

            User32.GetCursorPos( out POINT cp );
            if (cp.x != InputSetter.SavedCursorPos.x && cp.y != InputSetter.SavedCursorPos.y)
                InputSetter.RestoreSavedCursorPos();  
            
        }
    }


    // base input classes
    public class PubgInputKeysEventArgs {

        public PubgInputKeysEventArgs( Keys key ) { Key = key; }
        public Keys Key { get; private set; }
    }

    public class PubgInputMouseEventArgs {

        public PubgInputMouseEventArgs( MouseButtons button, int x, int y ) { Button = button; X = x; Y = y; }
        public MouseButtons Button { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
    }

    public class InputBase 
    {

        public virtual void MoveCam( bool toBot ) { }  

        public static bool CanInteract => (!Setti.PassiveMode) || (Setti.PassiveMode && STime.GetUserIdleTime() > 5000);

        //bool FocusDepended => ( GetType() == typeof(InputEvnt) || GetType() == typeof(InputSM2) );

        public bool Test(Type T) => GetType() == T;

        public IntPtr Handle { get => PubgWindow.Handle; }

        public Keys LastKey { get; set; }

        //keys
        public delegate void InputEventKeysHandler( PubgInputKeysEventArgs e );     


        public event InputEventKeysHandler InputEventKeyDown;
        public event InputEventKeysHandler InputEventKeyUp;
        public event InputEventKeysHandler InputEventKeyPress;


        public void RaiseInputEventKeysDown( Keys key ) {

            Log.Add(String.Format("{0} | Keydown {1}", this, key));
            InputEventKeyDown?.Invoke( new PubgInputKeysEventArgs( key ) );
        }

        public void RaiseInputEventKeysUp( Keys key ) {

            Log.Add(String.Format("{0} | Keyup {1}", this, key));
            InputEventKeyUp?.Invoke( new PubgInputKeysEventArgs( key ) );
        }

        public void RaiseInputEventKeysPress( Keys key ) {

            Log.Add(String.Format("{0} | Keypress {1}", this, key));
            InputEventKeyPress?.Invoke( new PubgInputKeysEventArgs( key ) );
        }



        //mouse
        public delegate void InputEventMouseHandler( PubgInputMouseEventArgs e );


        public event InputEventMouseHandler InputEventMouseDown;

        public event InputEventMouseHandler InputEventMouseUp;

        public event InputEventMouseHandler InputEventMouseClick;

        public event InputEventMouseHandler InputEventMouseMove;

        public void RaiseInputEventMouseDown( MouseButtons button, int x, int y  ) {

            Log.Add(String.Format("{3} | Mouse down {0} [{1} : {2}]", button, x, y, this));
            InputEventMouseDown?.Invoke( new PubgInputMouseEventArgs( button, x, y  ) );
        }

        public void RaiseInputEventMouseUp( MouseButtons button, int x, int y  ) {

            Log.Add(String.Format("{3} | Mouse up {0} [{1} : {2}]", button, x, y, this));
            InputEventMouseUp?.Invoke( new PubgInputMouseEventArgs( button,  x,  y  ) );
        }

        public void RaiseInputEventMouseClick( MouseButtons button, int x, int y  ) {

            Log.Add(String.Format("{3} | Mouse click {0} [{1} : {2}]", button, x, y, this));
            InputEventMouseClick?.Invoke( new PubgInputMouseEventArgs( button,  x,  y  ) );
        }

        public void RaiseInputEventMouseMove( int x, int y ) {

            Log.Add(String.Format("{2} | Mouse move {0} : {1}", x, y, this) );
            InputEventMouseMove?.Invoke( new PubgInputMouseEventArgs( 0,  x,  y  ) );
        }

        public virtual void KeyDown( Keys key ) {
            RaiseInputEventKeysDown(key);
        }
        public virtual void KeyUp( Keys key ) {
            RaiseInputEventKeysUp(key);
        }
        public virtual void KeyPress( Keys key ) {
            KeyDown(key);
            KeyUp(key);
        }

        public virtual void MouseDClick( MouseButtons button, int x, int y ) {
            MouseClick( button, x, y);
            MouseClick( button, x, y );
        }
        public virtual void MouseMove( int x, int y ) {
            RaiseInputEventMouseMove(x, y);
        }    
        public virtual void MouseDown( MouseButtons button, int x, int y ) {
            RaiseInputEventMouseDown(button, x, y);
        }
        public virtual void MouseUp( MouseButtons button, int x, int y ) {
            RaiseInputEventMouseUp(button, x, y);
        }
        public virtual void MouseClick( MouseButtons button, int x, int y ) {
            MouseDown(button, x, y);
            MouseUp(button, x, y);
        }       
        public virtual void MouseClickLeft( int x, int y ) {
            MouseClick(MouseButtons.Left, x, y);
        }
        public virtual void MouseClickRight( int x, int y ) {
            MouseClick(MouseButtons.Right, x, y);
        }
        public virtual void MouseClickMiddle( int x, int y ) {
            MouseClick( MouseButtons.Middle, x, y);
        }
        public virtual void ReleaseKey( Keys key ) { KeyUp(key); }

        public int EjectClickedTime {get; set;} = int.MaxValue;
        public void Eject() {

            EjectClickedTime = Environment.TickCount;
            KeyPress(Keys.F);
        }

        public int ParachuteClickedTime {get; set;} = int.MaxValue;
        public void Parachute() {

            ParachuteClickedTime = Environment.TickCount;
            KeyPress(Keys.F);
        }

        public int DownClickedTime {get; set;} = int.MaxValue;
        public void Down() {

            DownClickedTime = Environment.TickCount;
            KeyPress(Keys.Z);
        }

        public void Sit() {

            KeyPress(Keys.C);
        }

        public void Jump() {

            KeyPress(Keys.Space);
        }

        public void Forward() {

            KeyDown( Keys.W );
        }

        public void Back() {

            KeyDown( Keys.S );
        }

        public void ViewPerson() {

            KeyPress(Keys.V);
        }

    }

    public class InputBaseSend: InputBase
    {
        protected delegate int CallMethod( IntPtr hwnd, int wMsg, int wParam, int lParam );
        protected CallMethod Caller;

        public InputBaseSend()
        {

            Caller = delegate ( IntPtr hwnd, int wMsg, int wParam, int lParam ) {
                return User32.SendMessage( hwnd, wMsg, wParam, lParam );
            };
        }

        public override void KeyDown( Keys key )
        {
            Caller( Handle, User32.WM_KEYDOWN, (int) key, 0 );
            LastKey = key;
            RaiseInputEventKeysDown( key );
        }

        public override void KeyUp( Keys key )
        {
            Caller( Handle, User32.WM_KEYUP, (int) key, 0 );
            LastKey = key;
            RaiseInputEventKeysUp( key );
        }

        public override void KeyPress( Keys key )
        {            
            KeyDown( key );
            KeyUp( key );
        }

        void NativeMouseDownUp( MouseButtons button, int x, int y, bool isDown ) {

            int lp = (int) ( ( (ushort) x ) | (uint) ( y << 16 ) );
            int msg = 0;
            int wp = 0;

            switch (button) {

            case MouseButtons.Left:
                wp = User32.MK_LBUTTON;
                msg = isDown ? User32.WM_LBUTTONDOWN : User32.WM_LBUTTONUP;
                break;

            case MouseButtons.Right:
                wp = User32.MK_RBUTTON;
                msg = isDown ? User32.WM_RBUTTONDOWN : User32.WM_RBUTTONUP;
                break;
            }

            Caller( Handle, msg, wp, lp );

        }

        public override void MouseDown( MouseButtons button, int x, int y ) {

            NativeMouseDownUp(button, x, y, true);
            RaiseInputEventMouseDown(button, x, y);
        }

        public override void MouseUp( MouseButtons button, int x, int y ) {

            NativeMouseDownUp(button, x, y, false);
            RaiseInputEventMouseUp(button, x, y);
        } 
        
        public override void MouseClick( MouseButtons button, int x, int y ) {

            MouseDown( button, x, y );
            MouseUp( button, x, y );
        }

        public override void MouseClickLeft( int x, int y ) {

            MouseClick(MouseButtons.Left, x, y);
        }

        public override void MouseClickRight( int x, int y ) {

            MouseClick(MouseButtons.Right, x, y);
        }

        public override void MouseClickMiddle( int x, int y ) {

            MouseClick(MouseButtons.Middle, x, y);
        }

        public override void MouseMove( int x, int y ) {

            int lp = (int) ( ( (ushort) x ) | (uint) ( y << 16 ) );
            Caller( Handle, User32.WM_SYSCOMMAND, User32.SC_MOUSEMOVE, lp );

            RaiseInputEventMouseMove(x, y);
        }
    }

    public class InputBasePost : InputBaseSend
    {
        public InputBasePost()
        {
            Caller = delegate ( IntPtr hwnd, int wMsg, int wParam, int lParam ) {
                return User32.PostMessage( hwnd, wMsg, wParam, lParam );
            };
        }
    }

    public class InputBaseEvnt : InputBase
    {              
        void NativeMouseDownUp( MouseButtons button, int x, int y, bool isDown ) {

            int uMsg = 0;

            switch (button) {

                case MouseButtons.Left:
                    uMsg = isDown ? User32.MOUSEEVENTF_LEFTDOWN : User32.MOUSEEVENTF_LEFTUP;
                    break;

                case MouseButtons.Right:
                    uMsg = isDown ? User32.MOUSEEVENTF_RIGHTDOWN : User32.MOUSEEVENTF_RIGHTUP;
                    break;

                case MouseButtons.Middle:
                    uMsg = isDown ? User32.MOUSEEVENTF_MIDDLEDOWN : User32.MOUSEEVENTF_MIDDLEUP;
                    break;
            }

            User32.mouse_event( uMsg, 0, 0, 0, 0 );
        }

        public override void MouseDown( MouseButtons button, int x, int y ) {

            NativeMouseDownUp(button, x, y, true);
            RaiseInputEventMouseDown(button, x, y);
        }

        public override void MouseUp( MouseButtons button, int x, int y ) {

            NativeMouseDownUp(button, x, y, false);
            RaiseInputEventMouseUp(button, x, y);
        } 
        
        public override void MouseClick( MouseButtons button, int x, int y ) {

            MouseDown( button, x, y );
            Thread.Sleep(64);
            MouseUp( button, x, y );
        }

        void NativeMouseMove( int x, int y, bool absolute ) {

            int flag = User32.MOUSEEVENTF_MOVE;

            if (absolute) {

                var b = Screen.PrimaryScreen.Bounds;
                x = x * 65535 / b.Width;
                y = y * 65535 / b.Height;
                flag |= User32.MOUSEEVENTF_ABSOLUTE;
            }

            User32.mouse_event(flag, x, y, 0, 0);
        }

        public override void MouseMove( int x, int y ) {

            NativeMouseMove(x, y, false);
            RaiseInputEventMouseMove(x, y);
        }

        public void MouseMoveAbsolute( int x, int y ) {

            NativeMouseMove( x, y, true );
            RaiseInputEventMouseMove( x, y );
        }

        void NativeKeyDownUp( Keys key, bool isDown ) {   

            if (isDown) User32.keybd_event(key, 0x45, User32.KEYEVENTF_EXTENDEDKEY | 0, 0);
            else User32.keybd_event(key, 0x45, User32.KEYEVENTF_EXTENDEDKEY | User32.KEYEVENTF_KEYUP, 0);      
        }

        public override void KeyDown( Keys key )
        {
            NativeKeyDownUp(key, true);
            RaiseInputEventKeysDown( key );
        }

        public override void KeyUp( Keys key )
        {
            NativeKeyDownUp( key, false);
            RaiseInputEventKeysUp( key );
        }

        public override void KeyPress( Keys key )
        {
            KeyDown( key );
            Thread.Sleep(64);
            KeyUp( key );
        }
    }

    //
    public class InputSetter {

        public static POINT SavedCursorPos;
        public static void SaveCursorPos() {

            User32.GetCursorPos(out SavedCursorPos);
        }

        public static void SetCursorPos(int x, int y, bool save) {

            if (save) {
                SaveCursorPos();
                Thread.Sleep( 10 );
            }
            User32.SetCursorPos( x, y );
            Thread.Sleep(10);
            Log.Add("Setter: Cursor => " + x.ToString() + ":" + y.ToString());
        }

        public static void RestoreSavedCursorPos() {

            SetCursorPos(SavedCursorPos.x, SavedCursorPos.y, false);
        } 

        public static IntPtr SavedForegoundWindow;
        public static void SaveForegroundWindow() {

            SavedForegoundWindow = User32.ForegroundWindow;
        } 

        public static void SetForegroundWindow(IntPtr handle, bool save) {

            if (save) {
                SaveForegroundWindow();
                Thread.Sleep(10);
            }
            User32.SetActiveWindow(handle);
            User32.SetForegroundWindow(handle);            
            Thread.Sleep(10);
            Log.Add("Setter: Foreground => " + handle.ToString() + " " + SWindow.GetWindowText(handle));
        }

        public static void RestoreSavedForegroundWindow() {

            SetForegroundWindow(SavedForegoundWindow, false);
        }
    }
    //

    partial class Form1 : Form {

        public void Init_cbox_PubgInput() {

            cbox_PubgInput.Items.Add(new PubgInputAuto().ToString() );
            cbox_PubgInput.Items.Add(new PubgInputEvnt().ToString() );
            cbox_PubgInput.Items.Add(new PubgInputMsg().ToString() );
            cbox_PubgInput.Items.Add(new PubgInputVirtual().ToString() );

        }

        private void Cbox_PubgInput_SelectedIndexChanged( object sender ) {

            switch (((ComboBox)sender).SelectedIndex) {
                    case 0: PubgInput = new PubgInputAuto(); break;
                    case 1: PubgInput = new PubgInputEvnt(); break;
                    case 2: PubgInput = new PubgInputMsg(); break;
                    case 3: PubgInput = new PubgInputVirtual(); break;
            }
            ReadGui();
        }

        public static InputBase PubgInput;
    }

}
