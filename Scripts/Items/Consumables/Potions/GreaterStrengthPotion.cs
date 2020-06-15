using System;

namespace Server.Items
{
    public class GreaterStrengthPotion : BaseStrengthPotion
    {
        [Constructable]
        public GreaterStrengthPotion()
            : base(PotionEffect.StrengthGreater)
        {
        }

        public GreaterStrengthPotion(Serial serial)
            : base(serial)
        {
        }

        public override int StrOffset => 20;
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