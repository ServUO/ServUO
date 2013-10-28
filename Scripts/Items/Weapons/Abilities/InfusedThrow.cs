using System;
using Server.Mobiles;

namespace Server.Items
{
    // The projectile will be infused with energy by the attacker causing it to do more damage and stun or dismount the target.
    // The player infuses their throwing projectile with mystical power. 
    // The infused projectile will dismount the target if possible; otherwise it will temporarily stun the target. 
    // The target will be hit with chaos damage regardless of whether they were dismounted or paralyzed.
    public class InfusedThrow : WeaponAbility
    {
        public override int BaseMana
        {
            get
            {
                return 15;
            }
        }
        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            IMount mount = defender.Mount;

            if (mount != null && !(defender is ChaosDragoon || defender is ChaosDragoonElite))
            {
                defender.SendLocalizedMessage(1062315); // You fall off your mount!

                defender.PlaySound(0x140);
                defender.FixedParticles(0x3728, 10, 15, 9955, EffectLayer.Waist);

                mount.Rider = null;

                BaseMount.SetMountPrevention(defender, BlockMountType.Dazed, TimeSpan.FromSeconds(10.0));

                if (Core.ML && attacker is BaseCreature && ((BaseCreature)attacker).ControlMaster != null)
                    BaseMount.SetMountPrevention(((BaseCreature)attacker).ControlMaster, BlockMountType.DismountRecovery, TimeSpan.FromSeconds(3.0));
                else
                    BaseMount.SetMountPrevention(attacker, BlockMountType.DismountRecovery, TimeSpan.FromSeconds(3.0));
            }
            else
            {
                //if ( WeaponAbility.ParalyzingBlow.IsImmune( defender ) )
                //{
                //attacker.SendLocalizedMessage( 1070804 ); // Your target resists paralysis.
                //defender.SendLocalizedMessage( 1070813 ); // You resist paralysis.
                //}
                //else
                //{
                defender.FixedEffect(0x376A, 9, 32);
                defender.PlaySound(0x204);
                attacker.SendLocalizedMessage(1060163); // You deliver a paralyzing blow!
                defender.SendLocalizedMessage(1060164); // The attack has temporarily paralyzed you!
                TimeSpan duration = defender.Player ? TimeSpan.FromSeconds(3.0) : TimeSpan.FromSeconds(6.0);
                defender.Paralyze(duration);
                //WeaponAbility.ParalyzingBlow.BeginImmunity( defender, duration + TimeSpan.FromSeconds( 8.0 ) );
                //}
            }

            int amount = 15;

            switch( Utility.Random(5) )
            {
                case 0:
                    AOS.Damage(defender, attacker, amount, 100, 0, 0, 0, 0);
                    break;
                case 1:
                    AOS.Damage(defender, attacker, amount, 0, 100, 0, 0, 0);
                    break;
                case 2:
                    AOS.Damage(defender, attacker, amount, 0, 0, 100, 0, 0);
                    break;
                case 3:
                    AOS.Damage(defender, attacker, amount, 0, 0, 0, 100, 0);
                    break;
                case 4:
                    AOS.Damage(defender, attacker, amount, 0, 0, 0, 0, 100);
                    break;
            }
        }
    }
}