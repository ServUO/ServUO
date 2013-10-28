using System;
using Server.Targeting;

namespace Server.Commands
{
    public class SkillsCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("SetSkill", AccessLevel.GameMaster, new CommandEventHandler(SetSkill_OnCommand));
            CommandSystem.Register("GetSkill", AccessLevel.GameMaster, new CommandEventHandler(GetSkill_OnCommand));
            CommandSystem.Register("SetAllSkills", AccessLevel.GameMaster, new CommandEventHandler(SetAllSkills_OnCommand));
        }

        [Usage("SetSkill <name> <value>")]
        [Description("Sets a skill value by name of a targeted mobile.")]
        public static void SetSkill_OnCommand(CommandEventArgs arg)
        {
            if (arg.Length != 2)
            {
                arg.Mobile.SendMessage("SetSkill <skill name> <value>");
            }
            else
            {
                SkillName skill;

                if (Enum.TryParse(arg.GetString(0), true, out skill))
                {
                    arg.Mobile.Target = new SkillTarget(skill, arg.GetDouble(1));
                }
                else
                {
                    arg.Mobile.SendLocalizedMessage(1005631); // You have specified an invalid skill to set.
                }
            }
        }

        [Usage("SetAllSkills <name> <value>")]
        [Description("Sets all skill values of a targeted mobile.")]
        public static void SetAllSkills_OnCommand(CommandEventArgs arg)
        {
            if (arg.Length != 1)
            {
                arg.Mobile.SendMessage("SetAllSkills <value>");
            }
            else
            {
                arg.Mobile.Target = new AllSkillsTarget(arg.GetDouble(0));
            }
        }

        [Usage("GetSkill <name>")]
        [Description("Gets a skill value by name of a targeted mobile.")]
        public static void GetSkill_OnCommand(CommandEventArgs arg)
        {
            if (arg.Length != 1)
            {
                arg.Mobile.SendMessage("GetSkill <skill name>");
            }
            else
            {
                SkillName skill;

                if (Enum.TryParse(arg.GetString(0), true, out skill))
                {
                    arg.Mobile.Target = new SkillTarget(skill);
                }
                else
                {
                    arg.Mobile.SendMessage("You have specified an invalid skill to get.");
                }
            }
        }

        public class AllSkillsTarget : Target
        {
            private readonly double m_Value;
            public AllSkillsTarget(double value)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Value = value;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;
                    Server.Skills skills = targ.Skills;

                    for (int i = 0; i < skills.Length; ++i)
                        skills[i].Base = this.m_Value;

                    CommandLogging.LogChangeProperty(from, targ, "EverySkill.Base", this.m_Value.ToString());
                }
                else
                {
                    from.SendMessage("That does not have skills!");
                }
            }
        }

        public class SkillTarget : Target
        {
            private readonly bool m_Set;
            private readonly SkillName m_Skill;
            private readonly double m_Value;
            public SkillTarget(SkillName skill, double value)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Set = true;
                this.m_Skill = skill;
                this.m_Value = value;
            }

            public SkillTarget(SkillName skill)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Set = false;
                this.m_Skill = skill;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;
                    Skill skill = targ.Skills[this.m_Skill];

                    if (skill == null)
                        return;

                    if (this.m_Set)
                    {
                        skill.Base = this.m_Value;
                        CommandLogging.LogChangeProperty(from, targ, String.Format("{0}.Base", this.m_Skill), this.m_Value.ToString());
                    }

                    from.SendMessage("{0} : {1} (Base: {2})", this.m_Skill, skill.Value, skill.Base);
                }
                else
                {
                    from.SendMessage("That does not have skills!");
                }
            }
        }
    }
}