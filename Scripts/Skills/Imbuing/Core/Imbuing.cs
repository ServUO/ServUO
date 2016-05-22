using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Commands;
//using Server.Engines.QueensLoyalty;
using Server.Factions;
using Server.Engines.Craft;

namespace Server.SkillHandlers
{
    public class Imbuing
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Imbuing].Callback = new SkillUseCallback(OnUse);

            LoadImbuingDefinition();

            CommandSystem.Register("GetTotalWeight", AccessLevel.GameMaster, new CommandEventHandler(GetTotalWeight_OnCommand));
            CommandSystem.Register("GetTotalMods", AccessLevel.GameMaster, new CommandEventHandler(GetTotalMods_OnCommand));
        }

        private static Dictionary<Mobile, ImbuingContext> m_ContextTable = new Dictionary<Mobile, ImbuingContext>();
        public static Dictionary<Mobile, ImbuingContext> ContextTable { get { return m_ContextTable; } }

        public static TimeSpan OnUse(Mobile from)
        {
            if (!from.Alive)
                from.SendLocalizedMessage(500949); //You can't do that when you're dead.
            else
            {
                from.CloseGump(typeof(ImbuingGump));
                from.SendGump(new ImbuingGump(from));
            }

            return TimeSpan.FromSeconds(1.0);
        }

        public static ImbuingContext GetContext(Mobile m)
        {
            if (!m_ContextTable.ContainsKey(m))
            {
                ImbuingContext context = new ImbuingContext(m);
                m_ContextTable[m] = context;
                return context;
            }

            return m_ContextTable[m];
        }

        public static void AddContext(Mobile from, ImbuingContext context)
        {
            m_ContextTable[from] = context;
        }

        public static bool CanImbueItem(Mobile from, Item item)
        {
            if (item == null || !item.IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1079575);  // The item must be in your backpack to imbue it.
            else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
                from.SendLocalizedMessage(1080438);  // You cannot imbue a blessed item.
            else if (item is BaseWeapon && Spells.Mystic.EnchantSpell.IsUnderSpellEffects(from, (BaseWeapon)item))
                from.SendLocalizedMessage(1080130);  // You cannot imbue an item that is currently enchanted.
            else if (item is BaseWeapon && ((BaseWeapon)item).FocusWeilder != null)
                from.SendLocalizedMessage(1080444);  //You cannot imbue an item that is under the effects of the ninjitsu focus attack ability.
            else if (IsSpecialItem(item))
                from.SendMessage("You cannot imbue an item with such strange magical properties.");
            else if (item is IFactionItem && ((IFactionItem)item).FactionItemState != null)
                from.SendLocalizedMessage(1114312); // You cannot imbue faction items.
            else if (item is BaseJewel && !(item is BaseRing) && !(item is BaseBracelet))
                from.SendLocalizedMessage(1079576); // You cannot imbue this item.
            else if (IsInNonImbueList(item.GetType()))
                from.SendLocalizedMessage(1079576); // You cannot imbue this item.
            else if (item is BaseWeapon && ((BaseWeapon)item).TimesImbued >= 20)
                from.SendMessage("This item has been modified too many times and cannot be imbued any further.");
            else if (item is BaseArmor && ((BaseArmor)item).TimesImbued >= 20)
                from.SendMessage("This item has been modified too many times and cannot be imbued any further.");
            else if (item is BaseJewel && ((BaseJewel)item).TimesImbued >= 20)
                from.SendMessage("This item has been modified too many times and cannot be imbued any further.");
            else if (item is BaseHat && ((BaseHat)item).TimesImbued >= 20)
                from.SendMessage("This item has been modified too many times and cannot be imbued any further.");
            else
                return true;

            return false;
        }

        public static bool OnBeforeImbue(Mobile from, Item item, int mod, int value)
        {
            return OnBeforeImbue(from, item, mod, value, Imbuing.GetTotalMods(item, mod), ImbuingGumpC.MaxProps, Imbuing.GetTotalWeight(item, mod), Imbuing.GetMaxWeight(item));
        }

        public static bool OnBeforeImbue(Mobile from, Item item, int mod, int value, int totalprops, int maxprops, int totalitemweight, int maxweight)
        {
         
            if (totalprops >= maxprops || totalitemweight > maxweight)
            {
                from.SendLocalizedMessage(1079772); // You cannot imbue this item with any more item properties.
                from.CloseGump(typeof(ImbuingGumpC));
                return false;
            }

            return true;
        }

        public static bool CanUnravelItem(Mobile from, Item item, bool message = true)
        {
            if (!item.IsChildOf(from.Backpack))
            {
                if(message)
                    from.SendLocalizedMessage(1080424);  // The item must be in your backpack to magically unravel it.
            }
            else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
            {
                if (message)
                    from.SendLocalizedMessage(1080421);  // You cannot unravel the magic of a blessed item.
            }
            else if (item is BaseWeapon && Spells.Mystic.EnchantSpell.IsUnderSpellEffects(from, (BaseWeapon)item))
            {
                if (message)
                    from.SendLocalizedMessage(1080427);  // You cannot magically unravel an item that is currently enchanted.
            }
            //else if (item is BaseWeapon && ((BaseWeapon)item).FocusWeilder != null)
            //{
            //    if (message)
            //        from.SendLocalizedMessage(1080445); //You cannot magically unravel an item that is under the effects of the ninjitsu focus attack ability.
            //}
            else if (item is IFactionItem && ((IFactionItem)item).FactionItemState != null)
            {
                if (message)
                    from.SendLocalizedMessage(1112408); // You cannot magically unravel a faction reward item.
            }
            /*else if (IsSpecialItem(item))
            {
                if (message)
                    from.SendMessage("You cannot magically unravel an item with such strange magical properties.");
            }*/
            else
                return true;
            return false;
        }

        public static bool IsSpecialItem(Item item)
        {
            if (item == null)
                return true;

			if (item.IsArtifact)
				return true;

            if (item is BaseWeapon && ((BaseWeapon)item).ArtifactRarity > 0)
                return true;

            if (item is BaseArmor && ((BaseArmor)item).ArtifactRarity > 0)
                return true;

            if (item is BaseJewel && ((BaseJewel)item).ArtifactRarity > 0)
                return true;

            if (item is BaseClothing && ((BaseClothing)item).ArtifactRarity > 0)
                return true;

			if (item.GetType() == typeof(SilverRing) || item.GetType() == typeof(SilverBracelet))
				return false;

            foreach (CraftSystem system in CraftSystem.Systems)
            {
                CraftItem crItem = null;
                
                if(system != null && system.CraftItems != null)
                    crItem = system.CraftItems.SearchFor(item.GetType());

                if (crItem != null)
                    return false;
            }

            return true;
        }

        public static double GetSuccessChance(Mobile from, Item item, int totalItemWeight, int propWeight, out double dif)
        {
            double suc = 0;     // display difficulty
            double bonus = 0;   // bonuses

            ImbuingContext context = GetContext(from);
            double skill = from.Skills[SkillName.Imbuing].Value;

            // - Racial Bonus - SA ONLY -
            if (from.Race == Race.Gargoyle)
                bonus = 10;

            // Queen Soul Forge Bonus
            if (context.Imbue_SFBonus > 0)
                bonus += context.Imbue_SFBonus;

            bonus += GetQualityBonus(item);
            bonus /= 100;

            double resultWeight = totalItemWeight + propWeight;

            if (resultWeight <= 500)
            {
                dif = (((resultWeight) / 22) * (1)) + ((resultWeight) / 8);
                suc = ((skill - dif) * 1);
            }
            else
            {
                dif = ((((resultWeight) - 500) / 75) * (1)) + (((resultWeight) - 500) / 64);
                suc = ((skill - (dif + 64)) * 1) + bonus;
            }

            suc += suc * bonus;

            if (suc < 0)
                suc = 0;

            if (suc > 100 && from.AccessLevel == AccessLevel.Player) //display purposes
                suc = 100;

            suc = Math.Round(suc, 2);

            return suc;
        }

        public static int GetQualityBonus(Item item)
        {
            if (item is BaseWeapon)
            {
                if (((BaseWeapon)item).Quality == WeaponQuality.Exceptional)
                    return 20;

                if (((BaseWeapon)item).PlayerConstructed)
                    return 10;
            }
            else if (item is BaseArmor)
            {
                if (((BaseArmor)item).Quality == ArmorQuality.Exceptional)
                    return 20;

                if (((BaseArmor)item).PlayerConstructed)
                    return 10;
            }
            else if (item is BaseHat)
            {
                if (((BaseHat)item).Quality == ClothingQuality.Exceptional)
                    return 20;

                if (((BaseHat)item).PlayerConstructed)
                    return 10;
            }
            else if (item is BaseJewel)
            {
            }

            return 0;
        }

        /// <summary>
        /// Imbues Item with selected mod
        /// </summary>
        /// <param name="from">Player Imbuing</param>
        /// <param name="i">Item to be imbued</param>
        /// <param name="mod">mod to be imbued, see m_Table</param>
        /// <param name="value">value for mod</param>
        public static void ImbueItem(Mobile from, Item i, int mod, int value)
        {
            if (!CheckSoulForge(from, 1))
                return;

            ImbuingContext context = Imbuing.GetContext(from);

            context.LastImbued = i;
            context.Imbue_Mod = mod;
            context.Imbue_ModInt = value;

            if (!Imbuing.Table.ContainsKey(mod))
                return;

            ImbuingDefinition def = Imbuing.Table[mod];
            Type gem = def.GemRes;
            Type primary = def.PrimaryRes;
            Type special = def.SpecialRes;
            context.Imbue_ModVal = def.Weight;

            int itemRef = ImbuingGump.GetItemRef(i);
            int gemAmount = GetGemAmount(i, mod, value);
            int primResAmount = GetPrimaryAmount(i, mod, value);
            int specResAmount = GetSpecialAmount(i, mod, value);

            if (from.AccessLevel == AccessLevel.Player && 
                (from.Backpack == null || from.Backpack.GetAmount(gem) < gemAmount || 
                from.Backpack.GetAmount(primary) < primResAmount || 
                from.Backpack.GetAmount(special) < specResAmount))
                from.SendLocalizedMessage(1079773); //You do not have enough resources to imbue this item.     
            else
            {
                int maxWeight = GetMaxWeight(i);
                context.Imbue_IWmax = maxWeight;

                int totalItemWeight = GetTotalWeight(i, mod);
                int totalItemMods = GetTotalMods(i, mod);

                double propWeight = ((double)def.Weight / (double)def.MaxIntensity) * value;
                propWeight = Math.Round(propWeight);
                int propweight = Convert.ToInt32(propWeight);

                if ((totalItemWeight + propweight) > maxWeight)
                {
                    from.SendLocalizedMessage(1079772); // You cannot imbue this item with any more item properties.
                    from.CloseGump(typeof(ImbuingGumpC));
                    return;
                }

                double difficulty = 0;
                double success = GetSuccessChance(from, i, (int)totalItemWeight, propweight, out difficulty);

                from.CheckSkill(SkillName.Imbuing, difficulty - 50, difficulty + 50);

                success /= 100;

                from.Backpack.ConsumeTotal(primary, primResAmount);

                if (i is BaseWeapon) ((BaseWeapon)i).TimesImbued += 1;
                if (i is BaseArmor) ((BaseArmor)i).TimesImbued += 1;
                if (i is BaseJewel) ((BaseJewel)i).TimesImbued += 1;
                if (i is BaseClothing) ((BaseClothing)i).TimesImbued += 1;

                if (success >= Utility.RandomDouble() || mod < 0 || mod > 180)
                {
                    if(from.AccessLevel == AccessLevel.Player)
                        from.Backpack.ConsumeTotal(gem, gemAmount);

                    if(specResAmount > 0)
                        from.Backpack.ConsumeTotal(special, specResAmount);

                    from.SendLocalizedMessage(1079775); // You successfully imbue the item!
                    if (i is BaseWeapon) ((BaseWeapon)i).IsImbued = true;
                    if (i is BaseArmor) ((BaseArmor)i).IsImbued = true;
                    if (i is BaseJewel) ((BaseJewel)i).IsImbued = true;
                    if (i is BaseClothing) ((BaseClothing)i).IsImbued = true;

                    from.PlaySound(0x5D1);
                    Effects.SendLocationParticles(
                    EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x373A,
                          10, 30, 0, 4, 0, 0);

                    object prop = GetAttribute(mod);

                    if (i is BaseWeapon)
                    {
                        BaseWeapon wep = i as BaseWeapon;
                        wep.WeaponAttributes.SelfRepair = 0;

                        if (prop is AosAttribute)
                        {
                            AosAttribute attr = (AosAttribute)prop;

                            if (attr == AosAttribute.SpellChanneling)
                            {
                                wep.Attributes.SpellChanneling = value;

                                if (wep.Attributes.CastSpeed >= 0) 
                                    wep.Attributes.CastSpeed -= 1;
                            }
                            else if (attr == AosAttribute.WeaponDamage)
                            {
                                wep.Attributes.WeaponDamage = value;

                                if (!wep.DImodded)
                                    wep.DImodded = true;
                            }
                            else
                                wep.Attributes[attr] = value;
                        }

                        if (prop is AosWeaponAttribute)
                        {
                            if (mod >= 30 && mod <= 34)
                            {
                                wep.WeaponAttributes.HitPhysicalArea = 0;
                                wep.WeaponAttributes.HitFireArea = 0;
                                wep.WeaponAttributes.HitColdArea = 0;
                                wep.WeaponAttributes.HitPoisonArea = 0;
                                wep.WeaponAttributes.HitEnergyArea = 0;
                            }
                            else if (mod >= 35 && mod <= 39)
                            {
                                wep.WeaponAttributes.HitMagicArrow = 0;
                                wep.WeaponAttributes.HitHarm = 0;
                                wep.WeaponAttributes.HitFireball = 0;
                                wep.WeaponAttributes.HitLightning = 0;
                                wep.WeaponAttributes.HitDispel = 0;
                            }

                            wep.WeaponAttributes[(AosWeaponAttribute)prop] = value;
                        }

                        if (prop is SlayerName)
                            wep.Slayer = (SlayerName)prop;

                        if(prop is SAAbsorptionAttribute)
                            wep.AbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;

                        if (prop is AosElementAttribute)
                        {
                            AosElementAttribute attr = (AosElementAttribute)prop;

                            switch (attr)
                            {
                                case AosElementAttribute.Physical: wep.WeaponAttributes.ResistPhysicalBonus = value; break;
                                case AosElementAttribute.Fire: wep.WeaponAttributes.ResistFireBonus = value; break;
                                case AosElementAttribute.Cold: wep.WeaponAttributes.ResistColdBonus = value; break;
                                case AosElementAttribute.Poison: wep.WeaponAttributes.ResistPoisonBonus = value; break;
                                case AosElementAttribute.Energy: wep.WeaponAttributes.ResistEnergyBonus = value; break;
                            }
                        }

                        if (prop is string)
                        {
                            string p = prop as string;

                            if (p == "BalancedWeapon" && wep is BaseRanged)
                                ((BaseRanged)wep).Balanced = true;
                            else if (p == "WeaponVelocity" && wep is BaseRanged)
                                ((BaseRanged)wep).Velocity = value;
                        }
                    }
                    else if (itemRef == 3)
                    {
                        BaseArmor arm = i as BaseArmor;
                        arm.ArmorAttributes.SelfRepair = 0;

                        if (prop is AosAttribute)
                        {
                            AosAttribute attr = (AosAttribute)prop;

                            if (attr == AosAttribute.SpellChanneling)
                            {
                                arm.Attributes.SpellChanneling = value;
                                arm.Attributes.CastSpeed -= 1;
                            }
                            else if (attr == AosAttribute.WeaponDamage)
                                arm.Attributes.WeaponDamage = value;
                            else
                                arm.Attributes[attr] = value;
                        }

                        if (prop is AosElementAttribute)
                        {
                            AosElementAttribute attr = (AosElementAttribute)prop;

                            switch (attr)
                            {
                                case AosElementAttribute.Physical: arm.PhysicalBonus = value; arm.PhysImbuing = 0; break;
                                case AosElementAttribute.Fire: arm.FireBonus = value; arm.FireImbuing = 0; break;
                                case AosElementAttribute.Cold: arm.ColdBonus = value; arm.ColdImbuing = 0; break;
                                case AosElementAttribute.Poison: arm.PoisonBonus = value; arm.PoisonImbuing = 0; break;
                                case AosElementAttribute.Energy: arm.EnergyBonus = value; arm.EnergyImbuing = 0; break;
                            }
                        }

                        if (prop is SAAbsorptionAttribute)
                            arm.AbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;

                        if (prop is AosArmorAttribute)
                            arm.ArmorAttributes[(AosArmorAttribute)prop] = value;
                    }
                    else if (itemRef == 4)
                    {
                        BaseShield shield = i as BaseShield;
                        shield.ArmorAttributes.SelfRepair = 0;

                        if (prop is AosAttribute)
                        {
                            AosAttribute attr = (AosAttribute)prop;

                            if (attr == AosAttribute.SpellChanneling)
                            {
                                shield.Attributes.SpellChanneling = value;

                                if (shield.Attributes.CastSpeed >= 0)
                                    shield.Attributes.CastSpeed -= 1;
                            }
                            else
                                shield.Attributes[attr] = value;
                        }

                        if (prop is SAAbsorptionAttribute)
                            shield.AbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;

                        if (prop is AosArmorAttribute)
                            shield.ArmorAttributes[(AosArmorAttribute)prop] = value;
                    }
                    else if (i is BaseHat)
                    {
                        BaseHat hat = i as BaseHat;

                        if (prop is AosAttribute)
                            hat.Attributes[(AosAttribute)prop] = value;

                        if (prop is SAAbsorptionAttribute)
                            hat.SAAbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;

                        if (prop is AosElementAttribute)
                        {
                            AosElementAttribute attr = (AosElementAttribute)prop;

                            switch (attr)
                            {
                                case AosElementAttribute.Physical: hat.Resistances.Physical = value; hat.PhysImbuing = 0; break;
                                case AosElementAttribute.Fire: hat.Resistances.Fire = value; hat.FireImbuing = 0; break;
                                case AosElementAttribute.Cold: hat.Resistances.Cold = value; hat.ColdImbuing = 0; break;
                                case AosElementAttribute.Poison: hat.Resistances.Poison = value; hat.PoisonImbuing = 0; break;
                                case AosElementAttribute.Energy: hat.Resistances.Energy = value; hat.EnergyImbuing = 0; break;
                            }
                        }
                    }
                    else if (i is BaseJewel)
                    {
                        BaseJewel jewel = i as BaseJewel;

                        if (jewel.MaxHitPoints <= 0 && jewel.TimesImbued >= 1)
                        {
                            jewel.MaxHitPoints = 255;
                            jewel.HitPoints = 255;
                        }

                        if (prop is AosAttribute)
                            jewel.Attributes[(AosAttribute)prop] = value;

                        if (prop is SAAbsorptionAttribute)
                            jewel.AbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;

                        if (prop is AosElementAttribute)
                        {
                            AosElementAttribute attr = (AosElementAttribute)prop;

                            switch (attr)
                            {
                                case AosElementAttribute.Physical: jewel.Resistances.Physical = value; break;
                                case AosElementAttribute.Fire: jewel.Resistances.Fire = value; break;
                                case AosElementAttribute.Cold: jewel.Resistances.Cold = value; break;
                                case AosElementAttribute.Poison: jewel.Resistances.Poison = value; break;
                                case AosElementAttribute.Energy: jewel.Resistances.Energy = value; break;
                            }
                        }

                        if (prop is SkillName)
                        {
                            SkillName skill = (SkillName)prop;

                            //Removes skill bonus if jewel already exist
                            for (int j = 0; j < 5; j++)
                            {
                                if (jewel.SkillBonuses.GetSkill(j) == skill)
                                {
                                    jewel.SkillBonuses.SetBonus(j, 0.0);
                                    jewel.SkillBonuses.SetSkill(j, SkillName.Alchemy);
                                }
                            }

                            if (mod >= 151 && mod <= 155)
                                jewel.SkillBonuses.SetValues(0, skill, value);
                            else if (mod >= 156 && mod <= 160)
                                jewel.SkillBonuses.SetValues(1, skill, value);
                            else if (mod >= 161 && mod <= 166)
                                jewel.SkillBonuses.SetValues(2, skill, value);
                            else if (mod >= 167 && mod <= 173)
                                jewel.SkillBonuses.SetValues(3, skill, value);
                            else if (mod >= 174 && mod <= 180)
                                jewel.SkillBonuses.SetValues(4, skill, value);
                        }
                    }

                    i.InvalidateProperties();
                }
                // == FAILURE == 
                else
                {
                    from.SendLocalizedMessage(1079774); // Fail
                    from.PlaySound(0x5AC);
                }
            }
        }

        public static bool UnravelItem(Mobile from, Item item, bool message = true)
        {
            int weight = GetTotalWeight(item);

            ImbuingContext context = Imbuing.GetContext(from);

            int bonus = context.Imbue_SFBonus;
            int unravelQTY = weight / 100 ;
            bool success = false;

            if (weight > 0)
            {
                // == Relic Fragment ==
                if (weight >= (480 - bonus))
                {
                    if (from.Skills[SkillName.Imbuing].Base >= 95.0)
                    {
                        if (success = from.CheckSkill(SkillName.Imbuing, 95.0, 120.0))
                        {
                            item.Delete();
                            from.AddToBackpack(new RelicFragment(unravelQTY - Utility.Random(0, 3)));

                        }
                        else if (success = from.CheckSkill(SkillName.Imbuing, 45.0, 95.0))
                        {
                            item.Delete();
                            from.AddToBackpack(new EnchantEssence(unravelQTY - Utility.Random(0, 3)));
                        }
                        else
                        {
                            Effects.PlaySound(from.Location, from.Map, 0x3BF);
                            if (message)
                                from.SendLocalizedMessage(1080428);  //You attempt to magically unravel the item, but fail.
                        }
                    }
                    else
                    {
                        if (message)
                            from.SendLocalizedMessage(1080434); // Your Imbuing skill is not high enough to magically unravel this item.
                        return false;
                    }
                }
                // == Enchanted Essence ==
                else if (weight > (200 - bonus) && weight < (480 - bonus))
                {
                    if (from.Skills[SkillName.Imbuing].Base >= 45.0)
                    {
                        if (success = from.CheckSkill(SkillName.Imbuing, 45.0, 95.0))
                        {
                            item.Delete();
                            from.AddToBackpack(new EnchantEssence(unravelQTY));
                        }
                        else if(success = from.CheckSkill(SkillName.Imbuing, 0.0, 45.0))
                        {
                            item.Delete();
                            from.AddToBackpack(new MagicalResidue(unravelQTY + Utility.Random(0, 2)));
                        }
                        else
                        {
                            Effects.PlaySound(from.Location, from.Map, 0x3BF);
                            if (message)
                                from.SendLocalizedMessage(1080428);  //You attempt to magically unravel the item, but fail.
                        }
                    }
                    else
                    {
                        if (message)
                            from.SendLocalizedMessage(1080434); // Your Imbuing skill is not high enough to magically unravel this item.
                        return false;
                    }
                }
                 // == Magical Residue ==
                else if (weight <= (200 - bonus))
                {
                    if (success = from.CheckSkill(SkillName.Imbuing, 0.0, 45.0))
                    {
                        item.Delete();
                        from.AddToBackpack(new MagicalResidue(unravelQTY + Utility.Random(1,2)));
                    }
                    else
                    {
                        Effects.PlaySound(from.Location, from.Map, 0x3BF);
                        if (message)
                            from.SendLocalizedMessage(1080428);  //You attempt to magically unravel the item, but fail.
                    }
                }
                     
            }
            else if (message)
                from.SendLocalizedMessage(1080437); // You cannot magically unravel this item. It appears to possess little or no magic.

            return success;
        }

        public static int GetMaxWeight(object itw)
        {
            int MaxW = 450;

            if (itw is BaseWeapon)
            {
                BaseWeapon itemToImbue = itw as BaseWeapon;

                if (itemToImbue.Quality == WeaponQuality.Exceptional)
                {
                    if (itemToImbue is BaseRanged)
                     return 550;
                    else if (itemToImbue.Layer == Layer.TwoHanded)
                      return 600;

                  else
                     return  500;

                }
                else if (itemToImbue.Quality == WeaponQuality.Regular)
                {
                    if (itemToImbue is BaseRanged)
                        return 500;
                    else if (itemToImbue.Layer == Layer.TwoHanded)
                        return 550;
                    else
                        return 450;

                }
                else
                    return 450;
            }
            else if (itw is BaseArmor)
            {
                BaseArmor itemToImbue = itw as BaseArmor;
                if (itemToImbue.Quality == ArmorQuality.Exceptional)
                    MaxW = 500;
                else if (itemToImbue.Quality == ArmorQuality.Regular)
                    MaxW = 450;
                
            }
            else if (itw is BaseHat)
            {
                BaseHat itemToImbue = itw as BaseHat;
                if (itemToImbue.Quality == ClothingQuality.Exceptional)
                    MaxW = 500;
                else if (itemToImbue.Quality == ClothingQuality.Regular)
                    MaxW = 450;
               
            }
            else if (itw is BaseJewel)
            {
                return 500;
            }

            return MaxW;
        }

        public static int GetGemAmount(Item item, int mod, int value)
        {
            ImbuingDefinition def = Imbuing.Table[mod];

            int Max = def.MaxIntensity;
            int Inc = def.IncAmount;

            if (item is BaseJewel && mod == 12)
                Max /= 2;

            if (Max == 1 && Inc == 0)
                return 10;

            double v = value / ((double)Max / 10);
            double newV = Math.Floor(v);

            if (newV > 10) newV = 10;
            if (newV < 1) newV = 1;

            return (int)newV;
        }

        public static int GetPrimaryAmount(Item item, int mod, int value)
        {
            ImbuingDefinition def = Imbuing.Table[mod];

            int Max = def.MaxIntensity;
            int Inc = def.IncAmount;
            

            if (item is BaseJewel && mod == 12)
                Max /= 2;

            if (Max == 1 && Inc == 0)
                return 5;

            double v = value / ((double)Max / 5.0);
            double newV = Math.Floor(v);

            if (newV > 5) newV = 5;
            if (newV < 1) newV = 1;

            return (int)newV;
        }

        public static int GetSpecialAmount(Item item, int mod, int value)
        {
            ImbuingDefinition def = Imbuing.Table[mod];

            int Max = def.MaxIntensity;
            int Inc = def.IncAmount;
            int MWeight = def.Weight;

            double currentIntensity = ((double)MWeight / (double)Max * value);
            currentIntensity = Math.Floor(currentIntensity);

            if (currentIntensity == MWeight)
                return 10;
            else if (MWeight - currentIntensity >= 1 && currentIntensity > 90 )
            {
                if (Max < 10)
                    return 0;
                else if (Max >= 16 && Max <= 89)
                    return 5;
                else if (Max > 90 && Max <= 100)
                    return (int)currentIntensity - 90;
                else
                    return 3;
            }
            else
                return 0;
        }

        [Usage("GetTotalMods")]
        [Description("Displays the total mods, ie AOS attributes for the targeted item.")]
        public static void GetTotalMods_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(12, false, TargetFlags.None, new TargetCallback(GetTotalMods_OnTarget));
            e.Mobile.SendMessage("Target the item to get total AOS Attributes.");
        }

        public static void GetTotalMods_OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                int mods = GetTotalMods((Item)targeted);

                ((Item)targeted).LabelTo(from, String.Format("Total Mods: {0}", mods.ToString()));
            }
            else
                from.SendMessage("That is not an item!");
        }

        public static int GetTotalMods(Item itw, int mod = -1)
        {
            int total = 0;
            object prop = GetAttribute(mod);

            if (itw is BaseWeapon)
            {
                BaseWeapon wep = itw as BaseWeapon;

                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                {
                    if (i >= 0x10000000) // Brittle/Inc. Karma Loss
                        continue;

                    AosAttribute attr = (AosAttribute)i;

                    if (wep.Attributes[attr] > 0 && (attr != AosAttribute.WeaponDamage || (attr == AosAttribute.WeaponDamage && wep.Attributes[attr] > 50)))
                    {
                        if (!(prop is AosAttribute) || ((AosAttribute)prop) != attr)
                            total += 1;
                    }
                    else if (wep.Attributes[attr] == 0 && attr == AosAttribute.CastSpeed && wep.Attributes[AosAttribute.SpellChanneling] > 0)
                    {
                        if(!(prop is AosAttribute) || (AosAttribute)prop != attr)
                            total += 1;
                    }
                }

                total += GetSkillBonuses(wep.SkillBonuses, prop);

                foreach (long i in Enum.GetValues(typeof(AosWeaponAttribute)))
                {
                    AosWeaponAttribute attr = (AosWeaponAttribute)i;

                    if (wep.WeaponAttributes[attr] > 0)
                    {
                        if (IsHitAreaOrSpell(attr, mod))
                            continue;

                        if (!(prop is AosWeaponAttribute) || ((AosWeaponAttribute)prop) != attr)
                            total += 1;
                    }
                }

                if (wep.Slayer != SlayerName.None && (!(prop is SlayerName) || ((SlayerName)prop) != wep.Slayer))
                    total += 1;

                if (wep.Slayer2 != SlayerName.None)
                    total += 1;

                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                {
                    SAAbsorptionAttribute attr = (SAAbsorptionAttribute)i;

                    if (wep.AbsorptionAttributes[attr] > 0)
                    {
                        if (!(prop is SAAbsorptionAttribute) || ((SAAbsorptionAttribute)prop) != attr)
                            total += 1;
                    }
                }

                if (wep is BaseRanged && !(prop is string))
                {
                    BaseRanged ranged = wep as BaseRanged;

                    if (ranged.Balanced && mod != 61)
                        total++;

                    if (ranged.Velocity > 0 && mod != 60)
                        total++;
                }

            }
            else if (itw is BaseArmor)
            {
                BaseArmor armor = itw as BaseArmor;

                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                {
                    if (i >= 0x10000000) // Brittle/Inc. Karma Loss
                        continue;

                    AosAttribute attr = (AosAttribute)i;

                    if (armor.Attributes[attr] > 0)
                    {
                        if (!(prop is AosAttribute) || ((AosAttribute)prop) != attr)
                            total += 1;
                    }
                    else if (armor.Attributes[attr] == 0 && attr == AosAttribute.CastSpeed && armor.Attributes[AosAttribute.SpellChanneling] > 0)
                    {
                        if (!(prop is AosAttribute) || (AosAttribute)prop == attr)
                            total += 1;
                    }
                }

                total += GetSkillBonuses(armor.SkillBonuses, prop);

                if (armor.PhysicalBonus > armor.PhysImbuing && mod != 51) { total += 1; }
                if (armor.FireBonus > armor.FireImbuing && mod != 52) { total += 1; }
                if (armor.ColdBonus > armor.ColdImbuing && mod != 53) { total += 1; }
                if (armor.PoisonBonus > armor.PoisonImbuing && mod != 54) { total += 1; }
                if (armor.EnergyBonus > armor.EnergyImbuing && mod != 55) { total += 1; }

                foreach (int i in Enum.GetValues(typeof(AosArmorAttribute)))
                {
                    AosArmorAttribute attr = (AosArmorAttribute)i;

                    if (armor.ArmorAttributes[attr] > 0)
                    {
                        if (!(prop is AosArmorAttribute) || ((AosArmorAttribute)prop) != attr)
                            total += 1;
                    }
                }


                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                {
                    SAAbsorptionAttribute attr = (SAAbsorptionAttribute)i;

                    if (armor.AbsorptionAttributes[attr] > 0)
                    {
                        if (!(prop is SAAbsorptionAttribute) || ((SAAbsorptionAttribute)prop) != attr)
                            total += 1;
                    }
                }
            }
            else if (itw is BaseJewel)
            {
                BaseJewel j = itw as BaseJewel;

                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                {
                    if (i >= 0x10000000) // Brittle/Inc. Karma Loss
                        continue;

                    AosAttribute attr = (AosAttribute)i;

                    if (j.Attributes[attr] > 0)
                    {
                        if (!(prop is AosAttribute) || ((AosAttribute)prop) != attr)
                            total += 1;
                    }
                }

                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                {
                    SAAbsorptionAttribute attr = (SAAbsorptionAttribute)i;

                    if (j.AbsorptionAttributes[attr] > 0)
                    {
                        if (!(prop is SAAbsorptionAttribute) || ((SAAbsorptionAttribute)prop) != attr)
                            total += 1;
                    }
                }

                total += GetSkillBonuses(j.SkillBonuses, prop);

                if (j.Resistances.Physical > 0 && mod != 51) { total += 1; }
                if (j.Resistances.Fire > 0 && mod != 52) { total += 1; }
                if (j.Resistances.Cold > 0 && mod != 53) { total += 1; }
                if (j.Resistances.Poison > 0 && mod != 54) { total += 1; }
                if (j.Resistances.Energy > 0 && mod != 55) { total += 1; }
            }
            else if (itw is BaseHat)
            {
                BaseHat h = itw as BaseHat;

                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                {
                    if (i >= 0x10000000) // Brittle/Inc. Karma Loss
                        continue;

                    AosAttribute attr = (AosAttribute)i;

                    if (h.Attributes[attr] > 0)
                    {
                        if (!(prop is AosAttribute) || ((AosAttribute)prop) != attr)
                            total += 1;
                    }
                }

                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                {
                    SAAbsorptionAttribute attr = (SAAbsorptionAttribute)i;

                    if (h.SAAbsorptionAttributes[attr] > 0)
                    {
                        if (!(prop is SAAbsorptionAttribute) || ((SAAbsorptionAttribute)prop) != attr)
                            total += 1;
                    }
                }

                total += GetSkillBonuses(h.SkillBonuses, prop);

                if (h.Resistances.Physical > h.PhysImbuing && mod != 51) { total += 1; }
                if (h.Resistances.Fire > h.FireImbuing && mod != 52) { total += 1; }
                if (h.Resistances.Cold > h.ColdImbuing && mod != 53) { total += 1; }
                if (h.Resistances.Poison > h.PoisonImbuing && mod != 54) { total += 1; }
                if (h.Resistances.Energy > h.EnergyImbuing && mod != 55) { total += 1; }
            }

            return total;
        }

        private static bool IsHitAreaOrSpell(AosWeaponAttribute attr, int mod)
        {
            if (attr >= AosWeaponAttribute.HitMagicArrow && attr <= AosWeaponAttribute.HitDispel)
                return mod >= 35 && mod <= 39;
            else if (attr >= AosWeaponAttribute.HitColdArea && attr <= AosWeaponAttribute.HitPhysicalArea)
                return mod >= 30 && mod <= 34;
            return false;
        }

        private static bool IsInSkillGroup(SkillName skill, int index)
        {
            if (index < 0 || index >= m_SkillGroups.Length)
                return false;

            foreach (SkillName name in m_SkillGroups[index])
            {
                if (name == skill)
                    return true;
            }
            return false;
        }

        private static int GetSkillBonuses(AosSkillBonuses bonus, object prop)
        {
            int mod = 0;

            for (int j = 0; j < 5; j++)
            {
                if (bonus.GetBonus(j) > 0)
                {
                    if (!(prop is SkillName) || !IsInSkillGroup((SkillName)prop, j))
                        mod += 1;
                }
            }

            return mod;
        }

        [Usage("GetTotalWeight")]
        [Description("Displays the total imbuing weight of the targeted item.")]
        public static void GetTotalWeight_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(12, false, TargetFlags.None, new TargetCallback(GetTotalWeight_OnTarget));
            e.Mobile.SendMessage("Target the item to get total imbuing weight.");
        }

        public static void GetTotalWeight_OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                int w = GetTotalWeight((Item)targeted);

                ((Item)targeted).LabelTo(from, String.Format("Imbuing Weight: {0}", w.ToString()));
            }
            else
                from.SendMessage("That is not an item!");
        }

        public static int GetTotalWeight(Item item, int mod = -1)
        {
            double weight = 0;

            AosAttributes aosAttrs = null;
            AosWeaponAttributes wepAttrs = null;
            SAAbsorptionAttributes saAttrs = null;
            AosArmorAttributes armorAttrs = null;
            AosElementAttributes resistAttrs = null;

            if (item is BaseWeapon)
            {
                aosAttrs = ((BaseWeapon)item).Attributes;
                wepAttrs = ((BaseWeapon)item).WeaponAttributes;
                saAttrs = ((BaseWeapon)item).AbsorptionAttributes;

                if(((BaseWeapon)item).Slayer != SlayerName.None)
                    weight += GetIntensityForAttribute(((BaseWeapon)item).Slayer, mod, 1);

                if (((BaseWeapon)item).Slayer2 != SlayerName.None)
                    weight += GetIntensityForAttribute(((BaseWeapon)item).Slayer2, mod, 1);

                if (item is BaseRanged)
                {
                    BaseRanged ranged = item as BaseRanged;

                    if(ranged.Balanced)
                        weight += GetIntensityForAttribute("BalancedWeapon", mod, 1);

                    if(ranged.Velocity > 0)
                        weight += GetIntensityForAttribute("WeaponVelocity", mod, ranged.Velocity);
                }
            }
            else if (item is BaseArmor)
            {
                aosAttrs = ((BaseArmor)item).Attributes;
                armorAttrs = ((BaseArmor)item).ArmorAttributes;
                saAttrs = ((BaseArmor)item).AbsorptionAttributes;

                weight += CheckResists((BaseArmor)item, mod);
            }
            else if (item is BaseHat)
            {
                aosAttrs = ((BaseHat)item).Attributes;
                armorAttrs = ((BaseHat)item).ClothingAttributes;
                resistAttrs = ((BaseHat)item).Resistances;
                saAttrs = ((BaseHat)item).SAAbsorptionAttributes;
            }
            else if (item is BaseJewel)
            {
                aosAttrs = ((BaseJewel)item).Attributes;
                resistAttrs = ((BaseJewel)item).Resistances;
                saAttrs = ((BaseJewel)item).AbsorptionAttributes;
            }
            else
                return 0;

            if (aosAttrs != null)
                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                    weight += GetIntensityForAttribute((AosAttribute)i, mod, aosAttrs[(AosAttribute)i]);

            if (wepAttrs != null)
                foreach (long i in Enum.GetValues(typeof(AosWeaponAttribute)))
                    weight += GetIntensityForAttribute((AosWeaponAttribute)i, mod, wepAttrs[(AosWeaponAttribute)i]);

            if (saAttrs != null)
                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                    weight += GetIntensityForAttribute((SAAbsorptionAttribute)i, mod, saAttrs[(SAAbsorptionAttribute)i]);

            if (armorAttrs != null)
                foreach (int i in Enum.GetValues(typeof(AosArmorAttribute)))
                    weight += GetIntensityForAttribute((AosArmorAttribute)i, mod, armorAttrs[(AosArmorAttribute)i]);

            if (resistAttrs != null)
                foreach (int i in Enum.GetValues(typeof(AosElementAttribute)))
                    weight += GetIntensityForAttribute((AosElementAttribute)i, mod, resistAttrs[(AosElementAttribute)i]);

            weight += CheckSkillBonuses(item, mod);

            return (int)Math.Round(weight);
        }

        private static int CheckSkillBonuses(Item item, int modification)
        {
            double weight = 0;
            int mod = -1;

            AosSkillBonuses skills = null;

            if (item is BaseWeapon)
                skills = ((BaseWeapon)item).SkillBonuses;

            if(item is BaseArmor)
                skills = ((BaseArmor)item).SkillBonuses;

            if(item is BaseHat)
                skills = ((BaseHat)item).SkillBonuses;

            if (item is BaseJewel)
            {
                skills = ((BaseJewel)item).SkillBonuses;
                mod = modification;
            }

            if (skills != null)
            {
                if (skills.GetBonus(0) > 0) { if (mod < 151 || mod > 155) { weight += ((double)(140.0 / 15.0) * (double)skills.GetBonus(0)); } }
                if (skills.GetBonus(1) > 0) { if (mod < 156 || mod > 160) { weight += ((double)(140.0 / 15.0) * (double)skills.GetBonus(1)); } }
                if (skills.GetBonus(2) > 0) { if (mod < 161 || mod > 166) { weight += ((double)(140.0 / 15.0) * (double)skills.GetBonus(2)); } }
                if (skills.GetBonus(3) > 0) { if (mod < 167 || mod > 173) { weight += ((double)(140.0 / 15.0) * (double)skills.GetBonus(3)); } }
                if (skills.GetBonus(4) > 0) { if (mod < 174 || mod > 180) { weight += ((double)(140.0 / 15.0) * (double)skills.GetBonus(4)); } }
            }

            return (int)Math.Round(weight);
        }

        private static int CheckResists(BaseArmor i, int mod)
        {
            double weight = 0;

            if (i.Quality != ArmorQuality.Exceptional)
            {
                if (i.PhysicalBonus > 0) { if (mod != 51) { weight += ((double)(100.0 / 15.0) * (double)i.PhysicalBonus); } }
                if (i.FireBonus > 0) { if (mod != 52) { weight += ((double)(100.0 / 15.0) * (double)i.FireBonus); } }
                if (i.ColdBonus > 0) { if (mod != 53) { weight += ((double)(100.0 / 15.0) * (double)i.ColdBonus); } }
                if (i.PoisonBonus > 0) { if (mod != 54) { weight += ((double)(100.0 / 15.0) * (double)i.PoisonBonus); } }
                if (i.EnergyBonus > 0) { if (mod != 55) { weight += ((double)(100.0 / 15.0) * (double)i.EnergyBonus); } }
            }
            else if (i.Quality == ArmorQuality.Exceptional)
            {
                if (i.PhysicalBonus > i.PhysImbuing) { if (mod != 51) { weight += ((double)(100.0 / 15.0) * (double)(i.PhysicalBonus - i.PhysImbuing)); } }
                if (i.FireBonus > i.FireImbuing) { if (mod != 52) { weight += ((double)(100.0 / 15.0) * (double)(i.FireBonus - i.FireImbuing)); } }
                if (i.ColdBonus > i.ColdImbuing) { if (mod != 53) { weight += ((double)(100.0 / 15.0) * (double)(i.ColdBonus - i.ColdImbuing)); } }
                if (i.PoisonBonus > i.PoisonImbuing) { if (mod != 54) { weight += ((double)(100.0 / 15.0) * (double)(i.PoisonBonus - i.PoisonImbuing)); } }
                if (i.EnergyBonus > i.EnergyImbuing) { if (mod != 55) { weight += ((double)(100.0 / 15.0) * (double)(i.EnergyBonus - i.EnergyImbuing)); } }
            }

            return (int)Math.Round(weight);
        }

        public static SkillName[] PossibleSkills { get { return m_PossibleSkills; } }
        private static SkillName[] m_PossibleSkills = new SkillName[]
			{
				SkillName.Swords,
				SkillName.Fencing,
				SkillName.Macing,
				SkillName.Archery,
				SkillName.Wrestling,
				SkillName.Parry,
				SkillName.Tactics,
				SkillName.Anatomy,
				SkillName.Healing,
				SkillName.Magery,
				SkillName.Meditation,
				SkillName.EvalInt,
				SkillName.MagicResist,
				SkillName.AnimalTaming,
				SkillName.AnimalLore,
				SkillName.Veterinary,
				SkillName.Musicianship,
				SkillName.Provocation,
				SkillName.Discordance,
				SkillName.Peacemaking,
				SkillName.Chivalry,
				SkillName.Focus,
				SkillName.Necromancy,
				SkillName.Stealing,
				SkillName.Stealth,
				SkillName.SpiritSpeak,
				SkillName.Bushido,
				SkillName.Ninjitsu,
                SkillName.Throwing,
                SkillName.Mysticism
			};

        private static SkillName[][] m_SkillGroups = new SkillName[][]
        {
            new SkillName[] { SkillName.Fencing, SkillName.Macing, SkillName.Swords, SkillName.Musicianship, SkillName.Magery },
            new SkillName[] { SkillName.Wrestling, SkillName.AnimalTaming, SkillName.SpiritSpeak, SkillName.Tactics, SkillName.Provocation },
            new SkillName[] { SkillName.Focus, SkillName.Parry, SkillName.Stealth, SkillName.Meditation, SkillName.AnimalLore, SkillName.Discordance },
            new SkillName[] { SkillName.Mysticism, SkillName.Bushido, SkillName.Necromancy, SkillName.Veterinary, SkillName.Stealing, SkillName.EvalInt, SkillName.Anatomy },
            new SkillName[] { SkillName.Peacemaking, SkillName.Ninjitsu, SkillName.Chivalry, SkillName.Archery, SkillName.MagicResist, SkillName.Healing, SkillName.Throwing }
        };

        // == SoulForge Check ==
        public static bool CheckSoulForge(Mobile from, int range)
        {
            return CheckSoulForge(from, range, true);
        }

        public static bool CheckSoulForge(Mobile from, int range, bool message)
        {
            PlayerMobile m = from as PlayerMobile;

            ImbuingContext context = Imbuing.GetContext(m);
            context.Imbue_SFBonus = 0;
            Map map = from.Map;

            if (map == null)
                return false;

            bool isForge = false;

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, range);

            foreach (Item item in eable)
            {
                if ((item.ItemID >= 0x4277 && item.ItemID <= 0x4286) ||
						(item.ItemID >= 0x4263 && item.ItemID <= 0x4272) ||
						(item.ItemID >= 17607 && item.ItemID <= 17610))
				{
					isForge = true;
					break;
				}
            }

            eable.Free();

			if (!isForge)
			{
				if (message)
					from.SendLocalizedMessage(1079787); // You must be near a soulforge to imbue an item.
				return false;
			}

			if (from.Region != null && from.Region.IsPartOf("Royal Soulforge"))
			{
				long level = ((PlayerMobile)from).Exp;

				if (level < PlayerMobile.Noble)
				{
					if (message)
						from.SendMessage("You must be of Noble loyalty to the Queen in order to use this forge.");
					return false;
				}
				else
					context.Imbue_SFBonus = 10;
			}
			else if (from.Region != null && from.Region.IsPartOf("Royal City"))
			{
				context.Imbue_SFBonus = 5;
			}

            return true;
        }

        private static Dictionary<int, ImbuingDefinition> m_Table;
        public static Dictionary<int, ImbuingDefinition> Table { get { return m_Table; } }

        public static void LoadImbuingDefinition()
        {
            m_Table = new Dictionary<int, ImbuingDefinition>();
			
			m_Table[1] = new ImbuingDefinition(AosAttribute.DefendChance, 	        1075620, 110,	typeof(RelicFragment), 	typeof(Tourmaline), 	typeof(EssenceSingularity), 15, 1, 1111947);
			m_Table[2] = new ImbuingDefinition(AosAttribute.AttackChance, 	        1075616, 130, 	typeof(EnchantEssence), typeof(Tourmaline), 	typeof(EssencePrecision), 	15, 1, 1111958);
			m_Table[3] = new ImbuingDefinition(AosAttribute.RegenHits,    	        1075627, 100, 	typeof(EnchantEssence), typeof(Tourmaline),     typeof(SeedRenewal), 	    2, 1, 1111994);
            m_Table[4] = new ImbuingDefinition(AosAttribute.RegenStam,    	        1079411, 100, 	typeof(EnchantEssence), typeof(Diamond), 		typeof(SeedRenewal), 	    3, 1, 1112043);
			m_Table[5] = new ImbuingDefinition(AosAttribute.RegenMana,      	    1079410, 100, 	typeof(EnchantEssence), typeof(Sapphire), 	    typeof(SeedRenewal), 	    2, 1, 1112003);
			m_Table[6] = new ImbuingDefinition(AosAttribute.BonusStr,     	        1079767, 110, 	typeof(EnchantEssence), typeof(Diamond), 		typeof(FireRuby), 		    8, 1, 1112044);
			m_Table[7] = new ImbuingDefinition(AosAttribute.BonusDex,     	        1079732, 110, 	typeof(EnchantEssence), typeof(Ruby), 		    typeof(BlueDiamond), 	    8, 1, 1111948);
			m_Table[8] = new ImbuingDefinition(AosAttribute.BonusInt,     	        1079756, 110, 	typeof(EnchantEssence), typeof(Tourmaline), 	typeof(Turquoise), 		    8, 1, 1111995);
			m_Table[9] = new ImbuingDefinition(AosAttribute.BonusHits,    	        1075630, 110, 	typeof(EnchantEssence), typeof(Ruby),			typeof(LuminescentFungi),   5, 1, 1111993);
			m_Table[10] = new ImbuingDefinition(AosAttribute.BonusStam,     	    1075632, 110, 	typeof(EnchantEssence), typeof(Diamond), 		typeof(LuminescentFungi),   8, 1, 1112042);
			m_Table[11] = new ImbuingDefinition(AosAttribute.BonusMana,   	        1075631, 110, 	typeof(EnchantEssence), typeof(Sapphire), 	    typeof(LuminescentFungi),   8, 1, 1112002);
			m_Table[12] = new ImbuingDefinition(AosAttribute.WeaponDamage,          1075619, 100, 	typeof(EnchantEssence), typeof(Citrine), 		typeof(CrystalShards), 	    50, 1, 1112005);
			m_Table[13] = new ImbuingDefinition(AosAttribute.WeaponSpeed, 	        1075629, 110, 	typeof(RelicFragment), 	typeof(Tourmaline), 	typeof(EssenceControl),     30, 5, 1112045);
			m_Table[14] = new ImbuingDefinition(AosAttribute.SpellDamage,           1075628, 100, 	typeof(EnchantEssence), typeof(Emerald), 		typeof(CrystalShards), 	    12, 1, 1112041);
			m_Table[15] = new ImbuingDefinition(AosAttribute.CastRecovery,          1075618, 120, 	typeof(RelicFragment), 	typeof(Amethyst), 	    typeof(EssenceDiligence),   3, 1, 1111952);
			m_Table[16] = new ImbuingDefinition(AosAttribute.CastSpeed,             1075617, 140, 	typeof(RelicFragment),  typeof(Ruby), 		    typeof(EssenceAchievement), 1, 1, 1111951);
			m_Table[17] = new ImbuingDefinition(AosAttribute.LowerManaCost,         1075621, 110,	typeof(RelicFragment),  typeof(Tourmaline), 	typeof(EssenceOrder), 	    8, 1, 1111996);
			m_Table[18] = new ImbuingDefinition(AosAttribute.LowerRegCost,          1075625, 100,	typeof(MagicalResidue), typeof(Amber), 		    typeof(FaeryDust), 	        20, 1, 1111997);
			m_Table[19] = new ImbuingDefinition(AosAttribute.ReflectPhysical,       1075626, 100, 	typeof(MagicalResidue), typeof(Citrine), 		typeof(ReflectiveWolfEye),  15, 1, 1112006);
			m_Table[20] = new ImbuingDefinition(AosAttribute.EnhancePotions,        1075624, 100, 	typeof(EnchantEssence), typeof(Citrine), 		typeof(CrushedGlass), 	    25, 5, 1111950);
			m_Table[21] = new ImbuingDefinition(AosAttribute.Luck, 			        1061153, 100, 	typeof(MagicalResidue), typeof(Citrine), 		typeof(ChagaMushroom), 	    100, 1, 1111999);
			m_Table[22] = new ImbuingDefinition(AosAttribute.SpellChanneling,       1079766, 100, 	typeof(MagicalResidue), typeof(Diamond), 		typeof(SilverSnakeSkin),    1, 0, 1112040);
			m_Table[23] = new ImbuingDefinition(AosAttribute.NightSight, 	        1015168, 50, 	typeof(MagicalResidue), typeof(Tourmaline), 	typeof(BottleIchor), 	    1, 0, 1112004);

			m_Table[24] = new ImbuingDefinition(AosWeaponAttribute.LowerStatReq,	1079757, 100, 	typeof(EnchantEssence), typeof(Amethyst), 	    typeof(ElvenFletchings),     100, 10, 1111998);
			m_Table[25] = new ImbuingDefinition(AosWeaponAttribute.HitLeechHits,  	1079698, 110, 	typeof(MagicalResidue), typeof(Ruby), 		    typeof(VoidOrb), 	        50, 2, 1111964);
			m_Table[26] = new ImbuingDefinition(AosWeaponAttribute.HitLeechStam,    1079707, 100, 	typeof(MagicalResidue), typeof(Diamond), 		typeof(VoidOrb), 	        50, 2, 1111992);
			m_Table[27] = new ImbuingDefinition(AosWeaponAttribute.HitLeechMana,    1079701, 110, 	typeof(MagicalResidue), typeof(Sapphire), 	    typeof(VoidOrb), 	        50, 2, 1111967);
			m_Table[28] = new ImbuingDefinition(AosWeaponAttribute.HitLowerAttack,  1079699, 110, 	typeof(EnchantEssence), typeof(Emerald),		typeof(ParasiticPlant),     50, 2, 1111965);
			m_Table[29] = new ImbuingDefinition(AosWeaponAttribute.HitLowerDefend,  1079700, 130, 	typeof(EnchantEssence), typeof(Tourmaline), 	typeof(ParasiticPlant),     50, 2, 1111966);
			m_Table[30] = new ImbuingDefinition(AosWeaponAttribute.HitPhysicalArea, 1079696, 100, 	typeof(MagicalResidue), typeof(Diamond), 		typeof(RaptorTeeth),        50, 2, 1111956);
			m_Table[31] = new ImbuingDefinition(AosWeaponAttribute.HitFireArea,  	1079695, 100, 	typeof(MagicalResidue), typeof(Ruby), 		    typeof(RaptorTeeth),        50, 2, 1111955);
			m_Table[32] = new ImbuingDefinition(AosWeaponAttribute.HitColdArea, 	1079693, 100, 	typeof(MagicalResidue), typeof(Sapphire), 	    typeof(RaptorTeeth),        50, 2, 1111953);
			m_Table[33] = new ImbuingDefinition(AosWeaponAttribute.HitPoisonArea,   1079697, 100, 	typeof(MagicalResidue), typeof(Emerald), 		typeof(RaptorTeeth),        50, 2, 1111957);
			m_Table[34] = new ImbuingDefinition(AosWeaponAttribute.HitEnergyArea,  	1079694, 100, 	typeof(MagicalResidue), typeof(Amethyst), 	    typeof(RaptorTeeth),        50, 2, 1111954);
			m_Table[35] = new ImbuingDefinition(AosWeaponAttribute.HitMagicArrow,   1079706, 120, 	typeof(RelicFragment),  typeof(Amber), 		    typeof(EssenceFeeling),     50, 2, 1111963);
			m_Table[36] = new ImbuingDefinition(AosWeaponAttribute.HitHarm, 		1079704, 110,	typeof(EnchantEssence), typeof(Emerald), 		typeof(ParasiticPlant),     50, 2, 1111961);
			m_Table[37] = new ImbuingDefinition(AosWeaponAttribute.HitFireball,  	1079703, 140,	typeof(EnchantEssence), typeof(Ruby), 		    typeof(FireRuby), 	        50, 2, 1111960);
			m_Table[38] = new ImbuingDefinition(AosWeaponAttribute.HitLightning,	1079705, 140, 	typeof(RelicFragment),  typeof(Amethyst), 	    typeof(EssencePassion),     50, 2, 1111962);
			m_Table[39] = new ImbuingDefinition(AosWeaponAttribute.HitDispel,		1079702, 100, 	typeof(MagicalResidue), typeof(Amber), 		    typeof(SlithTongue),        50, 2, 1111959);
			m_Table[40] = new ImbuingDefinition(AosWeaponAttribute.UseBestSkill, 	1079592, 150, 	typeof(EnchantEssence), typeof(Amber), 		    typeof(DelicateScales),     1, 0, 1111946);
			m_Table[41] = new ImbuingDefinition(AosWeaponAttribute.MageWeapon,		1079759, 100, 	typeof(EnchantEssence), typeof(Emerald), 		typeof(ArcanicRuneStone),   10, 1, 1112001);
			m_Table[42] = new ImbuingDefinition(AosWeaponAttribute.DurabilityBonus,	1017323, 100, 	typeof(EnchantEssence), typeof(Diamond), 		typeof(PowderedIron),       100, 10, 1112949);

            m_Table[49] = new ImbuingDefinition(AosArmorAttribute.MageArmor,        1079758, 0,   typeof(EnchantEssence), typeof(Diamond),        typeof(AbyssalCloth), 1, 0, 1112000);

            m_Table[51] = new ImbuingDefinition(AosElementAttribute.Physical,       1061158, 100,   typeof(MagicalResidue), typeof(Diamond),        typeof(BouraPelt), 15, 1, 1112010);
            m_Table[52] = new ImbuingDefinition(AosElementAttribute.Fire,           1061159, 100,   typeof(MagicalResidue), typeof(Ruby),           typeof(BouraPelt), 15, 1, 1112009);
            m_Table[53] = new ImbuingDefinition(AosElementAttribute.Cold,           1061160, 100,   typeof(MagicalResidue), typeof(Sapphire),       typeof(BouraPelt), 15, 1, 1112007);
            m_Table[54] = new ImbuingDefinition(AosElementAttribute.Poison,         1061161, 100,   typeof(MagicalResidue), typeof(Emerald),        typeof(BouraPelt), 15, 1, 1112011);
            m_Table[55] = new ImbuingDefinition(AosElementAttribute.Energy,         1061162, 100,   typeof(MagicalResidue), typeof(Amethyst),       typeof(BouraPelt), 15, 1, 1112008);
            
            m_Table[60] = new ImbuingDefinition("WeaponVelocity",                   1080416, 130, 	typeof(RelicFragment),  typeof(Tourmaline), 	typeof(EssenceDirection),   50, 2, 1112048);
			m_Table[61] = new ImbuingDefinition("BalancedWeapon",	                1072792, 150, 	typeof(RelicFragment),  typeof(Amber), 		    typeof(EssenceBalance),     1, 0, 1112047);

			m_Table[101] = new ImbuingDefinition(SlayerName.OrcSlaying,			1060470, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111977);
			m_Table[102] = new ImbuingDefinition(SlayerName.TrollSlaughter,  	1060480, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111990);
			m_Table[103] = new ImbuingDefinition(SlayerName.OgreTrashing,   	1060468, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111975);
			m_Table[104] = new ImbuingDefinition(SlayerName.DragonSlaying,   	1060462, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111970);
			m_Table[105] = new ImbuingDefinition(SlayerName.Terathan, 			1060478, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111989);
			m_Table[106] = new ImbuingDefinition(SlayerName.SnakesBane, 		1060475, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111980);
			m_Table[107] = new ImbuingDefinition(SlayerName.LizardmanSlaughter,	1060467, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111974);
			//m_Table[108] = new ImbuingDefinition(SlayerName.DaemonDismissal,  	 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl), 1, 0, 1112984);  //check
			m_Table[108] = new ImbuingDefinition(SlayerName.GargoylesFoe, 		1060466, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111973);
			//m_Table[110] = new ImbuingDefinition(SlayerName.BalronDamnation,   	 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl), 1, 0, 1112001);  //check
			m_Table[111] = new ImbuingDefinition(SlayerName.Ophidian,  			1060469, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111976);
			m_Table[112] = new ImbuingDefinition(SlayerName.SpidersDeath,   	1060477, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111982);
			m_Table[113] = new ImbuingDefinition(SlayerName.ScorpionsBane, 		1060474, 100,	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111979);
			m_Table[114] = new ImbuingDefinition(SlayerName.FlameDousing,  		1060465, 100,	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111972);
			m_Table[115] = new ImbuingDefinition(SlayerName.WaterDissipation,	1060481, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111991);
			m_Table[116] = new ImbuingDefinition(SlayerName.Vacuum,		        1060457, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111968);
			m_Table[117] = new ImbuingDefinition(SlayerName.ElementalHealth, 	1060471, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111978);
			m_Table[118] = new ImbuingDefinition(SlayerName.EarthShatter,		1060463, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111971);
			m_Table[119] = new ImbuingDefinition(SlayerName.BloodDrinking,		1060459, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111969);
			m_Table[120] = new ImbuingDefinition(SlayerName.SummerWind,			1060476, 100, 	typeof(MagicalResidue), typeof(Emerald),            typeof(WhitePearl),         1, 0, 1111981);
		
			m_Table[121] = new ImbuingDefinition(SlayerName.Silver,			    1060479, 130, 	typeof(RelicFragment),      typeof(Ruby), 		    typeof(UndyingFlesh), 	    1, 0, 1111988);
			m_Table[122] = new ImbuingDefinition(SlayerName.Repond,			    1060472, 130, 	typeof(RelicFragment),      typeof(Ruby), 		    typeof(GoblinBlood), 	    1, 0, 1111986);
            m_Table[123] = new ImbuingDefinition(SlayerName.ReptilianDeath,     1060473, 130,   typeof(RelicFragment),      typeof(Ruby),           typeof(LavaSerpentCrust),   1, 0, 1111987);
			m_Table[124] = new ImbuingDefinition(SlayerName.Exorcism,		    1060460, 130, 	typeof(RelicFragment),      typeof(Ruby), 		    typeof(DaemonClaw), 	    1, 0, 1111984);
			m_Table[125] = new ImbuingDefinition(SlayerName.ArachnidDoom,	    1060458, 130, 	typeof(RelicFragment),      typeof(Ruby), 		    typeof(SpiderCarapace),     1, 0, 1111983);
			m_Table[126] = new ImbuingDefinition(SlayerName.ElementalBan,	    1060464, 130, 	typeof(RelicFragment),      typeof(Ruby), 		    typeof(VialOfVitriol), 	    1, 0, 1111985);
            m_Table[127] = new ImbuingDefinition(SlayerName.Fey,                1070855, 130,   typeof(RelicFragment),      typeof(Ruby),           typeof(FeyWings),           1, 0, 1154652);

            m_Table[151] = new ImbuingDefinition(SkillName.Fencing,		 	    1044102, 140, 	typeof(EnchantEssence), 	typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112012);
			m_Table[152] = new ImbuingDefinition(SkillName.Macing, 	            1044101, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112013);
			m_Table[153] = new ImbuingDefinition(SkillName.Swords,	            1044100, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112016);
			m_Table[154] = new ImbuingDefinition(SkillName.Musicianship,	    1044089, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112015);
			m_Table[155] = new ImbuingDefinition(SkillName.Magery,			    1044085, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112014);
			
			m_Table[156] = new ImbuingDefinition(SkillName.Wrestling,		    1044103, 140, 	typeof(EnchantEssence), 	typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112021);
			m_Table[157] = new ImbuingDefinition(SkillName.AnimalTaming, 	    1044095, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112017);
			m_Table[158] = new ImbuingDefinition(SkillName.SpiritSpeak,		    1044092, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112019);
			m_Table[159] = new ImbuingDefinition(SkillName.Tactics,			    1044087, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112020);
			m_Table[160] = new ImbuingDefinition(SkillName.Provocation,		    1044082, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112018);
			
			m_Table[161] = new ImbuingDefinition(SkillName.Focus,			    1044110, 140, 	typeof(EnchantEssence),	typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112024);
			m_Table[162] = new ImbuingDefinition(SkillName.Parry, 		        1044065, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112026);
			m_Table[163] = new ImbuingDefinition(SkillName.Stealth,			    1044107, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112027);
			m_Table[164] = new ImbuingDefinition(SkillName.Meditation,		    1044106, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112025);
			m_Table[165] = new ImbuingDefinition(SkillName.AnimalLore,		    1044062, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112022);
			m_Table[166] = new ImbuingDefinition(SkillName.Discordance,		    1044075, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112023);
			
            m_Table[167] = new ImbuingDefinition(SkillName.Mysticism,			1044115, 140, 	typeof(EnchantEssence),	typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1115213);
			m_Table[168] = new ImbuingDefinition(SkillName.Bushido,			    1044112, 140, 	typeof(EnchantEssence),	typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112029);
			m_Table[169] = new ImbuingDefinition(SkillName.Necromancy, 		    1044109, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112031);
			m_Table[170] = new ImbuingDefinition(SkillName.Veterinary,		    1044099, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112033);
			m_Table[171] = new ImbuingDefinition(SkillName.Stealing,		    1044093, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112032);
			m_Table[172] = new ImbuingDefinition(SkillName.EvalInt, 		    1044076, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112030);
			m_Table[173] = new ImbuingDefinition(SkillName.Anatomy,			    1044061, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112028);
			
			m_Table[174] = new ImbuingDefinition(SkillName.Peacemaking,		    1044069, 140, 	typeof(EnchantEssence), 	typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112038);
			m_Table[175] = new ImbuingDefinition(SkillName.Ninjitsu, 		    1044113, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112037);
			m_Table[176] = new ImbuingDefinition(SkillName.Chivalry,		    1044111, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112035);
			m_Table[177] = new ImbuingDefinition(SkillName.Archery,			    1044091, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112034);
			m_Table[178] = new ImbuingDefinition(SkillName.MagicResist,	        1044086, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112039);
			m_Table[179] = new ImbuingDefinition(SkillName.Healing,			    1044077, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1112036);
            m_Table[180] = new ImbuingDefinition(SkillName.Throwing,			1044117, 140, 	typeof(EnchantEssence),   typeof(StarSapphire), 	typeof(CrystallineBlackrock),  15, 1, 1115212); 
            
            // Non-Imbuables for getting item intensity only
            m_Table[200] = new ImbuingDefinition(AosWeaponAttribute.BloodDrinker,           1017407, 140, null, null, null, 1, 1, 1152387);
            m_Table[201] = new ImbuingDefinition(AosWeaponAttribute.BattleLust,             1113710, 140, null, null, null, 1, 1, 1152385);
            m_Table[202] = new ImbuingDefinition(AosWeaponAttribute.HitCurse,               1154673, 140, null, null, null, 50, 1, 1152438);
            m_Table[203] = new ImbuingDefinition(AosWeaponAttribute.HitFatigue,             1154668, 140, null, null, null, 50, 1, 1152437);
            m_Table[204] = new ImbuingDefinition(AosWeaponAttribute.HitManaDrain,           1154669, 140, null, null, null, 50, 1, 1152436);
            m_Table[205] = new ImbuingDefinition(AosWeaponAttribute.SplinteringWeapon,      1154670, 140, null, null, null, 20, 1, 1152396);
            m_Table[206] = new ImbuingDefinition(AosWeaponAttribute.ReactiveParalyze,       1154660, 140, null, null, null, 1, 1, 1152400);

            m_Table[233] = new ImbuingDefinition(AosWeaponAttribute.ResistPhysicalBonus,    1061158, 100, typeof(MagicalResidue), typeof(Diamond),  typeof(BouraPelt), 15, 1, 1112010);
            m_Table[234] = new ImbuingDefinition(AosWeaponAttribute.ResistFireBonus,        1061159, 100, typeof(MagicalResidue), typeof(Ruby),     typeof(BouraPelt), 15, 1, 1112009);
            m_Table[235] = new ImbuingDefinition(AosWeaponAttribute.ResistColdBonus,        1061160, 100, typeof(MagicalResidue), typeof(Sapphire), typeof(BouraPelt), 15, 1, 1112007);
            m_Table[236] = new ImbuingDefinition(AosWeaponAttribute.ResistPoisonBonus,      1061161, 100, typeof(MagicalResidue), typeof(Emerald),  typeof(BouraPelt), 15, 1, 1112011);
            m_Table[237] = new ImbuingDefinition(AosWeaponAttribute.ResistEnergyBonus,      1061162, 100, typeof(MagicalResidue), typeof(Amethyst), typeof(BouraPelt), 15, 1, 1112008); 

            m_Table[208] = new ImbuingDefinition(SAAbsorptionAttribute.EaterFire,           1154662, 140, null, null, null, 10, 1, 1152390);
            m_Table[209] = new ImbuingDefinition(SAAbsorptionAttribute.EaterCold,           1154663, 140, null, null, null, 10, 1, 1152390);
            m_Table[210] = new ImbuingDefinition(SAAbsorptionAttribute.EaterPoison,         1154664, 140, null, null, null, 10, 1, 1152390);
            m_Table[211] = new ImbuingDefinition(SAAbsorptionAttribute.EaterEnergy,         1154665, 140, null, null, null, 10, 1, 1152390);
            m_Table[212] = new ImbuingDefinition(SAAbsorptionAttribute.EaterKinetic,        1154666, 140, null, null, null, 10, 1, 1152390);
            m_Table[213] = new ImbuingDefinition(SAAbsorptionAttribute.EaterDamage,         1154667, 140, null, null, null, 10, 1, 1152390);
            m_Table[214] = new ImbuingDefinition(SAAbsorptionAttribute.ResonanceFire,       1154655, 140, null, null, null, 20, 1, 1152391);
            m_Table[215] = new ImbuingDefinition(SAAbsorptionAttribute.ResonanceCold,       1154656, 140, null, null, null, 20, 1, 1152391);
            m_Table[216] = new ImbuingDefinition(SAAbsorptionAttribute.ResonancePoison,     1154657, 140, null, null, null, 20, 1, 1152391);
            m_Table[217] = new ImbuingDefinition(SAAbsorptionAttribute.ResonanceEnergy,     1154658, 140, null, null, null, 20, 1, 1152391);
            m_Table[218] = new ImbuingDefinition(SAAbsorptionAttribute.ResonanceKinetic,    1154659, 140, null, null, null, 20, 1, 1152391);

            m_Table[218] = new ImbuingDefinition(SAAbsorptionAttribute.SoulChargeFire,      1154659, 140, null, null, null, 20, 1, 1152391);
            m_Table[218] = new ImbuingDefinition(SAAbsorptionAttribute.SoulChargeCold,      1154659, 140, null, null, null, 20, 1, 1152391);
            m_Table[218] = new ImbuingDefinition(SAAbsorptionAttribute.SoulChargePoison,    1154659, 140, null, null, null, 20, 1, 1152391);
            m_Table[218] = new ImbuingDefinition(SAAbsorptionAttribute.SoulChargeEnergy,    1154659, 140, null, null, null, 20, 1, 1152391);
            m_Table[218] = new ImbuingDefinition(SAAbsorptionAttribute.SoulChargeKinetic,   1154659, 140, null, null, null, 20, 1, 1152391);
            m_Table[218] = new ImbuingDefinition(SAAbsorptionAttribute.CastingFocus,        1154659, 140, null, null, null, 20, 1, 1116535);

            m_Table[219] = new ImbuingDefinition(AosArmorAttribute.ReactiveParalyze,        1154660, 140, null, null, null, 1, 1,  1152400);
            m_Table[220] = new ImbuingDefinition(AosArmorAttribute.SoulCharge,              1116536, 140, null, null, null, 20, 1, 1152391);
            //243 already used above
        }

        private static bool IsInNonImbueList(Type itemType)
        {
            foreach (Type type in m_CannotImbue)
            {
                if (type == itemType)
                    return true;
            }

            return false;
        }

        private static Type[] m_CannotImbue = new Type[]
        {
            typeof(GargishPlateWingArmor), typeof(GargishLeatherWingArmor), typeof(GargishClothWingArmor), typeof(GargishStoneWingArmor)
        };

        public static object GetAttribute(int mod)
        {
            if (m_Table.ContainsKey(mod))
                return m_Table[mod].Attribute;

            return null;
        }

        public static int GetValueForMod(Item item, int mod)
        {
            object attr = GetAttribute(mod);

            if (item is BaseWeapon)
            {
                BaseWeapon w = (BaseWeapon)item;

                if (mod == 16 && w.Attributes.SpellChanneling > 0)
                    return w.Attributes[AosAttribute.CastSpeed] + 1;

                if (attr is AosAttribute)
                    return w.Attributes[(AosAttribute)attr];

                else if (attr is AosWeaponAttribute)
                    return w.WeaponAttributes[(AosWeaponAttribute)attr];

                else if (attr is SAAbsorptionAttribute)
                    return w.AbsorptionAttributes[(SAAbsorptionAttribute)attr];

                else if (attr is SlayerName && w.Slayer == (SlayerName)attr)
                    return 1;

                else if (attr is AosElementAttribute)
                {
                    AosElementAttribute ele = (AosElementAttribute)attr;

                    switch (ele)
                    {
                        case AosElementAttribute.Physical: return w.WeaponAttributes.ResistPhysicalBonus;
                        case AosElementAttribute.Fire: return w.WeaponAttributes.ResistFireBonus;
                        case AosElementAttribute.Cold: return w.WeaponAttributes.ResistColdBonus;
                        case AosElementAttribute.Poison: return w.WeaponAttributes.ResistPoisonBonus;
                        case AosElementAttribute.Energy: return w.WeaponAttributes.ResistEnergyBonus;
                    }
                }
            }
            else if (item is BaseArmor)
            {
                BaseArmor a = (BaseArmor)item;

                if (a is BaseShield && mod == 16 && a.Attributes.SpellChanneling > 0)
                    return a.Attributes[AosAttribute.CastSpeed] + 1;

                if (attr is AosAttribute)
                    return a.Attributes[(AosAttribute)attr];

                else if (attr is AosArmorAttribute)
                    return a.ArmorAttributes[(AosArmorAttribute)attr];

                else if (attr is SAAbsorptionAttribute)
                    return a.AbsorptionAttributes[(SAAbsorptionAttribute)attr];

                else if (attr is AosElementAttribute)
                {
                    AosElementAttribute ele = (AosElementAttribute)attr;

                    switch (ele)
                    {
                        case AosElementAttribute.Physical: return a.PhysicalBonus;
                        case AosElementAttribute.Fire: return a.FireBonus;
                        case AosElementAttribute.Cold: return a.ColdBonus;
                        case AosElementAttribute.Poison: return a.PoisonBonus;
                        case AosElementAttribute.Energy: return a.EnergyBonus;
                    }
                }
            }
            else if (item is BaseClothing)
            {
                BaseClothing c = (BaseClothing)item;

                if (attr is AosAttribute)
                    return c.Attributes[(AosAttribute)attr];

                else if (attr is AosElementAttribute)
                    return c.Resistances[(AosElementAttribute)attr];

                else if (attr is AosArmorAttribute)
                    return c.ClothingAttributes[(AosArmorAttribute)attr];
            }
            else if (item is BaseJewel)
            {
                BaseJewel j = (BaseJewel)item;

                if (attr is AosAttribute)
                    return j.Attributes[(AosAttribute)attr];

                else if (attr is AosElementAttribute)
                    return j.Resistances[(AosElementAttribute)attr];

                else if (attr is SAAbsorptionAttribute)
                    return j.AbsorptionAttributes[(SAAbsorptionAttribute)attr];

                else if (attr is SkillName)
                {
                    SkillName sk = (SkillName)attr;

                    if (j.SkillBonuses.Skill_1_Name == sk)
                        return (int)j.SkillBonuses.Skill_1_Value;

                    if (j.SkillBonuses.Skill_2_Name == sk)
                        return (int)j.SkillBonuses.Skill_2_Value;

                    if (j.SkillBonuses.Skill_3_Name == sk)
                        return (int)j.SkillBonuses.Skill_3_Value;

                    if (j.SkillBonuses.Skill_4_Name == sk)
                        return (int)j.SkillBonuses.Skill_4_Value;

                    if (j.SkillBonuses.Skill_5_Name == sk)
                        return (int)j.SkillBonuses.Skill_5_Value;
                }
            }

            return 0;
        }

        public static int GetIntensityForAttribute(object attr, int checkMod, int value)
        {
            if (value <= 0)
                return 0;

            int mod = -1;

            if (attr is AosAttribute)
                mod = GetModForAttribute((AosAttribute)attr);

            else if (attr is AosWeaponAttribute)
                mod = GetModForAttribute((AosWeaponAttribute)attr);

            else if (attr is SkillName)
                mod = GetModForAttribute((SkillName)attr);

            else if (attr is SlayerName)
                mod = GetModForAttribute((SlayerName)attr);

            else if (attr is SAAbsorptionAttribute)
                mod = GetModForAttribute((SAAbsorptionAttribute)attr);

            else if (attr is AosArmorAttribute)
                mod = GetModForAttribute((AosArmorAttribute)attr);

            else if (attr is AosElementAttribute)
                mod = GetModForAttribute((AosElementAttribute)attr);

            else if (attr is string)
                mod = GetModForAttribute((string)attr);

            if (mod != checkMod && m_Table.ContainsKey(mod))
                return (int)(((double)m_Table[mod].Weight / (double)m_Table[mod].MaxIntensity) * (double)value);

            return 0;
        }

        public static int GetModForAttribute(AosAttribute attr)
        {
            foreach (KeyValuePair<int, ImbuingDefinition> kvp in m_Table)
            {
                int mod = kvp.Key;
                ImbuingDefinition def = kvp.Value;

                if (def.Attribute is AosAttribute && (AosAttribute)def.Attribute == attr)
                    return mod;
            }

            return -1;
        }

        public static int GetModForAttribute(AosWeaponAttribute attr)
        {
            foreach (KeyValuePair<int, ImbuingDefinition> kvp in m_Table)
            {
                int mod = kvp.Key;
                ImbuingDefinition def = kvp.Value;

                if (def.Attribute is AosWeaponAttribute && (AosWeaponAttribute)def.Attribute == attr)
                    return mod;
            }

            return -1;
        }

        public static int GetModForAttribute(SAAbsorptionAttribute attr)
        {
            foreach (KeyValuePair<int, ImbuingDefinition> kvp in m_Table)
            {
                int mod = kvp.Key;
                ImbuingDefinition def = kvp.Value;

                if (def.Attribute is SAAbsorptionAttribute && (SAAbsorptionAttribute)def.Attribute == attr)
                    return mod;
            }

            return -1;
        }

        public static int GetModForAttribute(AosArmorAttribute attr)
        {
            if (attr == AosArmorAttribute.LowerStatReq)
                return GetModForAttribute(AosWeaponAttribute.LowerStatReq);

            if (attr == AosArmorAttribute.DurabilityBonus)
                return GetModForAttribute(AosWeaponAttribute.DurabilityBonus);

            foreach (KeyValuePair<int, ImbuingDefinition> kvp in m_Table)
            {
                int mod = kvp.Key;
                ImbuingDefinition def = kvp.Value;

                if (def.Attribute is AosArmorAttribute && (AosArmorAttribute)def.Attribute == attr)
                    return mod;
            }

            return -1;
        }

        public static int GetModForAttribute(SkillName attr)
        {
            foreach (KeyValuePair<int, ImbuingDefinition> kvp in m_Table)
            {
                int mod = kvp.Key;
                ImbuingDefinition def = kvp.Value;

                if (def.Attribute is SkillName && (SkillName)def.Attribute == attr)
                    return mod;
            }

            return -1;
        }

        public static int GetModForAttribute(SlayerName attr)
        {
            foreach (KeyValuePair<int, ImbuingDefinition> kvp in m_Table)
            {
                int mod = kvp.Key;
                ImbuingDefinition def = kvp.Value;

                if (def.Attribute is SlayerName && (SlayerName)def.Attribute == attr)
                    return mod;
            }

            return -1;
        }

        public static int GetModForAttribute(AosElementAttribute type)
        {
            switch (type)
            {
                case AosElementAttribute.Physical: return 51;
                case AosElementAttribute.Fire: return 52;
                case AosElementAttribute.Cold: return 53;
                case AosElementAttribute.Poison: return 54;
                case AosElementAttribute.Energy: return 55;
            }

            return -1;
        }

        public static int GetModForAttribute(string str)
        {
            if (str == "BalancedWeapon")
                return 61;

            if (str == "WeaponVelocity")
                return 60;

            return -1;
        }

        #region Prop Ranges
        public static int[] GetPropRange(AosAttribute attr)
        {
            switch (attr)
            {
                case AosAttribute.Luck: return new int[] { 1, 100 };
                case AosAttribute.WeaponDamage: return new int[] { 2, 50 };
                case AosAttribute.WeaponSpeed: return new int[] { 5, 30 };
                case AosAttribute.LowerRegCost: return new int[] { 1, 20 };
                case AosAttribute.EnhancePotions: return new int[] { 5, 25 };
                case AosAttribute.AttackChance:
                case AosAttribute.DefendChance:
                case AosAttribute.ReflectPhysical: return new int[] { 1, 15 };
                case AosAttribute.SpellDamage: return new int[] { 1, 12 };
                case AosAttribute.BonusStr:
                case AosAttribute.BonusInt:
                case AosAttribute.BonusDex:
                case AosAttribute.BonusStam:
                case AosAttribute.BonusMana:
                case AosAttribute.LowerManaCost: return new int[] { 1, 8 };
                case AosAttribute.BonusHits: return new int[] { 1, 5 };
                case AosAttribute.CastRecovery:
                case AosAttribute.RegenStam: return new int[] { 1, 3 };
                case AosAttribute.RegenHits:
                case AosAttribute.RegenMana: return new int[] { 1, 2 };
                default:
                case AosAttribute.SpellChanneling:
                case AosAttribute.CastSpeed:
                case AosAttribute.Brittle:
                case AosAttribute.NightSight: return new int[] { 1, 1 };
            }
        }

        public static int[] GetPropRange(AosWeaponAttribute attr)
        {
            switch (attr)
            {
                case AosWeaponAttribute.DurabilityBonus:
                case AosWeaponAttribute.LowerStatReq: return new int[] { 10, 100 };
                case AosWeaponAttribute.HitLeechHits:
                case AosWeaponAttribute.HitLeechMana:
                case AosWeaponAttribute.HitLeechStam:
                case AosWeaponAttribute.HitLowerAttack:
                case AosWeaponAttribute.HitLowerDefend:
                case AosWeaponAttribute.HitMagicArrow:
                case AosWeaponAttribute.HitHarm:
                case AosWeaponAttribute.HitLightning:
                case AosWeaponAttribute.HitFireball:
                case AosWeaponAttribute.HitDispel:
                case AosWeaponAttribute.HitColdArea:
                case AosWeaponAttribute.HitFireArea:
                case AosWeaponAttribute.HitPhysicalArea:
                case AosWeaponAttribute.HitPoisonArea:
                case AosWeaponAttribute.HitCurse:
                case AosWeaponAttribute.HitFatigue:
                case AosWeaponAttribute.HitManaDrain: return new int[] { 2, 50 };
                case AosWeaponAttribute.HitEnergyArea:
                case AosWeaponAttribute.ResistFireBonus:
                case AosWeaponAttribute.ResistColdBonus:
                case AosWeaponAttribute.ResistPoisonBonus:
                case AosWeaponAttribute.ResistEnergyBonus:
                case AosWeaponAttribute.ResistPhysicalBonus: return new int[] { 1, 15 };
                case AosWeaponAttribute.MageWeapon: return new int[] { 1, 10 };
                case AosWeaponAttribute.SelfRepair: return new int[] { 1, 5 };
                case AosWeaponAttribute.SplinteringWeapon: return new int[] { 5, 30 };
                default:
                case AosWeaponAttribute.UseBestSkill:
                case AosWeaponAttribute.BattleLust:
                case AosWeaponAttribute.BloodDrinker: return new int[] { 1, 1 };
            }
        }

        public static int[] GetPropRange(AosArmorAttribute attr)
        {
            switch (attr)
            {
                case AosArmorAttribute.LowerStatReq:
                case AosArmorAttribute.DurabilityBonus: return new int[] { 10, 100 };
                case AosArmorAttribute.SoulCharge: return new int[] { 1, 10 };
                default:
                case AosArmorAttribute.ReactiveParalyze:
                case AosArmorAttribute.SelfRepair:
                case AosArmorAttribute.MageArmor: return new int[] { 1, 1 };
            }
        }

        public static int[] GetPropRange(SAAbsorptionAttribute attr)
        {
            switch (attr)
            {
                default:
                case SAAbsorptionAttribute.EaterFire:
                case SAAbsorptionAttribute.EaterCold:
                case SAAbsorptionAttribute.EaterPoison:
                case SAAbsorptionAttribute.EaterEnergy:
                case SAAbsorptionAttribute.EaterKinetic:
                case SAAbsorptionAttribute.EaterDamage:
                case SAAbsorptionAttribute.ResonanceFire:
                case SAAbsorptionAttribute.ResonanceCold:
                case SAAbsorptionAttribute.ResonancePoison:
                case SAAbsorptionAttribute.ResonanceEnergy:
                case SAAbsorptionAttribute.ResonanceKinetic:
                case SAAbsorptionAttribute.SoulChargeFire:
                case SAAbsorptionAttribute.SoulChargeCold:
                case SAAbsorptionAttribute.SoulChargePoison:
                case SAAbsorptionAttribute.SoulChargeEnergy:
                case SAAbsorptionAttribute.SoulChargeKinetic:
                case SAAbsorptionAttribute.CastingFocus: return new int[] { 1, 10 };
            }
        }

        public static int[] GetPropRange(AosElementAttribute attr)
        {
            return new int[] { 1, 15 };
        }
        #endregion
    }
}        
