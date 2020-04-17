using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using System.Collections.Generic;

namespace Server.Items
{
    public class SingingBall : Item, ISecurable
    {
        public override int LabelNumber => 1041245;  // Singing Ball

        private bool m_TurnedOn;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get { return m_TurnedOn; }
            set
            {
                m_TurnedOn = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public SingingBall()
            : this(0xE2E)
        {
        }

        [Constructable]
        public SingingBall(int ItemId)
            : base(ItemId)
        {
            Weight = 10.0;
            LootType = LootType.Blessed;

            Light = LightType.Circle300;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
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

        public SingingBall(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement => m_TurnedOn && IsLockedDown;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_TurnedOn && IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, Location, 2) && !Utility.InRange(oldLocation, Location, 2))
            {
                Effects.PlaySound(Location, Map, SoundList());
            }

            base.OnMovement(m, oldLocation);
        }

        public virtual int SoundList()
        {
            return Utility.RandomMinMax(0, 1338);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_TurnedOn)
                list.Add(502695); // turned on
            else
                list.Add(502696); // turned off
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (CheckAccessible(from, this))
            {
                OnOffGump onOffGump = new OnOffGump(this);
                from.SendGump(onOffGump);
            }
            else
            {
                PublicOverheadMessage(MessageType.Regular, 0x3E9, 1061637); // You are not allowed to access 
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write((int)Level);
            writer.Write(m_TurnedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version > 0)
                Level = (SecureLevel)reader.ReadInt();

            m_TurnedOn = reader.ReadBool();
        }

        private class OnOffGump : Gump
        {
            private readonly SingingBall m_SingingBall;

            public OnOffGump(SingingBall ball)
                : base(150, 200)
            {
                m_SingingBall = ball;

                AddBackground(0, 0, 300, 150, 0xA28);

                AddHtmlLocalized(45, 20, 300, 35, ball.TurnedOn ? 1011035 : 1011034, false, false); // [De]Activate this item

                AddButton(40, 53, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 55, 65, 35, 1011036, false, false); // OKAY

                AddButton(150, 53, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(190, 55, 100, 35, 1011012, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 1)
                {
                    bool newValue = !m_SingingBall.TurnedOn;

                    m_SingingBall.TurnedOn = newValue;

                    if (newValue && !m_SingingBall.IsLockedDown)
                        from.SendLocalizedMessage(502693); // Remember, this only works when locked down.
                }
                else
                {
                    from.SendLocalizedMessage(502694); // Cancelled action.
                }
            }
        }
    }
}
