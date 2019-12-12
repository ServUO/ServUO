using System;
using System.Linq;

using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.SkillHandlers;

namespace Server.Gumps
{
    public class ImbueGump : BaseGump
    {
        private const int LabelColor = 0x7FFF;
        private const int IceHue = 0x481;
        private const int Green = 0x41;
        private const int Yellow = 0x36;
        private const int DarkYellow = 0x2E;
        private const int Red = 0x26;

        private int m_ID, m_Value;
        private Item m_Item;
        private int m_TotalItemWeight;
        private int m_TotalProps;
        private int m_MaxWeight;

        private ItemPropertyInfo m_Info;

        public ImbueGump(PlayerMobile pm, Item item, int id, int value)
            : base(pm, 50, 50)
        {
            pm.CloseGump(typeof(ImbuingGump));
            pm.CloseGump(typeof(ImbueSelectGump));
            pm.CloseGump(typeof(RunicReforgingGump));

            m_ID = id;
            m_Value = value;
            m_Item = item;
        }

        public override void AddGumpLayout()
        {
            double bonus = 0.0;

            if (!Imbuing.CheckSoulForge(User, 2, out bonus))
                return;

            ImbuingContext context = Imbuing.GetContext(User);

            if (!ItemPropertyInfo.Table.ContainsKey(m_ID))
                return;

            m_Info = ItemPropertyInfo.Table[m_ID];

            int minInt = ItemPropertyInfo.GetMinIntensity(m_Item, m_ID);
            int maxInt = ItemPropertyInfo.GetMaxIntensity(m_Item, m_ID, true);
            int weight = m_Info.Weight;

            if (m_Value < minInt)
            {
                m_Value = minInt;
            }

            if (m_Value > maxInt)
            {
                m_Value = maxInt;
            }

            double currentIntensity = Math.Floor((m_Value / (double)maxInt) * 100);

            // Set context
            context.LastImbued = m_Item;
            context.Imbue_Mod = m_ID;
            context.Imbue_ModVal = weight;
            context.ImbMenu_ModInc = ItemPropertyInfo.GetScale(m_Item, m_ID);

            // Current Mod Weight
            m_TotalItemWeight = Imbuing.GetTotalWeight(m_Item, m_ID, false, true);
            m_TotalProps = Imbuing.GetTotalMods(m_Item, m_ID);

            if (maxInt <= 1)
                currentIntensity = 100;

            var propWeight = (int)Math.Floor(((double)weight / (double)maxInt) * m_Value);

            // Maximum allowed Property Weight & Item Mod Count
            m_MaxWeight = Imbuing.GetMaxWeight(m_Item);

            // Times Item has been Imbued
            int timesImbued = Imbuing.TimesImbued(m_Item);

            // Check Ingredients needed at the current Intensity
            var gemAmount = Imbuing.GetGemAmount(m_Item, m_ID, m_Value);
            var primResAmount = Imbuing.GetPrimaryAmount(m_Item, m_ID, m_Value);
            var specResAmount = Imbuing.GetSpecialAmount(m_Item, m_ID, m_Value);

            AddPage(0);
            AddBackground(0, 0, 520, 440, 5054);
            AddImageTiled(10, 10, 500, 420, 2624);

            AddImageTiled(10, 30, 500, 10, 5058);
            AddImageTiled(250, 40, 10, 290, 5058);
            AddImageTiled(10, 180, 500, 10, 5058);
            AddImageTiled(10, 330, 500, 10, 5058);
            AddImageTiled(10, 400, 500, 10, 5058);

            AddAlphaRegion(10, 10, 500, 420);

            AddHtmlLocalized(10, 12, 520, 20, 1079717, LabelColor, false, false); // <CENTER>IMBUING CONFIRMATION</CENTER>
            AddHtmlLocalized(50, 50, 200, 20, 1114269, LabelColor, false, false); // PROPERTY INFORMATION

            AddHtmlLocalized(25, 80, 80, 20, 1114270, LabelColor, false, false);  // Property:

            if (m_Info.AttributeName != null)
            {
                AddHtmlLocalized(95, 80, 150, 20, 1114057, m_Info.AttributeName.ToString(), LabelColor, false, false);
            }

            AddHtmlLocalized(25, 100, 80, 20, 1114271, LabelColor, false, false); // Replaces:
            var replace = WhatReplacesWhat(m_ID, m_Item);

            if (replace != null)
            {
                AddHtmlLocalized(95, 100, 150, 20, 1114057, replace.ToString(), LabelColor, false, false);
            }

            // Weight Modifier
            AddHtmlLocalized(25, 120, 80, 20, 1114272, 0xFFFFFF, false, false); // Weight:
            AddLabel(95, 120, IceHue, String.Format("{0}x", ((double)m_Info.Weight / 100.0).ToString("0.0")));

            AddHtmlLocalized(25, 140, 80, 20, 1114273, LabelColor, false, false); // Intensity:
            AddLabel(95, 140, IceHue, String.Format("{0}%", currentIntensity));

            // Materials needed
            AddHtmlLocalized(10, 200, 245, 20, 1044055, LabelColor, false, false); // <CENTER>MATERIALS</CENTER>

            AddHtmlLocalized(40, 230, 180, 20, m_Info.PrimaryName, LabelColor, false, false);
            AddLabel(210, 230, IceHue, primResAmount.ToString());

            AddHtmlLocalized(40, 255, 180, 20, m_Info.GemName, LabelColor, false, false);
            AddLabel(210, 255, IceHue, gemAmount.ToString());

            if (specResAmount > 0)
            {
                AddHtmlLocalized(40, 280, 180, 17, m_Info.SpecialName, LabelColor, false, false);
                AddLabel(210, 280, IceHue, specResAmount.ToString());
            }

            // Mod Description
            AddHtmlLocalized(280, 55, 200, 110, m_Info.Description, LabelColor, false, false);

            AddHtmlLocalized(350, 200, 150, 20, 1113650, LabelColor, false, false); // RESULTS

            AddHtmlLocalized(280, 220, 150, 20, 1113645, LabelColor, false, false); // Properties:
            AddLabel(430, 220, GetColor(m_TotalProps + 1, 5), String.Format("{0}/{1}", m_TotalProps + 1, Imbuing.GetMaxProps(m_Item)));

            int projWeight = m_TotalItemWeight + propWeight;
            AddHtmlLocalized(280, 240, 150, 20, 1113646, LabelColor, false, false); // Total Property Weight:
            AddLabel(430, 240, GetColor(projWeight, m_MaxWeight), String.Format("{0}/{1}", projWeight, m_MaxWeight));

            AddHtmlLocalized(280, 260, 150, 20, 1113647, LabelColor, false, false); // Times Imbued:
            AddLabel(430, 260, GetColor(timesImbued, 20), String.Format("{0}/20", timesImbued));

            // ===== CALCULATE DIFFICULTY =====
            var truePropWeight = (int)(((double)propWeight / (double)weight) * 100);
            var trueTotalWeight = Imbuing.GetTotalWeight(m_Item, -1, true, true);

            double suc = Imbuing.GetSuccessChance(User, m_Item, trueTotalWeight, truePropWeight, bonus);

            AddHtmlLocalized(300, 300, 150, 20, 1044057, 0xFFFFFF, false, false); // Success Chance:
            AddLabel(420, 300, GetSuccessChanceHue(suc), String.Format("{0}%", suc.ToString("0.0")));

            // - Attribute Level
            if (maxInt > 1)
            {
                AddHtmlLocalized(235, 350, 100, 17, 1062300, LabelColor, false, false); // New Value:

                if (m_ID == 41)                                                    // - Mage Weapon Value ( i.e [Mage Weapon -25] )
                {
                    AddLabel(250, 370, IceHue, String.Format("-{0}", 30 - m_Value));
                }
                else if (maxInt <= 8 || m_ID == 21 || m_ID == 17)                 // - Show Property Value as just Number ( i.e [Mana Regen 2] )
                {
                    AddLabel(256, 370, IceHue, String.Format("{0}", m_Value));      // - Show Property Value as % ( i.e [Hit Fireball 25%] )
                }
                else
                {
                    int val = m_Value;

                    if (m_ID >= 51 && m_ID <= 55)
                    {
                        var resistances = Imbuing.GetBaseResists(m_Item);

                        switch (m_ID)
                        {
                            case 51: val += resistances[0]; break;
                            case 52: val += resistances[1]; break;
                            case 53: val += resistances[2]; break;
                            case 54: val += resistances[3]; break;
                            case 55: val += resistances[4]; break;
                        }
                    }

                    AddLabel(256, 370, IceHue, String.Format("{0}%", val));
                }

                // Buttons
                AddButton(179, 372, 0x1464, 0x1464, 10053, GumpButtonType.Reply, 0);
                AddButton(187, 372, 0x1466, 0x1466, 10053, GumpButtonType.Reply, 0);

                AddButton(199, 372, 0x1464, 0x1464, 10052, GumpButtonType.Reply, 0);
                AddButton(207, 372, 0x1466, 0x1466, 10052, GumpButtonType.Reply, 0);

                AddButton(221, 372, 0x1464, 0x1464, 10051, GumpButtonType.Reply, 0);
                AddButton(229, 372, 0x1466, 0x1466, 10051, GumpButtonType.Reply, 0);

                AddButton(280, 372, 0x1464, 0x1464, 10054, GumpButtonType.Reply, 0);
                AddButton(288, 372, 0x1466, 0x1466, 10054, GumpButtonType.Reply, 0);

                AddButton(300, 372, 0x1464, 0x1464, 10055, GumpButtonType.Reply, 0);
                AddButton(308, 372, 0x1466, 0x1466, 10055, GumpButtonType.Reply, 0);

                AddButton(320, 372, 0x1464, 0x1464, 10056, GumpButtonType.Reply, 0);
                AddButton(328, 372, 0x1466, 0x1466, 10056, GumpButtonType.Reply, 0);

                AddLabel(322, 370, 0, ">");
                AddLabel(326, 370, 0, ">");
                AddLabel(330, 370, 0, ">");

                AddLabel(304, 370, 0, ">");
                AddLabel(308, 370, 0, ">");

                AddLabel(286, 370, 0, ">");

                AddLabel(226, 370, 0, "<");

                AddLabel(203, 370, 0, "<");
                AddLabel(207, 370, 0, "<");

                AddLabel(181, 370, 0, "<");
                AddLabel(185, 370, 0, "<");
                AddLabel(189, 370, 0, "<");
            }

            AddButton(15, 410, 4005, 4007, 10099, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 410, 100, 18, 1114268, LabelColor, false, false); // Back 

            AddButton(390, 410, 4005, 4007, 10100, GumpButtonType.Reply, 0);
            AddHtmlLocalized(425, 410, 120, 18, 1114267, LabelColor, false, false); // Imbue Item
        }

        private int GetColor(int value, int limit)
        {
            if (value < limit)
                return Green;
            else if (value == limit)
                return Yellow;
            else
                return Red;
        }

        private int GetSuccessChanceHue(double suc)
        {
            if (suc >= 100)
                return IceHue;
            else if (suc >= 80)
                return Green;
            else if (suc >= 50)
                return Yellow;
            else if (suc >= 10)
                return DarkYellow;
            else
                return Red;
        }

        public override void OnResponse(RelayInfo info)
        {
            ImbuingContext context = Imbuing.GetContext(User);

            switch (info.ButtonID)
            {
                case 0: //Close
                    {
                        User.EndAction(typeof(Imbuing));
                        break;
                    }
                case 10051: // Decrease Mod Value [<]
                    {
                        m_Value = Math.Max(ItemPropertyInfo.GetMinIntensity(m_Item, m_Info.ID), m_Value - 1);
                        Refresh();

                        break;
                    }
                case 10052:// Decrease Mod Value [<<]
                    {
                        m_Value = Math.Max(ItemPropertyInfo.GetMinIntensity(m_Item, m_Info.ID), m_Value - 10);
                        Refresh();
                        
                        break;
                    }
                case 10053:// Minimum Mod Value [<<<]
                    {
                        m_Value = ItemPropertyInfo.GetMinIntensity(m_Item, m_Info.ID);
                        Refresh();
                        
                        break;
                    }
                case 10054: // Increase Mod Value [>]
                    {
                        m_Value = Math.Min(ItemPropertyInfo.GetMaxIntensity(m_Item, m_Info.ID, true), m_Value + 1);
                        Refresh();

                        break;
                    }
                case 10055: // Increase Mod Value [>>]
                    {
                        m_Value = Math.Min(ItemPropertyInfo.GetMaxIntensity(m_Item, m_Info.ID, true), m_Value + 10);
                        Refresh();
                        
                        break;
                    }
                case 10056: // Maximum Mod Value [>>>]
                    {
                        m_Value = ItemPropertyInfo.GetMaxIntensity(m_Item, m_Info.ID, true);
                        Refresh();
                        
                        break;
                    }

                case 10099: // Back
                    {
                        BaseGump.SendGump(new ImbueSelectGump(User, context.LastImbued));
                        break;
                    }
                case 10100:  // Imbue the Item
                    {
                        context.Imbue_IWmax = m_MaxWeight;

                        if (Imbuing.OnBeforeImbue(User, m_Item, m_ID, m_Value, m_TotalProps, Imbuing.GetMaxProps(m_Item), m_TotalItemWeight, m_MaxWeight))
                        {
                            Imbuing.TryImbueItem(User, m_Item, m_ID, m_Value);
                            SendGumpDelayed(User);
                        }

                        break;
                    }
            }
        }

        public static void SendGumpDelayed(PlayerMobile pm)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), () =>
            {
                BaseGump.SendGump(new ImbuingGump(pm));
            });
        }

        // =========== Check if Choosen Attribute Replaces Another =================
        public static TextDefinition WhatReplacesWhat(int id, Item item)
        {
            if (Imbuing.GetValueForID(item, id) > 0)
            {
                return ItemPropertyInfo.GetAttributeName(id);
            }

            if (item is BaseWeapon)
            {
                BaseWeapon i = item as BaseWeapon;

                // Slayers replace Slayers
                if (id >= 101 && id <= 127)
                {
                    if (i.Slayer != SlayerName.None)
                        return GetNameForAttribute(i.Slayer);

                    if (i.Slayer2 != SlayerName.None)
                        return GetNameForAttribute(i.Slayer2);
                }
                // OnHitEffect replace OnHitEffect
                if (id >= 35 && id <= 39)
                {
                    if (i.WeaponAttributes.HitMagicArrow > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitMagicArrow);
                    else if (i.WeaponAttributes.HitHarm > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitHarm);
                    else if (i.WeaponAttributes.HitFireball > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitFireball);
                    else if (i.WeaponAttributes.HitLightning > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitLightning);
                    else if (i.WeaponAttributes.HitDispel > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitDispel);
                }
                // OnHitArea replace OnHitArea
                if (id >= 30 && id <= 34)
                {
                    if (i.WeaponAttributes.HitPhysicalArea > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitPhysicalArea);
                    else if (i.WeaponAttributes.HitColdArea > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitFireArea);
                    else if (i.WeaponAttributes.HitFireArea > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitColdArea);
                    else if (i.WeaponAttributes.HitPoisonArea > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitPoisonArea);
                    else if (i.WeaponAttributes.HitEnergyArea > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitEnergyArea);
                }
            }
            if (item is BaseJewel)
            {
                BaseJewel jewel = item as BaseJewel;

                if (id >= 151 && id <= 183)
                {
                    var bonuses = jewel.SkillBonuses;
                    var group = Imbuing.GetSkillGroup((SkillName)ItemPropertyInfo.GetAttribute(id));

                    for (int i = 0; i < 5; i++)
                    {
                        if (bonuses.GetBonus(i) > 0 && group.Any(sk => sk == bonuses.GetSkill(i)))
                        {
                            return GetNameForAttribute(bonuses.GetSkill(i));
                        }
                    }
                }

                // SkillGroup1 replace SkillGroup1
                /*if (id >= 151 && id <= 155)
                {
                    if (i.SkillBonuses.GetBonus(0) > 0)
                    {
                        foreach (SkillName sk in Imbuing.PossibleSkills)
                        {
                            if (i.SkillBonuses.GetSkill(0) == sk)
                                return GetNameForAttribute(sk);
                        }
                    }
                }
                // SkillGroup2 replace SkillGroup2
                if (id >= 156 && id <= 160)
                {
                    if (i.SkillBonuses.GetBonus(1) > 0)
                    {
                        foreach (SkillName sk in Imbuing.PossibleSkills)
                        {
                            if (i.SkillBonuses.GetSkill(1) == sk)
                                return GetNameForAttribute(sk);
                        }
                    }
                }
                // SkillGroup3 replace SkillGroup3
                if (id >= 161 && id <= 166)
                {
                    if (i.SkillBonuses.GetBonus(2) > 0)
                    {
                        foreach (SkillName sk in Imbuing.PossibleSkills)
                        {
                            if (i.SkillBonuses.GetSkill(2) == sk)
                                return GetNameForAttribute(sk);
                        }
                    }
                }
                // SkillGroup4 replace SkillGroup4
                if (id >= 167 && id <= 172)
                {
                    if (i.SkillBonuses.GetBonus(3) > 0)
                    {
                        foreach (SkillName sk in Imbuing.PossibleSkills)
                        {
                            if (i.SkillBonuses.GetSkill(3) == sk)
                                return GetNameForAttribute(sk);
                        }
                    }
                }
                // SkillGroup5 replace SkillGroup5
                if (id >= 173 && id <= 178)
                {
                    if (i.SkillBonuses.GetBonus(4) > 0)
                    {
                        foreach (SkillName sk in Imbuing.PossibleSkills)
                        {
                            if (i.SkillBonuses.GetSkill(4) == sk)
                                return GetNameForAttribute(sk);
                        }
                    }
                }*/
            }

            return null;
        }

        public static TextDefinition GetNameForAttribute(object attribute)
        {
            if (attribute is AosArmorAttribute && (AosArmorAttribute)attribute == AosArmorAttribute.LowerStatReq)
                attribute = AosWeaponAttribute.LowerStatReq;

            if (attribute is AosArmorAttribute && (AosArmorAttribute)attribute == AosArmorAttribute.DurabilityBonus)
                attribute = AosWeaponAttribute.DurabilityBonus;

            foreach (var info in ItemPropertyInfo.Table.Values)
            {
                if (attribute is SlayerName && info.Attribute is SlayerName && (SlayerName)attribute == (SlayerName)info.Attribute)
                    return info.AttributeName;

                if (attribute is AosAttribute && info.Attribute is AosAttribute && (AosAttribute)attribute == (AosAttribute)info.Attribute)
                    return info.AttributeName;

                if (attribute is AosWeaponAttribute && info.Attribute is AosWeaponAttribute && (AosWeaponAttribute)attribute == (AosWeaponAttribute)info.Attribute)
                    return info.AttributeName;

                if (attribute is SkillName && info.Attribute is SkillName && (SkillName)attribute == (SkillName)info.Attribute)
                    return info.AttributeName;

                if (info.Attribute == attribute)
                    return info.AttributeName;
            }

            return null;
        }
    }
}
