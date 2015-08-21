using System;
using Server;

namespace Server
{
	public class LevelItems
	{
		//These are the definable features for item leveling...
        public static readonly int DefaultMaxLevel = 200; //Default Max level for items.
        public static readonly int MaxLevelsCap = 1000; //Number of total levels items can go up to when maxed.
        public static readonly bool EnableExpCap = true; //true = Cap experience per level.  false = no cap.
        public static readonly bool DisplayExpProp = true; //true = Display experience on item onmouseover/click.

        //These are the definable features for spending points...
        public static readonly int PointsPerLevel = 4; //How many spending points an item gets per level.
        public static readonly bool DoubleArtifactCost = true; //true = Artifact attributes will cost double points.

        //These are the definable features for Level Increasing...
        public const bool BlacksmithOnly = true; //true = May only be used by char with blacksmithy.
        public const double BlacksmithSkillRequired = 100; // Amount of Blacksmith skill required to validate deeds (if BlacksmithOnly is true).
        public const bool RewardBlacksmith = true; //true = give the blacksmith reward for validating (if BlacksmithOnly is true).
        public const int BlacksmithRewardAmt = 500; // Amount to pay blacksmith if they validate deed for another player (if BlacksmithOnly and RewardBlacksmith are true).
	}
}