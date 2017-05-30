using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class SingingBall : Item
    {
        public override int LabelNumber { get { return 1041245; } } // Singing Ball
        private bool m_TurnedOn;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get { return this.m_TurnedOn; }
            set
            {
                this.m_TurnedOn = value;
                this.InvalidateProperties();
            }
        }

        [Constructable]
        public SingingBall() : base(0xE2E)
        {
            Weight = 10.0;
            LootType = LootType.Blessed;

            Light = LightType.Circle300;
        }

        public SingingBall(Serial serial) : base(serial)
        {
        }

        public override bool HandlesOnMovement { get { return this.m_TurnedOn && this.IsLockedDown; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.m_TurnedOn && this.IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, this.Location, 2) && !Utility.InRange(oldLocation, this.Location, 2))
                Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0, 1338));

            base.OnMovement(m, oldLocation);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_TurnedOn)
                list.Add(502695); // turned on
            else
                list.Add(502696); // turned off
        }

        public bool IsOwner(Mobile mob)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            return (house != null && house.IsOwner(mob));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsOwner(from))
            {
                OnOffGump onOffGump = new OnOffGump(this);
                from.SendGump(onOffGump);
            }
            else
            {
                from.SendLocalizedMessage(502691); // You must be the owner to use this.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((bool)m_TurnedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

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
                    bool newValue = !this.m_SingingBall.TurnedOn;

                    this.m_SingingBall.TurnedOn = newValue;

                    if (newValue && !this.m_SingingBall.IsLockedDown)
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