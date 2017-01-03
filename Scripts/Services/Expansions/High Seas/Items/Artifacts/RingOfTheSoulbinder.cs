using Server;
using System;

namespace Server.Items
{
    public class RingOfTheSoulbinder : SilverRing
    {
        public override int LabelNumber { get { return 1116620; } }

        [Constructable]
        public RingOfTheSoulbinder()
        {
            Hue = 288;
            Attributes.RegenMana = 2;
            Attributes.DefendChance = 15;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 3;
            Attributes.SpellDamage = 10;
            Attributes.LowerRegCost = 10;
        }

        public RingOfTheSoulbinder(Serial serial)
            : base(serial)
        {
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