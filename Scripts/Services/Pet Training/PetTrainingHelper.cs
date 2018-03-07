using System;
using Server;
using System.Collections.Generic;
using System.Linq;

// Notes:
/* 1. combo of 3. Once 3 is chosen, only magical abilty can replace another magical ability.
 * 2, combat check when adding stuff 1156876
 * 
 * 
 * 
 */

namespace Server.Mobiles
{
    [Flags]
    public enum Stats
    {
        Str,
        Dex,
        Int,
        Hits,
        Stam,
        Mana,
        RegenHits,
        RegenStam,
        RegenMana,
        BaseDamage
    }

    [Flags]
    public enum Class
    {
        None        = 0x00000000,
        Magical     = 0x00000001,
        Necromantic = 0x00000002,
        Tokuno      = 0x00000004,
        StickSkin   = 0x00000008,
        Clawed      = 0x00000010,
        Tailed      = 0x00000020,
        Insectoid   = 0x00000040,
        Restricted  = 0x00000080,

        MagicalAndNecromantic = Magical | Necromantic,
        MagicalAndClawed = Magical | Clawed,
        MagicalAndTailed = Magical | Tailed,
        ClawedAndTailed = Clawed | Tailed,
        MagicalAndInsectoid = Magical | Insectoid,
        StickySkinAndTailed = StickySkinn | Tailed,
        TailedAndNecromantic = Tailed | Necromantic,

        ClawedNecromanticAndTokuno = Clawed | Necromantic | Tokuno,
        MagicalClawedAndTailed = MagicalAndClawed | Tailed,
        MagicalNecromanticAndTokuno = MagicalAndNecromantic | Tokuno,
        ClawedTailedAndTokuno = ClawedAndTailed | Tokuno,
        ClawedTailedAndNecromantic = ClawedAndTailed | Necromantic,
        
        MagicalClawedTailedAndNecromantic = MagicalClawedAndTailed | Necromantic,
        ClawedTailedNecromanticAndTokuno = ClawedNecromanticAndTokuno | Tailed,
        ClawedTailedMagicalAndTokuno = MagicalClawedAndTailed | Tokuno,

        MagicalClawedTailedNecromanticAndTokuno = MagicalClawedTailedAndNecromantic | Tokuno,
    }

    [Flags]
    public enum MagicalAbility
    {
        None = 0x00000000,

        // Magical Ability
        Piercing            = 0x00000001,
        Bashing             = 0x00000002,
        Slashing            = 0x00000004,
        BattleDefense       = 0x00000008,
        WrestlingMastery    = 0x00000010,

        // Magical Schools
        Poisoning           = 0x00000020,
        Bushido             = 0x00000040,
        Ninjitsu            = 0x00000080,
        Discordance         = 0x00000100,
        MageryMaster        = 0x00000200,
        Mysticism           = 0x00000400,
        Spellweaving        = 0x00000800,
        Chivalry            = 0x00001000,
        Necromage           = 0x00002000,
        Necromancy          = 0x00004000,
        Magery              = 0x00008000,

        Tokuno = Bushido | Ninjitsu,
        SabreToothedTiger = Bashing | Piercing | Poisoning,

        Vartiety = Piercing | Slashing | WrestlingMastery,
        Variety1 = Bashing | Vartiety,
        Variety2 = Bashing | Chivalry | Discordance | MageryMastery | Mysticism | Necromage | Necromancy | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,

        StandardClawedOrTailed = SabreToothedTiger | Slashing | WrestlingMastery,
        Tokuno1 = Tokuno | Chivalry | Discordance | MageryMastery | Mysticism | Poisoning | Spellweaving,
        Dragon1 = Bashing | BattleDefense | Chivalry | Discordance | MageryMastery | Mysticism | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,
        Dragon2 = Bashing | Chivalry | Discordance | MageryMastery | Mysticism | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery, 
        Cusidhe = Chivalry | Discordance | Mysticism | Poisoning | Spellweaving | WrestlingMastery,
        Wolf = Bashing | Tokuno | Necromage | Piercing | Poisoning | Slashing | WrestlingMastery,
        DragonWolf = Bashing | BattleDefense | Piercing | Poisoning | Slashing | WrestlingMastery,
        DreadSpider = Bashing | Tokuno | Chivlary | Discordance | MageryMastery | Mysticism | Necromage | Necromancy | Piercing | Slashing | Spellweaving | WrestlingMastery,
        DreadWarhorse = Bashing | BattleDefense | Chivarly | Discordance | MageryMastery | Mysticism | Necromage | Necromancy | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,
        GreaterDragon = Chivalry | Discordance | MageryMastery | Mysticism | Poisoning | Spellweaving,
        Hellcat = Bashing | Necromage | Necromancy | Piercing | Poisoning | Slashing | WrestlingMastery,
        Hiryu = Tokuno | Chivalry | Discordance | Poisoning | Spellweaving | WrestlingMastery,
        LavaLizard = Tokuno | Chivalry | Bashing,
        RuneBeetle = Chivlary | Discordance | MageryMastery | Mysticism | Spellweaving,
        StygianDrake = Bashing | Chivalry | Discordance | Mysticism | Piercing | Poisoning | Slashing | Spellweaving | WrestlingMastery,
        Triceratops = Bashing | Poisoning | Slashing | WrestlingMastery,
        TsukiWolf = Tokuno | Chivalry | Discordance | Mysticism | Necromage | Necromancy | Poisoning | Spellweaving | WrestlingMastery
    }

    public static class PetTrainingHelper
    {
        public static bool Enabled { get { return Core.TOL; } }

        public static List<TrainingPoint> TrainingPoints { get { return _TrainingPoints; } }
        public static List<TrainingPoint> _TrainingPoints;

        public static TrainingDefinition[] Definitions { get { return _Defs; } }
        private static TrainingDefinition[] _Defs =
        {
            new TrainingDefinition(typeof(Alligator), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility1, AreaEffect.None),  
            new TrainingDefinition(typeof(BakeKitsune), Class.ClawedTailedAndTokuno, MagicalAbility.Tokuno1, SpecialAbility.ClawedTailedAndMagical2, WepAbility2, AreaEffect.Area1), 
            new TrainingDefinition(typeof(BaneDragon), Class.MagicalClawedAndTailed, MagicalAbility.Dragon1, SpecialAbility.BaneDragon, WepAbility2, AreaEffect.Area2), 
            new TrainingDefinition(typeof(BattleChickenLizard), Class.Untrainable, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None), 
            new TrainingDefinition(typeof(Bird), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(BlackBear), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(BloodFox), Class.None, MagicialAbility.Poisoning, SpecialAbility.None, WepAbility3, AreaEffect.None),
            new TrainingDefinition(typeof(Boar), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(BrownBear), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Bull), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Bullfrog), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Cat), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility4, AreaEffect.None), 
            new TrainingDefinition(typeof(Chickadee), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Chicken), Class.Clawed, MagicalAbility.None, SpecialAbility.Clawed, WepAbilityNone, AreaEffect.None), 
            new TrainingDefinition(typeof(ChickenLizard), Class.Untrainable, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(ClockworkLeatherWolf), Class.None, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(ColdDrake), Class.None, MagicalAbility.Dragon1, SpecialAbility.None, WepAbility2, AreaEffect.None),
            new TrainingDefinition(typeof(CorrosiveSlime), Class.StickySkin, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Cougar), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Cow), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(CrimsonDrake), Class.None, MagicalAbility.Dragon2, SpecialAbility.None, WepAbility2, AreaEffect.Area1), // CrimsonDrake[Poison] = AreaEffect.Area2
            new TrainingDefinition(typeof(Crossbill), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Crow), Class.Clawed, MagicalAbility.StandardClawedOrTaile, SpecialAbility.Clawed, WepAbilityNone, AreaEffect.None), 
            new TrainingDefinition(typeof(Cusidhe), Class.MagicalClawedAndTailed, MagicalAbility.Cusidhe, SpecialAbility.ClawedTailedAndMagical2, WepAbility5, AreaEffect.Area1), 
            new TrainingDefinition(typeof(Cuckoo), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(DarkSteed), Class.None, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None), 
            new TrainingDefinition(typeof(DeathwatchBeetle), Class.Insectoid, MagicialAbility.Poisoning, SpecialAbility.MagicalInsectoid, WepAbility5, AreaEffect.None), 
            new TrainingDefinition(typeof(DesertOstard), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Dimetrosaur), Class.None, MagicialAbility.Poisoning, SpecialAbility.None, WepAbilityNone, AreaEffect.None), 
            new TrainingDefinition(typeof(DireWolf), Class.ClawedNecromanticAndTokuno, MagicialAbility.Wolf, SpecialAbility.ClawedAndNecromantic, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Dog), Class.Tailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Tailed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Dragon), Class.MagicalClawedAndTailed, MagicalAbility.Dragon2, SpecialAbility.None, WepAbility2, AreaEffect.Area1), 
            new TrainingDefinition(typeof(DragonTurtleHatchling), Class.MagicalAndtailed, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None), 
            new TrainingDefinition(typeof(DragonWolf), Class.None, MagicalAbility.DragonWolf, SpecialAbility.None, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Drake), Class.MagicalAndTailed, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None), 
            new TrainingDefinition(typeof(DreadSpider), Class.MagicalNecromanticAndTokuno, MagicalAbility.DreadSpider, SpecialAbility.DreadSpider, WepAbility2, AreaEffect.Area2), 
            new TrainingDefinition(typeof(DreadWarhorse), Class.MagicalAndNecromantic, MagicalAbility.DreadWarhorse, SpecialAbility.None, WepAbility2, AreaEffect.Area2), 
            new TrainingDefinition(typeof(Eagle), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Ferret), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(Finch), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None), 
            new TrainingDefinition(typeof(FireBeetle), Class.MagicalAndInsectoid, MagicalAbility.StandardClawedOrTailed, SpecialAbility.MagicalInsectoid, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(FireSteed), Class.Magical, MagicalAbility.Dragon2, SpecialAbility.None, WepAbility2, AreaEffect.Area1),
            new TrainingDefinition(typeof(ForestOstard), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(FrenziedOstard), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(ForstDragon), Class.None, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(FrostDrake), Class.None, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(FrostMite), Class.Insectoid, MagicialAbility.Poisoning, SpecialAbility.MagicalInsectoid, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(FrostSpider), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility2, AreaEffect.None),
            new TrainingDefinition(typeof(Gallusaurus), Class.None, MagicialAbility.Poisoning, SpecialAbility.None, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Gaman), Class.Tailed, MagicialAbility.Poisoning, SpecialAbility.Tailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(GiantBeetle), Class.Insectoid, MagicalAbility.StandardClawedOrTailed, SpecialAbility.MagicalInsectoid, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(GiantIceWyrm), Class.Tailed, MagicalAbility.Variety1, SpecialAbility.BitingTailed, WepAbility2, AreaEffect.Area3),
            new TrainingDefinition(typeof(GiantRat), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(GiantSpider), Class.None, MagicalAbility.Variety1, SpecialAbility.BitingAnimal, WepAbility1, AreaEffect.Area3),
            new TrainingDefinition(typeof(GiantToad), Class.StickySkin, MagicalAbility.StandardClawedOrTailed, SpecialAbility.StickySkin, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Goat), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Gorilla), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(GreatHart), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(GreaterChicken), Class.Clawed, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(GreaterDragon), Class.MagicalClawedAndTailed, MagicalAbility.GreaterDragon, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(GreaterMongbat), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(GreyWolf), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(GrizzlyBear), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Hawk), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Hellhound), Class.ClawedTailedNecromanticAndTokuno, MagicialAbility.Wolf, SpecialAbility.None, WepAbility1, AreaEffect.ExplosiveGoo),
            new TrainingDefinition(typeof(Hellcat), Class.ClawedTailedAndNecromantic, MagicalAbility.Hellcat, SpecialAbility.None, WepAbility1, AreaEffect.ExplosiveGoo),
            new TrainingDefinition(typeof(HighPlainsBoura), Class.Tailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.None, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Hind), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Hiryu), Class.ClawedTailedMagicalAndTokuno, MagicalAbility.Hiryu, SpecialAbility.None, WepAbility7, AreaEffect.Area1),
            new TrainingDefinition(typeof(Horse), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Imp), Class.MagicalClawedTailedAndNecromantic, MagicalAbility.Variety2, SpecialAbility.Imp, WepAbility2, AreaEffect.Area2),
            new TrainingDefinition(typeof(IronBeetle), Class.Insectoid, MagicalAbility.StandardClawedOrTailed, SpecialAbility.MagicalInsectoid, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(JackRabbit), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Kirin), Class.MagicalClawedAndTailed, MagicalAbility.Dragon2, SpecialAbility.ClawedTailedAndMagical2, WepAbility2, AreaEffect.Area1),
            new TrainingDefinition(typeof(Kingfisher), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Lapwig), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Lasher), Class.Magical, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(LavaLizard), Class.ClawedTailedMagicalAndTokuno, MagicalAbility.LavaLizard, SpecialAbility.None, WepAbility6, AreaEffect.Area1),
            new TrainingDefinition(typeof(LesserHiryu), Class.ClawedTailedMagicalAndTokuno, MagicalAbility.Tokuno1, SpecialAbility.None, WepAbility7, AreaEffect.Area1),
            new TrainingDefinition(typeof(Lion), Class.ClawedAndTailed, MagicialAbility.Poisoning, SpecialAbility.BitingClawedAndTailed, WepAbility1, AreaEffect.Area3),
            new TrainingDefinition(typeof(Llama), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility9, AreaEffect.None),
            new TrainingDefinition(typeof(LowlandBoura), Class.Tailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Tailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Magpie), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(MisterGobbles), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Mongbat), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(MountainGoat), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Najasaurus), Class.StickySkinAndTailed, MagicalAbility.Variety1, SpecialAbility.TailedAndStickySkin, WepAbility1, AreaEffect.Area3),
            new TrainingDefinition(typeof(Nightingale), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Nightmare), Class.MagicalAndNecromantic, MagicalAbility.Variety2, SpecialAbility.None, WepAbility2, AreaEffect.Area1),
            new TrainingDefinition(typeof(Nuthatch), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(OsseinRam), Class.None, MagicalAbility.Variety2, SpecialAbility.None, WepAbilityNone, AreaEffect.Area1),
            new TrainingDefinition(typeof(PackHorse), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(PackLlama), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(PalaminoHorse), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Panther), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Paralithode), Class.None, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(ParoxysmusSwampDragon), Class.Untrainable, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Phoenix), Class.MagicalAndClawed, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Pig), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(PlatinumDrake), Class.None, MagicalAbility.Dragon2, SpecialAbility.None, WepAbility2, AreaEffect.Area1), // PlatinumDrake[Poison] = AreaEffect.Area2
            new TrainingDefinition(typeof(Plover), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(PolarBear), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(PredatorHellcat), Class.ClawedTailedAndNecromantic, MagicalAbility.Hellcat, SpecialAbility.None, WepAbility1, AreaEffect.ExplosiveGoo),
            new TrainingDefinition(typeof(Rabbit), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Raptor), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Rat), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Raven), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Reptalon), Class.MagicalAndTailed, MagicalAbility.Cusidhe, SpecialAbility.None, WepAbility10, AreaEffect.Area2),
            new TrainingDefinition(typeof(RideableLlama), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Ridgeback), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(RuddyBoura), Class.Tailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Tailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(RuneBeetle), Class.Insectoid, MagicalAbility.RuneBeetle, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(SabreToothedTiger), Class.ClawedAndTailed, MagicalAbility.SabreToothedTiger, SpecialAbility.SabreTri, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(SakkhranBirdOfPrey), Class.MagicalAndTailed, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Saurosaurus), Class.Tailed, MagicialAbility.Poisoning, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(SavageRidgeback), Class.Clawed, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Scorpion), Class.Tailed, MagicalAbility.Variety1, SpecialAbility.BitingTailed, WepAbility1, AreaEffect.Area3),
            new TrainingDefinition(typeof(SerpentineDragon), Class.MagicalClawedAndTailed, MagicalAbility.Dragon2, SpecialAbility.None, WepAbility2, AreaEffect.Area2),
            new TrainingDefinition(typeof(SewerRat), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility3, AreaEffect.None),
            new TrainingDefinition(typeof(ShadowWyrm), Class.TailedAndNecromantic, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Sheep), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(SilverSteed), Class.None, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(SkitteringHopper), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Skree), Class.MagicalClawedAndTailed, MagicalAbility.Dragon2, SpecialAbility.ClawedTailedAndMagical1, WepAbility2, AreaEffect.Area1, AreaEffect.None),
            new TrainingDefinition(typeof(Skylark), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Slime), Class.StickySkin, MagicalAbility.Variety1, SpecialAbility.BitingStickySkin, WepAbility1, AreaEffect.Area3),
            new TrainingDefinition(typeof(Slith), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.None, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Snake), Class.Tailed, MagicalAbility.Variety1, SpecialAbility.BitingTailed, WepAbility1, AreaEffect.Area3),
            new TrainingDefinition(typeof(SnowLeopard), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Sparrow), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Squirrel), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Starling), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(StoneSlith), Class.ClawedAndTailed, MagicalAbility.Poisoning, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(StygianDrake), Class.MagicalClawedAndTailed, MagicalAbility.StygianDrake, SpecialAbility.ClawedTailedAndMagical1, WepAbility2, AreaEffect.Area1),
            new TrainingDefinition(typeof(Swallow), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(SwampDragon), Class.Untrainable, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Swift), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Tern), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Thrush), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Timerwolf), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Towhee), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Triceratops), Class.Tailed, MagicalAbility.Triceratops, SpecialAbility.SabreTri, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(TropicalBird), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(TsukiWolf), Class.MagicalClawedTailedNecromanticAndTokuno, MagicalAbility.TsukiWolf, SpecialAbility.TsukiWolf, WepAbility2, AreaEffect.Area1),
            new TrainingDefinition(typeof(Turkey), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Unicorn), Class.Magical, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Vollem), Class.MagicalAndTailed, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Walrus), Class.None, MagicalAbility.StandardClawedOrTailed, SpecialAbility.AnimalStandard, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(WarHorse), Class.None, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(WarOstard), Class.Clawed, MagicalAbility.None, SpecialAbility.Clawed, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(Warbler), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(WhiteWolf), Class.ClawedAndTailed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.ClawedAndTailed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(WhiteWyrm), Class.MagicalClawedAndTailed, MagicalAbility.Dragon1, SpecialAbility.ClawedTailedAndMagical, WepAbility2, AreaEffect.Earthen),
            new TrainingDefinition(typeof(WildTiger), Class.ClawedAndTailed, MagicialAbility.Poisoning, SpecialAbility.None, WepAbility3, AreaEffect.None),
            new TrainingDefinition(typeof(Windrunner), Class.TailedAndNecromantic, MagicalAbility.None, SpecialAbility.None, WepAbilityNone, AreaEffect.None),
            new TrainingDefinition(typeof(WolfSpider), Class.None, MagicalAbility.Vartiety, SpecialAbility.BitingAnimal, WepAbility1, AreaEffect.Disease),
            new TrainingDefinition(typeof(Woodpecker), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),
            new TrainingDefinition(typeof(Wren), Class.Clawed, MagicalAbility.StandardClawedOrTailed, SpecialAbility.Clawed, WepAbility1, AreaEffect.None),  
        };

        #region Accessors
        public TrainingDefinition GetTrainingDefinition(BaseCreature bc)
        {
            if (bc == null || !bc.Controlled)
                return null;

            return _Defs.FirstOrDefault(def => def.CreatureType == bc.GetType());
        }

        public static TrainingPoint GetTrainingPoint(object o)
        {
            return _TrainingPoints.FirstOrDefault(tp => tp.TrainPoint == o);
        }

        public AbilityProfile GetProfile(BaseCreature bc, bool create = false)
        {
            var profile = bc.AbilityProfile;

            if (profile == null && create)
                bc.AbilityProfile = profile = new AbilityProfile(bc);

            return profile;
        }
        #endregion

        #region SpecialAbility Defs
        public static SpecialAbility[] Abilities =
        {
            SpecialAbility.RuneCorruption,
            SpecialAbility.GraspingClaw,
            SpecialAbility.RagingBreath,
            SpecialAbility.ConductiveBlast,
            SpecialAbility.LightningForce,
            SpecialAbility.StealLife,
            SpecialAbility.AngryFire,
            SpecialAbility.DragonBreath,
            SpecialAbility.Inferno,
            SpecialAbility.FlurryForce,
            SpecialAbility.ViciousBite,
            SpecialAbility.SearingWounds,
            SpecialAbility.LifeLeech,
            SpecialAbility.StickySkin,
            SpecialAbility.TailSwipe,
            SpecialAbility.VenomousBite,
            SpecialAbility.ManaDrain,
            SpecialAbility.Repel,
        };

        public static SpecialAbility[] None = { };

        public static SpecialAbility[] Magical1 = 
        { 
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath
        };

        public static SpecialAbility[] Magical2 = 
        { 
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.StealLife
        };

        public static SpecialAbility[] Magical3 = 
        { 
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.PsychicAttack
        };

        public static SpecialAbility[] Magical4 = 
        { 
            SpecialAbility.AngryFire, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.RagingBreath
        };

        public static SpecialAbility[] NecroMagical = 
        { 
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech
        };

        public static SpecialAbility[] Bites =
        {
            SpecialAbility.VenomousBite, SpecialAbility.ViciousBite
        };

        public static SpecialAbility[] AnimalStandard =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds
        };

        public static SpecialAbility[] BitingAnimal =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite
        };

        public static SpecialAbility[] Clawed =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw
        };

        public static SpecialAbility[] Tailed =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.TailSwipe
        };

        public static SpecialAbility[] ClawedAndTailed =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe
        };

        public static SpecialAbility[] Insectoid =
        {
             SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.RuneCorruption
        };

        public static SpecialAbility[] MagicalInsectoid =
        {
             SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.RuneCorruption,
             SpecialAbility.AngryFire, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.RagingBreath
        };

        public static SpecialAbility[] StickySkin =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds,
            SpecialAbility.StickySkin
        };

        public static SpecialAbility[] BitingStickySkin =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds,
            SpecialAbility.StickySkin, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite
        };

        public static SpecialAbility[] BitingTailed =
        {
            SpecialAbility.VenomousBite, SpecialAbility.ViciousBite, SpecialAbility.TailSwipe
        };

        public static SpecialAbility[] BitingClawedAndTailed =
        {
            SpecialAbility.VenomousBite, SpecialAbility.ViciousBite,
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, 
            SpecialAbility.TailSwipe
        };

        public static SpecialAbility[] StickySkin =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds,
            SpecialAbility.StickySkin, SpecialAbility.TailSwipe
        };

        public static SpecialAbility[] ClawedAndNecromantic =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw,
            SpecialAbility.NecroMagical
        };

        public static SpecialAbility[] ClawedTailedAndMagical1 =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe,
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath
        };

        public static SpecialAbility[] ClawedTailedAndMagical2 =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe,
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.StealLife
        };

        public static SpecialAbility[] BaneDragon =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe,
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite
        };

        public static SpecialAbility[] DreadSpider =
        {
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech,
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.VenomousBite, SpecialAbility.ViciousBite
        };

        public static SpecialAbility[] FireBeetle =
        {
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.RuneCorruption,
            SpecialAbility.AngryFire, SpecialAbility.DragonBreath, SpecialAbility.Inferno, SpecialAbility.RagingBreath
        };

        public static SpecialAbility[] Imp =
        {
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech,
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe,
            SpecialAbiltiy.ViciousBite
        };

        public static SpecialAbility[] TsukiWolf =
        {
            SpecialAbility.AngryFire, SpecialAbility.ConductiveBlast, SpecialAbility.DragonBreath, SpecialAbility.Inferno,
            SpecialAbility.LightningForce, SpecialAbility.RagingBreath, SpecialAbility.LifeLeech,
            SpecialAbility.ManaDrain, SpecialAbility.Repel, SpecialAbility.SearingWounds, SpecialAbility.GraspingClaw, SpecialAbility.TailSwipe
        };

        public static SpecialAbility[] SabreTri =
        {
            SpecialAbility.SearingWounds, SpecialAbility.TailSwipe
        };
        #endregion

        #region AreaEffect Defs
        public AreaEffect[] AreaEffects =
        {
            AreaEffect.AuraOfEnergy,
            AreaEffect.Firestorm,
            AreaEffect.ExplosiveGoo,
            AreaEffect.EssenceOfEarth,
            AreaEffect.AuraOfNausea,
            AreaEffect.EssenceOfDisease,
        };

        public AreaEffec[] Earthen = 
        {
            AreaEffect.EssenceOfEarth, AreaEffect.ExplosiveGoo
        };

        public AreaEffec[] Disease = 
        {
            AreaEffect.AuraOfNausea, AreaEffect.EssenceOfDisease
        };

        public AreaEffec[] Area1 = 
        {
            AreaEffect.EssenceOfEarth, AreaEffect.ExplosiveGoo, AreaEffect.AuraOfEnergy
        };

        public AreaEffec[] Area2 = 
        {
            AreaEffect.EssenceOfEarth, AreaEffect.ExplosiveGoo, AreaEffect.AuraOfEnergy, 
            AreaEffect.AuraOfNausea, AreaEffect.EssenceOfDisease,
            AreaEffect.PoisonBreath
        };

        public AreaEffec[] Area3 = 
        {
            AreaEffect.AuraOfNauesea, AreaEffect.EssenceOfDisease, AreaEffect.PoisonBreath, 
        };
        #endregion

        #region Weapon Ability Defs
        public WeaponAbility[] WeaponAbilities =
        {
            WeaponAbility.NerveStrike,
            WeaponAbility.WhirlwindAttack,
            WeaponAbility.LightningArrow,
            WeaponAbility.InfusedThrow,
            WeaponAbility.MysticArc,
            WeaponAbility.TalonStrike,
            WeaponAbility.PsychicAttack,
            WeaponAbility.ArmorIgnore,
            WeaponAbility.ArmorPierce,
            WeaponAbility.Bladeweave,
            WeaponAbility.BleedAttack,
            WeaponAbility.Block,
            WeaponAbility.ConcussionBlow,
            WeaponAbility.CrushingBlow,
            WeaponAbility.Disarm,
            WeaponAbility.Dismount,
            WeaponAbility.DoubleStrike,
            WeaponAbility.DualWield,
            WeaponAbility.Feint,
            WeaponAbility.ForceOfNature,
            WeaponAbility.FrenziedWhirlwind,
            WeaponAbility.MortalStrike,
            WeaponAbility.ParalyzingBlow,
            WeaponAbility.ColdWind,
        };

        public static WeaponAbility[] WepAbilityNone = { };

        public static WeaponAbility[] WepAbility1 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow,
            WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind,
            WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility2 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ColdWind, WeaponAbility.ConcussionBlow,
            WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind,
            WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility3 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, 
            WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike,
            WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility4 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ConcussionBlow, 
            WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.MortalStrike,
            WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility5 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.ColdWind, WeaponAbility.ConcussionBlow, 
            WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind,
            WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility6 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ColdWind, 
            WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Disarm, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind,
            WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility7 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ColdWind,
            WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind,
            WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility8 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ColdWind,
            WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Feint, WeaponAbility.ForceOfNature, WeaponAbility.FrenziedWhirlwind,
            WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility9 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.Block,
            WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, 
            WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.ParalyzingBlow, WeaponAbility.PsychicAttack, 
            WeaponAbility.TalonStrike
        };

        public static WeaponAbility[] WepAbility10 =
        {
            WeaponAbility.ArmorIgnore, WeaponAbility.ArmorPierce, WeaponAbility.Bladeweave, WeaponAbility.BleedAttack, WeaponAbility.ColdWind,
            WeaponAbility.ConcussionBlow, WeaponAbility.CrushingBlow, WeaponAbility.Dismount, WeaponAbility.Feint, WeaponAbility.ForceOfNature, 
            WeaponAbility.FrenziedWhirlwind, WeaponAbility.MortalStrike, WeaponAbility.NerveStrike, WeaponAbility.PsychicAttack, WeaponAbility.TalonStrike
        };
        #endregion

        #region Training Points Configuration
        public static void Configure()
        {
            _TrainingPoints = new List<TrainingPoint>();

            _TrainingPoints.Add(new TrainingPoint(Stats.Str, 3.0, 700, 1061146, 1157507));
            _TrainingPoints.Add(new TrainingPoint(Stats.Dex, 0.1, 150, 1061147, 1157508));
            _TrainingPoints.Add(new TrainingPoint(Stats.Int, 0.5, 700, 1061148, 1157509));

            _TrainingPoints.Add(new TrainingPoint(Stats.Hits, 3.0, 1100, 1061149, 1157510));
            _TrainingPoints.Add(new TrainingPoint(Stats.Stam, 0.5, 150, 1061150, 1157511));
            _TrainingPoints.Add(new TrainingPoint(Stats.Mana, 0.5, 1500, 1061151, 1157512));

            _TrainingPoints.Add(new TrainingPoint(Stats.RegenHits, 18.0, 20, 1075627, 1157513));
            _TrainingPoints.Add(new TrainingPoint(Stats.RegenStam, 12.0, 30, 1079410, 1157514));
            _TrainingPoints.Add(new TrainingPoint(Stats.RegenMana, 12.0, 30, 1079411, 1157515));

            // TODO: Damage Per Second 1157516

            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Physcial, 3.0, 80, 1061158, 1157517));
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Fire, 3.0, 80, 1061159, 1157518));
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Cold, 3.0, 80, 1061160, 1157519));
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Poison, 3.0, 80, 1061161, 1157520));
            _TrainingPoints.Add(new TrainingPoint(ResistanceType.Energy, 3.0, 80, 1061162, 1157521));

            _TrainingPoints.Add(new TrainingPoint(SkillName.Magery, 0.5, 120, 1002106, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.EvalInt, 1.0, 120, 1044076, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Necromancy, 0.5, 120, 1044109, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.SpiritSpeak, 1.0, 120, 1044092, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Chivarly, 0.5, 120, 1044111, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Focus, 0.1, 120, 1044110, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Bushido, 0.5, 120, 1044112, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Ninjitsu, 0.5, 120, 1044113, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Spellweaving, 0.5, 120), 1044114, 1157522);
            _TrainingPoints.Add(new TrainingPoint(SkillName.Mysticism, 0.5, 120, 1044115, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Meditation, 0.1, 120, 1044106, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.MagicResist, 0.1, 120, 1044086, 1157522));

            _TrainingPoints.Add(new TrainingPoint(SkillName.Wrestling, 1.0, 120, 1044103, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Tactics, 1.0, 120, 1044087, 1157522));
            _TrainingPoints.Add(new TrainingPoint(SkillName.Anatomy, 0.1, 120, 1044061, 1157522));

            TextDefinition[][] loc = _MagicalAbilityLocalizations;

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Piercing, 1.0, 1, loc[0][0], loc[0][1],
                new TrainingPointRequirement(WeaponAbility.ArmorIgnore, 100),
                new TrainingPointRequirement(WeaponAbility.ParalyzingBlow, 100),
                new TrainingPointRequirement(WeaponAbility.BleedAttack, 100)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Bashing, 1.0, 1, loc[1][0], loc[1][1],
                new TrainingPointRequirement(WeaponAbility.MortalStrike, 100),
                new TrainingPointRequirement(WeaponAbility.ConcussionBlow, 100),
                new TrainingPointRequirement(WeaponAbility.Disarm, 100)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Slashing, 1.0, 1, loc[2][0], loc[2][1],
                new TrainingPointRequirement(SkillName.Bushido, 500),
                new TrainingPointRequirement(WeaponAbility.ArmorIgnore, 100),
                new TrainingPointRequirement(WeaponAbility.Disarm, 100),
                new TrainingPointRequirement(WeaponAbility.NerveStrike, 100)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.BattleDefense, 1.0, 1, loc[3][0], loc[3][1],
                new TrainingPointRequirement(WeaponAbility.Disarm, 100),
                new TrainingPointRequirement(WeaponAbility.ParalyzingBlow, 100)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.WrestlingMastery, 1.0, 1, loc[4][0], loc[4][1],
                new TrainingPointRequirement(WeaponAbility.Disarm, 100),
                new TrainingPointRequirement(WeaponAbility.ParalyzingBlow, 100)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Poisoning, 1.0, 1, loc[5][0], loc[5][1],
                new TrainingPointRequirement(SkillName.Chivalry, 100)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Bushido, 1.0, 1, loc[6][0], loc[6][1],
                new TrainingPointRequirement(SkillName.Bushido, 500)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Ninjitsu, 1.0, 1, loc[7][0], loc[7][1],
                new TrainingPointRequirement(SkillName.Ninjitsu, 500)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Discordance, 1.0, 1, loc[8][0], loc[8][1],
                new TrainingPointRequirement(SkillName.Discordance, 500)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.MageryMaster, 1.0, 1, loc[9][0], loc[9][1],
                new TrainingPointRequirement(SkillName.Magery, 100),
                new TrainingPointRequirement(SkillName.EvalInt, 100)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Mysticism, 1.0, 1, loc[10][0], loc[10][1],
                new TrainingPointRequirement(SkillName.Mysticism, 500)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Spellweaving, 1.0, 1, loc[11][0], loc[11][1],
                new TrainingPointRequirement(SkillName.Spellweaving, 500)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Chivalry, 1.0, 1, loc[12][0], loc[12][1],
                new TrainingPointRequirement(SkillName.Chivalry, 500)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Necromage, 1.0, 1, loc[13][0], loc[13][1],
                new TrainingPointRequirement(SkillName.Magery, 100),
                new TrainingPointRequirement(SkillName.Necromancy, 100),
                new TrainingPointRequirement(SkillName.SpiritSpeak, 100),
                new TrainingPointRequirement(SkillName.EvalInt, 100)));

            _TrainingPoints.Add(new TrainingPoint(MagicalAbility.Necromancy, 1.0, 1, loc[14][0], loc[14][1],
                new TrainingPointRequirement(SkillName.Necromancy, 100),
                new TrainingPointRequirement(SkillName.SpiritSpeak, 100)));

            loc = _SpecialAbilityLocalizations;
            int index = 0;

            foreach (var i in Enum.GetValues(typeof(SpecialAbility)))
            {
                _TrainingPoints.Add(new TrainingPoint((SpecialAbility)i, 1.0, 100, loc[index][0], loc[index][1]));
                index++;
            }

            loc = _AreaEffectLocalizations;
            index = 0;

            foreach (var i in Enum.GetValues(typeof(AreaEffect)))
            {
                _TrainingPoints.Add(new TrainingPoint((AreaEffect)i, 1.0, 100, loc[index][0], loc[index][1]));
                index++;
            }

            loc = _WeaponAbilityLocalizations;
            index = 0;

            foreach (var ability in WeaponAbilities)
            {
                TrainingPointRequirement requirement = null;

                switch (ability)
                {
                    default: break;
                    case WeaponAbility.NerveStrike: requirement = new TrainingPointRequirement(SkillName.Bushido, 500); break;
                    case WeaponAbility.TalonStrike: requirement = new TrainingPointRequirement(SkillName.Ninjitsu, 500); break;
                    case WeaponAbility.Feint: requirement = new TrainingPointRequirement(SkillName.Bushido, 500); break;
                    case WeaponAbility.FrenziedWhirlwind: requirement = new TrainingPointRequirement(SkillName.Ninjitsu, 500); break;
                    case WeaponAbility.Bladeweave: requirement = new TrainingPointRequirement(SkillName.Bushido, 500); break;
                }

                _TrainingPoints.Add(new TrainingPoint(ability, 1.0, 100, loc[index][0], loc[index][1], requirement));
            }

            index++;
        }
        #endregion

        #region Training Helpers
        public static int GetTrainingCapTotal(Stats stat)
        {
            switch (stat)
            {
                case Stat.Str:
                case Stat.Int:
                case Stat.Dex: return 2300;
                case Stat.Hits:
                case Stat.Stam:
                case Stat.Mana: return 3300;
            }

            return 0;
        }

        public static int GetTrainingCapTotal(ResistanceType resist)
        {
            return 1095;
        }

        public static int GetTotalStatWeight(BaseCreature bc)
        {
            var str = GetTrainingPoint(Stats.Str);
            var dex = GetTrainingPoint(Stats.Dex);
            var intel = GetTrainingPoint(Stats.Int);

            return (int)(((double)bc.RawStr * str.Weight) + 
                ((double)bc.RawDex * dex.Weight) + 
                ((double)bc.RawInt * intel.Weight));
        }

        public static int GetTotalAttributeWeight(BaseCreature bc)
        {
            var hits = GetTrainingPoint(Stats.Hits);
            var stam = GetTrainingPoint(Stats.Stam);
            var mana = GetTrainingPoint(Stats.Mana);

            return (int)((double)bc.HitsMax * hits.Weight) + 
                ((double)bc.StamMax * stam.Weight) + 
                ((double)bc.ManaMax * mana.Weight));
        }

        public static int GetTotalResistWeight(BaseCreature bc)
        {
            var phys = GetTrainingPoint(ResistanceType.Physical);
            var fire = GetTrainingPoint(ResistanceType.Fire);
            var cold = GetTrainingPoint(ResistanceType.Cold);
            var pois = GetTrainingPoint(ResistanceType.Poison);
            var nrgy = GetTrainingPoint(ResistanceType.Energy);

            return (int)(((double)PhysicalResistanceSeed * phys.Weight) +
                   ((double)FireResistanceSeed * fire.Weight) +
                   ((double)ColdResistanceSeed * cold.Weight) +
                   ((double)PoisonResistanceSeed * pois.Weight) +
                   ((double)EnergyResistanceSeed * nrgy.Weight));
        }

        public static bool ApplyTrainingPoint(BaseCreature bc, TrainingPoint trainingPoint, int value)
        {
            var profile = GetProfile(bc, true);

            if (trainingPoint.TrainPoint is Stats)
            {
                Stats stat = trainingPoint.TrainPoint as Stats;

                switch (stat)
                {
                    case Stats.Str: bc.RawStr = value;
                    case Stats.Dex: bc.RawDex = value;
                    case Stats.Int: bc.RawInt = value;
                    case Stats.Hits: bc.HitsMax = value;
                    case Stats.Stam: bc.Stam = value;
                    case Stats.Mana: bc.Mana = value;
                    case Stats.RegenHits: profile.RegenHits = value;
                    case Stats.RegenStam: profile.RegenStam = value;
                    case Stats.RegenMana: profile.RegenMana = value;
                    //TODO: Damage Per Second
                }

                return true;
            }

            else if (trainingPoint.TrainPoint is MagicalAbilitiy)
            {
                MagicalAbilitiy ability = trainingPoint.TrainPoint as MagicalAbility;

                if (ValidateTrainingPoint(bc, ability) && profile.AddAbility(ability))
                {
                    return true;
                }
            }

            else if (trainingPoint.TrainPoint is SpecialAbility)
            {
                SpecialAbility ability = trainingPoint.TrainPoint as SpecialAbility;

                if (ValidateTrainingPoint(bc, ability) && profile.AddAbility(ability))
                {
                    return true;
                }
            }

            else if (trainingPoint.TrainPoint is AreaEffect)
            {
                AreaEffect ability = trainingPoint.TrainPoint as AreaEffect;

                if (ValidateTrainingPoint(bc, ability) && profile.AddAbility(ability))
                {
                    return true;
                }
            }

            else if (trainingPoint.TrainPoint is WeaponAbility)
            {
                WeaponAbility ability = trainingPoint.TrainPoint as WeaponAbility;

                if (ValidateTrainingPoint(bc, ability) && profile.AddAbility(ability))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, MagicalAbilitiy ability)
        {
            var def = GetTrainingDefinition(bc);

            if (def == null)
                return false;

            return (def.MagicalAbilities & ability) != 0;
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, SpecialAbility ability)
        {
            var def = GetTrainingDefinition(bc);

            if (def == null)
                return false;

            return (def.SpecialAbilities & ability) != 0;
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, AreaEffect ability)
        {
            var def = GetTrainingDefinition(bc);

            if (def == null)
                return false;

            return (def.AreaEffects & ability) != 0;
        }

        public static bool ValidateTrainingPoint(BaseCreature bc, WeaponAbility ability)
        {
            var def = GetTrainingDefinition(bc);

            if (def == null)
                return false;

            return def.WeaponAbilities.FirstOrDefault(a => a == ability) != null;
        }
        #endregion

        #region Localizations
        public static TextDefinition[][] MagicalAbilityLocalizations { get { return _MagicalAbilityLocalizations; } }
        private static TextDefinition[][] _MagicalAbilityLocalizations =
        {
            new TextDefinition[] { 1157559, 1157392 }, // piercing
            new TextDefinition[] { 1157560, 1157471 }, // bashing
            new TextDefinition[] { 1157561, 1157396 }, // slashing
            new TextDefinition[] { 1157562, 1157395 }, // battle defense
            new TextDefinition[] { 1155784, 1157397 }, // wrestling mastery
            new TextDefinition[] { 1002122, 1157389 }, // poisoning
            new TextDefinition[] { 1044112, 1157383 }, // bushido
            new TextDefinition[] { 1044113, 1157388 }, // ninjitsu
            new TextDefinition[] { 1044075, 1157385 }, // discordance
            new TextDefinition[] { 1155771, 1157400 }, // magery mastery
            new TextDefinition[] { 1044115, 1157386 }, // mysticism
            new TextDefinition[] { 1044114, 1157390 }, // spellweaving
            new TextDefinition[] { 1044111, 1157384 }, // chivalry - must have pos karma
            new TextDefinition[] { 1157473, 1157474 }, // necromage
            new TextDefinition[] { 1044109, 1157387 }, // necromancy - must have neg karma
            new TextDefinition[] { 1002106, 1157391 }  // magery
        };

        public static TextDefinition[][] SpecialAbilityLocalizations { get { return _SpecialAbilityLocalizations; } }
        private static TextDefinition[][] _SpecialAbilityLocalizations =
        {
            new TextDefinition[] { 1157398, 1157399 }, // Rune Corruption
            new TextDefinition[] { 1157400, 1157401 }, // Grasping Claw
            new TextDefinition[] { 1157404, 1157405 }, // Raging Breath
            new TextDefinition[] { 1157406, 1157407 }, // Conductive Blast
            new TextDefinition[] { 1157408, 1157409 }, // Lightning Force
            new TextDefinition[] { 1157410, 1157411 }, // Steal Life
            new TextDefinition[] { 1157412, 1157413 }, // Angry Fire
            new TextDefinition[] { 1157414, 1157415 }, // Dragon Breath
            new TextDefinition[] { 1157416, 1157417 }, // Inferno
            new TextDefinition[] { 1157418, 1157419 }, // Flurry Force
            new TextDefinition[] { 1157420, 1157421 }, // Vicious Bite
            new TextDefinition[] { 1157422, 1157423 }, // Searing Wounds
            new TextDefinition[] { 1157424, 1157425 }, // Life Leech
            new TextDefinition[] { 1157426, 1157427 }, // Sticky Skin
            new TextDefinition[] { 1157428, 1157429 }, // Tail Swipe
            new TextDefinition[] { 1157430, 1157431 }, // Venomous Bite
            new TextDefinition[] { 1157432, 1157433 }, // Mana Drain
            new TextDefinition[] { 1157434, 1157435 }, // Repel
        };

        public static TextDefinition[][] AreaEffectLocalizations { get { return _AreaEffectLocalizations; } }
        private static TextDefinition[][] _AreaEffectLocalizations =
        {
            new TextDefinition[] { 1157459, 1157460 }, // Aura of Energy
            new TextDefinition[] { 1157461, 1157462 }, // Firestorm
            new TextDefinition[] { 1157463, 1157464 }, // Explosive Goo
            new TextDefinition[] { 1157465, 1157466 }, // Essence of Earth
            new TextDefinition[] { 1157467, 1157468 }, // Aura of Nausea
            new TextDefinition[] { 1157469, 1157470 }, // Essence of Disease
        };

        public static TextDefinition[][] WeaponAbilityLocalizations { get { return _WeaponAbilityLocalizations; } }
        private static TextDefinition[][] _WeaponAbilityLocalizations =
        {
            new TextDefinition[] { 1028855, 1157436 }, // Nerve Strike
            new TextDefinition[] { 1028850, 1157437 }, // Whirlwind Attack
            new TextDefinition[] { 1028863, 1157438 }, // Lightning Arrow
            new TextDefinition[] { 1113283, 1157439 }, // Infused Throw
            new TextDefinition[] { 1113282, 1157440 }, // Mystic Arc
            new TextDefinition[] { 1028856, 1157441 }, // Talon Strike
            new TextDefinition[] { 1028864, 1157442 }, // Psychic Attack
            new TextDefinition[] { 1028838, 1157443 }, // Armor Ignore
            new TextDefinition[] { 1028860, 1157444 }, // Armor Pierce
            new TextDefinition[] { 1028861, 1157445 }, // Bladeweave
            new TextDefinition[] { 1028839, 1157446 }, // Bleed Attack
            new TextDefinition[] { 1028853, 1157447 }, // Block
            new TextDefinition[] { 1028840, 1157448 }, // Concussion Blow
            new TextDefinition[] { 1028841, 1157449 }, // Crushing Blow
            new TextDefinition[] { 1028842, 1157450 }, // Disarm
            new TextDefinition[] { 1028843, 1157451 }, // Dismount
            new TextDefinition[] { 1028844, 1157452 }, // Double Strike
            new TextDefinition[] { 1028858, 1157453 }, // Dual Wield
            new TextDefinition[] { 1028857, 1157454 }, // Feint
            new TextDefinition[] { 1028866, 1157455 }, // Force of Nature
            new TextDefinition[] { 1028852, 1157456 }, // Frenzied Whirlwind
            new TextDefinition[] { 1028846, 1157457 }, // Mortal Strike
            new TextDefinition[] { 1028848, 1157458 }, // Paralyzing Blow
            new TextDefinition[] { 1157402, 1157403 }, // Cold Wind
        };

        public static int GetLocalization(BaseCreature pet, SkillName sk)
        {
            return pet.Skills[sk].Localization;
        }

        public static TextDefinition[] GetLocalization(object o)
        {
            if (o is MagicalAbility)
                return GetLocalization((MagicalAbility)o);

            if(o is SpecialAbility)
                return GetLocalization((SpecialAbility)o);

            if (o is AreaEffect)
                return GetLocalization((AreaEffect)o);

            if(o is WeaponAbility)
                return GetLocalization((WeaponAbilty)o);

            return new TextDefinition[] { 0, 0 };
        }

        public static TextDefinition[] GetLocalization(MagicalAbility ability)
        {
            switch (ability)
            {
                case MagicalAbility.Piercing: return _MagicalAbilityLocalizations[0];
                case MagicalAbility.Bashing: return _MagicalAbilityLocalizations[1];
                case MagicalAbility.Slashing: return _MagicalAbilityLocalizations[2];
                case MagicalAbility.BattleDefense: return _MagicalAbilityLocalizations[3];
                case MagicalAbility.WrestlingMastery: return _MagicalAbilityLocalizations[4];
                case MagicalAbility.Poisoning: return _MagicalAbilityLocalizations[5];
                case MagicalAbility.Bushido: return _MagicalAbilityLocalizations[6];
                case MagicalAbility.Ninjitsu: return _MagicalAbilityLocalizations[7];
                case MagicalAbility.Discordance: return _MagicalAbilityLocalizations[8];
                case MagicalAbility.MageryMastery: return _MagicalAbilityLocalizations[9];
                case MagicalAbility.Mysticism: return _MagicalAbilityLocalizations[10];
                case MagicalAbility.Spellweaving: return _MagicalAbilityLocalizations[11];
                case MagicalAbility.Chivalry: return _MagicalAbilityLocalizations[12];
                case MagicalAbility.Necromage: return _MagicalAbilityLocalizations[13];
                case MagicalAbility.Necromancy: return _MagicalAbilityLocalizations[14];
            }
        }

        public static TextDefinition[] GetLocalization(SpecialAbility ability)
        {
            switch (ability)
            {
                case SpecialAbility.RuneCorruption: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.GraspingClaw: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.RagingBreath: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.ConductiveBlast: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.LightningForce: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.StealLife: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.AngryFire: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.DragonBreath: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.Inferno: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.FlurryForce: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.ViciousBite: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.SearingWounds: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.LifeLeech: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.StickySkin: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.TailSwipe: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.VenomousBite: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.ManaDrain: return _SpecialAbilityLocalizations[0];
                case SpecialAbility.Repel: return _SpecialAbilityLocalizations[0];
            }
        }

        public static TextDefinition[] GetLocalization(AreaEffect effect)
        {
            switch (effect)
            {
                case WeaponAbility.NerveStrike: return _WeaponAbilityLocalizations[0];
                case WeaponAbility.WhirlwindAttack: return _WeaponAbilityLocalizations[1];
                case WeaponAbility.LightningArrow: return _WeaponAbilityLocalizations[2];
                case WeaponAbility.InfusedThrow: return _WeaponAbilityLocalizations[3];
                case WeaponAbility.MysticArc: return _WeaponAbilityLocalizations[4];
                case WeaponAbility.TalonStrike: return _WeaponAbilityLocalizations[5];
                case WeaponAbility.PsychicAttack: return _WeaponAbilityLocalizations[6];
                case WeaponAbility.ArmorIgnore: return _WeaponAbilityLocalizations[7];
                case WeaponAbility.ArmorPierce: return _WeaponAbilityLocalizations[8];
                case WeaponAbility.Bladeweave: return _WeaponAbilityLocalizations[9];
                case WeaponAbility.BleedAttack: return _WeaponAbilityLocalizations[10];
                case WeaponAbility.Block: return _WeaponAbilityLocalizations[11];
                case WeaponAbility.ConcussionBlow: return _WeaponAbilityLocalizations[12];
                case WeaponAbility.CrushingBlow: return _WeaponAbilityLocalizations[13];
                case WeaponAbility.Disarm: return _WeaponAbilityLocalizations[14];
                case WeaponAbility.Dismount: return _WeaponAbilityLocalizations[15];
                case WeaponAbility.DoubleStrike: return _WeaponAbilityLocalizations[16];
                case WeaponAbility.DualWield: return _WeaponAbilityLocalizations[17];
                case WeaponAbility.Feint: return _WeaponAbilityLocalizations[18];
                case WeaponAbility.ForceOfNature: return _WeaponAbilityLocalizations[19];
                case WeaponAbility.FrenziedWhirlwind: return _WeaponAbilityLocalizations[20];
                case WeaponAbility.MortalStrike: return _WeaponAbilityLocalizations[21];
                case WeaponAbility.ParalyzingBlow: return _WeaponAbilityLocalizations[22];
                case WeaponAbility.ColdWind: return _WeaponAbilityLocalizations[23];
            }
        }
        #endregion
    }
}
