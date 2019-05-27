using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageCompression
{
    class YCbCr
    {
        public byte Y;
        public byte Cb;
        public byte Cr;
        public YCbCr(byte Y, byte Cb, byte Cr)
        {
            this.Y = Y;
            this.Cb = Cb;
            this.Cr = Cr;
        }

        public static YCbCr FromColor(Color c)
        {
            //byte y = c.R;
            //byte cb = c.G;
            //byte cr = c.B;
            double yd = 0.257 * c.R + 0.504 * c.G + 0.098 * c.B + 16;
            double cbd = -0.148 * c.R - 0.291 * c.G + 0.439 * c.B + 128;
            double crd = 0.439 * c.R - 0.368 * c.G - 0.071 * c.B + 128;
            byte y = DoubleToByte(0.257 * c.R + 0.504 * c.G + 0.098 * c.B + 16);
            byte cb = DoubleToByte(-0.148 * c.R - 0.291 * c.G + 0.439 * c.B + 128);
            byte cr = DoubleToByte(0.439 * c.R - 0.368 * c.G - 0.071 * c.B + 128);
            return new YCbCr(y, cb, cr);
        }

        public static Color ToColor(YCbCr ycbcr)
        {
            return ToColor(ycbcr.Y, ycbcr.Cb, ycbcr.Cr);
        }

        public Color ToColor()
        {
            return ToColor(Y, Cb, Cr);
        }

        public static Color ToColor(byte Y, byte Cb, byte Cr)
        {
            byte r = DoubleToByte(1.164 * (Y - 16) + 1.596 * (Cr - 128));
            byte g = DoubleToByte(1.164 * (Y - 16) - 0.813 * (Cr - 128) - 0.392 * (Cb - 128));
            byte b = DoubleToByte(1.164 * (Y - 16) + 2.017 * (Cb - 128));

            //r = Y;
            //g = Y;
            //b = Y;


            return Color.FromArgb(r, g, b);
        }


        public static byte DoubleToByte(double d)
        {
            if(d > Byte.MaxValue)
            {

            }
            else if(d < 0)
            {

            }
            return (byte)(Math.Max(0, Math.Min(Math.Round(d), Byte.MaxValue)));
        }


        public static void RunTest()
        {
            Color red = Color.FromArgb(012, 123, 234);
            Color yellow = Color.FromArgb(255, 128, 0);
            Color green = Color.FromArgb(0, 0, 0);
            Color cyan = Color.FromArgb(255, 255, 255);
            Color blue = Color.FromArgb(175, 75, 125);
            Color magenta = Color.FromArgb(000, 111, 222);
            YCbCr red2 = YCbCr.FromColor(red);
            YCbCr yellow2 = YCbCr.FromColor(yellow);
            YCbCr green2 = YCbCr.FromColor(green);
            YCbCr cyan2 = YCbCr.FromColor(cyan);
            YCbCr blue2 = YCbCr.FromColor(blue);
            YCbCr magenta2 = YCbCr.FromColor(magenta);
            Color red3 = YCbCr.ToColor(red2);
            Color yellow3 = YCbCr.ToColor(yellow2);
            Color green3 = YCbCr.ToColor(green2);
            Color cyan3 = YCbCr.ToColor(cyan2);
            Color blue3 = YCbCr.ToColor(blue2);
            Color magenta3 = YCbCr.ToColor(magenta2);
        }
    }
}
