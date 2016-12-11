using System;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class ForgedMetalOfArtifacts : Item
    {
        private int m_UsesRemaining;

        public override int LabelNumber { get { return 1149868; } } // Forged Metal of Artifacts

        [Constructable]
        public ForgedMetalOfArtifacts(int uses)
            : base(0xE8A)
        {
            Weight = 5.0;
            Hue = 51;
            ItemID = 4023;
            m_UsesRemaining = uses;
            LootType = LootType.Blessed;
        }

        [Constructable]
        public ForgedMetalOfArtifacts()
            : this(5)
        {
        }

        public ForgedMetalOfArtifacts(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_UsesRemaining = reader.ReadInt();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            from.CloseGump(typeof(InternalGump));
            from.CloseGump(typeof(CancelGump));

            if (IsChildOf(from.Backpack))
            {
                if (pm.NextEnhanceSuccess)
                {
                    from.SendGump(new CancelGump(pm, this));
                }
                else
                {
                    from.SendGump(new InternalGump(pm, this));
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public void Use(PlayerMobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.NextEnhanceSuccess = true;
                from.SendLocalizedMessage(1149956); // A magical aura surrounds you and you feel your next item enhancing attempt will most certainly be successful.
                this.m_UsesRemaining -= 1;
                InvalidateProperties();
                if (this.m_UsesRemaining <= 0)
                {
                    this.Delete();
                    from.SendLocalizedMessage(1044038); // You have worn out your tool!
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public class InternalGump : Gump
        {
            private PlayerMobile m_Mobile;
            private ForgedMetalOfArtifacts m_Tool;

            public InternalGump(PlayerMobile from, ForgedMetalOfArtifacts tool)
                : base(50, 50)
            {
                m_Mobile = from;
                m_Tool = tool;

                AddPage(0);

                AddBackground(0, 0, 400, 230, 0x13BE);

                AddImageTiled(5, 5, 390, 25, 0xA40);
                AddHtmlLocalized(5, 8, 390, 25, 1113302, "#1149868", 0x7FFF, false, false); // <CENTER>Forged Metal of Artifacts</CENTER>

                AddImageTiled(5, 35, 390, 125, 0xA40);

                /* 
				 * The next time that you try to enhance an item the success rate will be the value listed
				 * below regardless of your skill and total intensity of the item. Skill requirements still
				 * apply to material usage.  One charge on this item will be consumed per use and only one
				 * instance of this buff can be active at any time.
				 */
                AddHtmlLocalized(10, 40, 380, 125, 1149920, 0x7FFF, false, false);

                AddImageTiled(5, 165, 120, 30, 0xA40);
                AddHtmlLocalized(10, 170, 120, 30, 1149921, 0x7FFF, false, false); // Skill:
                AddImageTiled(125, 165, 140, 30, 0xA40);
                AddHtmlLocalized(130, 170, 140, 30, 1114057, "#1149992", 0x7FFF, false, false); // All Crafting Skills

                AddImageTiled(5, 195, 120, 30, 0xA40);
                AddHtmlLocalized(10, 200, 120, 30, 1149933, 0x7FFF, false, false); // Success Rate:
                AddImageTiled(125, 195, 140, 30, 0xA40);
                AddHtmlLocalized(130, 200, 140, 30, 1149934, "100", 0x7FFF, false, false); // ~1_VAL~%

                AddImageTiled(270, 165, 125, 60, 0xA40);
                AddButton(275, 170, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(310, 172, 85, 30, 1149935, 0x7FFF, false, false); // OKAY
                AddButton(275, 200, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(310, 202, 85, 30, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 1)
                {
                    m_Tool.Use(m_Mobile);
                }
            }
        }

        public class CancelGump : Gump
        {
            private PlayerMobile m_Mobile;
            private ForgedMetalOfArtifacts m_Tool;

            public CancelGump(PlayerMobile from, ForgedMetalOfArtifacts tool)
                : base(50, 50)
            {
                m_Mobile = from;
                m_Tool = tool;

                AddPage(0);

                AddBackground(0, 0, 400, 155, 0x13BE);
                AddImageTiled(5, 5, 390, 25, 0xA40);
                AddHtmlLocalized(5, 8, 390, 25, 1113302, "#1149868", 0x7FFF, false, false); // <CENTER>~1_VAL~</CENTER>
                AddImageTiled(5, 35, 390, 52, 0xA40);
                AddHtmlLocalized(10, 40, 380, 52, 1149967, 0x7FFF, false, false); // *CAUTION* You are under the following ENHANCE ITEM buff. Do you want to remove it?
                AddImageTiled(5, 92, 120, 29, 0xA40);
                AddHtmlLocalized(10, 97, 120, 29, 1149921, 0x7FFF, false, false); // Skill:
                AddImageTiled(5, 121, 120, 29, 0xA40);
                AddHtmlLocalized(10, 126, 120, 29, 1149933, 0x7FFF, false, false); // Success Rate:
                AddImageTiled(125, 92, 140, 29, 0xA40);
                AddHtmlLocalized(130, 97, 140, 29, 1114057, "#1149992", 0x7FFF, false, false); // ~1_val~
                AddImageTiled(125, 121, 140, 29, 0xA40);
                AddHtmlLocalized(130, 126, 140, 29, 1149934, "100", 0x7FFF, false, false); // ~1_VAL~%
                AddImageTiled(270, 92, 125, 58, 0xA40);
                AddButton(275, 97, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(310, 99, 85, 29, 1149968, 0x7FFF, false, false); // REMOVE
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 1)
                {
                    if (m_Tool == null || m_Tool.Deleted)
                    {
                        m_Mobile.SendLocalizedMessage(1155766); // A charge could not be refunded to the last Forged Metal of Artifacts tool you used, you must use this charge.
                    }
                    else if (!m_Tool.IsChildOf(m_Mobile.Backpack))
                    {
                        m_Mobile.SendLocalizedMessage(1155767); // A charge could not be refunded to the last Forged Metal of Artifacts tool you used because the item is not in your backpack.
                    }
                    else
                    {
                        m_Mobile.NextEnhanceSuccess = false;
                        m_Mobile.SendLocalizedMessage(1149969); // The magical aura that surrounded you disipates and you feel that your item enhancement chances have returned to normal.

                        m_Tool.UsesRemaining += 1;
                        m_Mobile.SendLocalizedMessage(1155768); // A charged has been refunded to your Forged Metal of Artifacts tool.
                    }
                }
            }
        }
    }
}