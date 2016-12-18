using System;

namespace Server.Items
{
    /// <summary>
    /// This attack allows you to disarm your foe.
    /// Now in Age of Shadows, a successful Disarm leaves the victim unable to re-arm another weapon for several seconds.
    /// </summary>
    public class Disarm : WeaponAbility
    {
        public static readonly TimeSpan BlockEquipDuration = TimeSpan.FromSeconds(5.0);
        public Disarm()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }
        // No longer active in pub21:

        /*public override bool CheckSkills( Mobile from )
        {
        if ( !base.CheckSkills( from ) )
        return false;

        if ( !(from.Weapon is Fists) )
        return true;

        Skill skill = from.Skills[SkillName.ArmsLore];

        if ( skill != null && skill.Base >= 80.0 )
        return true;

        from.SendLocalizedMessage( 1061812 ); // You lack the required skill in armslore to perform that attack!

        return false;
        }*/
        public override bool RequiresTactics(Mobile from)
        {
            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (weapon == null)
                return false;

            return weapon.Skill != SkillName.Wrestling;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker))
                return;

            ClearCurrentAbility(attacker);

            Item toDisarm = defender.FindItemOnLayer(Layer.OneHanded);

            if (toDisarm == null || !toDisarm.Movable)
                toDisarm = defender.FindItemOnLayer(Layer.TwoHanded);

            Container pack = defender.Backpack;

            if (pack == null || (toDisarm != null && !toDisarm.Movable))
            {
                attacker.SendLocalizedMessage(1004001); // You cannot disarm your opponent.
            }
            else if (toDisarm == null || toDisarm is BaseShield || toDisarm is Spellbook && !Core.ML)
            {
                attacker.SendLocalizedMessage(1060849); // Your target is already unarmed!
            }
            else if (this.CheckMana(attacker, true))
            {
                attacker.SendLocalizedMessage(1060092); // You disarm their weapon!
                defender.SendLocalizedMessage(1060093); // Your weapon has been disarmed!

                defender.PlaySound(0x3B9);
                defender.FixedParticles(0x37BE, 232, 25, 9948, EffectLayer.LeftHand);

                pack.DropItem(toDisarm);
                
                BuffInfo.AddBuff(defender, new BuffInfo( BuffIcon.NoRearm, 1075637, BlockEquipDuration, defender));

                BaseWeapon.BlockEquip(defender, BlockEquipDuration);
            }
        }
    }
}
