using System.Xml;

namespace Server.Regions
{
    public class Jail : BaseRegion
    {
        public Jail(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override bool AllowAutoClaim(Mobile from)
        {
            return false;
        }

        public override bool AllowBeneficial(Mobile from, Mobile target)
        {
            if (from.IsPlayer())
                from.SendLocalizedMessage(1115999); // You may not do that in this area.

            return (from.IsStaff());
        }

        public override bool AllowHarmful(Mobile from, IDamageable target)
        {
            if (from.Player)
                from.SendLocalizedMessage(1115999); // You may not do that in this area.

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
            {
                from.SendLocalizedMessage(502629); // You cannot cast spells here.
                return false;
            }

            return base.OnBeginSpellCast(from, s);
        }

        public override bool OnSkillUse(Mobile from, int Skill)
        {
            if (from.IsPlayer())
                from.SendLocalizedMessage(1116000); // You may not use that skill in this area.

            return (from.IsStaff());
        }

        public override bool OnCombatantChange(Mobile from, IDamageable Old, IDamageable New)
        {
            return (from.IsStaff());
        }
    }
}