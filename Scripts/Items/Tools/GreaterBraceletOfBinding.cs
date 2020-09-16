using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class GreaterBraceletOfBinding : BraceletOfBinding
    {
        public BindEntry[] Friends = new BindEntry[10];
        public BindEntry Pending { get; set; }

        public bool IsFull => Friends.FirstOrDefault(entry => entry == null) != null;

        [CommandProperty(AccessLevel.GameMaster)]
        public override int MaxRecharges => -1;

        [Constructable]
        public GreaterBraceletOfBinding()
        {
            Hue = 2575;
            Weight = 1.0;

            LootType = LootType.Blessed;
        }

        public GreaterBraceletOfBinding(Serial serial)
            : base(serial)
        {
        }


        public override string TranslocationItemName => "greater bracelet of binding";

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1151769); // Greater Bracelet of Binding
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Siege.SiegeShard)
            {
                return;
            }

            if (from is PlayerMobile && from.Items.Contains(this))
            {
                if (Pending != null)
                {
                    BaseGump.SendGump(new GreaterBraceletOfBindingGump((PlayerMobile)from, this, Pending));
                }
                else
                {
                    BaseGump.SendGump(new GreaterBraceletOfBindingGump((PlayerMobile)from, this));
                }
            }
            else
            {
                from.SendLocalizedMessage(1151772); // You must be wearing this item to bind to another character.
            }
        }

        public bool IsBound(Mobile m)
        {
            foreach (BindEntry e in Friends)
            {
                if (e != null && m == e.Mobile)
                {
                    return true;
                }
            }

            return false;
        }

        public void Remove(PlayerMobile pm)
        {
            for (int i = 0; i < Friends.Length; i++)
            {
                if (Friends[i] != null && Friends[i].Mobile == pm)
                {
                    Friends[i] = null;
                    break;
                }
            }
        }

        public void Add(BindEntry entry, int index)
        {
            if (index >= 0 && index < Friends.Length && Friends[index] == null)
            {
                Friends[index] = entry;

                if (Pending == entry)
                {
                    Pending = null;
                }
            }
        }

        public override void OnRemoved(object parent)
        {
            if (RootParent is Mobile)
            {
                Mobile m = (Mobile)RootParent;

                m.CloseGump(typeof(GreaterBraceletOfBindingGump));
                m.CloseGump(typeof(ConfirmBindGump));
            }
        }

        public class GreaterBraceletOfBindingGump : BaseGump
        {
            public GreaterBraceletOfBinding Bracelet { get; set; }
            public bool Choose { get; set; }
            public BindEntry Entry { get; set; }

            public GreaterBraceletOfBindingGump(PlayerMobile pm, GreaterBraceletOfBinding bracelet, BindEntry entry)
                : base(pm, 100, 100)
            {
                Bracelet = bracelet;
                Entry = entry;
                Choose = true;
            }

            public GreaterBraceletOfBindingGump(PlayerMobile pm, GreaterBraceletOfBinding bracelet)
                : base(pm, 100, 100)
            {
                Bracelet = bracelet;
                Choose = false;
            }

            public override void AddGumpLayout()
            {
                User.CloseGump(typeof(ConfirmBindGump));
                User.CloseGump(typeof(GreaterBraceletOfBindingGump));

                AddBackground(0, 0, 220, 320, 5170);

                if (Choose)
                {
                    AddHtmlLocalized(0, 3, 220, 20, CenterLoc, "#1151796", Engines.Quests.BaseQuestGump.C32216(0x0000CD), false, false); // **Choose slot to bind**
                }
                else
                {
                    AddHtmlLocalized(0, 3, 220, 20, CenterLoc, "#1151769", Engines.Quests.BaseQuestGump.C32216(0x0000CD), false, false); // Greater Bracelet of Binding
                    AddHtmlLocalized(40, 275, 220, 20, 1017337, Bracelet.Charges.ToString(), Engines.Quests.BaseQuestGump.C32216(0x0000CD), false, false); // Teleport Charges: ~1_val~
                }

                for (int i = 0; i < 10; i++)
                {
                    if (Bracelet.Friends[i] != null)
                    {
                        AddLabel(60, 25 + (i * 25), 0x21, Bracelet.Friends[i].Mobile.Name);

                        if (!Choose)
                        {
                            if (Bracelet.Friends[i].Mobile.NetState != null)
                            {
                                AddButton(32, 28 + (i * 25), 0x2C89, 0x2C8A, i + 10, GumpButtonType.Reply, 0);
                            }

                            AddButton(165, 25 + (i * 25), 4017, 4019, i + 100, GumpButtonType.Reply, 0);
                        }
                    }
                    else
                    {
                        AddButton(25, 25 + (i * 25), 4006, 4005, i + 10, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(60, 25 + (i * 25), 150, 20, 3006110, false, false);
                    }
                }
            }

            public override void OnResponse(RelayInfo info)
            {
                if (info.ButtonID == 0)
                    return;

                int id = info.ButtonID - 10;

                if (id >= 0 && id < Bracelet.Friends.Length)
                {
                    if (Bracelet.Friends[id] == null)
                    {
                        if (Choose)
                        {
                            Bracelet.Add(Entry, id);
                            Choose = false;

                            Refresh();
                        }
                        else if (!Bracelet.IsFull)
                        {
                            User.BeginTarget(10, false, TargetFlags.None, (from, targeted) =>
                                {
                                    if (targeted is PlayerMobile)
                                    {
                                        PlayerMobile pm = targeted as PlayerMobile;
                                        BraceletOfBinding pmBrac = pm.FindItemOnLayer(Layer.Bracelet) as BraceletOfBinding;

                                        if (pm == User)
                                        {
                                            User.SendLocalizedMessage(1151779); // The bracelet cannot bind with itself.
                                        }
                                        else if (Bracelet.IsBound(pm))
                                        {
                                            User.SendLocalizedMessage(1151770); // This bracelet is already bound to this character.
                                        }
                                        else if (!User.Items.Contains(Bracelet))
                                        {
                                            User.SendLocalizedMessage(1151772); // You must be wearing this item to bind to another character.
                                        }
                                        else if (pmBrac == null)
                                        {
                                            User.SendLocalizedMessage(1151771); // The target player must be wearing a Bracelet of Binding or Greater Bracelet of Binding for the device to work.
                                        }
                                        else if (pm.HasGump(typeof(ConfirmBindGump)) || (pmBrac is GreaterBraceletOfBinding && ((GreaterBraceletOfBinding)pmBrac).Pending != null))
                                        {
                                            User.SendLocalizedMessage(1151833); // You may not get confirmation from this player at this time.
                                        }
                                        else if ((pmBrac is GreaterBraceletOfBinding && ((GreaterBraceletOfBinding)pmBrac).IsFull) || (!(pmBrac is GreaterBraceletOfBinding) && pmBrac.Bound != null))
                                        {
                                            User.SendLocalizedMessage(1151781); // The target player's greater bracelet of binding is full.
                                        }
                                        else
                                        {
                                            User.SendLocalizedMessage(1151777, pm.Name); // Waiting for ~1_val~ to respond.
                                            Refresh();

                                            SendGump(new ConfirmBindGump(pm, User, id, Bracelet, false));
                                        }
                                    }
                                    else
                                    {
                                        User.SendLocalizedMessage(1151775); // You may not bind your bracelet to that.
                                    }
                                });
                        }
                    }
                    else
                    {
                        Bracelet.Bound = Bracelet.Friends[id].Bracelet;
                        Bracelet.Activate(User);
                    }
                }
                else
                {
                    id = info.ButtonID - 100;

                    if (id >= 0 && id < Bracelet.Friends.Length && Bracelet.Friends[id] != null)
                    {
                        SendGump(new ConfirmBindGump(User, Bracelet.Friends[id].Mobile, id, Bracelet.Friends[id].Bracelet as GreaterBraceletOfBinding, true));
                    }
                }
            }
        }

        public class ConfirmBindGump : BaseGump
        {
            public GreaterBraceletOfBinding Bracelet { get; set; }
            public PlayerMobile From { get; set; }
            public int Index { get; set; }

            public bool RemoveFromBracelet { get; set; }

            public ConfirmBindGump(PlayerMobile user, PlayerMobile from, int index, GreaterBraceletOfBinding bracelet, bool remove)
                : base(user, 200, 200)
            {
                Bracelet = bracelet;
                From = from;

                RemoveFromBracelet = remove;
                Index = index;
            }

            public override void AddGumpLayout()
            {
                AddBackground(0, 0, 405, 110, 5054);
                AddImageTiled(10, 10, 385, 90, 2702);
                AddAlphaRegion(10, 10, 385, 90);

                if (RemoveFromBracelet)
                {
                    AddHtmlLocalized(55, 20, 285, 60, 1151774, From.Name, 0xFFFF, false, false); // Releasing ~1_val~'s bracelet from yours.  Are you sure you want to do this?
                }
                else
                {
                    AddHtmlLocalized(55, 20, 285, 60, 1151773, From.Name, 0xFFFF, false, false); // ~1_val~ is attempting to bind bracelets with you.  Do you accept?
                }

                AddHtml(110, 60, 100, 20, "<BASEFONT COLOR=#FFFFFF>Yes", false, false);
                AddButton(75, 60, 4005, 4007, 1, GumpButtonType.Reply, 0);

                AddHtml(250, 60, 100, 20, "<BASEFONT COLOR=#FFFFFF>No", false, false);
                AddButton(285, 60, 4005, 4007, 2, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case 1:
                        if (RemoveFromBracelet)
                        {
                            GreaterBraceletOfBinding bracelet = User.FindItemOnLayer(Layer.Bracelet) as GreaterBraceletOfBinding;

                            if (bracelet != null && bracelet.Friends[Index] != null)
                            {
                                BindEntry entry = bracelet.Friends[Index];

                                if (entry.Bracelet is GreaterBraceletOfBinding)
                                {
                                    PlayerMobile pm = entry.Mobile;
                                    GreaterBraceletOfBinding gbr = entry.Bracelet as GreaterBraceletOfBinding;

                                    gbr.Remove(User);

                                    if (pm != null && pm.NetState != null)
                                    {
                                        GreaterBraceletOfBindingGump gump = pm.FindGump<GreaterBraceletOfBindingGump>();

                                        if (gump != null)
                                        {
                                            gump.Refresh();
                                        }
                                    }
                                }

                                bracelet.Remove(entry.Mobile);
                                SendGump(new GreaterBraceletOfBindingGump(User, bracelet));
                            }
                        }
                        else
                        {
                            BraceletOfBinding brac = User.FindItemOnLayer(Layer.Bracelet) as BraceletOfBinding;

                            if (brac != null)
                            {
                                BindEntry entry = new BindEntry(User, brac);
                                Bracelet.Add(entry, Index);

                                GreaterBraceletOfBindingGump g = From.FindGump<GreaterBraceletOfBindingGump>();

                                if (g != null)
                                {
                                    g.Refresh();
                                }

                                if (brac is GreaterBraceletOfBinding && !((GreaterBraceletOfBinding)brac).IsBound(From))
                                {
                                    entry = new BindEntry(From, Bracelet);
                                    ((GreaterBraceletOfBinding)brac).Pending = entry;

                                    SendGump(new GreaterBraceletOfBindingGump(User, (GreaterBraceletOfBinding)brac, entry));
                                }
                                else
                                {
                                    brac.Bound = Bracelet;
                                }
                            }
                            else
                            {
                                User.SendLocalizedMessage(1151772); // You must be wearing this item to bind to another character.
                                From.SendLocalizedMessage(1151771); // The target player must be wearing a Bracelet of Binding or Greater Bracelet of Binding for the device to work.
                            }
                        }
                        break;
                    case 2:
                        if (!RemoveFromBracelet)
                            From.SendLocalizedMessage(1151778, User.Name); // ~1_val~ has declined your request to bind bracelets.
                        break;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            if (Pending != null)
            {
                writer.Write(1);
                writer.WriteMobile(Pending.Mobile);
                writer.WriteItem(Pending.Bracelet);
            }
            else
            {
                writer.Write(0);
            }

            foreach (BindEntry entry in Friends)
            {
                if (entry == null)
                {
                    writer.Write(0);
                }
                else
                {
                    writer.Write(1);

                    writer.WriteMobile(entry.Mobile);
                    writer.WriteItem(entry.Bracelet);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (reader.ReadInt() == 1)
            {
                PlayerMobile pm = reader.ReadMobile<PlayerMobile>();
                GreaterBraceletOfBinding brac = reader.ReadItem<GreaterBraceletOfBinding>();

                if (pm != null && brac != null)
                {
                    Pending = new BindEntry(pm, brac);
                }
            }

            for (int i = 0; i < Friends.Length; i++)
            {
                if (reader.ReadInt() == 0)
                    continue;

                PlayerMobile pm = reader.ReadMobile<PlayerMobile>();
                GreaterBraceletOfBinding brac = reader.ReadItem<GreaterBraceletOfBinding>();

                if (pm != null && brac != null)
                {
                    Friends[i] = new BindEntry(pm, brac);
                }
            }
        }

        public class BindEntry
        {
            public PlayerMobile Mobile { get; set; }
            public BraceletOfBinding Bracelet { get; set; }

            public BindEntry(PlayerMobile m, BraceletOfBinding bracelet)
            {
                Mobile = m;
                Bracelet = bracelet;
            }
        }
    }
}
