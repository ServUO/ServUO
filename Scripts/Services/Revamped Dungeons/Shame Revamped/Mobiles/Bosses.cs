using System;
using Server;
using Server.Items;
using Server.Engines.ShameRevamped;
using Server.Engines.PartySystem;

namespace Server.Mobiles
{
    public class ShameGuardian : BaseCreature
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public ShameAltar Altar { get; set; }

        public ShameGuardian(AIType type)
            : base(type, FightMode.Aggressor, 10, 1, .4, .2)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Altar != null)
                Altar.OnGuardianKilled();

            c.DropItem(new ShameCrystal(Utility.RandomMinMax(3, 5)));

            Altar = null;
        }

        public override void Damage(int amount, Mobile from, bool informMount, bool checkfizzle)
        {
            if (Altar == null || Altar.Summoner == null)
                base.Damage(amount, from, informMount, checkfizzle);
            else
            {
                bool good = false;

                if (from == Altar.Summoner || (Altar.DeadLine > DateTime.UtcNow && Altar.DeadLine - DateTime.UtcNow < TimeSpan.FromMinutes(10)))
                    good = true;
                else if (ShameAltar.AllowParties)
                {
                    Server.Engines.PartySystem.Party p = Server.Engines.PartySystem.Party.Get(from);

                    foreach (PartyMemberInfo info in p.Members)
                    {
                        if (info.Mobile == from)
                        {
                            good = true;
                            break;
                        }
                    }
                }

                if (good)
                    base.Damage(amount, from, informMount, checkfizzle);
                else if (from != null)
                    from.SendLocalizedMessage(1151633); // You did not summon this champion, so you may not attack it at this time.
            }
        }

        public override bool AutoDispel { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public ShameGuardian(Serial serial)
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

    [CorpseName("an quartz elemental corpse")]
    public class QuartzElemental : ShameGuardian
    {
        [Constructable]
        public QuartzElemental()
            : base(AIType.AI_Melee)
        {
            Name = "an quartz elemental";
            Body = 14;
            BaseSoundID = 268;
            Hue = 2575;

            SetStr(240, 260);
            SetDex(70, 80);
            SetInt(100, 110);

            SetHits(1000, 1100);

            SetDamage(14, 21);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 15, 25);

            SetSkill(SkillName.MagicResist, 110, 120);
            SetSkill(SkillName.Tactics, 100, 110.0);
            SetSkill(SkillName.Wrestling, 110, 120);

            Fame = 4500;
            Karma = -4500;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 1);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if ((c.Map != null && c.Map.Rules == MapRules.FeluccaRules) || 0.5 > Utility.RandomDouble())
                c.DropItem(new QuartzGrit());
        }

        public QuartzElemental(Serial serial)
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

    [CorpseName("an flame elemental corpse")]
    public class FlameElemental : ShameGuardian
    {
        [Constructable]
        public FlameElemental()
            : base(AIType.AI_Mage)
        {
            Name = "an flame elemental";
            Body = 15;
            BaseSoundID = 838;
            Hue = 1196;

            SetStr(420, 460);
            SetDex(160, 210);
            SetInt(120, 190);

            SetHits(700, 800);
            SetMana(1000, 1300);

            SetDamage(18, 20);

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

            Fame = 4500;
            Karma = -4500;

            PackItem(new SulfurousAsh(5));
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 1);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if ((c.Map != null && c.Map.Rules == MapRules.FeluccaRules) || 0.5 > Utility.RandomDouble())
                c.DropItem(new CorrosiveAsh());
        }

        public FlameElemental(Serial serial)
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

    [CorpseName("an wind elemental corpse")]
    public class WindElemental : ShameGuardian
    {
        [Constructable]
        public WindElemental()
            : base(AIType.AI_Mage)
        {
            Name = "an wind elemental";
            Body = 13;
            BaseSoundID = 655;

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

            Fame = 4500;
            Karma = -4500;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 1);
            this.AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(3, 5));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if ((c.Map != null && c.Map.Rules == MapRules.FeluccaRules) || 0.5 > Utility.RandomDouble())
                c.DropItem(new CursedOilstone());
        }

        public WindElemental(Serial serial)
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