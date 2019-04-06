using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.SkillHandlers;

namespace Server.Gumps
{
    public class ImbuingGumpB : Gump
    {
        public Item m_Item;
        private const int LabelColor = 0x7FFF;

        public ImbuingGumpB(Mobile from, Item item)
            : base(50, 50)
        {
            from.CloseGump(typeof(ImbuingGump));
            from.CloseGump(typeof(ImbuingGumpC));

            // SoulForge Check
            if (!Imbuing.CheckSoulForge(from, 2))
                return;

            Mobile m = from;
            PlayerMobile pm = from as PlayerMobile;

            ImbuingContext context = Imbuing.GetContext(m);

            m_Item = context.LastImbued;

            int itemRef = ImbuingGump.GetItemRef(m_Item);
            bool twoHanded = item.Layer == Layer.TwoHanded;

            AddPage(0);
            AddBackground(0, 0, 520, 520, 5054);
            AddImageTiled(10, 10, 500, 500, 2624);
            AddImageTiled(10, 30, 500, 10, 5058);
            AddImageTiled(230, 40, 10, 440, 5058);
            AddImageTiled(10, 480, 500, 10, 5058);

            AddAlphaRegion(10, 10, 520, 500);

            AddHtmlLocalized(10, 12, 520, 20, 1079588, LabelColor, false, false); //IMBING MENU
            int yOffset = 0;

            // ===== Attribute Catagories ========================================
            AddHtmlLocalized(10, 60, 220, 20, 1044010, LabelColor, false, false); // <CENTER>CATEGORIES</CENTER>
            AddHtmlLocalized(240, 60, 270, 20, 1044011, LabelColor, false, false); // <CENTER>SELECTIONS</CENTER>

            AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10001, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114248, LabelColor, false, false);       //Casting
            yOffset += 1;

            if (itemRef == 1 || itemRef == 2 || itemRef == 4 || itemRef == 6)
            {
                AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10002, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114249, LabelColor, false, false);   //Combat
                yOffset += 1;
            }

            if (itemRef == 1 || itemRef == 2)
            {
                AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10006, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114250, LabelColor, false, false);   //Hit Area Effects
                yOffset += 1;

                AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10007, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114251, LabelColor, false, false);   //Hit Effects
                yOffset += 1;
            }

            AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10003, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114252, LabelColor, false, false);       //Misc.
            yOffset += 1;

            if (itemRef == 2)
            {
                AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10015, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114253, LabelColor, false, false);   //Ranged
                yOffset += 1;
            }

            if (itemRef == 1 || itemRef == 2 || itemRef == 3 || itemRef == 5 || itemRef == 6)
            {
                AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10004, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114254, LabelColor, false, false);   //Resists
                yOffset += 1;
            }            

            if (itemRef == 1 || itemRef == 2)
            {
                AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10008, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114263, LabelColor, false, false);   //Slayers
                yOffset += 1;

                AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10009, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114264, LabelColor, false, false);   //Super Slayers
                yOffset += 1;
            }

            if (itemRef == 6)
            {
                for (int i = 0; i < 5; i++)
                {
                    AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10010 + i, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114255 + i, LabelColor, false, false);       //Skill Group 1
                    yOffset += 1;
                }
            }

            if (itemRef == 3 || itemRef == 5 || itemRef == 6)
            {
                AddButton(15, 90 + (yOffset * 25), 4005, 4007, 10005, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 90 + (yOffset * 25), 150, 18, 1114262, LabelColor, false, false);   //Stats
                yOffset += 1;
            }

            // ===== Attribute Catagories ========================================
            yOffset = 0;
            int menuCat = context.ImbMenu_Cat;

            if (menuCat == 1) // == CASTING ==
            {
                if (itemRef == 1 || itemRef == 2)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10122, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079766, LabelColor, false, false);       //Spell Channeling
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10141, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079759, LabelColor, false, false);       //Mage Weapon 
                    yOffset += 1;

                    if (item is BaseWeapon && (((BaseWeapon)item).Attributes.SpellChanneling == 0 || ((BaseWeapon)item).Attributes.CastSpeed < 0))
                    {
                        AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10116, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075617, LabelColor, false, false);       //Faster Casting
                        yOffset += 1;
                    }
                }
                else if (itemRef == 3 || itemRef == 5)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10118, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075625, LabelColor, false, false);       //Lower Reg Cost
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10117, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075621, LabelColor, false, false);       //Lower Mana Cost
                    yOffset += 1;

                }
                else if (itemRef == 4)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10122, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079766, LabelColor, false, false);       //Spell Channeling 
                    yOffset += 1;

                    if (item is BaseShield && (((BaseShield)item).Attributes.SpellChanneling == 0 || ((BaseShield)item).Attributes.CastSpeed < 0))
                    {
                        AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10116, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075617, LabelColor, false, false);       //Faster Casting
                        yOffset += 1;
                    }
                }
                else if (itemRef == 6)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10114, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075628, LabelColor, false, false);       //Spell Damage Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10118, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075625, LabelColor, false, false);       //Lower Reg Cost
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10117, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075621, LabelColor, false, false);       //Lower Mana Cost
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10116, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075617, LabelColor, false, false);       //Faster Casting
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10115, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075618, LabelColor, false, false);       //Faster Cast Recovery 
                    yOffset += 1;
                }
            }
            else if (menuCat == 2) // == COMBAT ==
            {
                if (itemRef == 1)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10112, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079399, LabelColor, false, false);       //Damage Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10113, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075629, LabelColor, false, false);       //Swing Speed Increase
                    yOffset += 1;

                    if (twoHanded)
                    {
                        AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10161, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1072792, LabelColor, false, false);   //Balanced
                        yOffset += 1;
                    }

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10102, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075616, LabelColor, false, false);       //Hit Chance Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10101, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075620, LabelColor, false, false);       //Defense Chance Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10140, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079592, LabelColor, false, false);       //Use Best Weapon Skill
                    yOffset += 1;
                }
                else if (itemRef == 2)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10112, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079399, LabelColor, false, false);       //Damage Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10113, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075629, LabelColor, false, false);       //Swing Speed Increase
                    yOffset += 1;

                }
                else if (itemRef == 4)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10101, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075620, LabelColor, false, false);       //Defense Chance Increase
                    yOffset += 1;

                }
                else if (itemRef == 6)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10112, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079399, LabelColor, false, false);       //Damage Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10102, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075616, LabelColor, false, false);       //Hit Chance Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10101, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075620, LabelColor, false, false);       //Defense Chance Increase
                    yOffset += 1;
                }
            }
            else if (menuCat == 3)  // == MISC ==
            {
                if (itemRef == 1 || itemRef == 2)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10121, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061153, LabelColor, false, false);       //Luck
                    yOffset += 1;
                }
                else if (itemRef == 3 || itemRef == 5)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10119, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075626, LabelColor, false, false);       //Reflect Physical Damage
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10123, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1015168, LabelColor, false, false);       //Night Sight
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10121, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061153, LabelColor, false, false);       //Luck
                    yOffset += 1;
                }
                else if (itemRef == 4)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10119, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075626, LabelColor, false, false);       //Reflect Physical Damage
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10124, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079757, LabelColor, false, false);       //Lower Requirements
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10142, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1017323, LabelColor, false, false);       //Durability
                    yOffset += 1;
                }
                else if (itemRef == 6)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10123, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1015168, LabelColor, false, false);       //Night Sight 
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10121, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061153, LabelColor, false, false);       //Luck
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10120, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075624, LabelColor, false, false);       //Enhance Potions
                    yOffset += 1;
                }
            }
            else if (menuCat == 15)  // == Ranged ==
            {
                if (itemRef == 2)
                {
                    if (twoHanded)
                    {
                        AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10160, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1080416, LabelColor, false, false);         //Velocity
                        yOffset += 1;
                    }

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10102, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075616, LabelColor, false, false);       //Hit Chance Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10101, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075620, LabelColor, false, false);       //Defense Chance Increase
                    yOffset += 1;

                    if (twoHanded)
                    {
                        AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10161, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1072792, LabelColor, false, false);   //Balanced
                        yOffset += 1;
                    }
                }
            }
            else if (menuCat == 4) // == RESISTS ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10154, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061161, LabelColor, false, false);           //Poison Resist
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10151, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061158, LabelColor, false, false);           //Physcial Resist
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10152, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061159, LabelColor, false, false);           //Fire Resist
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10155, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061162, LabelColor, false, false);           //Energy Resist
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10153, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061160, LabelColor, false, false);           //Cold Resist
                yOffset += 1;
            }

            else if (menuCat == 5)  // == STATS ==
            {
                if (itemRef == 3 || itemRef == 5)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10110, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075632, LabelColor, false, false);       //Stamina Increase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10103, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075627, LabelColor, false, false);       //Hit Points Regeneration
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10104, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079411, LabelColor, false, false);       //Stamina Regeneration
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10105, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079410, LabelColor, false, false);       //Mana Regeneration
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10111, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075631, LabelColor, false, false);       //Mana Increaase
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10109, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1075630, LabelColor, false, false);       //Hit Point Increase
                    yOffset += 1;
                }
                else if (itemRef == 6)
                {
                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10106, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079767, LabelColor, false, false);       //Strength Bonus
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10108, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079756, LabelColor, false, false);       //Intelligence Bonus                  
                    yOffset += 1;

                    AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10107, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079732, LabelColor, false, false);       //Dexterity Bonus
                    yOffset += 1;
                }
            }
            else if (menuCat == 6)  // == HIT AREA EFFECTS ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10133, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079697, LabelColor, false, false);           //Hit Poison Area
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10130, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079696, LabelColor, false, false);           //Hit Physical Area
                yOffset += 1;
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10131, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079695, LabelColor, false, false);           //Hit Fire Area
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10134, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079694, LabelColor, false, false);           //Hit Energy Area
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10132, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079693, LabelColor, false, false);           //Hit Cold Area
                yOffset += 1;
            }
            else if (menuCat == 7)  // == ON HIT EFFECTS ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10126, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079707, LabelColor, false, false);            // Hit Stam Leech
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10127, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079701, LabelColor, false, false);           //Hit Mana Leech
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10135, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079706, LabelColor, false, false);           //Hit Magic Arrow
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10129, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079700, LabelColor, false, false);           //Hit Lower Defense
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10128, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079699, LabelColor, false, false);           //Hit Lower Attack
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10138, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079705, LabelColor, false, false);           //Hit Lightning
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10125, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079698, LabelColor, false, false);           //Hit Life Leeach
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10136, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079704, LabelColor, false, false);           //Hit Harm                
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10137, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079703, LabelColor, false, false);           //Hit Fireball
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10139, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079702, LabelColor, false, false);           //Hit Dispel
                yOffset += 1;
            }
            else if (menuCat == 8)  // == SLAYERS ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10215, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079755, LabelColor, false, false);           //Water Elemental Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10202, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079754, LabelColor, false, false);           //Troll Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10205, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079753, LabelColor, false, false);           //Terathan Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10212, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079746, LabelColor, false, false);           //Spider Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10220, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079745, LabelColor, false, false);           //Snow Elemental Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10206, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079744, LabelColor, false, false);           //Snake Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10213, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079743, LabelColor, false, false);           //Scorpion Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10217, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079742, LabelColor, false, false);           //Poison Elemental Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10201, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079741, LabelColor, false, false);           //Orc Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10211, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079740, LabelColor, false, false);           //Ophidian Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10203, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079739, LabelColor, false, false);           //Ogre Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10207, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079738, LabelColor, false, false);           //Lizardman Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10208, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079737, LabelColor, false, false);           //Gargoyle Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10214, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079736, LabelColor, false, false);           //Fire Elemental Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10218, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079735, LabelColor, false, false);           //Earth Elemental Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10204, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1061284, LabelColor, false, false);           //Dragon Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10219, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079734, LabelColor, false, false);           //Blood Elemental Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10216, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079733, LabelColor, false, false);           //Air Elemental Slayer
                yOffset += 1;

            }
            else if (menuCat == 9)  // == SUPER SLAYERS ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10221, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079752, LabelColor, false, false);           //Undead Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10223, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079751, LabelColor, false, false);           //Reptile Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10222, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079750, LabelColor, false, false);           //Repond Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10227, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1154652, LabelColor, false, false);           //Fey Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10226, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079749, LabelColor, false, false);           //Elemental Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10224, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079748, LabelColor, false, false);           //Demon Slayer
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10225, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1079747, LabelColor, false, false);           //Arachnid Slayer
                yOffset += 1;

            }
            else if (menuCat == 10)  // == SKILL GROUP 1 ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10251, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044102, LabelColor, false, false);           //Fencing
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10252, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044101, LabelColor, false, false);           //Mace Fighting
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10253, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044100, LabelColor, false, false);           //Swordsmanship
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10254, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044089, LabelColor, false, false);           //Musicianship
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10255, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044085, LabelColor, false, false);           //Magery
                yOffset += 1;
            }
            else if (menuCat == 11)  // == SKILL GROUP 2 ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10256, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044103, LabelColor, false, false);           //Wrestling
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10257, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044095, LabelColor, false, false);           //Animal Taming
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10258, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044092, LabelColor, false, false);           //Spirit Speak
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10259, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044087, LabelColor, false, false);           //Tactics
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10260, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044082, LabelColor, false, false);           //Provocation
                yOffset += 1;
            }
            else if (menuCat == 12)  // == SKILL GROUP 3 ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10261, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044110, LabelColor, false, false);           //Focus
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10262, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044065, LabelColor, false, false);           //Parrying
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10263, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044107, LabelColor, false, false);           //Stealth
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10264, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044106, LabelColor, false, false);           //Meditation
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10265, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044062, LabelColor, false, false);           //Animal Lore
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10266, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044075, LabelColor, false, false);           //Discordance
                yOffset += 1;
            }
            else if (menuCat == 13)  // == SKILL GROUP 4 ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10267, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044115, LabelColor, false, false);           //Mysticism
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10268, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044112, LabelColor, false, false);           //Bushido
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10269, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044109, LabelColor, false, false);           //Necromancy
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10270, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044099, LabelColor, false, false);           //Veterinary
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10271, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044093, LabelColor, false, false);           //Stealing
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10272, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044076, LabelColor, false, false);           //Eval Intelligence
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10273, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044061, LabelColor, false, false);           //Anatomy
                yOffset += 1;
            }
            else if (menuCat == 14)  // == SKILL GROUP 5 ==
            {
                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10274, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044069, LabelColor, false, false);           //Peacemaking
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10280, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044117, LabelColor, false, false);          //Throwing
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10275, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044113, LabelColor, false, false);           //Ninjitsu
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10276, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044111, LabelColor, false, false);           //Chivalary
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10277, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044091, LabelColor, false, false);           //Archery
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10278, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044086, LabelColor, false, false);           //Resist Spells
                yOffset += 1;

                AddButton(250, 90 + (yOffset * 20), 4005, 4007, 10279, GumpButtonType.Reply, 0);
                AddHtmlLocalized(295, 90 + (yOffset * 20), 150, 18, 1044077, LabelColor, false, false);          //Healing
                yOffset += 1;
            }

            AddButton(15, 490, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(50, 490, 150, 20, 1011012, LabelColor, false, false); //Cancel
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            ImbuingContext context = Imbuing.GetContext(from);

            m_Item = context.LastImbued;

            switch (info.ButtonID)
            {
                case 0: // Close/Cancel
                case 1:
                    {
                        from.EndAction(typeof(Imbuing));
                        break;
                    }                
                case 10001:
                    {
                        context.ImbMenu_Cat = 1;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10002:
                    {
                        context.ImbMenu_Cat = 2;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10003:
                    {
                        context.ImbMenu_Cat = 3;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10004:
                    {
                        context.ImbMenu_Cat = 4;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10005:
                    {
                        context.ImbMenu_Cat = 5;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10006:
                    {
                        context.ImbMenu_Cat = 6;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10007:
                    {
                        context.ImbMenu_Cat = 7;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10008:
                    {
                        context.ImbMenu_Cat = 8;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10009:
                    {
                        context.ImbMenu_Cat = 9;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10010:
                    {
                        context.ImbMenu_Cat = 10;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10011:
                    {
                        context.ImbMenu_Cat = 11;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10012:
                    {
                        context.ImbMenu_Cat = 12;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10013:
                    {
                        context.ImbMenu_Cat = 13;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10014:
                    {
                        context.ImbMenu_Cat = 14;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                case 10015:
                    {
                        context.ImbMenu_Cat = 15;

                        from.SendGump(new ImbuingGumpB(from, context.LastImbued));
                        break;
                    }
                default:  // = Proceed to Attribute Intensity Menu [ImbuingC.cs]
                    {
                        int buttonNum = info.ButtonID - 10100;
                        context.Imbue_Mod = buttonNum;

                        if (Imbuing.OnBeforeImbue(from, context.LastImbued, buttonNum, -1))
                        {
                            from.SendGump(new ImbuingGumpC(from, context.LastImbued, buttonNum, -1));
                        }
                        
                        break;
                    }
            }
        }
    }
}