using System;
using Server.Items;
using Server.Targeting;

namespace Server.Engines.Craft
{
    public enum EnhanceResult
    {
        None,
        NotInBackpack,
        BadItem,
        BadResource,
        AlreadyEnhanced,
        Success,
        Failure,
        Broken,
        NoResources,
        NoSkill
    }

    public class Enhance
    {
        public static EnhanceResult Invoke(Mobile from, CraftSystem craftSystem, BaseTool tool, Item item, CraftResource resource, Type resType, ref object resMessage)
        {
            if (item == null)
                return EnhanceResult.BadItem;

            if (!item.IsChildOf(from.Backpack))
                return EnhanceResult.NotInBackpack;

            if (!(item is BaseArmor) && !(item is BaseWeapon))
                return EnhanceResult.BadItem;

            if (item is IArcaneEquip)
            {
                IArcaneEquip eq = (IArcaneEquip)item;
                if (eq.IsArcane)
                    return EnhanceResult.BadItem;
            }

            if (CraftResources.IsStandard(resource))
                return EnhanceResult.BadResource;
			
            int num = craftSystem.CanCraft(from, tool, item.GetType());
			
            if (num > 0)
            {
                resMessage = num;
                return EnhanceResult.None;
            }

            CraftItem craftItem = craftSystem.CraftItems.SearchFor(item.GetType());

            if (craftItem == null || craftItem.Resources.Count == 0)
                return EnhanceResult.BadItem;

            #region Mondain's Legacy
            if (craftItem.ForceNonExceptional)
                return EnhanceResult.BadItem;
            #endregion

            bool allRequiredSkills = false;
            if (craftItem.GetSuccessChance(from, resType, craftSystem, false, ref allRequiredSkills) <= 0.0)
                return EnhanceResult.NoSkill;

            CraftResourceInfo info = CraftResources.GetInfo(resource);

            if (info == null || info.ResourceTypes.Length == 0)
                return EnhanceResult.BadResource;

            CraftAttributeInfo attributes = info.AttributeInfo;

            if (attributes == null)
                return EnhanceResult.BadResource;

            int resHue = 0, maxAmount = 0;

            if (!craftItem.ConsumeRes(from, resType, craftSystem, ref resHue, ref maxAmount, ConsumeType.None, ref resMessage))
                return EnhanceResult.NoResources;

            if (craftSystem is DefBlacksmithy)
            {
                AncientSmithyHammer hammer = from.FindItemOnLayer(Layer.OneHanded) as AncientSmithyHammer;
                if (hammer != null)
                {
                    hammer.UsesRemaining--;
                    if (hammer.UsesRemaining < 1)
                        hammer.Delete();
                }
            }

            int phys = 0, fire = 0, cold = 0, pois = 0, nrgy = 0;
            int dura = 0, luck = 0, lreq = 0, dinc = 0;
            int baseChance = 0;

            bool physBonus = false;
            bool fireBonus = false;
            bool coldBonus = false;
            bool nrgyBonus = false;
            bool poisBonus = false;
            bool duraBonus = false;
            bool luckBonus = false;
            bool lreqBonus = false;
            bool dincBonus = false;

            if (item is BaseWeapon)
            {
                BaseWeapon weapon = (BaseWeapon)item;

                if (!CraftResources.IsStandard(weapon.Resource))
                    return EnhanceResult.AlreadyEnhanced;

                baseChance = 20;

                dura = weapon.MaxHitPoints;
                luck = weapon.Attributes.Luck;
                lreq = weapon.WeaponAttributes.LowerStatReq;
                dinc = weapon.Attributes.WeaponDamage;

                fireBonus = (attributes.WeaponFireDamage > 0);
                coldBonus = (attributes.WeaponColdDamage > 0);
                nrgyBonus = (attributes.WeaponEnergyDamage > 0);
                poisBonus = (attributes.WeaponPoisonDamage > 0);

                duraBonus = (attributes.WeaponDurability > 0);
                luckBonus = (attributes.WeaponLuck > 0);
                lreqBonus = (attributes.WeaponLowerRequirements > 0);
                dincBonus = (dinc > 0);
            }
            else
            {
                BaseArmor armor = (BaseArmor)item;

                if (!CraftResources.IsStandard(armor.Resource))
                    return EnhanceResult.AlreadyEnhanced;

                baseChance = 20;

                phys = armor.PhysicalResistance;
                fire = armor.FireResistance;
                cold = armor.ColdResistance;
                pois = armor.PoisonResistance;
                nrgy = armor.EnergyResistance;

                dura = armor.MaxHitPoints;
                luck = armor.Attributes.Luck;
                lreq = armor.ArmorAttributes.LowerStatReq;

                physBonus = (attributes.ArmorPhysicalResist > 0);
                fireBonus = (attributes.ArmorFireResist > 0);
                coldBonus = (attributes.ArmorColdResist > 0);
                nrgyBonus = (attributes.ArmorEnergyResist > 0);
                poisBonus = (attributes.ArmorPoisonResist > 0);

                duraBonus = (attributes.ArmorDurability > 0);
                luckBonus = (attributes.ArmorLuck > 0);
                lreqBonus = (attributes.ArmorLowerRequirements > 0);
                dincBonus = false;
            }

            int skill = from.Skills[craftSystem.MainSkill].Fixed / 10;

            if (skill >= 100)
                baseChance -= (skill - 90) / 10;

            EnhanceResult res = EnhanceResult.Success;

            if (physBonus)
                CheckResult(ref res, baseChance + phys);

            if (fireBonus)
                CheckResult(ref res, baseChance + fire);

            if (coldBonus)
                CheckResult(ref res, baseChance + cold);

            if (nrgyBonus)
                CheckResult(ref res, baseChance + nrgy);

            if (poisBonus)
                CheckResult(ref res, baseChance + pois);

            if (duraBonus)
                CheckResult(ref res, baseChance + (dura / 40));

            if (luckBonus)
                CheckResult(ref res, baseChance + 10 + (luck / 2));

            if (lreqBonus)
                CheckResult(ref res, baseChance + (lreq / 4));

            if (dincBonus)
                CheckResult(ref res, baseChance + (dinc / 4));

            switch ( res )
            {
                case EnhanceResult.Broken:
                    {
                        if (!craftItem.ConsumeRes(from, resType, craftSystem, ref resHue, ref maxAmount, ConsumeType.Half, ref resMessage))
                            return EnhanceResult.NoResources;

                        item.Delete();
                        break;
                    }
                case EnhanceResult.Success:
                    {
                        if (!craftItem.ConsumeRes(from, resType, craftSystem, ref resHue, ref maxAmount, ConsumeType.All, ref resMessage))
                            return EnhanceResult.NoResources;

                        if (item is BaseWeapon)
                        {
                            BaseWeapon w = (BaseWeapon)item;

                            w.Resource = resource;

                            #region Mondain's Legacy
                            if (resource != CraftResource.Heartwood)
                            {
                                w.Attributes.WeaponDamage += attributes.WeaponDamage;
                                w.Attributes.WeaponSpeed += attributes.WeaponSwingSpeed;
                                w.Attributes.AttackChance += attributes.WeaponHitChance;
                                w.Attributes.RegenHits += attributes.WeaponRegenHits;			
                                w.WeaponAttributes.HitLeechHits += attributes.WeaponHitLifeLeech;
                            }
                            else
                            {
                                switch ( Utility.Random(6) )
                                {
                                    case 0:
                                        w.Attributes.WeaponDamage += attributes.WeaponDamage;
                                        break;
                                    case 1:
                                        w.Attributes.WeaponSpeed += attributes.WeaponSwingSpeed;
                                        break;
                                    case 2:
                                        w.Attributes.AttackChance += attributes.WeaponHitChance;
                                        break;
                                    case 3:
                                        w.Attributes.Luck += attributes.WeaponLuck;
                                        break;
                                    case 4:
                                        w.WeaponAttributes.LowerStatReq += attributes.WeaponLowerRequirements;
                                        break;
                                    case 5:
                                        w.WeaponAttributes.HitLeechHits += attributes.WeaponHitLifeLeech;
                                        break;
                                }
                            }
                            #endregion

                            int hue = w.GetElementalDamageHue();
                            if (hue > 0)
                                w.Hue = hue;
                        }
                        #region Mondain's Legacy
                        else if (item is BaseShield)
                        {
                            BaseShield shield = (BaseShield)item;

                            shield.Resource = resource;

                            switch ( resource )
                            {
                                case CraftResource.AshWood: 
                                    shield.ArmorAttributes.LowerStatReq += 20; 
                                    break;
                                case CraftResource.YewWood: 
                                    shield.Attributes.RegenHits += 1; 
                                    break;
                                case CraftResource.Heartwood:
                                    switch ( Utility.Random(7) )
                                    {
                                        case 0:
                                            shield.Attributes.BonusDex += 2;
                                            break;
                                        case 1:
                                            shield.Attributes.BonusStr += 2;
                                            break; 
                                        case 2:
                                            shield.Attributes.ReflectPhysical += 5;
                                            break;
                                        case 3:
                                            shield.Attributes.SpellChanneling = 1;
                                            shield.Attributes.CastSpeed = -1;
                                            break;
                                        case 4:
                                            shield.ArmorAttributes.SelfRepair += 2;
                                            break;			
                                        case 5:
                                            shield.PhysicalBonus += 5;
                                            break;
                                        case 6:
                                            shield.ColdBonus += 3;
                                            break;
                                    }
                                    break;
                                case CraftResource.Bloodwood: 
                                    shield.Attributes.RegenHits += 2;
                                    shield.Attributes.Luck += 40;
                                    break;
                                case CraftResource.Frostwood:
                                    shield.Attributes.SpellChanneling = 1; 
                                    shield.Attributes.CastSpeed = -1;
                                    break;
                            }
                        }
                        #endregion
                        else if (item is BaseArmor)	//Sanity
                        {
                            ((BaseArmor)item).Resource = resource;

                            #region Mondain's Legacy
                            BaseArmor armor = (BaseArmor)item;

                            if (resource != CraftResource.Heartwood)
                            {
                                armor.Attributes.WeaponDamage += attributes.ArmorDamage;
                                armor.Attributes.AttackChance += attributes.ArmorHitChance;
                                armor.Attributes.RegenHits += attributes.ArmorRegenHits;				
                                armor.ArmorAttributes.MageArmor += attributes.ArmorMage;
                            }
                            else
                            {
                                switch ( Utility.Random(5) )
                                {
                                    case 0:
                                        armor.Attributes.WeaponDamage += attributes.ArmorDamage;
                                        break;
                                    case 1:
                                        armor.Attributes.AttackChance += attributes.ArmorHitChance;
                                        break;
                                    case 2:
                                        armor.ArmorAttributes.MageArmor += attributes.ArmorMage;
                                        break;			 
                                    case 3:
                                        armor.Attributes.Luck += attributes.ArmorLuck;
                                        break;
                                    case 4:
                                        armor.ArmorAttributes.LowerStatReq += attributes.ArmorLowerRequirements;
                                        break;
                                }
                            }
                            #endregion
                        }

                        break;
                    }
                case EnhanceResult.Failure:
                    {
                        if (!craftItem.ConsumeRes(from, resType, craftSystem, ref resHue, ref maxAmount, ConsumeType.Half, ref resMessage))
                            return EnhanceResult.NoResources;

                        break;
                    }
            }

            return res;
        }

        public static void CheckResult(ref EnhanceResult res, int chance)
        {
            if (res != EnhanceResult.Success)
                return; // we've already failed..

            int random = Utility.Random(100);

            if (10 > random)
                res = EnhanceResult.Failure;
            else if (chance > random)
                res = EnhanceResult.Broken;
        }

        public static void BeginTarget(Mobile from, CraftSystem craftSystem, BaseTool tool)
        {
            CraftContext context = craftSystem.GetContext(from);

            if (context == null)
                return;

            int lastRes = context.LastResourceIndex;
            CraftSubResCol subRes = craftSystem.CraftSubRes;

            if (lastRes >= 0 && lastRes < subRes.Count)
            {
                CraftSubRes res = subRes.GetAt(lastRes);

                if (from.Skills[craftSystem.MainSkill].Value < res.RequiredSkill)
                {
                    from.SendGump(new CraftGump(from, craftSystem, tool, res.Message));
                }
                else
                {
                    CraftResource resource = CraftResources.GetFromType(res.ItemType);

                    if (resource != CraftResource.None)
                    {
                        from.Target = new InternalTarget(craftSystem, tool, res.ItemType, resource);
                        from.SendLocalizedMessage(1061004); // Target an item to enhance with the properties of your selected material.
                    }
                    else
                    {
                        from.SendGump(new CraftGump(from, craftSystem, tool, 1061010)); // You must select a special material in order to enhance an item with its properties.
                    }
                }
            }
            else
            {
                from.SendGump(new CraftGump(from, craftSystem, tool, 1061010)); // You must select a special material in order to enhance an item with its properties.
            }
        }

        private class InternalTarget : Target
        {
            private readonly CraftSystem m_CraftSystem;
            private readonly BaseTool m_Tool;
            private readonly Type m_ResourceType;
            private readonly CraftResource m_Resource;

            public InternalTarget(CraftSystem craftSystem, BaseTool tool, Type resourceType, CraftResource resource)
                : base(2, false, TargetFlags.None)
            {
                this.m_CraftSystem = craftSystem;
                this.m_Tool = tool;
                this.m_ResourceType = resourceType;
                this.m_Resource = resource;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Item)
                {
                    object message = null;
                    EnhanceResult res = Enhance.Invoke(from, this.m_CraftSystem, this.m_Tool, (Item)targeted, this.m_Resource, this.m_ResourceType, ref message);

                    switch ( res )
                    {
                        case EnhanceResult.NotInBackpack:
                            message = 1061005;
                            break; // The item must be in your backpack to enhance it.
                        case EnhanceResult.AlreadyEnhanced:
                            message = 1061012;
                            break; // This item is already enhanced with the properties of a special material.
                        case EnhanceResult.BadItem:
                            message = 1061011;
                            break; // You cannot enhance this type of item with the properties of the selected special material.
                        case EnhanceResult.BadResource:
                            message = 1061010;
                            break; // You must select a special material in order to enhance an item with its properties.
                        case EnhanceResult.Broken:
                            message = 1061080;
                            break; // You attempt to enhance the item, but fail catastrophically. The item is lost.
                        case EnhanceResult.Failure:
                            message = 1061082;
                            break; // You attempt to enhance the item, but fail. Some material is lost in the process.
                        case EnhanceResult.Success:
                            message = 1061008;
                            break; // You enhance the item with the properties of the special material.
                        case EnhanceResult.NoSkill:
                            message = 1044153;
                            break; // You don't have the required skills to attempt this item.
                    }

                    from.SendGump(new CraftGump(from, this.m_CraftSystem, this.m_Tool, message));
                }
            }
        }
    }
}