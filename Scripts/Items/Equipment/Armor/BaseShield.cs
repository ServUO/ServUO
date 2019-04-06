using System;
using Server.Network;

namespace Server.Items
{
    public class BaseShield : BaseArmor
    {
        public BaseShield(int itemID)
            : base(itemID)
        {
        }

        public BaseShield(Serial serial)
            : base(serial)
        {
        }

        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Plate;
            }
        }
        public override double ArmorRating
        {
            get
            {
                Mobile m = this.Parent as Mobile;
                double ar = base.ArmorRating;

                if (m != null)
                    return ((m.Skills[SkillName.Parry].Value * ar) / 200.0) + 1.0;
                else
                    return ar;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                if (this is Aegis)
                    return;

                // The 15 bonus points to resistances are not applied to shields on OSI.
                this.PhysicalBonus = 0;
                this.FireBonus = 0;
                this.ColdBonus = 0;
                this.PoisonBonus = 0;
                this.EnergyBonus = 0;
            }
        }

        public override int OnHit(BaseWeapon weapon, int damage)
        {
            if (Core.AOS)
            {
                if (this.ArmorAttributes.SelfRepair > Utility.Random(10))
                {
                    this.HitPoints += 2;
                }
                else
                {
                    double halfArmor = this.ArmorRating / 2.0;
                    int absorbed = (int)(halfArmor + (halfArmor * Utility.RandomDouble()));

                    if (absorbed < 2)
                        absorbed = 2;

                    int wear;

                    if (weapon.Type == WeaponType.Bashing)
                        wear = (absorbed / 2);
                    else
                        wear = Utility.Random(2);

                    if (wear > 0 && this.MaxHitPoints > 0)
                    {
                        if (this.HitPoints >= wear)
                        {
                            this.HitPoints -= wear;
                            wear = 0;
                        }
                        else
                        {
                            wear -= this.HitPoints;
                            this.HitPoints = 0;
                        }

                        if (wear > 0)
                        {
                            if (this.MaxHitPoints > wear)
                            {
                                this.MaxHitPoints -= wear;

                                if (this.Parent is Mobile)
                                    ((Mobile)this.Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                            }
                            else
                            {
                                this.Delete();
                            }
                        }
                    }
                }

                return 0;
            }
            else
            {
                Mobile owner = this.Parent as Mobile;
                if (owner == null)
                    return damage;

                double ar = this.ArmorRating;
                double chance = (owner.Skills[SkillName.Parry].Value - (ar * 2.0)) / 100.0;

                if (chance < 0.01)
                    chance = 0.01;
                /*
                FORMULA: Displayed AR = ((Parrying Skill * Base AR of Shield) ÷ 200) + 1 

                FORMULA: % Chance of Blocking = parry skill - (shieldAR * 2)

                FORMULA: Melee Damage Absorbed = (AR of Shield) / 2 | Archery Damage Absorbed = AR of Shield 
                */
                if (owner.CheckSkill(SkillName.Parry, chance))
                {
                    if (weapon.Skill == SkillName.Archery)
                        damage -= (int)ar;
                    else
                        damage -= (int)(ar / 2.0);

                    if (damage < 0)
                        damage = 0;

                    owner.FixedEffect(0x37B9, 10, 16);

                    if (25 > Utility.Random(100)) // 25% chance to lower durability
                    {
                        int wear = Utility.Random(2);

                        if (wear > 0 && this.MaxHitPoints > 0)
                        {
                            if (this.HitPoints >= wear)
                            {
                                this.HitPoints -= wear;
                                wear = 0;
                            }
                            else
                            {
                                wear -= this.HitPoints;
                                this.HitPoints = 0;
                            }

                            if (wear > 0)
                            {
                                if (this.MaxHitPoints > wear)
                                {
                                    this.MaxHitPoints -= wear;

                                    if (this.Parent is Mobile)
                                        ((Mobile)this.Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                                }
                                else
                                {
                                    this.Delete();
                                }
                            }
                        }
                    }
                }

                return damage;
            }
        }

        public override int GetLuckBonus()
        {
            if (CraftResources.GetType(Resource) != CraftResourceType.Wood)
            {
                return base.GetLuckBonus();
            }
            else
            {
                CraftAttributeInfo attrInfo = GetResourceAttrs(Resource);

                if (attrInfo == null)
                    return 0;

                return attrInfo.ShieldLuck;
            }
        }

        public override void DistributeExceptionalBonuses(Mobile from, int amount)
        {
        }

        public override void DistributeMaterialBonus(CraftAttributeInfo attrInfo)
        {
            if (CraftResources.GetType(Resource) != CraftResourceType.Wood)
            {
                base.DistributeMaterialBonus(attrInfo);
            }
            else
            {
                if (Resource != CraftResource.Heartwood)
                {
                    Attributes.SpellChanneling += attrInfo.ShieldSpellChanneling;
                    ArmorAttributes.LowerStatReq += attrInfo.ShieldLowerRequirements;
                    Attributes.RegenHits += attrInfo.ShieldRegenHits;
                }
                else
                {
                    switch (Utility.Random(7))
                    {
                        case 0: Attributes.BonusDex += attrInfo.ShieldBonusDex; break;
                        case 1: Attributes.BonusStr += attrInfo.ShieldBonusStr; break;
                        case 2: PhysicalBonus += attrInfo.ShieldPhysicalRandom; break;
                        case 3: Attributes.ReflectPhysical += attrInfo.ShieldReflectPhys; break;
                        case 4: ArmorAttributes.SelfRepair += attrInfo.ShieldSelfRepair; break;
                        case 5: ColdBonus += attrInfo.ShieldColdRandom; break;
                        case 6: Attributes.SpellChanneling += attrInfo.ShieldSpellChanneling; break;
                    }
                }
            }
        }

        protected override void ApplyResourceResistances(CraftResource oldResource)
        {
            if (CraftResources.GetType(Resource) != CraftResourceType.Wood)
            {
                base.ApplyResourceResistances(oldResource);
            }
            else
            {
                CraftAttributeInfo info;

                if (oldResource > CraftResource.None)
                {
                    info = GetResourceAttrs(oldResource);
                    // Remove old bonus

                    PhysicalBonus = Math.Max(0, PhysicalBonus - info.ShieldPhysicalResist);
                    FireBonus = Math.Max(0, FireBonus - info.ShieldFireResist);
                    ColdBonus = Math.Max(0, ColdBonus - info.ShieldColdResist);
                    PoisonBonus = Math.Max(0, PoisonBonus - info.ShieldPoisonResist);
                    EnergyBonus = Math.Max(0, EnergyBonus - info.ShieldEnergyResist);

                    PhysNonImbuing = Math.Max(0, PhysNonImbuing - info.ShieldPhysicalResist);
                    FireNonImbuing = Math.Max(0, FireNonImbuing - info.ShieldFireResist);
                    ColdNonImbuing = Math.Max(0, ColdNonImbuing - info.ShieldColdResist);
                    PoisonNonImbuing = Math.Max(0, PoisonNonImbuing - info.ShieldPoisonResist);
                    EnergyNonImbuing = Math.Max(0, EnergyNonImbuing - info.ShieldEnergyResist);
                }

                info = GetResourceAttrs(Resource);

                // add new bonus
                PhysicalBonus += info.ShieldPhysicalResist;
                FireBonus += info.ShieldFireResist;
                ColdBonus += info.ShieldColdResist;
                PoisonBonus += info.ShieldPoisonResist;
                EnergyBonus += info.ShieldEnergyResist;

                PhysNonImbuing += info.ShieldPhysicalResist;
                FireNonImbuing += info.ShieldFireResist;
                ColdNonImbuing += info.ShieldColdResist;
                PoisonNonImbuing += info.ShieldPoisonResist;
                EnergyNonImbuing += info.ShieldEnergyResist;
            }
        }
    }
}