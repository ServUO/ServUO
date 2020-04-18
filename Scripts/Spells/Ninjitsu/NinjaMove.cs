namespace Server.Spells
{
    public class NinjaMove : SpecialMove
    {
        public override SkillName MoveSkill => SkillName.Ninjitsu;
        public override void CheckGain(Mobile m)
        {
            m.CheckSkill(MoveSkill, RequiredSkill - 12.5, RequiredSkill + 37.5);	//Per five on friday 02/16/07
        }
    }
}