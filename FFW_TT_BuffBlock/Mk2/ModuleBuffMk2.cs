using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FFW_TT_BuffBlock
{
    public class ModuleBuffMk2 : Module
    {
        public float Strength(int i)
        {
            return (!this.m_NeedsToBeAnchored[i] || base.block.tank.IsAnchored) ? this.m_Strength[i] : 1.0f;
        }

        public float AddAfter(int i)
        {
            return (!this.m_NeedsToBeAnchored[i] || base.block.tank.IsAnchored) ? this.m_AddAfter[i] : 0.0f;
            
        }

        private void OnAttach()
        {
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            buff.AddBuff(this);
            base.block.tank.AnchorEvent.Subscribe(new Action<ModuleAnchor, bool, bool>(this.OnAnchor));
        }

        private void OnDetach()
        {
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            buff.RemoveBuff(this);
            if (base.block.tank != null)
            {
                base.block.tank.AnchorEvent.Unsubscribe(new Action<ModuleAnchor, bool, bool>(this.OnAnchor));
            }
        }

        private void OnAnchor(ModuleAnchor anchor, bool anchored, bool fromAfterTechPopulate)
        {
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            List<string> list = new List<string>();
            for (int i = 0; i < m_BuffPath.Length; i++)
            {
                if (m_NeedsToBeAnchored[i] == true)
                {
                    List<string> splitPath = m_BuffPath[i].Split('.').ToList();
                    Type component = typeof(TankBlock).Assembly.GetType(splitPath[0]);
                    if (buff.pathToSegment.ContainsKey(m_BuffPath[i]) && buff.typeToBlock.ContainsKey(component))
                    {
                        buff.pathToSegment[m_BuffPath[i]].ManipulateObj(buff.typeToBlock[component], "UPDATE");
                    }
                }
            }
        }

        private void OnPool()
        {
            if ((this.m_Strength == null) || (this.m_Strength.Length != this.m_BuffPath.Length))
            {
                this.m_Strength = this.m_BuffPath.Select(x => 1.0f).ToArray();
            }
            if ((this.m_AddAfter == null) || (this.m_AddAfter.Length != this.m_BuffPath.Length))
            {
                this.m_AddAfter = this.m_BuffPath.Select(x => 0.0f).ToArray();
            }
            if ((this.m_NeedsToBeAnchored == null) || (this.m_NeedsToBeAnchored.Length != this.m_BuffPath.Length))
            {
                this.m_NeedsToBeAnchored = this.m_BuffPath.Select(x => false).ToArray();
            }
            base.block.AttachEvent.Subscribe(new Action(this.OnAttach));
            base.block.DetachEvent.Subscribe(new Action(this.OnDetach));
        }

        [SerializeField]
        public string[] m_BuffPath;

        [SerializeField]
        public float[] m_Strength;

        [SerializeField]
        public float[] m_AddAfter;

        [SerializeField]
        public bool[] m_NeedsToBeAnchored;
    }
}
