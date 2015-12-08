using System.Windows.Forms;

namespace ImgConv
{
    /// <summary>
    /// Child form two class.
    /// Combined images.
    /// </summary>
    public partial class CombinedImgs : Form
    {
        /// <summary>
        /// Form initialization.
        /// </summary>
        public CombinedImgs()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Combined images picturebox.
        /// Dimensions 400 width x 480 height.
        /// For combined images, eg: screenshots taken on 3ds.
        /// </summary>
        public PictureBox F3Picbox
        {
            get { return pictureBox1; }
        }
    }
}
