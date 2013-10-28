using System;

namespace Server.Mobiles
{
    [CorpseName("an elder gazer corpse")]
    public class ElderGazer : BaseCreature
    {
        [Constructable]
        public ElderGazer()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an elder gazer";
            this.Body = 22;
            this.BaseSoundID = 377;

            this.SetStr(296, 325);
            this.SetDex(86, 105);
            this.SetInt(291, 385);

            this.SetHits(178, 195);

            this.SetDamage(8, 19);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 62.0, 100.0);
            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 115.1, 130.0);
            this.SetSkill(SkillName.Tactics, 80.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 100.0);

            this.Fame = 12500;
            this.Karma = -12500;

            this.VirtualArmor = 50;
        }

        public ElderGazer(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel
        {
            get
            {
                return Core.AOS ? 4 : 0;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
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