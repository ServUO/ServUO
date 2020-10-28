using Server.Items;

namespace Server.Engines.VendorSearching
{
    public class SearchCriteriaCategory
    {
        public Category Category { get; }
        public int PageID { get; }
        public int Cliloc { get; }
        public SearchCriterionEntry[] Criteria { get; }

        public SearchCriteriaCategory(Category cat, int pid, int cliloc, SearchCriterionEntry[] criteria)
        {
            Category = cat;
            PageID = pid;
            Cliloc = cliloc;
            Criteria = criteria;
        }

        public static SearchCriteriaCategory[] AllCategories = new SearchCriteriaCategory[]
        {
            new SearchCriteriaCategory(Category.PriceRange, 2, 1154512, new SearchCriterionEntry[] { /* Price Range */
            }),
            new SearchCriteriaCategory(Category.Equipment, 3, 1154531, new[] // Equipment
            { 
				new SearchCriterionEntry(Layer.Shoes, 1154602),
                new SearchCriterionEntry(Layer.Pants, 1154603),
                new SearchCriterionEntry(Layer.Shirt, 1154604),
                new SearchCriterionEntry(Layer.Helm, 1154605),
                new SearchCriterionEntry(Layer.Gloves, 1154606),
                new SearchCriterionEntry(Layer.Ring, 1154607),
                new SearchCriterionEntry(Layer.Talisman, 1154608),
                new SearchCriterionEntry(Layer.Neck, 1154609),
                new SearchCriterionEntry(Layer.Waist, 1154611),
                new SearchCriterionEntry(Layer.InnerTorso, 1154612),
                new SearchCriterionEntry(Layer.Bracelet, 1154613),
                new SearchCriterionEntry(Layer.MiddleTorso, 1154616),
                new SearchCriterionEntry(Layer.Earrings, 1154617),
                new SearchCriterionEntry(Layer.Arms, 1154618),
                new SearchCriterionEntry(Layer.Cloak, 1154619),
                new SearchCriterionEntry(Layer.OuterTorso, 1154621),
                new SearchCriterionEntry(Layer.OuterLegs, 1154622),
            }),
            new SearchCriteriaCategory(Category.Combat, 4, 1154541, new[] // Combat
            { 
				new SearchCriterionEntry(AosAttribute.WeaponDamage, 1079760, 1060401),
                new SearchCriterionEntry(AosAttribute.DefendChance, 1075620, 1060408),
                new SearchCriterionEntry(AosAttribute.AttackChance, 1075616, 1060415),
                new SearchCriterionEntry(AosAttribute.WeaponSpeed, 1075629, 1060486),
                new SearchCriterionEntry(AosArmorAttribute.SoulCharge, 1116536, 1113630),
                new SearchCriterionEntry(AosWeaponAttribute.UseBestSkill, 1079592),
                new SearchCriterionEntry(AosWeaponAttribute.ReactiveParalyze, 1154660),
                new SearchCriterionEntry(ExtendedWeaponAttribute.AssassinHoned, 1152206),
                new SearchCriterionEntry("SearingWeapon", 1151183),
                new SearchCriterionEntry(AosWeaponAttribute.BloodDrinker, 1113591),
                new SearchCriterionEntry(AosWeaponAttribute.BattleLust, 1113710),
                new SearchCriterionEntry(AosAttribute.BalancedWeapon, 1072792),
                new SearchCriterionEntry(ExtendedWeaponAttribute.Focus, 1150018),
                new SearchCriterionEntry(SAAbsorptionAttribute.EaterFire, 1154662, 1113593),
                new SearchCriterionEntry(SAAbsorptionAttribute.EaterCold, 1154663, 1113594),
                new SearchCriterionEntry(SAAbsorptionAttribute.EaterPoison, 1154664, 1113595),
                new SearchCriterionEntry(SAAbsorptionAttribute.EaterEnergy, 1154665, 1113596),
                new SearchCriterionEntry(SAAbsorptionAttribute.EaterKinetic, 1154666, 1113597),
                new SearchCriterionEntry(SAAbsorptionAttribute.EaterDamage, 1154667, 1113598),
            }),
            new SearchCriteriaCategory(Category.Casting, 5, 1154538, new[] // Casting
            { 
				new SearchCriterionEntry(SAAbsorptionAttribute.ResonanceFire, 1154655, 1113691),
                new SearchCriterionEntry(SAAbsorptionAttribute.ResonanceCold, 1154656, 1113692),
                new SearchCriterionEntry(SAAbsorptionAttribute.ResonancePoison, 1154657, 1113693),
                new SearchCriterionEntry(SAAbsorptionAttribute.ResonanceEnergy, 1154658, 1113692),
                new SearchCriterionEntry(SAAbsorptionAttribute.ResonanceKinetic, 1154659, 1113695),
                new SearchCriterionEntry(AosAttribute.SpellDamage, 1075628, 1060483),
                new SearchCriterionEntry(SAAbsorptionAttribute.CastingFocus, 1116535, 1113696),
                new SearchCriterionEntry(AosAttribute.CastRecovery, 1075618, 1060412),
                new SearchCriterionEntry(AosAttribute.CastSpeed, 1075617, 1060413),
                new SearchCriterionEntry(AosAttribute.LowerManaCost, 1075621, 1060433),
                new SearchCriterionEntry(AosAttribute.LowerRegCost, 1075625, 1060434),
                new SearchCriterionEntry(AosWeaponAttribute.MageWeapon, 1079759, 1060438),
                new SearchCriterionEntry(AosArmorAttribute.MageArmor, 1079758),
                new SearchCriterionEntry(AosAttribute.SpellChanneling, 1079766),
            }),
            new SearchCriteriaCategory(Category.Misc, 6, 1154647, new[] // Miscellaneous
            {
				new SearchCriterionEntry(Misc.ExcludeFel, 1154646),
                new SearchCriterionEntry(Misc.GargoyleOnly, 1154648),
                new SearchCriterionEntry(Misc.NotGargoyleOnly, 1154704),
                new SearchCriterionEntry(Misc.ElvesOnly, 1154650),
                new SearchCriterionEntry(Misc.NotElvesOnly, 1154703),
                new SearchCriterionEntry(Misc.FactionItem, 1154661),
                new SearchCriterionEntry(Misc.PromotionalToken, 1154682),
                new SearchCriterionEntry(AosAttribute.NightSight, 1075643),
                new SearchCriterionEntry(Misc.Cursed, 1116639),
                new SearchCriterionEntry(Misc.NotCursed, 1154701),
                new SearchCriterionEntry(Misc.CannotRepair, 1151826),
                new SearchCriterionEntry(Misc.NotCannotBeRepaired, 1154705),
                new SearchCriterionEntry(Misc.Brittle, 1116209),
                new SearchCriterionEntry(Misc.NotBrittle, 1154702),
                new SearchCriterionEntry(Misc.Antique, 1152714),
                new SearchCriterionEntry(Misc.NotAntique, 1156479),
                new SearchCriterionEntry(AosAttribute.EnhancePotions, 1075624, 1060411),
                new SearchCriterionEntry(AosArmorAttribute.LowerStatReq, 1079757, 1060435),
                new SearchCriterionEntry(AosAttribute.Luck, 1061153, 1060436),
                new SearchCriterionEntry(AosAttribute.ReflectPhysical, 1075626, 1060442),
                new SearchCriterionEntry(AosArmorAttribute.SelfRepair, 1079709, 1060450),
                new SearchCriterionEntry("ArtifactRarity", 1154693, 1061078),
            }),
            new SearchCriteriaCategory(Category.DamageType, 9, 1154535, new[] // Damage Type
            {
				new SearchCriterionEntry(AosElementAttribute.Physical, 1151800, 1060403),
                new SearchCriterionEntry(AosElementAttribute.Cold, 1151802, 1060404),
                new SearchCriterionEntry(AosElementAttribute.Fire, 1151801, 1060405),
                new SearchCriterionEntry(AosElementAttribute.Poison, 1151803, 1060406),
                new SearchCriterionEntry(AosElementAttribute.Energy, 1151804, 1060407),
            }),
            new SearchCriteriaCategory(Category.HitSpell, 10, 1154536, new[] // Hit Spell
            {
				new SearchCriterionEntry(AosWeaponAttribute.HitDispel, 1079702, 1060417),
                new SearchCriterionEntry(AosWeaponAttribute.HitFireball, 1079703, 1060420),
                new SearchCriterionEntry(AosWeaponAttribute.HitHarm, 1079704, 1060421),
                new SearchCriterionEntry(AosWeaponAttribute.HitCurse, 1154673, 1113712),
                new SearchCriterionEntry(AosWeaponAttribute.HitLeechHits, 1079698, 1060422),
                new SearchCriterionEntry(AosWeaponAttribute.HitLightning, 1079705, 1060423),
                new SearchCriterionEntry("WeaponVelocity", 1080416, 1072793),
                new SearchCriterionEntry(AosWeaponAttribute.HitLowerAttack, 1079699, 1060424),
                new SearchCriterionEntry(AosWeaponAttribute.HitLowerDefend, 1079700, 1060425),
                new SearchCriterionEntry(AosWeaponAttribute.HitMagicArrow, 1079706, 1060426),
                new SearchCriterionEntry(AosWeaponAttribute.HitLeechMana, 1079701, 1060427),
                new SearchCriterionEntry(AosWeaponAttribute.HitLeechStam, 1079707, 1060430),
                new SearchCriterionEntry(AosWeaponAttribute.HitFatigue, 1154668, 1113700),
                new SearchCriterionEntry(AosWeaponAttribute.HitManaDrain, 1154669, 1113699),
                new SearchCriterionEntry(AosWeaponAttribute.SplinteringWeapon, 1154670, 1112857),
                new SearchCriterionEntry(ExtendedWeaponAttribute.Bane, 1154671),
            }),
            new SearchCriteriaCategory(Category.HitArea, 11, 1154537, new[] // Hit Area
            {
				new SearchCriterionEntry(AosWeaponAttribute.HitColdArea, 1079693, 1060416),
                new SearchCriterionEntry(AosWeaponAttribute.HitEnergyArea, 1079694, 1060418),
                new SearchCriterionEntry(AosWeaponAttribute.HitFireArea, 1079695, 1060419),
                new SearchCriterionEntry(AosWeaponAttribute.HitPhysicalArea, 1079696, 1060428),
                new SearchCriterionEntry(AosWeaponAttribute.HitPoisonArea, 1079697, 1060429),
            }),
            new SearchCriteriaCategory(Category.Resists, 12, 1154539, new[] // Resists
            {
				new SearchCriterionEntry(AosElementAttribute.Cold, 1079761, 1060445),
                new SearchCriterionEntry(AosElementAttribute.Energy, 1079762, 1060446),
                new SearchCriterionEntry(AosElementAttribute.Fire, 1079763, 1060447),
                new SearchCriterionEntry(AosElementAttribute.Physical, 1079764, 1060448),
                new SearchCriterionEntry(AosElementAttribute.Poison, 1079765, 1060449),
            }),
            new SearchCriteriaCategory(Category.Stats, 13, 1154540, new[] // Stats
            {
				new SearchCriterionEntry(AosAttribute.BonusStr, 1079767, 1060485),
                new SearchCriterionEntry(AosAttribute.BonusDex, 1079732, 1060409),
                new SearchCriterionEntry(AosAttribute.BonusInt, 1079756, 1060432),
                new SearchCriterionEntry(AosAttribute.BonusHits, 1079404, 1060431),
                new SearchCriterionEntry(AosAttribute.BonusStam, 1079405, 1060484),
                new SearchCriterionEntry(AosAttribute.BonusMana, 1079406, 1060439),
                new SearchCriterionEntry(AosAttribute.RegenHits, 1075627, 1060444),
                new SearchCriterionEntry(AosAttribute.RegenStam, 1079411, 1060443),
                new SearchCriterionEntry(AosAttribute.RegenMana, 1079410, 1060440),
            }),
            new SearchCriteriaCategory(Category.Slayer1, 15, 1154683, new[] // Arachnid/Reptile Slayers
            {
				new SearchCriterionEntry(SlayerName.ReptilianDeath, 1079751),
                new SearchCriterionEntry(SlayerName.DragonSlaying, 1061284),
                new SearchCriterionEntry(SlayerName.LizardmanSlaughter, 1079738),
                new SearchCriterionEntry(SlayerName.Ophidian, 1079740),
                new SearchCriterionEntry(SlayerName.SnakesBane, 1079744),
                new SearchCriterionEntry(SlayerName.ArachnidDoom, 1079747),
                new SearchCriterionEntry(SlayerName.ScorpionsBane, 1079743),
                new SearchCriterionEntry(SlayerName.SpidersDeath, 1079746),
                new SearchCriterionEntry(SlayerName.Terathan, 1079753),
            }),
            new SearchCriteriaCategory(Category.Slayer2, 16, 1154684, new[] // Repond/Undead Slayers
            {
				new SearchCriterionEntry(SlayerName.Repond, 1079750),
                new SearchCriterionEntry(TalismanSlayerName.Bat, 1072506),
                new SearchCriterionEntry(TalismanSlayerName.Bear, 1072504),
                new SearchCriterionEntry(TalismanSlayerName.Beetle, 1072508),
                new SearchCriterionEntry(TalismanSlayerName.Bird, 1072509),
                new SearchCriterionEntry(TalismanSlayerName.Bovine, 1072512),
                new SearchCriterionEntry(TalismanSlayerName.Flame, 1072511),
                new SearchCriterionEntry(TalismanSlayerName.Goblin, 1095010),
                new SearchCriterionEntry(TalismanSlayerName.Ice, 1072510),
                new SearchCriterionEntry(TalismanSlayerName.Mage, 1072507),
                new SearchCriterionEntry(SlayerName.OgreTrashing, 1079739),
                new SearchCriterionEntry(SlayerName.OrcSlaying, 1079741),
                new SearchCriterionEntry(SlayerName.TrollSlaughter, 1079754),
                new SearchCriterionEntry(TalismanSlayerName.Vermin, 1072505),
                new SearchCriterionEntry(TalismanSlayerName.Undead, 1079752),
                new SearchCriterionEntry(TalismanSlayerName.Wolf, 1075462),
            }),
            new SearchCriteriaCategory(Category.Slayer3, 17, 1154685, new[] // Demon/Fey/Elemental Slayers
            {
				new SearchCriterionEntry(SlayerName.Exorcism, 1079748),
                new SearchCriterionEntry(SlayerName.GargoylesFoe, 1079737),
                new SearchCriterionEntry(SlayerName.Fey, 1154652),
                new SearchCriterionEntry(SlayerName.ElementalBan, 1079749),
                new SearchCriterionEntry(SlayerName.Vacuum, 1079733),
                new SearchCriterionEntry(SlayerName.BloodDrinking, 1079734),
                new SearchCriterionEntry(SlayerName.EarthShatter, 1079735),
                new SearchCriterionEntry(SlayerName.FlameDousing, 1079736),
                new SearchCriterionEntry(SlayerName.ElementalHealth, 1079742),
                new SearchCriterionEntry(SlayerName.SummerWind, 1079745),
                new SearchCriterionEntry(SlayerName.WaterDissipation, 1079755),
            }),
            new SearchCriteriaCategory(Category.RequiredSkill, 18, 1154543, new[] // Required Skill
            {
				new SearchCriterionEntry(SkillName.Swords, 1044100),
                new SearchCriterionEntry(SkillName.Macing, 1044101),
                new SearchCriterionEntry(SkillName.Fencing, 1044102),
                new SearchCriterionEntry(SkillName.Archery, 1044091),
                new SearchCriterionEntry(SkillName.Throwing, 1044117),
            }),
            new SearchCriteriaCategory(Category.Skill1, 19, 1114255, new[] // Skill Group 1
            {
                new SearchCriterionEntry(SkillName.Swords, 1044100),
                new SearchCriterionEntry(SkillName.Fencing, 1044102),
                new SearchCriterionEntry(SkillName.Macing, 1044101),
                new SearchCriterionEntry(SkillName.Magery, 1044085),
                new SearchCriterionEntry(SkillName.Musicianship, 1044089),
            }),
            new SearchCriteriaCategory(Category.Skill2, 20, 1114256, new[] // Skill Group 2
            {
                new SearchCriterionEntry(SkillName.Wrestling, 1044103),
                new SearchCriterionEntry(SkillName.Tactics, 1044087),
                new SearchCriterionEntry(SkillName.AnimalTaming, 1044095),
                new SearchCriterionEntry(SkillName.Provocation, 1044082),
                new SearchCriterionEntry(SkillName.SpiritSpeak, 1044092),
            }),
            new SearchCriteriaCategory(Category.Skill3, 21, 1114257, new[] // Skill Group 3
            {
                new SearchCriterionEntry(SkillName.Stealth, 1044107),
                new SearchCriterionEntry(SkillName.Parry, 1044065),
                new SearchCriterionEntry(SkillName.Meditation, 1044106),
                new SearchCriterionEntry(SkillName.AnimalLore, 1044062),
                new SearchCriterionEntry(SkillName.Discordance, 1044075),
                new SearchCriterionEntry(SkillName.Focus, 1044110),
            }),
            new SearchCriteriaCategory(Category.Skill4, 22, 1114258, new[] // Skill Group 4
            {
                new SearchCriterionEntry(SkillName.Stealing, 1044093),
                new SearchCriterionEntry(SkillName.Anatomy, 1044061),
                new SearchCriterionEntry(SkillName.EvalInt, 1044076),
                new SearchCriterionEntry(SkillName.Veterinary, 1044099),
                new SearchCriterionEntry(SkillName.Necromancy, 1044109),
                new SearchCriterionEntry(SkillName.Bushido, 1044112),
                new SearchCriterionEntry(SkillName.Mysticism, 1044115),
            }),
            new SearchCriteriaCategory(Category.Skill5, 23, 1114259, new[] // Skill Group 5
            {
                new SearchCriterionEntry(SkillName.Healing, 1044077),
                new SearchCriterionEntry(SkillName.MagicResist, 1044086),
                new SearchCriterionEntry(SkillName.Peacemaking, 1044069),
                new SearchCriterionEntry(SkillName.Archery, 1044091),
                new SearchCriterionEntry(SkillName.Chivalry, 1044111),
                new SearchCriterionEntry(SkillName.Ninjitsu, 1044113),
                new SearchCriterionEntry(SkillName.Throwing, 1044117),
            }),
            new SearchCriteriaCategory(Category.Skill6, 24, 1114260, new[] // Skill Group 6
            {
                new SearchCriterionEntry(SkillName.Lumberjacking, 1044104),
                new SearchCriterionEntry(SkillName.Snooping, 1044088),
                new SearchCriterionEntry(SkillName.Mining, 1044105)
            }),
            new SearchCriteriaCategory(Category.Sort, 25, 1154695, new SearchCriterionEntry[] { /* Sort */
            }),
            new SearchCriteriaCategory(Category.Auction, 26, 1159353, new SearchCriterionEntry[] { /* Auction Item */
            }),
        };
    }

    public class SearchCriterionEntry
    {
        public object Object { get; }
        public int Cliloc { get; }
        public int PropCliloc { get; }

        public SearchCriterionEntry(object obj, int cliloc)
            : this(obj, cliloc, 0)
        {
        }

        public SearchCriterionEntry(object obj, int cliloc, int pcliloc)
        {
            Object = obj;
            Cliloc = cliloc;
            PropCliloc = pcliloc;
        }
    }
}
