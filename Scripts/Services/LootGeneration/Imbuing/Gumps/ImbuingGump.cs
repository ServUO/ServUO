using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
    public class ImbuingGump : BaseGump
    {
        private const int LabelColor = 0x7FFF;

        public ImbuingGump(PlayerMobile pm)
            : base(pm, 25, 50)
        {
            User.CloseGump(typeof(ImbueSelectGump));
            User.CloseGump(typeof(ImbueGump));
        }

        public override void AddGumpLayout()
        {
            ImbuingContext context = Imbuing.GetContext(User);

            context.Imbue_ModVal = 0;
            context.ImbMenu_Cat = 0;

            AddPage(0);
            AddBackground(0, 0, 520, 310, 5054);
            AddImageTiled(10, 10, 500, 290, 2624);
            AddImageTiled(10, 30, 500, 10, 5058);
            AddImageTiled(10, 270, 500, 10, 5058);
            AddAlphaRegion(10, 10, 520, 310);

            AddHtmlLocalized(10, 12, 520, 20, 1079588, LabelColor, false, false); //<CENTER>IMBUING MENU</CENTER>

            AddButton(15, 60, 4005, 4007, 10005, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 60, 430, 20, 1080432, LabelColor, false, false); //Imbue Item - Adds or modifies an item property on an item

            AddButton(15, 90, 4005, 4007, 10006, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 90, 430, 20, 1113622, LabelColor, false, false); //Reimbue Last - Repeats the last imbuing attempt

            AddButton(15, 120, 4005, 4007, 10007, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 120, 430, 20, 1113571, LabelColor, false, false); //Imbue Last Item - Auto targets the last imbued item

            AddButton(15, 150, 4005, 4007, 10008, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 150, 430, 20, 1114274, LabelColor, false, false); //Imbue Last Property - Imbues a new item with the last property

            AddButton(15, 180, 4005, 4007, 10010, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 180, 470, 20, 1080431, LabelColor, false, false); //Unravel Item - Extracts magical ingredients User an item, destroying it

            AddButton(15, 210, 4005, 4007, 10011, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 210, 430, 20, 1114275, LabelColor, false, false); //Unravel Container - Unravels all items in a container

            AddButton(15, 280, 4017, 4019, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 280, 50, 20, 1011012, LabelColor, false, false); //CANCEL
        }

        public override void OnResponse(RelayInfo info)
        {
            ImbuingContext context = Imbuing.GetContext(User);

            switch (info.ButtonID)
            {
                case 0: // Close
                case 1:
                    {
                        User.EndAction(typeof(Imbuing));

                        break;
                    }
                case 10005:  // Imbue Item
                    {
                        User.SendLocalizedMessage(1079589);  //Target an item you wish to imbue.

                        User.Target = new ImbueItemTarget();
                        User.Target.BeginTimeout(User, TimeSpan.FromSeconds(10.0));

                        break;
                    }
                case 10006:  // Reimbue Last
                    {
                        Item item = context.LastImbued;
                        int mod = context.Imbue_Mod;
                        int modint = context.Imbue_ModInt;

                        if (item == null || mod < 0 || modint == 0)
                        {
                            User.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            User.EndAction(typeof(Imbuing));
                            break;
                        }

                        if (Imbuing.CanImbueItem(User, item) && Imbuing.OnBeforeImbue(User, item, mod, modint))
                        {
                            Imbuing.TryImbueItem(User, item, mod, modint);
                            ImbueGump.SendGumpDelayed(User);
                        }
                        break;
                    }

                case 10007:  // Imbue Last Item
                    {
                        Item item = context.LastImbued;
                        int mod = context.Imbue_Mod;
                        int modint = context.Imbue_ModInt;

                        if (context.LastImbued == null)
                        {
                            User.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            User.EndAction(typeof(Imbuing));
                            break;
                        }
                        else
                        {
                            ImbueStep1(User, item);
                        }
                        break;
                    }
                case 10008:  // Imbue Last Property
                    {
                        context.LastImbued = null;
                        int mod = context.Imbue_Mod;
                        int modint = context.Imbue_ModInt;

                        if (modint < 0)
                            modint = 0;

                        if (mod < 0)
                        {
                            User.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            User.EndAction(typeof(Imbuing));
                            break;
                        }
                        else
                        {
                            ImbueLastProp(User, mod, modint);
                        }

                        break;
                    }
                case 10010:  // Unravel Item
                    {
                        User.SendLocalizedMessage(1080422); // Target an item you wish to magically unravel.

                        User.Target = new UnravelTarget();
                        User.Target.BeginTimeout(User, TimeSpan.FromSeconds(10.0));

                        break;
                    }
                case 10011:  // Unravel Container
                    {
                        User.SendLocalizedMessage(1080422); // Target an item you wish to magically unravel.

                        User.Target = new UnravelContainerTarget();
                        User.Target.BeginTimeout(User, TimeSpan.FromSeconds(10.0));

                        break;
                    }
            }

            return;
        }

        private class UnravelTarget : Target
        {
            public UnravelTarget()
                : base(-1, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile m, object o)
            {
                m.EndAction(typeof(Imbuing));

                Item item = o as Item;

                if (item == null)
                {
                    m.SendLocalizedMessage(1080425); // You cannot magically unravel this item.
                }
                else if (m is PlayerMobile && Imbuing.CanUnravelItem(m, item))
                {
                    m.BeginAction(typeof(Imbuing));
                    SendGump(new UnravelGump((PlayerMobile)m, item));
                }
            }

            protected override void OnTargetCancel(Mobile User, TargetCancelType cancelType)
            {
                User.EndAction(typeof(Imbuing));
            }

            private class UnravelGump : BaseGump
            {
                private readonly Item m_Item;

                public UnravelGump(PlayerMobile pm, Item item)
                    : base(pm, 60, 36)
                {
                    m_Item = item;
                }

                public override void AddGumpLayout()
                {
                    AddPage(0);
                    AddBackground(0, 0, 520, 245, 5054);
                    AddImageTiled(10, 10, 500, 225, 2624);
                    AddImageTiled(10, 30, 500, 10, 5058);
                    AddImageTiled(10, 202, 500, 10, 5058);
                    AddAlphaRegion(10, 10, 500, 225);

                    AddHtmlLocalized(10, 12, 520, 20, 1112402, LabelColor, false, false); // <CENTER>UNRAVEL MAGIC ITEM CONFIRMATION</CENTER>

                    AddHtmlLocalized(15, 58, 490, 113, 1112403, true, true); // WARNING! You have targeted an item made out of special material.<BR><BR>This item will be DESTROYED.<BR><BR>Are you sure you wish to unravel this item?

                    AddButton(10, 180, 4005, 4007, 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 180, 430, 20, 1114292, LabelColor, false, false); // Unravel Item

                    AddButton(10, 212, 4017, 4019, 0, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 212, 50, 20, 1011012, LabelColor, false, false); // CANCEL
                }

                public override void OnResponse(RelayInfo info)
                {
                    User.EndAction(typeof(Imbuing));

                    if (info.ButtonID == 0 || m_Item.Deleted)
                        return;

                    if (Imbuing.CanUnravelItem(User, m_Item) && Imbuing.UnravelItem(User, m_Item))
                    {
                        Effects.SendPacket(User, User.Map, new GraphicalEffect(EffectType.FixedFrom, User.Serial, Server.Serial.Zero, 0x375A, User.Location, User.Location, 1, 17, true, false));
                        User.PlaySound(0x1EB);

                        User.SendLocalizedMessage(1080429); // You magically unravel the item!
                        User.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                    }
                }
            }
        }

        private class UnravelContainerTarget : Target
        {
            public UnravelContainerTarget() : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile m, object o)
            {
                m.EndAction(typeof(Imbuing));
                Container cont = o as Container;

                if (cont == null)
                    return;

                if (!cont.IsChildOf(m.Backpack))
                {
                    m.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                    m.EndAction(typeof(Imbuing));
                }
                else if (cont == null || (cont is LockableContainer && ((LockableContainer)cont).Locked))
                {
                    m.SendLocalizedMessage(1111814, "0\t0"); // Unraveled: ~1_COUNT~/~2_NUM~ items
                    m.EndAction(typeof(Imbuing));
                }
                else if (m is PlayerMobile)
                {
                    bool unraveled = cont.Items.FirstOrDefault(x => Imbuing.CanUnravelItem(m, x, false)) != null;

                    if (unraveled)
                    {
                        m.BeginAction(typeof(Imbuing));
                        SendGump(new UnravelContainerGump((PlayerMobile)m, cont));
                    }
                    else
                    {
                        TryUnravelContainer(m, cont);
                        m.EndAction(typeof(Imbuing));
                    }
                }
            }

            protected override void OnTargetCancel(Mobile User, TargetCancelType cancelType)
            {
                User.EndAction(typeof(Imbuing));
            }

            public static void TryUnravelContainer(Mobile User, Container c)
            {
                c.Items.ForEach(y =>
                {
                    Imbuing.CanUnravelItem(User, y, true);
                });

                User.SendLocalizedMessage(1111814, string.Format("{0}\t{1}", 0, c.Items.Count)); // Unraveled: ~1_COUNT~/~2_NUM~ items
            }

            private class UnravelContainerGump : BaseGump
            {
                private readonly Container m_Container;
                private readonly List<Item> m_List;

                public UnravelContainerGump(PlayerMobile pm, Container c)
                    : base(pm, 25, 50)
                {
                    m_Container = c;
                    m_List = new List<Item>(c.Items);
                }

                public override void AddGumpLayout()
                {
                    AddPage(0);
                    AddBackground(0, 0, 520, 245, 5054);
                    AddImageTiled(10, 10, 500, 225, 2624);
                    AddImageTiled(10, 30, 500, 10, 5058);
                    AddImageTiled(10, 202, 500, 10, 5058);
                    AddAlphaRegion(10, 10, 500, 225);

                    AddHtmlLocalized(10, 12, 520, 20, 1112402, LabelColor, false, false); // <CENTER>UNRAVEL MAGIC ITEM CONFIRMATION</CENTER>

                    AddHtmlLocalized(15, 58, 490, 113, 1112404, true, true); // WARNING! The selected container contains items made with a special material.<BR><BR>These items will be DESTROYED.<BR><BR>Do you wish to unravel these items as well?

                    AddButton(10, 180, 4005, 4007, 1, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 180, 430, 20, 1049717, LabelColor, false, false); // YES

                    AddButton(10, 212, 4017, 4019, 0, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 212, 50, 20, 1049718, LabelColor, false, false); // NO
                }

                public override void OnResponse(RelayInfo info)
                {
                    User.EndAction(typeof(Imbuing));

                    if (m_Container == null || m_List == null)
                        return;

                    if (info.ButtonID == 0)
                    {
                        TryUnravelContainer(User, m_Container);
                        return;
                    }

                    int count = 0;

                    m_List.ForEach(x =>
                    {
                        if (Imbuing.CanUnravelItem(User, x, true) && Imbuing.UnravelItem(User, x, true))
                        {
                            count++;
                        }
                    });

                    if (count > 0)
                    {
                        User.SendLocalizedMessage(1080429); // You magically unravel the item!
                        User.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                    }

                    User.SendLocalizedMessage(1111814, string.Format("{0}\t{1}", count, m_List.Count));

                    ColUtility.Free(m_List);
                }
            }
        }

        private class ImbueItemTarget : Target
        {
            public ImbueItemTarget()
                : base(-1, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile m, object o)
            {
                Item item = o as Item;

                if (item == null)
                {
                    m.SendLocalizedMessage(1079576); // You cannot imbue this item.
                    return;
                }

                ImbuingContext context = Imbuing.GetContext(m);
                ItemType itemType = ItemPropertyInfo.GetItemType(item);

                if (itemType == ItemType.Invalid)
                {
                    m.SendLocalizedMessage(1079576); // You cannot imbue this item.
                    return;
                }

                ImbueStep1(m, item);
            }

            protected override void OnTargetCancel(Mobile m, TargetCancelType cancelType)
            {
                m.EndAction(typeof(Imbuing));
            }
        }

        public static void ImbueStep1(Mobile m, Item item)
        {
            if (m is PlayerMobile && Imbuing.CanImbueItem(m, item))
            {
                ImbuingContext context = Imbuing.GetContext(m);
                context.LastImbued = item;

                if (context.ImbMenu_Cat == 0)
                    context.ImbMenu_Cat = 1;

                m.CloseGump(typeof(ImbuingGump));
                SendGump(new ImbueSelectGump((PlayerMobile)m, item));
            }
        }

        public static void ImbueLastProp(Mobile m, int Mod, int Mint)
        {
            m.Target = new ImbueLastModTarget();
        }

        private class ImbueLastModTarget : Target
        {
            public ImbueLastModTarget()
                : base(-1, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile m, object o)
            {
                Item item = o as Item;

                if (item == null || !(m is PlayerMobile))
                {
                    m.SendLocalizedMessage(1079576); // You cannot imbue this item.
                    return;
                }

                ImbuingContext context = Imbuing.GetContext(m);

                int mod = context.Imbue_Mod;
                int modInt = context.Imbue_ModInt;

                if (!Imbuing.CanImbueItem(m, item) || !Imbuing.OnBeforeImbue(m, item, mod, modInt) || !Imbuing.CanImbueProperty(m, item, mod))
                {
                    ImbueGump.SendGumpDelayed((PlayerMobile)m);
                }
                else
                {
                    Imbuing.TryImbueItem(m, item, mod, modInt);
                    ImbueGump.SendGumpDelayed((PlayerMobile)m);
                }
            }

            protected override void OnTargetCancel(Mobile m, TargetCancelType cancelType)
            {
                m.EndAction(typeof(Imbuing));
            }
        }
    }
}
