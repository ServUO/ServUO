using System;

namespace Server.Items
{
    public class DeliciouslyTastyTreat : TastyTreat
    {
        public override double Bonus => 0.10;
        public override TimeSpan Duration => TimeSpan.FromMinutes(10);
        public override TimeSpan CoolDown => TimeSpan.FromMinutes(60);

        [Constructable]
        public DeliciouslyTastyTreat() : this(1)
        {
        }

        [Constructable]
        public DeliciouslyTastyTreat(int amount)
        {
            Stackable = true;
            Amount = amount;
        }

        public DeliciouslyTastyTreat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113004;  //Deliciously Tasty Treat

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = (InheritsItem ? 0 : reader.ReadInt()); //Required for TastyTreat Insertion
        }
    }
}