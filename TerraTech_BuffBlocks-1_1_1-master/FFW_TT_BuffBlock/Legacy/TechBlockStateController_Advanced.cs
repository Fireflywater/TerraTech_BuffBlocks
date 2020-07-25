/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFW_TT_BuffBlock
{
    class TechBlockStateController_Advanced : TechBlockStateController
    {
        public float ffw_ShotCooldownMult
        {
            get
            {
                if (this.m_WeaponModules.Count > 0)
                {
                    return this.m_WeaponModules[0].ffw_ShotCooldownMult; //ffw_ShotCooldownMult
                }
                return 1f;
            }
            set
            {
                foreach (ModuleWeapon moduleWeapon in this.m_WeaponModules)
                {
                    moduleWeapon.ffw_ShotCooldownMult = value; //ffw_ShotCooldownMult
                }
            }
        }
        public void DeregisterKillSwitch(BlockControllerModuleTypes module)
        {
            if (this.m_RegisteredKillSwitches[(int)module] == 0)
            {
                if (module == BlockControllerModuleTypes.Hover)
                {
                    this.ffw_ShotCooldownMult = 1f;
                    return;
                }
            }
        }
        public void AddWeapon(ModuleWeapon weapon)
        {
            this.m_WeaponModules.Add(weapon);
        }

        public void Removeweapon(ModuleWeapon weapon)
        {
            this.m_WeaponModules.Remove(weapon);
        }

        private List<ModuleWeapon> m_WeaponModules = new List<ModuleWeapon> ();
    }
}
*/