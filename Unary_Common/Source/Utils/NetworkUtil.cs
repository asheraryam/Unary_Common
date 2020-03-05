﻿/*
MIT License

Copyright (c) 2020 Unary Incorporated

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Unary_Common.Structs;
using System;
using System.Collections.Generic;

using MessagePack;

namespace Unary_Common.Utils
{
    public static class NetworkUtil
    {
        public static byte[] Pack(Arguments.Arguments Arguments)
        {
            return MessagePackSerializer.Serialize(Arguments, 
			MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray));
        }

        public static Arguments.Arguments Unpack(byte[] Bytes)
        {
            return MessagePackSerializer.Deserialize<Arguments.Arguments>(Bytes, 
			MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray));
        }
    }
}