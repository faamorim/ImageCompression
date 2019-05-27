using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageCompression
{
    class MotionVectors
    {
        public byte[] directions;
        public MotionVectors(byte[] dir)
        {
            directions = dir;
        }
        public MotionVectors(Bitmap source, Bitmap destination, int searchSize = 7, int blockSize = 8)
        {
            int HBlocks = (int)Math.Ceiling(1.0 * destination.Width / blockSize);
            int VBlocks = (int)Math.Ceiling(1.0 * destination.Height / blockSize);
            int blockCount = HBlocks * VBlocks;
            directions = new byte[blockCount];
            for(int j = 0; j < VBlocks; j++)
            {
                for(int i = 0; i < HBlocks; i++)
                {
                    directions[j * HBlocks + i] = GetBestVector(ref source, ref destination, i, j, searchSize, blockSize);
                }
            }
        }
        public static void GetXAndYFromByteDir(byte direction, out int x, out int y)
        {
            direction += 8;
            y = direction / 16 - 8;
            x = direction % 16 - 8;
        }
        public static byte GetByteDirFromXAndY(int x, int y)
        {
            return (byte)(16 * (y + 8) + (x + 8) - 8);
        }
        public static byte GetBestVector(ref Bitmap source, ref Bitmap destination, int blockPosX, int blockPosY, int searchSize = 7, int blockSize = 8)
        {
            int minDiffX = 0, minDiffY = 0;
            int minDiff = CalculateDifference(ref source, ref destination, blockPosX, blockPosY, 0, 0, blockSize);
            int curDiff;
            for (int y = -searchSize; y <= searchSize; y++)
            {
                for(int x = -searchSize; x <= searchSize; x++)
                {
                    if (blockPosX * blockSize + x < 0
                        || (blockPosX + 1) * blockSize - 1 + x >= source.Width
                        || blockPosY * blockSize + y < 0
                        || (blockPosY + 1) * blockSize - 1 + y >= source.Height)
                    {
                        continue;
                    }
                    curDiff = CalculateDifference(ref source, ref destination, blockPosX, blockPosY, x, y, blockSize);
                    
                    if (curDiff < minDiff)
                    {
                        minDiff = curDiff;
                        minDiffX = x;
                        minDiffY = y;
                        if(curDiff == 0)
                        {
                            x = searchSize + 1;
                            y = searchSize + 1;
                        }
                    }
                }
            }
            return GetByteDirFromXAndY(minDiffX, minDiffY);
        }
        static int CalculateDifference(ref Bitmap source, ref Bitmap destination, int blockPosX, int blockPosY, int searchX, int searchY, int blockSize = 8)
        {
            int destX, destY, srcX, srcY;
            byte srcR, srcG, srcB, destR, destG, destB;
            Color dest, src;
            int curDiff = 0;
            for (int j = 0; j < blockSize; j++)
            {
                for (int i = 0; i < blockSize; i++)
                {
                    destX = blockPosX * blockSize + i;
                    destY = blockPosY * blockSize + j;
                    srcX = blockPosX * blockSize + i + searchX;
                    srcY = blockPosY * blockSize + j + searchY;
                    destR = 0; destG = 0; destB = 0; srcR = 0; srcG = 0; srcB = 0;

                    if (destX >= 0 && destX < destination.Width && destY >= 0 && destY < destination.Height)
                    {
                        dest = destination.GetPixel(destX, destY);
                        destR = dest.R;
                        destG = dest.G;
                        destB = dest.B;
                        if (srcX >= 0 && srcX < source.Width && srcY >= 0 && srcY < source.Height)
                        {
                            src = source.GetPixel(srcX, srcY);
                            srcR = src.R;
                            srcG = src.G;
                            srcB = src.B;
                        }
                    }
                    curDiff += Math.Abs(destR - srcR) + Math.Abs(destG - srcG) + Math.Abs(destB - srcB);
                }
            }
            return curDiff;
        }
        public Bitmap ToBitmap(int width, int height, int blockSize = 8, Bitmap background = null)
        {
            Bitmap bmp;
            int maxdmv = 4;
            if (background != null)
            {
                bmp = new Bitmap(background);
            }
            else
            {
                bmp = new Bitmap(width, height);
            }
            int HBlocks = (int)Math.Ceiling(1.0 * width / blockSize);
            int VBlocks = (int)Math.Ceiling(1.0 * height / blockSize);
            for (int j = 0; j < VBlocks; j++)
            {
                for (int i = 0; i < HBlocks; i++)
                {
                    int x, y;
                    GetXAndYFromByteDir(directions[j * HBlocks + i], out x, out y);
                    int absX = Math.Abs(x);
                    int absY = Math.Abs(y);
                    int signX = Math.Sign(x);
                    int signY = Math.Sign(y);
                    for (int dmv = 0; dmv <= maxdmv; dmv++)
                    {
                        for (int dx = 3; dx <= 4; dx++)
                        {
                            for (int dy = 3; dy <= 4; dy++)
                            {
                                int px = i * blockSize + dx + dmv * x / maxdmv;
                                int py = j * blockSize + dy + dmv * y / maxdmv;
                                if (px >= 0 && px < bmp.Width && py >= 0 && py < bmp.Height)
                                {
                                    int pg = 200 * (dmv) / maxdmv;
                                    Color pc = Color.FromArgb(pg, pg, pg);
                                    bmp.SetPixel(px, py, pc);
                                }
                            }
                        }
                    }
                }
            }
            return bmp;
        }
    }
    class MFNTTFrame
    {
        public Bitmap bmp;
        public FNTT fntt;
        public MotionVectors mv;
        public int keyindex;
        public Bitmap finalbmp;
        public static MFNTTFrame KeyFrame(Bitmap keyFrameBmp, int index)
        {
            FNTT framefntt = FNTT.FromBitmap(keyFrameBmp);
            return KeyFrame(framefntt, index);
        }

        public static MFNTTFrame KeyFrame(FNTT keyFrameFntt, int index)
        {
            Bitmap framebmp = keyFrameFntt.ToBitmap();
            return new MFNTTFrame
            {
                fntt = keyFrameFntt,
                bmp = framebmp,
                mv = null,
                keyindex = index,
                finalbmp = framebmp
            };
        }

        public static MFNTTFrame IntermediaryFrame(Bitmap frameSourceBmp, Bitmap keyFrame, int keyindex)
        {
            MotionVectors framemv = new MotionVectors(keyFrame, frameSourceBmp);
            Bitmap compbmp = MFNTT.GetMotionCompensation(keyFrame, frameSourceBmp, framemv);
            FNTT framefntt = FNTT.FromBitmap(compbmp);
            return IntermediaryFrame(framefntt, framemv, keyFrame, keyindex);
        }


        public static MFNTTFrame IntermediaryFrame(FNTT frameSourceFntt, MotionVectors framemv, Bitmap keyFrame, int keyindex)
        {
            Bitmap framebmp = frameSourceFntt.ToBitmap();
            return new MFNTTFrame
            {
                fntt = frameSourceFntt,
                bmp = framebmp,
                mv = framemv,
                keyindex = keyindex,
                finalbmp = MFNTT.GetIntermediaryFrame(keyFrame, framebmp, framemv)
            };
        }

    }
    class MFNTT
    {
        int CurrentIndex = 0;
        int IntermediaryFrameCount;
        Bitmap LastKeyFrameOriginalBmp;
        
        public ArrayList FrameList = new ArrayList();

        public MFNTT(byte[] data)
        {
            int offset = 0;
            int framecount = 0;
            FNTT.GetNext(ref IntermediaryFrameCount, ref data, ref offset);
            FNTT.GetNext(ref framecount, ref data, ref offset);
            for (int i = 0; i < framecount; i++)
            {
                bool key = true;
                int mvsize = 0;
                FNTT.GetNext(ref mvsize, ref data, ref offset);
                MotionVectors mv = null;
                if (mvsize != 0)
                {
                    byte[] mvba = new byte[mvsize];
                    for(int mvi = 0; mvi < mvsize; mvi++)
                    {
                        FNTT.GetNext(ref mvba[mvi], ref data, ref offset);
                    }
                    mv = new MotionVectors(mvba);
                    key = false;
                }
                int fnttsize = 0;
                FNTT.GetNext(ref fnttsize, ref data, ref offset);
                byte[] fnttba = new byte[fnttsize];
                for (int fntti = 0; fntti < fnttsize; fntti++)
                {
                    FNTT.GetNext(ref fnttba[fntti], ref data, ref offset);
                }
                FNTT fntt = new FNTT(fnttba);
                AddFrame(mv, fntt);
            }
        }

        public MFNTT(int intermediaryFrameCount = 9)
        {
            IntermediaryFrameCount = Math.Max(1,intermediaryFrameCount);
        }

        public void AddFrame(Bitmap bmp)
        {
            int curIndex = FrameList.Count;
            int prevKey = GetPrevKeyIndex(curIndex);
            if (FrameList.Count % (IntermediaryFrameCount + 1) == 0)
            {
                MFNTTFrame keyframe = MFNTTFrame.KeyFrame(bmp,curIndex);
                FrameList.Add(keyframe);
                LastKeyFrameOriginalBmp = keyframe.bmp;
            }
            else
            {
                MFNTTFrame interFrame = MFNTTFrame.IntermediaryFrame(bmp, GetFrame(prevKey).bmp, prevKey);
                FrameList.Add(interFrame);
            }
        }

        public void AddFrame(MotionVectors mv, FNTT fntt)
        {
            int curIndex = FrameList.Count;
            int prevKey = GetPrevKeyIndex(curIndex);
            if (FrameList.Count % (IntermediaryFrameCount + 1) == 0)
            {
                MFNTTFrame keyframe = MFNTTFrame.KeyFrame(fntt, curIndex);
                FrameList.Add(keyframe);
                LastKeyFrameOriginalBmp = keyframe.bmp;
            }
            else
            {
                MFNTTFrame interFrame = MFNTTFrame.IntermediaryFrame(fntt, mv, GetFrame(prevKey).bmp, prevKey);
                FrameList.Add(interFrame);
            }
        }

        public int GetPrevKeyIndex(int index)
        {
            return (int)Math.Floor(index / (IntermediaryFrameCount + 1.0)) * (IntermediaryFrameCount + 1);
        }

        public MFNTTFrame GetFrame(int index)
        {
            return index < FrameList.Count && index >= 0 ? ((MFNTTFrame)FrameList[index]) : null;
        }

        public MFNTTFrame GetNext()
        {
            CurrentIndex = Math.Min(FrameList.Count - 1, CurrentIndex + 1);
            return GetFrame(CurrentIndex);
        }

        public MFNTTFrame GetPrev()
        {
            CurrentIndex = Math.Max(0, CurrentIndex - 1);
            return GetFrame(CurrentIndex);
        }


        public static Bitmap GetMotionCompensation(Bitmap source, Bitmap destination, MotionVectors mvs, int blockSize = 8)
        {
            Bitmap compensation = new Bitmap(destination.Width, destination.Height, destination.PixelFormat);
            int HBlocks = (int)Math.Ceiling(1.0 * destination.Width / blockSize);
            int VBlocks = (int)Math.Ceiling(1.0 * destination.Height / blockSize);
            int modX, modY, curX, curY;
            int r, g, b;
            Color destPixel, srcPixel, compPixel;
            for (int j = 0; j < VBlocks; j++)
            {
                for (int i = 0; i < HBlocks; i++)
                {
                    MotionVectors.GetXAndYFromByteDir(mvs.directions[j * HBlocks + i], out modX, out modY);
                    for (int y = 0; y < blockSize && j * blockSize + y < destination.Height; y++)
                    {
                        for (int x = 0; x < blockSize && i * blockSize + x < destination.Width; x++)
                        {
                            curX = i * blockSize + x;
                            curY = j * blockSize + y;

                            destPixel = destination.GetPixel(curX, curY);
                            if (curX + modX >= 0 && curX + modX < source.Width && curY + modY >= 0 && curY + modY < source.Height)
                            {
                                srcPixel = source.GetPixel(curX + modX, curY + modY);
                            }
                            else
                            {
                                srcPixel = Color.FromArgb(0, 0, 0);
                            }
                            r = YCbCr.DoubleToByte(128.0 + destPixel.R - srcPixel.R);
                            g = YCbCr.DoubleToByte(128.0 + destPixel.G - srcPixel.G);
                            b = YCbCr.DoubleToByte(128.0 + destPixel.B - srcPixel.B);

                            compPixel = Color.FromArgb(r, g, b);
                            compensation.SetPixel(curX, curY, compPixel);
                        }
                    }
                }
            }
            return compensation;
        }


        public static Bitmap GetIntermediaryFrame(Bitmap keyframe, Bitmap compensation, MotionVectors mvs, int blockSize = 8)
        {
            Bitmap frame = new Bitmap(compensation.Width, compensation.Height, compensation.PixelFormat);
            int HBlocks = (int)Math.Ceiling(1.0 * compensation.Width / blockSize);
            int VBlocks = (int)Math.Ceiling(1.0 * compensation.Height / blockSize);
            int modX, modY, curX, curY;
            int r, g, b;
            Color compPixel, keyPixel, framePixel;
            for (int j = 0; j < VBlocks; j++)
            {
                for (int i = 0; i < HBlocks; i++)
                {
                    MotionVectors.GetXAndYFromByteDir(mvs.directions[j * HBlocks + i], out modX, out modY);
                    for (int y = 0; y < blockSize && j * blockSize + y < compensation.Height; y++)
                    {
                        for (int x = 0; x < blockSize && i * blockSize + x < compensation.Width; x++)
                        {
                            curX = i * blockSize + x;
                            curY = j * blockSize + y;

                            compPixel = compensation.GetPixel(curX, curY);
                            if (curX + modX >= 0 && curX + modX < keyframe.Width && curY + modY >= 0 && curY + modY < keyframe.Height)
                            {
                                keyPixel = keyframe.GetPixel(curX + modX, curY + modY);
                            }
                            else
                            {
                                keyPixel = Color.FromArgb(0, 0, 0);
                            }
                            r = YCbCr.DoubleToByte(compPixel.R - 128.0 + keyPixel.R);
                            g = YCbCr.DoubleToByte(compPixel.G - 128.0 + keyPixel.G);
                            b = YCbCr.DoubleToByte(compPixel.B - 128.0 + keyPixel.B);

                            framePixel = Color.FromArgb(r, g, b);
                            frame.SetPixel(curX, curY, framePixel);
                        }
                    }
                }
            }
            return frame;
        }


        public byte[] GetData()
        {
            byte[] bytearr;
            using (MemoryStream mem = new MemoryStream())
            {
                using (BinaryWriter bin = new BinaryWriter(mem))
                {
                    bin.Write(IntermediaryFrameCount);
                    bin.Write(FrameList.Count);
                    for (int i = 0; i < FrameList.Count; i++)
                    {
                        MFNTTFrame frame = (MFNTTFrame)FrameList[i];
                        if (frame.mv != null)
                        {
                            bin.Write(frame.mv.directions.Length);
                            bin.Write(frame.mv.directions);
                        }
                        else
                        {
                            bin.Write((int)0);
                        }
                        bin.Write(frame.fntt.data.Length);
                        bin.Write(frame.fntt.data);
                    }
                }
                bytearr = mem.ToArray();
            }
            return bytearr;
        }
    }
}
