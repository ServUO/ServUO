using System;

namespace Server.Items
{
    public class LiveRock : Item, ICommodity
    {
        public override int LabelNumber { get { return 1125985; } } // live rock

        [Constructable]
        public LiveRock()
            : this(1)
        {
        }

        [Constructable]
        public LiveRock(int amount)
            : base(0xA3E9)
        {
            Stackable = true;
            Amount = amount;
        }

        public LiveRock(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description { get { return LabelNumber; } }
        bool ICommodity.IsDeedable { get { return true; } }
        
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
