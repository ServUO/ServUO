using System;
using Server;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class GenderChangeToken : Item, IPromotionalToken
    {
        public override int LabelNumber { get { return 1070997; } } // a promotional token
        public TextDefinition ItemName { get { return 1075252; } } // gender change

        [Constructable]
        public GenderChangeToken()
            : base(0x2AAA)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else if (from is PlayerMobile)
            {
                from.CloseGump(typeof(GenderChangeConfirmGump));
                BaseGump.SendGump(new GenderChangeConfirmGump((PlayerMobile)from, this));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, ItemName.ToString()); // Use this to redeem<br>Your ~1_PROMO~ : gender change
        }

        public GenderChangeToken(Serial serial)
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
            int version = reader.ReadInt();
        }
    }

    public class GenderChangeConfirmGump : BaseGump
    {
        public GenderChangeToken Token { get; private set; }

        public GenderChangeConfirmGump(PlayerMobile pm, GenderChangeToken token)
            : base(pm, 100, 100)
        {
            Token = token;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 291, 159, 9200);
            AddImageTiled(5, 5, 281, 20, 2702);
            AddImageTiled(5, 30, 281, 100, 2702);

            AddHtmlLocalized(8, 5, 279, 20, 1075249, 0x7FFF, false, false); // Change your character's gender.

            AddButton(5, 132, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 132, 100, 20, 1011012, 0x7FFF, false, false); // CANCEL

            AddHtmlLocalized(8, 30, 279, 124, User.Female ? 1075254 : 1075253, 0x7FFF, false, false); // Click OK to change your gender to female. This change is permanent. Reversing this requires the purchase of an additional gender change token. For more details, <A HREF="http://www.uo.com/genderchange.html">visit our web site</A>.

            AddButton(126, 132, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(160, 132, 120, 20, User.Female ? 1075251 : 1075250, 0x7FFF, false, false); // Make me a woman!
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (User.Female)
                {
                    User.Body = User.Race.MaleBody;
                    User.Female = false;
                }
                else
                {
                    User.Body = User.Race.FemaleBody;
                    User.Female = true;

                    User.FacialHairItemID = 0;
                }

                ChangeHairstyleEntry[] entries = null;

                if (User.Race == Race.Human)
                    entries = ChangeHairstyleEntry.HairEntries;
                else if (User.Race == Race.Elf)
                    entries = ChangeHairstyleEntry.HairEntriesElf;
                else if (User.Race == Race.Gargoyle)
                    entries = ChangeHairstyleEntry.HairEntriesGargoyle;

                if (entries != null)
                {
                    if (!User.Female && User.Race != Race.Elf)
                    {
                        User.SendGump(new ChangeHairstyleGump(User, null, 0, false, entries, m =>
                            {
                                m.SendGump(new ChangeHairstyleGump(User, null, 0, true, m.Race == Race.Gargoyle ? ChangeHairstyleEntry.BeardEntriesGargoyle : ChangeHairstyleEntry.BeardEntries));
                            }));
                    }
                    else
                    {
                        User.SendGump(new ChangeHairstyleGump(User, null, 0, false, entries));
                    }
                }

                User.SendMessage("You are now a {0}.", User.Female ? "woman" : "man"); // TODO: Message?
                Token.Delete();
            }
        }
    }
}