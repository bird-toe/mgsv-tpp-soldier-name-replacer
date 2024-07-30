// The code contained in this file is authored by Atvaark and comes from https://github.com/Atvaark/FoxEngine.TranslationTool which has the following License:

/* 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2015 Atvaark
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.IO;
using System.Text;

namespace LangTool.Utility
{
    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream output) : base(output)
        {
            
        }

        public BigEndianBinaryWriter(Stream output, Encoding encoding)
            : base(output, encoding)
        {
            
        }

        public BigEndianBinaryWriter(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
            
        }

        private byte[] Reverse(byte[] b)
        {
            Array.Reverse(b);
            return b;
        }

        public override void Write(float value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(ulong value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(long value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(uint value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(int value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(ushort value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(short value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }

        public override void Write(decimal value)
        {
            int[] bits = decimal.GetBits(value);
            byte[] bytes = new byte[4 * sizeof(int)];
            Buffer.BlockCopy(bits, 0, bytes, 0, bytes.Length);
            Write(Reverse(bytes));
        }

        public override void Write(double value)
        {
            Write(Reverse(BitConverter.GetBytes(value)));
        }
    }
}