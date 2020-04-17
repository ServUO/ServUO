using Server.Engines.Quests;
using Server.Mobiles;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class BindingRope : Item
    {
        private Mobile m_BoundMobile;
        private ProfessionalBountyQuest m_Quest;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BoundMobile
        {
            get { return m_BoundMobile; }
            set { m_BoundMobile = value; }
        }

        public ProfessionalBountyQuest Quest
        {
            get { return m_Quest; }
            set { m_Quest = value; }
        }

        public override int LabelNumber => 1116717;

        public BindingRope(ProfessionalBountyQuest quest) : base(5368)
        {
            m_Quest = quest;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && m_BoundMobile == null)
            {
                from.Target = new InternalTarget(this);
                from.SendLocalizedMessage(1116720); //Who do you want to tie up?
            }
        }

        public void DoDelayedDelete(Mobile from)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(2.5), new TimerStateCallback(DoDelete), from);
        }

        public void DoDelete(object o)
        {
            Mobile from = (Mobile)o;

            if (from != null)
                from.SendMessage("Your crew uses the rope to bind the captain to the front of your galleon.");

            Delete();
        }

        private class InternalTarget : Target
        {
            private readonly BindingRope m_Rope;

            public InternalTarget(BindingRope rope) : base(2, false, TargetFlags.None)
            {
                m_Rope = rope;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    if (targeted is PirateCaptain)
                    {
                        PirateCaptain cap = (PirateCaptain)targeted;

                        if (cap.Hits > cap.HitsMax / 10)
                        {
                            from.SendLocalizedMessage(1116756); //The pirate seems to have too much fight left to be bound.
                        }
                        else if (cap.TryBound(from, m_Rope.Quest))
                        {
                            m_Rope.BoundMobile = cap;
                            m_Rope.Quest.OnBound(cap);
                            cap.OnBound(m_Rope.Quest);

                            from.SendLocalizedMessage(1116721); //You begin binding the pirate.

                            m_Rope.DoDelayedDelete(from);
                        }
                    }
                    else
                    {
                        from.SendMessage("They cannot by bound by that!");
                    }
                }
                else
                {
                    from.SendMessage("They cannot by bound by that!");
                }
            }
        }

        public BindingRope(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_BoundMobile);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_BoundMobile = reader.ReadMobile();
        }
    }
}