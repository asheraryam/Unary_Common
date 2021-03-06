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
using Unary.Common.Shared;
using Unary.Common.Utils;
using Unary.Common.Structs;
using Unary.Common.Abstract;

using System;
using System.Collections.Generic;

namespace Unary.Common.Shared
{
    class EnvironmentSys : SysObject
    {
        private const string FolderPath = "Environment/";
        private Dictionary<string, string> EnvPaths;

        public override void Init()
        {
            EnvPaths = new Dictionary<string, string>();

            List<string> EnvFolders = FilesystemUtil.Sys.DirGetDirs(FolderPath);

            foreach(var Folder in EnvFolders)
            {
                string ModID = System.IO.Path.GetFileNameWithoutExtension(Folder);
                EnvPaths[ModID] = Folder;
            }
        }

        public override void Clear()
        {
            DataPaths.Clear();
        }

        public override void ClearMod(Mod Mod)
        {
            if (DataPaths.ContainsKey(Mod.ModID))
            {
                DataPaths.Remove(Mod.ModID);
            }
        }

        public override void InitCore(Mod Mod)
        {
            if(!FilesystemUtil.Sys.DirExists(FolderPath + Mod.ModID))
            {
                FilesystemUtil.Sys.DirCreate(FolderPath + Mod.ModID);
            }
            DataPaths[Mod.ModID] = FolderPath + Mod.ModID;
        }

        public override void InitMod(Mod Mod)
        {
            if (!FilesystemUtil.Sys.DirExists(FolderPath + Mod.ModID))
            {
                FilesystemUtil.Sys.DirCreate(FolderPath + Mod.ModID);
            }
            DataPaths[Mod.ModID] = FolderPath + Mod.ModID;
        }

        public string GetPath(string ModID)
        {
            if(DataPaths.ContainsKey(ModID))
            {
                return DataPaths[ModID];
            }
            else
            {
                return null;
            }
        }
    }
}
