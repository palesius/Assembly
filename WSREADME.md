This adds batch editing features to Assembly.

Currently the path to the template files is hardcoded to:
g:\th\reach\wsset\
in WalkingSimulator.cs
this can be changed to whatever you desire.

To Use:
1) Set up a template file.
2) Open all maps you want to edit.
3) Select "Walking Sim" from the menu. It will backup each map and apply the changes from the template. If it isn't able to find some of the tags, the info will be copied to the clipboard at the end of the process so you can investigate. Sometimes it is legitimate (applying a tag to a level where it never occurs). Sometimes it is because the tag definitions have changed and the template needs to be updated.
4) "WS Complete" will use GenPat to create a patch file for each open map using GenPat. (Path currently hardcoded to G:\th\reach\VPatch32\GenPat.exe). Maximum instances of GenPat is currently set to 4, but you can adjust if you have more or less memory. (It's a fairly memory intensive process, taking about twice as much memory as the size of the map file.)

Template Files:
These are named based on the game engine with a .txt extension (e.g. halo1.txt halo3odst.txt). You may also create a level specific file that will overlay the base template for that game engine.

Here are some representative lines for halo1:
matg|globals\globals|float32:429:Airborne Acceleration:5
bipd|characters\cyborg\cyborg|flags32:733:Flags:+7
weap|*|int16:742:Rounds Total Initial:32000

They are pipe delimited tokens are as follows:
1) tag type it applies to
2) entity it applies to, the wildcards here can only be used at the end of the entity name, and will apply to anything with the same beginning you entered. (e.g. * applies to all entities with that tag, objects\characters\dervish\* would applied to all entities starting with objects\characters\dervish\ )
3) tag identification and value. This is colon separated
	a) data type as per the assembly template
	b) line number in the assembly template. This is the line number of the tag, not of the enum value or flag.
	c) tag name in assembly template
	d) value to change to. For flags a - or + in front of it will unset or set a particular flag respectively.

