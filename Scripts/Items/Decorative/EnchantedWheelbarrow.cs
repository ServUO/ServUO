using Server.Gumps;
using Server.Multis;
using System;

namespace Server.Items
{
    public class EnchantedWheelbarrow : Item, IFlipable, ISecurable
    {
        public override int LabelNumber => 1125214;  // enchanted wheelbarrow

        private bool m_Harvest;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Harvest
        {
            get => m_Harvest;
            set
            {

                if (value && (ItemID == 0xA0E6 || ItemID == 0xA0E7))
                    ItemID = ItemID + 2;
                else if (ItemID == 0xA0E8 || ItemID == 0xA0E9)
                    ItemID = ItemID - 2;

                m_Harvest = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        private Timer m_Timer;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextHarvest { get; set; }

        private static readonly Type[] DecorativePlants =
        {
            typeof(DecorativePlant),    typeof(DecorativePlantWhiteFlowers),     typeof(DecorativePlantVines),
            typeof(DecorativePlantFlax),     typeof(DecorativePlantPoppies),          typeof(DecorativePlantLilypad)
        };

        [Constructable]
        public EnchantedWheelbarrow()
            : base(0xA0E6)
        {
            Harvest = true;
            Weight = 5.0;
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

        public void OnTick()
        {
            if (Harvest)
            {
                StopTimer();
            }
            else
            {
                if (NextHarvest < DateTime.UtcNow)
                {
                    Harvest = true;
                }
            }
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

        public void OnFlip(Mobile from)
        {
            if (ItemID == 0xA0E6 || ItemID == 0xA0E8)
                ItemID = ItemID + 1;
            else
                ItemID = ItemID - 1;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!m.InRange(GetWorldLocation(), 3))
            {
                m.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (!IsLockedDown)
            {
                m.SendLocalizedMessage(1114298); // This must be locked down in order to use it.
            }
            else
            {
                if (CheckAccessible(m, this))
                {
                    if (Harvest)
                    {
                        Item i = Activator.CreateInstance(DecorativePlants[Utility.Random(DecorativePlants.Length)]) as Item;

                        if (i != null)
                        {
                            m.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1158330); // *You collect a plant from the wheelbarrow*
                            m.AddToBackpack(i);
                            Harvest = false;
                            NextHarvest = DateTime.UtcNow + TimeSpan.FromDays(7);
                            StartTimer();
                        }
                    }
                    else
                    {
                        m.SendLocalizedMessage(1158329); // There is nothing to harvest.
                    }
                }
            }
        }

        public EnchantedWheelbarrow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Harvest);
            writer.Write((int)Level);
            writer.Write(NextHarvest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_Harvest = reader.ReadBool();
            Level = (SecureLevel)reader.ReadInt();
            NextHarvest = reader.ReadDateTime();

            if (!Harvest)
                StartTimer();
        }
    }

    public class DecorativePlant : Item
    {
        public override int LabelNumber => 1125205;  // decorative plant

        private static readonly int[] DecorativePlants =
        {
            0xA0E1, 0xA0E2, 0xA0E3, 0xA0E4, 0xA0E5, 0xA0ED, 0xA11B, 0xA11C, 0xA11F, 0xA120, 0xA121, 0xA122, 0xA123, 0xA124, 0xA125, 0xA128, 0xA12B
        };

        [Constructable]
        public DecorativePlant()
            : base(DecorativePlants[Utility.Random(DecorativePlants.Length)])
        {
        }

        [Constructable]
        public DecorativePlant(int id)
            : base(id)
        {
        }

        public DecorativePlant(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0xA0DD, 0xA0DE)]
    public class DecorativePlantWhiteFlowers : DecorativePlant
    {
        [Constructable]
        public DecorativePlantWhiteFlowers()
            : base(0xA0DD)
        {
        }

        public DecorativePlantWhiteFlowers(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0xA0DF, 0xA0E0)]
    public class DecorativePlantVines : DecorativePlant
    {
        [Constructable]
        public DecorativePlantVines()
            : base(0xA0DF)
        {
        }

        public DecorativePlantVines(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0xA11D, 0xA11E)]
    public class DecorativePlantFlax : DecorativePlant
    {
        [Constructable]
        public DecorativePlantFlax()
            : base(0xA11D)
        {
        }

        public DecorativePlantFlax(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0xA126, 0xA127)]
    public class DecorativePlantPoppies : DecorativePlant
    {
        [Constructable]
        public DecorativePlantPoppies()
            : base(0xA126)
        {
        }

        public DecorativePlantPoppies(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0xA129, 0xA12A)]
    public class DecorativePlantLilypad : DecorativePlant
    {
        [Constructable]
        public DecorativePlantLilypad()
            : base(0xA129)
        {
        }

        public DecorativePlantLilypad(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
