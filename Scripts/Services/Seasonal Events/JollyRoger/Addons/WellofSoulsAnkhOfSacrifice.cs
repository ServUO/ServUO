using Server.Engines.JollyRoger;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Linq;

namespace Server.Items
{
    public class WOSAnkhOfSacrifice : BaseAddon
    {
        [Constructable]
        public WOSAnkhOfSacrifice()
            : base()
        {
            AddComponent(new AnkhOfSacrificeComponent(0x1E5D), 0, 0, 0);
            AddComponent(new AnkhOfSacrificeComponent(0x1E5C), 1, 0, 0);
        }

        public WOSAnkhOfSacrifice(Serial serial)
            : base(serial)
        {
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            var l = JollyRogerData.GetList(from);
            var pm = from as PlayerMobile;

            if (pm != null && l != null && l.Shrine != null)
            {
                var title = JollyRogerData.GetShrineTitle(pm);

                if (title > 0)
                {
                    var shrine = JollyRogerData.GetShrine(title);
                    var s = l.Shrine.FirstOrDefault(y => y.Shrine == shrine);

                    if (s != null)
                    {
                        var count = s.MasterDeath;

                        if (count >= 8)
                        {
                            from.CloseGump(typeof(TabardRewardGump));
                            from.SendGump(new TabardRewardGump(shrine));
                        }
                        else if (count < 3)
                        {
                            from.SendLocalizedMessage(1159362); // Thou art virtuous, but have not truly fought for virtue.
                        }
                        else
                        {
                            from.SendLocalizedMessage(1159370,
                                shrine.ToString()); // Thou art virtuous, but have not truly fought for ~1_VIRTUE~
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1159361); // Thou art not virtuous...
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1159361); // Thou art not virtuous...
                }
            }
            else
            {
                from.SendLocalizedMessage(1159361); // Thou art not virtuous...
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadEncodedInt();

        }
    }

    public class TabardRewardGump : Gump
    {
        public Shrine _Shrine { get; set; }

        public TabardRewardGump(Shrine shrine)
            : base(100, 100)
        {
            _Shrine = shrine;

            AddPage(0);

            AddBackground(0, 0, 370, 470, 0x6DB);
            AddItem(145, 70, 0x2);
            AddItem(145, 50, 0x376F);
            AddItem(167, 70, 0x3);
            AddItem(75, 160, 0xA517);
            AddItem(50, 110, 0xA519);
            AddItem(50, 60, 0xA51B);
            AddItem(75, 20, 0xA51D);
            AddItem(250, 20, 0xA520);
            AddItem(275, 60, 0xA522);
            AddItem(275, 110, 0xA524);
            AddItem(250, 160, 0xA526);
            AddHtmlLocalized(10, 250, 350, 72, 1114513, "#1159363", 0x43FF, false,
                false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddButton(10, 337, 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(35, 337, 150, 34, 1159364, 0x7FFF, false, false); // Claim Reward
            AddButton(185, 337, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(210, 337, 200, 34, 1159365, 0x7FFF, false, false); // Convert Existing Reward
            AddItem(50, 375, 0xA412);
            AddItem(200, 375, 0x7816);
            AddTooltip(1156299);
            AddImage(250, 370, 0x1196);
            AddItem(300, 385, 0xA412);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        var l = JollyRogerData.GetList(from);

                        if (l != null)
                        {
                            if (l.Tabard)
                            {
                                from.SendLocalizedMessage(1152687); // You've already claimed your reward! 
                            }
                            else
                            {
                                from.CloseGump(typeof(TabardClaimConfirmGump));
                                from.SendGump(new TabardClaimConfirmGump(_Shrine));
                            }
                        }

                        break;
                    }
                case 2:
                    {
                        from.CloseGump(typeof(TabardConvertConfirmGump));
                        from.SendGump(new TabardConvertConfirmGump());
                        break;
                    }
            }
        }
    }

    public class TabardClaimConfirmGump : Gump
    {
        public Shrine _Shrine { get; set; }

        public TabardClaimConfirmGump(Shrine shrine)
            : base(340, 340)
        {
            _Shrine = shrine;

            AddPage(0);

            AddBackground(0, 0, 291, 179, 0x13BE);
            AddImageTiled(5, 6, 280, 20, 0xA40);
            AddHtmlLocalized(9, 8, 280, 20, 1159367, 0x7FFF, false, false); // Tabard of Virtue
            AddImageTiled(5, 31, 280, 120, 0xA40);
            AddHtmlLocalized(9, 35, 272, 120, 1159366, 0x7FFF, false, false); // You are about to claim your Tabard of Virtue. The tabard will be named and hued based on your current devotion to the virtues shown on your character mouseover.  Are you sure you wish to claim this tabard right now?
            AddButton(5, 153, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 155, 100, 20, 1011012, 0x7FFF, false, false); // CANCEL
            AddButton(160, 153, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 155, 120, 20, 1006044, 0x7FFF, false, false); // OK
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                Mobile from = sender.Mobile;

                var item = new Tabard(_Shrine);

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
                {
                    from.SendLocalizedMessage(1152338); // A reward item will be delivered to you once you free up room in your backpack.
                    item.Delete();
                }
                else
                {
                    JollyRogerData.SetTabard(from, true);
                    from.SendLocalizedMessage(1152340); // A reward item has been placed in your backpack.
                    from.PlaySound(0x419);
                }
            }
        }
    }

    public class TabardConvertConfirmGump : Gump
    {
        public TabardConvertConfirmGump()
            : base(340, 340)
        {
            AddPage(0);

            AddBackground(0, 0, 291, 179, 0x13BE);
            AddImageTiled(5, 6, 280, 20, 0xA40);
            AddHtmlLocalized(9, 8, 280, 20, 1159367, 0x7FFF, false, false); // Tabard of Virtue
            AddImageTiled(5, 31, 280, 120, 0xA40);
            AddHtmlLocalized(9, 35, 272, 120, 1159368, 0x7FFF, false, false); // You are about to apply special properties to your Tabard of Virtue. The item you are applying the special properties from will be destroyed and your Tabard of Virtue will then have those properties.  Do you wish to proceed?
            AddButton(5, 153, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 155, 100, 20, 1011012, 0x7FFF, false, false); // CANCEL
            AddButton(160, 153, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 155, 120, 20, 1006044, 0x7FFF, false, false); // OK
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                Mobile from = sender.Mobile;

                var robe = from.Backpack.FindItemByType(typeof(HawkwindsRobe));

                if (robe != null)
                {
                    var tabard = (Tabard)from.Backpack.FindItemByType(typeof(Tabard));

                    if (tabard != null && !tabard.Converted)
                    {
                        robe.Delete();
                        tabard.CovertRobe();
                    }
                    else
                    {
                        from.SendLocalizedMessage(1159372); // The item you wish to apply special properties TO could not be found in your main backpack.  Please check your backpack and try again.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1159371); // The item you wish to apply special properties FROM could not be found in your main backpack.  Please check your backpack and try again.
                }
            }
        }
    }
}
