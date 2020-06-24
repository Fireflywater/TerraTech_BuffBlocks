TerraTech Buff Blocks

~~Only RATE OF FIRE functions.
Be aware this is not packaged for implementation, not yet.

Plesae assist in making `FFW_TT_BuffBlock.BuffController.RefreshWheels(ModuleWheels, float)` work.

What I want to do is...~~
```
RefreshWheels(ModuleWheels wheels, float rpm)
{
	ManWheels.TorqueParams torque = wheels.m_TorqueParams;
	torque.torqueCurveMaxTorque = rpm;
	foreach (ManWheels.Wheel wheel in wheels)
	{
		Singleton.Manager<ManWheels>.inst.m_WheelState[wheel.attachedID].torqueParams = torque;
	}

}
```

~~Reflection is going over my head. Please use an assembly editor or view like dnSpy for TerraTech's source code.~~

Success! No help in this context neccessary... not yet, at least.