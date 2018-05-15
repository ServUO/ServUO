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
            IDamageable m = o as IDamageable;

            if (m != null)
            {
                if (CheckHSequence(m))
                {
                    IDamageable target = m;
                    IDamageable source = Caster;

                    SpellHelper.Turn(Caster, target);

                    if (SpellHelper.CheckReflect(0, ref source, ref target))
                    {
                        Server.Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                        {
                            source.MovingParticles(target, 0x9BB5, 7, 0, false, true, 9502, 4019, 0x160);
                            source.PlaySound(0x5CE);
                        });
                    }

                    double skill = (Caster.Skills[CastSkill].Value + GetWeaponSkill() + (GetMasteryLevel() * 40)) / 3;
                    double damage = skill + (double)Caster.Karma / 1000;

                    damage += Utility.RandomMinMax(0, 5);

                    if (m is BaseCreature && IsUndead((BaseCreature)m))
                        damage *= 1.5;
                    else if (m is PlayerMobile)
                        damage = Math.Min(35, damage);

                    Caster.MovingParticles(m, 0x9BB5, 7, 0, false, true, 9502, 4019, 0x160);
                    Caster.PlaySound(0x5CE);

                    SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 100);

                    if (target is Mobile && !CheckResisted((Mobile)target) && ((Mobile)target).NetState != null)
                    {
                        Mobile mob = target as Mobile;

                        if (!TransformationSpellHelper.UnderTransformation(mob, typeof(AnimalForm)))
                            mob.SendSpeedControl(SpeedControlType.WalkSpeed);

                        Server.Timer.DelayCall(TimeSpan.FromSeconds(skill / 60), () =>
                            {
                                if (!TransformationSpellHelper.UnderTransformation(mob, typeof(AnimalForm)) &&
                                    !TransformationSpellHelper.UnderTransformation(mob, typeof(Server.Spells.Spellweaving.ReaperFormSpell)))
                                    mob.SendSpeedControl(SpeedControlType.Disable);
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