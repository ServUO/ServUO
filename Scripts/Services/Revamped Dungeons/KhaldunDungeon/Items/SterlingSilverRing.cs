using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class SterlingSilverRing : SilverRing
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1155606; } } // Sterling Silver Ring

        [Constructable]
        public SterlingSilverRing()
            : base()
        {
            Hue = 2500;

            SkillBonuses.SetValues(1, SkillName.Meditation, 20);
            Attributes.RegenHits = 3;
            Attributes.RegenMana = 5;
            Attributes.WeaponDamage = 75;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack) && SkillBonuses.GetBonus(2) == 0)
            {
                m.SendGump(new InternalGump(m, this));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (SkillBonuses.GetBonus(2) == 0)
            {
                list.Add(1155609); // Double Click to Set Skill Bonus
            }
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public SterlingSilverRing(Serial serial)
            : base(serial)
        {
        }

        public class InternalGump : Gump
        {
            private Mobile m_Mobile;
            private SterlingSilverRing m_Ring;

            public InternalGump(Mobile mobile, SterlingSilverRing ring)
                : base(20, 20)
            {
                mobile.CloseGump(typeof(SterlingSilverRing.InternalGump));

                m_Mobile = mobile;
                m_Ring = ring;

                int font = 0x7FFF;

                AddBackground(0, 0, 170, 210, 9270);
                AddAlphaRegion(10, 10, 150, 190);

                AddHtmlLocalized(20, 22, 150, 16, 1155610, font, false, false); //Please Choose A Skill

                AddButton(20, 50, 9702, 9703, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 50, 200, 16, 1044091, font, false, false); //Archery

                AddButton(20, 75, 9702, 9703, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 75, 200, 16, 1044102, font, false, false); //Fencing

                AddButton(20, 100, 9702, 9703, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 100, 200, 16, 1044101, font, false, false); //Mace Fighting

                AddButton(20, 125, 9702, 9703, 4, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 125, 200, 16, 1044100, font, false, false); //Swordsmanship

                AddButton(20, 150, 9702, 9703, 5, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 150, 200, 16, 1044117, font, false, false); //Throwing

                AddButton(20, 175, 9702, 9703, 6, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 175, 200, 16, 1044103, font, false, false); //Wrestling
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0)
                    return;

                SkillName skill;

                switch (info.ButtonID)
                {
                    default:
                    case 1: skill = SkillName.Archery; break;
                    case 2: skill = SkillName.Fencing; break;
                    case 3: skill = SkillName.Macing; break;
                    case 4: skill = SkillName.Swords; break;
                    case 5: skill = SkillName.Throwing; break;
                    case 6: skill = SkillName.Wrestling; break;
                }

                m_Mobile.SendGump(new ConfirmCallbackGump((PlayerMobile)m_Mobile, m_Mobile.Skills[skill].Info.Name, 1155611, null, confirm: (pm, state) =>
                {
                    if (m_Ring != null && m_Ring.IsChildOf(pm.Backpack) && !m_Ring.Deleted && m_Ring.SkillBonuses.GetBonus(2) == 0)
                    {
                        m_Ring.SkillBonuses.SetValues(2, skill, 20.0);

                        pm.SendLocalizedMessage(1155612); // A skill bonus has been applied to the item!
                    }
                }));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
