# SkillBallPlus.cs
Add this script to your custom scripts folder.

To add in game:
[add SkillBallPlus

Can be added to player's backback on character creation also, add this to CharacterCreation.cs

PackItem(new SkillBallPlus());

Edit SkillBallPlus.cs to set default skills/level

	public static int skillsToBoost = 5;  // How many skills to boost
	public static double boostValue = 85;  // How high to boost each selected skill

These values can also be set in game by GM

By default, SkillBallPlus is locked down, so players cannot trade .

Check this thread for support and information:

https://www.servuo.com/archive/skillballplus-cs.773/
