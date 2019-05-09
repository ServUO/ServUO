using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an Anlorlem corpse")]
    public class Anlorlem : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.Grouping; } }
        public override int Stage { get { return 2; } }

        [Constructable]
        public Anlorlem()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an Anlorlem";
            Body = 72;
            Hue = 2071;
            BaseSoundID = 644;

            SetStr(900, 1000);
            SetDex(1000, 1200);
            SetInt(900, 950);

            SetHits(500, 650);

            SetDamage(18, 22);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 90, 100);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 40.1, 70.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 16000;
            Karma = -16000;

            VirtualArmor = 50;

            PackItem(new DaemonBone(15));
        }

        public Anlorlem(Serial serial)
            : base(serial)
        {
        }

		public override int TreasureMapLevel
        {
            get
            {
                return 3;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Average, 2);
            this.AddLoot(LootPack.MedScrolls, 2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}