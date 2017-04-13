using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.ContextMenus;
using Server.Network;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public abstract class BaseTurnInMobile : ShrineHealer
    {
        public override bool IsInvulnerable { get { return true; } }
        public override bool DisallowAllMoves { get { return true; } }
        public override bool ClickTitle { get { return true; } }
        public override bool CanTeach { get { return false; } }

        public virtual int TitleLocalization { get { return 0; } }
        public virtual int CancelLocalization { get { return 0; } }
        public virtual int TurnInLocalization { get { return 0; } }
        public virtual int ClaimLocalization { get { return 1155593; } } // Claim Rewards

        public virtual int TurnInPoints { get { return 1; } }

        public BaseTurnInMobile(string title)
        {
            Title = title;
        }

        public virtual IEnumerable<ItemTileButtonInfo> FindRedeemableItems(Mobile m)
        {
            if (m == null || m.Backpack == null)
                yield break;

            foreach (var item in m.Backpack.Items)
            {
                if (IsRedeemableItem(item))
                    yield return new ItemTileButtonInfo(item);
            }
        }

        public abstract bool IsRedeemableItem(Item item);
        public abstract void SendRewardGump(Mobile m);

        public virtual void AwardPoints(PlayerMobile pm, int points)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from is PlayerMobile)
            {
                if (TurnInLocalization > 0)
                {
                    list.Add(new TurnInEntry(from, this));
                }

                list.Add(new ClaimEntry(from, this));
            }
        }

        private class TurnInEntry : ContextMenuEntry
        {
            private Mobile m_Mobile;
            private BaseTurnInMobile m_Vendor;
            private IEnumerable<ItemTileButtonInfo> m_Buttons;

            public TurnInEntry(Mobile mobile, BaseTurnInMobile vendor)
                : base(vendor.TurnInLocalization, 2)
            {
                m_Mobile = mobile;
                m_Vendor = vendor;

                m_Buttons = m_Vendor.FindRedeemableItems(m_Mobile);

                if (m_Buttons.Count() > 0)
                    Enabled = true;
                else
                    Enabled = false;
            }

            public override void OnClick()
            {
                m_Mobile.SendGump(new TurnInGump(m_Vendor, m_Buttons));
            }
        }

        private class ClaimEntry : ContextMenuEntry
        {
            private Mobile m_Mobile;
            private BaseTurnInMobile m_Vendor;

            public ClaimEntry(Mobile mobile, BaseTurnInMobile vendor)
                : base(vendor.ClaimLocalization, 2)
            {
                m_Mobile = mobile;
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                if (m_Mobile.CheckAlive())
                    m_Vendor.SendRewardGump(m_Mobile);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile && from.InRange(Location, 5))
                SendRewardGump(from);
        }

        public BaseTurnInMobile(Serial serial)
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

    public class TurnInGump : BaseImageTileButtonsGump
    {
        private BaseTurnInMobile m_Collector;

        public BaseTurnInMobile Collector { get { return m_Collector; } }

        public TurnInGump(BaseTurnInMobile collector, IEnumerable<ItemTileButtonInfo> buttons)
            : base(collector.TitleLocalization, buttons.ToArray())
        {
            m_Collector = collector;
        }

        public override void HandleButtonResponse(NetState sender, int adjustedButton, ImageTileButtonInfo buttonInfo)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;

            Item item = ((ItemTileButtonInfo)buttonInfo).Item;

            if (!(pm != null && item.IsChildOf(pm.Backpack) && pm.InRange(this.m_Collector.Location, 7)))
                return;

            item.Delete();

            m_Collector.AwardPoints(pm, m_Collector.TurnInPoints);

            IEnumerable<ItemTileButtonInfo> buttons = m_Collector.FindRedeemableItems(pm);

            if (buttons != null && buttons.Count() > 0)
                pm.SendGump(new TurnInGump(m_Collector, buttons));
        }

        public override void HandleCancel(NetState sender)
        {
            m_Collector.LocalOverheadMessage(MessageType.Regular, 0x3B2, m_Collector.CancelLocalization);
        }
    }
}