using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FFW_TT_BuffBlock
{
    public class ModuleBuff : Module
    {
        public List<string> AllEffects
        {
            get
            {
                if (m_BuffType != null)
                {
                    return m_BuffType.ToList();
                }
                return new List<string>();
            }
        }

        public float Strength(ModuleBuff buff, int i)
        {
            return (!this.m_NeedsToBeAnchored[i] || buff.block.tank.IsAnchored) ? this.m_Strength[i] : 1.0f;
        }

        public float AddAfter(ModuleBuff buff, int i)
        {
            return (!this.m_NeedsToBeAnchored[i] || buff.block.tank.IsAnchored) ? this.m_AddAfter[i] : 0.0f;
        }

        private void OnAttach()
        {
            BuffController buff = BuffController.MakeNewIfNone(this.block.tank);
            buff.AddBuff(this);
            base.block.tank.AnchorEvent.Subscribe(new Action<ModuleAnchor, bool, bool>(this.OnAnchor));
        }

        private void OnDetach()
        {
            BuffController buff = BuffController.MakeNewIfNone(this.block.tank);
            buff.RemoveBuff(this);
            if (base.block.tank != null)
            {
                base.block.tank.AnchorEvent.Unsubscribe(new Action<ModuleAnchor, bool, bool>(this.OnAnchor));
            }
        }

        private void OnAnchor(ModuleAnchor anchor, bool anchored, bool fromAfterTechPopulate)
        {
            BuffController buff = BuffController.MakeNewIfNone(this.block.tank);
            List<string> list = new List<string>();
            list.AddRange(m_BuffType);
            list.Add("Anchor");
            buff.Update(list.ToArray());
        }

        private void OnPool()
        {
            if ((this.m_Strength == null) || (this.m_Strength.Length != this.m_BuffType.Length))
            {
                this.m_Strength = this.m_BuffType.Select(x => 1.0f).ToArray();
            }
            if ((this.m_AddAfter == null) || (this.m_AddAfter.Length != this.m_BuffType.Length))
            {
                this.m_AddAfter = this.m_BuffType.Select(x => 0.0f).ToArray();
            }
            if ((this.m_NeedsToBeAnchored == null) || (this.m_NeedsToBeAnchored.Length != this.m_BuffType.Length))
            {
                this.m_NeedsToBeAnchored = this.m_BuffType.Select(x => false).ToArray();
            }
            base.block.AttachEvent.Subscribe(new Action(this.OnAttach));
            base.block.DetachEvent.Subscribe(new Action(this.OnDetach));
        }

        public int GetEffect(string tgt)
        {
            for (int i = 0; i < m_BuffType.Length; i++)
            {
                if (m_BuffType[i] == tgt)
                {
                    return i;
                }
            }
            return 0;
        }

        [SerializeField]
        public string[] m_BuffType;
        // Valid floats: "WeaponCooldown", "WeaponRotation", "WeaponSpread", "WeaponVelocity", "DrillDps"...
        // Valid floats: "WheelsRpm", "WheelsBrake", "WheelsTorque", "WheelsGrip", "WheelsSuspension", "WheelsTurnAngle"...
        // Valid floats: "HoverForce", "HoverRange", "HoverDamping", "BoosterBurnRate"...
        // Valid floats: "ItemPickupRange", "ItemProSpeed"...

        [SerializeField]
        public float[] m_Strength;
        // Please do not go under 0.0f, negative numbers are likely dangerous.
        // For bools, use 0.0f for FALSE and 1.0f for TRUE.

        [SerializeField]
        public float[] m_AddAfter;
        // Fixed value to be added after the m_Strength multiplier.
        // Extreme values may result in dangerous behaviors.

        [SerializeField]
        public bool[] m_NeedsToBeAnchored;
        // if true, buff block needs to be anchored, else m_Strength is overridden to 1.0f.
        // if false, buff block doesn't need to be anchored.
    }
}
