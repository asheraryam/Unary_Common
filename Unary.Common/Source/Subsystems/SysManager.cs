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

using Unary.Common.Shared;
using Unary.Common.Enums;
using Unary.Common.Interfaces;
using Unary.Common.Abstract;
using Unary.Common.Utils;

using System.Collections.Generic;
using System.Linq;
using System;

using Godot;

namespace Unary.Common.Structs
{
    public class SysManager : Node
    {
        private Dictionary<string, SysObject> Objects;
        private Dictionary<string, NodeID> Nodes;
        private Dictionary<string, SysType> Types;
        public List<string> Order { get; private set; }

        public override void _Ready()
        {
            Objects = new Dictionary<string, SysObject>();
            Nodes = new Dictionary<string, NodeID>();
            Types = new Dictionary<string, SysType>();
            Order = new List<string>();
        }

        public void Clear()
        {
            for (int i = Order.Count - 1; i >= 0; --i)
            {
                Clear(Order[i]);
                Order.RemoveAt(i);
            }
        }

        public List<ISysEntry> GetAll()
        {
            List<ISysEntry> Result = new List<ISysEntry>();

            foreach (var OrderEntry in Order)
            {
                if(Types[OrderEntry] == SysType.Object)
                {
                    Result.Add(Objects[OrderEntry]);
                }
                else
                {
                    Result.Add(GetChild<ISysEntry>(Nodes[OrderEntry].ID));
                }
            }

            return Result;
        }

        public bool Contains<T>()
        {
            return Contains(ModIDUtil.FromType(typeof(T)));
        }

        public bool Contains(string ModIDEntry)
        {
            return Order.Contains(ModIDEntry);
        }

        public SysType GetType<T>() where T : ISysEntry
        {
            return GetType(ModIDUtil.FromType(typeof(T)));
        }

        public SysType GetType(string ModIDEntry)
        {
            if(Types.ContainsKey(ModIDEntry))
            {
                return Types[ModIDEntry];
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to get type of an unregistered system " + ModIDEntry);
                return default;
            }
        }

        public void AddObject<T>(T NewSystem, string ModIDEntry = null) where T : SysObject
        {
            if(!Sys.Ref.Running)
            {
                NewSystem.Free();
                return;
            }

            if (ModIDEntry == null)
            {
                ModIDEntry = ModIDUtil.FromType(NewSystem.GetType());
            }

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                NewSystem.Free();
                Sys.Ref.ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (Types.ContainsKey(ModIDEntry))
            {
                NewSystem.Free();
                Sys.Ref.ConsoleSys.Error(ModIDEntry + " is already registered.");
                return;
            }

            try
            {
                NewSystem.Init();
            }
            catch (Exception)
            {
                NewSystem.Free();
                return;
            }
            
            Objects[ModIDEntry] = NewSystem;
            Types[ModIDEntry] = SysType.Object;
            Order.Add(ModIDEntry);
        }

        public void AddNode<T>(T NewNode, string LoadPath = null, string ModIDEntry = null) where T : SysNode
        {
            if (!Sys.Ref.Running)
            {
                NewNode.QueueFree();
                return;
            }

            if (ModIDEntry == null)
            {
                ModIDEntry = ModIDUtil.FromType(NewNode.GetType());
            }

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                NewNode.QueueFree();
                Sys.Ref.ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (Types.ContainsKey(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error(ModIDEntry + " is already registered.");
                return;
            }

            if (LoadPath != null)
            {
                NewNode.QueueFree();
                NewNode = NodeUtil.NewNode<T>(ModIDUtil.ModID(ModIDEntry), LoadPath);
            }

            try
            {
                NewNode.Init();
            }
            catch(Exception)
            {
                NewNode.QueueFree();
                return;
            }

            NewNode.Name = ModIDEntry;
            AddChild(NewNode, true);

            NodeID NewNodeID = new NodeID()
            {
                ID = GetChildCount() - 1
            };

            if (LoadPath != null)
            {
                NewNodeID.Path = LoadPath;
            }

            Nodes[ModIDEntry] = NewNodeID;

            Types[ModIDEntry] = SysType.Node;
            Order.Add(ModIDEntry);
        }

        public void AddUI<T>(T NewUI, string LoadPath = null, string ModIDEntry = null) where T : SysUI
        {
            if (!Sys.Ref.Running)
            {
                NewUI.QueueFree();
                return;
            }

            if (ModIDEntry == null)
            {
                ModIDEntry = ModIDUtil.FromType(NewUI.GetType());
            }

            if (!ModIDUtil.Validate(ModIDEntry))
            {
                NewUI.QueueFree();
                Sys.Ref.ConsoleSys.Error("Failed to validate " + ModIDEntry);
                return;
            }

            if (Types.ContainsKey(ModIDEntry))
            {
                NewUI.QueueFree();
                Sys.Ref.ConsoleSys.Error(ModIDEntry + " is already registered.");
                return;
            }

            if (LoadPath != null)
            {
                NewUI.QueueFree();
                NewUI = NodeUtil.NewNode<T>(ModIDUtil.ModID(ModIDEntry), LoadPath);
            }

            try
            {
                NewUI.Init();
            }
            catch(Exception)
            {
                NewUI.QueueFree();
                return;
            }

            NewUI.Name = ModIDEntry;
            AddChild(NewUI, true);

            NodeID NewNodeID = new NodeID()
            {
                ID = GetChildCount() - 1
            };

            if (LoadPath != null)
            {
                NewNodeID.Path = LoadPath;
            }

            Nodes[ModIDEntry] = NewNodeID;

            Types[ModIDEntry] = SysType.UI;
            Order.Add(ModIDEntry);
        }

        public ISysEntry Get<T>() where T : ISysEntry
        {
            return Get(ModIDUtil.FromType(typeof(T)));
        }

        public new ISysEntry Get(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return null;
            }

            if(Types.ContainsKey(ModIDEntry))
            {
                if(Types[ModIDEntry] == SysType.Object)
                {
                    return Objects[ModIDEntry];
                }
                else
                {
                    return GetChild<ISysEntry>(Nodes[ModIDEntry].ID);
                }
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to get type of " + ModIDEntry);
                return null;
            }
        }

        public T GetObject<T>() where T : SysObject
        {
            return (T)GetObject(ModIDUtil.FromType(typeof(T)));
        }

        public SysObject GetObject(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return null;
            }

            if (Objects.ContainsKey(ModIDEntry))
            {
                return Objects[ModIDEntry];
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to find " + ModIDEntry + " as an object");
                return null;
            }
        }

        public T GetNode<T>() where T : SysNode
        {
            return (T)GetNode(ModIDUtil.FromType(typeof(T)));
        }

        public SysNode GetNode(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return null;
            }

            if (Nodes.ContainsKey(ModIDEntry))
            {
                return GetChild<SysNode>(Nodes[ModIDEntry].ID);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to find " + ModIDEntry + " as a node");
                return null;
            }
        }

        public T GetUI<T>() where T : SysUI
        {
            return (T)GetUI(ModIDUtil.FromType(typeof(T)));
        }

        public SysUI GetUI(string ModIDEntry)
        {
            if (!ModIDUtil.Validate(ModIDEntry))
            {
                Sys.Ref.ConsoleSys.Error("Failed to verify " + ModIDEntry);
                return null;
            }

            if (Nodes.ContainsKey(ModIDEntry))
            {
                return GetChild<SysUI>(Nodes[ModIDEntry].ID);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to find " + ModIDEntry + " as an UI");
                return null;
            }
        }

        public void InitCore(Mod Mod)
        {
            foreach(var SysEntry in GetAll())
            {
                SysEntry.InitCore(Mod);
            }
        }

        public void InitMod(Mod Mod)
        {
            foreach (var SysEntry in GetAll())
            {
                SysEntry.InitMod(Mod);
            }
        }

        public void InitMods()
        {
            foreach (var Mod in Sys.Ref.Shared.GetObject<ModSys>().LoadOrder)
            {
                InitMod(Mod);
            }
        }

        public void Reload()
        {
            foreach(var OrderEntry in Order)
            {
                SysType Type = Types[OrderEntry];

                if (Type != SysType.Object)
                {
                    GetChild(Nodes[OrderEntry].ID).
                    ReplaceBy(NodeUtil.ReloadNode(Nodes[OrderEntry].Path, Sys.Ref.GetChild(Nodes[OrderEntry].ID)), false);
                    GetChild(Nodes[OrderEntry].ID)._Ready();
                }
            }
        }

        public void Clear<T>() where T : ISysEntry
        {
            Clear(ModIDUtil.FromType(typeof(T)));
        }

        //Invoked in order to clear a system
        public void Clear(string ModIDEntry)
        {
            if (Objects.ContainsKey(ModIDEntry))
            {
                Objects[ModIDEntry].Clear();
                Objects[ModIDEntry].Free();
                Objects.Remove(ModIDEntry);
                Types.Remove(ModIDEntry);
            }
            else if (Nodes.ContainsKey(ModIDEntry))
            {
                GetChild<ISysEntry>(Nodes[ModIDEntry].ID).Clear();
                GetChild(Nodes[ModIDEntry].ID).QueueFree();
                Nodes.Remove(ModIDEntry);
                Types.Remove(ModIDEntry);
            }
            else
            {
                Sys.Ref.ConsoleSys.Error("Failed to clear " + ModIDEntry);
            }
        }

        //Invoked in order to actually clear mod
        public void ClearMod(string ModIDEntry)
        {
            for (int i = Order.Count - 1; i >= 0; --i)
            {
                if (Order[i].BeginsWith(ModIDEntry + '.'))
                {
                    Clear(ModIDEntry);
                    Order.RemoveAt(i);
                }
            }
        }

        //Invoked in order to notify that a mod got cleared
        public void ClearMod(Mod Mod)
        {
            foreach (var SysEntry in GetAll())
            {
                SysEntry.ClearMod(Mod);
            }
        }

        //Invoked in order to notify that we finished clearing
        public void ClearedMods()
        {
            foreach (var SysEntry in GetAll())
            {
                SysEntry.ClearedMods();
            }
        }

        public void ClearMods()
        {
            ModSys ModSys = Sys.Ref.Shared.GetObject<ModSys>();

            if(ModSys != null)
            {
                for (int m = ModSys.LoadOrder.Count - 1; m >= 0; --m)
                {
                    Mod TargetMod = ModSys.LoadOrder[m];

                    ClearMod(TargetMod.ModID);

                    ClearMod(TargetMod);
                }

            }

            ClearedMods();
        }
    }
}
