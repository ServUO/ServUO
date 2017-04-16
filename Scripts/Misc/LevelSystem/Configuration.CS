using System;
using System.Collections;
using Server;

namespace Server
{
    //This File Is Where You Setup The System
    //It Is Very Customizable with good comments.
    public class Configured
    {
        public const Active Curr = Active.Enviroment;
        // -<  Off | Classic | Enviroment | PvP >-

        public const View Cnfg = View.BelowName;
        // -< None | InName | BelowName >-


        public int StartMaxLvl = 100;   //What is the highest level reachable without scrolls?
        public int EndMaxLvl = 200;     //What is the highest level reachable with scrolls?
        public int PartyRange = 250;     //When parties share exp, how close do they need to be to get it?


        public bool PetKillGivesExp = true;     //When players pet kills something player gets exp
        public bool CraftGivesExp = true;       //A sucessful craft gives players exp.
        public bool PartyExpShare = true;       //Do parties share exp?
        public bool PartySplitExp = true;       //if parties share exp do they split it evenly?
        //EX: kill gives 30xp 3 people in party each person gets 10 xp, even split.

        public bool AdvancedSkillExp = false;   //Only fighting skills give exp?
        public bool AdvancedExp = false;        // Use tables to give exp off of killed.
        // ^ NOT RECOMENDED FOR BEGINNERS!! ^


        public bool StaffHasLevel = true;   //Do Staff Display A Level?
        public bool PaperdollLevel = true;  //Show Players Level In Paperdoll?
        public bool MaxLevel = false;       //Show the max level? ex: 86/100
        public bool CreatureLevels = false;  //Do creatures have levels?
        public bool VendorLevels = false;   //When CreatureLvls is true, do vendors display a level also?
        public bool BonusStatOnLvl = false;  //On Level Give A Chance To Get A Bonous Stat?
        public bool RefreshOnLevel = true;  //Sets players hits, stam, and mana to max on level.


        #region Mount Options

        //Place The Level Mounts Require To Ride
        //For any mount that doesnt require a level -
        // leave it as a '0'

        public int ToMountLevel = 0; //Set Every mount rideable at this level

        //For Individual Levels:
        public int Beetle = 0;
        public int DesertOstard = 0;
        public int FireSteed = 0;
        public int ForestOstard = 0;
        public int FrenziedOstard = 0;
        public int HellSteed = 0;
        public int Hiryu = 0;
        public int Horse = 0;
        public int Kirin = 0;
        public int LesserHiryu = 0;
        public int NightMare = 0;
        public int RidableLlama = 0;
        public int Ridgeback = 0;
        public int SavageRidgeback = 0;
        public int ScaledSwampDragon = 0;
        public int SeaHorse = 0;
        public int SilverSteed = 0;
        public int SkeletalMount = 0;
        public int SwampDragon = 0;
        public int Unicorn = 0;

        #endregion

    }

    #region  !!! DO NOT CHANGE ANY THING BELOW THIS POINT !!!
    public class Off
    {
        public static bool Enabled { get { return Configured.Curr == Active.Off; } }
    }
    public class Cl
    {
        public static bool Enabled { get { return Configured.Curr == Active.Classic; } }
    }
    public class En
    {
        public static bool Enabled { get { return Configured.Curr == Active.Enviroment; } }
    }
    public class PvP
    {
        public static bool Enabled { get { return Configured.Curr == Active.PvP; } }
    }
    public class None
    {
        public static bool Enabled { get { return Configured.Cnfg == View.None; } }
    }
    public class InName
    {
        public static bool Enabled { get { return Configured.Cnfg == View.InName; } }
    }
    public class BelowName
    {
        public static bool Enabled { get { return Configured.Cnfg == View.BelowName; } }
    }

    public enum Active
    {
        Off,
        Classic,
        Enviroment,
        PvP
    }

    public enum View
    {
        None,
        InName,
        BelowName
    }
    #endregion
}