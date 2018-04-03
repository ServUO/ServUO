using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anlorvaglem corpse")]
    public class Anlorvaglem : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.Grouping; } }
        public override int Stage { get { return 3; } }

        [Constructable]
        public Anlorvaglem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            Name = "anlorvaglem";
            Hue = 2071;
            Body = 152;

            SetStr(1000, 1200);
            SetDex(1000, 1200);
            SetInt(100, 1200);

            SetHits(3205);

            SetDamage(11, 13);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 20, 50);
            SetResistance(ResistanceType.Fire, 20, 60);
            SetResistance(ResistanceType.Cold, 20, 58);
            SetResistance(ResistanceType.Poison, 80, 100);
            SetResistance(ResistanceType.Energy, 30, 50);

            SetSkill(SkillName.Wrestling, 75.8, 100.0);
            SetSkill(SkillName.Tactics, 50.0, 100.0);
            SetSkill(SkillName.MagicResist, 50.9, 90.0);

            Fame = 8000;
            Karma = -8000;

            VirtualArmor = 48;

            PackItem(new DaemonBone(30));
        }

        public Anlorvaglem(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
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
        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}
