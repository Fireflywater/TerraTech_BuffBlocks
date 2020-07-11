## TerraTech Buff Blocks (v1.0.0)

Adds a Module that allows for tech-wide buffs or debuffs, while it's attached to the tech. Shoot faster, drive faster, and drill harder! 

Want to add your own? Consule `MODULE IMPLEMENTATION TO JSON BLOCKS` below.

Adding your own, and want to suggest additional variables or functionality? Contact me on Discord at `Fireflywater#0457`.

### RELEASE INSTALLATION:

* Install TerraTech Mod Manager.

* Install NuTerra Block Injector via Mod Manager.

* Install Buff Blocks via Mod Manager.

### TEMP PLACEHOLDER BLOCK INSTALLATION: 

* For placeholder blocks, clone this repository and move `<Cloned Repository>/CustomBlocks/FFW_BuffBlock_Test/` to `<TerraTech Root>/CustomBlocks`.

* Placeholder blocks can be found In-game as GSO Accessories.

### MODULE IMPLEMENTATION TO JSON BLOCKS: 

* Use placeholder blocks from `<Cloned Repository>/CustomBlocks/FFW_BuffBlock_Test/` for reference.

* Add the following code to target JSON Block's `"JSONBLOCK"` key...

```
"FFW_TT_BuffBlock.ModuleBuff": {
	"m_BuffType": "<BuffType>",
	"m_Strength": <Strength>,
	"m_NeedsToBeAnchored": <Anchor>
}
```

* Where <BuffType> can be... (Required)

* "WeaponCooldown", "WeaponRotation", "WeaponSpread", "WeaponVelocity", "WheelsRpm", "WheelsBrake", "WheelsTorque"...

* "BoosterBurnRate", "ShieldRadius", "DrillDps", "EnergyOps", "EnergyStoreCap"...

* Where <Strength> can be a float. Strength is based on a multiplier, please be careful with `0.0`, and do not enter the negatives. Unforeseen consequences may ensue. (Required)

* Where <Anchor> can be a boolean, true or false. Dictating if the tech must be anchored to benefit from the buff. There is no "If Not Anchored" flag, unless requested. (Optional, default False)

### CREDITS: 

* TerraTech Discord / Continued Advice and Support, Feature Requests

* RimWorld Discord / Initial Advice and Support

* FLSoz / Initial Advice and Framework

* Aceba1 / TerraTech Mod Manager Comptibility

* Rafs / Modeling and JSON Block Implementation (TBD)