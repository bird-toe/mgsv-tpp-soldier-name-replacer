﻿// The code contained in this file is authored by Atvaark and comes from https://github.com/Atvaark/FoxEngine.TranslationTool which has the following License:

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

using System.Xml.Serialization;
using LangTool.Utility;

namespace LangTool.Lang
{
    [XmlType("Entry")]
    public class LangEntry
    {
        [XmlAttribute]
        public uint Key { get; set; }

        [XmlAttribute]
        public string LangId { get; set; }

        [XmlIgnore]
        public int Offset { get; set; }

        [XmlAttribute]
        public short Color { get; set; }

        [XmlAttribute]
        public string Value { get; set; }

        public bool ShouldSerializeKey()
        {
            return string.IsNullOrEmpty(LangId);
        }

        public void UpdateKey()
        {
            if (!string.IsNullOrEmpty(LangId))
            {
                Key = Fox.GetStrCode32(LangId);
            }
        }
    }
}
