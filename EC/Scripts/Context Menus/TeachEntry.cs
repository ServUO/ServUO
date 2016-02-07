using System;
using Server.Mobiles;

namespace Server.ContextMenus
{
    public class TeachEntry : ContextMenuEntry
    {
        private readonly SkillName m_Skill;
        private readonly BaseCreature m_Mobile;
        private readonly Mobile m_From;
        public TeachEntry(SkillName skill, BaseCreature m, Mobile from, bool enabled)
            : base(6000 + (int)skill)
        {
            this.m_Skill = skill;
            this.m_Mobile = m;
            this.m_From = from;

            if (!enabled)
                this.Flags |= Network.CMEFlags.Disabled;
        }

        public override void OnClick()
        {
            if (!this.m_From.CheckAlive())
                return;

            this.m_Mobile.Teach(this.m_Skill, this.m_From, 0, false);
        }
    }
}