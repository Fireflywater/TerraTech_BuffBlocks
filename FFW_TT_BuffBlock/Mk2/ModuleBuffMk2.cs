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
        /*public List<string> AllEffects
        {
            get
            {
                if (m_BuffType != null)
                {
                    return m_BuffType.ToList();
                }
                return new List<string>();
            }
        }*/

        public float Strength(int i)
        {
            //return (!this.m_NeedsToBeAnchored[i] || buff.block.tank.IsAnchored) ? this.m_Strength[i] : 1.0f;
            return this.m_Strength[i];
        }

        public float AddAfter(int i)
        {
            //return (!this.m_NeedsToBeAnchored[i] || buff.block.tank.IsAnchored) ? this.m_AddAfter[i] : 0.0f;
            return this.m_AddAfter[i];
        }

        private void OnAttach()
        {
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            buff.AddBuff(this);
            //base.block.tank.AnchorEvent.Subscribe(new Action<ModuleAnchor, bool, bool>(this.OnAnchor));
        }

        private void OnDetach()
        {
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            buff.RemoveBuff(this);
            /*if (base.block.tank != null)
            {
                base.block.tank.AnchorEvent.Unsubscribe(new Action<ModuleAnchor, bool, bool>(this.OnAnchor));
            }*/
        }

        /*private void OnAnchor(ModuleAnchor anchor, bool anchored, bool fromAfterTechPopulate)
        {
            BuffController buff = BuffController.MakeNewIfNone(this.block.tank);
            List<string> list = new List<string>();
            for (int i = 0; i < m_BuffType.Length; i++)
            {
                if (m_NeedsToBeAnchored[i] == true)
                {
                    list.Add(m_BuffType[i]);
                }
            }
            //list.AddRange(m_BuffType);
            list.Add("Anchor");
            buff.Update(list.ToArray());
        }*/

        private void OnPool()
        {
            base.block.AttachEvent.Subscribe(new Action(this.OnAttach));
            base.block.DetachEvent.Subscribe(new Action(this.OnDetach));
        }

        public int GetEffect(string tgt)
        {
            for (int i = 0; i < m_BuffComponent.Length; i++)
            {
                if (m_BuffComponent[i] == tgt)
                {
                    return i;
                }
            }
            return 0;
        }

        [SerializeField]
        public string[] m_BuffComponent;

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
