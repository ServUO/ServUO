using Server.Engines.ShameRevamped;
using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a mud pie corpse")]
    public class MudPie : BaseCreature
    {
        public static Dictionary<Mobile, Timer> Table { get; private set; }

        [Constructable]
        public MudPie()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.2)
        {
            Name = "a mud pie";
            Body = 779;
            BaseSoundID = 422;

            Hue = 2012;

            SetStr(140, 210);
            SetDex(70, 100);
            SetInt(90, 110);

            SetHits(280, 340);

            SetDamage(9, 12);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 30, 45);
            SetResistance(ResistanceType.Fire, 35, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 40);

            SetSkill(SkillName.MagicResist, 65, 85);
            SetSkill(SkillName.Tactics, 65, 85);
            SetSkill(SkillName.Wrestling, 65, 85);

            Fame = 500;
            Karma = -500;

            SetSpecialAbility(SpecialAbility.StickySkin);
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
            AddLoot(LootPack.MageryRegs, Utility.RandomMinMax(1, 2));
            AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 2));
            AddLoot(LootPack.LootItem<ExecutionersCap>());
            AddLoot(LootPack.LootItem<ExecutionersCap>(33.0));
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 5.0, 1, false, false));
        }

        public MudPie(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a stone elemental corpse")]
    public class StoneElemental : EarthElemental
    {
        [Constructable]
        public StoneElemental()
        {
            Name = "a stone elemental";
            Hue = 2401;

            SetStr(140, 210);
            SetDex(80, 110);
            SetInt(90, 120);

            SetHits(900, 1000);

            SetDamage(15, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60, 65);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 55, 60);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Tactics, 80.0, 96.0);
            SetSkill(SkillName.Wrestling, 80.0, 97.0);

            Fame = 4000;
            Karma = -4000;

            SetSpecialAbility(SpecialAbility.ColossalRage);
        }

        public override int TreasureMapLevel => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.MageryRegs, Utility.RandomMinMax(1, 2));
            AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 2));
            AddLoot(LootPack.LootItem<Granite>());
            AddLoot(LootPack.LootItem<Sand>());
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 15.0, 1, false, false));
        }

        public StoneElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a cave troll corpse")]
    public class CaveTroll : Troll
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public ShameWall Wall { get; set; }

        [Constructable]
        public CaveTroll() : this(null)
        {
        }

        [Constructable]
        public CaveTroll(ShameWall wall)
        {
            Name = "a cave troll";
            BodyValue = 0x1;
            FightMode = FightMode.Aggressor;

            if (wall != null)
                Title = "the wall guardian";

            Hue = 638;
            Wall = wall;

            SetStr(180, 210);
            SetDex(107, 205);
            SetInt(40, 70);

            SetHits(638, 978);

            SetDamage(15, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 70, 90);
            SetSkill(SkillName.Tactics, 80, 110);
            SetSkill(SkillName.Wrestling, 80, 110);
            SetSkill(SkillName.DetectHidden, 100.0);

            Fame = 3500;
            Karma = -3500;

            SetWeaponAbility(WeaponAbility.ArmorIgnore);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.Gems);
            AddLoot(LootPack.LootItem<Saltpeter>(100.0, Utility.RandomMinMax(1, 5), false, true));
            AddLoot(LootPack.LootItem<Potash>(100.0, Utility.RandomMinMax(1, 5), false, true));
            AddLoot(LootPack.LootItem<Charcoal>(100.0, Utility.RandomMinMax(1, 5), false, true));
            AddLoot(LootPack.LootItem<BlackPowder>(100.0, Utility.RandomMinMax(1, 5), false, true));
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 10.0, 1, false, false));
        }

        public override MeatType MeatType => MeatType.Ribs;
        public override int Meat => 2;

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Wall != null)
                Wall.OnTrollKilled();
        }

        public override int TreasureMapLevel => 1;

        public CaveTroll(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a clay golem corpse")]
    public class ClayGolem : Golem
    {
        [Constructable]
        public ClayGolem()
        {
            Name = "a clay golem";
            Hue = 654;

            SetStr(450, 600);
            SetDex(100, 150);
            SetInt(100, 150);

            SetHits(700, 900);

            SetDamage(13, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 99);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 150, 200);
            SetSkill(SkillName.Tactics, 80, 120);
            SetSkill(SkillName.Wrestling, 80, 110);
            SetSkill(SkillName.Parry, 70, 80);
            SetSkill(SkillName.DetectHidden, 70.0, 80.0);

            Fame = 4500;
            Karma = -4500;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItem<ExecutionersCap>());
            AddLoot(LootPack.LootItemCallback(SpawnGears, 5.0, 1, false, false));
            AddLoot(LootPack.LootItemCallback(CheckSpawnCrystal, 20.0, 1, false, false));
        }

        public static Item CheckSpawnCrystal(IEntity e)
        {
            if (Region.Find(e.Location, e.Map).IsPartOf("Shame"))
            {
                return new ShameCrystal();
            }

            return null;
        }

        public ClayGolem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a greater earth elemental corpse")]
    public class GreaterEarthElemental : EarthElemental
    {
        [Constructable]
        public GreaterEarthElemental()
        {
            Name = "a greater earth elemental";
            Hue = 1143;

            SetHits(500, 600);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 65);
            SetResistance(ResistanceType.Fire, 35, 45);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 40, 70);
            SetSkill(SkillName.Tactics, 70, 90);
            SetSkill(SkillName.Wrestling, 80, 95);

            Fame = 2500;
            Karma = -2500;

            SetSpecialAbility(SpecialAbility.ColossalRage);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 20.0, 1, false, false));
        }

        public GreaterEarthElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a mud elemental corpse")]
    public class MudElemental : EarthElemental
    {
        [Constructable]
        public MudElemental()
        {
            Name = "a mud elemental";
            Hue = 542;

            SetStr(400, 550);
            SetHits(650, 850);
            SetDamage(17, 19);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 50, 65);
            SetResistance(ResistanceType.Fire, 55, 65);
            SetResistance(ResistanceType.Cold, 45, 50);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 100);
            SetSkill(SkillName.Tactics, 100);
            SetSkill(SkillName.Wrestling, 120);
            SetSkill(SkillName.Parry, 120);

            Fame = 3500;
            Karma = -3500;

            SetSpecialAbility(SpecialAbility.ColossalRage);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 20.0, 1, false, false));
            AddLoot(LootPack.LootItem<FertileDirt>());
            AddLoot(LootPack.LootItem<ExecutionersCap>());
        }

        public MudElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GreaterAirElemental : AirElemental
    {
        [Constructable]
        public GreaterAirElemental()
        {
            SetStr(250, 315);
            SetHits(800, 900);
            SetDamage(15, 17);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 75, 85);
            SetResistance(ResistanceType.Fire, 55, 65);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.MagicResist, 100, 120);
            SetSkill(SkillName.Tactics, 100, 120);
            SetSkill(SkillName.Wrestling, 100, 120);
            SetSkill(SkillName.Magery, 100, 120);
            SetSkill(SkillName.EvalInt, 100, 120);

            Fame = 4500;
            Karma = -4500;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 20.0, 1, false, false));
        }

        public GreaterAirElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a molten earth elemental corpse")]
    public class MoltenEarthElemental : GreaterEarthElemental
    {
        [Constructable]
        public MoltenEarthElemental()
        {
            Hue = 442;
            Name = "a molten earth elemental";

            SetStr(400, 550);
            SetHits(1200, 1400);
            SetDamage(17, 19);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 50, 70);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 100);
            SetSkill(SkillName.Tactics, 100);
            SetSkill(SkillName.Wrestling, 120);
            SetSkill(SkillName.Parry, 120);

            Fame = 5000;
            Karma = -5000;

            SetSpecialAbility(SpecialAbility.SearingWounds);
            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 10.0, 1, false, false));
        }

        public MoltenEarthElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a flame elemental corpse")]
    public class LesserFlameElemental : BaseCreature, IAuraCreature
    {
        [Constructable]
        public LesserFlameElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.4, 0.2)
        {
            Name = "a flame elemental";
            Body = 15;
            BaseSoundID = 838;
            Hue = 1161;

            SetStr(420, 460);
            SetDex(160, 210);
            SetInt(120, 190);

            SetHits(700, 800);
            SetMana(1000, 1200);

            SetDamage(13, 15);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 75);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 100);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.MagicResist, 90, 140);
            SetSkill(SkillName.Tactics, 90, 130.0);
            SetSkill(SkillName.Wrestling, 90, 120);
            SetSkill(SkillName.Magery, 100, 145);
            SetSkill(SkillName.EvalInt, 90, 140);
            SetSkill(SkillName.Meditation, 80, 120);
            SetSkill(SkillName.Parry, 100, 120);

            Fame = 3500;
            Karma = -3500;

            SetSpecialAbility(SpecialAbility.DragonBreath);
            SetAreaEffect(AreaEffect.AuraDamage);
        }

        public void AuraEffect(Mobile m)
        {
            m.SendLocalizedMessage(1008112); // The intense heat is damaging you!
        }

        public override int TreasureMapLevel => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 10.0, 1, false, false));
            AddLoot(LootPack.LootItem<SulfurousAsh>(5, false));
        }

        public LesserFlameElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a wind elemental corpse")]
    public class LesserWindElemental : BaseCreature
    {
        [Constructable]
        public LesserWindElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.4, 0.2)
        {
            Name = "a wind elemental";
            Body = 13;
            BaseSoundID = 655;
            Hue = 33765;

            SetStr(370, 460);
            SetDex(160, 250);
            SetInt(150, 220);

            SetHits(2500, 2600);
            SetMana(1000, 1300);

            SetDamage(15, 17);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 55, 65);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 60, 75);

            SetSkill(SkillName.MagicResist, 60, 80);
            SetSkill(SkillName.Tactics, 60, 80.0);
            SetSkill(SkillName.Wrestling, 60, 80);
            SetSkill(SkillName.Magery, 60, 80);
            SetSkill(SkillName.EvalInt, 60, 80);

            Fame = 3500;
            Karma = -3500;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 10.0, 1, false, false));
        }

        public LesserWindElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("an eternal gazer corpse")]
    public class EternalGazer : ElderGazer
    {
        [Constructable]
        public EternalGazer()
        {
            Name = "an eternal gazer";
            SetStr(450, 600);
            SetDex(125, 165);
            SetInt(350, 550);

            SetHits(7250, 7600);
            SetMana(2500, 2900);
            SetDamage(18, 21);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 70, 75);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.MagicResist, 125, 140);
            SetSkill(SkillName.Tactics, 115, 130);
            SetSkill(SkillName.Wrestling, 110, 130);
            SetSkill(SkillName.Anatomy, 75, 90);
            SetSkill(SkillName.Magery, 120, 130);
            SetSkill(SkillName.EvalInt, 120, 130);
        }

        public override MeatType MeatType => MeatType.Ribs;
        public override int Meat => 1;

        public override void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature && (((BaseCreature)from).Summoned || ((BaseCreature)from).Controlled))
                damage /= 2;

            base.AlterSpellDamageFrom(from, ref damage);
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature && (((BaseCreature)from).Summoned || ((BaseCreature)from).Controlled))
                damage /= 2;

            base.AlterMeleeDamageFrom(from, ref damage);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 15.0, 1, false, false));
        }

        public EternalGazer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a burning mage corpse")]
    public class BurningMage : BaseCreature
    {
        public static Dictionary<Mobile, Timer> Table { get; private set; }

        [Constructable]
        public BurningMage() : base(AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.4, 0.2)
        {
            Name = NameList.RandomName("male");
            Title = "the burning";
            SetStr(100, 125);

            BodyValue = 0x190;
            Hue = 1281;

            SetHits(3000);
            SetMana(600, 800);
            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 125, 140);
            SetSkill(SkillName.Tactics, 100, 120);
            SetSkill(SkillName.Wrestling, 110, 130);
            SetSkill(SkillName.Magery, 120, 130);
            SetSkill(SkillName.EvalInt, 120, 130);

            AddItem(new Robe(1156));
            AddItem(new Sandals());

            Utility.AssignRandomHair(this);

            Fame = 22000;
            Karma = -22000;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override bool CanRummageCorpses => true;
        public override bool AlwaysMurderer => true;

        public override void OnDamagedBySpell(Mobile from)
        {
            base.OnDamagedBySpell(from);

            if (!IsUnderEffects(from) && 0.50 > Utility.RandomDouble())
            {
                DoEffects(from);
            }
        }

        public static bool IsUnderEffects(Mobile from)
        {
            return from != null && Table != null && Table.ContainsKey(from);
        }

        public void DoEffects(Mobile from)
        {
            if (Table == null)
                Table = new Dictionary<Mobile, Timer>();

            if (!Table.ContainsKey(from))
            {
                Table[from] = Timer.DelayCall(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5), new TimerStateCallback(SapMana), new object[] { from, this });
                Table[from].Start();

                from.SendLocalizedMessage(1151482); // Your mana has been tainted!
                from.SendLocalizedMessage(1151485); // Your mana is being diverted.
            }
        }

        private static void SapMana(object o)
        {
            object[] objs = o as object[];
            Mobile from = objs[0] as Mobile;
            Mobile mob = objs[1] as Mobile;

            if (IsUnderEffects(from))
            {
                if (mob.Alive && from.Alive)
                {
                    from.SendLocalizedMessage(1151484); // You feel extra mana being drawn from you.
                    from.SendLocalizedMessage(1151481); // Channeling the corrupted mana has damaged you!

                    int toSap = Math.Min(from.Mana, Utility.RandomMinMax(30, 40));
                    from.Mana -= toSap;

                    AOS.Damage(from, mob, Math.Max(1, toSap / 10), false, 0, 0, 0, 0, 0, 0, 100, false, false, false);

                    if (0.5 > Utility.RandomDouble())
                        EndEffects(from);
                }
                else
                    EndEffects(from);
            }
        }

        public static void EndEffects(Mobile from)
        {
            if (IsUnderEffects(from))
            {
                Table[from].Stop();
                Table.Remove(from);

                from.SendLocalizedMessage(1151486); // Your mana is no longer being diverted.
                from.SendLocalizedMessage(1151483); // Your mana is no longer corrupted.
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(5, 20));
            AddLoot(LootPack.MageryRegs, 31);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 33.0, 5, false, false));
        }

        public BurningMage(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a crazed corpse")]
    public class CrazedMage : BaseCreature
    {
        public static Dictionary<Mobile, Timer> Table { get; private set; }

        [Constructable]
        public CrazedMage() : base(AIType.AI_Mystic, FightMode.Weakest, 10, 1, 0.4, 0.2)
        {
            Name = NameList.RandomName("male");
            Title = "the crazed";

            BodyValue = 0x190;
            SetStr(225, 400);

            SetHits(3500);
            SetMana(600, 800);
            SetDamage(15, 21);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60, 80);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 125, 140);
            SetSkill(SkillName.Tactics, 100, 120);
            SetSkill(SkillName.Wrestling, 140);
            SetSkill(SkillName.Anatomy, 100, 120);
            SetSkill(SkillName.Magery, 100, 110);
            SetSkill(SkillName.EvalInt, 100, 110);

            AddItem(new Robe(1157));
            AddItem(new Sandals());

            Utility.AssignRandomHair(this);
            Hue = Race.RandomSkinHue();

            Fame = 15000;
            Karma = -15000;
        }

        public override bool CanRummageCorpses => true;
        public override bool AlwaysMurderer => true;

        public override void OnDamagedBySpell(Mobile from)
        {
            base.OnDamagedBySpell(from);

            if (!IsUnderEffects(from) && 0.50 > Utility.RandomDouble())
            {
                DoEffects(from);
            }
        }

        public static bool IsUnderEffects(Mobile from)
        {
            return from != null && Table != null && Table.ContainsKey(from);
        }

        public void DoEffects(Mobile from)
        {
            if (Table == null)
                Table = new Dictionary<Mobile, Timer>();

            if (!Table.ContainsKey(from))
            {
                Table[from] = Timer.DelayCall(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5), new TimerStateCallback(SapMana), new object[] { from, this });
                Table[from].Start();

                from.SendLocalizedMessage(1151482); // Your mana has been tainted!
                from.SendLocalizedMessage(1151485); // Your mana is being diverted.
            }
        }

        private static void SapMana(object o)
        {
            object[] objs = o as object[];
            Mobile from = objs[0] as Mobile;
            Mobile mob = objs[1] as Mobile;

            if (IsUnderEffects(from))
            {
                if (mob.Alive && from.Alive)
                {
                    from.SendLocalizedMessage(1151484); // You feel extra mana being drawn from you.
                    from.SendLocalizedMessage(1151481); // Channeling the corrupted mana has damaged you!

                    int toSap = Math.Min(from.Mana, Utility.RandomMinMax(30, 40));
                    from.Mana -= toSap;

                    AOS.Damage(from, mob, Math.Max(1, toSap / 10), false, 0, 0, 0, 0, 0, 0, 100, false, false, false);

                    if (0.5 > Utility.RandomDouble())
                        EndEffects(from);
                }
                else
                    EndEffects(from);
            }
        }

        public static void EndEffects(Mobile from)
        {
            if (IsUnderEffects(from))
            {
                Table[from].Stop();
                Table.Remove(from);

                from.SendLocalizedMessage(1151486); // Your mana is no longer being diverted.
                from.SendLocalizedMessage(1151483); // Your mana is no longer corrupted.
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 33.0, 5, false, false));
        }

        public CrazedMage(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a corrupted mage corpse")]
    public class CorruptedMage : EvilMage
    {
        public static Dictionary<Mobile, Timer> Table { get; private set; }

        [Constructable]
        public CorruptedMage()
        {
            Title = "the corrupted mage";

            SetStr(150, 170);
            SetInt(100, 120);
            SetDex(110, 120);

            SetHits(1200, 1250);
            SetMana(800, 900);
            SetDamage(14, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 70);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.MagicResist, 115, 120);
            SetSkill(SkillName.Tactics, 110, 120);
            SetSkill(SkillName.Wrestling, 100, 110);
            SetSkill(SkillName.Magery, 120, 130);
            SetSkill(SkillName.EvalInt, 120, 130);
            SetSkill(SkillName.Meditation, 100, 110);
        }

        public override bool CanRummageCorpses => true;
        public override bool AlwaysMurderer => true;

        public override void OnDamagedBySpell(Mobile from)
        {
            base.OnDamagedBySpell(from);

            if (!IsUnderEffects(from) && 0.10 > Utility.RandomDouble())
            {
                DoEffects(from);
            }
        }

        public static bool IsUnderEffects(Mobile from)
        {
            return from != null && Table != null && Table.ContainsKey(from);
        }

        public void DoEffects(Mobile from)
        {
            if (Table == null)
                Table = new Dictionary<Mobile, Timer>();

            if (!Table.ContainsKey(from))
            {
                Table[from] = Timer.DelayCall(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1.5), new TimerStateCallback(SapMana), new object[] { from, this });
                Table[from].Start();

                from.SendLocalizedMessage(1151482); // Your mana has been tainted!
                from.SendLocalizedMessage(1151485); // Your mana is being diverted.
            }
        }

        private static void SapMana(object o)
        {
            object[] objs = o as object[];
            Mobile from = objs[0] as Mobile;
            Mobile mob = objs[1] as Mobile;

            if (IsUnderEffects(from))
            {
                if (mob.Alive && from.Alive)
                {
                    from.SendLocalizedMessage(1151484); // You feel extra mana being drawn from you.
                    from.SendLocalizedMessage(1151481); // Channeling the corrupted mana has damaged you!

                    int toSap = Math.Min(from.Mana, Utility.RandomMinMax(30, 40));
                    from.Mana -= toSap;

                    AOS.Damage(from, mob, Math.Max(1, toSap / 10), false, 0, 0, 0, 0, 0, 0, 100, false, false, false);

                    if (0.5 > Utility.RandomDouble())
                        EndEffects(from);
                }
                else
                    EndEffects(from);
            }
        }

        public static void EndEffects(Mobile from)
        {
            if (IsUnderEffects(from))
            {
                Table[from].Stop();
                Table.Remove(from);

                from.SendLocalizedMessage(1151486); // Your mana is no longer being diverted.
                from.SendLocalizedMessage(1151483); // Your mana is no longer corrupted.
            }
        }

        public override int TreasureMapLevel => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 33.0, 3, false, false));
        }

        public CorruptedMage(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a vile mage corpse")]
    public class VileMage : CorruptedMage
    {
        [Constructable]
        public VileMage()
        {
            Title = "the vile mage";

            SetStr(150, 170);
            SetInt(150, 170);
            SetDex(100, 110);

            SetHits(500, 900);
            SetMana(550, 600);
            SetDamage(11, 13);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 70);
            SetResistance(ResistanceType.Fire, 55, 65);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.MagicResist, 110, 115);
            SetSkill(SkillName.Tactics, 110, 115);
            SetSkill(SkillName.Wrestling, 100, 110);
            SetSkill(SkillName.Magery, 110, 115);
            SetSkill(SkillName.EvalInt, 115, 125);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 33.0, 3, false, false));
        }

        public VileMage(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a chaos vortex corpse")]
    public class ChaosVortex : BaseCreature
    {
        [Constructable]
        public ChaosVortex()
            : base(AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.4, 0.2)
        {
            Name = "a chaos vortex";
            Body = 164;
            Hue = 34212;

            SetStr(450);
            SetDex(200);
            SetInt(100);

            SetHits(27000);
            SetMana(0);

            SetDamage(21, 23);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 65, 75);
            SetResistance(ResistanceType.Cold, 65, 75);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.MagicResist, 100, 110);
            SetSkill(SkillName.Tactics, 110, 130);
            SetSkill(SkillName.Wrestling, 124, 140);

            Fame = 22500;
            Karma = -22500;
        }

        public override int GetAngerSound()
        {
            return 0x15;
        }

        public override int GetAttackSound()
        {
            return 0x28;
        }

        public override bool AlwaysMurderer => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;

        private DateTime NextTeleport { get; set; }

        public override void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature && (((BaseCreature)from).Summoned || ((BaseCreature)from).Controlled))
                damage /= 2;

            if (NextTeleport < DateTime.UtcNow)
                DoTeleport(from);

            base.AlterSpellDamageFrom(from, ref damage);
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature && (((BaseCreature)from).Summoned || ((BaseCreature)from).Controlled))
                damage /= 2;

            if (NextTeleport < DateTime.UtcNow)
                DoTeleport(from);

            base.AlterMeleeDamageFrom(from, ref damage);
        }

        public void DoTeleport(Mobile m)
        {
            if (!InRange(m.Location, 1))
            {
                int x, y, z = 0;
                Point3D p = Point3D.Zero;

                for (int i = 0; i < 25; i++)
                {
                    x = Utility.RandomMinMax(X - 1, X + 1);
                    y = Utility.RandomMinMax(Y - 1, Y + 1);
                    z = Map.GetAverageZ(x, y);

                    if (Map.CanSpawnMobile(x, y, z) && (x != X || y != Y))
                    {
                        p = new Point3D(x, y, z);
                        break;
                    }
                }

                if (p == Point3D.Zero)
                    p = Location;

                Point3D from = m.Location;

                Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                Effects.SendLocationParticles(EffectItem.Create(p, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                m.MoveToWorld(p, Map);

                m.PlaySound(0x1FE);
            }

            NextTeleport = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 33.0, 5, false, false));
        }

        public ChaosVortex(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("an unbound energy vortex corpse")]
    public class UnboundEnergyVortex : BaseCreature
    {
        [Constructable]
        public UnboundEnergyVortex() : base(AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.4, 0.2)
        {
            Name = "an unbound energy vortex";
            Body = 13;

            SetStr(450);
            SetDex(200);
            SetInt(100);

            SetHits(20000);
            SetMana(0);

            SetDamage(21, 23);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Energy, 100);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 65, 75);
            SetResistance(ResistanceType.Cold, 65, 75);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 100);

            SetSkill(SkillName.MagicResist, 100, 110);
            SetSkill(SkillName.Tactics, 110, 130);
            SetSkill(SkillName.Wrestling, 124, 140);

            Fame = 22500;
            Karma = -22500;
        }

        public override bool AlwaysMurderer => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public override int GetAngerSound()
        {
            return 0x15;
        }

        public override int GetAttackSound()
        {
            return 0x28;
        }

        private DateTime NextTeleport { get; set; }

        public override void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature && (((BaseCreature)from).Summoned || ((BaseCreature)from).Controlled))
                damage /= 2;

            if (NextTeleport < DateTime.UtcNow)
                DoTeleport(from);

            base.AlterSpellDamageFrom(from, ref damage);
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature && (((BaseCreature)from).Summoned || ((BaseCreature)from).Controlled))
                damage /= 2;

            if (NextTeleport < DateTime.UtcNow)
                DoTeleport(from);

            base.AlterMeleeDamageFrom(from, ref damage);
        }

        public void DoTeleport(Mobile m)
        {
            if (!InRange(m.Location, 1))
            {
                int x, y, z = 0;
                Point3D p = Point3D.Zero;

                for (int i = 0; i < 25; i++)
                {
                    x = Utility.RandomMinMax(X - 1, X + 1);
                    y = Utility.RandomMinMax(Y - 1, Y + 1);
                    z = Map.GetAverageZ(x, y);

                    if (Map.CanSpawnMobile(x, y, z) && (x != X || y != Y))
                    {
                        p = new Point3D(x, y, z);
                        break;
                    }
                }

                if (p == Point3D.Zero)
                    p = Location;

                Point3D from = m.Location;

                Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                Effects.SendLocationParticles(EffectItem.Create(p, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                m.MoveToWorld(p, Map);

                m.PlaySound(0x1FE);
            }

            NextTeleport = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 33.0, 5, false, false));
            AddLoot(LootPack.LootItem<VoidCore>(20.0));
        }

        public UnboundEnergyVortex(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [CorpseName("a diseased blood elemental")]
    public class DiseasedBloodElemental : BloodElemental
    {
        [Constructable]
        public DiseasedBloodElemental()
        {
            Name = "a diseased blood elemental";
            Body = 0x9F;
            Hue = 1779;

            SetStr(650, 750);
            SetDex(70, 80);
            SetInt(300, 400);

            SetHits(2500, 2700);
            SetMana(1400, 1600);

            SetDamage(19, 27);

            SetDamageType(ResistanceType.Poison, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 55, 65);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 110, 125);
            SetSkill(SkillName.Tactics, 130, 140);
            SetSkill(SkillName.Wrestling, 120, 140);
            SetSkill(SkillName.Poisoning, 100);
            SetSkill(SkillName.Magery, 110, 120);
            SetSkill(SkillName.EvalInt, 115, 130);
            SetSkill(SkillName.Meditation, 130, 155);
            SetSkill(SkillName.DetectHidden, 80.0);
            SetSkill(SkillName.Parry, 90.0, 100.0);

            int scrolls = Utility.RandomMinMax(4, 6);

            Fame = 8500;
            Karma = -8500;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.LifeLeech);
        }

        public override bool AutoDispel => true;
        public override double AutoDispelChance => 1.0;
        public override int TreasureMapLevel => 5;
        public override double TreasureMapChance => 1.0;
        public override Poison HitPoison => Poison.Lethal;
        public override Poison PoisonImmune => Poison.Parasitic;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(1, 8));
            AddLoot(LootPack.MageryRegs, Utility.RandomMinMax(7, 11));
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 33.0, 5, false, false));
        }

        public DiseasedBloodElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GreaterWaterElemental : WaterElemental
    {
        [Constructable]
        public GreaterWaterElemental()
        {
            SetStr(400, 500);
            SetDex(150, 160);
            SetInt(120, 140);

            SetHits(500, 600);
            SetMana(600, 700);

            SetDamage(14, 16);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 100, 110);
            SetSkill(SkillName.Tactics, 90, 110);
            SetSkill(SkillName.Wrestling, 90, 110);
            SetSkill(SkillName.Magery, 90, 110);
            SetSkill(SkillName.EvalInt, 90, 100);

            Fame = 3500;
            Karma = -3500;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 10.0, 1, false, false));
        }

        public GreaterWaterElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShameGreaterPoisonElemental : PoisonElemental
    {
        [Constructable]
        public ShameGreaterPoisonElemental()
        {
            Hue = 32854;

            SetStr(400, 500);
            SetDex(170, 175);
            SetInt(400, 450);

            SetHits(950, 1050);

            SetDamage(16, 19);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Poison, 90);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 110, 120);
            SetSkill(SkillName.Tactics, 90, 120);
            SetSkill(SkillName.Wrestling, 100, 115);
            SetSkill(SkillName.Magery, 90, 110);
            SetSkill(SkillName.EvalInt, 90, 100);
            SetSkill(SkillName.Meditation, 100, 120);
            SetSkill(SkillName.DetectHidden, 85.1);
            SetSkill(SkillName.Parry, 80, 100);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 1);
            AddLoot(LootPack.FilthyRich, 1);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 10.0, 5, false, false));
        }

        public ShameGreaterPoisonElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GreaterBloodElemental : BloodElemental
    {
        [Constructable]
        public GreaterBloodElemental()
        {
            SetStr(500, 600);
            SetDex(60, 90);
            SetInt(230, 350);

            SetHits(1350, 1500);
            SetHits(900, 1000);

            SetDamage(17, 27);

            SetDamageType(ResistanceType.Poison, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 115, 120);
            SetSkill(SkillName.Tactics, 100, 120);
            SetSkill(SkillName.Wrestling, 110, 120);
            SetSkill(SkillName.Magery, 80, 100);
            SetSkill(SkillName.EvalInt, 110, 120);
            SetSkill(SkillName.Meditation, 120, 140);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 1);
            AddLoot(LootPack.FilthyRich, 1);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 10.0, 5, false, false));
        }

        public GreaterBloodElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ShameEarthElemental : EarthElemental
    {
        [Constructable]
        public ShameEarthElemental()
        {
            SetHits(300, 400);
            SetDamage(11, 13);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 25, 30);
            SetResistance(ResistanceType.Poison, 25, 30);
            SetResistance(ResistanceType.Energy, 20, 25);

            SetSkill(SkillName.MagicResist, 65, 85);
            SetSkill(SkillName.Tactics, 65, 90);
            SetSkill(SkillName.Wrestling, 80, 85);

            Fame = 3500;
            Karma = -3500;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
            AddLoot(LootPack.LootItemCallback(ClayGolem.CheckSpawnCrystal, 8.0, 1, false, false));
        }

        public ShameEarthElemental(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
