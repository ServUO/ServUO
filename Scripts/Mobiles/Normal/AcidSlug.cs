using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an acid slug corpse")]
    public class AcidSlug : BaseCreature
    {
        [Constructable]
        public AcidSlug()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an acid slug";
            Body = 51;

            switch (Utility.Random(4))
            {
                case 0: Hue = 242; break;
                case 1: Hue = 243; break;
                case 2: Hue = 244; break;
                case 3: Hue = 245; break;
            }

			SetStr(213, 294);
			SetDex(80, 82);
            SetInt(18, 22);

			SetHits(333, 370);

			SetDamage(21, 28);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 10, 15);
            SetResistance(ResistanceType.Fire, 0);
            SetResistance(ResistanceType.Cold, 10, 15);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 10, 15);

            SetSkill(SkillName.MagicResist, 25.0);
            SetSkill(SkillName.Tactics, 30.0, 50.0);
            SetSkill(SkillName.Wrestling, 30.0, 80.0);

            if (0.75 > Utility.RandomDouble())
                PackItem(new AcidSac());

            PackItem(new CongealedSlugAcid());
        }

        public AcidSlug(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
        }

        public override int GetIdleSound()
        {
            return 1499;
        }

        public override int GetAngerSound()
        {
            return 1496;
        }

        public override int GetHurtSound()
        {
            return 1498;
        }

        public override int GetDeathSound()
        {
            return 1497;
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