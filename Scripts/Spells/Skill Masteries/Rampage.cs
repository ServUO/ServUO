using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

// Check: Wrestling, poisoning and parry masteries.  In but not tested.

/*The wrestler attempts to continually hit their opponent where with each successful hit the wrestler receives a 
  bonus to hit point regeneration, stamina regeneration, casting focus, and swing speed increase based on mastery level.  
  The effect is lost if the wrestler misses, the wrestler's opponent parries the attack, or fails to cast a spell.*/

namespace Server.Spells.SkillMasteries
{
    public class RampageSpell : SkillMasterySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
                "Rampage", "",
                -1,
                9002
            );

        public override int RequiredMana => 20;
        public override int DamageThreshold => 0;
        public override bool CheckManaBeforeCast => !HasSpell(Caster, GetType());

        public override SkillName CastSkill => SkillName.Wrestling;

        public RampageSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            BaseWeapon wep = GetWeapon();

            if (Caster.Player && (wep == null || !(wep is Fists)))
            {
                Caster.SendLocalizedMessage(1155979); // You may not wield a weapon and use this ability.
                return false;
            }

            return base.CheckCast();
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            if (!HasSpell(Caster, GetType()))
            {
                Caster.PrivateOverheadMessage(MessageType.Regular, 1150, 1155890, Caster.NetState); // *You attempt channel your wrestling mastery into a fit of rage!*

                if (Caster.Player)
                {
                    Caster.PlaySound(Caster.Female ? 0x338 : 0x44A);
                }
                else if (Caster is BaseCreature)
                {
                    Caster.PlaySound(((BaseCreature)Caster).GetAngerSound());
                }
            }
        }

        public override void OnCast()
        {
            RampageSpell spell = GetSpell(Caster, typeof(RampageSpell)) as RampageSpell;

            if (spell != null)
            {
                spell.Expire();
            }
            else if (CheckSequence())
            {
                Effects.SendTargetParticles(Caster, 0x37CC, 1, 40, 2724, 5, 9907, EffectLayer.LeftFoot, 0);
                Caster.PlaySound(0x101);

                TimeSpan duration = TimeSpan.FromSeconds(60);

                Expires = DateTime.UtcNow + duration;
                BeginTimer();

                AddToTable();
            }

            FinishSequence();
        }

        public void AddToTable()
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, RampageContext>();

            RampageContext c;

            _Table[Caster] = c = new RampageContext(this);

            BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Rampage, 1155929, 1155893, TimeSpan.FromSeconds(60), Caster,
                string.Format("{0}\t{1}\t{1}\t{1}\t{2}\t{3}\t{4}\t{5}",
                (1 + GetMasteryLevel()).ToString(),
                GetMasteryLevel().ToString(),
                c.HitsRegen.ToString(),
                c.StamRegen.ToString(),
                c.SwingSpeed.ToString(),
                c.Focus.ToString()
                )));
            //Each successful hit or offensive spell grants:<br>+~1_VAL~ Hit Point Regeneration<br>+~2_VAL~ Stamina Regeneration<br>+~3_VAL~ Swing Speed Increase.<br>+~4_VAL~ Casting Focus.<br>(Once you miss your target this buff will end)<br>Current totals:<br>+~5_VAL~ Hit Point Regeneration<br>+~6_VAL~ Stamina Regeneration<br>+~7_VAL~ Swing Speed Increase.<br>+~8_VAL~ Casting Focus.<br>
        }

        public override void EndEffects()
        {
            RemoveFromTable(Caster);
        }

        public override void OnGotParried(Mobile defender)
        {
            RemoveFromTable(Caster);
            Expire();
        }

        public override void OnMiss(Mobile defender)
        {
            RemoveFromTable(Caster);
            Expire();
        }

        public override void OnHit(Mobile defender, ref int damage)
        {
            if (_Table != null && _Table.ContainsKey(Caster))
            {
                Caster.PlaySound(0x3B4);

                RampageContext c = _Table[Caster];
                c.IncreaseBuffs();

                BuffInfo.RemoveBuff(Caster, BuffIcon.Rampage);
                BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.Rampage, 1155929, 1155893, TimeSpan.FromSeconds(60), Caster,
                    string.Format("{0}\t{1}\t{1}\t{1}\t{2}\t{3}\t{4}\t{5}",
                    (1 + GetMasteryLevel()).ToString(),
                    GetMasteryLevel().ToString(),
                    c.HitsRegen.ToString(),
                    c.StamRegen.ToString(),
                    c.SwingSpeed.ToString(),
                    c.Focus.ToString()
                    )));
            }
        }

        public static bool RemoveFromTable(Mobile m)
        {
            bool removed = false;

            if (_Table != null && _Table.ContainsKey(m))
            {
                removed = true;
                _Table.Remove(m);

                BuffInfo.RemoveBuff(m, BuffIcon.Rampage);
            }

            if (_Table != null && _Table.Count == 0)
                _Table = null;

            return removed;
        }

        public static int GetBonus(Mobile m, BonusType type)
        {
            if (_Table == null || !_Table.ContainsKey(m))
                return 0;

            switch (type)
            {
                default:
                case BonusType.HitPointRegen: return _Table[m].HitsRegen;
                case BonusType.StamRegen: return _Table[m].StamRegen;
                case BonusType.Focus: return _Table[m].Focus;
                case BonusType.SwingSpeed: return _Table[m].SwingSpeed;
            }
        }

        private static Dictionary<Mobile, RampageContext> _Table;

        public enum BonusType
        {
            HitPointRegen,
            StamRegen,
            Focus,
            SwingSpeed
        }

        private class RampageContext
        {
            public RampageSpell Spell { get; set; }

            public int HitsRegen { get; set; }
            public int StamRegen { get; set; }
            public int Focus { get; set; }
            public int SwingSpeed { get; set; }

            private const int _HitsMax = 18;
            private const int _StamMax = 24;
            private const int _SwingMax = 60;
            private const int _FocusMax = 12;

            public RampageContext(RampageSpell spell)
            {
                Spell = spell;
            }

            public void IncreaseBuffs()
            {
                HitsRegen = Math.Min(_HitsMax, HitsRegen + (1 + Spell.GetMasteryLevel()));
                StamRegen = Math.Min(_StamMax, StamRegen + Spell.GetMasteryLevel());
                Focus = Math.Min(_FocusMax, Focus + Spell.GetMasteryLevel());
                SwingSpeed = Math.Min(_SwingMax, SwingSpeed + Spell.GetMasteryLevel());
            }
        }
    }
}
