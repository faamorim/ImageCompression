using System;
using System.Collections;
using System.Collections.Generic;
//using System.IO;
using System.Linq;
using System.Text;

namespace ImageCompression
{
    public class HuffmanTree
    {

        private class BinaryWriter : System.IO.BinaryWriter
        {
            private bool[] curByte = new bool[8];
            private byte curBitIndx = 0;
            private System.Collections.BitArray ba;

            public BinaryWriter(System.IO.Stream s) : base(s) { }

            public override void Flush()
            {
                base.Write(ConvertToByte(curByte));
                base.Flush();
            }

            public override void Write(bool value)
            {
                curByte[curBitIndx] = value;
                curBitIndx++;

                if (curBitIndx == 8)
                {
                    base.Write(ConvertToByte(curByte));
                    this.curBitIndx = 0;
                    this.curByte = new bool[8];
                }
            }

            public override void Write(byte value)
            {
                ba = new BitArray(new byte[] { value });
                for (byte i = 0; i < 8; i++)
                {
                    this.Write(ba[i]);
                }
                ba = null;
            }

            public override void Write(byte[] buffer)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    this.Write((byte)buffer[i]);
                }
            }

            public override void Write(uint value)
            {
                ba = new BitArray(BitConverter.GetBytes(value));
                for (byte i = 0; i < 32; i++)
                {
                    this.Write(ba[i]);
                }
                ba = null;
            }

            public override void Write(ulong value)
            {
                ba = new BitArray(BitConverter.GetBytes(value));
                for (byte i = 0; i < 64; i++)
                {
                    this.Write(ba[i]);
                }
                ba = null;
            }

            public override void Write(ushort value)
            {
                ba = new BitArray(BitConverter.GetBytes(value));
                for (byte i = 0; i < 16; i++)
                {
                    this.Write(ba[i]);
                }
                ba = null;
            }

            private static byte ConvertToByte(bool[] bools)
            {
                byte b = 0;

                byte bitIndex = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (bools[i])
                    {
                        b |= (byte)(((byte)1) << bitIndex);
                    }
                    bitIndex++;
                }

                return b;
            }
        }

        private class BinaryReader : System.IO.BinaryReader
        {
            private bool[] curByte = new bool[8];
            private byte curBitIndx = 0;
            private BitArray ba;

            public BinaryReader(System.IO.Stream s) : base(s)
            {
                ba = new BitArray(new byte[] { base.ReadByte() });
                ba.CopyTo(curByte, 0);
                ba = null;
            }

            public override bool ReadBoolean()
            {
                if (curBitIndx == 8)
                {
                    ba = new BitArray(new byte[] { base.ReadByte() });
                    ba.CopyTo(curByte, 0);
                    ba = null;
                    this.curBitIndx = 0;
                }

                bool b = curByte[curBitIndx];
                curBitIndx++;
                return b;
            }

            public override byte ReadByte()
            {
                bool[] bar = new bool[8];
                byte i;
                for (i = 0; i < 8; i++)
                {
                    bar[i] = this.ReadBoolean();
                }

                byte b = 0;
                byte bitIndex = 0;
                for (i = 0; i < 8; i++)
                {
                    if (bar[i])
                    {
                        b |= (byte)(((byte)1) << bitIndex);
                    }
                    bitIndex++;
                }
                return b;
            }

            public override byte[] ReadBytes(int count)
            {
                byte[] bytes = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    bytes[i] = this.ReadByte();
                }
                return bytes;
            }

            public override ushort ReadUInt16()
            {
                byte[] bytes = ReadBytes(2);
                return BitConverter.ToUInt16(bytes, 0);
            }

            public override uint ReadUInt32()
            {
                byte[] bytes = ReadBytes(4);
                return BitConverter.ToUInt32(bytes, 0);
            }

            public override ulong ReadUInt64()
            {
                byte[] bytes = ReadBytes(8);
                return BitConverter.ToUInt64(bytes, 0);
            }
        }


        private List<Node> nodes = new List<Node>();
        private Node Root { get; set; }
        private Dictionary<byte, int> Frequencies = new Dictionary<byte, int>();


        private HuffmanTree() { }

        public static HuffmanTree BuildFromSource(byte[] source)
        {
            HuffmanTree huff = new HuffmanTree();
            for (int i = 0; i < source.Length; i++)
            {
                if (!huff.Frequencies.ContainsKey(source[i]))
                {
                    huff.Frequencies.Add(source[i], 0);
                }

                huff.Frequencies[source[i]]++;
            }
            huff.Build();
            return huff;
        }
        public static HuffmanTree BuildFromDictionary(byte[] source)
        {
            int dictSize = source[0];
            HuffmanTree huff = new HuffmanTree();
            int extraIndex = 0;
            for(int i = 0; i < source.Length / 2; i++)
            {
                byte curByte = source[2 * i];
                int curCount = source[2 * i + 1];
                if (huff.Frequencies.ContainsKey(curByte))
                {
                    huff.Frequencies[curByte] = huff.Frequencies[curByte] * (Byte.MaxValue + 1) + curCount;
                    extraIndex++;
                }
                else
                    huff.Frequencies.Add(curByte, curCount);
            }
            huff.Build();
            return huff;
        }

        private void Build()
        {
            foreach (KeyValuePair<byte, int> symbol in Frequencies)
            {
                nodes.Add(new Node() {
                    Symbol = symbol.Key,
                    Frequency = symbol.Value,
                    uid = symbol.Key + ""
                });
            }

            while (nodes.Count > 1)
            {
                List<Node> orderedNodes = new List<Node>(nodes);
                orderedNodes.Sort((node1, node2) => (FrequencyCompare(node1, node2)));

                if (orderedNodes.Count >= 2)
                {
                    // Take first two items
                    List<Node> taken = orderedNodes.Take(2).ToList<Node>();

                    // Create a parent node by combining the frequencies
                    Node parent = new Node()
                    {
                        Symbol = 0,
                        Frequency = taken[0].Frequency + taken[1].Frequency,
                        Left = taken[0],
                        Right = taken[1],
                        uid = "<" + taken[0].uid + "," + taken[1].uid + ">"
                    };

                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                Root = nodes.FirstOrDefault();

            }
        }
        
        static public byte[] Encode(byte[] source)
        {
            HuffmanTree huffman = BuildFromSource(source);
            List<KeyValuePair<byte, int>> dictionary = huffman.Frequencies.ToList();
            byte dictSize = (byte)dictionary.Count;
            dictionary.Sort((pair1, pair2) => (FrequencyCompare(pair1, pair2)));
            BitArray bitarr = huffman.EncodeSource(source);
            byte[] bytearr = null;
            using(System.IO.MemoryStream mem = new System.IO.MemoryStream())
            {
                using (BinaryWriter bin = new BinaryWriter(mem))
                {
                    bin.Write(dictSize);
                    int realDictSize = dictSize == 0 ? Byte.MaxValue + 1 : dictSize;
                    for (int d = 0; d < realDictSize; ++d)
                    {
                        int count = dictionary[d].Value;
                        byte key = dictionary[d].Key;
                        if(count <= Byte.MaxValue)
                        {
                            bin.Write(key);
                            bin.Write((byte)count);
                        }
                        else
                        {
                            List<byte> countList = new List<byte>();
                            do
                            {
                                countList.Add((byte)(count % (Byte.MaxValue + 1)));
                                count /= (Byte.MaxValue + 1);
                            } while (count >= 1);
                            for(int i = countList.Count - 1; i >= 0; --i)
                            {
                                bin.Write(key);
                                bin.Write(countList[i]);
                            }
                        }

                    }
                    byte padding = (byte)((8 - (bitarr.Length % 8)) % 8);
                    bin.Write(padding);
                    for (int b = 0; b < bitarr.Length; ++b) {
                        bin.Write(bitarr[b]);
                    }
                    for (int p = 0; p < padding; ++p)
                    {
                        bin.Write(false);
                    }
                }
                bytearr = mem.ToArray();
            }
            
            bool[] encodedArrayBit = new bool[bitarr.Count];
            bitarr.CopyTo(encodedArrayBit, 0);

            return bytearr;
        }

        public static byte[] Decode(byte[] encoded)
        {
            long bitCount = 0;
            int dictSize;
            BitArray encodedBitArray = new BitArray(0);
            List<byte> dictionary = new List<byte>();
            int realDictSize = 0;
            using (System.IO.MemoryStream input = new System.IO.MemoryStream(encoded))
            {
                using (BinaryReader read = new BinaryReader(input))
                {
                    dictSize = read.ReadByte();
                    if(dictSize == 0)
                    {
                        dictSize = Byte.MaxValue + 1;
                    }
                    //dictionary.Add(dictSize);
                    int previousByte = -1;
                    byte curByte = 0;
                    for(int i = 0; i <= dictSize; ++i)
                    {
                        curByte = read.ReadByte();
                        if (curByte == previousByte)
                        {
                            i--;
                        }
                        if (i != dictSize)
                        {
                            realDictSize++;
                            dictionary.Add((byte)curByte);
                            dictionary.Add(read.ReadByte());
                            previousByte = curByte;
                        }
                    }
                    byte padding = curByte;
                    bitCount = (encoded.Length - (realDictSize * 2 )- 2) * 8 - padding;
                    encodedBitArray = new BitArray((int)bitCount);
                    for(long p = 0; p < bitCount; ++p)
                    {
                        bool curBit = read.ReadBoolean();
                        encodedBitArray[(int)p] = curBit;
                    }
                }
            }
            List<byte> finalDictionary = new List<byte>();
            //finalDictionary.Add((byte)dictSize);
            finalDictionary.AddRange(dictionary);
            bool[] encodedArrayBit = new bool[bitCount];
            encodedBitArray.CopyTo(encodedArrayBit, 0);
            HuffmanTree huff = BuildFromDictionary(finalDictionary.ToArray());

            byte[] output = huff.DecodeSource(encodedBitArray);
            return output;
        }

        public BitArray EncodeSource(byte[] source)
        {
            List<bool> encodedSource = new List<bool>();

            for (int i = 0; i < source.Length; i++)
            {
                List<bool> encodedSymbol = this.Root.Traverse(source[i], new List<bool>());
                encodedSource.AddRange(encodedSymbol);
            }

            BitArray bits = new BitArray(encodedSource.ToArray());

            return bits;
        }

        public byte[] DecodeSource(BitArray bits)
        {
            Node current = this.Root;
            List<byte> decodedList = new List<byte>();

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    if (current.Right != null)
                    {
                        current = current.Right;
                    }
                }
                else
                {
                    if (current.Left != null)
                    {
                        current = current.Left;
                    }
                }

                if (IsLeaf(current))
                {
                    decodedList.Add(current.Symbol);
                    current = this.Root;
                }
            }

            return decodedList.ToArray<byte>();
        }
        

        public bool IsLeaf(Node node)
        {
            return (node.Left == null && node.Right == null);
        }

        static int FrequencyCompare(Node node1, Node node2)
        {
            int nodeCompare = FrequencyCompare(node1.Symbol, node1.Frequency, node2.Symbol, node2.Frequency);
            if(nodeCompare == 0)
            {
                nodeCompare = node1.uid.CompareTo(node2.uid);
            }
            return nodeCompare;
        }

        static int FrequencyCompare(KeyValuePair<byte, int> pair1, KeyValuePair<byte, int> pair2)
        {
            return FrequencyCompare(pair1.Key, pair1.Value, pair2.Key, pair2.Value);
        }

        static int FrequencyCompare(byte key1, int value1, byte key2, int value2)
        {
            int result = value1.CompareTo(value2);
            if (result == 0)
            {
                result = key1.CompareTo(key2);
            }
            return result;
        }




        public long[] GetPreOrderTreeArray(Node curNode = null, List<long> byteList = null)
        {
            if (byteList == null)
            {
                byteList = new List<long>();
            }
            if (curNode == null)
            {
                curNode = Root;
            }
            byteList.Add(curNode.Symbol * 1000000 + curNode.Frequency);
            if (curNode.Left != null)
                GetPreOrderTreeArray(curNode.Left, byteList);
            if(curNode.Right != null)
                GetPreOrderTreeArray(curNode.Right, byteList);

            return byteList.ToArray();
        }
    }

    public class Node
    {
        public byte Symbol { get; set; }
        public int Frequency { get; set; }
        public string uid = "";
        public Node Right { get; set; }
        public Node Left { get; set; }

        public List<bool> Traverse(byte symbol, List<bool> data)
        {
            // Leaf
            if (Right == null && Left == null)
            {
                if (symbol.Equals(this.Symbol))
                {
                    return data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if (Left != null)
                {
                    List<bool> leftPath = new List<bool>();
                    leftPath.AddRange(data);
                    leftPath.Add(false);

                    left = Left.Traverse(symbol, leftPath);
                }

                if (Right != null)
                {
                    List<bool> rightPath = new List<bool>();
                    rightPath.AddRange(data);
                    rightPath.Add(true);
                    right = Right.Traverse(symbol, rightPath);
                }

                if (left != null)
                {
                    return left;
                }
                else
                {
                    return right;
                }
            }
        }
    }



}