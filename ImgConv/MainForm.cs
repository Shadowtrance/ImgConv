using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ImgConv
{
    /// <summary>
    /// Main form class.
    /// </summary>
    public partial class MainForm : Form
    {
        //==========================================================================================
        #region Setup
        /// <summary>
        /// Setup region.
        /// Global variables for current file, image, current directory.
        /// Strings for our "converted" output folder and sub-folders per extension.
        /// Listbox last index.
        /// Declare our child forms.
        /// </summary>
        
        string CurrentFile;
        Image img;

        string curDir = Directory.GetCurrentDirectory();

        string converted = "converted";
        string[] exts = { "bmp", "gif", "ico", "jpg", "png", "tif", "bin" };

        private int lastIndex = 0;

        private bool isExiting;

        /// <summary>
        /// Separate images child form.
        /// </summary>
        SeparateImgs form2;

        /// <summary>
        /// Combined images child form.
        /// </summary>
        CombinedImgs form3;

        /// <summary>
        /// Big images child form.
        /// </summary>
        BigImgs form4;

        /// <summary>
        /// Main form initialization.
        /// Move our sub forms into place.
        /// Copy our "convert.exe" to the current folder if it doesn't already exist (it shouldn't exist).
        /// Create our output directories.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            Move += new EventHandler(MoveSubForm);
            Resize += new EventHandler(MoveSubForm);

            if (!File.Exists("convert.exe"))
            {
                //Copy convert.exe from resources to the folder.
                File.WriteAllBytes("convert.exe", Properties.Resources.convert);
            }

            CreateDirs();
        }

        /// <summary>
        /// Load our child forms when the main form loads.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            form2 = new SeparateImgs();
            form2.Show();
            form3 = new CombinedImgs();
            form3.Show();
            MoveSubForm(this, e);
        }

        /// <summary>
        /// Move the child forms into place on either side of the main form.
        /// </summary>
        protected void MoveSubForm(object sender, EventArgs e)
        {
            if (form2 != null && form3 != null)
            {
                form2.Height = form2.Height;
                form2.Width = form2.Width;
                form2.Left = Left + Width + 6;
                form2.Top = Top;

                form3.Height = form3.Height;
                form3.Width = form3.Width;
                form3.Left = Left - Width - 48;
                form3.Top = Top;
            }
        }

        /// <summary>
        /// Create our output directories.
        /// In order created below...
        /// 'converted' base dir
        /// bmp dir
        /// gif dir
        /// ico dir
        /// jpg dir
        /// png dir
        /// tif dir
        /// bin dir
        /// 
        /// See string[] exts at the top of this region.
        /// </summary>
        public void CreateDirs()
        {
            if (!Directory.Exists(converted))
            {
                Directory.CreateDirectory(converted);

                Directory.CreateDirectory(converted + "\\" + exts[0]);
                Directory.CreateDirectory(converted + "\\" + exts[1]);
                Directory.CreateDirectory(converted + "\\" + exts[2]);
                Directory.CreateDirectory(converted + "\\" + exts[3]);
                Directory.CreateDirectory(converted + "\\" + exts[4]);
                Directory.CreateDirectory(converted + "\\" + exts[5]);
                Directory.CreateDirectory(converted + "\\" + exts[6]);
            }
        }

        /// <summary>
        /// Select Folder menustrip.
        /// </summary>
        private void selectFoldertoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = curDir;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                listBox1.Items.Clear();
                DirectoryInfo dinfo = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                FileInfo[] files = (dinfo.GetFiles("*.bmp"))
                                    .Union(dinfo.GetFiles("*.gif"))
                                    .Union(dinfo.GetFiles("*.ico"))
                                    .Union(dinfo.GetFiles("*.jpg"))
                                    .Union(dinfo.GetFiles("*.png"))
                                    .Union(dinfo.GetFiles("*.tif"))
                                    .Union(dinfo.GetFiles("*.bin"))
                                    .ToArray();

                listBox1.Items.AddRange(files);
            }
            if (listBox1.Items.Count != 0)
            {
                listBox1.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Save file - Not currently used - Maybe use later.
        /// </summary>
        void Save(string data)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = curDir;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, data);
                    MessageBox.Show(this, "Saved to " + saveFileDialog.FileName, "DONE");
                }
            }
        }

        /// <summary>
        /// Exit application menustrip.
        /// </summary>
        private void exittoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Cleanup on exit.
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isExiting)
            {
                DialogResult response;
                response = MessageBox.Show(this, "Are you sure you want to Exit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (response == DialogResult.Yes)
                {
                    isExiting = true;
                    if (File.Exists("batch_bin.bat"))
                    {
                        File.Delete("batch_bin.bat");
                    }
                    
                    if (File.Exists("convert.exe"))
                    {
                        File.Delete("convert.exe");
                    }
                    Application.Exit();
                }
                else if (response == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Quick and dirty workaround borrowed (and changed slightly) from http://stackoverflow.com/a/11448060
        /// Save file as icon
        /// .ico
        /// https://en.wikipedia.org/wiki/ICO_%28file_format%29
        /// </summary>
        /// <param name="SourceBitmap"></param>
        /// <param name="FilePath"></param>
        public static void SaveAsIcon(Bitmap SourceBitmap, string FilePath)
        {
            using (FileStream FS = new FileStream(FilePath, FileMode.Create))
            {
                // ICO header
                FS.WriteByte(0); FS.WriteByte(0);
                FS.WriteByte(1); FS.WriteByte(0);
                FS.WriteByte(1); FS.WriteByte(0);

                // Image size
                // Set to 0 for 256 px width/height
                FS.WriteByte(0);
                FS.WriteByte(0);
                // Palette
                FS.WriteByte(0);
                // Reserved
                FS.WriteByte(0);
                // Number of color planes
                FS.WriteByte(1); FS.WriteByte(0);
                // Bits per pixel
                FS.WriteByte(32); FS.WriteByte(0);

                // Data size, will be written after the data
                FS.WriteByte(0);
                FS.WriteByte(0);
                FS.WriteByte(0);
                FS.WriteByte(0);

                // Offset to image data, fixed at 22
                FS.WriteByte(22);
                FS.WriteByte(0);
                FS.WriteByte(0);
                FS.WriteByte(0);

                // Writing actual data
                SourceBitmap.Save(FS, ImageFormat.Png);

                // Getting data length (file length minus header)
                long Len = FS.Length - 22;

                // Write it in the correct place
                FS.Seek(14, SeekOrigin.Begin);
                FS.WriteByte((byte)Len);
                FS.WriteByte((byte)(Len >> 8));
                FS.WriteByte((byte)(Len >> 16));
                FS.WriteByte((byte)(Len >> 24));
            }
        }

        #endregion
        //==========================================================================================
        #region Conversion Types - Single
        /// <summary>
        /// Conversion Types region - single
        /// </summary>

        /// <summary>
        /// Convert to Bitmap.
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            string newName = Path.GetFileNameWithoutExtension(CurrentFile);
            newName = newName + ".bmp";
            string path = curDir + "\\" + converted + "\\" + exts[0];

            try
            {
                img.Save(path + "\\" + newName, ImageFormat.Bmp);
            }
            catch
            {
                MessageBox.Show(this, "Failed to save image to bitmap.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this, "Image file saved to " + path + "\\" + newName.ToString(), "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Convert to GIF.
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            string newName = Path.GetFileNameWithoutExtension(CurrentFile);
            newName = newName + ".gif";
            string path = curDir + "\\" + converted + "\\" + exts[1];

            try
            {
                img.Save(path + "\\" + newName, ImageFormat.Gif);
            }
            catch
            {
                MessageBox.Show(this, "Failed to save image to GIF format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this, "Image file saved to " + path + "\\" + newName.ToString(), "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Convert to Icon.
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            string newName = Path.GetFileNameWithoutExtension(CurrentFile);
            newName = newName + ".ico";
            string path = curDir + "\\" + converted + "\\" + exts[2];

            FileInfo fi = (FileInfo)listBox1.SelectedItem;
            CurrentFile = listBox1.SelectedItem.ToString();

            try
            {
                SaveAsIcon((Bitmap)Image.FromFile(CurrentFile), path + "\\" + newName);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, "Failed to save image to ICO format." + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this, "Image file saved to " + path + "\\" + newName.ToString(), "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Convert to JPEG.
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            string newName = Path.GetFileNameWithoutExtension(CurrentFile);
            newName = newName + ".jpg";
            string path = curDir + "\\" + converted + "\\" + exts[3];

            try
            {
                img.Save(path + "\\" + newName, ImageFormat.Jpeg);
            }
            catch
            {
                MessageBox.Show(this, "Failed to save image to JPEG format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this, "Image file saved to " + path + "\\" + newName.ToString(), "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Convert to PNG.
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            string newName = Path.GetFileNameWithoutExtension(CurrentFile);
            newName = newName + ".png";
            string path = curDir + "\\" + converted + "\\" + exts[4];

            try
            {
                img.Save(path + "\\" + newName, ImageFormat.Png);
            }
            catch
            {
                MessageBox.Show(this, "Failed to save image to PNG format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this, "Image file saved to " + path + "\\" + newName.ToString(), "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Convert to TIFF.
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            string newName = Path.GetFileNameWithoutExtension(CurrentFile);
            newName = newName + ".tif";
            string path = curDir + "\\" + converted + "\\" + exts[5];

            try
            {
                img.Save(path + "\\" + newName, ImageFormat.Tiff);
            }
            catch
            {
                MessageBox.Show(this, "Failed to save image to TIFF format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this, "Image file saved to " + path + "\\" + newName.ToString(), "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Convert to BIN - single.
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            string newName = Path.GetFileNameWithoutExtension(CurrentFile);
            string newName2 = Path.GetFileNameWithoutExtension(CurrentFile);
            string filepath = Path.GetFullPath(CurrentFile);

            newName = newName + ".bgr";
            newName2 = newName2 + ".bin";
            string path = curDir + "\\" + converted + "\\" + exts[6];

            try
            {
                Process proc;
                proc = new Process();
                proc.StartInfo.FileName = @"cmd";

                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.Arguments = "/C" + " " + "\"" + curDir + "\\" + "convert" + " " + filepath + "\"" + " " + "-rotate 90" + " " + path + "\\" + newName;

                proc.Start();

                Console.WriteLine(proc.StartInfo.Arguments);

                proc.WaitForExit();
                proc.Close();

                File.Copy(path + "\\" + newName, path + "\\" + newName2);
                File.Delete(path + "\\" + newName);
            }
            catch
            {
                MessageBox.Show(this, "Failed to save image to BIN format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(this, "Image file saved to " + path + "\\" + newName2.ToString(), "Image Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
        //==========================================================================================
        #region Conversion Types - Batch
        /// <summary>
        /// Conversion Types region - batch.
        /// </summary>

        /// <summary>
        /// Convert to BIN - batch button.
        /// </summary>
        private void button8_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Convert to BIN - batch background worker.
        /// </summary>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string newName2 = Path.GetFileNameWithoutExtension(CurrentFile);
            newName2 = newName2 + ".bin";
            string path = curDir + "\\" + converted + "\\" + exts[6];

            Process proc;
            proc = new Process();
            proc.StartInfo.FileName = @"cmd";

            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.Arguments = "/C" + "batch_bin";

            try
            {
                WriteBatch_MassBin();
            }
            finally
            {
                if (File.Exists("batch_bin.bat"))
                {
                    proc.Start();
                    while (!proc.HasExited)
                    {
                        for (int i = 0; i <= 100; i++)
                        {
                            backgroundWorker1.ReportProgress(i);
                            Thread.Sleep(100);
                        }
                    }
                    proc.WaitForExit();
                    proc.Close();
                }
                else
                {
                    MessageBox.Show(this, "Something went wrong, batch file does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Convert to BIN - batch background worker completed.
        /// </summary>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string path = curDir + "\\" + converted + "\\" + exts[6];
            MessageBox.Show(this, "Image files saved to " + path + "\\", "Images Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            label2.Text = "";
        }

        /// <summary>
        /// Convert to BIN - batch worker progress reporting.
        /// </summary>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label2.Text = "Working... " + e.ProgressPercentage.ToString() + "%";
        }

        /// <summary>
        /// Create our batch file for mass convert to BIN.
        /// I'm a noob and don't actually know how to do this properly (without the batch file) in C# yet. :(
        /// So the temporary batch file does it for us instead.
        /// </summary>
        public void WriteBatch_MassBin()
        {
            string dir = curDir + "\\";
            string filename = "batch_bin.bat";
            string path = Path.Combine(dir + filename);

            Stream stream = null;

            try
            {
                stream = new FileStream(path, FileMode.Create);
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.WriteLine("@ECHO OFF");
                    sw.WriteLine("Setlocal EnableDelayedExpansion" + Environment.NewLine);
                    sw.WriteLine("for %%1 in " + "(" + "\"" + folderBrowserDialog1.SelectedPath + "\\" + "*.png" + "\"" + ")" + " do (");
                    sw.WriteLine("convert " + "\"" + "%%1" + "\"" + " -rotate 90 " + "\"" + "%%~n1.bgr" + "\"");
                    sw.WriteLine("sleep 0.1");
                    sw.WriteLine("ren " + "\"" + "%%~n1.bgr" + "\"" + " " + "\"" + "%%~n1.bin" + "\"");
                    sw.WriteLine("sleep 0.1");
                    sw.WriteLine(")");
                    sw.WriteLine("GOTO:MOVE" + Environment.NewLine);
                    sw.WriteLine(":MOVE");
                    sw.WriteLine("move /Y " + "\"" + "*.bin" + "\"" + " " + "\"" + "converted/bin" + "\"" + " >nul");
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Flush();
                }
            }
        }

        #endregion
        //==========================================================================================
        #region Picturebox / Listbox
        /// <summary>
        /// Picturebox / Listbox handling region.
        /// </summary>

        /// <summary>
        /// Show selected image in picturebox
        /// </summary>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                GetSize();
            }
        }

        /// <summary>
        /// Listbox jump to top from last item / Listbox jump to bottom from first item.
        /// </summary>
        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (listBox1.SelectedIndex == lastIndex)
            {
                if (e.KeyCode == Keys.Up)
                {
                    listBox1.SelectedIndex = listBox1.Items.Count - 1;
                }

                if (e.KeyCode == Keys.Down)
                {
                    listBox1.SelectedIndex = 0;
                }
            }
            lastIndex = listBox1.SelectedIndex;
        }

        /// <summary>
        /// Check image dimensions and put it in the correct picturebox.
        /// If the image doesn't match the dimensions for either the "combined" images picturebox
        /// or the "separate" images picturebox it will display in the small picturebox on the main form.
        /// If the image is too big for that picturebox it opens "Form4", "Big Images" and displays the image
        /// in that picturebox instead.
        /// If the file selected in the listbox happens to be a .bin file, a messagebox will show saying so.
        /// </summary>
        private void GetSize()
        {
            FileInfo fi = (FileInfo)listBox1.SelectedItem;
            CurrentFile = listBox1.SelectedItem.ToString();
            img = Image.FromFile(fi.FullName);

            int[] ds_size = { 400, 320, 480, 240 }; //0 - 3 width/width/height/height

            if (fi.Extension != ".bin")
            {
                if (img.Width == ds_size[0] && img.Height == ds_size[2])
                {
                    form3.F3Picbox.ImageLocation = fi.FullName;
                }

                if (img.Width == ds_size[0] && img.Height == ds_size[3])
                {
                    form2.F2Picbox.ImageLocation = fi.FullName;
                }

                if (img.Width == ds_size[1] && img.Height == ds_size[3])
                {
                    form2.F2Picbox2.ImageLocation = fi.FullName;
                }

                if (img.Width != ds_size[1] && img.Width != ds_size[0] && img.Height != ds_size[2] && img.Height != ds_size[3])
                {
                    if (img.Width >= ds_size[1] && img.Height >= ds_size[3])
                    {
                        form4 = new BigImgs();
                        form4.Show();
                        form4.BigPicBox.ImageLocation = fi.FullName;
                    }
                    else
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                        pictureBox1.ImageLocation = fi.FullName;
                    }
                }
            }
            else
            {
                MessageBox.Show(this, "BIN file detected...no image to show.\n\nNothing to do just yet, BIN to (other image types) not implemented yet.", "BIN FILE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion
        //==========================================================================================
    }
}
