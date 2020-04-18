using Server.ContextMenus;
using Server.Engines.BulkOrders;
using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Tinker : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public Tinker()
            : base("the tinker")
        {
            SetSkill(SkillName.Lockpicking, 60.0, 83.0);
            SetSkill(SkillName.RemoveTrap, 75.0, 98.0);
            SetSkill(SkillName.Tinkering, 64.0, 100.0);
        }

        public Tinker(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.TinkersGuild;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBTinker(this));
        }

        #region Bulk Orders
        public override BODType BODType => BODType.Tinkering;

        public override bool IsValidBulkOrder(Item item)
        {
            return (item is SmallTinkerBOD || item is LargeTinkerBOD);
        }

        public override bool SupportsBulkOrders(Mobile from)
        {
            return BulkOrderSystem.NewSystemEnabled && from is PlayerMobile && from.Skills[SkillName.Tinkering].Base > 0;
        }

        public override void OnSuccessfulBulkOrderReceive(Mobile from)
        {
            if (from is PlayerMobile)
                ((PlayerMobile)from).NextTinkeringBulkOrder = TimeSpan.Zero;
        }

        #endregion

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.AddCustomContextEntries(from, list);

            if (from.Alive)
            {
                list.Add(new RechargeEntry(from, this));
            }
        }

        private class RechargeEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly Mobile m_Vendor;
            private readonly BaseEngravingTool Tool;

            public RechargeEntry(Mobile from, Mobile vendor)
                : base(6271, 6)
            {
                m_From = from;
                m_Vendor = vendor;

                Tool = BaseEngravingTool.Find(from);

                Enabled = Tool != null;
            }

            public override void OnClick()
            {
                if (m_Vendor == null || m_Vendor.Deleted)
                    return;

                if (Tool != null)
                {
                    if (Banker.GetBalance(m_From) >= 100000)
                        m_From.SendGump(new BaseEngravingTool.ConfirmGump(Tool, m_Vendor));
                    else
                        m_Vendor.Say(1076167); // You need a 100,000 gold and a blue diamond to recharge the weapon engraver.
                }
                else
                    m_Vendor.Say(1076164); // I can only help with this if you are carrying an engraving tool that needs repair.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
