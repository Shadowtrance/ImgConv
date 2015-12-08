using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImgConv
{
    /// <summary>
    /// Child form three class.
    /// Big images.
    /// </summary>
    public partial class BigImgs : Form
    {
        /// <summary>
        /// Global variables for screen width and screen height.
        /// bool to check if we're panning or not.
        /// start point for our mouse pointer.
        /// </summary>
        int screenWidth;
        int screenHeight;

        bool _isPanning = false;
        Point startPt;

        /// <summary>
        /// Form initialization.
        /// </summary>
        public BigImgs()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Big images picturebox.
        /// Dimensions any width x any height that are larger than the combined and separate image pictureboxes.
        /// </summary>
        public PictureBox BigPicBox
        {
            get { return pictureBox1; }
        }

        /// <summary>
        /// grabs the resolution of the monitor when this form loads.
        /// sets the size of the window of Picture viewer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form4_Load(object sender, EventArgs e)
        {
            Screen screen = Screen.PrimaryScreen;
            screenWidth = screen.Bounds.Width;
            screenHeight = screen.Bounds.Height;

            ClientSize = new Size(screenWidth, screenHeight);

            pictureBox1.Size = new Size(screenWidth, screenHeight);

            pictureBox1.Location = new Point((ClientSize.Width / 2) - (pictureBox1.Width / 2), (ClientSize.Height / 2) - (pictureBox1.Height / 2));
        }

        /// <summary>
        /// Handle our left mouse button control for panning.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPanning = true;
                startPt = e.Location;
            }
        }

        /// <summary>
        /// Handle our left mouse button control for panning.
        /// Right mouse button close this form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPanning = false;
                Cursor = Cursors.Default;
            }

            else if (e.Button == MouseButtons.Right)
            {
                Close();
            }
        }

        /// <summary>
        /// Our panning function.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_isPanning)
                {
                    Cursor = Cursors.SizeAll;
                    Control c = (Control)sender;
                    c.Left = (c.Left + e.X) - startPt.X;
                    c.Top = (c.Top + e.Y) - startPt.Y;
                    c.BringToFront();
                }
            }
        }

        /// <summary>
        /// Our zoom function.
        /// If the mouse wheel is moved forward (Zoom in).
        /// Check if the pictureBox dimensions are in range (15 is the minimum and maximum zoom level).
        /// Change the size of the picturebox, multiply it by the ZOOMFACTOR.
        /// Formula to move the picturebox, to zoom in the point selected by the mouse cursor.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if ((pictureBox1.Width < (15 * Width)) && (pictureBox1.Height < (15 * Height)))
                {
                    pictureBox1.Width = (int)(pictureBox1.Width * 1.25);
                    pictureBox1.Height = (int)(pictureBox1.Height * 1.25);

                    pictureBox1.Top = (int)(e.Y - 1.25 * (e.Y - pictureBox1.Top));
                    pictureBox1.Left = (int)(e.X - 1.25 * (e.X - pictureBox1.Left));
                }
            }
            else
            {
                if ((pictureBox1.Width > (Width / 15)) && (pictureBox1.Height > (Height / 15)))
                {
                    pictureBox1.Width = (int)(pictureBox1.Width / 1.25);
                    pictureBox1.Height = (int)(pictureBox1.Height / 1.25);

                    pictureBox1.Top = (int)(e.Y - 0.80 * (e.Y - pictureBox1.Top));
                    pictureBox1.Left = (int)(e.X - 0.80 * (e.X - pictureBox1.Left));
                }
            }
        }
    }
}
