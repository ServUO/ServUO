using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class PersonalAttendantDeed : Item
    {
        private Mobile m_Owner;
        [Constructable]
        public PersonalAttendantDeed()
            : this(null)
        {
        }

        [Constructable]
        public PersonalAttendantDeed(Mobile owner)
            : base(0x14F0)
        {
            this.m_Owner = owner;

            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public PersonalAttendantDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076030;
            }
        }// A Contract for a Personal Attendant
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                if (this.m_Owner == null || this.m_Owner == from)
                {
                    if (!PersonalAttendant.CheckAttendant(from))
                    {
                        from.CloseGump(typeof(InternalGump));
                        from.SendGump(new InternalGump(this));
                    }
                    else
                        from.SendLocalizedMessage(1076053); // You already have an attendant.
                }
                else
                    from.SendLocalizedMessage(501023); // You must be the owner to use this item.
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Owner != null)
                list.Add(1076144, this.m_Owner.Name); // Property of ~1_OWNER~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version

            writer.Write((Mobile)this.m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Owner = reader.ReadMobile();
        }

        public class InternalGump : Gump
        {
            private readonly PersonalAttendantDeed m_Deed;
            public InternalGump(PersonalAttendantDeed deed)
                : base(60, 36)
            {
                this.m_Deed = deed;

                this.AddPage(0);

                this.AddPage(0);

                this.AddBackground(0, 0, 273, 324, 0x13BE);
                this.AddImageTiled(10, 10, 253, 20, 0xA40);
                this.AddImageTiled(10, 40, 253, 244, 0xA40);
                this.AddImageTiled(10, 294, 253, 20, 0xA40);
                this.AddAlphaRegion(10, 10, 253, 304);
                this.AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                this.AddHtmlLocalized(14, 12, 273, 20, 1076143, 0x7FFF, false, false); // Choose your attendant

                this.AddPage(1);

                this.AddButton(19, 49, 0x845, 0x846, 100, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 47, 213, 20, 1076031, 0x7FFF, false, false); // Guide (Male)
                this.AddButton(19, 73, 0x845, 0x846, 101, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 71, 213, 20, 1076032, 0x7FFF, false, false); // Guide (Female)
                this.AddButton(19, 97, 0x845, 0x846, 102, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 95, 213, 20, 1076033, 0x7FFF, false, false); // Herald (Male)
                this.AddButton(19, 121, 0x845, 0x846, 103, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 119, 213, 20, 1076034, 0x7FFF, false, false); // Herald (Female)
                this.AddButton(19, 145, 0x845, 0x846, 104, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 143, 213, 20, 1076035, 0x7FFF, false, false); // Lucky Dealer (Male)
                this.AddButton(19, 169, 0x845, 0x846, 105, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 167, 213, 20, 1076036, 0x7FFF, false, false); // Lucky Dealer (Female)
                this.AddButton(19, 193, 0x845, 0x846, 106, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 191, 213, 20, 1076037, 0x7FFF, false, false); // Fortune Teller (Female)
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Deed == null || this.m_Deed.Deleted)
                    return;

                Mobile m = sender.Mobile;

                if (PersonalAttendant.CheckAttendant(m))
                {
                    m.SendLocalizedMessage(1076053); // You already have an attendant.
                    return;
                }

                PersonalAttendant attendant = null;								

                switch ( info.ButtonID )
                {
                    case 100:
                        attendant = new AttendantMaleGuide();
                        break;
                    case 101:
                        attendant = new AttendantFemaleGuide();
                        break;
                    case 102:
                        attendant = new AttendantMaleHerald();
                        break;
                    case 103:
                        attendant = new AttendantFemaleHerald();
                        break;
                    case 104:
                        attendant = new AttendantMaleLuckyDealer();
                        break;
                    case 105:
                        attendant = new AttendantFemaleLuckyDealer();
                        break;
                    case 106:
                        attendant = new AttendantFortuneTeller();
                        break;
                }

                if (attendant != null)
                {
                    PersonalAttendant.AddAttendant(m, attendant);

                    attendant.BindedToPlayer = (this.m_Deed.Owner != null);
                    attendant.SetControlMaster(m);
                    attendant.ControlOrder = OrderType.Follow;
                    attendant.ControlTarget = m;
                    attendant.MoveToWorld(m.Location, m.Map);

                    this.m_Deed.Delete();
                }
                else if (info.ButtonID != 0)
                {
                    sender.Mobile.SendLocalizedMessage(501311); // This option is currently disabled, while we evaluate it for game balance.
                }
            }
        }
    }
}