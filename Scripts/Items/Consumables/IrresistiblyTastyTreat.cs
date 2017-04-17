using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class IrresistiblyTastyTreat : TastyTreat
    {
        public override double Bonus { get { return 0.15; } }
        public override TimeSpan Duration { get { return TimeSpan.FromMinutes(20); } }
        public override TimeSpan CoolDown { get { return TimeSpan.FromMinutes(120); } }
        public override int DamageBonus { get { return 10; } }

        [Constructable]
        public IrresistiblyTastyTreat() : this(1)
        {
        }

        [Constructable]
        public IrresistiblyTastyTreat(int amount)
        {
            Stackable = true;
            Amount = amount;
        }

        public IrresistiblyTastyTreat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber { get { return 1113005; } } //Irresistibly Tasty Treat

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = (InheritsItem ? 0 : reader.ReadInt()); //Required for TastyTreat Insertion
        }
    }
}