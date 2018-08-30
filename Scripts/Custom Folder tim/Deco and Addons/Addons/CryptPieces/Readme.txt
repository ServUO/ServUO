--- Dungeon Maker Beta.3, made by Kingsmidgens/Ribald Ron for RunUO! ---

Table of Contents (do I really need this?)

0.5 *NEW! UPDATES!*
1. Introduction/TYs
2. Setup/Usage
3. Building a dungeon
4. Suggestions
5. FAQ (soon to come, if I get enough feedback!)



0.5 *NEW! UPDATES!*

Included in version "Beta.3" is of course, the completed set of wrapped
bodies inside of coffins, sarcophagi and caskets.  Thanks for posting, 
Pyro, and reminding me that people still check it out.  Also included is
a bunch of artifacts that have a "crypt" feel to em, not quite as powerful
as "minor" arties, but they're still pretty cool I think.  If anyone asks me
to, i'll probably make some custom crypt-like monsters, too!


1. Introduction/TYs

Hello everyone, and thanks for downloading my scripts :) I'm pretty sure 
they're free of bugs, they're just addons after all. This is a tileset 
of a crypt, including many pieces for quick setup of dungeon(s) for use
in events, or just everyday hunting!  Thanks go to A_Li_N for giving me
tips on how to write this walkthru and helping me to make sure it will
run nice and clean on a new server (hey, I wanted it to be professional 
like!) and TMSTKSBK for telling me how and where to post scripts (I
didn't really want to be banned for posting in the wrong place!)
Btw, thanks for all the + karma and messages linked with such!
Please, someone, leave me some feedback!


2. Setup/Usage

This script is pretty easy to set up.. basically, you just need to
unzip and run!  Now, I don't claim to be any sort of scripting
Wizard, but I could make a GUMP if I wanted to spend a lot of time
on this, which I don't, to be honest.  I simply wanted a way to
build dungeons much faster than by hand, and I decided that if it
turns out well i'll release it, and it works great from my standpoint.
Basically, you can just type [add crypt for a list of all the pieces!
All you need to do is click on the arrow, then hit ESC when you're
ready to select another piece (the GUMP comes back up)  Included
are several pieces I feel should be explained:  #x#Stair#, CryptCorner,
CryptWallNW and CryptEntrance.  First you should understand how the numbers work..
The first number is the length of the piece East to West, and the second
is North to South, just like custom housing.

Pieces included:

[add Crypt#x# (a #-by-# section of Stone Flooring)
(2x4, 2x8, 3x5, 4x2, 4x4, 4x8, 5x3, 5x5, 6x6, 7x7, 8x2, 8x8, 9x9, 10x10)

[add Crypt2x5StairN/S (a 2-by-5 section of stairs leading north or south)
[add Crypt5x2StairE/W (a 5-by-2 section of stairs leading east or west)
[add CryptCorner (a corner of wall section)
[add CryptWallN/S (a piece of wall section facing north or south)
[add CryptWallN/S# (3 or 5 crypt walls facing north or south)
[add CryptWallNW (an L shaped crypt wall for making corners)
[add CryptWEntrance16x9 (16-by-9 crypt, with stairs leading down and west)
[add SecretCryptDoorNS/EW (special hidden door that resembles a wall!)
[add CryptLeverNS/EW (lever that will open a door, even a locked one)
[add *many many kinds of coffins, caskets etc* (you get the idea, ty for suggestions)



That said, the Stair piece needs a little explanation.. For example, 
the 5x2StairW is a pre-built5x2 stair (duh).. the W simply means that the 
elevated side is West, meaning you'd need to build a platform West of it 
to connect to ("[set z 25" on a floor section, one of the things i'm enjoying 
about multis compared to statics) 

Also, the CryptWall pieces are fairly simple, but might be confusing to
some.  the CryptWall1-5 are just pieces of wall, built one tile away from the
one you select.  I added them because it takes a little extra time to place
walls while above ground, because you need to either make them at 0 Z, or
set the X or Y so that they're not blocking you.

The CryptCorner and CryptWallNW pieces are pretty simple as well.  The
CryptCorner is a pre-built three piece corner, including W, N and N/W
wall segments.  The CryptWallNW is just the L-shaped wall that you use
to connect a N and a W piece at an intersection.

There's the 16x9 crypt entrance, also.  Obviously,
the addon is 16x9 tiles, but what is it?  It's an entrance to a crypt,
of course!  It has stairs, going up by 15 Z, including a small iron fence
to keep intruders out.  There's a staircase that leads down and West, and
it would be an ideal spot to set up with teleporters to link your crypt
to the outside world.

The secret door can be a neat thing to mess with, giving you the option
of adding any number of secret areas to your crypt.  You can even lock it,
and use it in conjunction with the CryptLever to make an even more complicated
secret area to find. ;)



3. Building a dungeon

Alright, ready to start building the dungeon?  I'm assuming since you're
still reading this, you are.  Find the spot for your dungeon and begin
building! Anyway, once you've typed [add crypt, and brought up the add item 
menu, you're ready to begin (or you can just type [add crypt3x3, etc.. but 
that takes so long!) Simply place the segment, and even i'm not sure yet how 
to place them exatly, but that's what the [move command is for.  Just click 
the edge, and then re-click where you want it to meet the other room.. nifty, 
eh? You can build a fairly sizable dungeon fairly quickly.  Just be sure that
there's a way to get inside of your dungeon, and something to do down there, and
your players should be happy with it!  Included is the giant 16x9 CryptEntrance,
it's an ideal link to the outside world!  Hope you enjoy :)


4. Suggestions

Some people like to build a small teleport-in room, so that
people won't be killed upon entering, but that's completely up to you.

The 2x8/8x2 segments can be used for hallways or additions to make a room
a little bigger, that's your call!

Wall pieces can be added for extra effect,
either statics or the wall segments that I have included.

Add a lot of twists, extra levels and side chambers to your dungeon,
this will add to the size of the dungeon without adding to the space
it takes up.

When using a staircase, you can build under the second story you've added
to make a little more adventuring area to the dungeon, without taking
up extra space.. if you wish, you can [add blocker to prevent players
from jumping from the second story to the first one, simply place the
blocker where you don't want the players to move, and set the z to 25.
They'll still be able to walk under them.

Upper stories blocking visibility of first story?  Yeah, this gets annoying..
You can add segments on top of the first, second, etc story, and set their Z
to the upper levels.  Players walking under them will no longer have limited
visibility!  If you don't want players on the upper floors to be able to walk
there, you can place walls.. you can also set the hue of the segment(s) to 1,
blending them in with the black background (if you're building in the blackness)

Square rooms are all fine and good, but adding a smaller room to a larger
one adds some more character and uniqueness to a certain room.. try it!

Decorations go a long way, but i'm just providing you with an outline.
Decorate how you see fit, but be warned that excessive decoration
can get annoying and laggy, a deadly combination for players.

You can [set hue #### to change the color of pieces (ive mentioned this
a few times already! :p).. all of the pieces in a segment will hue
the same color, allowing you to make a noticeable difference in
any dungeons you may have.

The crypt entrance can be decorated well, with some cracks in the stone and
vines climbing up the walls.  You can also add a portcullis or door to the
front for an added effect.  Use of the regions-in-a-box to make an area around
the crypt seem darker is also very cool!

Use of the [link command to link a lever to a locked secret door can add some
fun for your players, just make sure they don't get stuck in the room!.. You
can use the [chainlink command to add multiple levers and doors, or simply
add a teleporter in the locked room to prevent this.  Also, if you wish, you
can alter the opened/closedid of the lever, and make it a deco item so that
players have to hunt for an even longer time for the secret chamber.


5: FAQ

Q:  You've said a lot of basic information that I already know in
    this FAQ, are you assuming i'm a newbie?

A:  Not at all.  A lot of people don't know these things, and that
    hardly makes them newbies anyway!  I'm simply throwing in
    tidbits that might increase the quality or ease of the dungeons
    people create with my script. ^^


Q: Okay, so, why is this "Beta"? It works fine for me!

A: It may work fine, but i'm not sure how i'll be adding on to
   the system, if at all.. until I know, it won't be a full
   release!


Q: I have a suggeston, should I PM or email you?

A: Yeah, you can.. as many of you have.. but i'd rather you create a post
   in the forum so that I can stop deleting your PMs :P (I can only hold
   20, after all) also, people may see it and it may spark an idea they
   have as well.


Q: I hate thanksgiving.

A: What?   Yes, I really got this email because of my script. (somehow)