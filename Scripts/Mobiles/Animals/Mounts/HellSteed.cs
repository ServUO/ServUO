using System;

namespace Server.Mobiles
{
    [CorpseName("a hellsteed corpse")]
    public class HellSteed : BaseMount
    {
        [Constructable] 
        public HellSteed()
            : this("a hellsteed")
        {
        }

        [Constructable]
        public HellSteed(string name)
            : base(name, 793, 0x3EBB, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            SetStats(this);
        }

        public HellSteed(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }
        public override int BreathChaosDamage
        {
            get
            {
                return 100;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public static void SetStats(BaseCreature steed)
        {
            steed.SetStr(201, 210);
            steed.SetDex(101, 110);
            steed.SetInt(101, 115);

            steed.SetHits(201, 220);

            steed.SetDamage(20, 24);

            steed.SetDamageType(ResistanceType.Physical, 25);
            steed.SetDamageType(ResistanceType.Fire, 75);

            steed.SetResistance(ResistanceType.Physical, 60, 70);
            steed.SetResistance(ResistanceType.Fire, 90);
            steed.SetResistance(ResistanceType.Poison, 100);

            steed.SetSkill(SkillName.MagicResist, 90.1, 110.0);
            steed.SetSkill(SkillName.Tactics, 50.0);
            steed.SetSkill(SkillName.Wrestling, 90.1, 110.0);

            steed.Fame = 0;
            steed.Karma = 0;
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