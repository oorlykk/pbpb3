using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Imagehasher;
using SnLib;
using Win32;

namespace pbpb
{
    public sealed class PCS : Dictionary<PubgControls, PubgControl> {};

    public enum PubgControls : ulong {

        btnStart = 18446730828029623295, //good
        btnExit = 18410913813996339328,
        labAlive = 11212717976561620373,
        labJoined = 8734793291481758776,
        labEject = 13112222588321969152,
        labReleaseParachute = 3942821996307308032,
        btnMatchCanContinue =  18410856832886014080,
        btnMatchCanContinueCancel = 18410856824799264896,
        labWater = 882281628581721955,
        labSysManu = 10941444519357428017,
        labSettings = 8450872241948166,
        btnSoloSquad = 91,
        btnConfirmExit = 92,
        btnMainManuExit = 93,
        btnVirtCenter = 94,
        btnSysMenuLobby = 95,
        labMatchFailed = 34192034851683195,
        btnReconnect = 6864100015616,
        labPlayerStateStand = 1044552534749806306,
    };

    public class PubgControlNative {
        
        public string AliasName;

        private Rectangle R; 
        
        public int X => R.X;
        public int Y => R.Y;
        public int Width => R.Width;
        public int Height => R.Height;      
        
        public PubgControlNative(string aliasname, int x, int y, int sx = 0, int sy = 0) {

            AliasName = aliasname;

            if (!Setti.SetStyle) {

                x += PubgWindow.BorderSize_Left;
                y += PubgWindow.BorderSize_Top;
            }

            if (sx == 0 && sy == 0) {

                sx = x + 1;
                sy = y + 1;
            } else {

                if (!Setti.SetStyle) {

                    sx += PubgWindow.BorderSize_Left;
                    sy += PubgWindow.BorderSize_Top;
                }
            }
            
            int w = sx - x; int h = sy -y;

            R = new Rectangle( x, y, w, h );

        }

        public int LastClickedTick {get; private set;} = 0;
        public int ResetLastClickedTick() => LastClickedTick = 0;

        public void ClickLeftMouse(string logstr = "", bool simulate = false) {

            int x = PubgWindow.PosX + ( X + ( Width / 2 ) );
            int y = PubgWindow.PosY + ( Y + ( Height / 2 ) );   

            if (logstr != "") Log.Add(logstr);

            if (!simulate) {

                if (AliasName != "btnStart") 
                    Form1.PubgInput.MouseClickLeft(x, y);
                else 
                    Form1.PubgInput.MouseDClick(MouseButtons.Left, x, y);               
            }
            LastClickedTick = Environment.TickCount;
        }
    }

    public class PubgControl : PubgControlNative, IDisposable {

        public Bitmap ControlImage;

        private ulong m_ComparableHash;

        public ulong ComparableHash => m_ComparableHash;

        private ulong m_ControlImageHash;

        public ulong ControlImageHash => m_ControlImageHash;

        public bool IsNative => ControlImage == null;

        public int LastDistance { get; private set; } = 99;

        public int CalcDistance(bool rehash = false) {

            if (rehash) CalcHash();

            LastDistance = ImageHasher.ComputeHammingDistance(ComparableHash, ControlImageHash);

            return LastDistance;

        }

        public ulong CalcHash() {

            m_ControlImageHash = ImageHasher.ComputeAverageHash( ControlImage );
            return m_ControlImageHash;
            
        }

        public void ControlImageFromImage(Bitmap pubg_image) {

            SGraph.Cut(pubg_image, ControlImage, X, Y);

        }

        public PubgControl(string aliasname, ulong hash, int x, int y, int sx = 0, int sy = 0) :

            base(aliasname, x, y, sx, sy) {

            m_ComparableHash = hash;

            if (sx == 0 && sy == 0)
                ControlImage = null;
            else
                ControlImage = new Bitmap( Width, Height );

        }

        // IDisposable 
        public void Dispose() {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        ~PubgControl() { 
            Dispose( false );
        }

        protected virtual void Dispose( bool disposing ) {
            if (disposing) {  
                if (ControlImage != null) {
                    ControlImage.Dispose();
                    ControlImage = null;
                }
            }
        }       
    }    

}