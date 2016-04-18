using System;

namespace Server.Items
{
    public class LesserPoisonPotion : BasePoisonPotion
    {
        [Constructable]
        public LesserPoisonPotion()
            : base(PotionEffect.PoisonLesser)
        {
        }

        public LesserPoisonPotion(Serial serial)
            : base(serial)
        {
        }

        public override Poison Poison
        {
            get
            {
                return Poison.Lesser;
            }
        }
        public override double MinPoisoningSkill
        {
            get
            {
                return 0.0;
            }
        }
        public override double MaxPoisoningSkill
        {
            get
            {
                return 60.0;
            }
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