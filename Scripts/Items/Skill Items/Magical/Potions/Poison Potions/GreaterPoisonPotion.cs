using System;

namespace Server.Items
{
    public class GreaterPoisonPotion : BasePoisonPotion
    {
        [Constructable]
        public GreaterPoisonPotion()
            : base(PotionEffect.PoisonGreater)
        {
        }

        public GreaterPoisonPotion(Serial serial)
            : base(serial)
        {
        }

        public override Poison Poison
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override double MinPoisoningSkill
        {
            get
            {
                return 60.0;
            }
        }
        public override double MaxPoisoningSkill
        {
            get
            {
                return 100.0;
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