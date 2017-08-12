using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.BulkOrders
{
    public class SmallBODGump : Gump
    {
        private readonly SmallBOD m_Deed;
        private readonly Mobile m_From;

        public SmallBODGump(Mobile from, SmallBOD deed)
            : base(25, 25)
        {
            this.m_From = from;
            this.m_Deed = deed;

            this.m_From.CloseGump(typeof(LargeBODGump));
            this.m_From.CloseGump(typeof(SmallBODGump));

            this.AddPage(0);

            int height = 0;

            if (BulkOrderSystem.NewSystemEnabled)
            {
                if (deed.RequireExceptional || deed.Material != BulkMaterialType.None)
                    height += 24;

                if (deed.RequireExceptional)
                    height += 24;

                if (deed.Material != BulkMaterialType.None)
                    height += 24;
            }

            this.AddBackground(50, 10, 455, 245 + height, 5054);
            this.AddImageTiled(58, 20, 438, 226 + height, 2624);
            this.AddAlphaRegion(58, 20, 438, 226 + height);

            this.AddImage(45, 5, 10460);
            this.AddImage(480, 5, 10460);
            this.AddImage(45, 230 + height, 10460);
            this.AddImage(480, 230 + height, 10460);

            this.AddHtmlLocalized(225, 25, 120, 20, 1045133, 0x7FFF, false, false); // A bulk order

            this.AddHtmlLocalized(75, 48, 250, 20, 1045138, 0x7FFF, false, false); // Amount to make:
            this.AddLabel(275, 48, 1152, deed.AmountMax.ToString());

            this.AddHtmlLocalized(275, 76, 200, 20, 1045153, 0x7FFF, false, false); // Amount finished:
            this.AddHtmlLocalized(75, 72, 120, 20, 1045136, 0x7FFF, false, false); // Item requested:

            this.AddItem(410, 72, deed.Graphic, deed.GraphicHue);

            this.AddHtmlLocalized(75, 96, 210, 20, deed.Number, 0x7FFF, false, false);
            this.AddLabel(275, 96, 0x480, deed.AmountCur.ToString());

            int y = 120;

            if (deed.RequireExceptional || deed.Material != BulkMaterialType.None)
            {
                this.AddHtmlLocalized(75, y, 200, 20, 1045140, 0x7FFF, false, false); // Special requirements to meet:
                y += 24;
            }

            if (deed.RequireExceptional)
            {
                this.AddHtmlLocalized(75, y, 300, 20, 1045141, 0x7FFF, false, false); // All items must be exceptional.
                y += 24;
            }

            if (deed.Material != BulkMaterialType.None)
            {
                this.AddHtmlLocalized(75, y, 300, 20, GetMaterialNumberFor(deed.Material), 0x7FFF, false, false); // All items must be made with x material.
                y += 24;
            }

            if (from is PlayerMobile && BulkOrderSystem.NewSystemEnabled)
            {
                BODContext c = BulkOrderSystem.GetContext((PlayerMobile)from);

                int points = 0;
                double banked = 0.0;
                
                BulkOrderSystem.ComputePoints(deed, out points, out banked);

                AddHtmlLocalized(75, y, 300, 20, 1157301, String.Format("{0}\t{1}", points, banked.ToString("0.000000")), 0x7FFF, false, false); // Worth ~1_POINTS~ turn in points and ~2_POINTS~ bank points.
                y += 24;

                AddButton(125, y, 4005, 4007, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, y, 300, 20, c.PointsMode == PointsMode.Enabled ? 1157302 : c.PointsMode == PointsMode.Disabled ? 1157303 : 1157309, 0x7FFF, false, false); // Banking Points Enabled/Disabled/Automatic
                y += 24;

                AddButton(125, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, y, 300, 20, 1045154, 0x7FFF, false, false); // Combine this deed with the item requested.
                y += 24;

                this.AddButton(125, y, 4005, 4007, 4, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(160, y, 300, 20, 1157304, 0x7FFF, false, false); // Combine this deed with contained items.
                y += 24;
            }
            else
            {
                this.AddButton(125, y, 4005, 4007, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(160, y, 300, 20, 1045154, 0x7FFF, false, false); // Combine this deed with the item requested.
                y += 24;
            }

            this.AddButton(125, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(160, y, 120, 20, 1011441, 0x7FFF, false, false); // EXIT
        }

        public static int GetMaterialNumberFor(BulkMaterialType material)
        {
            if (material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite)
                return 1045142 + (int)(material - BulkMaterialType.DullCopper);
            else if (material >= BulkMaterialType.Spined && material <= BulkMaterialType.Barbed)
                return 1049348 + (int)(material - BulkMaterialType.Spined);
            else if (material >= BulkMaterialType.OakWood && material <= BulkMaterialType.Frostwood)
            {
                switch (material)
                {
                    case BulkMaterialType.OakWood: return 1071428;
                    case BulkMaterialType.AshWood: return 1071429;
                    case BulkMaterialType.YewWood: return 1071430;
                    case BulkMaterialType.Heartwood: return 1071432;
                    case BulkMaterialType.Bloodwood: return 1071431;
                    case BulkMaterialType.Frostwood: return 1071433;
                }
            }
            return 0;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Deed.Deleted || !this.m_Deed.IsChildOf(this.m_From.Backpack))
                return;

            switch (info.ButtonID)
            {
                case 2: // Combine
                    {
                        this.m_From.SendGump(new SmallBODGump(this.m_From, this.m_Deed));
                        this.m_Deed.BeginCombine(this.m_From);
                        break;
                    }
                case 3: // points mode
                    {
                        BODContext c = BulkOrderSystem.GetContext(m_From);

                        if (c != null)
                        {
                            switch (c.PointsMode)
                            {
                                case PointsMode.Enabled: c.PointsMode = PointsMode.Disabled; break;
                                case PointsMode.Disabled: c.PointsMode = PointsMode.Automatic; break;
                                case PointsMode.Automatic: c.PointsMode = PointsMode.Enabled; break;
                            }
                        }

                        m_From.SendGump(new SmallBODGump(this.m_From, this.m_Deed));
                        break;
                    }
                case 4: // combine from container
                    {
                        m_From.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, (m, targeted) =>
                            {
                                if (!m_Deed.Deleted && targeted is Container)
                                {
                                    List<Item> list = new List<Item>(((Container)targeted).Items);

                                    foreach (Item item in list)
                                    {
                                        m_Deed.EndCombine(m_From, item);
                                    }
                                }
                            });
                        break;
                    }
            }
        }
    }
}