using System;

namespace Server.Spells
{
    public class NinjaMove : SpecialMove
    {
        public override SkillName MoveSkill
        {
            get
            {
                return SkillName.Ninjitsu;
            }
        }
        public override void CheckGain(Mobile m)
        {
            m.CheckSkill(this.MoveSkill, this.RequiredSkill - 12.5, this.RequiredSkill + 37.5);	//Per five on friday 02/16/07
        }
    }
}