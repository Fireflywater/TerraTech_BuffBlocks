using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace FFW_TT_BuffBlock
{
    class ModuleBuffWrapperMk2 : Module
    {
        private void OnAttach()
        {
            /*ModuleWeaponGun guntest = base.block.GetComponent<ModuleWeaponGun>();
            if (guntest)
            {
                Console.WriteLine("Successfully got gun");
            }*/

            /*TankBlock blocktest = ManSpawn.inst.GetBlockPrefab((BlockTypes)block.visible.ItemType);
            if (blocktest.name == pointer.name)
            {
                Console.WriteLine("Successfully got prefab");
            }*/
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            buff.AddBlock(pointer);
        }

        private void OnDetach()
        {
            BuffControllerMk2 buff = BuffControllerMk2.MakeNewIfNone(this.block.tank);
            buff.RemoveBlock(pointer);

        }

        public void Init()
        {
            FieldInfo field_Block = typeof(Module).GetField("_block", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            field_Block.SetValue(this, pointer);
            base.block.AttachEvent.Subscribe(new Action(this.OnAttach));
            base.block.DetachEvent.Subscribe(new Action(this.OnDetach));
        }

        [SerializeField]
        public TankBlock pointer;
    }
}
