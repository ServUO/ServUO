using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.FlaskOfOil ")]
    public class OilFlask : Item
    {
        [Constructable]
        public OilFlask()
            : this(1)
        {
        }

        [Constructable]
        public OilFlask(int amount)
            : base(0x1C18)
        {
            Stackable = true;
            Amount = amount;
        }

        public OilFlask(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1027199; // Oil Flask
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
