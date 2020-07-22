using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace FFW_TT_BuffBlock
{
    class ModuleRemoteChargerWrapper : Module
    {
        private void OnAttach()
        {
            BuffController buff = BuffController.MakeNewIfNone(this.block.tank);
            if (this.pointer.GetType() == typeof(ModuleRemoteCharger))
            {
                buff.AddCharger((ModuleRemoteCharger)this.pointer);
            }
        }

        private void OnDetach()
        {
            BuffController buff = BuffController.MakeNewIfNone(this.block.tank);
            if (this.pointer.GetType() == typeof(ModuleRemoteCharger))
            {
                buff.RemoveCharger((ModuleRemoteCharger)this.pointer);
            }
        }

        private void PrePool()
        {
        }

        public void OnPool()
        {
            FieldInfo field_Block = typeof(Module).GetField("_block", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            field_Block.SetValue(this, pointer.block);
            base.block.AttachEvent.Subscribe(new Action(this.OnAttach));
            base.block.DetachEvent.Subscribe(new Action(this.OnDetach));
        }

        [SerializeField]
        public ModuleRemoteCharger pointer;
    }
}
