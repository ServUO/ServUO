using System;
using Server.Items;

namespace Server.Kiasta.Settings
{
    #region Item Stat Deed Settings
    /*****************************************
     * ***********Item Stat Deeds*********** *
     *****************************************/
    public static class AttributeMaxValue //For each item
    {
        public static int AttackChance = 100;
        public static int BonusDex = 20;
        public static int BonusHits = 20;
        public static int BonusInt = 20;
        public static int BonusMana = 20;
        public static int BonusStam = 20;
        public static int BonusStr = 20;
        public static int CastSpeed = 5;
        public static int CastRecovery = 5;
        public static int DefendChance = 25;
        public static int DurabilityBonus = 500;
        public static int EnhancePotions = 20;
        public static int HitColdArea = 100;
        public static int HitDispel = 100;
        public static int HitEnergyArea = 100;
        public static int HitFireArea = 100;
        public static int HitFireball = 100;
        public static int HitHarm = 100;
        public static int HitLeechHits = 100;
        public static int HitLightning = 100;
        public static int HitLowerAttack = 100;
        public static int HitLowerDefend = 100;
        public static int HitMagicArrow = 100;
        public static int HitLeechMana = 100;
        public static int HitPhysicalArea = 100;
        public static int HitPoisonArea = 100;
        public static int HitLeechStam = 100;
        public static int LowerManaCost = 20;
        public static int LowerRegCost = 20;
        public static int Luck = 500;
        public static int ReflectPhysical = 25;
        public static int RegenHits = 5;
        public static int RegenMana = 5;
        public static int RegenStam = 5;
        public static int ResistColdBonus = 50;
        public static int ResistEnergyBonus = 50;
        public static int ResistFireBonus = 50;
        public static int ResistPhysicalBonus = 50;
        public static int ResistPoisonBonus = 50;
        public static int SelfRepair = 5;
        public static int SpellDamage = 25;
        public static int WeaponSpeed = 25;
        public static int WeaponDamage = 25;
        /**********************************************************
        * DO NOT MODIFY THESE UNLESS YOU KNOW WHAT YOU ARE DOING *
        * ********************************************************/
        public static int MageArmor = 1;
        public static int NightSight = 1;
        public static int SpellChanneling = 1;
        public static int UseBestSkill = 1;
    }
    public static class AttributeModifierValue //For each item
    {
        public static int AttackChance = 5;
        public static int BonusDex = 2;
        public static int BonusHits = 2;
        public static int BonusInt = 2;
        public static int BonusMana = 2;
        public static int BonusStam = 2;
        public static int BonusStr = 2;
        public static int CastSpeed = 1;
        public static int CastRecovery = 1;
        public static int DefendChance = 5;
        public static int DurabilityBonus = 25;
        public static int EnhancePotions = 2;
        public static int HitColdArea = 5;
        public static int HitDispel = 5;
        public static int HitEnergyArea = 5;
        public static int HitFireArea = 5;
        public static int HitFireball = 5;
        public static int HitHarm = 5;
        public static int HitLeechHits = 5;
        public static int HitLightning = 5;
        public static int HitLowerAttack = 5;
        public static int HitLowerDefend = 5;
        public static int HitMagicArrow = 5;
        public static int HitLeechMana = 5;
        public static int HitPhysicalArea = 5;
        public static int HitPoisonArea = 5;
        public static int HitLeechStam = 5;
        public static int LowerManaCost = 2;
        public static int LowerRegCost = 2;
        public static int Luck = 25;
        public static int ReflectPhysical = 5;
        public static int RegenHits = 1;
        public static int RegenMana = 1;
        public static int RegenStam = 1;
        public static int ResistColdBonus = 5;
        public static int ResistEnergyBonus = 5;
        public static int ResistFireBonus = 5;
        public static int ResistPhysicalBonus = 5;
        public static int ResistPoisonBonus = 5;
        public static int SelfRepair = 1;
        public static int SpellDamage = 5;
        public static int WeaponSpeed = 5;
        public static int WeaponDamage = 5;
        /**********************************************************
        * DO NOT MODIFY THESE UNLESS YOU KNOW WHAT YOU ARE DOING *
        * ********************************************************/
        public static LootType Bless = LootType.Blessed;
        public static LootType Curse = LootType.Cursed;
        public static int MageArmor = 1;
        public static int NightSight = 1;
        public static int SpellChanneling = 1;
        public static int UseBestSkill = 1;
    }
    public static class AttributeName //For each item
    {
        public static string AttackChance = "hit chance increase";
        public static string Bless = "blessed";
        public static string BonusDex = "dexterity bonus";
        public static string BonusHits = "hit point increase";
        public static string BonusInt = "intelligence bonus";
        public static string BonusMana = "mana increase";
        public static string BonusStam = "stamina increase";
        public static string BonusStr = "strength bonus";
        public static string CastSpeed = "faster cating";
        public static string CastRecovery = "faster cast recovery";
        public static string DefendChance = "defense chance increase";
        public static string DurabilityBonus = "durability";
        public static string EnhancePotions = "enhance potions";
        public static string HitColdArea = "hit cold area";
        public static string HitDispel = "hit dispel";
        public static string HitEnergyArea = "hit energy area";
        public static string HitFireArea = "hit fire area";
        public static string HitFireball = "hit fireball";
        public static string HitHarm = "hit harm";
        public static string HitLeechHits = "hit life leech";
        public static string HitLightning = "hit lightning";
        public static string HitLowerAttack = "hit lower attack";
        public static string HitLowerDefend = "hit lower defense";
        public static string HitMagicArrow = "hit magic arrow";
        public static string HitLeechMana = "hit leech mana";
        public static string HitPhysicalArea = "hit physical area";
        public static string HitPoisonArea = "hit poison area";
        public static string HitLeechStam = "hit stamina leech";
        public static string LowerManaCost = "lower mana cost";
        public static string LowerRegCost = "lower reagent cost";
        public static string Luck = "luck";
        public static string MageArmor = "mage armor";
        public static string NightSight = "night sight";
        public static string ReflectPhysical = "reflect physical damage";
        public static string RegenHits = "hit point regeration";
        public static string RegenMana = "mana regeneration";
        public static string RegenStam = "stamina regeneration";
        public static string ResistColdBonus = "cold resist";
        public static string ResistEnergyBonus = "energy resist";
        public static string ResistFireBonus = "fire resist";
        public static string ResistPhysicalBonus = "physical resist";
        public static string ResistPoisonBonus = "poison resist";
        public static string SelfRepair = "self repair";
        public static string SpellChanneling = "spell channeling";
        public static string SpellDamage = "spell damage increase";
        public static string UseBestSkill = "use best weapon skill";
        public static string WeaponSpeed = "swing speed increase";
        public static string WeaponDamage = "damage increase";
    }
    #endregion

    #region Slayer Deed Settings
    public static class SlayerNameModifier //For each item
    {
        public static SlayerName ArachnidDoom = SlayerName.ArachnidDoom;
        public static SlayerName BalronDamnation = SlayerName.BalronDamnation;
        public static SlayerName BloodDrinking = SlayerName.BloodDrinking;
        public static SlayerName DaemonDismissal = SlayerName.DaemonDismissal;
        public static SlayerName DragonSlaying = SlayerName.DragonSlaying;
        public static SlayerName EarthShatter = SlayerName.EarthShatter;
        public static SlayerName ElementalBan = SlayerName.ElementalBan;
        public static SlayerName ElementalHealth = SlayerName.ElementalHealth;
        public static SlayerName Exorcism = SlayerName.Exorcism;
        public static SlayerName Fey = SlayerName.Fey;
        public static SlayerName FlameDousing = SlayerName.FlameDousing;
        public static SlayerName GargoylesFoe = SlayerName.GargoylesFoe;
        public static SlayerName LizardmanSlaughter = SlayerName.LizardmanSlaughter;
        public static SlayerName None = SlayerName.None;
        public static SlayerName OgreTrashing = SlayerName.OgreTrashing;
        public static SlayerName Ophidian = SlayerName.Ophidian;
        public static SlayerName OrcSlaying = SlayerName.OrcSlaying;
        public static SlayerName Repond = SlayerName.Repond;
        public static SlayerName ReptilianDeath = SlayerName.ReptilianDeath;
        public static SlayerName ScorpionsBane = SlayerName.ScorpionsBane;
        public static SlayerName Silver = SlayerName.Silver;
        public static SlayerName SnakesBane = SlayerName.SnakesBane;
        public static SlayerName SpidersDeath = SlayerName.SpidersDeath;
        public static SlayerName SummerWind = SlayerName.SummerWind;
        public static SlayerName Terathan = SlayerName.Terathan;
        public static SlayerName TrollSlaughter = SlayerName.TrollSlaughter;
        public static SlayerName Vacuum = SlayerName.Vacuum;
        public static SlayerName WaterDissipation = SlayerName.WaterDissipation;

    }
    public static class SlayerNameName
    {
        public static string ArachnidDoom = "arachnid slayer";
        public static string BalronDamnation = "balron damnation"; //Not working
        public static string BloodDrinking = "blood elemental slayer";
        public static string DaemonDismissal = "daemon dismissal"; //Not working
        public static string DragonSlaying = "dragon slayer";
        public static string EarthShatter = "earth elemental slayer";
        public static string ElementalBan = "elemental slayer";
        public static string ElementalHealth = "poison elemental slayer";
        public static string Exorcism = "demon slayer";
        public static string Fey = "fey slayer";
        public static string FlameDousing = "fire elemental slayer";
        public static string GargoylesFoe = "gargoyle slayer";
        public static string LizardmanSlaughter = "lizardman slayer";
        public static string None = "slayer removal";
        public static string OgreTrashing = "ogre slayer";
        public static string Ophidian = "ophidian slayer";
        public static string OrcSlaying = "orc slayer";
        public static string Repond = "repond slayer";
        public static string ReptilianDeath = "reptile slayer";
        public static string ScorpionsBane = "scorpion slayer";
        public static string Silver = "undead slayer";
        public static string SnakesBane = "snake slayer";
        public static string SpidersDeath = "spider slayer";
        public static string SummerWind = "snow elemental slayer";
        public static string Terathan = "terathan slayer";
        public static string TrollSlaughter = "troll slayer";
        public static string Vacuum = "air elemental slayer";
        public static string WaterDissipation = "water elemental slayer";
    }
    #endregion

    #region Animations
    public static class AttributeModifiyAnimation
    {
        public static void DoAnimation(Mobile from)
        {
            from.SendMessage("You feel a surge of magic!");

            Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
            Effects.PlaySound(from.Location, from.Map, 0x243);

            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 4, from.Y - 6, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            Effects.SendMovingParticles(new Entity(Serial.Zero, new Point3D(from.X - 6, from.Y - 4, from.Z + 15), from.Map), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

            Effects.SendTargetParticles(from, 0x375A, 30, 30, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);
        }
    }
    #endregion

    #region Equipment Types Allowed
    public static class EquipmentTypes
    {
        public static Type[] AllowedItems = new Type[] 
        {
            typeof(KiastasBaseArmor),
            typeof(KiastasBaseClothing),
            typeof(KiastasBaseJewel),
            typeof(KiastasBaseShield),
            typeof(KiastasBaseWeapon)
        };

        public static int Length = AllowedItems.Length;
    }
    #endregion

    public static partial class Misc
    {
        public static int DeedHue = 1150;
        public static int DeedItemID = 0x14F0;
        public static bool DeedIsStackable = false;
        public static double DeedWeight = 1.0;
    }
}
