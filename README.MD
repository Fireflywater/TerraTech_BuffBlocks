## TerraTech Buff Blocks (v1.2.2)

Adds a Module that allows for tech-wide buffs or debuffs, while it's attached to the tech. Shoot faster, drive faster, and drill harder! 

Want to add your own? Consule `MODULE IMPLEMENTATION TO JSON BLOCKS` below.

Adding your own, and jave problems with your variables? Contact me on Discord at `Fireflywater#0457`.

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
	"m_BuffPath": [<BuffPath>],
	"m_Strength": [<Strength>],
	"m_AddAfter": [<AddAfter>],
	"m_NeedsToBeAnchored": [<Anchor>],
	"m_AffectedBlockList": [<Name>],
	"m_AffectedBlockListType": [<"black"/"white">]
}
```

* Where <BuffPath> must be a string related to the target Module, a path to target value, and the target value itself. Only Floats, Ints, and Bools are supported. 

* If you have trouble ascertaining a path, use DnSpy to view the game's architecture. Example path for Weapon Rotation Speed. `"ModuleWeaponGun.m_WeaponModule.m_RotateSpeed"`

* Where <Strength> can be a float. Strength is based on a multiplier, please be careful with `0.0`, and do not enter the negatives. Unforeseen consequences may ensue. 1.0 for no change. Default 1.0.

* Where <AddAfter> can be a float. AddAfter is added after Strength, go figure. Use this to force 0s to become positives. Be careful with extreme numbers. 0.0 for no change. Default 0.0.

* Where <Anchor> can be a boolean, true or false. Dictating if the tech must be anchored to benefit from the buff. There is no "If Not Anchored" flag, unless requested. Default false.

* Where <Name> is the internal name of a block to be excluded or only included for that buff. Be advised this is only for ONE block, no lists! Default "".

* Where <"black"/"white"> is either "black" or "white". "black" means the block stated in "m_AffectedBlockList" will be ignored, and white means everything except that is ignored. Default "black".

* As arrays, you can put multiple variables in each. Matching indexes are read together. For example, the 1st m_BuffType will have the 1st m_Strength, ditto 2nd.

### CREDITS: 

* TerraTech Discord / Continued Advice and Support, Feature Requests

* RimWorld Discord / Initial Advice and Support

* FLSoz / Initial Advice and Framework

* Aceba1 / TerraTech Mod Manager Comptibility

* Rafs / Modeling and JSON Block Implementation (TBD)

### CHANGELOG:
9/5/2020, v1.2.2: 

* Fixed a bug where weapon firerate fix (recoil animation) wasn't being applied, due to new white/black list code 

9/4/2020, v.1.2.1: 

* Suppressed most console logging.

* "m_AffectedBlockList" and "m_AffectedBlockListType" functionality added, allowing buffs to ignore or only affect certain blocks. Functionality requested by Saelem Black.

7/24/2020, v1.2.0:

* Another massive refactor to remove arbitrary BuffTypes in favor of directly specified Paths.

* Anchoring will only send requests if associated m_Anchor variable is true.

* Console Logging is still enabled in the likely cash crashes are reached. Please report these to me on Discord via `Fireflywater#0457`.

7/21/2020, v1.1.1:

* Added support for Ints. When doing math, Ints are *rounded up*. 

* Added `ItemStoreCap`, allowing Silos to hold more, but their beams won't. Which leads to...

* Added `ItemHoldCap`, allowing Silos, Conveyors, and Collectors to hold more per beam.

* Added `ChargerRange`, which can increase the range of energy chargers.

* Added `ChargerSpeed`, which relates to the interval of charger zaps. In this case, lower numbers are better.

* Added a wrapper for Chargers, because they do not have an OnAttach function. This wrappper will eventually assist in removal of arbitrary BuffTypes.

7/19/2020, v1.1.0:

* Absolutely massive refactor, greatly increases speed of new variable implementation.

* Refactor prepares for future "raw" variable, allowing modders to point directly at variables.

* Recoil Animations of weapons are now consistant with WeaponCooldown buffs, allowing even faster weapon shooting.

* Added `WheelsTurnAngle` buff type. But please be careful with it...

7/17/2020, v1.0.6:

* Added `HoverForce` buff type, which can increase the weight handling capabilities of hoverpads.

* Added `HoverRange` buff type, which can increase or decrease the ride height of hoverpads.

* Added `HoverDamping` buff type, which, uh, how do I explain it?

7/16/2020, v1.0.5:

* Added `WheelsSuspension` buff type, allowing super rigid, or super loose wheel springs.

* Added `ItemProSpeed` buff type, increases the speed at which item producers produce. Especially miners.

* Refactored code so wheels refresh less often, improving performance. Especially when techs spawn with many wheels.

* Thanks to Aceba, transpiled away the error message of `ManWheels.Wheels.UpdateAttachData()`, resulting in far less output_log spam.

7/14/2020, v1.0.4:

* Anchoring now sends smaller Refresh requests, reducing On Anchor lagspikes.

* ~~Rebuilt `ManWheels.Wheels.UpdateAttachData()` to not log errors, cleaning up the Output_log spam.~~ Rollback due to Reflection's MethodInfo.Invoke() not behaving.

* m_Strength, m_AddAfter, and m_NeedsToBeAnchored now make default values if they're not stated in block JSON.

* Added broken code for damage buff, further code and implementation TBD.

* Fixed a bug where "index 0 wheels" were not affected.

7/12/2020, v1.0.3:

* Added `ItemPickupRange` buff type, increases range of collectors, SCUs, etc.

* Added `HeartAnchored` buff type, for SCUs. Boolean buffs have limited support for now.

7/12/2020, v1.0.2:

* Added `m_AddAfter` parameter, allowing further hacking of variables.

* All previously made Buff Blocks will need to be updated to include this parameter.

7/11/2020, v1.0.1:

* Suddenly want to make a changelog

* Let this be used as basis of comparison