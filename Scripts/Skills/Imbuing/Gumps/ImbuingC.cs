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
using Server.SkillHandlers;

namespace Server.Gumps
{
    public class ImbuingGumpC : Gump
    {
        private const int LabelHue = 0x480;
        private const int LabelColor = 0x7FFF;  //Localized
        private const int FontColor = 0xFFFFFF; //string
        private const int ValueColor = 0xCCCCFF;

        public const int MaxProps = 5;

        private int m_Mod, m_Value;
        private Item m_Item;
        private int m_GemAmount = 0, m_PrimResAmount = 0, m_SpecResAmount = 0;
		private int m_TotalItemWeight;
		private int m_TotalProps;
		private int m_PropWeight;
		private int m_MaxWeight;
		
        private ImbuingDefinition m_Definition;

        public ImbuingGumpC(Mobile from, Item item, int mod, int value) : base(520, 340)
        {
            PlayerMobile m = from as PlayerMobile;

            from.CloseGump(typeof(ImbuingGump));
            from.CloseGump(typeof(ImbuingGumpB));

            // SoulForge Check
            if (!Imbuing.CheckSoulForge(from, 1))
                return;

            ImbuingContext context = Imbuing.GetContext(m);

            // = Check Type of Ingredients Needed 
            if (!Imbuing.Table.ContainsKey(mod))
                return;

            m_Definition = Imbuing.Table[mod];

            if (value == -1)
                value = m_Definition.IncAmount;

            m_Item = item;
            m_Mod = mod;
            m_Value = value;

            int maxInt = Imbuing.GetMaxIntensity(item, m_Definition);
            int inc = m_Definition.IncAmount;
            int weight = m_Definition.Weight;

            if (m_Item is BaseJewel && m_Mod == 12)
                maxInt /= 2;

            if (m_Value < inc) m_Value = inc;
            if (m_Value > maxInt) m_Value = maxInt;

            if (m_Value <= 0)
                m_Value = 1;

            double currentIntensity = Math.Floor((m_Value / (double)maxInt) * 100);

			//Set context
			context.LastImbued = item;
            context.Imbue_Mod = mod;
            context.Imbue_ModVal = weight;
            context.ImbMenu_ModInc = inc;
            context.Imbue_ModInt = value;

			// - Current Mod Weight
            m_TotalItemWeight = Imbuing.GetTotalWeight(m_Item, m_Mod); 
            m_TotalProps = Imbuing.GetTotalMods(m_Item, m_Mod);
			
            if (maxInt <= 1)
				currentIntensity= 100;

            m_PropWeight = (int)Math.Floor(((double)weight / (double)maxInt) * m_Value);

            // - Maximum allowed Property Weight & Item Mod Count
            m_MaxWeight = Imbuing.GetMaxWeight(m_Item);
			
            // = Times Item has been Imbued
            int timesImbued = 0;
            if (m_Item is BaseWeapon) 
                timesImbued = ((BaseWeapon)m_Item).TimesImbued;
            if (m_Item is BaseArmor)
                timesImbued = ((BaseArmor)m_Item).TimesImbued;
            if (m_Item is BaseJewel)
                timesImbued = ((BaseJewel)m_Item).TimesImbued;
            if (m_Item is BaseHat)
                timesImbued = ((BaseHat)m_Item).TimesImbued;

            // = Check Ingredients needed at the current Intensity
            m_GemAmount = Imbuing.GetGemAmount(m_Item, m_Mod, m_Value);
            m_PrimResAmount = Imbuing.GetPrimaryAmount(m_Item, m_Mod, m_Value);
            m_SpecResAmount = Imbuing.GetSpecialAmount(m_Item, m_Mod, m_Value);

            // ------------------------------ Gump Menu -------------------------------------------------------------
            AddPage(0);
            AddBackground(0, 0, 540, 450, 5054);
            AddImageTiled(10, 10, 520, 430, 2624);

            AddImageTiled(10, 35, 520, 10, 5058);
            AddImageTiled(260, 45, 15, 290, 5058);
            AddImageTiled(10, 185, 520, 10, 5058);
            AddImageTiled(10, 335, 520, 10, 5058);
            AddImageTiled(10, 405, 520, 10, 5058);

            AddAlphaRegion(10, 10, 520, 430);

            AddHtmlLocalized(10, 13, 520, 18, 1079717, LabelColor, false, false); //<CENTER>IMBUING CONFIRMATION</CENTER>
            AddHtmlLocalized(57, 49, 200, 18, 1114269, LabelColor, false, false); //PROPERTY INFORMATION

            // - Attribute to Imbue
            AddHtmlLocalized(30, 80, 80, 17, 1114270, LabelColor, false, false);  //Property:
            AddHtmlLocalized(100, 80, 150, 17, m_Definition.AttributeName, LabelColor, false, false);

            // - Weight Modifier
            AddHtmlLocalized(30, 120, 80, 17, 1114272, 0xFFFFFF, false, false);   //Weight:

            double w = (double)m_Definition.Weight / 100.0;
            AddHtml(90, 120, 80, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}x", w), false, false);

            AddHtmlLocalized(30, 140, 80, 17, 1114273, LabelColor, false, false);   //Intensity:
            AddHtml(90, 140, 80, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}%", currentIntensity), false, false);

            // - Materials needed
            AddHtmlLocalized(10, 199, 255, 18, 1044055, LabelColor, false, false); //<CENTER>MATERIALS</CENTER>
            AddHtmlLocalized(40, 230, 180, 17, m_Definition.PrimaryName, LabelColor, false, false);
            AddHtml(210, 230, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}", m_PrimResAmount.ToString()), false, false);
            AddHtmlLocalized(40, 255, 180, 17, m_Definition.GemName, LabelColor, false, false);
            AddHtml(210, 255, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}", m_GemAmount.ToString()), false, false);
            if (m_SpecResAmount > 0)
            {
                AddHtmlLocalized(40, 280, 180, 17, m_Definition.SpecialName, LabelColor, false, false);
                AddHtml(210, 280, 40, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}", m_SpecResAmount.ToString()), false, false);
            }

            // - Mod Description
            AddHtmlLocalized(290, 65, 215, 110, m_Definition.Description, LabelColor, false, false); 

            AddHtmlLocalized(365, 199, 150, 18, 1113650, LabelColor, false, false); //RESULTS
			
            AddHtmlLocalized(288, 220, 150, 17, 1113645, LabelColor, false, false);                                 //Properties:
            AddHtml(443, 220, 80, 17, String.Format("<BASEFONT COLOR=#CCFFCC> {0}/5", m_TotalProps + 1), false, false);
            AddHtmlLocalized(288, 240, 150, 17, 1113646, LabelColor, false, false);                                   //Total Property Weight:
            AddHtml(443, 240, 80, 17, String.Format("<BASEFONT COLOR=#CCFFCC> {0}/{1}", m_TotalItemWeight + (int)m_PropWeight, m_MaxWeight), false, false);

            // - Times Imbued
            AddHtmlLocalized(288, 260, 150, 17, 1113647, LabelColor, false, false);                                   //Times Imbued:
            AddHtml(443, 260, 80, 17, String.Format("<BASEFONT COLOR=#CCFFCC> {0}/20", timesImbued + 1), false, false);

            // - Name of Attribute to be Replaced
            int replace = WhatReplacesWhat(m_Mod, m_Item);
            AddHtmlLocalized(30, 100, 80, 17, 1114271, LabelColor, false, false);
            if (replace <= 0)
                replace = m_Definition.AttributeName;

            AddHtmlLocalized(100, 100, 150, 17, replace, LabelColor, false, false);

            // ===== CALCULATE DIFFICULTY =====
            double dif;
            double suc = Imbuing.GetSuccessChance(from, item, m_TotalItemWeight, m_PropWeight, out dif);

            int Succ = Convert.ToInt32(suc);
            string color;

            // = Imbuing Success Chance % 
            AddHtmlLocalized(305, 300, 150, 17, 1044057, 0xFFFFFF, false, false);
            if (Succ <= 1) color = "#FF5511";
            else if (Succ > 1 && Succ < 10) color = "#EE6611";
            else if (Succ >= 10 && Succ < 20) color = "#DD7711";
            else if (Succ >= 20 && Succ < 30) color = "#CC8811";
            else if (Succ >= 30 && Succ < 40) color = "#BB9911";
            else if (Succ >= 40 && Succ < 50) color = "#AAAA11";
            else if (Succ >= 50 && Succ < 60) color = "#99BB11";
            else if (Succ >= 60 && Succ < 70) color = "#88CC11";
            else if (Succ >= 70 && Succ < 80) color = "#77DD11";
            else if (Succ >= 80 && Succ < 90) color = "#66EE11";
            else if (Succ >= 90 && Succ < 100) color = "#55FF11";
            else if (Succ >= 100) color = "#01FF01";
            else color = "#FFFFFF";

            if (suc > 100) suc = 100;
            if (suc < 0) suc = 0;

            AddHtml(430, 300, 80, 17, String.Format("<BASEFONT COLOR={0}>\t{1}%", color, suc), false, false);

            // - Attribute Level
            if (maxInt > 1)
            {
                // - Set Intesity to Minimum
                if (m_Value <= 0)
                    m_Value = 1;

                // = New Value:
                AddHtmlLocalized(245, 350, 100, 17, 1062300, LabelColor, false, false); 
                // - Mage Weapon Value ( i.e [Mage Weapon -25] )
                if (m_Mod == 41)
					AddHtml(254, 374, 50, 17, String.Format("<BASEFONT COLOR=#CCCCFF> -{0}", 30 - m_Value), false, false);
                // - Show Property Value as % ( i.e [Hit Fireball 25%] )
                else if (maxInt <= 8 || m_Mod == 21 || m_Mod == 17) 
                    AddHtml(254, 374, 50, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}", m_Value), false, false);
                // - Show Property Value as just Number ( i.e [Mana Regen 2] )
                else
					AddHtml(254, 374, 50, 17, String.Format("<BASEFONT COLOR=#CCCCFF> {0}%", m_Value), false, false);

                // == Buttons ==
                //0x1467???
                AddButton(192, 376, 5230, 5230, 10053, GumpButtonType.Reply, 0); // To Minimum Value
                AddButton(211, 376, 5230, 5230, 10052, GumpButtonType.Reply, 0); // Dec Value by %
                AddButton(230, 376, 5230, 5230, 10051, GumpButtonType.Reply, 0); // dec value by 1

                AddButton(331, 376, 5230, 5230, 10056, GumpButtonType.Reply, 0); //To Maximum Value
                AddButton(312, 376, 5230, 5230, 10055, GumpButtonType.Reply, 0); // Inc Value by %
                AddButton(293, 376, 5230, 5230, 10054, GumpButtonType.Reply, 0); // inc Value by 1

                AddLabel(341, 374, 0, ">");
                AddLabel(337, 374, 0, ">");
                AddLabel(333, 374, 0, ">");

                AddLabel(320, 374, 0, ">");
                AddLabel(316, 374, 0, ">");

                AddLabel(298, 374, 0, ">");

                AddLabel(235, 374, 0, "<");

                AddLabel(216, 374, 0, "<");
                AddLabel(212, 374, 0, "<");

                AddLabel(199, 374, 0, "<");
                AddLabel(195, 374, 0, "<");
                AddLabel(191, 374, 0, "<");
            }

            AddButton(19, 416, 4005, 4007, 10099, GumpButtonType.Reply, 0);
            AddHtmlLocalized(58, 417,  100, 18, 1114268, LabelColor, false, false);           //Back 

            AddButton(400, 416, 4005, 4007, 10100, GumpButtonType.Reply, 0);
            AddHtmlLocalized(439, 417, 120, 18, 1114267, LabelColor, false, false);         //Imbue Item
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            PlayerMobile pm = from as PlayerMobile;

            ImbuingContext context = Imbuing.GetContext(pm);

            int buttonNum = 0;
            if (info.ButtonID > 0 && info.ButtonID < 10000)
                buttonNum = 0;
            else if (info.ButtonID > 20004)
                buttonNum = 30000;
            else
                buttonNum = info.ButtonID;

            switch (buttonNum)
            {
                case 0:
                    {
                        //Close
                        break;
                    }
                case 10051: // = Decrease Mod Value [<]
                    {
                        if (context.Imbue_ModInt > m_Definition.IncAmount)
                            context.Imbue_ModInt -= m_Definition.IncAmount;

                        from.SendGump(new ImbuingGumpC(from, m_Item, context.Imbue_Mod, context.Imbue_ModInt));
                        break;
                    }
                case 10052:// = Decrease Mod Value [<<]
                    {
                        if ((m_Mod == 42 || m_Mod == 24) && context.Imbue_ModInt > 20)
                            context.Imbue_ModInt -= 20;
                        if ((m_Mod == 13 || m_Mod == 20 || m_Mod == 21) && context.Imbue_ModInt > 10)
                            context.Imbue_ModInt -= 10;
                        else  if (context.Imbue_ModInt > 5)
                            context.Imbue_ModInt -= 5;

                        from.SendGump(new ImbuingGumpC(from, context.LastImbued, context.Imbue_Mod, context.Imbue_ModInt));
                        break;
                    }
                case 10053:// = Minimum Mod Value [<<<]
                    {
                        context.Imbue_ModInt = 1;
                        from.SendGump(new ImbuingGumpC(from, context.LastImbued, context.Imbue_Mod, context.Imbue_ModInt));
                        break;
                    }
                case 10054: // = Increase Mod Value [>]
                    {
                        int max = Imbuing.GetMaxIntensity(m_Item, m_Definition);

                        if(m_Mod == 12 && context.LastImbued is BaseJewel)
                            max /= 2;

                        if (context.Imbue_ModInt + m_Definition.IncAmount <= max)
                            context.Imbue_ModInt += m_Definition.IncAmount;

                        from.SendGump(new ImbuingGumpC(from, context.LastImbued, context.Imbue_Mod, context.Imbue_ModInt));
                        break;
                    }
                case 10055: // = Increase Mod Value [>>]
                    {
                        int max = Imbuing.GetMaxIntensity(m_Item, m_Definition);

                        if (m_Mod == 12 && context.LastImbued is BaseJewel)
                            max /= 2;

                        if (m_Mod == 42 || m_Mod == 24)
                        {
                            if (context.Imbue_ModInt + 20 <= max)
                                context.Imbue_ModInt += 20;
                            else
                                context.Imbue_ModInt = max;
                        }
                        if (m_Mod == 13 || m_Mod == 20 || m_Mod == 21)
                        {
                            if (context.Imbue_ModInt + 10 <= max)
                                context.Imbue_ModInt += 10;
                            else
                                context.Imbue_ModInt = max;
                        }
                        else if (context.Imbue_ModInt + 5 <= max)
                            context.Imbue_ModInt += 5;
                        else
                            context.Imbue_ModInt = Imbuing.GetMaxIntensity(m_Item, m_Definition);

                        from.SendGump(new ImbuingGumpC(from, context.LastImbued, context.Imbue_Mod, context.Imbue_ModInt));
                        break;
                    }
                case 10056: // = Maximum Mod Value [>>>]
                    {
                        int max = Imbuing.GetMaxIntensity(m_Item, m_Definition);

                        if (m_Mod == 12 && context.LastImbued is BaseJewel)
                            max /= 2;

                        context.Imbue_ModInt = max;
                        from.SendGump(new ImbuingGumpC(from, context.LastImbued, context.Imbue_Mod, context.Imbue_ModInt));
                        break;
                    }

                case 10099: // - Back
                    {
                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10100:  // = Imbue the Item
                    {
                        context.Imbue_IWmax = m_MaxWeight;

                        if (Imbuing.OnBeforeImbue(from, m_Item, m_Mod, m_Value, m_TotalProps, MaxProps, m_TotalItemWeight, m_MaxWeight))
                        {
                            Imbuing.ImbueItem(from, m_Item, m_Mod, m_Value);
                            SendGumpDelayed(from);
                        }

                        break;
                    }
            }
        }

        public void SendGumpDelayed(Mobile from)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), new TimerStateCallback(SendGump_Callback), from);
        }

        public void SendGump_Callback(object o)
        {
            Mobile from = o as Mobile;

            if (from != null)
                from.SendGump(new ImbuingGump(from));
        }

        // =========== Check if Choosen Attribute Replaces Another =================
        public static int WhatReplacesWhat(int mod, Item item)
        {
            if (item is BaseWeapon)
            {
                BaseWeapon i = item as BaseWeapon;

                // Slayers replace Slayers
                if (mod >= 101 && mod <= 127)
                {
                    if (i.Slayer != SlayerName.None)
                        return GetNameForAttribute(i.Slayer);

                    if (i.Slayer2 != SlayerName.None)
                        return GetNameForAttribute(i.Slayer2);
                }
                // OnHitEffect replace OnHitEffect
                if (mod >= 35 && mod <= 39)
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
                if (mod >= 30 && mod <= 34)
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
                // OnHitLeech replace OnHitLeech
                /*if (mod >= 25 && mod <= 27)
                {
                    if (i.WeaponAttributes.HitLeechHits > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitLeechHits);
                    else if (i.WeaponAttributes.HitLeechStam > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitLeechStam);
                    else if (i.WeaponAttributes.HitLeechMana > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitLeechMana);
                }
                // HitLower replace HitLower 
                if (mod >= 28 && mod <= 29)
                {
                    if (i.WeaponAttributes.HitLowerAttack > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitLowerAttack);
                    else if (i.WeaponAttributes.HitLowerDefend > 0)
                        return GetNameForAttribute(AosWeaponAttribute.HitLowerDefend);
                }*/
            }
            if (item is BaseJewel)
            {
                BaseJewel i = item as BaseJewel;

                // SkillGroup1 replace SkillGroup1
                if (mod >= 151 && mod <= 155)
                {
                    if (i.SkillBonuses.GetBonus(0) > 0)
                    {
                        foreach (SkillName sk in Imbuing.PossibleSkills)
                        {
                            if(i.SkillBonuses.GetSkill(0) == sk)
                                return GetNameForAttribute(sk);
                        }
                    }
                }
                // SkillGroup2 replace SkillGroup2
                if (mod >= 156 && mod <= 160)
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
                if (mod >= 161 && mod <= 166)
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
                if (mod >= 167 && mod <= 172)
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
                if (mod >= 173 && mod <= 178)
                {
                    if (i.SkillBonuses.GetBonus(4) > 0)
                    {
                        foreach (SkillName sk in Imbuing.PossibleSkills)
                        {
                            if (i.SkillBonuses.GetSkill(4) == sk)
                                return GetNameForAttribute(sk);
                        }
                    }
                }
            }

            return -1;
        }

        public static int GetNameForAttribute(object attribute)
        {
            if(attribute is AosArmorAttribute && (AosArmorAttribute)attribute == AosArmorAttribute.LowerStatReq)
                attribute = AosWeaponAttribute.LowerStatReq;

            if (attribute is AosArmorAttribute && (AosArmorAttribute)attribute == AosArmorAttribute.DurabilityBonus)
                attribute = AosWeaponAttribute.DurabilityBonus;

            foreach (ImbuingDefinition def in Imbuing.Table.Values)
            {
                if (attribute is SlayerName && def.Attribute is SlayerName && (SlayerName)attribute == (SlayerName)def.Attribute)
                    return def.AttributeName;

                if (attribute is AosAttribute && def.Attribute is AosAttribute && (AosAttribute)attribute == (AosAttribute)def.Attribute)
                    return def.AttributeName;

                if (attribute is AosWeaponAttribute && def.Attribute is AosWeaponAttribute && (AosWeaponAttribute)attribute == (AosWeaponAttribute)def.Attribute)
                    return def.AttributeName;

                if (attribute is SkillName && def.Attribute is SkillName && (SkillName)attribute == (SkillName)def.Attribute)
                    return def.AttributeName;

                if (def.Attribute == attribute)
                    return def.AttributeName;
            }

            return -1;
        }
    }                
}