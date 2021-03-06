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

using Unary.Common.Interfaces;
using Unary.Common.Structs;
using Unary.Common.Abstract;

using Godot;
using System.Collections.Generic;

namespace Unary.Common.Shared
{
    public class OSSys : SysNode
    {
        public override void Init()
        {
            ConfigSys ConfigSys = Sys.Ref.Shared.GetObject<ConfigSys>();

            ConfigSys.Shared.Subscribe(this, nameof(SetUseVsync), 
            "Unary.Common.Window.VSync", SubscriberType.Method);
            ConfigSys.Shared.Subscribe(this, nameof(SetWindowFullscreen), 
            "Unary.Common.Window.Fullscreen", SubscriberType.Method);
            ConfigSys.Shared.Subscribe(this, nameof(SetWindowTitle), 
            "Unary.Common.Window.Title", SubscriberType.Method);
            ConfigSys.Shared.Subscribe(this, nameof(SetWindowSize), 
            "Unary.Common.Window.Size", SubscriberType.Method);
        }

        public void SetUseVsync(bool Value)
        {
            OS.VsyncEnabled = Value;
        }

        public void SetWindowFullscreen(bool Value)
        {
            OS.WindowFullscreen = Value;
        }

        public void SetWindowTitle(string Value)
        {
            OS.SetWindowTitle(Value);
        }

        public void SetWindowSize(Vector2 NewSize)
        {
            OS.WindowSize = NewSize;
        }
    }
}
