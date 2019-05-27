namespace ImageCompression
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.BitmapImage = new System.Windows.Forms.PictureBox();
            this.CompressedImage = new System.Windows.Forms.PictureBox();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.BitmapDescription = new System.Windows.Forms.Label();
            this.CompressedDescription = new System.Windows.Forms.Label();
            this.LoadBmpBtn = new System.Windows.Forms.Button();
            this.CompressBtn = new System.Windows.Forms.Button();
            this.SaveBmpBtn = new System.Windows.Forms.Button();
            this.BitmapControl = new System.Windows.Forms.GroupBox();
            this.CancelCompressBtn = new System.Windows.Forms.Button();
            this.CompareBtn = new System.Windows.Forms.Button();
            this.WorkerStepProgressBar = new System.Windows.Forms.ProgressBar();
            this.WorkerProgressBar = new System.Windows.Forms.ProgressBar();
            this.WorkerProgressText = new System.Windows.Forms.Label();
            this.CompressControl = new System.Windows.Forms.GroupBox();
            this.CancelDecompressBtn = new System.Windows.Forms.Button();
            this.SaveCmpBtn = new System.Windows.Forms.Button();
            this.DecompressBtn = new System.Windows.Forms.Button();
            this.SaveDifferenceBtn = new System.Windows.Forms.Button();
            this.LoadCmpBtn = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AutoDecompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.BackgroundCompressor = new System.ComponentModel.BackgroundWorker();
            this.BackgroundDecompressor = new System.ComponentModel.BackgroundWorker();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.PreviousImage = new System.Windows.Forms.PictureBox();
            this.BackgroundFrameAdder = new System.ComponentModel.BackgroundWorker();
            this.PreviousFrameLabel = new System.Windows.Forms.Label();
            this.MotionVectorImage = new System.Windows.Forms.PictureBox();
            this.MotionVectorsLabel = new System.Windows.Forms.Label();
            this.VideoControl = new System.Windows.Forms.GroupBox();
            this.NextFrameBtn = new System.Windows.Forms.Button();
            this.LoadFramesBtn = new System.Windows.Forms.Button();
            this.AddFrameBtn = new System.Windows.Forms.Button();
            this.LoadVidBtn = new System.Windows.Forms.Button();
            this.PrevFrameBtn = new System.Windows.Forms.Button();
            this.SaveVidBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.BitmapImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompressedImage)).BeginInit();
            this.BitmapControl.SuspendLayout();
            this.CompressControl.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviousImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MotionVectorImage)).BeginInit();
            this.VideoControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // BitmapImage
            // 
            this.BitmapImage.Location = new System.Drawing.Point(25, 44);
            this.BitmapImage.Margin = new System.Windows.Forms.Padding(16);
            this.BitmapImage.Name = "BitmapImage";
            this.BitmapImage.Size = new System.Drawing.Size(500, 500);
            this.BitmapImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BitmapImage.TabIndex = 0;
            this.BitmapImage.TabStop = false;
            // 
            // CompressedImage
            // 
            this.CompressedImage.Location = new System.Drawing.Point(955, 44);
            this.CompressedImage.Margin = new System.Windows.Forms.Padding(16);
            this.CompressedImage.Name = "CompressedImage";
            this.CompressedImage.Size = new System.Drawing.Size(500, 500);
            this.CompressedImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.CompressedImage.TabIndex = 1;
            this.CompressedImage.TabStop = false;
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.FileName = "OpenFileDialog";
            // 
            // BitmapDescription
            // 
            this.BitmapDescription.AutoSize = true;
            this.BitmapDescription.Location = new System.Drawing.Point(25, 563);
            this.BitmapDescription.Name = "BitmapDescription";
            this.BitmapDescription.Size = new System.Drawing.Size(77, 17);
            this.BitmapDescription.TabIndex = 2;
            this.BitmapDescription.Text = "Bitmap File";
            // 
            // CompressedDescription
            // 
            this.CompressedDescription.AutoSize = true;
            this.CompressedDescription.Location = new System.Drawing.Point(955, 563);
            this.CompressedDescription.Name = "CompressedDescription";
            this.CompressedDescription.Size = new System.Drawing.Size(113, 17);
            this.CompressedDescription.TabIndex = 3;
            this.CompressedDescription.Text = "Compressed File";
            // 
            // LoadBmpBtn
            // 
            this.LoadBmpBtn.Location = new System.Drawing.Point(6, 21);
            this.LoadBmpBtn.Name = "LoadBmpBtn";
            this.LoadBmpBtn.Size = new System.Drawing.Size(100, 23);
            this.LoadBmpBtn.TabIndex = 4;
            this.LoadBmpBtn.Text = "Load";
            this.LoadBmpBtn.UseVisualStyleBackColor = true;
            this.LoadBmpBtn.Click += new System.EventHandler(this.LoadBmpBtn_Click);
            // 
            // CompressBtn
            // 
            this.CompressBtn.Location = new System.Drawing.Point(244, 21);
            this.CompressBtn.Name = "CompressBtn";
            this.CompressBtn.Size = new System.Drawing.Size(100, 23);
            this.CompressBtn.TabIndex = 5;
            this.CompressBtn.Text = "Compress";
            this.CompressBtn.UseVisualStyleBackColor = true;
            this.CompressBtn.Click += new System.EventHandler(this.CompressBtn_Click);
            // 
            // SaveBmpBtn
            // 
            this.SaveBmpBtn.Location = new System.Drawing.Point(125, 50);
            this.SaveBmpBtn.Margin = new System.Windows.Forms.Padding(16, 3, 16, 3);
            this.SaveBmpBtn.Name = "SaveBmpBtn";
            this.SaveBmpBtn.Size = new System.Drawing.Size(100, 23);
            this.SaveBmpBtn.TabIndex = 6;
            this.SaveBmpBtn.Text = "Save BMP";
            this.SaveBmpBtn.UseVisualStyleBackColor = true;
            this.SaveBmpBtn.Click += new System.EventHandler(this.SaveBmpBtn_Click);
            // 
            // BitmapControl
            // 
            this.BitmapControl.Controls.Add(this.CancelCompressBtn);
            this.BitmapControl.Controls.Add(this.LoadBmpBtn);
            this.BitmapControl.Controls.Add(this.CompressBtn);
            this.BitmapControl.Controls.Add(this.CompareBtn);
            this.BitmapControl.Location = new System.Drawing.Point(25, 583);
            this.BitmapControl.Name = "BitmapControl";
            this.BitmapControl.Size = new System.Drawing.Size(500, 110);
            this.BitmapControl.TabIndex = 7;
            this.BitmapControl.TabStop = false;
            this.BitmapControl.Text = "Bitmap Controller";
            // 
            // CancelCompressBtn
            // 
            this.CancelCompressBtn.Location = new System.Drawing.Point(244, 50);
            this.CancelCompressBtn.Name = "CancelCompressBtn";
            this.CancelCompressBtn.Size = new System.Drawing.Size(100, 23);
            this.CancelCompressBtn.TabIndex = 7;
            this.CancelCompressBtn.Text = "Cancel";
            this.CancelCompressBtn.UseVisualStyleBackColor = true;
            // 
            // CompareBtn
            // 
            this.CompareBtn.Location = new System.Drawing.Point(125, 21);
            this.CompareBtn.Name = "CompareBtn";
            this.CompareBtn.Size = new System.Drawing.Size(100, 23);
            this.CompareBtn.TabIndex = 6;
            this.CompareBtn.Text = "Compare";
            this.CompareBtn.UseVisualStyleBackColor = true;
            this.CompareBtn.Click += new System.EventHandler(this.CompareBtn_Click);
            // 
            // WorkerStepProgressBar
            // 
            this.WorkerStepProgressBar.Location = new System.Drawing.Point(12, 727);
            this.WorkerStepProgressBar.Name = "WorkerStepProgressBar";
            this.WorkerStepProgressBar.Size = new System.Drawing.Size(1458, 8);
            this.WorkerStepProgressBar.TabIndex = 8;
            // 
            // WorkerProgressBar
            // 
            this.WorkerProgressBar.Location = new System.Drawing.Point(12, 713);
            this.WorkerProgressBar.Name = "WorkerProgressBar";
            this.WorkerProgressBar.Size = new System.Drawing.Size(1458, 15);
            this.WorkerProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.WorkerProgressBar.TabIndex = 8;
            // 
            // WorkerProgressText
            // 
            this.WorkerProgressText.AutoSize = true;
            this.WorkerProgressText.Location = new System.Drawing.Point(9, 733);
            this.WorkerProgressText.Name = "WorkerProgressText";
            this.WorkerProgressText.Size = new System.Drawing.Size(146, 17);
            this.WorkerProgressText.TabIndex = 2;
            this.WorkerProgressText.Text = "Worker Progress Text";
            // 
            // CompressControl
            // 
            this.CompressControl.Controls.Add(this.CancelDecompressBtn);
            this.CompressControl.Controls.Add(this.SaveCmpBtn);
            this.CompressControl.Controls.Add(this.DecompressBtn);
            this.CompressControl.Controls.Add(this.SaveDifferenceBtn);
            this.CompressControl.Controls.Add(this.SaveBmpBtn);
            this.CompressControl.Controls.Add(this.LoadCmpBtn);
            this.CompressControl.Location = new System.Drawing.Point(955, 583);
            this.CompressControl.Name = "CompressControl";
            this.CompressControl.Size = new System.Drawing.Size(500, 110);
            this.CompressControl.TabIndex = 8;
            this.CompressControl.TabStop = false;
            this.CompressControl.Text = "Compressed Controller";
            // 
            // CancelDecompressBtn
            // 
            this.CancelDecompressBtn.Location = new System.Drawing.Point(243, 50);
            this.CancelDecompressBtn.Name = "CancelDecompressBtn";
            this.CancelDecompressBtn.Size = new System.Drawing.Size(100, 23);
            this.CancelDecompressBtn.TabIndex = 8;
            this.CancelDecompressBtn.Text = "Cancel";
            this.CancelDecompressBtn.UseVisualStyleBackColor = true;
            // 
            // SaveCmpBtn
            // 
            this.SaveCmpBtn.Location = new System.Drawing.Point(125, 21);
            this.SaveCmpBtn.Margin = new System.Windows.Forms.Padding(16, 3, 16, 3);
            this.SaveCmpBtn.Name = "SaveCmpBtn";
            this.SaveCmpBtn.Size = new System.Drawing.Size(100, 23);
            this.SaveCmpBtn.TabIndex = 7;
            this.SaveCmpBtn.Text = "Save FNTT";
            this.SaveCmpBtn.UseVisualStyleBackColor = true;
            this.SaveCmpBtn.Click += new System.EventHandler(this.SaveCmpBtn_Click);
            // 
            // DecompressBtn
            // 
            this.DecompressBtn.Location = new System.Drawing.Point(243, 21);
            this.DecompressBtn.Name = "DecompressBtn";
            this.DecompressBtn.Size = new System.Drawing.Size(100, 23);
            this.DecompressBtn.TabIndex = 6;
            this.DecompressBtn.Text = "Decompress";
            this.DecompressBtn.UseVisualStyleBackColor = true;
            this.DecompressBtn.Click += new System.EventHandler(this.DecompressBtn_Click);
            // 
            // SaveDifferenceBtn
            // 
            this.SaveDifferenceBtn.Location = new System.Drawing.Point(125, 79);
            this.SaveDifferenceBtn.Margin = new System.Windows.Forms.Padding(16, 3, 16, 3);
            this.SaveDifferenceBtn.Name = "SaveDifferenceBtn";
            this.SaveDifferenceBtn.Size = new System.Drawing.Size(100, 23);
            this.SaveDifferenceBtn.TabIndex = 6;
            this.SaveDifferenceBtn.Text = "Save Diff";
            this.SaveDifferenceBtn.UseVisualStyleBackColor = true;
            this.SaveDifferenceBtn.Click += new System.EventHandler(this.SaveDiffBtn_Click);
            // 
            // LoadCmpBtn
            // 
            this.LoadCmpBtn.Location = new System.Drawing.Point(10, 21);
            this.LoadCmpBtn.Name = "LoadCmpBtn";
            this.LoadCmpBtn.Size = new System.Drawing.Size(100, 23);
            this.LoadCmpBtn.TabIndex = 6;
            this.LoadCmpBtn.Text = "Load FNTT";
            this.LoadCmpBtn.UseVisualStyleBackColor = true;
            this.LoadCmpBtn.Click += new System.EventHandler(this.LoadCmpBtn_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1482, 28);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AutoDecompressToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // AutoDecompressToolStripMenuItem
            // 
            this.AutoDecompressToolStripMenuItem.Checked = true;
            this.AutoDecompressToolStripMenuItem.CheckOnClick = true;
            this.AutoDecompressToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AutoDecompressToolStripMenuItem.Name = "AutoDecompressToolStripMenuItem";
            this.AutoDecompressToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.AutoDecompressToolStripMenuItem.Text = "Auto Decompress";
            // 
            // BackgroundCompressor
            // 
            this.BackgroundCompressor.WorkerReportsProgress = true;
            this.BackgroundCompressor.WorkerSupportsCancellation = true;
            this.BackgroundCompressor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundCompressor_DoWork);
            this.BackgroundCompressor.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundCompressor_ProgressChanged);
            this.BackgroundCompressor.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundCompressor_RunWorkerCompleted);
            // 
            // BackgroundDecompressor
            // 
            this.BackgroundDecompressor.WorkerReportsProgress = true;
            this.BackgroundDecompressor.WorkerSupportsCancellation = true;
            this.BackgroundDecompressor.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundDecompressor_DoWork);
            this.BackgroundDecompressor.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundDecompressor_ProgressChanged);
            this.BackgroundDecompressor.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundDecompressor_RunWorkerCompleted);
            // 
            // PreviousImage
            // 
            this.PreviousImage.Location = new System.Drawing.Point(640, 44);
            this.PreviousImage.Margin = new System.Windows.Forms.Padding(16);
            this.PreviousImage.Name = "PreviousImage";
            this.PreviousImage.Size = new System.Drawing.Size(200, 200);
            this.PreviousImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PreviousImage.TabIndex = 0;
            this.PreviousImage.TabStop = false;
            // 
            // PreviousFrameLabel
            // 
            this.PreviousFrameLabel.AutoSize = true;
            this.PreviousFrameLabel.Location = new System.Drawing.Point(637, 247);
            this.PreviousFrameLabel.Name = "PreviousFrameLabel";
            this.PreviousFrameLabel.Size = new System.Drawing.Size(98, 17);
            this.PreviousFrameLabel.TabIndex = 3;
            this.PreviousFrameLabel.Text = "Compensation";
            // 
            // MotionVectorImage
            // 
            this.MotionVectorImage.Location = new System.Drawing.Point(640, 290);
            this.MotionVectorImage.Margin = new System.Windows.Forms.Padding(16);
            this.MotionVectorImage.Name = "MotionVectorImage";
            this.MotionVectorImage.Size = new System.Drawing.Size(200, 200);
            this.MotionVectorImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MotionVectorImage.TabIndex = 0;
            this.MotionVectorImage.TabStop = false;
            // 
            // MotionVectorsLabel
            // 
            this.MotionVectorsLabel.AutoSize = true;
            this.MotionVectorsLabel.Location = new System.Drawing.Point(637, 493);
            this.MotionVectorsLabel.Name = "MotionVectorsLabel";
            this.MotionVectorsLabel.Size = new System.Drawing.Size(102, 17);
            this.MotionVectorsLabel.TabIndex = 3;
            this.MotionVectorsLabel.Text = "Motion Vectors";
            // 
            // VideoControl
            // 
            this.VideoControl.Controls.Add(this.NextFrameBtn);
            this.VideoControl.Controls.Add(this.LoadFramesBtn);
            this.VideoControl.Controls.Add(this.AddFrameBtn);
            this.VideoControl.Controls.Add(this.LoadVidBtn);
            this.VideoControl.Controls.Add(this.PrevFrameBtn);
            this.VideoControl.Controls.Add(this.SaveVidBtn);
            this.VideoControl.Location = new System.Drawing.Point(540, 583);
            this.VideoControl.Name = "VideoControl";
            this.VideoControl.Size = new System.Drawing.Size(400, 110);
            this.VideoControl.TabIndex = 7;
            this.VideoControl.TabStop = false;
            this.VideoControl.Text = "Video Controller";
            // 
            // NextFrameBtn
            // 
            this.NextFrameBtn.Location = new System.Drawing.Point(244, 50);
            this.NextFrameBtn.Name = "NextFrameBtn";
            this.NextFrameBtn.Size = new System.Drawing.Size(110, 23);
            this.NextFrameBtn.TabIndex = 7;
            this.NextFrameBtn.Text = "Next";
            this.NextFrameBtn.UseVisualStyleBackColor = true;
            this.NextFrameBtn.Click += new System.EventHandler(this.NextFrameBtn_Click);
            // 
            // LoadFramesBtn
            // 
            this.LoadFramesBtn.Location = new System.Drawing.Point(6, 79);
            this.LoadFramesBtn.Name = "LoadFramesBtn";
            this.LoadFramesBtn.Size = new System.Drawing.Size(110, 23);
            this.LoadFramesBtn.TabIndex = 4;
            this.LoadFramesBtn.Text = "Load Frames";
            this.LoadFramesBtn.UseVisualStyleBackColor = true;
            this.LoadFramesBtn.Click += new System.EventHandler(this.LoadFramesBtn_Click);
            // 
            // AddFrameBtn
            // 
            this.AddFrameBtn.Location = new System.Drawing.Point(6, 50);
            this.AddFrameBtn.Name = "AddFrameBtn";
            this.AddFrameBtn.Size = new System.Drawing.Size(110, 23);
            this.AddFrameBtn.TabIndex = 4;
            this.AddFrameBtn.Text = "Add Frame";
            this.AddFrameBtn.UseVisualStyleBackColor = true;
            this.AddFrameBtn.Click += new System.EventHandler(this.AddFrameBtn_Click);
            // 
            // LoadVidBtn
            // 
            this.LoadVidBtn.Location = new System.Drawing.Point(6, 21);
            this.LoadVidBtn.Name = "LoadVidBtn";
            this.LoadVidBtn.Size = new System.Drawing.Size(110, 23);
            this.LoadVidBtn.TabIndex = 4;
            this.LoadVidBtn.Text = "Load MFNTT";
            this.LoadVidBtn.UseVisualStyleBackColor = true;
            this.LoadVidBtn.Click += new System.EventHandler(this.LoadVidBtn_Click);
            // 
            // PrevFrameBtn
            // 
            this.PrevFrameBtn.Location = new System.Drawing.Point(244, 21);
            this.PrevFrameBtn.Name = "PrevFrameBtn";
            this.PrevFrameBtn.Size = new System.Drawing.Size(110, 23);
            this.PrevFrameBtn.TabIndex = 5;
            this.PrevFrameBtn.Text = "Previous";
            this.PrevFrameBtn.UseVisualStyleBackColor = true;
            this.PrevFrameBtn.Click += new System.EventHandler(this.PrevFrameBtn_Click);
            // 
            // SaveVidBtn
            // 
            this.SaveVidBtn.Location = new System.Drawing.Point(125, 21);
            this.SaveVidBtn.Name = "SaveVidBtn";
            this.SaveVidBtn.Size = new System.Drawing.Size(110, 23);
            this.SaveVidBtn.TabIndex = 6;
            this.SaveVidBtn.Text = "Save MFNTT";
            this.SaveVidBtn.UseVisualStyleBackColor = true;
            this.SaveVidBtn.Click += new System.EventHandler(this.SaveVidBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1482, 753);
            this.Controls.Add(this.WorkerProgressBar);
            this.Controls.Add(this.WorkerStepProgressBar);
            this.Controls.Add(this.WorkerProgressText);
            this.Controls.Add(this.CompressControl);
            this.Controls.Add(this.VideoControl);
            this.Controls.Add(this.BitmapControl);
            this.Controls.Add(this.MotionVectorsLabel);
            this.Controls.Add(this.PreviousFrameLabel);
            this.Controls.Add(this.CompressedDescription);
            this.Controls.Add(this.BitmapDescription);
            this.Controls.Add(this.CompressedImage);
            this.Controls.Add(this.MotionVectorImage);
            this.Controls.Add(this.PreviousImage);
            this.Controls.Add(this.BitmapImage);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Image Compression Test";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.BitmapImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompressedImage)).EndInit();
            this.BitmapControl.ResumeLayout(false);
            this.CompressControl.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviousImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MotionVectorImage)).EndInit();
            this.VideoControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox BitmapImage;
        private System.Windows.Forms.PictureBox CompressedImage;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.Label BitmapDescription;
        private System.Windows.Forms.Label CompressedDescription;
        private System.Windows.Forms.Button LoadBmpBtn;
        private System.Windows.Forms.Button CompressBtn;
        private System.Windows.Forms.Button SaveBmpBtn;
        private System.Windows.Forms.GroupBox BitmapControl;
        private System.Windows.Forms.GroupBox CompressControl;
        private System.Windows.Forms.Button LoadCmpBtn;
        private System.Windows.Forms.Button SaveCmpBtn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private System.ComponentModel.BackgroundWorker BackgroundCompressor;
        private System.Windows.Forms.Button DecompressBtn;
        private System.Windows.Forms.ProgressBar WorkerProgressBar;
        private System.Windows.Forms.Button CancelCompressBtn;
        private System.Windows.Forms.Button CancelDecompressBtn;
        private System.Windows.Forms.Label WorkerProgressText;
        private System.ComponentModel.BackgroundWorker BackgroundDecompressor;
        private System.Windows.Forms.ToolStripMenuItem AutoDecompressToolStripMenuItem;
        private System.Windows.Forms.ProgressBar WorkerStepProgressBar;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button CompareBtn;
        private System.Windows.Forms.Button SaveDifferenceBtn;
        private System.Windows.Forms.PictureBox PreviousImage;
        private System.ComponentModel.BackgroundWorker BackgroundFrameAdder;
        private System.Windows.Forms.Label PreviousFrameLabel;
        private System.Windows.Forms.PictureBox MotionVectorImage;
        private System.Windows.Forms.Label MotionVectorsLabel;
        private System.Windows.Forms.GroupBox VideoControl;
        private System.Windows.Forms.Button NextFrameBtn;
        private System.Windows.Forms.Button AddFrameBtn;
        private System.Windows.Forms.Button LoadVidBtn;
        private System.Windows.Forms.Button PrevFrameBtn;
        private System.Windows.Forms.Button SaveVidBtn;
        private System.Windows.Forms.Button LoadFramesBtn;
    }
}

