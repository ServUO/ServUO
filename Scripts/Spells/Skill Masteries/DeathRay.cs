using Server.Mobiles;
using System;

namespace Server.Spells.SkillMasteries
{
    public class DeathRaySpell : SkillMasterySpell
    {
        /*The mage focuses a death ray on their opponent which snares the mage to their 
         * location and does damage based on magery skill, evaluating intelligence skill,
         * and mastery level as long as the mage has mana and the target is in range.*/

        // BuffIcon: 1155798 ~1_STR~ Energy Resist.<br>~2_DAM~ energy damage every 3 seconds while death ray remains in effect.<br>

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Death Ray", "In Grav Corp",
                204,
                9061,
                Reagent.BlackPearl,
                Reagent.Bloodmoss,
                Reagent.SpidersSilk
            );

        private Point3D _Location;
        private ResistanceMod _Mod;

        public override double UpKeep => 35;
        public override int RequiredMana => 50;
        public override int DamageThreshold => 0;
        public override bool DamageCanDisrupt => true;
        public override double TickTime => 3;

        public override int UpkeepCancelMessage => 1155874;  // You do not have enough mana to keep your death ray active.
        public override int DisruptMessage => 1155793;  // This action disturbs the focus necessary to keep your death ray active and it dissipates.

        public override TimeSpan ExpirationPeriod => TimeSpan.FromMinutes(360);

        public override SkillName CastSkill => SkillName.Magery;
        public override SkillName DamageSkill => SkillName.EvalInt;

        public DeathRaySpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
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
                if (GetSpell<DeathRaySpell>(Caster, m) != null)
                {
                    Caster.SendLocalizedMessage(1156094); // Your target is already under the effect of this ability.
                }
                else if (CheckHSequence(m))
                {
                    if (CheckResisted(m))
                    {
                        m.SendLocalizedMessage(1156101); // You resist the effects of death ray.
                        Caster.SendLocalizedMessage(1156102); // Your target resists the effects of death ray.
                    }
                    else
                    {
                        SpellHelper.CheckReflect(this, Caster, ref m);
                        _Location = Caster.Location;

                        m.FixedParticles(0x374A, 1, 15, 5054, 0x7A2, 7, EffectLayer.Head);
                        Caster.FixedParticles(0x0000, 10, 5, 2054, EffectLayer.Head);

                        double damage = (Caster.Skills[CastSkill].Base + Caster.Skills[DamageSkill].Base) * (GetMasteryLevel() * .8);
                        damage /= Target is PlayerMobile ? 5.15 : 2.5;

                        int mod = (int)Caster.Skills[DamageSkill].Value / 12;
                        _Mod = new ResistanceMod(ResistanceType.Energy, -mod);
                        m.AddResistanceMod(_Mod);

                        BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.DeathRay, 1155896, 1156085, string.Format("{0}\t{1}", ((int)damage).ToString(), m.Name))); // Deals ~2_DAMAGE~ to ~1_NAME~ every 3 seconds while in range. Preforming any action will end spell.
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.DeathRayDebuff, 1155896, 1156086, mod.ToString())); // Energy Resist Debuff: ~1_VAL~%

                        Target = m;
                        BeginTimer();
                    }
                }
            }
        }

        public override void EndEffects()
        {
            if (Target != null && _Mod != null)
                Target.RemoveResistanceMod(_Mod);

            BuffInfo.RemoveBuff(Caster, BuffIcon.DeathRay);

            if (Target != null)
                BuffInfo.RemoveBuff(Target, BuffIcon.DeathRayDebuff);
        }

        public override bool OnTick()
        {
            if (!base.OnTick())
                return false;

            if (Target == Caster || !Target.Alive)
            {
                Expire();
                Caster.SendLocalizedMessage(1156097); // Your ability was interrupted.
            }
            else if (Caster.Location != _Location)
            {
                Expire(true);
                return false;
            }
            else
            {
                double damage = (Caster.Skills[CastSkill].Base + Caster.Skills[DamageSkill].Base) * (GetMasteryLevel() * .8);
                damage /= Target is PlayerMobile ? 5.15 : 2.5;

                damage *= GetDamageScalar(Target);

                int sdiBonus = SpellHelper.GetSpellDamageBonus(Caster, Target, CastSkill, Caster.Player && Target.Player);

                damage *= (100 + sdiBonus);
                damage /= 100;

                SpellHelper.Damage(this, Target, (int)damage, 0, 0, 0, 0, 100);
            }

            return true;
        }
    }
}
