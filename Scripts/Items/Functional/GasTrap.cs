using Server.Network;
using System;

namespace Server.Items
{
    public enum GasTrapType
    {
        NorthWall,
        WestWall,
        Floor
    }

    public class GasTrap : BaseTrap
    {
        private Poison m_Poison;
        [Constructable]
        public GasTrap()
            : this(GasTrapType.Floor)
        {
        }

        [Constructable]
        public GasTrap(GasTrapType type)
            : this(type, Poison.Lesser)
        {
        }

        [Constructable]
        public GasTrap(Poison poison)
            : this(GasTrapType.Floor, Poison.Lesser)
        {
        }

        [Constructable]
        public GasTrap(GasTrapType type, Poison poison)
            : base(GetBaseID(type))
        {
            m_Poison = poison;
        }

        public GasTrap(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get
            {
                return m_Poison;
            }
            set
            {
                m_Poison = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public GasTrapType Type
        {
            get
            {
                switch (ItemID)
                {
                    case 0x113C:
                        return GasTrapType.NorthWall;
                    case 0x1147:
                        return GasTrapType.WestWall;
                    case 0x11A8:
                        return GasTrapType.Floor;
                }

                return GasTrapType.WestWall;
            }
            set
            {
                ItemID = GetBaseID(value);
            }
        }
        public override bool PassivelyTriggered => false;
        public override TimeSpan PassiveTriggerDelay => TimeSpan.Zero;
        public override int PassiveTriggerRange => 0;
        public override TimeSpan ResetDelay => TimeSpan.FromSeconds(0.0);
        public static int GetBaseID(GasTrapType type)
        {
            switch (type)
            {
                case GasTrapType.NorthWall:
                    return 0x113C;
                case GasTrapType.WestWall:
                    return 0x1147;
                case GasTrapType.Floor:
                    return 0x11A8;
            }

            return 0;
        }

        public override void OnTrigger(Mobile from)
        {
            if (m_Poison == null || !from.Player || !from.Alive || from.IsStaff())
                return;

            Effects.SendLocationEffect(Location, Map, GetBaseID(Type) - 2, 16, 3, GetEffectHue(), 0);
            Effects.PlaySound(Location, Map, 0x231);

            from.ApplyPoison(from, m_Poison);

            from.LocalOverheadMessage(MessageType.Regular, 0x22, 500855); // You are enveloped by a noxious gas cloud!
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            Poison.Serialize(m_Poison, writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Poison = Poison.Deserialize(reader);
                        break;
                    }
            }
        }
    }
}