using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// If you are on foot, dismounts your opponent and damage the ethereal's rider or the
    /// living mount(which must be healed before ridden again). If you are mounted, damages
    /// and stuns the mounted opponent.
    /// </summary>
    public class RidingSwipe : WeaponAbility
    {
        public RidingSwipe()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 25;
            }
        }
        public override bool RequiresSE
        {
            get
            {
                return true;
            }
        }
        public override bool CheckSkills(Mobile from)
        {
            if (this.GetSkill(from, SkillName.Bushido) < 50.0)
            {
                from.SendLocalizedMessage(1070768, "50"); // You need ~1_SKILL_REQUIREMENT~ Bushido skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!defender.Mounted && !defender.Flying && (!Core.ML || !Server.Spells.Ninjitsu.AnimalForm.UnderTransformation(defender)))
            {
                attacker.SendLocalizedMessage(1060848); // This attack only works on mounted targets
                ClearCurrentAbility(attacker);
                return;
            }

            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            int amount = 10 + (int)(10.0 * (attacker.Skills[SkillName.Bushido].Value - 50.0) / 70.0 + 5);

            if (!attacker.Mounted)
            {
                IMount mount = defender.Mount;

                if (mount != null || defender.Flying || (Core.ML && Server.Spells.Ninjitsu.AnimalForm.UnderTransformation(defender)))
                {
                    BaseMount.Dismount(defender);

                    if (mount is Mobile)
                    {
                        AOS.Damage((Mobile)mount, attacker, amount, 100, 0, 0, 0, 0);

                        if (!m_MountPrevented.Contains((Mobile)mount))
                            m_MountPrevented.Add((Mobile)mount);
                    }
                    else
                    {
                        AOS.Damage(defender, attacker, amount, 100, 0, 0, 0, 0);
                    }

                    attacker.SendLocalizedMessage(1060082); // The force of your attack has dislodged them from their mount!

                    defender.PlaySound(0x140);
                    defender.FixedParticles(0x3728, 10, 15, 9955, EffectLayer.Waist);
                }
            }
            else
            {
                AOS.Damage(defender, attacker, amount, 100, 0, 0, 0, 0);

                if (Server.Items.ParalyzingBlow.IsImmune(defender))	//Does it still do damage?
                {
                    attacker.SendLocalizedMessage(1070804); // Your target resists paralysis.
                    defender.SendLocalizedMessage(1070813); // You resist paralysis.
                }
                else
                {
                    defender.Paralyze(TimeSpan.FromSeconds(3.0));
                    Server.Items.ParalyzingBlow.BeginImmunity(defender, Server.Items.ParalyzingBlow.FreezeDelayDuration);
                }
            }
        }

        public static List<Mobile> MountPrevented { get { return m_MountPrevented; } }
        private static List<Mobile> m_MountPrevented = new List<Mobile>();

        public static bool CanMount(Mobile mount)
        {
            if (m_MountPrevented.Contains(mount))
            {
                if (mount.Hits == mount.HitsMax)
                {
                    m_MountPrevented.Remove(mount);
                    return true;
                }
                else
                    return false;
            }

            return true;
        }
    }
}