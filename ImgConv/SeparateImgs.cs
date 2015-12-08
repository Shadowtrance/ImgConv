using System.Windows.Forms;

namespace ImgConv
{
    /// <summary>
    /// Child form one class.
    /// Separate images.
    /// </summary>
    public partial class SeparateImgs : Form
    {
        /// <summary>
        /// Form initialization.
        /// </summary>
        public SeparateImgs()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Top image picturebox.
        /// Dimensions 400 width x 240 height.
        /// </summary>
        public PictureBox F2Picbox
        {
            get{ return pictureBox2; }
        }

        /// <summary>
        /// Bottom image picturebox.
        /// Dimensions 320 width x 240 height.
        /// </summary>
        public PictureBox F2Picbox2
        {
            get { return pictureBox3; }
        }
    }
}
