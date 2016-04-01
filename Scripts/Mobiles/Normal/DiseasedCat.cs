using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class DiseasedCat : BaseCreature
    {
        [Constructable]
        public DiseasedCat()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a diseased cat";
            this.Body = 0xC9;
            this.Hue = Utility.RandomAnimalHue();
            this.BaseSoundID = 0x69;

            this.SetStr(9);
            this.SetDex(35);
            this.SetInt(5);

            this.SetHits(6);
            this.SetMana(0);

            this.SetDamage(1);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);

            this.SetSkill(SkillName.MagicResist, 5.0);
            this.SetSkill(SkillName.Tactics, 4.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.VirtualArmor = 8;
        }

        public DiseasedCat(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (this.Name == "a deseased cat")
                this.Name = "a diseased cat";
        }
    }
}