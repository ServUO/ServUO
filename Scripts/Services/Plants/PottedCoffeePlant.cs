using Server.Engines.Plants;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using System;

namespace Server.Items
{
    public class CoffeeGrounds : Item
    {
        public override int LabelNumber => 1155735;  // Coffee Grounds

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
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CoffeePod : Item
    {
        public override int LabelNumber => 1123484;  // Coffee Pod

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
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PottedCoffeePlant : Item, ISecurable
    {
        public static readonly TimeSpan CheckDelay = TimeSpan.FromHours(23.0);

        public override int LabelNumber => 1123480;  // Potted Coffee Plant

        private PlantStatus m_PlantStatus;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
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

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextGrowth { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public PottedCoffeePlant()
            : base(0x9A20)
        {
            Weight = 5.0;
            PlantStatus = PlantStatus.Stage1;
            NextGrowth = DateTime.UtcNow + CheckDelay;
            StartTimer();
        }

        public bool CheckAccessible(Mobile from, Item item)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true; // Staff can access anything

            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null)
                return false;

            switch (Level)
            {
                case SecureLevel.Owner: return house.IsOwner(from);
                case SecureLevel.CoOwners: return house.IsCoOwner(from);
                case SecureLevel.Friends: return house.IsFriend(from);
                case SecureLevel.Anyone: return true;
                case SecureLevel.Guild: return house.IsGuildMember(from);
            }

            return false;
        }

        public void OnTick()
        {
            if (PlantStatus < PlantStatus.Stage4)
            {
                if (NextGrowth < DateTime.UtcNow)
                {
                    PlantStatus++;
                    NextGrowth = DateTime.UtcNow + CheckDelay;
                }
            }
            else
            {
                StopTimer();
            }
        }

        public void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public void StartTimer()
        {
            if (m_Timer != null)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromHours(1.0), TimeSpan.FromHours(1.0), OnTick);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 4))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that.
                return;
            }

            if (CheckAccessible(from, this))
            {
                if (PlantStatus == PlantStatus.Stage4)
                {
                    LabelTo(from, 1155694); // *You carefully pick some pods from the plant*
                    PlantStatus--;
                    from.AddToBackpack(new CoffeePod());
                    NextGrowth = DateTime.UtcNow + CheckDelay;
                    StartTimer();
                }
                else
                {
                    from.SendLocalizedMessage(1155695); // The plant is not ready to be picked from.
                }
            }
        }

        public PottedCoffeePlant(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write((int)PlantStatus);
            writer.Write((int)Level);
            writer.Write(NextGrowth);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            PlantStatus = (PlantStatus)reader.ReadInt();
            Level = (SecureLevel)reader.ReadInt();
            NextGrowth = reader.ReadDateTime();

            if (PlantStatus < PlantStatus.Stage4)
                StartTimer();
        }
    }
}
