using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace FFW_TT_BuffBlock
{
    class BuffSpecificFix
    {
        public static void RefreshWheels(List<TankBlock> blockList)
        {
            FieldInfo field_TorqueParams = typeof(ModuleWheels) // Maybe I should find a way to compress these?
                .GetField("m_TorqueParams", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_Wheels = typeof(ModuleWheels)
                .GetField("m_Wheels", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_WheelParams = typeof(ModuleWheels)
                .GetField("m_WheelParams", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_AttachedId = typeof(ManWheels.Wheel)
                .GetField("attachedID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_WheelState = typeof(ManWheels)
                .GetField("m_WheelState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Console.WriteLine("FFW! Update Wheels. Count: " + blockList.Count);
            foreach (TankBlock block in blockList)
            {
                ModuleWheels wheels = block.GetComponent<ModuleWheels>();
                ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels); // Read active Torque... 
                ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels); // Read active WheelParams... 

                List<ManWheels.Wheel> value_Wheels = (List<ManWheels.Wheel>)field_Wheels.GetValue(wheels);
                foreach (ManWheels.Wheel wheel in value_Wheels)
                {
                    Array value_WheelState = (Array)field_WheelState.GetValue(Singleton.Manager<ManWheels>.inst);

                    int value_WheelAttachedId = (int)field_AttachedId.GetValue(wheel); // Important, determines what AttachedWheelState is associated
                    if (value_WheelAttachedId > -1) // value is -1 if it's unloaded, I think...
                    {
                        object value_AttachedWheelState = value_WheelState.GetValue(value_WheelAttachedId); // AttachedWheelState is a PRIVATE STRUCT, `object` neccessary
                        FieldInfo field_p_TorqueParams = value_AttachedWheelState.GetType() // Get types of private struct...
                            .GetField("torqueParams", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        FieldInfo field_p_WheelParams = value_AttachedWheelState.GetType()
                            .GetField("wheelParams", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        field_p_TorqueParams.SetValue(value_AttachedWheelState, torque); // Apply new Torque to ManWheels.Wheel
                        field_p_WheelParams.SetValue(value_AttachedWheelState, wheelparams); // Apply new WheelParams...

                        FieldInfo field_p_Inertia = value_AttachedWheelState.GetType()
                            .GetField("inertia", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        ManWheels.WheelParams value_WheelParams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels); // Note: Keep these incase inertia read causes issues
                                                                                                                             //float i = wheels.block.CurrentMass * 0.9f / (float)value_Wheels.Count * value_WheelParams.radius * value_WheelParams.radius;
                        ModuleWheels.AttachData moduleData = new ModuleWheels.AttachData(wheels, (float)field_p_Inertia.GetValue(value_AttachedWheelState), value_Wheels.Count);

                        wheel.UpdateAttachData(moduleData); // Update it! Live! Do it!

                    }
                }
            }
        }

        public static void ManipulateBarrels(List<TankBlock> blockList, string request, Dictionary<CannonBarrel, float> memory, float average)
        {
            FieldInfo field_NumCannonBarrels = typeof(ModuleWeaponGun) // Holy mess!
                .GetField("m_NumCannonBarrels", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_CannonBarrels = typeof(ModuleWeaponGun)
                .GetField("m_CannonBarrels", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_BarrelTransform = typeof(ModuleWeaponGun)
                .GetField("m_BarrelTransform", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_FiringData = typeof(ModuleWeaponGun)
                .GetField("m_FiringData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_WeaponModule = typeof(ModuleWeaponGun)
                .GetField("m_WeaponModule", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_ShotCooldown = typeof(ModuleWeaponGun)
                .GetField("m_ShotCooldown", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_transform = typeof(Transform)
                .GetField("transform", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_AnimState = typeof(CannonBarrel)
                .GetField("animState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            /*MethodInfo method_Setup = typeof(CannonBarrel)
                .GetMethod("Setup", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo method_CapRecoilDuration = typeof(CannonBarrel)
                .GetMethod("CapRecoilDuration", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);*/

            foreach (TankBlock block in blockList)
            {
                ModuleWeaponGun weapon = block.GetComponent<ModuleWeaponGun>();
                int value_NumCannonBarrels = (int)field_NumCannonBarrels.GetValue(weapon);
                if (value_NumCannonBarrels != 0)
                {
                    Array value_CannonBarrels = (Array)field_CannonBarrels.GetValue(weapon);
                    for (int i = 0; i < value_CannonBarrels.Length; i++)
                    {
                        /*Transform value_BarrelTransform = (Transform)field_BarrelTransform.GetValue(weapon);
                        if (value_BarrelTransform == null) // Will this ever check true?
                        {
                            field_BarrelTransform.SetValue(weapon, field_transform.GetValue(value_CannonBarrels.GetValue(i)));
                        }*/
                        CannonBarrel thisBarrel = (CannonBarrel)value_CannonBarrels.GetValue(i);
                        FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                        ModuleWeapon value_WeaponModule = (ModuleWeapon)field_WeaponModule.GetValue(weapon);
                        float value_ShotCooldown = (float)field_ShotCooldown.GetValue(weapon);
                        //method_Setup.Invoke(value_CannonBarrels.GetValue(i), new object[] { value_FiringData, value_WeaponModule });
                        //method_CapRecoilDuration.Invoke(value_CannonBarrels.GetValue(i), new object[] { value_ShotCooldown });
                        object value_AnimState = field_AnimState.GetValue(thisBarrel);
                        if (value_AnimState != null)
                        {
                            PropertyInfo prop_GetSpeed = value_AnimState.GetType()
                                .GetProperty("speed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
                            PropertyInfo prop_SetSpeed = value_AnimState.GetType()
                                .GetProperty("speed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty);
                            PropertyInfo prop_GetLength = value_AnimState.GetType()
                                .GetProperty("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
                            PropertyInfo prop_SetLength = value_AnimState.GetType()
                                .GetProperty("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty);
                            if (request == "SAVE")
                            {
                                memory.Add(thisBarrel, (float)prop_GetSpeed.GetValue(value_AnimState));
                            }
                            else if (request == "UPDATE")
                            {
                                prop_SetSpeed.SetValue(value_AnimState, memory[thisBarrel] / average);
                            }
                            else if (request == "CLEAN")
                            {
                                prop_SetSpeed.SetValue(value_AnimState, memory[thisBarrel]);
                                memory.Remove(thisBarrel);
                            }
                        }

                    }
                }
            }
        }
    }
}
