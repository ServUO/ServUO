using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class CrystalBallOfKnowledge : Item
    {
        private static SkillName[] _ExcludedSkills =
        {
            SkillName.Meditation, SkillName.Focus
        };

        public override int LabelNumber { get { return 1112568; } } // Crystal Ball of Knowledge

        private bool m_Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                if (m_Active != value)
                {
                    m_Active = value;
                    InvalidateProperties();
                }
            }
        }

        [Constructable]
        public CrystalBallOfKnowledge()
            : base(0xE2E)
        {
            Weight = 10.0;
            Light = LightType.Circle150;
            LootType = LootType.Blessed;
        }

        public CrystalBallOfKnowledge(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                // You must have the object in your backpack to use it.
                from.SendLocalizedMessage(1042010);
            }
            /*else if ( ??? ) // TODO (SA)
			{
				// You cannot use the Crystal Ball of Knowledge right now.
				from.SendLocalizedMessage( 1112569 );
			}*/
            else if (!from.HasGump(typeof(ToggleActivationGump)))
            {
                from.SendGump(new ToggleActivationGump(this));
            }
        }

        public static bool IsAllowed(SkillName skill)
        {
            return !_ExcludedSkills.Any(sk => sk == skill);
        }

        public static bool HasActiveBall(Mobile from)
        {
            if (from.Backpack == null)
                return false;

            CrystalBallOfKnowledge[] balls = from.Backpack.FindItemsByType<CrystalBallOfKnowledge>().ToArray();

            for (int i = 0; i < balls.Length; i++)
                if (balls[i].Active)
                    return true;

            return false;
        }

        public static void TellSkillDifficultyActive(Mobile from, SkillName skill, double chance)
        {
            if (HasActiveBall(from))
            {
                GiveSkillDifficulty(from, skill, chance);
            }
        }

        public static void TellSkillDifficulty(Mobile from, SkillName skill, double chance)
        {
            if (HasActiveBall(from) && IsAllowed(skill))
            {
                GiveSkillDifficulty(from, skill, chance);
            }
        }

        public static void GiveSkillDifficulty(Mobile from, SkillName skill, double chance)
        {
            Utility.FixMinMax(ref chance, 0.0, 1.0);

            int number;

            if (chance == 0.0)
                number = 1078457; // ~1_skillname~ Difficulty: Too Challenging
            else if (chance <= 0.1)
                number = 1078458; // ~1_skillname~ Difficulty: Very Challenging
            else if (chance <= 0.25)
                number = 1078459; // ~1_skillname~ Difficulty: Challenging
            else if (chance <= 0.75)
                number = 1078460; // ~1_skillname~ Difficulty: Optimal
            else if (chance <= 0.9)
                number = 1078461; // ~1_skillname~ Difficulty: Easy
            else if (chance <= 1.0)
                number = 1078462; // ~1_skillname~ Difficulty: Very Easy
            else
                number = 1078463; // ~1_skillname~ Difficulty: Too Easy

            from.SendLocalizedMessage(number, SkillInfo.Table[(int)skill].Name);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Active)
                list.Add(502695); // turned on
            else
                list.Add(502696); // turned off
        }

        public override bool OnDragLift(Mobile from)
        {
            if (!base.OnDragLift(from))
                return false;

            Active = false;

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((bool)m_Active);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Active = reader.ReadBool();
        }

        public class ToggleActivationGump : Gump
        {
            private CrystalBallOfKnowledge m_Ball;

            public ToggleActivationGump(CrystalBallOfKnowledge ball)
                : base(150, 200)
            {
                m_Ball = ball;

                AddPage(0);

                AddBackground(0, 0, 300, 150, 0xA28);

                if (m_Ball.Active)
                    AddHtmlLocalized(45, 20, 300, 35, 1011035, false, false); // Deactivate this item
                else
                    AddHtmlLocalized(45, 20, 300, 35, 1011034, false, false); // Activate this item

                AddButton(40, 53, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(80, 55, 65, 35, 1011036, false, false); // OKAY

                AddButton(150, 53, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(190, 55, 100, 35, 1011012, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 2)
                {
                    if (!m_Ball.Deleted && m_Ball.IsChildOf(from.Backpack))
                    {
                        m_Ball.Active = !m_Ball.Active;

                        if (m_Ball.Active)
                            from.SendLocalizedMessage(1078486); // I will now tell you skill difficulty.
                        else
                            from.SendLocalizedMessage(1078485); // I will no longer tell you skill difficulty.
                    }
                }
            }
        }
    }
}