The Orcs have a new ability, called 'Fast Healing'.  This ability is scaled according to the 
monster's maximum hits, so it enables recovery of damage at the same rate regardless of how 
high the mob's hitsmaxseed is set on a spawner .. ie, if the mob has 50 hp or 5000, it will
recover fully within one minute with the fast heal ability.  Keep this in mind when 
altering properties on the spawner!

Wargs are currently tamable, but that can be easily altered by editing the segment of code 
that reads Tamable = true; ect,ect to Tamable = false; ... the wargriders will still be capable 
of riding the wargs, and using their mount ability.  Mount ability is listed in time by a 
fraction of an hour ... essentially every six - 12 seconds or so, more or less, depending
upon when your engine checks for mount abilities.  Feel free to increase this value, if it seems
too overpowered for your shard.  Otherwise, the Warg is a lot like a half-strength CuSidhe.
(albeit with a trip ability).  My appologies for the limitations in this ability ... If
I could do some editing to the animations files of each player's clients, I could make
ANY mob fall down on trip ... as that is a bit much for a simpe drag and drop add, I'll
leave such custom modifications to the end user.  Simply being able to paralyze is good, too.

Since the orc features are a function of the mob's hair, not a mask or hat, western orcs CAN
be EQUIPped with headgear, although some items dont look very good with the orc face 
(it is fairly large).  Helmets, Kasas, and most hats work just fine, but things like the orchelm
don't look right, go figure, eh.

There is no referance to an oppositiongroup in this script ... if one is capable of making or 
customizing an oppositiongroup, one does not need any advice from me on how to do it.
Add it in, If you want ;)