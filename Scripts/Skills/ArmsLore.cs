using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;

namespace Server.SkillHandlers
{
    public class ArmsLore
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.ArmsLore].Callback = OnUse;
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.Target = new InternalTarget();

            m.SendLocalizedMessage(500349); // What item do you wish to get information about?

            return TimeSpan.FromSeconds(1.0);
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(2, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseWeapon)
                {
                    if (from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 100))
                    {
                        BaseWeapon weap = (BaseWeapon)targeted;

                        if (weap.MaxHitPoints != 0)
                        {
                            int hp = (int)((weap.HitPoints / (double)weap.MaxHitPoints) * 10);

                            if (hp < 0)
                                hp = 0;
                            else if (hp > 9)
                                hp = 9;

                            from.SendLocalizedMessage(1038285 + hp);
                        }

                        int damage = (weap.MaxDamage + weap.MinDamage) / 2;
                        int hand = (weap.Layer == Layer.OneHanded ? 0 : 1);

                        if (damage < 3)
                            damage = 0;
                        else
                            damage = (int)Math.Ceiling(Math.Min(damage, 30) / 5.0);

                        WeaponType type = weap.Type;

                        if (type == WeaponType.Ranged)
                            from.SendLocalizedMessage(1038224 + (damage * 9));
                        else if (type == WeaponType.Piercing)
                            from.SendLocalizedMessage(1038218 + hand + (damage * 9));
                        else if (type == WeaponType.Slashing)
                            from.SendLocalizedMessage(1038220 + hand + (damage * 9));
                        else if (type == WeaponType.Bashing)
                            from.SendLocalizedMessage(1038222 + hand + (damage * 9));
                        else
                            from.SendLocalizedMessage(1038216 + hand + (damage * 9));

                        if (weap.Poison != null && weap.PoisonCharges > 0)
                            from.SendLocalizedMessage(1038284); // It appears to have poison smeared on it.
                    }
                    else
                    {
                        from.SendLocalizedMessage(500353); // You are not certain...
                    }
                }
                else if (targeted is BaseArmor)
                {
                    if (from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 100))
                    {
                        BaseArmor arm = (BaseArmor)targeted;

                        if (arm.MaxHitPoints != 0)
                        {
                            int hp = (int)((arm.HitPoints / (double)arm.MaxHitPoints) * 10);

                            if (hp < 0)
                                hp = 0;
                            else if (hp > 9)
                                hp = 9;

                            from.SendLocalizedMessage(1038285 + hp);
                        }

                        from.SendLocalizedMessage(1038295 + (int)Math.Ceiling(Math.Min(arm.ArmorRating, 35) / 5.0));
                    }
                    else
                    {
                        from.SendLocalizedMessage(500353); // You are not certain...
                    }
                }
                else if (targeted is SwampDragon && ((SwampDragon)targeted).HasBarding)
                {
                    SwampDragon pet = (SwampDragon)targeted;

                    if (from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 100))
                    {
                        int perc = (4 * pet.BardingHP) / pet.BardingMaxHP;

                        if (perc < 0)
                            perc = 0;
                        else if (perc > 4)
                            perc = 4;

                        pet.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1053021 - perc, from.NetState);
                    }
                    else
                    {
                        from.SendLocalizedMessage(500353); // You are not certain...
                    }
                }
                else
                {
                    from.SendLocalizedMessage(500352); // This is neither weapon nor armor.
                }
            }
        }
    }
}
