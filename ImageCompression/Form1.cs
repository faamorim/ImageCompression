using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ImageCompression
{
    public partial class MainForm : Form
    {
        private Compressor compressor = new Compressor();
        private MFNTT mfntt = new MFNTT(2);
        public MainForm()
        {
            InitializeComponent();

            //FNTT.RunDCTTest();
            //YCbCr.RunTest();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void LoadBmpBtn_Click(object sender, EventArgs e)
        {
            string fileName = loadFile("Bitmap Files|*.bmp", "Select a Bitmap File", "Bitmap File");
            if(fileName != null)
            {
                FileInfo file = new FileInfo(fileName);
                Bitmap bmp = (Bitmap)Image.FromFile(fileName);
                BitmapImage.Image = bmp;
                BitmapDescription.Text = bmp.Width*bmp.Height*3 + " bytes";
                compressor.currentBmp = bmp;
            }
        }

        private void LoadCmpBtn_Click(object sender, EventArgs e)
        {
            string fileName = loadFile("FNTT Files|*.fntt", "Select a FNTT File", "FNTT File");
            if (fileName != null)
            {
                FileInfo file = new FileInfo(fileName);
                FNTT fntt = new FNTT(File.ReadAllBytes(fileName));
                CompressedDescription.Text = file.Length.ToString() + " bytes";
                compressor.currentFntt = fntt;
                if (AutoDecompressToolStripMenuItem.Checked)
                {
                    BackgroundDecompressor.RunWorkerAsync();
                }
            }
        }

        private string loadFile(string filter, string title, string fileName)
        {
            return loadFiles(filter, title, fileName, false)[0];
        }

        private string[] loadFiles(string filter, string title, string fileName, bool multiselect = true)
        {
            OpenFileDialog.Multiselect = multiselect;
            OpenFileDialog.Filter = filter;
            OpenFileDialog.Title = title;
            OpenFileDialog.FileName = fileName;
            if (OpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && OpenFileDialog.FileName != "")
            {
                return OpenFileDialog.FileNames;
            }
            return null;
        }

        private void SaveBmpBtn_Click(object sender, EventArgs e)
        {
            string fileName = saveFile("Bitmap Files|*.bmp", "Save Uncompressed Bitmap as", "Bitmap File");
            if (fileName != null)
            {
                CompressedImage.Image.Save(fileName);
            }
        }

        private void SaveCmpBtn_Click(object sender, EventArgs e)
        {
            string fileName = saveFile("FNTT Files|*.fntt", "Save Compressed FNTT as", "FNTT File");
            if(fileName != null)
            {
                File.WriteAllBytes(SaveFileDialog.FileName, compressor.currentFntt.data);
            }
        }

        private void SaveDiffBtn_Click(object sender, EventArgs e)
        {
            string fileName = saveFile("Bitmap Files|*.bmp", "Save Uncompressed Bitmap as", "Bitmap File");
            if (fileName != null)
            {
                BitmapImage.Image.Save(fileName);
            }
        }

        private string saveFile(string filter, string title, string fileName)
        {
            SaveFileDialog.Filter = filter;
            SaveFileDialog.Title = title;
            SaveFileDialog.FileName = fileName;
            if (SaveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && SaveFileDialog.FileName != "")
            {
                return SaveFileDialog.FileName;
            }
            return null;
        }

        private void CompressBtn_Click(object sender, EventArgs e)
        {
            BackgroundCompressor.RunWorkerAsync();
        }

        private void DecompressBtn_Click(object sender, EventArgs e)
        {
            BackgroundDecompressor.RunWorkerAsync();
        }

        private void CompareBtn_Click(object sender, EventArgs e)
        {
            BitmapImage.Image = compressor.Compare();
        }





        private void AddFrameBtn_Click(object sender, EventArgs e)
        {
            if (compressor.currentBmp != null)
            {
                mfntt.AddFrame(compressor.currentBmp);
                DisplayFrame(mfntt.GetNext());
            }
        }

        private void LoadFramesBtn_Click(object sender, EventArgs e)
        {
            string[] fileNames = loadFiles("Bitmap Files|*.bmp", "Select a Bitmap File", "Bitmap File");
            if (fileNames != null)
            {
                for(int i = 0; i < fileNames.Length; i++)
                {
                    FileInfo file = new FileInfo(fileNames[i]);
                    Bitmap bmp = (Bitmap)Image.FromFile(fileNames[i]);
                    mfntt.AddFrame(bmp);
                }
            }
        }




        private void PrevFrameBtn_Click(object sender, EventArgs e)
        {
            DisplayFrame(mfntt.GetPrev());
        }

        private void NextFrameBtn_Click(object sender, EventArgs e)
        {
            DisplayFrame(mfntt.GetNext());
        }

        private void DisplayFrame(MFNTTFrame frame)
        {
            CompressedImage.Image = frame?.finalbmp;
            MotionVectorImage.Image = frame?.mv?.ToBitmap(frame.finalbmp.Width, frame.finalbmp.Height);
            PreviousImage.Image = frame?.bmp;
        }

        private void SaveVidBtn_Click(object sender, EventArgs e)
        {
            string fileName = saveFile("MFNTT Files|*.mfntt", "Save Compressed Video as", "MFNTT File");
            if (fileName != null)
            {
                File.WriteAllBytes(SaveFileDialog.FileName, mfntt.GetData());
            }
        }
        
        private void LoadVidBtn_Click(object sender, EventArgs e)
        {
            string fileName = loadFile("MFNTT Files|*.mfntt", "Select a MFNTT File", "MFNTT File");
            if (fileName != null)
            {
                FileInfo file = new FileInfo(fileName);
                mfntt = new MFNTT(File.ReadAllBytes(fileName));
            }
        }




        //██╗    ██╗ ██████╗ ██████╗ ██╗  ██╗███████╗██████╗ ███████╗
        //██║    ██║██╔═══██╗██╔══██╗██║ ██╔╝██╔════╝██╔══██╗██╔════╝
        //██║ █╗ ██║██║   ██║██████╔╝█████╔╝ █████╗  ██████╔╝███████╗
        //██║███╗██║██║   ██║██╔══██╗██╔═██╗ ██╔══╝  ██╔══██╗╚════██║
        //╚███╔███╔╝╚██████╔╝██║  ██║██║  ██╗███████╗██║  ██║███████║
        // ╚══╝╚══╝  ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝╚══════╝
        //Workers


        private void BackgroundCompressor_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            e.Result = compressor.Compress(worker);
        }

        private void BackgroundDecompressor_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            e.Result = compressor.Decompress(worker);
        }

        private void BackgroundDecompressor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CompressStatus status = e.UserState as CompressStatus;
            WorkerProgressBar.Value = status.TotalPercentage;
            WorkerStepProgressBar.Value = status.StepPercentage;
            WorkerProgressText.Text = status.Text;
        }

        private void BackgroundCompressor_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CompressStatus status = e.UserState as CompressStatus;
            WorkerProgressBar.Value = status.TotalPercentage;
            WorkerStepProgressBar.Value = status.StepPercentage;
            WorkerProgressText.Text = status.Text;
        }

        private void BackgroundCompressor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerProgressText.Text = "Completed";
            WorkerProgressBar.Value = 100;
            CompressedDescription.Text = compressor.currentFntt.GetCompressedSize().ToString() + " bytes";
            if (AutoDecompressToolStripMenuItem.Checked)
            {
                BackgroundDecompressor.RunWorkerAsync();
            }
            else
            {
                FlashWindow.Flash(this);
            }
        }

        private void BackgroundDecompressor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkerProgressText.Text = "Completed";
            WorkerProgressBar.Value = 100;
            CompressedImage.Image = (Bitmap)(e.Result);
            double rate = compressor.currentFntt.Width * compressor.currentFntt.Height * 3 / compressor.currentFntt.GetCompressedSize();
            rate = Math.Round(rate * 100) / 100;
            CompressedDescription.Text = compressor.currentFntt.GetCompressedSize().ToString() + " bytes (" + rate + "x)";
            FlashWindow.Flash(this);
        }
    }
}
