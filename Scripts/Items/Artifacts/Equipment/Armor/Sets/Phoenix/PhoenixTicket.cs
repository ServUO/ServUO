namespace Server.Items
{

    public class PhoenixTicket : Item
    {
        [Constructable]
        public PhoenixTicket() : base(0x14F0)
        {
            LootType = LootType.Blessed;
        }

        public PhoenixTicket(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber => 1041234;// Ticket for a piece of phoenix armor

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                switch (Utility.Random(6))
                {
                    case 0: from.AddToBackpack(new PhoenixArms()); break;
                    case 1: from.AddToBackpack(new PhoenixChest()); break;
                    case 2: from.AddToBackpack(new PhoenixGloves()); break;
                    case 3: from.AddToBackpack(new PhoenixGorget()); break;
                    case 4: from.AddToBackpack(new PhoenixHelm()); break;
                    case 5: from.AddToBackpack(new PhoenixLegs()); break;
                }
                Delete();
                from.SendLocalizedMessage(502064); // A piece of phoenix armor has been placed in your backpack.
            }

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}