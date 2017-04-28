using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells.Ninjitsu;

/*The paladin unleashes a flying fist against a target that does energy damage based on the paladin's chivalry 
 * skill, best weapon skill, and mastery level.  A bonus to damage is provided by high karma as well against undead 
 * targets.*/

namespace Server.Spells.SkillMasteries
{
    public class HolyFistSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Holy Fist", "Kal Vas Grav",
                -1,
                9002
            );

        public override double RequiredSkill { get { return 90; } }
        public override int RequiredMana { get { return 40; } }

        public override SkillName CastSkill { get { return SkillName.Chivalry; } }
        public override SkillName DamageSkill { get { return SkillName.Chivalry; } }

        public int RequiredTithing { get { return 100; } }

        public override bool DelayedDamage { get { return true; } }

        public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds(2.5); } }

        public HolyFistSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            Caster.FixedEffect(0x37C4, 87, (int)(GetCastDelay().TotalSeconds * 28), 4, 3);
        }

        public override bool CheckCast()
        {
            if (Caster is PlayerMobile && Caster.TithingPoints < RequiredTithing)
            {
                Caster.SendLocalizedMessage(1060173, RequiredTithing.ToString()); // You must have at least ~1_TITHE_REQUIREMENT~ Tithing Points to use this ability,
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            Caster.Target = new MasteryTarget(this);
        }

        protected override void OnTarget(object o)
        {
            Mobile m = o as Mobile;

            if (m != null)
            {
                if (CheckHSequence(m))
                {
                    double skill = (Caster.Skills[CastSkill].Value + GetWeaponSkill() + (GetMasteryLevel() * 40)) / 3;
                    double damage = skill + (double)Caster.Karma / 1000;

                    if (m is BaseCreature && IsUndead((BaseCreature)m))
                        damage *= 1.5;
                    else if (m is PlayerMobile)
                        damage /= 1.5;

                    damage += Utility.RandomMinMax(0, 5);

                    Caster.MovingParticles(m, 0x9BB5, 7, 0, false, true, 9502, 4019, 0x160);
                    Caster.PlaySound(0x5CE);

                    SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);

                    if (!CheckResisted(m) && m.NetState != null)
                    {
                        if (!TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm)))
                            m.Send(SpeedControl.WalkSpeed);

                        Server.Timer.DelayCall(TimeSpan.FromSeconds(skill / 60), () =>
                            {
                                if (!TransformationSpellHelper.UnderTransformation(m, typeof(AnimalForm)) &&
                                    !TransformationSpellHelper.UnderTransformation(m, typeof(Server.Spells.Spellweaving.ReaperFormSpell)))
                                    m.Send(SpeedControl.Disable);
                            });
                    }
                }
            }
        }

        public override bool CheckSequence()
        {
            int requiredTithing = this.RequiredTithing;

            if (Caster is PlayerMobile && Caster.TithingPoints < requiredTithing)
            {
                Caster.SendLocalizedMessage(1060173, RequiredTithing.ToString()); // You must have at least ~1_TITHE_REQUIREMENT~ Tithing Points to use this ability,
                return false;
            }

            if (AosAttributes.GetValue(Caster, AosAttribute.LowerRegCost) > Utility.Random(100))
                requiredTithing = 0;

            if(requiredTithing > 0 && Caster is PlayerMobile)
                Caster.TithingPoints -= requiredTithing;

            return base.CheckSequence();
        }

        private bool IsUndead(BaseCreature bc)
        {
            SlayerEntry entry = SlayerGroup.GetEntryByName(SlayerName.Silver);

            return entry != null && entry.Slays(bc);
        }
    }
}