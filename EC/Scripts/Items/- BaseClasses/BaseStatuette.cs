using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class BaseStatuette : Item
    {
        private bool m_TurnedOn;
        [Constructable]
        public BaseStatuette(int itemID)
            : base(itemID)
        {
            this.LootType = LootType.Blessed;
        }

        public BaseStatuette(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement
        {
            get
            {
                return this.m_TurnedOn && this.IsLockedDown;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get
            {
                return this.m_TurnedOn;
            }
            set
            {
                this.m_TurnedOn = value;
                this.InvalidateProperties();
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.m_TurnedOn && this.IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, this.Location, 2) && !Utility.InRange(oldLocation, this.Location, 2))
            {
                this.PlaySound(m);
            }

            base.OnMovement(m, oldLocation);
        }

        public virtual void PlaySound(Mobile to)
        {
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

            writer.Write((bool)this.m_TurnedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_TurnedOn = reader.ReadBool();
                        break;
                    }
            }
        }

        private class OnOffGump : Gump
        {
            private readonly BaseStatuette m_Statuette;
            public OnOffGump(BaseStatuette statuette)
                : base(150, 200)
            {
                this.m_Statuette = statuette;

                this.AddBackground(0, 0, 300, 150, 0xA28);

                this.AddHtmlLocalized(45, 20, 300, 35, statuette.TurnedOn ? 1011035 : 1011034, false, false); // [De]Activate this item

                this.AddButton(40, 53, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(80, 55, 65, 35, 1011036, false, false); // OKAY

                this.AddButton(150, 53, 0xFA5, 0xFA7, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(190, 55, 100, 35, 1011012, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 1)
                {
                    bool newValue = !this.m_Statuette.TurnedOn;
                    this.m_Statuette.TurnedOn = newValue;

                    if (newValue && !this.m_Statuette.IsLockedDown)
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