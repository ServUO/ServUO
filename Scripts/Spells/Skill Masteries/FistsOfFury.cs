using Server.Items;
using System;
using System.Collections.Generic;

/*The wrestler attempts to land three hits in rapid succession to the next target that damages you within a 2 tile radius. 
  If successful the third hit will deal direct damage based on the wrestler's mastery level. The duration of this ability 
  is based on wrestling skill and anatomy skill or evaluating intelligence skill.*/

// Add OnCasterDamaged virutal in SkillMagerySpell and point to it in CheckDisruption(...) method
namespace Server.Spells.SkillMasteries
{
    public class FistsOfFurySpell : SkillMasteryMove
    {
        public override int BaseMana => 20;
        public override double RequiredSkill => 90.0;

        public override SkillName MoveSkill => SkillName.Wrestling;
        public override TextDefinition AbilityMessage => new TextDefinition(1155895);  // You ready yourself to unleash your fists of fury!

        public override TimeSpan CooldownPeriod => TimeSpan.FromSeconds(20);

        private Dictionary<Mobile, FistsOfFuryContext> _Table;

        public override bool Validate(Mobile from)
        {
            if (!CheckWeapon(from))
            {
                from.SendLocalizedMessage(1155979); // You may not wield a weapon and use this ability.
                return false;
            }

            bool validate = base.Validate(from);

            if (!validate)
                return false;

            return CheckMana(from, true);
        }

        public override void OnUse(Mobile from)
        {
            SkillName damageSkill = from.Skills[SkillName.Anatomy].Value > from.Skills[SkillName.EvalInt].Value ? SkillName.Anatomy : SkillName.EvalInt;
            double duration = (from.Skills[MoveSkill].Value + from.Skills[damageSkill].Value) / 24;

            AddToCooldown(from);

            Timer.DelayCall(TimeSpan.FromMilliseconds(400), () =>
                {
                    Effects.SendTargetParticles(from, 0x376A, 1, 40, 2724, 5, 9907, EffectLayer.Waist, 0);
                    Effects.SendTargetParticles(from, 0x37CC, 1, 40, 2724, 5, 9907, EffectLayer.Waist, 0);
                    from.PlaySound(0x101);
                    from.PlaySound(0x056D);

                    Timer.DelayCall(TimeSpan.FromSeconds(duration), () => Expire(from));

                    BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.FistsOfFury, 1155930, 1156256, TimeSpan.FromSeconds(duration), from));
                    // You prepare yourself to attempt to land three hits in rapid succession to the next target that damages you within a 2 tile radius.
                });
        }

        public override void OnDamaged(Mobile attacker, Mobile defender, DamageType type, ref int damage)
        {
            if (defender == null || attacker == null)
                return;

            BaseWeapon wep = defender.Weapon as BaseWeapon;

            if (wep == null || !(wep is Fists))
            {
                defender.SendLocalizedMessage(1155979); // You may not wield a weapon and use this ability.
                Expire(defender);
            }
            else if (defender.InRange(attacker.Location, 2) && !UnderEffects(defender))
            {
                if (_Table == null)
                    _Table = new Dictionary<Mobile, FistsOfFuryContext>();

                _Table[defender] = new FistsOfFuryContext(attacker);
                defender.NextCombatTime = Core.TickCount + (int)(wep.GetDelay(defender).TotalMilliseconds);

                for (int i = 0; i < 3; i++)
                {
                    wep.OnSwing(defender, attacker);
                }

                if (_Table[defender].Hit > 0)
                {
                    attacker.FixedParticles(0x36BD, 20, 10, 5044, 2724, 0, EffectLayer.Head, 0);
                    attacker.PlaySound(0x3B3);
                }

                _Table.Remove(defender);

                if (_Table.Count == 0)
                    _Table = null;
            }
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!UnderEffects(attacker))
                return;

            if (_Table[attacker].Target == defender)
            {
                _Table[attacker].Hit++;

                if (_Table[attacker].Hit >= 3)
                {
                    int level = MasteryInfo.GetMasteryLevel(attacker, MoveSkill);
                    int newDam = damage + Utility.RandomMinMax(level, level * 2);

                    AOS.Damage(defender, attacker, Utility.RandomMinMax(level + 1, (level * 7) - 1), 0, 0, 0, 0, 0, 0, 100);
                    damage = 0;
                }
            }
        }

        public void Expire(Mobile from)
        {
            ClearCurrentMove(from);
        }

        public override void OnClearMove(Mobile from)
        {
            BuffInfo.RemoveBuff(from, BuffIcon.FistsOfFury);
        }

        private bool UnderEffects(Mobile from)
        {
            return _Table != null && _Table.ContainsKey(from);
        }

        public class FistsOfFuryContext
        {
            public Mobile Target { get; set; }
            public int Hit { get; set; }

            public FistsOfFuryContext(Mobile target)
            {
                Target = target;
                Hit = 0;
            }
        }
    }
}
