using System;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Commands;
using Server.Targeting;
using Server.SkillHandlers;

namespace Server.Gumps
{
    public class ImbuingGump : Gump
    {
        private const int LabelHue = 0x480;
        private const int LabelColor = 0x7FFF;  //Localized
        private const int FontColor = 0xFFFFFF; //string

        public ImbuingGump(Mobile from)
            : base(520, 340)
        {
            Mobile m = from;
            PlayerMobile pm = from as PlayerMobile;

            from.CloseGump(typeof(ImbuingGumpB));
            from.CloseGump(typeof(ImbuingGumpC));

            ImbuingContext context = Imbuing.GetContext(pm);

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

            AddButton(15, 280, 4017, 4019, 10002, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 280, 50, 20, 1011012, FontColor, false, false); //CANCEL

        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            PlayerMobile pm = from as PlayerMobile;

            ImbuingContext context = Imbuing.GetContext(pm);

            int buttonNum = 0;

            if (info.ButtonID > 0 && info.ButtonID < 10000)
                buttonNum = 1;
            else if (info.ButtonID > 20004)
                buttonNum = 30000;
            else
                buttonNum = info.ButtonID;

            switch (buttonNum)
            {
                case 0:
                    {
                        //Close
                        break;
                    }
                case 1:
                    {
                        break;
                    }
                case 10002:  // = Cancel button
                    {
                        break;
                    }
                case 10005:  // = Imbue Item
                    {
                        if (!Imbuing.CheckSoulForge(from, 1))
                            break;

                        from.SendLocalizedMessage(1079589);  //Target an item you wish to imbue.
                        from.Target = new ImbueItemTarget();

                        break;
                    }
                case 10006:  // = ReImbue Last ( Mod & Item )
                    {
                        Item it = context.LastImbued;
                        int mod = context.Imbue_Mod;
                        int modint = context.Imbue_ModInt;

                        if (!Imbuing.CheckSoulForge(from, 1))
                            break;

                        if (it == null || mod < 0 || modint == 0)
                        {
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            break;
                        }

                        if (Imbuing.CanImbueItem(pm, it) && Imbuing.OnBeforeImbue(from, it, mod, modint))
                        {
                            Imbuing.ImbueItem(from, it, mod, modint);
                            from.SendGump(new ImbuingGump(from));
                        }
                        break;
                    }

                case 10007:  // = Imbue Last ( Select Last imbued Item )
                    {
                        Item item = context.LastImbued;
                        int mod = context.Imbue_Mod;
                        int modint = context.Imbue_ModInt;

                        if(!Imbuing.CheckSoulForge(from, 1))
                            break;

                        if (context.LastImbued == null)
                        {
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            break;
                        }
                        else
                        {
                            ImbueStep1(from, item);
                        }
                        break;
                    }
                case 10008:  // = Imbue Last Mod( To target Item )
                    {
                        context.LastImbued = null;
                        int mod = context.Imbue_Mod;
                        int modint = context.Imbue_ModInt;

                        if (modint < 0)
                            modint = 0;

                        if (!Imbuing.CheckSoulForge(from, 1))
                            break;

                        if (mod < 0)
                        {
                            from.SendLocalizedMessage(1113572); // You haven't imbued anything yet!
                            break;
                        }
                        else
                            ImbuingGump.ImbueLastProp(from, mod, modint);

                        break;
                    }
                case 10010:  // = Unravel Item
                    {
                        if (!Imbuing.CheckSoulForge(from, 1))
                            break;

                        from.SendLocalizedMessage(1080422); // What item do you wish to unravel?
                        from.Target = new UnravelTarget();
                        break;
                    }
                case 10011:  // = Unravel Container
                    {
                        if (!Imbuing.CheckSoulForge(from, 1))
                            break;

                        from.SendMessage("Which Container do you wish to unravel the contents of?");

                        from.Target = new UnravelContainerTarget();
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
                if (!Imbuing.CheckSoulForge(from, 1))
                    return;

                if (o is BaseWeapon || o is BaseArmor || o is BaseJewel || o is BaseHat)
                {
                    if (Imbuing.CanUnravelItem(from, (Item)o) && Imbuing.UnravelItem(from, (Item)o))
                    {
                        Effects.PlaySound(from.Location, from.Map, 0x1ED);
                        Effects.SendLocationParticles(
                            EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x373A,
                            10, 30, 0, 4, 0, 0);
                        from.SendLocalizedMessage(1080429); // Unravelled :P
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1080425); // You cannot magically unravel this item.
                    return;
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
                if (!Imbuing.CheckSoulForge(from, 1))
                    return;

                if (o is Container)
                {
                    Container cont = (Container)o;

                    if(cont.IsChildOf(from.Backpack) || cont == from.Backpack)
                    {
                        int unraveled = 0;
                        List<Item> list = new List<Item>(cont.Items);

                        foreach (Item item in list)
                        {
                            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat)
                            {
                                if (Imbuing.CanUnravelItem(from, item, false) && Imbuing.UnravelItem(from, item, false))
                                    unraveled++;
                            }
                        }

                        list.Clear();
                        list.TrimExcess();

                        if (unraveled <= 0)
                        {
                            from.SendMessage(2499, "You fail to unravel any items in the selected container.");
                            Effects.PlaySound(from.Location, from.Map, 0x3BF);
                        }
                        else
                        {
                            Effects.PlaySound(from.Location, from.Map, 0x1ED);
                            Effects.SendLocationParticles(
                                EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x373A,
                                10, 30, 0, 4, 0, 0);

                            from.SendMessage(2499, "You successfully unravel {0} items from the selected container.", unraveled);
                        }
                    }
                    else
                    {
                        from.SendMessage(2499, "The container must be in your backpack to magically unravel its contents.");
                        return;
                    }
                }
                else
                {
                    from.SendMessage(2499, "You must target a container.");
                    return;
                }
                return;
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
                if (o is Item)
                {
                    Item item = (Item)o;
                    PlayerMobile pm = from as PlayerMobile;
                    ImbuingContext context = Imbuing.GetContext(pm);

                    int itemRef = ImbuingGump.GetItemRef(o);

                    if (itemRef == 0)
                    {
                        from.SendLocalizedMessage(1079576); // You cannot imbue this item.
                        return;
                    }

                    ImbuingGump.ImbueStep1(from, item);
                }
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
            PlayerMobile pm = from as PlayerMobile;
            ImbuingContext context = Imbuing.GetContext(pm);

            if (Imbuing.CanImbueItem(pm, it))
            {
                context.LastImbued = it;

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
                    return;

                PlayerMobile pm = from as PlayerMobile;
                ImbuingContext context = Imbuing.GetContext(pm);

                int mod = context.Imbue_Mod;
                int modInt = context.Imbue_ModInt;

                Item it = o as Item;

                if (!Imbuing.CanImbueItem(pm, it) || !Imbuing.OnBeforeImbue(from, it, mod, modInt) || !Imbuing.CanImbueProperty(from, it, mod))
                {
                    from.SendGump(new ImbuingGump(from));
                    return;
                }

                Imbuing.ImbueItem(from, it, mod, modInt);
                ImbuingGumpC.SendGumpDelayed(from);
            }
        }
    }
}