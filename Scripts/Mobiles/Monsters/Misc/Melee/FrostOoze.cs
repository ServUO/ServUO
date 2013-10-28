using System;

namespace Server.Mobiles
{
    [CorpseName("a frost ooze corpse")]
    public class FrostOoze : BaseCreature
    {
        [Constructable]
        public FrostOoze()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a frost ooze";
            this.Body = 94;
            this.BaseSoundID = 456;

            this.SetStr(18, 30);
            this.SetDex(16, 21);
            this.SetInt(16, 20);

            this.SetHits(13, 17);

            this.SetDamage(3, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 5.1, 10.0);
            this.SetSkill(SkillName.Tactics, 19.3, 34.0);
            this.SetSkill(SkillName.Wrestling, 25.3, 40.0);

            this.Fame = 450;
            this.Karma = -450;

            this.VirtualArmor = 38;
        }

        public FrostOoze(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 2));
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