using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseWindChimes : Item
    {
        private static readonly int[] m_Sounds = new int[] { 0x505, 0x506, 0x507 };
        private bool m_TurnedOn;
        public BaseWindChimes(int itemID)
            : base(itemID)
        {
        }

        public BaseWindChimes(Serial serial)
            : base(serial)
        {
        }

        public static int[] Sounds
        {
            get
            {
                return m_Sounds;
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
        public override bool HandlesOnMovement
        {
            get
            {
                return this.m_TurnedOn && this.IsLockedDown;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.m_TurnedOn && this.IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, this.Location, 2) && !Utility.InRange(oldLocation, this.Location, 2))
                Effects.PlaySound(this.Location, this.Map, m_Sounds[Utility.Random(m_Sounds.Length)]);

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
            private readonly BaseWindChimes m_Chimes;
            public OnOffGump(BaseWindChimes chimes)
                : base(150, 200)
            {
                this.m_Chimes = chimes;

                this.AddBackground(0, 0, 300, 150, 0xA28);
                this.AddHtmlLocalized(45, 20, 300, 35, chimes.TurnedOn ? 1011035 : 1011034, false, false); // [De]Activate this item
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
                    bool newValue = !this.m_Chimes.TurnedOn;

                    this.m_Chimes.TurnedOn = newValue;

                    if (newValue && !this.m_Chimes.IsLockedDown)
                        from.SendLocalizedMessage(502693); // Remember, this only works when locked down.
                }
                else
                {
                    from.SendLocalizedMessage(502694); // Cancelled action.
                }
            }
        }
    }

    public class WindChimes : BaseWindChimes
    {
        [Constructable]
        public WindChimes()
            : base(0x2832)
        {
        }

        public WindChimes(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1030290;
            }
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

    public class FancyWindChimes : BaseWindChimes
    {
        [Constructable]
        public FancyWindChimes()
            : base(0x2833)
        {
        }

        public FancyWindChimes(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1030291;
            }
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
}