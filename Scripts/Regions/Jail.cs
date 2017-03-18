using System;
using System.Xml;

namespace Server.Regions
{
    public class Jail : BaseRegion
    {
        public Jail(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override bool AllowBeneficial(Mobile from, Mobile target)
        {
            if (from.IsPlayer())
                from.SendMessage("You may not do that in jail.");

            return (from.IsStaff());
        }

        public override bool AllowHarmful(Mobile from, IDamageable target)
        {
            if (from.Player)
                from.SendMessage("You may not do that in jail.");

            return (from.IsStaff());
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            global = LightCycle.JailLevel;
        }

        public override bool OnBeginSpellCast(Mobile from, ISpell s)
        {
            if (from.IsPlayer())
                from.SendLocalizedMessage(502629); // You cannot cast spells here.

            return (from.IsStaff());
        }

        public override bool OnSkillUse(Mobile from, int Skill)
        {
            if (from.IsPlayer())
                from.SendMessage("You may not use skills in jail.");

            return (from.IsStaff());
        }

        public override bool OnCombatantChange(Mobile from, IDamageable Old, IDamageable New)
        {
            return (from.IsStaff());
        }
    }
}