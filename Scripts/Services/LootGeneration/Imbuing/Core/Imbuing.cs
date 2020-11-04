using Server.Commands;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.SkillHandlers
{
    public class Imbuing
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Imbuing].Callback = OnUse;

            CommandSystem.Register("GetTotalWeight", AccessLevel.GameMaster, GetTotalWeight_OnCommand);
            CommandSystem.Register("GetTotalMods", AccessLevel.GameMaster, GetTotalMods_OnCommand);
        }

        private static void OnLogin(LoginEventArgs e)
        {
            if (!e.Mobile.CanBeginAction(typeof(Imbuing)))
                e.Mobile.EndAction(typeof(Imbuing));
        }

        private static readonly Dictionary<Mobile, ImbuingContext> m_ContextTable = new Dictionary<Mobile, ImbuingContext>();
        public static Dictionary<Mobile, ImbuingContext> ContextTable => m_ContextTable;

        public static TimeSpan OnUse(Mobile from)
        {
            if (!from.Alive)
            {
                from.SendLocalizedMessage(500949); //You can't do that when you're dead.
            }
            else if (from is PlayerMobile)
            {
                from.CloseGump(typeof(ImbuingGump));
                BaseGump.SendGump(new ImbuingGump((PlayerMobile)from));
                from.BeginAction(typeof(Imbuing));
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
            if (!CheckSoulForge(from, 2))
            {
                return false;
            }
            if (item == null || !item.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1079575);  // The item must be in your backpack to imbue it.
            }
            else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
            {
                from.SendLocalizedMessage(1080438);  // You cannot imbue a blessed item.
            }
            else if (item is BaseWeapon && Spells.Mysticism.EnchantSpell.IsUnderSpellEffects(from, (BaseWeapon)item))
            {
                from.SendLocalizedMessage(1080130);  // You cannot imbue an item that is currently enchanted.
            }
            else if (item is BaseWeapon && ((BaseWeapon)item).FocusWeilder != null)
            {
                from.SendLocalizedMessage(1080444);  //You cannot imbue an item that is under the effects of the ninjitsu focus attack ability.
            }
            else if (IsSpecialItem(item))
            {
                from.SendLocalizedMessage(1079576); // You cannot imbue this item.
            }
            else if (item.GetType() == typeof(JukaBow))
            {
                if (((JukaBow)item).IsModified)
                    from.SendLocalizedMessage(1079576); // You cannot imbue this item.
                else
                    return true;
            }
            else if (item is BaseJewel && !(item is BaseRing) && !(item is BaseBracelet))
            {
                from.SendLocalizedMessage(1079576); // You cannot imbue this item.
            }
            else if (IsInNonImbueList(item.GetType()))
            {
                from.SendLocalizedMessage(1079576); // You cannot imbue this item.
            }
            else
            {
                return true;
            }

            return false;
        }

        public static bool OnBeforeImbue(Mobile from, Item item, int id, int value)
        {
            return OnBeforeImbue(from, item, id, value, GetTotalMods(item, id), GetMaxProps(item), GetTotalWeight(item, id, false, true), GetMaxWeight(item));
        }

        public static bool OnBeforeImbue(Mobile from, Item item, int id, int value, int totalprops, int maxprops, int totalitemweight, int maxweight)
        {
            if (totalprops >= maxprops || totalitemweight > maxweight)
            {
                from.SendLocalizedMessage(1079772); // You cannot imbue this item with any more item properties.
                from.CloseGump(typeof(ImbueGump));
                from.EndAction(typeof(Imbuing));
                return false;
            }

            return true;
        }

        public static bool CanUnravelItem(Mobile from, Item item, bool message = true)
        {
            if (!CheckSoulForge(from, 2, false, false))
            {
                from.SendLocalizedMessage(1080433); // You must be near a soulforge to magically unravel an item.
            }
            else if (!item.IsChildOf(from.Backpack))
            {
                if (message)
                    from.SendLocalizedMessage(1080424);  // The item must be in your backpack to magically unravel it.
            }
            else if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied)
            {
                if (message)
                    from.SendLocalizedMessage(1080421);  // You cannot unravel the magic of a blessed item.
            }
            else if (!(item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseHat))
            {
                if (message)
                    from.SendLocalizedMessage(1080425); // You cannot magically unravel this item.
            }
            else if (item is BaseWeapon && Spells.Mysticism.EnchantSpell.IsUnderSpellEffects(from, (BaseWeapon)item))
            {
                if (message)
                    from.SendLocalizedMessage(1080427);  // You cannot magically unravel an item that is currently enchanted.
            }
            else if (item is BaseWeapon && ((BaseWeapon)item).FocusWeilder != null)
            {
                if (message)
                    from.SendLocalizedMessage(1080445); //You cannot magically unravel an item that is under the effects of the ninjitsu focus attack ability.
            }
            else
            {
                return true;
            }

            return false;
        }

        public static bool IsSpecialItem(Item item)
        {
            if (item == null)
                return true;

            if (IsSpecialImbuable(item))
                return false;

            if (item.IsArtifact)
                return true;

            if (RunicReforging.GetArtifactRarity(item) > 0)
                return true;

            if (NonCraftableImbuable(item))
                return false;

            if (CraftItem.GetCraftItem(item) != null)
                return false;

            return true;
        }

        public static bool IsSpecialImbuable(Item item)
        {
            return IsSpecialImbuable(item.GetType());
        }

        public static bool IsSpecialImbuable(Type type)
        {
            if (_SpecialImbuable.Any(i => i == type))
                return true;

            if (type.IsSubclassOf(typeof(BaseGlovesOfMining)) || typeof(IFishingAttire).IsAssignableFrom(type))
                return true;

            return false;
        }

        private static readonly Type[] _SpecialImbuable =
       {
            typeof(ClockworkLeggings), typeof(GargishClockworkLeggings), typeof(OrcishKinMask), typeof(SavageMask), typeof(VirtuososArmbands),
            typeof(VirtuososCap), typeof(VirtuososCollar), typeof(VirtuososEarpieces), typeof(VirtuososKidGloves), typeof(VirtuososKilt),
            typeof(VirtuososNecklace), typeof(VirtuososTunic), typeof(BestialArms), typeof(BestialEarrings), typeof(BestialGloves), typeof(BestialGorget),
            typeof(BestialHelm), typeof(BestialKilt), typeof(BestialLegs), typeof(BestialNecklace), typeof(BarbedWhip), typeof(BladedWhip),
            typeof(SpikedWhip), typeof(SkullGnarledStaff), typeof(GargishSkullGnarledStaff), typeof(SkullLongsword), typeof(GargishSkullLongsword), typeof(JukaBow),
            typeof(SlayerLongbow), typeof(JackOLanternHelm)
        };

        private static readonly Type[] _NonCraftables =
        {
            typeof(SilverRing), typeof(SilverBracelet)
        };

        public static bool NonCraftableImbuable(Item item)
        {
            if (item is BaseWand)
                return true;

            Type type = item.GetType();

            foreach (Type t in _NonCraftables)
            {
                if (t == type)
                    return true;
            }

            return false;
        }

        public static double GetSuccessChance(Mobile from, Item item, int totalItemIntensity, int propintensity, double bonus)
        {
            double skill = from.Skills[SkillName.Imbuing].Value;
            double resultWeight = totalItemIntensity + propintensity;

            double e = Math.E;
            double a, b, c, w;
            double i = item is BaseJewel ? 0.9162 : 1.0;

            // - Racial Bonus - SA ONLY -
            if (from.Race == Race.Gargoyle)
            {
                a = 1362.281555;
                b = 66.32801518;
                c = 235.2223147;
                w = -1481.037561;
            }
            else
            {
                a = 1554.96118;
                b = 53.81743328;
                c = 230.0038452;
                w = -1664.857794;
            }

            // Royal City Bonus, Fluctuation - Removed as EA doesn't seem to fluctuate despite stratics posts
            /*if (bonus == 0.02)
            {
                if (totalItemIntensity < 295)
                {
                    bonus = (double)Utility.RandomMinMax(190, 210) / 100.0;
                }
            }*/

            return Math.Max(0, Math.Round(Math.Floor(20 * skill + 10 * a * Math.Pow(e, (b / (resultWeight + c))) + 10 * w - 2400) / 1000 * (i) + bonus, 3) * 100);
        }

        public static int GetQualityBonus(Item item)
        {
            IQuality quality = item as IQuality;

            if (quality != null)
            {
                if (quality.Quality == ItemQuality.Exceptional)
                    return 20;

                if (quality.PlayerConstructed)
                    return 10;
            }

            return 0;
        }

        /// <summary>
        /// Imbues Item with selected id
        /// </summary>
        /// <param name="from">Player Imbuing</param>
        /// <param name="i">Item to be imbued</param>
        /// <param name="id">id to be imbued, see m_Table</param>
        /// <param name="value">value for id</param>
        public static void TryImbueItem(Mobile from, Item i, int id, int value)
        {
            double bonus = 0.0;

            if (!CheckSoulForge(from, 2, out bonus))
                return;

            ImbuingContext context = GetContext(from);

            context.LastImbued = i;
            context.Imbue_Mod = id;
            context.Imbue_ModInt = value;

            ItemPropertyInfo def = ItemPropertyInfo.GetInfo(id);

            if (def == null)
                return;

            Type gem = def.GemRes;
            Type primary = def.PrimaryRes;
            Type special = ItemPropertyInfo.GetSpecialRes(i, id, def.SpecialRes);

            context.Imbue_ModVal = def.Weight;

            int gemAmount = GetGemAmount(i, id, value);
            int primResAmount = GetPrimaryAmount(i, id, value);
            int specResAmount = GetSpecialAmount(i, id, value);

            if (from.AccessLevel == AccessLevel.Player &&
                (from.Backpack == null || from.Backpack.GetAmount(gem) < gemAmount ||
                from.Backpack.GetAmount(primary) < primResAmount ||
                from.Backpack.GetAmount(special) < specResAmount))
                from.SendLocalizedMessage(1079773); //You do not have enough resources to imbue this item.     
            else
            {
                int maxWeight = GetMaxWeight(i);

                int trueWeight = GetTotalWeight(i, id, true, true);
                int imbuingWeight = GetTotalWeight(i, id, false, true);
                int totalItemMods = GetTotalMods(i, id);
                int maxint = ItemPropertyInfo.GetMaxIntensity(i, id, true);

                int propImbuingweight = (int)((def.Weight / (double)maxint) * value);
                int propTrueWeight = (int)((propImbuingweight / (double)def.Weight) * 100);

                if ((imbuingWeight + propImbuingweight) > maxWeight)
                {
                    from.SendLocalizedMessage(1079772); // You cannot imbue this item with any more item properties.
                    from.CloseGump(typeof(ImbueGump));
                    return;
                }

                double skill = from.Skills[SkillName.Imbuing].Value;
                double success = GetSuccessChance(from, i, trueWeight, propTrueWeight, bonus);

                if (TimesImbued(i) < 20 && skill < from.Skills[SkillName.Imbuing].Cap)
                {
                    double s = Math.Min(100, success);
                    double mins = 120 - (s * 1.2);
                    double maxs = Math.Max(120 / (s / 100), skill);

                    from.CheckSkill(SkillName.Imbuing, mins, maxs);
                }

                success /= 100;

                Effects.SendPacket(from, from.Map, new GraphicalEffect(EffectType.FixedFrom, from.Serial, Serial.Zero, 0x375A, from.Location, from.Location, 1, 17, true, false));
                Effects.SendTargetParticles(from, 0, 1, 0, 0x1593, EffectLayer.Waist);

                if (success >= Utility.RandomDouble() || id < 0 || id > 180)
                {
                    if (from.AccessLevel == AccessLevel.Player)
                    {
                        from.Backpack.ConsumeTotal(gem, gemAmount);
                        from.Backpack.ConsumeTotal(primary, primResAmount);

                        if (specResAmount > 0)
                            from.Backpack.ConsumeTotal(special, specResAmount);
                    }


                    ImbueItem(from, i, id, value);
                }
                else
                {
                    // This is consumed regardless of success/fail
                    if (from.AccessLevel == AccessLevel.Player)
                    {
                        from.Backpack.ConsumeTotal(primary, primResAmount);
                    }

                    from.SendLocalizedMessage(1079774); // You attempt to imbue the item, but fail.
                    from.PlaySound(0x1E4);
                }
            }

            from.EndAction(typeof(Imbuing));
        }

        public static void ImbueItem(Mobile from, Item item, int id, int value)
        {
            from.SendLocalizedMessage(1079775); // You successfully imbue the item!
            from.PlaySound(0x1EB);

            if (item is BaseWeapon)
            {
                BaseWeapon wep = (BaseWeapon)item;

                // New property replaces the old one, so lets set them all to 0
                if (id >= 30 && id <= 34)
                {
                    wep.WeaponAttributes.HitPhysicalArea = 0;
                    wep.WeaponAttributes.HitFireArea = 0;
                    wep.WeaponAttributes.HitColdArea = 0;
                    wep.WeaponAttributes.HitPoisonArea = 0;
                    wep.WeaponAttributes.HitEnergyArea = 0;
                }
                else if (id >= 35 && id <= 39)
                {
                    wep.WeaponAttributes.HitMagicArrow = 0;
                    wep.WeaponAttributes.HitHarm = 0;
                    wep.WeaponAttributes.HitFireball = 0;
                    wep.WeaponAttributes.HitLightning = 0;
                    wep.WeaponAttributes.HitDispel = 0;
                }
            }

            if (item is BaseJewel && id >= 151 && id <= 183)
            {
                BaseJewel jewel = (BaseJewel)item;
                SkillName[] group = GetSkillGroup((SkillName)ItemPropertyInfo.GetAttribute(id));

                //Removes skill bonus if that group already exists on the item
                for (int j = 0; j < 5; j++)
                {
                    if (jewel.SkillBonuses.GetBonus(j) > 0 && group.Any(sk => sk == jewel.SkillBonuses.GetSkill(j)))
                    {
                        jewel.SkillBonuses.SetBonus(j, 0.0);
                        jewel.SkillBonuses.SetSkill(j, SkillName.Alchemy);
                    }
                }
            }

            SetProperty(item, id, value);

            // Sets DImodded, which is used in BaseWeapon
            if (item is BaseWeapon && id == 12 && !((BaseWeapon)item).DImodded)
            {
                ((BaseWeapon)item).DImodded = true;
            }

            // removes nom-imbued Imbuing value, which changes the way the items total weight is calculated
            if (id >= 51 && id <= 55)
            {
                if (item is BaseArmor)
                {
                    BaseArmor arm = (BaseArmor)item;

                    switch (id)
                    {
                        case 51: arm.PhysNonImbuing = 0; break;
                        case 52: arm.FireNonImbuing = 0; break;
                        case 53: arm.ColdNonImbuing = 0; break;
                        case 54: arm.PoisonNonImbuing = 0; break;
                        case 55: arm.EnergyNonImbuing = 0; break;
                    }
                }
                else if (item is BaseClothing)
                {
                    BaseClothing hat = (BaseClothing)item;

                    switch (id)
                    {
                        case 51: hat.PhysNonImbuing = 0; break;
                        case 52: hat.FireNonImbuing = 0; break;
                        case 53: hat.ColdNonImbuing = 0; break;
                        case 54: hat.PoisonNonImbuing = 0; break;
                        case 55: hat.EnergyNonImbuing = 0; break;
                    }
                }
            }

            if (item is IImbuableEquipement)
            {
                IImbuableEquipement imbuable = (IImbuableEquipement)item;

                imbuable.OnAfterImbued(from, id, value);
                imbuable.TimesImbued++;
            }

            // jewels get hits set to 255
            if (item is BaseJewel && ((BaseJewel)item).MaxHitPoints <= 0)
            {
                ((BaseJewel)item).MaxHitPoints = 255;
                ((BaseJewel)item).HitPoints = 255;
            }

            // Removes self repair
            AosArmorAttributes armorAttrs = RunicReforging.GetAosArmorAttributes(item);

            if (armorAttrs != null)
            {
                armorAttrs.SelfRepair = 0;
            }
            else
            {
                AosWeaponAttributes wepAttrs = RunicReforging.GetAosWeaponAttributes(item);

                if (wepAttrs != null)
                {
                    wepAttrs.SelfRepair = 0;
                }
            }
        }

        public static void SetProperty(Item item, int id, int value)
        {
            object prop = ItemPropertyInfo.GetAttribute(id);

            if (item is BaseWeapon)
            {
                BaseWeapon wep = item as BaseWeapon;

                if (prop is AosAttribute)
                {
                    AosAttribute attr = (AosAttribute)prop;

                    if (attr == AosAttribute.SpellChanneling)
                    {
                        wep.Attributes.SpellChanneling = value;

                        if (value > 0 && wep.Attributes.CastSpeed >= 0)
                            wep.Attributes.CastSpeed -= 1;
                    }
                    else if (attr == AosAttribute.CastSpeed)
                    {
                        wep.Attributes.CastSpeed += value;
                    }
                    else
                    {
                        wep.Attributes[attr] = value;
                    }
                }
                else if (prop is AosWeaponAttribute)
                {
                    wep.WeaponAttributes[(AosWeaponAttribute)prop] = value;
                }

                else if (prop is SlayerName)
                {
                    wep.Slayer = (SlayerName)prop;
                }
                else if (prop is SAAbsorptionAttribute)
                {
                    wep.AbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;
                }
                else if (prop is AosElementAttribute)
                {
                    int fire, phys, cold, nrgy, pois, chaos, direct;

                    wep.GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

                    value = Math.Min(phys, value);

                    wep.AosElementDamages[(AosElementAttribute)prop] = value;
                    wep.Hue = wep.GetElementalDamageHue();
                }
                else if (prop is string && wep is BaseRanged && (string)prop == "WeaponVelocity")
                {
                    ((BaseRanged)wep).Velocity = value;
                }
            }
            else if (item is BaseShield)
            {
                BaseShield shield = item as BaseShield;

                if (prop is AosWeaponAttribute && (AosWeaponAttribute)prop == AosWeaponAttribute.DurabilityBonus)
                {
                    prop = AosArmorAttribute.DurabilityBonus;
                }

                if (prop is AosAttribute)
                {
                    AosAttribute attr = (AosAttribute)prop;

                    if (attr == AosAttribute.SpellChanneling)
                    {
                        shield.Attributes.SpellChanneling = value;

                        if (value > 0 && shield.Attributes.CastSpeed >= 0)
                            shield.Attributes.CastSpeed -= 1;
                    }
                    else if (attr == AosAttribute.CastSpeed)
                    {
                        shield.Attributes.CastSpeed += value;
                    }
                    else
                    {
                        shield.Attributes[attr] = value;
                    }
                }
                else if (prop is AosElementAttribute)
                {
                    AosElementAttribute attr = (AosElementAttribute)prop;

                    switch (attr)
                    {
                        case AosElementAttribute.Physical: shield.PhysicalBonus = value; break;
                        case AosElementAttribute.Fire: shield.FireBonus = value; break;
                        case AosElementAttribute.Cold: shield.ColdBonus = value; break;
                        case AosElementAttribute.Poison: shield.PoisonBonus = value; break;
                        case AosElementAttribute.Energy: shield.EnergyBonus = value; break;
                    }
                }
                else if (prop is SAAbsorptionAttribute)
                {
                    shield.AbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;
                }
                else if (prop is AosArmorAttribute)
                {
                    shield.ArmorAttributes[(AosArmorAttribute)prop] = value;
                }
            }
            else if (item is BaseArmor)
            {
                BaseArmor arm = item as BaseArmor;

                if (prop is AosWeaponAttribute && (AosWeaponAttribute)prop == AosWeaponAttribute.DurabilityBonus)
                {
                    prop = AosArmorAttribute.DurabilityBonus;
                }

                if (prop is AosAttribute)
                {
                    arm.Attributes[(AosAttribute)prop] = value;
                }
                else if (prop is AosElementAttribute)
                {
                    AosElementAttribute attr = (AosElementAttribute)prop;

                    switch (attr)
                    {
                        case AosElementAttribute.Physical: arm.PhysicalBonus = value; break;
                        case AosElementAttribute.Fire: arm.FireBonus = value; break;
                        case AosElementAttribute.Cold: arm.ColdBonus = value; break;
                        case AosElementAttribute.Poison: arm.PoisonBonus = value; break;
                        case AosElementAttribute.Energy: arm.EnergyBonus = value; break;
                    }
                }
                else if (prop is SAAbsorptionAttribute)
                {
                    arm.AbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;
                }
                else if (prop is AosArmorAttribute)
                {
                    arm.ArmorAttributes[(AosArmorAttribute)prop] = value;
                }
            }
            else if (item is BaseClothing)
            {
                BaseClothing clothing = item as BaseClothing;

                if (prop is AosAttribute)
                {
                    clothing.Attributes[(AosAttribute)prop] = value;
                }
                else if (prop is SAAbsorptionAttribute)
                {
                    clothing.SAAbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;
                }
                else if (prop is AosElementAttribute)
                {
                    AosElementAttribute attr = (AosElementAttribute)prop;

                    switch (attr)
                    {
                        case AosElementAttribute.Physical: clothing.Resistances.Physical = value; break;
                        case AosElementAttribute.Fire: clothing.Resistances.Fire = value; break;
                        case AosElementAttribute.Cold: clothing.Resistances.Cold = value; break;
                        case AosElementAttribute.Poison: clothing.Resistances.Poison = value; break;
                        case AosElementAttribute.Energy: clothing.Resistances.Energy = value; break;
                    }
                }
            }
            else if (item is BaseJewel)
            {
                BaseJewel jewel = item as BaseJewel;

                if (prop is AosAttribute)
                {
                    jewel.Attributes[(AosAttribute)prop] = value;
                }
                else if (prop is SAAbsorptionAttribute)
                {
                    jewel.AbsorptionAttributes[(SAAbsorptionAttribute)prop] = value;
                }
                else if (prop is AosElementAttribute)
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
                else if (prop is SkillName)
                {
                    SkillName skill = (SkillName)prop;
                    AosSkillBonuses bonuses = jewel.SkillBonuses;

                    int index = GetAvailableSkillIndex(bonuses);

                    if (index >= 0 && index <= 4)
                    {
                        bonuses.SetValues(index, skill, value);
                    }
                }
            }

            item.InvalidateProperties();
        }

        public static bool UnravelItem(Mobile from, Item item, bool message = true)
        {
            double bonus = 0.0;

            if (!CheckSoulForge(from, 2, out bonus))
                return false;

            int weight = GetTotalWeight(item, -1, false, true);

            if (weight <= 0)
            {
                if (message)
                {
                    // You cannot magically unravel this item. It appears to possess little or no magic.
                    from.SendLocalizedMessage(1080437);
                }

                return false;
            }

            ImbuingContext context = GetContext(from);

            Type resType = null;
            int resAmount = Math.Max(1, weight / 100);

            bool success = false;

            if (weight >= 480 - bonus)
            {
                if (from.Skills[SkillName.Imbuing].Value < 95.0)
                {
                    if (message)
                    {
                        // Your Imbuing skill is not high enough to magically unravel this item.
                        from.SendLocalizedMessage(1080434);
                    }

                    return false;
                }

                if (from.CheckSkill(SkillName.Imbuing, 90.1, 120.0))
                {
                    success = true;
                    resType = typeof(RelicFragment);
                    resAmount = 1;
                }
                else if (from.CheckSkill(SkillName.Imbuing, 45.0, 95.0))
                {
                    success = true;
                    resType = typeof(EnchantedEssence);
                    resAmount = Math.Max(1, resAmount - Utility.Random(3));
                }
            }
            else if (weight > 200 - bonus && weight < 480 - bonus)
            {
                if (from.Skills[SkillName.Imbuing].Value < 45.0)
                {
                    if (message)
                    {
                        // Your Imbuing skill is not high enough to magically unravel this item.
                        from.SendLocalizedMessage(1080434);
                    }

                    return false;
                }

                if (from.CheckSkill(SkillName.Imbuing, 45.0, 95.0))
                {
                    success = true;
                    resType = typeof(EnchantedEssence);
                    resAmount = Math.Max(1, resAmount);
                }
                else if (from.CheckSkill(SkillName.Imbuing, 0.0, 45.0))
                {
                    success = true;
                    resType = typeof(MagicalResidue);
                    resAmount = Math.Max(1, resAmount + Utility.Random(2));
                }
            }
            else if (weight <= 200 - bonus)
            {
                if (from.CheckSkill(SkillName.Imbuing, 0.0, 45.0))
                {
                    success = true;
                    resType = typeof(MagicalResidue);
                    resAmount = Math.Max(1, resAmount + Utility.Random(2));
                }
            }
            else
            {
                if (message)
                {
                    // You cannot magically unravel this item. It appears to possess little or no magic.
                    from.SendLocalizedMessage(1080437);
                }

                return false;
            }

            if (!success)
            {
                return false;
            }

            Item res;

            while (resAmount > 0)
            {
                res = Activator.CreateInstance(resType) as Item;

                if (res == null)
                {
                    break;
                }

                if (res.Stackable)
                {
                    res.Amount = Math.Max(1, Math.Min(60000, resAmount));
                }

                resAmount -= res.Amount;

                from.AddToBackpack(res);
            }

            item.Delete();

            return true;
        }

        public static int GetMaxWeight(object item)
        {
            int maxWeight = 450;

            IQuality quality = item as IQuality;

            if (quality != null && quality.Quality == ItemQuality.Exceptional)
                maxWeight += 50;

            if (item is BaseWeapon)
            {
                BaseWeapon itemToImbue = item as BaseWeapon;

                if (itemToImbue is BaseThrown)
                    maxWeight += 0;
                else if (itemToImbue is BaseRanged)
                    maxWeight += 50;
                else if (itemToImbue.Layer == Layer.TwoHanded)
                    maxWeight += 100;
            }
            else if (item is BaseJewel)
            {
                maxWeight = 500;
            }

            return maxWeight;
        }

        public static int GetMaxProps(Item item)
        {
            return 5;
        }

        public static int GetGemAmount(Item item, int id, int value)
        {
            int max = ItemPropertyInfo.GetMaxIntensity(item, id, true);
            int inc = ItemPropertyInfo.GetScale(item, id, false);

            if (max == 1 && inc == 0)
                return 10;

            double v = Math.Floor(value / ((double)max / 10));

            if (v > 10) v = 10;
            if (v < 1) v = 1;

            return (int)v;
        }

        public static int GetPrimaryAmount(Item item, int id, int value)
        {
            int max = ItemPropertyInfo.GetMaxIntensity(item, id, true);
            int inc = ItemPropertyInfo.GetScale(item, id, false);

            //if (item is BaseJewel && id == 12)
            //    max /= 2;

            if (max == 1 && inc == 0)
                return 5;

            double v = Math.Floor(value / (max / 5.0));

            if (v > 5) v = 5;
            if (v < 1) v = 1;

            return (int)v;
        }

        public static int GetSpecialAmount(Item item, int id, int value)
        {
            int max = ItemPropertyInfo.GetMaxIntensity(item, id, true);

            int intensity = (int)((value / (double)max) * 100);

            if (intensity >= 100)
            {
                return 10;
            }

            else if (intensity >= 1 && intensity > 90)
            {
                return intensity - 90;
            }

            return 0;
        }

        [Usage("GetTotalMods")]
        [Description("Displays the total mods, ie AOS attributes for the targeted item.")]
        public static void GetTotalMods_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(12, false, TargetFlags.None, GetTotalMods_OnTarget);
            e.Mobile.SendMessage("Target the item to get total AOS Attributes.");
        }

        public static void GetTotalMods_OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                int ids = GetTotalMods((Item)targeted);

                ((Item)targeted).LabelTo(from, string.Format("Total Mods: {0}", ids.ToString()));
            }
            else
                from.SendMessage("That is not an item!");
        }

        public static int GetTotalMods(Item item, int id = -1)
        {
            int total = 0;
            object prop = ItemPropertyInfo.GetAttribute(id);

            if (item is BaseWeapon)
            {
                BaseWeapon wep = item as BaseWeapon;

                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                {
                    AosAttribute attr = (AosAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (wep.Attributes[attr] > 0)
                    {
                        if (!(prop is AosAttribute) || ((AosAttribute)prop) != attr)
                            total++;
                    }
                    else if (wep.Attributes[attr] == 0 && attr == AosAttribute.CastSpeed && wep.Attributes[AosAttribute.SpellChanneling] > 0)
                    {
                        if (!(prop is AosAttribute) || (AosAttribute)prop != attr)
                            total++;
                    }
                }

                total += GetSkillBonuses(wep.SkillBonuses, prop);

                foreach (long i in Enum.GetValues(typeof(AosWeaponAttribute)))
                {
                    AosWeaponAttribute attr = (AosWeaponAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (wep.WeaponAttributes[attr] > 0)
                    {
                        if (IsHitAreaOrSpell(attr, id))
                            continue;

                        if (!(prop is AosWeaponAttribute) || ((AosWeaponAttribute)prop) != attr)
                            total++;
                    }
                }

                foreach (int i in Enum.GetValues(typeof(ExtendedWeaponAttribute)))
                {
                    ExtendedWeaponAttribute attr = (ExtendedWeaponAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (wep.ExtendedWeaponAttributes[attr] > 0)
                    {
                        if (!(prop is ExtendedWeaponAttribute) || ((ExtendedWeaponAttribute)prop) != attr)
                            total++;
                    }
                }

                if (wep.Slayer != SlayerName.None && (!(prop is SlayerName) || ((SlayerName)prop) != wep.Slayer))
                    total++;

                if (wep.Slayer2 != SlayerName.None)
                    total++;

                if (wep.Slayer3 != TalismanSlayerName.None)
                    total++;

                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                {
                    SAAbsorptionAttribute attr = (SAAbsorptionAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (wep.AbsorptionAttributes[attr] > 0)
                    {
                        if (!(prop is SAAbsorptionAttribute) || ((SAAbsorptionAttribute)prop) != attr)
                            total++;
                    }
                }

                if (wep is BaseRanged && !(prop is string))
                {
                    BaseRanged ranged = wep as BaseRanged;

                    if (ranged.Velocity > 0 && id != 60)
                        total++;
                }

                if (wep.SearingWeapon)
                    total++;
            }
            else if (item is BaseArmor)
            {
                BaseArmor armor = item as BaseArmor;

                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                {
                    AosAttribute attr = (AosAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (armor.Attributes[attr] > 0)
                    {
                        if (!(prop is AosAttribute) || ((AosAttribute)prop) != attr)
                            total++;
                    }
                    else if (armor.Attributes[attr] == 0 && attr == AosAttribute.CastSpeed && armor.Attributes[AosAttribute.SpellChanneling] > 0)
                    {
                        if (!(prop is AosAttribute) || (AosAttribute)prop == attr)
                            total++;
                    }
                }

                total += GetSkillBonuses(armor.SkillBonuses, prop);

                if (armor.PhysicalBonus > armor.PhysNonImbuing && id != 51) { total++; }
                if (armor.FireBonus > armor.FireNonImbuing && id != 52) { total++; }
                if (armor.ColdBonus > armor.ColdNonImbuing && id != 53) { total++; }
                if (armor.PoisonBonus > armor.PoisonNonImbuing && id != 54) { total++; }
                if (armor.EnergyBonus > armor.EnergyNonImbuing && id != 55) { total++; }

                foreach (int i in Enum.GetValues(typeof(AosArmorAttribute)))
                {
                    AosArmorAttribute attr = (AosArmorAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (armor.ArmorAttributes[attr] > 0)
                    {
                        if (!(prop is AosArmorAttribute) || ((AosArmorAttribute)prop) != attr)
                            total++;
                    }
                }


                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                {
                    SAAbsorptionAttribute attr = (SAAbsorptionAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (armor.AbsorptionAttributes[attr] > 0)
                    {
                        if (!(prop is SAAbsorptionAttribute) || ((SAAbsorptionAttribute)prop) != attr)
                            total++;
                    }
                }
            }
            else if (item is BaseJewel)
            {
                BaseJewel j = item as BaseJewel;

                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                {
                    AosAttribute attr = (AosAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (j.Attributes[attr] > 0)
                    {
                        if (!(prop is AosAttribute) || ((AosAttribute)prop) != attr)
                            total++;
                    }
                }

                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                {
                    SAAbsorptionAttribute attr = (SAAbsorptionAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (j.AbsorptionAttributes[attr] > 0)
                    {
                        if (!(prop is SAAbsorptionAttribute) || ((SAAbsorptionAttribute)prop) != attr)
                            total++;
                    }
                }

                total += GetSkillBonuses(j.SkillBonuses, prop);

                if (j.Resistances.Physical > 0 && id != 51) { total++; }
                if (j.Resistances.Fire > 0 && id != 52) { total++; }
                if (j.Resistances.Cold > 0 && id != 53) { total++; }
                if (j.Resistances.Poison > 0 && id != 54) { total++; }
                if (j.Resistances.Energy > 0 && id != 55) { total++; }
            }
            else if (item is BaseClothing)
            {
                BaseClothing clothing = item as BaseClothing;

                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                {
                    AosAttribute attr = (AosAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (clothing.Attributes[attr] > 0)
                    {
                        if (!(prop is AosAttribute) || ((AosAttribute)prop) != attr)
                            total++;
                    }
                }

                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                {
                    SAAbsorptionAttribute attr = (SAAbsorptionAttribute)i;

                    if (!ItemPropertyInfo.ValidateProperty(attr))
                        continue;

                    if (clothing.SAAbsorptionAttributes[attr] > 0)
                    {
                        if (!(prop is SAAbsorptionAttribute) || ((SAAbsorptionAttribute)prop) != attr)
                            total++;
                    }
                }

                total += GetSkillBonuses(clothing.SkillBonuses, prop);

                if (clothing.Resistances.Physical > clothing.PhysNonImbuing && id != 51) { total++; }
                if (clothing.Resistances.Fire > clothing.FireNonImbuing && id != 52) { total++; }
                if (clothing.Resistances.Cold > clothing.ColdNonImbuing && id != 53) { total++; }
                if (clothing.Resistances.Poison > clothing.PoisonNonImbuing && id != 54) { total++; }
                if (clothing.Resistances.Energy > clothing.EnergyNonImbuing && id != 55) { total++; }
            }

            Type type = item.GetType();

            if (IsDerivedArmorOrClothing(type))
            {
                int[] resists = null;

                if (ResistBuffer != null && ResistBuffer.ContainsKey(type))
                {
                    resists = ResistBuffer[type];
                }
                else
                {
                    Type baseType = type.BaseType;

                    if (IsDerivedArmorOrClothing(baseType))
                    {
                        Item temp = Loot.Construct(baseType);

                        if (temp != null)
                        {
                            resists = new int[5];

                            resists[0] = GetBaseResistBonus(item, AosElementAttribute.Physical) - GetBaseResistBonus(temp, AosElementAttribute.Physical);
                            resists[1] = GetBaseResistBonus(item, AosElementAttribute.Fire) - GetBaseResistBonus(temp, AosElementAttribute.Fire);
                            resists[2] = GetBaseResistBonus(item, AosElementAttribute.Cold) - GetBaseResistBonus(temp, AosElementAttribute.Cold);
                            resists[3] = GetBaseResistBonus(item, AosElementAttribute.Poison) - GetBaseResistBonus(temp, AosElementAttribute.Poison);
                            resists[4] = GetBaseResistBonus(item, AosElementAttribute.Energy) - GetBaseResistBonus(temp, AosElementAttribute.Energy);

                            if (ResistBuffer == null)
                                ResistBuffer = new Dictionary<Type, int[]>();

                            ResistBuffer[type] = resists;
                            temp.Delete();
                        }
                    }
                }

                if (resists != null)
                {
                    for (int i = 0; i < resists.Length; i++)
                    {
                        if (id != 51 + i && resists[i] > 0)
                        {
                            total++;
                        }
                    }
                }
            }

            return total;
        }

        private static bool IsHitAreaOrSpell(AosWeaponAttribute attr, int id)
        {
            if (attr >= AosWeaponAttribute.HitMagicArrow && attr <= AosWeaponAttribute.HitDispel)
                return id >= 35 && id <= 39;
            else if (attr >= AosWeaponAttribute.HitColdArea && attr <= AosWeaponAttribute.HitPhysicalArea)
                return id >= 30 && id <= 34;
            return false;
        }

        /*private static bool IsInSkillGroup(SkillName skill, int index)
        {
            if (index < 0 || index >= m_SkillGroups.Length)
                return false;

            foreach (SkillName name in m_SkillGroups[index])
            {
                if (name == skill)
                    return true;
            }
            return false;
        }*/

        private static int GetSkillBonuses(AosSkillBonuses bonus, object prop)
        {
            int id = 0;

            for (int j = 0; j < 5; j++)
            {
                if (bonus.GetBonus(j) > 0)
                {
                    if (!(prop is SkillName) || !IsInSkillGroup(bonus.GetSkill(j), (SkillName)prop))
                        id += 1;
                }
            }

            return id;
        }

        [Usage("GetTotalWeight")]
        [Description("Displays the total imbuing weight of the targeted item.")]
        public static void GetTotalWeight_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(12, false, TargetFlags.None, GetTotalWeight_OnTarget);
            e.Mobile.SendMessage("Target the item to get total imbuing weight.");
        }

        public static void GetTotalWeight_OnTarget(Mobile from, object targeted)
        {
            if (targeted is Item)
            {
                int w = GetTotalWeight((Item)targeted, -1, false, true);
                ((Item)targeted).LabelTo(from, string.Format("Imbuing Weight: {0}", w.ToString()));
                w = GetTotalWeight((Item)targeted, -1, false, false);
                ((Item)targeted).LabelTo(from, string.Format("Loot Weight: {0}", w.ToString()));
                w = GetTotalWeight((Item)targeted, -1, true, true);
                ((Item)targeted).LabelTo(from, string.Format("True Weight: {0}", w.ToString()));
            }
            else
                from.SendMessage("That is not an item!");
        }

        public static Dictionary<Type, int[]> ResistBuffer { get; private set; }

        public static int GetTotalWeight(Item item, int id, bool trueWeight, bool imbuing)
        {
            double weight = 0;

            AosAttributes aosAttrs = RunicReforging.GetAosAttributes(item);
            AosWeaponAttributes wepAttrs = RunicReforging.GetAosWeaponAttributes(item);
            SAAbsorptionAttributes saAttrs = RunicReforging.GetSAAbsorptionAttributes(item);
            AosArmorAttributes armorAttrs = RunicReforging.GetAosArmorAttributes(item);
            AosElementAttributes resistAttrs = RunicReforging.GetElementalAttributes(item);
            ExtendedWeaponAttributes extattrs = RunicReforging.GetExtendedWeaponAttributes(item);

            if (item is BaseWeapon)
            {
                if (((BaseWeapon)item).Slayer != SlayerName.None)
                    weight += GetIntensityForAttribute(item, ((BaseWeapon)item).Slayer, id, 1, trueWeight, imbuing);

                if (((BaseWeapon)item).Slayer2 != SlayerName.None)
                    weight += GetIntensityForAttribute(item, ((BaseWeapon)item).Slayer2, id, 1, trueWeight, imbuing);

                if (((BaseWeapon)item).Slayer3 != TalismanSlayerName.None)
                    weight += GetIntensityForAttribute(item, ((BaseWeapon)item).Slayer3, id, 1, trueWeight, imbuing);

                if (((BaseWeapon)item).SearingWeapon)
                    weight += GetIntensityForAttribute(item, "SearingWeapon", id, 1, trueWeight, imbuing);

                if (item is BaseRanged)
                {
                    BaseRanged ranged = item as BaseRanged;

                    if (ranged.Velocity > 0)
                        weight += GetIntensityForAttribute(item, "WeaponVelocity", id, ranged.Velocity, trueWeight, imbuing);
                }
            }
            else if (item is BaseArmor)
            {
                BaseArmor arm = (BaseArmor)item;

                if (arm.PhysicalBonus > arm.PhysNonImbuing) { if (id != 51) { weight += (100.0 / 15 * (arm.PhysicalBonus - arm.PhysNonImbuing)); } }
                if (arm.FireBonus > arm.FireNonImbuing) { if (id != 52) { weight += (100.0 / 15 * (arm.FireBonus - arm.FireNonImbuing)); } }
                if (arm.ColdBonus > arm.ColdNonImbuing) { if (id != 53) { weight += (100.0 / 15 * (arm.ColdBonus - arm.ColdNonImbuing)); } }
                if (arm.PoisonBonus > arm.PoisonNonImbuing) { if (id != 54) { weight += (100.0 / 15 * (arm.PoisonBonus - arm.PoisonNonImbuing)); } }
                if (arm.EnergyBonus > arm.EnergyNonImbuing) { if (id != 55) { weight += (100.0 / 15 * (arm.EnergyBonus - arm.EnergyNonImbuing)); } }
            }

            Type type = item.GetType();

            if (IsDerivedArmorOrClothing(type))
            {
                int[] resists = null;

                if (ResistBuffer != null && ResistBuffer.ContainsKey(type))
                {
                    resists = ResistBuffer[type];
                }
                else
                {
                    Type baseType = type.BaseType;

                    if (IsDerivedArmorOrClothing(baseType))
                    {
                        Item temp = Loot.Construct(baseType);

                        if (temp != null)
                        {
                            resists = new int[5];

                            resists[0] = GetBaseResistBonus(item, AosElementAttribute.Physical) - GetBaseResistBonus(temp, AosElementAttribute.Physical);
                            resists[1] = GetBaseResistBonus(item, AosElementAttribute.Fire) - GetBaseResistBonus(temp, AosElementAttribute.Fire);
                            resists[2] = GetBaseResistBonus(item, AosElementAttribute.Cold) - GetBaseResistBonus(temp, AosElementAttribute.Cold);
                            resists[3] = GetBaseResistBonus(item, AosElementAttribute.Poison) - GetBaseResistBonus(temp, AosElementAttribute.Poison);
                            resists[4] = GetBaseResistBonus(item, AosElementAttribute.Energy) - GetBaseResistBonus(temp, AosElementAttribute.Energy);

                            if (ResistBuffer == null)
                                ResistBuffer = new Dictionary<Type, int[]>();

                            ResistBuffer[type] = resists;
                            temp.Delete();
                        }
                    }
                }

                if (resists != null)
                {
                    for (int i = 0; i < resists.Length; i++)
                    {
                        if (id != 51 + i && resists[i] > 0)
                        {
                            weight += (100.0 / 15 * resists[i]);
                        }
                    }
                }
            }

            if (aosAttrs != null)
                foreach (int i in Enum.GetValues(typeof(AosAttribute)))
                    weight += GetIntensityForAttribute(item, (AosAttribute)i, id, aosAttrs[(AosAttribute)i], trueWeight, imbuing);

            if (wepAttrs != null)
                foreach (long i in Enum.GetValues(typeof(AosWeaponAttribute)))
                    weight += GetIntensityForAttribute(item, (AosWeaponAttribute)i, id, wepAttrs[(AosWeaponAttribute)i], trueWeight, imbuing);

            if (saAttrs != null)
                foreach (int i in Enum.GetValues(typeof(SAAbsorptionAttribute)))
                    weight += GetIntensityForAttribute(item, (SAAbsorptionAttribute)i, id, saAttrs[(SAAbsorptionAttribute)i], trueWeight, imbuing);

            if (armorAttrs != null)
                foreach (int i in Enum.GetValues(typeof(AosArmorAttribute)))
                    weight += GetIntensityForAttribute(item, (AosArmorAttribute)i, id, armorAttrs[(AosArmorAttribute)i], trueWeight, imbuing);

            if (resistAttrs != null && !(item is BaseWeapon))
                foreach (int i in Enum.GetValues(typeof(AosElementAttribute)))
                    weight += GetIntensityForAttribute(item, (AosElementAttribute)i, id, resistAttrs[(AosElementAttribute)i], trueWeight, imbuing);

            if (extattrs != null)
                foreach (int i in Enum.GetValues(typeof(ExtendedWeaponAttribute)))
                    weight += GetIntensityForAttribute(item, (ExtendedWeaponAttribute)i, id, extattrs[(ExtendedWeaponAttribute)i], trueWeight, imbuing);

            weight += CheckSkillBonuses(item, id, trueWeight, imbuing);

            return (int)weight;
        }

        public static int[] GetBaseResists(Item item)
        {
            int[] resists;

            // Special items base resist don't count as a property or weight. Once that resist is imbued, 
            // it then uses the base class resistance as the base resistance. EA is stupid.
            if (item is IImbuableEquipement && IsSpecialImbuable(item))
            {
                resists = ((IImbuableEquipement)item).BaseResists;
            }
            else
            {
                resists = new int[5];

                resists[0] = GetBaseResistBonus(item, AosElementAttribute.Physical);
                resists[1] = GetBaseResistBonus(item, AosElementAttribute.Fire);
                resists[2] = GetBaseResistBonus(item, AosElementAttribute.Cold);
                resists[3] = GetBaseResistBonus(item, AosElementAttribute.Poison);
                resists[4] = GetBaseResistBonus(item, AosElementAttribute.Energy);
            }

            return resists;
        }

        private static int GetBaseResistBonus(Item item, AosElementAttribute resist)
        {
            switch (resist)
            {
                case AosElementAttribute.Physical:
                    {
                        if (item is BaseArmor)
                            return ((BaseArmor)item).BasePhysicalResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).BasePhysicalResistance;

                        break;
                    }
                case AosElementAttribute.Fire:
                    {
                        if (item is BaseArmor)
                            return ((BaseArmor)item).BaseFireResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).BaseFireResistance;

                        break;
                    }
                case AosElementAttribute.Cold:
                    {
                        if (item is BaseArmor)
                            return ((BaseArmor)item).BaseColdResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).BaseColdResistance;

                        break;
                    }
                case AosElementAttribute.Poison:
                    {
                        if (item is BaseArmor)
                            return ((BaseArmor)item).BasePoisonResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).BasePoisonResistance;

                        break;
                    }
                case AosElementAttribute.Energy:
                    {
                        if (item is BaseArmor)
                            return ((BaseArmor)item).BaseEnergyResistance;

                        if (item is BaseClothing)
                            return ((BaseClothing)item).BaseEnergyResistance;

                        break;
                    }
            }

            return 0;
        }

        /// <summary>
        /// This is for special items such as artifacts, if you ever so chose to imbue them on your server. Without
        /// massive edits, this should never come back as true.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsDerivedArmorOrClothing(Type type)
        {
            if (IsSpecialImbuable(type))
            {
                return false;
            }

            return (type.IsSubclassOf(typeof(BaseClothing)) || type.IsSubclassOf(typeof(BaseArmor))) &&
                type != typeof(BaseHat) &&
                type != typeof(BaseShield) &&
                type != typeof(Item) &&
                type != typeof(BaseOuterTorso) &&
                type != typeof(BaseMiddleTorso) &&
                type != typeof(BaseOuterLegs) &&
                type != typeof(BasePants) &&
                type != typeof(BaseShirt) &&
                type != typeof(BaseWaist) &&
                type != typeof(BaseShoes) &&
                type != typeof(BaseCloak);
        }

        private static int CheckSkillBonuses(Item item, int check, bool trueWeight, bool imbuing)
        {
            double weight = 0;

            AosSkillBonuses skills = RunicReforging.GetAosSkillBonuses(item);

            if (skills != null)
            {
                int id = -1;

                if (item is BaseJewel)
                {
                    id = check;
                }

                // Place Holder. THis is in case the skill weight/max intensity every changes
                int totalWeight = trueWeight ? 100 : ItemPropertyInfo.GetWeight(151);
                int maxInt = ItemPropertyInfo.GetMaxIntensity(item, 151, imbuing);

                for (int i = 0; i < 5; i++)
                {
                    double bonus = skills.GetBonus(i);

                    if (bonus > 0)
                    {
                        var attr = ItemPropertyInfo.GetAttribute(id);

                        if ((id < 151 && id > 183) || !(attr is SkillName) || !IsInSkillGroup(skills.GetSkill(i), (SkillName)attr))
                        {
                            weight += (totalWeight / maxInt * bonus);
                        }
                    }
                }
            }

            return (int)weight;
        }

        public static SkillName[] PossibleSkills => m_PossibleSkills;
        private static readonly SkillName[] m_PossibleSkills = new SkillName[]
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

        private static readonly SkillName[][] m_SkillGroups = new SkillName[][]
        {
            new SkillName[] { SkillName.Fencing, SkillName.Macing, SkillName.Swords, SkillName.Musicianship, SkillName.Magery },
            new SkillName[] { SkillName.Wrestling, SkillName.AnimalTaming, SkillName.SpiritSpeak, SkillName.Tactics, SkillName.Provocation },
            new SkillName[] { SkillName.Focus, SkillName.Parry, SkillName.Stealth, SkillName.Meditation, SkillName.AnimalLore, SkillName.Discordance },
            new SkillName[] { SkillName.Mysticism, SkillName.Bushido, SkillName.Necromancy, SkillName.Veterinary, SkillName.Stealing, SkillName.EvalInt, SkillName.Anatomy },
            new SkillName[] { SkillName.Peacemaking, SkillName.Ninjitsu, SkillName.Chivalry, SkillName.Archery, SkillName.MagicResist, SkillName.Healing, SkillName.Throwing }
        };

        public static SkillName[] GetSkillGroup(SkillName skill)
        {
            return m_SkillGroups.FirstOrDefault(list => list.Any(sk => sk == skill));
        }

        public static int GetAvailableSkillIndex(AosSkillBonuses skills)
        {
            for (int i = 0; i < 5; i++)
            {
                if (skills.GetBonus(i) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public static int GetSkillGroupIndex(SkillName skill)
        {
            for (int i = 0; i < m_SkillGroups.Length; i++)
            {
                if (m_SkillGroups[i].Any(sk => sk == skill))
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool IsInSkillGroup(SkillName one, SkillName two)
        {
            var skillGroup1 = GetSkillGroupIndex(one);
            var skillGroup2 = GetSkillGroupIndex(two);

            return skillGroup1 != -1 && skillGroup2 != -1 && skillGroup1 == skillGroup2;
        }

        public static bool CheckSoulForge(Mobile from, int range)
        {
            return CheckSoulForge(from, range, true, true);
        }

        public static bool CheckSoulForge(Mobile from, int range, out double bonus)
        {
            return CheckSoulForge(from, range, true, true, out bonus);
        }

        public static bool CheckSoulForge(Mobile from, int range, bool message)
        {
            double bonus;
            return CheckSoulForge(from, range, message, false, out bonus);
        }

        public static bool CheckSoulForge(Mobile from, int range, bool message, bool checkqueen)
        {
            double bonus;
            return CheckSoulForge(from, range, message, checkqueen, out bonus);
        }

        public static bool CheckSoulForge(Mobile from, int range, bool message, bool checkqueen, out double bonus)
        {
            PlayerMobile m = from as PlayerMobile;
            bonus = 0.0;

            ImbuingContext context = GetContext(m);
            Map map = from.Map;

            if (map == null)
                return false;

            bool isForge = false;

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, range);

            foreach (Item item in eable)
            {
                if ((item.ItemID >= 0x4277 && item.ItemID <= 0x4286) || (item.ItemID >= 0x4263 && item.ItemID <= 0x4272) || (item.ItemID >= 17607 && item.ItemID <= 17610))
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

            if (checkqueen)
            {
                if (from.Region != null && from.Region.IsPartOf("Queen's Palace"))
                {
                    if (!Engines.Points.PointsSystem.QueensLoyalty.IsNoble(from))
                    {
                        if (message)
                        {
                            from.SendLocalizedMessage(1113736); // You must rise to the rank of noble in the eyes of the Gargoyle Queen before her majesty will allow you to use this soulforge.
                        }

                        return false;
                    }
                    else
                    {
                        bonus = 0.06;
                    }
                }
                else if (from.Region != null && from.Region.IsPartOf("Royal City"))
                {
                    bonus = 0.02;
                }
            }

            return true;
        }

        public static Type[] IngredTypes => m_IngredTypes;
        private static readonly Type[] m_IngredTypes = new Type[]
        {
            typeof(MagicalResidue),     typeof(EnchantedEssence),       typeof(RelicFragment),

            typeof(SeedOfRenewal),      typeof(ChagaMushroom),          typeof(CrystalShards),
            typeof(BottleIchor),        typeof(ReflectiveWolfEye),      typeof(FaeryDust),
            typeof(BouraPelt),          typeof(SilverSnakeSkin),        typeof(ArcanicRuneStone),
            typeof(SlithTongue),        typeof(VoidOrb),                typeof(RaptorTeeth),
            typeof(SpiderCarapace),     typeof(DaemonClaw),             typeof(VialOfVitriol),
            typeof(GoblinBlood),        typeof(LavaSerpentCrust),       typeof(UndyingFlesh),
            typeof(CrushedGlass),       typeof(CrystallineBlackrock),   typeof(PowderedIron),
            typeof(ElvenFletching),     typeof(DelicateScales),

            typeof(EssenceSingularity), typeof(EssenceBalance),       typeof(EssencePassion),
            typeof(EssenceDirection),   typeof(EssencePrecision),       typeof(EssenceControl),
            typeof(EssenceDiligence),   typeof(EssenceAchievement),     typeof(EssenceFeeling),
            typeof(EssenceOrder),

            typeof(ParasiticPlant),   typeof(LuminescentFungi),
            typeof(FireRuby),           typeof(WhitePearl),             typeof(BlueDiamond),
            typeof(Turquoise)
        };


        public static bool IsInNonImbueList(Type itemType)
        {
            foreach (Type type in m_CannotImbue)
            {
                if (type == itemType)
                    return true;
            }

            return false;
        }

        private static readonly Type[] m_CannotImbue = new Type[]
        {
            typeof(GargishLeatherWingArmor), typeof(GargishClothWingArmor)
        };

        public static int GetValueForID(Item item, int id)
        {
            object attr = ItemPropertyInfo.GetAttribute(id);

            if (item is BaseWeapon)
            {
                BaseWeapon w = (BaseWeapon)item;

                if (id == 16 && w.Attributes.SpellChanneling > 0)
                    return w.Attributes[AosAttribute.CastSpeed] + 1;

                if (attr is AosAttribute)
                    return w.Attributes[(AosAttribute)attr];

                else if (attr is AosWeaponAttribute)
                    return w.WeaponAttributes[(AosWeaponAttribute)attr];

                else if (attr is ExtendedWeaponAttribute)
                    return w.ExtendedWeaponAttributes[(ExtendedWeaponAttribute)attr];

                else if (attr is SAAbsorptionAttribute)
                    return w.AbsorptionAttributes[(SAAbsorptionAttribute)attr];

                else if (attr is SlayerName && w.Slayer == (SlayerName)attr)
                    return 1;

                else if (id == 60 && item is BaseRanged)
                    return ((BaseRanged)item).Velocity;

                else if (id == 62)
                    return w.SearingWeapon ? 1 : 0;

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

                if (a is BaseShield && id == 16 && a.Attributes.SpellChanneling > 0)
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
                    int value = 0;

                    switch (ele)
                    {
                        case AosElementAttribute.Physical: value = a.PhysicalBonus; break;
                        case AosElementAttribute.Fire: value = a.FireBonus; break;
                        case AosElementAttribute.Cold: value = a.ColdBonus; break;
                        case AosElementAttribute.Poison: value = a.PoisonBonus; break;
                        case AosElementAttribute.Energy: value = a.EnergyBonus; break;
                    }

                    if (value > 0)
                    {
                        return value;
                    }
                }
            }
            else if (item is BaseClothing)
            {
                BaseClothing c = (BaseClothing)item;

                if (attr is AosAttribute)
                    return c.Attributes[(AosAttribute)attr];

                else if (attr is AosElementAttribute)
                {
                    int value = c.Resistances[(AosElementAttribute)attr];

                    if (value > 0)
                    {
                        return value;
                    }
                }

                else if (attr is AosArmorAttribute)
                    return c.ClothingAttributes[(AosArmorAttribute)attr];

                else if (attr is SAAbsorptionAttribute)
                    return c.SAAbsorptionAttributes[(SAAbsorptionAttribute)attr];
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

            Type type = item.GetType();

            if (id >= 51 && id <= 55 && IsDerivedArmorOrClothing(type))
            {
                int[] resists = null;

                if (ResistBuffer != null && ResistBuffer.ContainsKey(type))
                {
                    resists = ResistBuffer[type];
                }
                else
                {
                    Type baseType = type.BaseType;

                    if (IsDerivedArmorOrClothing(baseType))
                    {
                        Item temp = Loot.Construct(baseType);

                        if (temp != null)
                        {
                            resists = new int[5];

                            resists[0] = GetBaseResistBonus(item, AosElementAttribute.Physical) - GetBaseResistBonus(temp, AosElementAttribute.Physical);
                            resists[1] = GetBaseResistBonus(item, AosElementAttribute.Fire) - GetBaseResistBonus(temp, AosElementAttribute.Fire);
                            resists[2] = GetBaseResistBonus(item, AosElementAttribute.Cold) - GetBaseResistBonus(temp, AosElementAttribute.Cold);
                            resists[3] = GetBaseResistBonus(item, AosElementAttribute.Poison) - GetBaseResistBonus(temp, AosElementAttribute.Poison);
                            resists[4] = GetBaseResistBonus(item, AosElementAttribute.Energy) - GetBaseResistBonus(temp, AosElementAttribute.Energy);

                            if (ResistBuffer == null)
                                ResistBuffer = new Dictionary<Type, int[]>();

                            ResistBuffer[type] = resists;
                            temp.Delete();
                        }
                    }
                }

                if (resists != null && resists.Length == 5)
                {
                    return resists[id - 51];
                }
            }

            return 0;
        }

        public static int GetIntensityForAttribute(Item item, object attr, int checkID, int value)
        {
            return GetIntensityForAttribute(item, attr, checkID, value, false);
        }

        public static int GetIntensityForAttribute(Item item, object attr, int checkID, int value, bool trueWeight)
        {
            return GetIntensityForID(item, ItemPropertyInfo.GetID(attr), checkID, value, trueWeight, true);
        }

        public static int GetIntensityForAttribute(Item item, object attr, int checkID, int value, bool trueWeight, bool imbuing)
        {
            return GetIntensityForID(item, ItemPropertyInfo.GetID(attr), checkID, value, trueWeight, imbuing);
        }

        public static int GetIntensityForID(Item item, int id, int checkID, int value)
        {
            return GetIntensityForID(item, id, checkID, value, false);
        }

        public static int GetIntensityForID(Item item, int id, int checkID, int value, bool trueWeight)
        {
            return GetIntensityForID(item, id, checkID, value, trueWeight, true);
        }

        public static int GetIntensityForID(Item item, int id, int checkID, int value, bool trueWeight, bool imbuing)
        {
            // This is terribly clunky, however we're accomidating 1 out of 50+ attributes that acts differently
            if (value <= 0 && id != 16)
            {
                return 0;
            }

            if (id == 61 && !(item is BaseRanged))
            {
                id = 63;
            }

            if ((item is BaseWeapon || item is BaseShield) && id == 16)
            {
                AosAttributes attrs = RunicReforging.GetAosAttributes(item);

                if (attrs != null && attrs.SpellChanneling > 0)
                    value++;
            }

            if (value <= 0)
                return 0;

            if (id != checkID)
            {
                int weight = trueWeight ? 100 : ItemPropertyInfo.GetWeight(id);

                if (weight == 0)
                {
                    return 0;
                }

                int max = ItemPropertyInfo.GetMaxIntensity(item, id, imbuing);

                return (int)(((double)weight / max) * value);
            }

            return 0;
        }

        public static bool CanImbueProperty(Mobile from, Item item, int id)
        {
            ItemPropertyInfo info = ItemPropertyInfo.GetInfo(id);
            bool canImbue = false;

            if (info != null)
            {
                if (item is BaseWeapon)
                {
                    if (item is BaseRanged && info.CanImbue(ItemType.Ranged))
                    {
                        canImbue = true;
                    }
                    else if (info.CanImbue(ItemType.Melee))
                    {
                        canImbue = true;
                    }
                }
                else if (item is BaseArmor)
                {
                    if (item is BaseShield && info.CanImbue(ItemType.Shield))
                    {
                        canImbue = true;
                    }
                    else if (info.CanImbue(ItemType.Armor))
                    {
                        canImbue = true;
                    }
                }
                else if (item is BaseJewel && info.CanImbue(ItemType.Jewel))
                {
                    canImbue = true;
                }

            }

            if (!canImbue)
            {
                from.CloseGump(typeof(ImbueGump));
                from.SendLocalizedMessage(1114291); // You cannot imbue the last property on that item.
            }

            return canImbue;
        }

        public static int TimesImbued(Item item)
        {
            if (item is IImbuableEquipement)
            {
                return ((IImbuableEquipement)item).TimesImbued;
            }

            return 0;
        }
    }
}
