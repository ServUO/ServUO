using Server.Mobiles;
using System;

namespace Server.Items
{
    /// <summary>
    /// If you are on foot, dismounts your opponent and damage the ethereal's rider or the
    /// living mount(which must be healed before ridden again). If you are mounted, damages
    /// and stuns the mounted opponent.
    /// </summary>
    public class RidingSwipe : WeaponAbility
    {
        public override int BaseMana => 25;

        public override bool CheckSkills(Mobile from)
        {
            if (GetSkill(from, SkillName.Bushido) < 50.0)
            {
                from.SendLocalizedMessage(1070768, "50"); // You need ~1_SKILL_REQUIREMENT~ Bushido skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!defender.Mounted && !defender.Flying && !Spells.Ninjitsu.AnimalForm.UnderTransformation(defender))
            {
                attacker.SendLocalizedMessage(1060848); // This attack only works on mounted targets
                ClearCurrentAbility(attacker);
                return;
            }

            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            int amount = 10 + (int)(10.0 * (attacker.Skills[SkillName.Bushido].Value - 50.0) / 70.0 + 5);

            if (!attacker.Mounted)
            {
                BlockMountType type = BlockMountType.RidingSwipe;
                IMount mount = defender.Mount;

                if (defender.Flying)
                {
                    type = BlockMountType.RidingSwipeFlying;
                }
                else if (mount is EtherealMount)
                {
                    type = BlockMountType.RidingSwipeEthereal;
                }

                Items.Dismount.DoDismount(attacker, defender, mount, 10, type);

                if (mount is Mobile)
                    AOS.Damage((Mobile)mount, attacker, amount, 100, 0, 0, 0, 0);

                defender.PlaySound(0x140);
                defender.FixedParticles(0x3728, 10, 15, 9955, EffectLayer.Waist);
            }
            else
            {
                AOS.Damage(defender, attacker, amount, 100, 0, 0, 0, 0);

                if (Items.ParalyzingBlow.IsImmune(defender))	//Does it still do damage?
                {
                    attacker.SendLocalizedMessage(1070804); // Your target resists paralysis.
                    defender.SendLocalizedMessage(1070813); // You resist paralysis.
                }
                else
                {
                    defender.Paralyze(TimeSpan.FromSeconds(3.0));
                    Items.ParalyzingBlow.BeginImmunity(defender, Items.ParalyzingBlow.FreezeDelayDuration);
                }
            }
        }
    }
}
