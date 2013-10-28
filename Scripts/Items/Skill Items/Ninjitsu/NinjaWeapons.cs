using System;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Targeting;

/*
* There really was no prettier way to do this,  other than the one
* suggestion to make a rigged baseninjaweapon class that bypasses its
* own serialization, due to the way these weapons were originaly coded.
*/
namespace Server.Items
{
    public interface INinjaAmmo : IUsesRemaining
    {
        int PoisonCharges { get; set; }
        Poison Poison { get; set; }
    }

    public interface INinjaWeapon : IUsesRemaining
    {
        int NoFreeHandMessage { get; }
        int EmptyWeaponMessage { get; }
        int RecentlyUsedMessage { get; }
        int FullWeaponMessage { get; }
        int WrongAmmoMessage { get; }
        Type AmmoType { get; }
        int PoisonCharges { get; set; }
        Poison Poison { get; set; }
        int WeaponDamage { get; }
        int WeaponMinRange { get; }
        int WeaponMaxRange { get; }
        void AttackAnimation(Mobile from, Mobile to);
    }

    public class NinjaWeapon
    {
        private const int MaxUses = 10;
        public static void AttemptShoot(PlayerMobile from, INinjaWeapon weapon)
        {
            if (CanUseWeapon(from, weapon))
            {
                from.BeginTarget(weapon.WeaponMaxRange, false, TargetFlags.Harmful, new TargetStateCallback<INinjaWeapon>(OnTarget), weapon);
            }
        }

        private static void Shoot(PlayerMobile from, Mobile target, INinjaWeapon weapon)
        {
            if (from != target && CanUseWeapon(from, weapon) && from.CanBeHarmful(target))
            {
                if (weapon.WeaponMinRange == 0 || !from.InRange(target, weapon.WeaponMinRange))
                {
                    from.NinjaWepCooldown = true;

                    from.Direction = from.GetDirectionTo(target);

                    from.RevealingAction();

                    weapon.AttackAnimation(from, target);

                    ConsumeUse(weapon);

                    if (CombatCheck(from, target))
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback<object[]>(OnHit), new object[] { from, target, weapon });
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(2.5), new TimerStateCallback<PlayerMobile>(ResetUsing), from);
                }
                else
                {
                    from.SendLocalizedMessage(1063303); // Your target is too close!
                }
            }
        }

        private static void ResetUsing(PlayerMobile from)
        {
            from.NinjaWepCooldown = false;
        }

        private static void Unload(Mobile from, INinjaWeapon weapon)
        {
            if (weapon.UsesRemaining > 0)
            {
                INinjaAmmo ammo = Activator.CreateInstance(weapon.AmmoType, new object[] { weapon.UsesRemaining }) as INinjaAmmo;

                ammo.Poison = weapon.Poison;
                ammo.PoisonCharges = weapon.PoisonCharges;

                from.AddToBackpack((Item)ammo);

                weapon.UsesRemaining = 0;
                weapon.PoisonCharges = 0;
                weapon.Poison = null;
            }
        }

        private static void Reload(PlayerMobile from, INinjaWeapon weapon, INinjaAmmo ammo)
        {
            if (weapon.UsesRemaining < MaxUses)
            {
                int need = Math.Min((MaxUses - weapon.UsesRemaining), ammo.UsesRemaining);

                if (need > 0)
                {
                    if (weapon.Poison != null && (ammo.Poison == null || weapon.Poison.Level > ammo.Poison.Level))
                    {
                        from.SendLocalizedMessage(1070767); // Loaded projectile is stronger, unload it first
                    }
                    else
                    {
                        if (weapon.UsesRemaining > 0)
                        {
                            if ((weapon.Poison == null && ammo.Poison != null) ||
                                ((weapon.Poison != null && ammo.Poison != null) && weapon.Poison.Level != ammo.Poison.Level))
                            {
                                Unload(from, weapon);
                                need = Math.Min(MaxUses, ammo.UsesRemaining);
                            }
                        }
                        int poisonneeded = Math.Min((MaxUses - weapon.PoisonCharges), ammo.PoisonCharges);

                        weapon.UsesRemaining += need;
                        weapon.PoisonCharges += poisonneeded;

                        if (weapon.PoisonCharges > 0)
                        {
                            weapon.Poison = ammo.Poison;
                        }

                        ammo.PoisonCharges -= poisonneeded;
                        ammo.UsesRemaining -= need;

                        if (ammo.UsesRemaining < 1)
                        {
                            ((Item)ammo).Delete();
                        }
                        else if (ammo.PoisonCharges < 1)
                        {
                            ammo.Poison = null;
                        }
                    }
                }// "else" here would mean they targeted "ammo" with 0 uses.  undefined behavior.
            }
            else
            {
                from.SendLocalizedMessage(weapon.FullWeaponMessage);
            }
        }

        private static void ConsumeUse(INinjaWeapon weapon)
        {
            if (weapon.UsesRemaining > 0)
            {
                weapon.UsesRemaining--;

                if (weapon.UsesRemaining < 1)
                {
                    weapon.PoisonCharges = 0;
                    weapon.Poison = null;
                }
            }
        }

        private static bool CanUseWeapon(PlayerMobile from, INinjaWeapon weapon)
        {
            if (WeaponIsValid(weapon, from))
            {
                if (weapon.UsesRemaining > 0)
                {
                    if (!from.NinjaWepCooldown)
                    {
                        if (BasePotion.HasFreeHand(from))
                        {
                            return true;
                        }
                        else
                        {
                            from.SendLocalizedMessage(weapon.NoFreeHandMessage);
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(weapon.RecentlyUsedMessage);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(weapon.EmptyWeaponMessage);
                }
            }
            return false;
        }

        private static bool CombatCheck(Mobile attacker, Mobile defender) /* mod'd from baseweapon */
        {
            BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

            Skill atkSkill = defender.Skills.Ninjitsu;
            Skill defSkill = defender.Skills[defWeapon.Skill];

            double atSkillValue = attacker.Skills.Ninjitsu.Value;
            double defSkillValue = defWeapon.GetDefendSkillValue(attacker, defender);

            double attackValue = AosAttributes.GetValue(attacker, AosAttribute.AttackChance);

            if (defSkillValue <= -20.0)
            {
                defSkillValue = -19.9;
            }

            if (Spells.Chivalry.DivineFurySpell.UnderEffect(attacker))
            {
                attackValue += 10;
            }

            if (AnimalForm.UnderTransformation(attacker, typeof(GreyWolf)) || AnimalForm.UnderTransformation(attacker, typeof(BakeKitsune)))
            {
                attackValue += 20;
            }

            if (HitLower.IsUnderAttackEffect(attacker))
            {
                attackValue -= 25;
            }

            if (attackValue > 45)
            {
                attackValue = 45;
            }

            attackValue = (atSkillValue + 20.0) * (100 + attackValue);

            double defenseValue = AosAttributes.GetValue(defender, AosAttribute.DefendChance);

            if (Spells.Chivalry.DivineFurySpell.UnderEffect(defender))
            {
                defenseValue -= 20;
            }

            if (HitLower.IsUnderDefenseEffect(defender))
            {
                defenseValue -= 25;
            }

            int refBonus = 0;

            if (Block.GetBonus(defender, ref refBonus))
            {
                defenseValue += refBonus;
            }

            if (SkillHandlers.Discordance.GetEffect(attacker, ref refBonus))
            {
                defenseValue -= refBonus;
            }

            if (defenseValue > 45)
            {
                defenseValue = 45;
            }

            defenseValue = (defSkillValue + 20.0) * (100 + defenseValue);

            double chance = attackValue / (defenseValue * 2.0);

            if (chance < 0.02)
            {
                chance = 0.02;
            }

            return attacker.CheckSkill(atkSkill.SkillName, chance);
        }

        private static void OnHit(object[] states)
        {
            Mobile from = states[0] as Mobile;
            Mobile target = states[1] as Mobile;
            INinjaWeapon weapon = states[2] as INinjaWeapon;

            if (from.CanBeHarmful(target))
            {
                from.DoHarmful(target);

                AOS.Damage(target, from, weapon.WeaponDamage, 100, 0, 0, 0, 0);

                if (weapon.Poison != null && weapon.PoisonCharges > 0)
                {
                    if (EvilOmenSpell.TryEndEffect(target))
                    {
                        target.ApplyPoison(from, Poison.GetPoison(weapon.Poison.Level + 1));
                    }
                    else
                    {
                        target.ApplyPoison(from, weapon.Poison);
                    }

                    weapon.PoisonCharges--;

                    if (weapon.PoisonCharges < 1)
                    {
                        weapon.Poison = null;
                    }
                }
            }
        }

        private static void OnTarget(Mobile from, object targeted, INinjaWeapon weapon)
        {
            PlayerMobile player = from as PlayerMobile;

            if (WeaponIsValid(weapon, from))
            {
                if (targeted is Mobile)
                {
                    Shoot(player, (Mobile)targeted, weapon);
                }
                else if (targeted.GetType() == weapon.AmmoType)
                {
                    Reload(player, weapon, (INinjaAmmo)targeted);
                }
                else
                {
                    player.SendLocalizedMessage(weapon.WrongAmmoMessage);
                }
            }
        }

        private static bool WeaponIsValid(INinjaWeapon weapon, Mobile from)
        {
            Item item = weapon as Item;

            if (!item.Deleted && item.RootParent == from)
            {
                return true;
            }
            return false;
        }

        public class LoadEntry : ContextMenuEntry
        {
            private readonly INinjaWeapon weapon;
            public LoadEntry(INinjaWeapon wep, int entry)
                : base(entry, 0)
            {
                this.weapon = wep;
            }

            public override void OnClick()
            {
                if (WeaponIsValid(this.weapon, this.Owner.From))
                {
                    this.Owner.From.BeginTarget(10, false, TargetFlags.Harmful, new TargetStateCallback<INinjaWeapon>(OnTarget), this.weapon);
                }
            }
        }

        public class UnloadEntry : ContextMenuEntry
        {
            private readonly INinjaWeapon weapon;
            public UnloadEntry(INinjaWeapon wep, int entry)
                : base(entry, 0)
            {
                this.weapon = wep;

                this.Enabled = (this.weapon.UsesRemaining > 0);
            }

            public override void OnClick()
            {
                if (WeaponIsValid(this.weapon, this.Owner.From))
                {
                    Unload(this.Owner.From, this.weapon);
                }
            }
        }
    }
}