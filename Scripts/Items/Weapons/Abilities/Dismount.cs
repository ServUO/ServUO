using System;
using Server.Mobiles;

namespace Server.Items
{
    /// <summary>
    /// Perfect for the foot-soldier, the Dismount special attack can unseat a mounted opponent.
    /// The fighter using this ability must be on his own two feet and not in the saddle of a steed
    /// (with one exception: players may use a lance to dismount other players while mounted).
    /// If it works, the target will be knocked off his own mount and will take some extra damage from the fall!
    /// </summary>
    public class Dismount : WeaponAbility
    {
        public static readonly TimeSpan DefenderRemountDelay = TimeSpan.FromSeconds(10.0);// TODO: Taken from bola script, needs to be verified
        public static readonly TimeSpan AttackerRemountDelay = TimeSpan.FromSeconds(3.0);
        public Dismount()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }
        public override bool Validate(Mobile from)
        {
            if (!base.Validate(from))
                return false;

            if (from.Mounted && !(from.Weapon is Lance))
            {
                from.SendLocalizedMessage(1061283); // You cannot perform that attack while mounted!
                return false;
            }

            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker))
                return;

            if (defender is ChaosDragoon || defender is ChaosDragoonElite)
                return;

            if (attacker.Mounted && !(defender.Weapon is Lance)) // TODO: Should there be a message here?
                return;

            ClearCurrentAbility(attacker);

            IMount mount = defender.Mount;

            if (mount == null)
            {
                attacker.SendLocalizedMessage(1060848); // This attack only works on mounted targets
                return;
            }

            if (!this.CheckMana(attacker, true))
                return;

            if (Core.ML && attacker is LesserHiryu && 0.8 >= Utility.RandomDouble())
            {
                return; //Lesser Hiryu have an 80% chance of missing this attack
            }

            attacker.SendLocalizedMessage(1060082); // The force of your attack has dislodged them from their mount!

            if (attacker.Mounted)
                defender.SendLocalizedMessage(1062315); // You fall off your mount!
            else
                defender.SendLocalizedMessage(1060083); // You fall off of your mount and take damage!

            defender.PlaySound(0x140);
            defender.FixedParticles(0x3728, 10, 15, 9955, EffectLayer.Waist);

            mount.Rider = null;

            BaseMount.SetMountPrevention(defender, BlockMountType.Dazed, DefenderRemountDelay);
            if (Core.ML && attacker is BaseCreature && ((BaseCreature)attacker).ControlMaster != null)
            {
                BaseMount.SetMountPrevention(((BaseCreature)attacker).ControlMaster, BlockMountType.DismountRecovery, AttackerRemountDelay);
            }
            else
            {
                BaseMount.SetMountPrevention(attacker, BlockMountType.DismountRecovery, AttackerRemountDelay);
            }
				
            if (!attacker.Mounted)
                AOS.Damage(defender, attacker, Utility.RandomMinMax(15, 25), 100, 0, 0, 0, 0);
        }
    }
}