using System;
using Server;
using Server.Network;
using System.Collections.Generic;
using Server.Targeting;
using Server.Items;

namespace Server.Gumps
{
    public class RefinementGump : Gump
    {
        public const int DarkGreen = 10000;
        public const int White = 0x7FFF;
        public const int WhiteLabel = 0xFFFFFF;
        public const int Yellow = 0xFFE0;

        private RefinementItem m_Item;
        private ModEntry m_Entry;
        private bool m_CanRefine;

        public RefinementGump(RefinementItem item) : base(50, 50)
        {
            m_Item = item;
            m_Entry = item.Entry;

            if (m_Entry == null)
                return;

            AddBackground(0, 0, 400, 350, 83);

            int y = 70;
            m_CanRefine = true;

            AddHtmlLocalized(10, 10, 200, 20, 1154094, DarkGreen, false, false); // Refinement Crafting Options
            AddHtmlLocalized(10, 35, 200, 20, 1153967, m_Item.GetNameArgs(), DarkGreen, false, false);

            for (int i = 0; i < m_Item.ModAmount; i++)
            {
                AddButton(15, y, 4005, 4007, i + 100, GumpButtonType.Reply, 0);

                AddHtmlLocalized(55, y, 150, 16, 1154097 + i, WhiteLabel, false, false); // CHOOSE RESIST #1
                AddHtmlLocalized(230, y, 150, 16, GetResistanceLabel(m_Entry.Resists[i], m_Entry.Values[i]), WhiteLabel, false, false);

                if (!m_Item.CheckBonus && i == m_Item.ModAmount - 1 && m_Item.GetBonusChance() > 0)
                {
                    if (Utility.Random(100) <= m_Item.GetBonusChance())
                        m_Item.ModAmount++;

                    m_Item.CheckBonus = true;
                }

                if (m_CanRefine && m_Entry.Values[i] == 0)
                    m_CanRefine = false;

                y += 20;
            }

            y += 50;

            AddHtmlLocalized(10, y, 150, 16, 1154106, Yellow, false, false); // MODIFICATIONS:
            AddHtmlLocalized(170, y, 10, 16, 1114057, m_Item.ModAmount.ToString(), Yellow, false, false);

            if (m_CanRefine)
            {
                AddHtmlLocalized(230, y, 150, 16, 1154105, WhiteLabel, false, false); // REFINE ITEM
                AddButton(360, y, 4014, 4016, 1, GumpButtonType.Reply, 0);
            }
            else
                AddHtmlLocalized(230, y, 150, 16, 1154108, WhiteLabel, false, false); // SELECT RESISTS

            y += 25;

            AddHtmlLocalized(10, y, 200, 16, 1154107, WhiteLabel, false, false); // BONUS MOD CHANCE:
            AddHtmlLocalized(170, y, 15, 16, 1114057, m_Item.GetBonusChance().ToString() + "%", WhiteLabel, false, false);

            AddButton(10, 320, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 320, 100, 16, 3002084, WhiteLabel, false, false); // Close

            AddHtmlLocalized(250, 320, 100, 16, 3000391, WhiteLabel, false, false); // Help
            AddButton(360, 320, 4011, 4013, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID == 1) // Refine Item
            {
                if (m_CanRefine)
                {
                    from.Target = new InternalTarget(m_Item);
                    from.SendLocalizedMessage(1153985); // Target the armor you wish to apply this Refinement.
                }
            }
            else if (info.ButtonID == 2)
            {
                from.SendGump(new RefinementHelpGump(m_Item.CraftType));
            }
            else if (info.ButtonID >= 100) // Choose Resist
            {
                int i = info.ButtonID - 100;
                ResistanceType oldType = m_Entry.Resists[i];
                int value = (int)oldType;

                if (value == 4) value = 0;
                else value++;

                ResistanceType newType = (ResistanceType)value;

                m_Entry.Resists[i] = newType;
                m_Entry.Values[i] = m_Item.RefinementType == RefinementType.Reinforcing ? 1 : -1;

                for (int idx = 0; idx < m_Entry.Resists.Length; idx++)
                {
                    if (idx != i && m_Entry.Resists[idx] == newType)
                    {
                        m_Entry.Resists[idx] = oldType;
                        break;
                    }
                }

                from.SendGump(new RefinementGump(m_Item));
            }
        }

        private class InternalTarget : Target
        {
            private RefinementItem m_Item;
            private ModEntry m_Entry;

            public InternalTarget(RefinementItem item) : base(-1, false, TargetFlags.None)
            {
                m_Item = item;
                m_Entry = item.Entry;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Item == null || m_Item.Deleted)
                    return;

                if (targeted is BaseArmor)
                {
                    BaseArmor armor = targeted as BaseArmor;

                    if (!armor.IsChildOf(from.Backpack))
                        from.SendLocalizedMessage(1054107); // This item must be in your backpack.
                    else if (armor.ArmorAttributes.MageArmor > 0)
                        from.SendLocalizedMessage(1153986); // You cannot refine this piece of armor! 
                    else if (!RefinementGump.CanApplyToArmor(from, armor, m_Item))
                        from.SendLocalizedMessage(1153987); // The type of armor you have selected is not compatible with this Refinement Tool. 
                    else if(RefinementItem.CheckForVendor(from, m_Item))
                    {
                        //Resets old refinement if it exists
                        armor.RefinedPhysical = 0;
                        armor.RefinedFire = 0;
                        armor.RefinedCold = 0;
                        armor.RefinedPoison = 0;
                        armor.RefinedEnergy = 0;

                        for (int i = 0; i < m_Item.ModAmount; i++)
                        {
                            if (i < 0 || i >= m_Entry.Resists.Length)
                                continue;

                            int value = i < m_Entry.Values.Length ? m_Entry.Values[i] : 0;

                            switch (m_Entry.Resists[i])
                            {
                                case ResistanceType.Physical: armor.RefinedPhysical += value; break;
                                case ResistanceType.Fire: armor.RefinedFire += value; break;
                                case ResistanceType.Cold: armor.RefinedCold += value; break;
                                case ResistanceType.Poison: armor.RefinedPoison += value; break;
                                case ResistanceType.Energy: armor.RefinedEnergy += value; break;
                            }
                        }

                        from.PrivateOverheadMessage(MessageType.Regular, 0, 1153980, from.NetState); // *You carefully apply the Refinement to the armor*
                        m_Item.Delete();
                        return;
                    }
                }
                else
                    from.SendLocalizedMessage(1153991); // You can only refine non-medable armor. 

                from.SendGump(new RefinementGump(m_Item));
            }
        }

        public int GetResistanceLabel(ResistanceType attr, int value)
        {
            if(value == 0)
                return 1062648; // None Selected

            switch (attr)
            {
                default:
                case ResistanceType.Physical: return 1061158;
                case ResistanceType.Fire: return 1061159;
                case ResistanceType.Cold: return 1061160;
                case ResistanceType.Poison: return 1061161;
                case ResistanceType.Energy: return 1061162;
            }
        }

        public static bool CanApplyToArmor(Mobile from, BaseArmor armor, RefinementItem item)
        {
            switch (item.SubCraftType)
            {
                case RefinementSubCraftType.StuddedLeather:
                    return armor is StuddedBustierArms || armor is StuddedArms || armor is StuddedChest || armor is StuddedGloves || armor is StuddedGorget || armor is StuddedLegs;
                case RefinementSubCraftType.StuddedSamurai:
                    return armor is StuddedDo || armor is StuddedHaidate || armor is StuddedHiroSode || armor is StuddedMempo || armor is StuddedSuneate;
                case RefinementSubCraftType.Hide:
                    return armor is HideChest || armor is HideFemaleChest || armor is HideGloves || armor is HideGorget || armor is HidePants || armor is HidePauldrons;
                case RefinementSubCraftType.Bone:
                    return armor is BoneArms || armor is BoneChest || armor is BoneGloves || armor is BoneLegs;
                case RefinementSubCraftType.Ringmail:
                    return armor is RingmailArms || armor is RingmailChest || armor is RingmailGloves || armor is RingmailLegs;
                case RefinementSubCraftType.Chainmail:
                    return armor is ChainChest || armor is ChainLegs || armor is ChainCoif;
                case RefinementSubCraftType.Platemail:
                    return armor is FemalePlateChest || armor is PlateArms || armor is PlateChest || armor is PlateGloves || armor is PlateGorget || armor is PlateLegs || armor is PlateHelm || armor is Bascinet || armor is CloseHelm || armor is Helmet;
                case RefinementSubCraftType.PlatemailSamurai:
                    return armor is DecorativePlateKabuto || armor is HeavyPlateJingasa || armor is LightPlateJingasa || armor is PlateBattleKabuto || armor is PlateDo || armor is PlateHaidate || armor is PlateHatsuburi || armor is PlateHiroSode
                            || armor is PlateMempo || armor is PlateSuneate || armor is SmallPlateJingasa || armor is StandardPlateKabuto;
                case RefinementSubCraftType.GargishPlatemail:
                    return armor is GargishPlateArms || armor is GargishPlateChest || armor is GargishPlateKilt || armor is GargishPlateLegs || armor is GargishPlateWingArmor;
                case RefinementSubCraftType.Dragon:
                    return armor is DragonArms || armor is DragonChest || armor is DragonGloves || armor is DragonHelm || armor is DragonLegs;
                case RefinementSubCraftType.Woodland:
                    return armor is WoodlandArms || armor is WoodlandChest || armor is WoodlandGloves || armor is WoodlandGorget || armor is WoodlandLegs;
                case RefinementSubCraftType.GargishStone:
                    return armor is GargishStoneArms || armor is GargishStoneChest || armor is GargishStoneLegs || armor is GargishStoneKilt || armor is GargishStoneWingArmor;
            }

            return false;
        }
    }

    public class RefinementHelpGump : Gump
    {
        public RefinementHelpGump(RefinementCraftType type)
            : base(50, 50)
        {
            AddImageTiled(50, 20, 400, 400, 0x1404);
            AddImageTiled(50, 29, 30, 390, 0x28DC);
            AddImageTiled(34, 140, 17, 279, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 419, 0x2842);
            AddImage(442, 419, 0x2840);
            AddImageTiled(51, 419, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 390, 0xA2D);
            AddImageTiled(415, 29, 30, 390, 0x28DC);
            AddImage(370, 50, 0x589);

            AddImage(379, 60, 0x15A9);
            AddImage(425, 0, 0x28C9);
            AddImage(90, 33, 0x232D);
            AddImageTiled(130, 65, 175, 1, 0x238D);

            AddPage(0);
            int cliloc = 0;

            switch (type)
            {
                case RefinementCraftType.Blacksmith: cliloc = 1153992; break;
                case RefinementCraftType.Tailor: cliloc = 1153993; break;
                case RefinementCraftType.Carpenter: cliloc = 1153994; break;
            }

            AddHtmlLocalized(130, 45, 270, 16, 1154001, 0xFFFFFF, false, false); // Armor Refinement

            AddButton( 313, 395, 0x2EEC, 0x2EEE, 0, GumpButtonType.Reply, 0 );

            AddPage(1);

            AddHtmlLocalized(98, 140, 312, 105, cliloc, 0xFFFFFF, false, true);

            AddButton(98, 255, 9904, 9905, 1, GumpButtonType.Page, 2);
            AddHtmlLocalized(125, 255, 250, 20, 1153995, 0xFFFFFF, false, false); // What is an Armor Refinement? 

            AddButton(98, 275, 9904, 9905, 1, GumpButtonType.Page, 3);
            AddHtmlLocalized(125, 275, 250, 20, 1153996, 0xFFFFFF, false, false); // Where can I find components? 

            AddButton(98, 295, 9904, 9905, 1, GumpButtonType.Page, 4);
            AddHtmlLocalized(125, 295, 250, 20, 1153997, 0xFFFFFF, false, false); // What raw materials do I need to use?

            AddButton(98, 315, 9904, 9905, 1, GumpButtonType.Page, 5);
            AddHtmlLocalized(125, 315, 250, 20, 1154006, 0xFFFFFF, false, false); // Where can I find raw materials?

            AddPage(2);

            AddHtmlLocalized(98, 140, 312, 205, 1153998, 0xFFFFFF, false, true);

            AddPage(3);

            AddHtmlLocalized(98, 140, 312, 205, 1153999, 0xFFFFFF, false, true);

            AddPage(4);

            AddHtmlLocalized(98, 140, 312, 205, 1154000, 0xFFFFFF, false, true);

            AddPage(5);

            AddHtmlLocalized(98, 140, 312, 205, 1154007, 0xFFFFFF, false, true);
        }
    }
}