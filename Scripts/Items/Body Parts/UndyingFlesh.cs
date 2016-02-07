using System;

namespace Server.Items
{
    public class UndyingFlesh : Item
    {
        [Constructable]
        public UndyingFlesh()
            : this(1)
        {
        }

        [Constructable]
        public UndyingFlesh(int amount)
            : base(0x5731)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public UndyingFlesh(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113337;
            }
        }// undying flesh
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