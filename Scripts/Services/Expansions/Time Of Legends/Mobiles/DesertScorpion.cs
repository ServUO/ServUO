using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a desert scorpion corpse")]
    public class DesertScorpion : BaseCreature
    {
        [Constructable]
        public DesertScorpion()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a desert scorpion";
            Body = 0x2CD;
            BaseSoundID = 397;

            SetStr(600, 700);
            SetDex(120, 128);
            SetInt(150, 200);

            SetDamage(15, 25);

            SetHits(350, 400);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Physical, 40);

            SetSkill(SkillName.MagicResist, 60, 70);
            SetSkill(SkillName.Tactics, 80, 90);
            SetSkill(SkillName.Wrestling, 50, 60);
            SetSkill(SkillName.Poisoning, 110, 120);

            Fame = 8100;
            Karma = -8100;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
        }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int Meat { get { return 3; } }

        public DesertScorpion(Serial serial)
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