using System;

namespace Server.Items
{
    public class GreaterAgilityPotion : BaseAgilityPotion
    {
        [Constructable]
        public GreaterAgilityPotion()
            : base(PotionEffect.AgilityGreater)
        {
        }

        public GreaterAgilityPotion(Serial serial)
            : base(serial)
        {
        }

        public override int DexOffset => 20;
        public override TimeSpan Duration => TimeSpan.FromMinutes(2.0);
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}