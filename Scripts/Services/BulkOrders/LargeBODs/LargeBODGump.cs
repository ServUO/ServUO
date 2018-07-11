using System;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.BulkOrders
{
    public class LargeBODGump : Gump
    {
        private readonly LargeBOD m_Deed;
        private readonly Mobile m_From;
        public LargeBODGump(Mobile from, LargeBOD deed)
            : base(25, 25)
        {
            m_From = from;
            m_Deed = deed;

            m_From.CloseGump(typeof(LargeBODGump));
            m_From.CloseGump(typeof(SmallBODGump));

            LargeBulkEntry[] entries = deed.Entries;

            AddPage(0);

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

            AddBackground(50, 10, 455, 218 + height + (entries.Length * 24), 5054);

            AddImageTiled(58, 20, 438, 200 + height + (entries.Length * 24), 2624);
            AddAlphaRegion(58, 20, 438, 200 + height + (entries.Length * 24));

            AddImage(45, 5, 10460);
            AddImage(480, 5, 10460);
            AddImage(45, 203 + height + (entries.Length * 24), 10460);
            AddImage(480, 203 + height + (entries.Length * 24), 10460);

            AddHtmlLocalized(225, 25, 120, 20, 1045134, 0x7FFF, false, false); // A large bulk order

            AddHtmlLocalized(75, 48, 250, 20, 1045138, 0x7FFF, false, false); // Amount to make:
            AddLabel(275, 48, 1152, deed.AmountMax.ToString());

            AddHtmlLocalized(75, 72, 120, 20, 1045137, 0x7FFF, false, false); // Items requested:
            AddHtmlLocalized(275, 76, 200, 20, 1045153, 0x7FFF, false, false); // Amount finished:

            int y = 96;

            for (int i = 0; i < entries.Length; ++i)
            {
                LargeBulkEntry entry = entries[i];
                SmallBulkEntry details = entry.Details;

                AddHtmlLocalized(75, y, 210, 20, details.Number, 0x7FFF, false, false);
                AddLabel(275, y, 0x480, entry.Amount.ToString());

                y += 24;
            }

            if (deed.RequireExceptional || deed.Material != BulkMaterialType.None)
            {
                AddHtmlLocalized(75, y, 200, 20, 1045140, 0x7FFF, false, false); // Special requirements to meet:
                y += 24;
            }

            if (deed.RequireExceptional)
            {
                AddHtmlLocalized(75, y, 300, 20, 1045141, 0x7FFF, false, false); // All items must be exceptional.
                y += 24;
            }

            if (deed.Material != BulkMaterialType.None)
            {
                AddHtmlLocalized(75, y, 300, 20, SmallBODGump.GetMaterialNumberFor(deed.Material), 0x7FFF, false, false); // All items must be made with x material.
                y += 24;
            }

            if (BulkOrderSystem.NewSystemEnabled)
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

                AddButton(125, y, 4005, 4007, 4, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, y, 300, 20, 1157304, 0x7FFF, false, false); // Combine this deed with contained items.
                y += 24;

                AddButton(125, y, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, y, 120, 20, 1011441, 0x7FFF, false, false); // EXIT
            }
            else
            {
                AddButton(125, 168 + (entries.Length * 24), 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, 168 + (entries.Length * 24), 300, 20, 1045155, 0x7FFF, false, false); // Combine this deed with another deed.

                AddButton(125, 192 + (entries.Length * 24), 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, 192 + (entries.Length * 24), 120, 20, 1011441, 0x7FFF, false, false); // EXIT
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Deed.Deleted || !m_Deed.IsChildOf(m_From.Backpack))
                return;

            if (info.ButtonID == 2) // Combine
            {
                m_From.SendGump(new LargeBODGump(m_From, m_Deed));
                m_Deed.BeginCombine(m_From);
            }
            else if (info.ButtonID == 3) // bank button
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

                m_From.SendGump(new LargeBODGump(m_From, m_Deed));
            }
            else if (info.ButtonID == 4) // combine from container
            {
                m_From.BeginTarget(-1, false, Server.Targeting.TargetFlags.None, (m, targeted) =>
                {
                    if (!m_Deed.Deleted && targeted is Container)
                    {
                        List<SmallBOD> list = ((Container)targeted).Items.OfType<SmallBOD>().ToList();

                        foreach (SmallBOD item in list)
                        {
                            m_Deed.EndCombine(m_From, item);
                        }

                        list.Clear();
                    }
                });
            }
        }
    }
}