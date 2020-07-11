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
        public float Strength
        {
            get
            {
                return (!this.m_NeedsToBeAnchored || this.block.tank.IsAnchored) ? this.m_Strength : 1.0f;
            }
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
            buff.Update(new string[] { "All" });
        }

        private void OnPool()
        {
            base.block.AttachEvent.Subscribe(new Action(this.OnAttach));
            base.block.DetachEvent.Subscribe(new Action(this.OnDetach));
        }

        [SerializeField]
        public string m_BuffType;
        // Valid floats: "WeaponCooldown", "WeaponRotation", "WeaponSpread", "WeaponVelocity", "WheelsRpm", "WheelsBrake", "WheelsTorque"...
        // Valid floats: "BoosterBurnRate", "ShieldRadius", "DrillDps", "EnergyOps", "EnergyStoreCap"...
        // Valid bools: "ItemConAnchored"...

        [SerializeField]
        public float m_Strength;
        // Please do not go under 0.0f, negative numbers are likely dangerious.
        // For bools, use 0.0f for FALSE and 1.0f for TRUE

        [SerializeField]
        public bool m_NeedsToBeAnchored = false;
        // if true, buff block needs to be anchored, else m_Strength is overridden to 1.0f
        // if false, buff block doesn't need to be anchored
    }
}
