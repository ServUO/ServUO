#region References
using Server.Items;
using Server.Spells;
using Server.Spells.Ninjitsu;
using System;
using System.Collections.Generic;
#endregion

namespace Server.Mobiles
{
    public class NinjaAI : MeleeAI
    {
        private DateTime m_NextCastTime;
        private DateTime m_NextRanged;

        public NinjaAI(BaseCreature bc)
            : base(bc)
        {
            m_NextCastTime = DateTime.UtcNow;
        }

        private void TryPerformHide()
        {
            if (!m_Mobile.Alive || m_Mobile.Deleted)
                return;

            if (!m_Mobile.Hidden && Core.TickCount - m_Mobile.NextSkillTime >= 0)
            {
                double chance = 0.05;

                if (m_Mobile.Hits < 20)
                    chance = 0.10;

                if (m_Mobile.Poisoned)
                    chance = 0.01;

                if (Utility.RandomDouble() < chance)
                    HideSelf();
            }
        }

        private void HideSelf()
        {
            Effects.SendLocationParticles(
                EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration),
                0x3728,
                10,
                10,
                2023);

            m_Mobile.PlaySound(0x22F);
            m_Mobile.Hidden = true;

            m_Mobile.UseSkill(SkillName.Stealth);
        }

        public virtual SpecialMove GetHiddenSpecialMove()
        {
            int skill = (int)m_Mobile.Skills[SkillName.Ninjitsu].Value;

            if (skill < 40)
                return null;

            if (skill >= 60)
            {
                //return .5 > Utility.RandomDouble() ? new SupriseAttack() : new Backstab();
                return .5 > Utility.RandomDouble() ? SpellRegistry.GetSpecialMove(504) : SpellRegistry.GetSpecialMove(505);
            }

            return SpellRegistry.GetSpecialMove(505); //new Backstab();
        }

        public virtual SpecialMove GetSpecialMove()
        {
            int skill = (int)m_Mobile.Skills[SkillName.Ninjitsu].Value;

            if (skill < 40)
                return null;

            int avail = 1;

            if (skill >= 85)
                avail = 3;
            else if (skill >= 80)
                avail = 2;

            switch (Utility.Random(avail))
            {
                case 0:
                    return SpellRegistry.GetSpecialMove(500); //new FocusAttack();
                case 1:
                    return SpellRegistry.GetSpecialMove(503); //new KiAttack();
                case 2:
                    return SpellRegistry.GetSpecialMove(501); //new DeathStrike();
            }

            return null;
        }

        public void DoRangedAttack()
        {
            Mobile c = m_Mobile.Combatant as Mobile;

            if (c == null)
                return;

            List<INinjaWeapon> list = new List<INinjaWeapon>();
            int d = (int)m_Mobile.GetDistanceToSqrt(c.Location);

            foreach (Item item in m_Mobile.Items)
                if (item is INinjaWeapon && ((INinjaWeapon)item).UsesRemaining > 0 && d >= ((INinjaWeapon)item).WeaponMinRange &&
                    d <= ((INinjaWeapon)item).WeaponMaxRange)
                    list.Add(item as INinjaWeapon);

            if (m_Mobile.Backpack != null)
            {
                foreach (Item item in m_Mobile.Backpack.Items)
                    if (item is INinjaWeapon && ((INinjaWeapon)item).UsesRemaining > 0 && d >= ((INinjaWeapon)item).WeaponMinRange &&
                        d <= ((INinjaWeapon)item).WeaponMaxRange)
                        list.Add(item as INinjaWeapon);
            }

            if (list.Count > 0)
            {
                INinjaWeapon toUse = list[Utility.Random(list.Count)];

                if (toUse != null)
                    NinjaWeapon.Shoot(m_Mobile, c, toUse);
            }

            ColUtility.Free(list);

            m_NextRanged = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 120));
        }

        public override bool DoActionWander()
        {
            base.DoActionWander();

            if (m_Mobile.Combatant == null)
                TryPerformHide();

            return true;
        }

        public override bool DoActionCombat()
        {
            base.DoActionCombat();

            if (m_Mobile.Combatant is Mobile c)
            {
                if (!m_Mobile.Controlled && !m_Mobile.Summoned && m_Mobile.CheckCanFlee())
                {
                    m_Mobile.DebugSay("I am going to flee from {0}", c.Name);
                    Action = ActionType.Flee;
                }
                else
                {
                    SpecialMove special = SpecialMove.GetCurrentMove(m_Mobile);

                    if (special == null && m_NextCastTime < DateTime.UtcNow && 0.05 > Utility.RandomDouble())
                    {
                        if (0.05 > Utility.RandomDouble())
                        {
                            new MirrorImage(m_Mobile, null).Cast();
                        }
                        else
                        {
                            if (m_Mobile.Hidden)
                            {
                                special = GetHiddenSpecialMove();
                            }
                            else
                            {
                                special = GetSpecialMove();
                            }

                            if (special != null)
                            {
                                SpecialMove.SetCurrentMove(m_Mobile, special);
                                m_NextCastTime = DateTime.UtcNow + GetCastDelay();
                            }
                        }
                    }

                    if (m_NextRanged < DateTime.UtcNow && 0.08 > Utility.RandomDouble())
                    {
                        DoRangedAttack();
                    }
                }
            }

            return true;
        }

        public override bool DoActionFlee()
        {
            base.DoActionFlee();
            TryPerformHide();
            return true;
        }

        public TimeSpan GetCastDelay()
        {
            int skill = (int)m_Mobile.Skills[SkillName.Ninjitsu].Value;

            if (skill >= 85)
                return TimeSpan.FromSeconds(15);
            if (skill > 40)
                return TimeSpan.FromSeconds(30);

            return TimeSpan.FromSeconds(45);
        }
    }
}
