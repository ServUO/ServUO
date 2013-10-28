using System;

namespace Server.Items
{
    public class AgilityPotion : BaseAgilityPotion
    {
        [Constructable]
        public AgilityPotion()
            : base(PotionEffect.Agility)
        {
        }

        public AgilityPotion(Serial serial)
            : base(serial)
        {
        }

        public override int DexOffset
        {
            get
            {
                return 10;
            }
        }
        public override TimeSpan Duration
        {
            get
            {
                return TimeSpan.FromMinutes(2.0);
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