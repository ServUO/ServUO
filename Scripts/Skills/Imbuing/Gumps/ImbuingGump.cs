using System;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using System.Collections.Generic;
using Server.Targeting;
using Server.SkillHandlers;
using System.Linq;

namespace Server.Gumps
{
    public class ImbuingGump : Gump
    {
        private const int LabelColor = 0x7FFF;

        public ImbuingGump(Mobile from)
            : base(25, 50)
        {
            from.CloseGump(typeof(ImbuingGumpB));
            from.CloseGump(typeof(ImbuingGumpC));

            ImbuingContext context = Imbuing.GetContext(from);

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
            AddHtmlLocalized(50, 180, 470, 20, 1080431, LabelColor, false, false); //Unravel Item - Extracts magical ingredients from an item, destroying it

            AddButton(15, 210, 4005, 4007, 10011, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 210, 430, 20, 1114275, LabelColor, false, false); //Unravel Container - Unravels all items in a container

            AddButton(15, 280, 4017, 4019, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 280, 50, 20, 1011012, LabelColor, false, false); //CANCEL
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            ImbuingContext context = Imbuing.GetContext(from);

            switch (info.ButtonID)
            {
                case 0: // Close
                case 1:
                    {
                        from.EndAction(typeof(Imbuing));

                        break;
                    }
                case 10005:  // Imbue Item
                    {
                        from.SendLocalizedMessage(1079589);  //Target an item you wish to imbue.

                        from.Target = new ImbueItemTarget();
                        from.Target.BeginTimeout(from, TimeSpan.FromSeconds(10.0));

                        break;
                    }
                case 10006:  // Reimbue Last
                    {
                        Item it = context.LastImbued;
                        int mod = context.Imbue_Mod;
                        int modint = context.Imbue_ModInt;        

                        if (it == null || mod < 0 || modint == 0)
                        {
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            from.EndAction(typeof(Imbuing));
                            break;
                        }

                        if (Imbuing.CanImbueItem(from, it) && Imbuing.OnBeforeImbue(from, it, mod, modint))
                        {
                            Imbuing.ImbueItem(from, it, mod, modint);
                            from.SendGump(new ImbuingGump(from));
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
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            from.EndAction(typeof(Imbuing));
                            break;
                        }
                        else
                        {
                            ImbueStep1(from, item);
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
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            from.EndAction(typeof(Imbuing));
                            break;
                        }
                        else
                            ImbuingGump.ImbueLastProp(from, mod, modint);

                        break;
                    }
                case 10010:  // Unravel Item
                    {
                        from.SendLocalizedMessage(1080422); // Target an item you wish to magically unravel.

                        from.Target = new UnravelTarget();
                        from.Target.BeginTimeout(from, TimeSpan.FromSeconds(10.0));

                        break;
                    }
                case 10011:  // Unravel Container
                    {
                        from.SendLocalizedMessage(1080422); // Target an item you wish to magically unravel.

                        from.Target = new UnravelContainerTarget();
                        from.Target.BeginTimeout(from, TimeSpan.FromSeconds(10.0));

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

            protected override void OnTarget(Mobile from, object o)
            {
                from.EndAction(typeof(Imbuing));

                if (!(o is Item))
                {
                    from.SendLocalizedMessage(1080425); // You cannot magically unravel this item.
                    return;
                }

                if (Imbuing.CanUnravelItem(from, (Item)o))
                {
                    from.BeginAction(typeof(Imbuing));
                    from.SendGump(new UnravelGump((Item)o));
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.EndAction(typeof(Imbuing));
            }

            private class UnravelGump : Gump
            {
                private Item m_Item;

                public UnravelGump(Item item) : base(60, 36)
                {
                    m_Item = item;

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

                public override void OnResponse(NetState sender, RelayInfo info)
                {                    
                    Mobile from = sender.Mobile;
                    from.EndAction(typeof(Imbuing));

                    if (info.ButtonID == 0 || m_Item.Deleted)
                        return;
                    
                    if (Imbuing.CanUnravelItem(from, m_Item) && Imbuing.UnravelItem(from, m_Item))
                    {
                        Effects.SendPacket(from, from.Map, new GraphicalEffect(EffectType.FixedFrom, from.Serial, Server.Serial.Zero, 0x375A, from.Location, from.Location, 1, 17, true, false));
                        from.PlaySound(0x1EB);

                        from.SendLocalizedMessage(1080429); // You magically unravel the item!
                        from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                    }
                }
            }
        }

        private class UnravelContainerTarget : Target
        {
            public UnravelContainerTarget() : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object o)
            {
                from.EndAction(typeof(Imbuing));

                if (!(o is Container))
                {
                    from.SendLocalizedMessage(1080425); // You cannot magically unravel this item.
                    return;
                }

                Container cont = o as Container;

                if (!cont.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                    from.EndAction(typeof(Imbuing));
                }
                else if (cont == null)
                {
                    from.SendLocalizedMessage(1111814, "0\t0"); // Unraveled: ~1_COUNT~/~2_NUM~ items
                    from.EndAction(typeof(Imbuing));
                }
                else
                {
                    bool unraveled = cont.Items.FirstOrDefault(x => Imbuing.CanUnravelItem(from, x, false)) != null;

                    if (unraveled)
                    {
                        from.BeginAction(typeof(Imbuing));
                        from.SendGump(new UnravelContainerGump(cont));                        
                    }
                    else
                    {
                        TryUnravelContainer(from, cont);
                        from.EndAction(typeof(Imbuing));
                    }
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.EndAction(typeof(Imbuing));
            }

            public static void TryUnravelContainer(Mobile from, Container c)
            {
                c.Items.ForEach(y =>
                {
                    Imbuing.CanUnravelItem(from, y, true);
                });

                from.SendLocalizedMessage(1111814, String.Format("{0}\t{1}", 0, c.Items.Count)); // Unraveled: ~1_COUNT~/~2_NUM~ items
            }            

            private class UnravelContainerGump : Gump
            {
                private Container m_Container;
                private List<Item> m_List;

                public UnravelContainerGump(Container c) 
                    : base(25, 50)
                {
                    m_Container = c;
                    m_List = new List<Item>(c.Items);

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

                public override void OnResponse(NetState sender, RelayInfo info)
                {
                    Mobile from = sender.Mobile;
                    from.EndAction(typeof(Imbuing));

                    if (m_Container == null || m_List == null)
                        return;

                    if (info.ButtonID == 0)
                    {
                        TryUnravelContainer(from, m_Container);
                        return;
                    }

                    int count = 0;

                    m_List.ForEach(x =>
                    {
                        if (Imbuing.CanUnravelItem(from, x, true) && Imbuing.UnravelItem(from, x, true))
                        {
                            count++;
                        }
                    });

                    if (count > 0)
                    {
                        from.SendLocalizedMessage(1080429); // You magically unravel the item!
                        from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.
                    }

                    from.SendLocalizedMessage(1111814, String.Format("{0}\t{1}", count, m_List.Count));

                    m_List.Clear();
                    m_List.TrimExcess();
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

            protected override void OnTarget(Mobile from, object o)
            {
                if (!(o is Item))
                {
                    from.SendLocalizedMessage(1079576); // You cannot imbue this item.
                    return;
                }
                
                Item item = (Item)o;
                ImbuingContext context = Imbuing.GetContext(from);

                int itemRef = ImbuingGump.GetItemRef(o);

                if (itemRef == 0)
                {
                    from.SendLocalizedMessage(1079576); // You cannot imbue this item.
                    return;
                }

                ImbuingGump.ImbueStep1(from, item);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.EndAction(typeof(Imbuing));
            }
        }

        public static int GetItemRef(object i)
        {
            if (i is BaseRanged) return 2;
            if (i is BaseWeapon) return 1;
            if (i is BaseShield) return 4;
            if (i is BaseArmor) return 3;
            if (i is BaseHat) return 5;
            if (i is BaseJewel) return 6;

            return 0;
        }

        public static void ImbueStep1(Mobile from, Item it)
        {
            ImbuingContext context = Imbuing.GetContext(from);

            if (Imbuing.CanImbueItem(from, it))
            {
                context.LastImbued = it;

                if (context.ImbMenu_Cat == 0)
                    context.ImbMenu_Cat = 1;

                from.CloseGump(typeof(ImbuingGump));
                from.SendGump(new ImbuingGumpB(from, it));
            }
        }

        public static void ImbueLastProp(Mobile from, int Mod, int Mint)
        {
            from.Target = new ImbueLastModTarget();
            return;
        }

        private class ImbueLastModTarget : Target
        {
            public ImbueLastModTarget()
                : base(-1, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (!(o is Item))
                {
                    from.SendLocalizedMessage(1079576); // You cannot imbue this item.
                    return;
                }                    
                
                ImbuingContext context = Imbuing.GetContext(from);

                int mod = context.Imbue_Mod;
                int modInt = context.Imbue_ModInt;

                Item it = o as Item;

                if (!Imbuing.CanImbueItem(from, it) || !Imbuing.OnBeforeImbue(from, it, mod, modInt) || !Imbuing.CanImbueProperty(from, it, mod))
                {
                    from.SendGump(new ImbuingGump(from));
                    return;
                }

                Imbuing.ImbueItem(from, it, mod, modInt);
                ImbuingGumpC.SendGumpDelayed(from);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                from.EndAction(typeof(Imbuing));
            }
        }
    }
}