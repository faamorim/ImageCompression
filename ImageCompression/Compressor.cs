using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace ImageCompression
{
    class CompressStatus
    {
        public bool alwaysReport = true;
        private BackgroundWorker Worker;
        private DoWorkEventArgs EventArgs;
        private int NumberOfSteps = 1;
        private int CurrentStep = 0;
        private int StepLength = 1;
        private int StepPosition = 0;
        private int PrevStepPosition = 0;
        public string Title { get; private set; }
        public string StepName { get; private set; }
        public string Text { get { return Title + ": Step " + (CurrentStep + 1) + "/" + NumberOfSteps + " - " + StepName; } }
        public int TotalPercentage { get { return (100 * CurrentStep) / NumberOfSteps; } }
        public int StepPercentage { get { return (100 * StepPosition) / StepLength; } }
        public CompressStatus(BackgroundWorker worker, DoWorkEventArgs eventArgs)
        {
            Worker = worker;
            EventArgs = eventArgs;
        }
        public void Report()
        {
            Worker.ReportProgress(0, this);
        }
        public void FirstStep(string title = null, string name = null, int length = 1)
        {
            Title = title;
            NextStep(name, length);
            CurrentStep = 0;
        }
        public void NextStep(string name = null, int length = 1)
        {
            CurrentStep++;
            if (name == null)
            {
                name = "Step " + CurrentStep;
            }
            SetStepLength(length);
            StepName = name;
            StepPosition = 0;
            if (alwaysReport)
            {
                Report();
            }
        }
        public void SetNumSteps(int steps)
        {
            NumberOfSteps = Math.Max(1, steps);
        }
        public void SetStepLength(int length)
        {
            StepLength = Math.Max(1, length);
            PrevStepPosition = 0;
        }
        public void AdvanceInStep(int amount = 1)
        {
            StepPosition += amount;
            if (alwaysReport && (1.0 * StepPosition - PrevStepPosition) / StepLength >= 0.01)
            {
                Report();
                PrevStepPosition = StepPosition;
            }
        }
    }
    class Compressor
    {
        public FNTT currentFntt;
        private Bitmap currentFnttBmp;
        public Bitmap currentBmp;
        public Bitmap compareBmp;

        public FNTT Compress(BackgroundWorker worker = null, DoWorkEventArgs eventArgs = null)
        {
            CompressStatus status = new CompressStatus(worker, eventArgs);
            currentFntt = FNTT.FromBitmap(currentBmp, null, status);
            return currentFntt;
        }
        public Bitmap Decompress(BackgroundWorker worker = null, DoWorkEventArgs eventArgs = null)
        {
            CompressStatus status = new CompressStatus(worker, eventArgs);
            currentFnttBmp = FNTT.ToBitmap(currentFntt, status);
            return currentFnttBmp;
        }

        public Bitmap Compare()
        {
            if (currentBmp == null || currentFnttBmp == null)
            {
                compareBmp = null;
                return compareBmp;
            }
            if (currentBmp.Width != currentFnttBmp.Width || currentBmp.Height != currentFnttBmp.Height)
            {
                compareBmp = null;
                return compareBmp;
            }
            compareBmp = currentBmp.Clone(new Rectangle(0, 0, currentBmp.Width, currentBmp.Height), currentBmp.PixelFormat);
            for (int i = 0; i < currentBmp.Width; ++i)
            {
                for (int j = 0; j < currentBmp.Height; ++j)
                {
                    Color c1 = currentBmp.GetPixel(i, j);
                    Color c2 = currentFnttBmp.GetPixel(i, j);
                    byte r = YCbCr.DoubleToByte(c2.R - c1.R + (byte.MaxValue / 2));
                    byte g = YCbCr.DoubleToByte(c2.G - c1.G + (byte.MaxValue / 2));
                    byte b = YCbCr.DoubleToByte(c2.B - c1.B + (byte.MaxValue / 2));
                    Color finalColor = Color.FromArgb(r, g, b);
                    compareBmp.SetPixel(i, j, finalColor);
                }
            }
            return compareBmp;
        }
    }
}
