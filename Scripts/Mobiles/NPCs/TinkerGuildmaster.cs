using Server.ContextMenus;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class TinkerGuildmaster : BaseGuildmaster
    {
        [Constructable]
        public TinkerGuildmaster()
            : base("tinker")
        {
            SetSkill(SkillName.Lockpicking, 65.0, 88.0);
            SetSkill(SkillName.Tinkering, 90.0, 100.0);
            SetSkill(SkillName.RemoveTrap, 85.0, 100.0);
        }

        public TinkerGuildmaster(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.TinkersGuild;
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
    }
}
