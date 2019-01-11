using Server;
using System;
using Server.Network;
using Server.Engines.Plants;

namespace Server.Items
{
    public class CoffeeGrounds : Item
    {
        public override int LabelNumber { get { return 1155735; } } // Coffee Grounds

        [Constructable]
        public CoffeeGrounds()
            : this(1)
        {
        }

        [Constructable]
        public CoffeeGrounds(int amount)
            : base(0x573B)
        {
            Hue = 1022;
            Stackable = true;
            Amount = amount;
        }

        public CoffeeGrounds(Serial serial)
            : base(serial)
        {
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

    public class CoffeePod : Item
    {
        public override int LabelNumber { get { return 1123484; } } // Coffee Pod

        [Constructable]
        public CoffeePod()
            : this(1)
        {
        }

        [Constructable]
        public CoffeePod(int amount)
            : base(Utility.RandomBool() ? 0x9A24 : 0x9A25)
        {
            Stackable = true;
            Amount = amount;
        }

        public CoffeePod(Serial serial)
            : base(serial)
        {
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

    public class PottedCoffeePlant : Item
    {
        public static readonly TimeSpan CheckDelay = TimeSpan.FromHours(23.0);

        public override int LabelNumber { get { return 1123480; } } // Potted Coffee Plant

        private PlantStatus m_PlantStatus;
        public PlantStatus PlantStatus
        {
            get { return m_PlantStatus; }
            set
            {
                switch (value)
                {
                    case PlantStatus.Stage1: { ItemID = 0x9A20; break; }
                    case PlantStatus.Stage2: { ItemID = 0x9A21; break; }
                    case PlantStatus.Stage3: { ItemID = 0x9A22; break; }
                    case PlantStatus.Stage4: { ItemID = 0x9A23; break; }
                }

                m_PlantStatus = value;
            }
        }
        public DateTime NextGrowth { get; set; }

        [Constructable]
        public PottedCoffeePlant()
            : base(0x9A20)
        {
            Weight = 5.0;
            PlantStatus = PlantStatus.Stage1;
            NextGrowth = DateTime.UtcNow + CheckDelay;
        }

        public void OnTick()
        {
            if (NextGrowth < DateTime.UtcNow && PlantStatus < PlantStatus.Stage4)
            {
                PlantStatus++;
                NextGrowth = DateTime.UtcNow + CheckDelay;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 4))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that.
                return;
            }

            if (PlantStatus == PlantStatus.Stage4)
            {
                LabelTo(from, 1155694); // *You carefully pick some pods from the plant*
                PlantStatus--;
                from.AddToBackpack(new CoffeePod());
                NextGrowth = DateTime.UtcNow + CheckDelay;
            }
            else
            {
                from.SendLocalizedMessage(1155695); // The plant is not ready to be picked from.
            }
        }

        public PottedCoffeePlant(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)PlantStatus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            PlantStatus = (PlantStatus)reader.ReadInt();
        }
    }
}
