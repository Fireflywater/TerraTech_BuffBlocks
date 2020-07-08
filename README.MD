## TerraTech Buff Blocks

### UNRELEASE INSTALLATION:

* Install TerraTech Mod Manager.

* Install NuTerra Block Injector.

* Clone repository.

* Move `<Cloned Repository>/QMods/BuffBlockTest/` to `<TerraTech Root>/QMods/`.

* Move `<Cloned Repository>/CustomBlocks/FFW_BuffBlock_Test/` to `<TerraTech Root>/CustomBlocks`.

* Placeholder blocks can be found In-game as GSO Accessories.

### MODULE IMPLEMENTATION TO JSON BLOCKS

* Use placeholder blocks from `<Cloned Repository>/CustomBlocks/FFW_BuffBlock_Test/` for reference.

* Add the following code to target JSON Block's `"JSONBLOCK"` key...

```
"FFW_TT_BuffBlock.ModuleBuff": {
	"m_BuffType": "<BuffType>",
	"m_Strength": <Strength>
}
```

* Where <BuffType> can be...

* "WeaponCooldown", "WeaponRotation", "WeaponSpread", "WeaponVelocity", "WheelsRPM", "WheelsBrake", "WheelsTorque"...

* "BoosterBurnRate", "ShieldRadius", "DrillDps", "EnergyOps", "EnergyStoreCap"...

* Where <Strength> can be a float. Strength is based on a multiplier, please be careful with `0.0`, and do not enter the negatives. Unforeseen consequences may ensue.

### TODO:

* Credits. (TerraTech Discord, Rimworld Discord)

* License. 

* TTMM direct installation.

* Release Example blocks, courtesy Rafs. 

* Additional values as requested.