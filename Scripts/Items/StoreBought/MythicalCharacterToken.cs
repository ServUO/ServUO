using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class MythicCharacterToken : Item, IPromotionalToken
    {
        public override int LabelNumber { get { return 1070997; } } // a promotional token
        public TextDefinition ItemName { get { return 1152353; } } // Mythic Character Token

        public Type GumpType { get { return typeof(MythicCharacterToken.InternalGump); } }

        [Constructable]
        public MythicCharacterToken()
            : base(0x2AAA)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && IsChildOf(m.Backpack))
            {
                if (m.Skills.Total > 2000)
                {
                    m.SendLocalizedMessage(1152368); // You cannot use this token on this character because you have over 200 skill points. If you 
                    // don’t have a way to lower your skill point total, you will have to use this Mythic Character
                    // Token on another character.
                }
                else
                {
                    BaseGump.SendGump(new InternalGump((PlayerMobile)m, this));
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, ItemName.ToString()); // Use this to redeem<br>Your ~1_PROMO~
        }

        public MythicCharacterToken(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public class InternalGump : BaseGump
        {
            public MythicCharacterToken Token { get; set; }
            public Skill[] Selected { get; set; }

            public bool Editing { get; set; }

            public int Str { get; set; }
            public int Dex { get; set; }
            public int Int { get; set; }

            public static readonly int Width = 500;
            public static readonly int Height = 510;

            public static int Green { get { return C32216(0x32CD32); } }
            public static int LightGreen { get { return C32216(0x90EE90); } }
            public static int Yellow { get { return C32216(0xFFE4C4); } }
            public static int Beige { get { return C32216(0xF5F5DC); } }
            public static int Gray { get { return C32216(0x696969); } }
            public static int White { get { return 0x7FFF; } }

            public bool HasAllFive
            {
                get
                {
                    if (Selected == null)
                        return false;

                    foreach (var sk in Selected)
                    {
                        if (sk == null)
                            return false;
                    }

                    return true;
                }
            }

            public InternalGump(PlayerMobile pm, MythicCharacterToken token)
                : base(pm, 100, 100)
            {
                Token = token;
                Selected = new Skill[5];
            }

            public override void AddGumpLayout()
            {
                AddPage(0);
                AddBackground(0, 0, Width, Height, 9200);

                AddImageTiled(10, 10, 480, 25, 2624);
                AddAlphaRegion(10, 10, 480, 25);

                AddImageTiled(10, 45, 480, 425, 2624);
                AddAlphaRegion(10, 45, 480, 425);

                AddImageTiled(10, 480, 480, 22, 2624);
                AddAlphaRegion(10, 480, 480, 22);

                AddHtmlLocalized(0, 12, Width, 20, 1152352, White, false, false); // <center>Mythic Character Skill Selection</center>

                AddHtmlLocalized(0, 45, Width / 3, 20, 1152354, Yellow, false, false); // <CENTER>Set Attributes</CENTER>
                AddHtmlLocalized(0, 65, Width / 3, 20, 1152355, User.StatCap.ToString(), Beige, false, false); // <CENTER>Total Must Equal ~1_VAL~

                AddBackground(11, 85, 80, 20, 3000);
                AddBackground(11, 106, 80, 20, 3000);
                AddBackground(11, 127, 80, 20, 3000);

                AddTextEntry(13, 85, 75, 20, 0, 1, Str > 0 ? Str.ToString() : "");
                AddTextEntry(13, 106, 75, 20, 0, 2, Dex > 0 ? Dex.ToString() : "");
                AddTextEntry(13, 127, 75, 20, 0, 3, Int > 0 ? Int.ToString() : "");

                AddHtmlLocalized(98, 85, 100, 20, 3000111, White, false, false); // Strength
                AddHtmlLocalized(98, 106, 100, 20, 3000113, White, false, false); // Dexterity
                AddHtmlLocalized(98, 127, 100, 20, 3000112, White, false, false); // Intelligence

                AddHtmlLocalized(0, 170, Width / 3, 20, 1152356, Yellow, false, false); // <CENTER>Selected Skills</CENTER>

                AddButton(10, Height - 30, 4017, 4018, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(42, Height - 30, 100, 20, 1153439, White, false, false); // CANCEL

                for (int i = 0; i < Selected.Length; i++)
                {
                    Skill sk = Selected[i];

                    if (sk == null)
                        continue;

                    AddButton(12, 190 + (i * 20), 4017, 4018, 5000 + i, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(45, 190 + (i * 20), 150, 20, sk.Info.Localization, Green, false, false);
                }

                if (HasAllFive)
                {
                    AddHtmlLocalized(Width / 3, 65, ((Width / 3) * 2) - 15, Height - 100, 1152358, LightGreen, false, false);
                    /*Please confirm that you wish to set your attributes as indicated in the upper left area of this window. 
                    If you wish to change these values, edit them and click the EDIT button below.<br><br>Please confirm that 
                    you wish to set the five skills selected on the left to 90.0 skill. If you wish to make changes, click the 
                    [X] button next to a skill name to remove it from the list.<br><br>If are sure you wish to apply the selected
                    skills and attributes, click the CONTINUE button below.<br><br>If you wish to abort the application of the 
                    Mythic Character Token, click the CANCEL button below.*/

                    AddButton(Width / 3, Height - 100, 4005, 4007, 2500, GumpButtonType.Reply, 0);
                    AddHtmlLocalized((Width / 3) + 32, Height - 100, 100, 20, 1150647, White, false, false); // EDIT

                    AddButton(Width / 3, Height - 120, 4005, 4007, 2501, GumpButtonType.Reply, 0);
                    AddHtmlLocalized((Width / 3) + 32, Height - 120, 100, 20, 1011011, White, false, false); // CONTINUE
                }
                else
                {
                    AddHtmlLocalized(Width / 3, 45, (Width / 3) * 2, 20, 1152357, White, false, false); // <CENTER>Select Five Skills to Advance</CENTER>

                    AddPage(1);

                    BuildSkillCategory(BaseSpecialScrollBook.GetCategoryLocalization(SkillCat.Magic), Width / 3, 65, SkillCat.Magic, ScrollOfAlacrityBook._SkillInfo[SkillCat.Magic]);
                    BuildSkillCategory(BaseSpecialScrollBook.GetCategoryLocalization(SkillCat.Bard), Width / 3, 345, SkillCat.Bard, ScrollOfAlacrityBook._SkillInfo[SkillCat.Bard]);
                    BuildSkillCategory(BaseSpecialScrollBook.GetCategoryLocalization(SkillCat.Combat), (Width / 3) * 2, 65, SkillCat.Combat, ScrollOfAlacrityBook._SkillInfo[SkillCat.Combat]);
                    BuildSkillCategory(BaseSpecialScrollBook.GetCategoryLocalization(SkillCat.Wilderness), (Width / 3) * 2, 305, SkillCat.Wilderness, ScrollOfAlacrityBook._SkillInfo[SkillCat.Wilderness]);

                    AddButton(Width - 120, Height - 30, 4005, 4007, 0, GumpButtonType.Page, 2);
                    AddHtmlLocalized(Width - 85, Height - 30, 75, 20, 3005109, White, false, false); // Next
                    AddPage(2);
                    AddButton(Width - 160, Height - 30, 4014, 4015, 0, GumpButtonType.Page, 1);
                    AddHtmlLocalized(Width - 128, Height - 30, 75, 20, 3010002, White, false, false); // Back

                    BuildSkillCategory(BaseSpecialScrollBook.GetCategoryLocalization(SkillCat.TradeSkills), Width / 3, 65, SkillCat.TradeSkills, ScrollOfAlacrityBook._SkillInfo[SkillCat.TradeSkills]);
                    BuildSkillCategory(BaseSpecialScrollBook.GetCategoryLocalization(SkillCat.Miscellaneous), Width / 3, 285, SkillCat.Miscellaneous, ScrollOfAlacrityBook._SkillInfo[SkillCat.Miscellaneous]);
                    BuildSkillCategory(BaseSpecialScrollBook.GetCategoryLocalization(SkillCat.Thievery), (Width / 3) * 2, 150, SkillCat.Thievery, ScrollOfAlacrityBook._SkillInfo[SkillCat.Thievery]);
                }
            }

            private void BuildSkillCategory(int titleLoc, int x, int y, SkillCat cat, List<SkillName> skills)
            {
                AddHtmlLocalized(x, y, Width / 3, 20, CenterLoc, "#" + titleLoc, White, false, false);
                y += 20;

                for (int i = 0; i < skills.Count; i++)
                {
                    int hue = Gray;
                    if (CanSelect(skills[i]))
                    {
                        AddButton(x, y + (i * 20), 4005, 4006, (int)skills[i] + 500, GumpButtonType.Reply, 0);
                        hue = Green;
                    }

                    AddHtmlLocalized(x + 34, y + (i * 20), Width / 3, 20, User.Skills[skills[i]].Info.Localization, hue, false, false);
                }
            }

            public override void OnResponse(RelayInfo info)
            {
                if (!Token.IsChildOf(User.Backpack) || !User.Alive || User.Skills.Total > 2000)
                    return;

                int buttonID = info.ButtonID;

                if (buttonID == 0)
                    return;

                switch (buttonID)
                {
                    case 2500: // Edit
                        SetStats(info);
                        break;
                    case 2501: // Continue
                        SetStats(info);
                        if ((Str + Dex + Int) != User.StatCap)
                        {
                            User.SendLocalizedMessage(1152359); // Your Strength, Dexterity, and Intelligence values do not add up to the total indicated in 
                            // the upper left area of this window. Before continuing, you must adjust these values so 
                            // their total adds up to exactly the displayed value. Please edit your desired attribute 
                            // values and click the EDIT button below to continue.
                        }
                        else
                        {
                            Effects.SendLocationParticles(EffectItem.Create(User.Location, User.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                            Effects.PlaySound(User.Location, User.Map, 0x243);

                            Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(User.X - 6, User.Y - 6, User.Z + 15), User.Map), User, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                            Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(User.X - 4, User.Y - 6, User.Z + 15), User.Map), User, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                            Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(User.X - 6, User.Y - 4, User.Z + 15), User.Map), User, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                            Effects.SendTargetParticles(User, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                            foreach (var sk in Selected)
                            {
                                sk.Base = 90;
                            }

                            User.RawStr = Str;
                            User.RawDex = Dex;
                            User.RawInt = Int;

                            Token.Delete();
                            return;
                        }
                        break;
                    default:
                        {
                            if (buttonID >= 5000)
                            {
                                Selected[buttonID - 5000] = null;
                            }
                            else if (!HasAllFive)
                            {
                                SkillName sk = (SkillName)buttonID - 500;

                                for (int i = 0; i < Selected.Length; i++)
                                {
                                    if (Selected[i] == null)
                                    {
                                        Selected[i] = User.Skills[sk];
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                }

                Refresh();
            }

            private void SetStats(RelayInfo info)
            {
                var entry1 = info.GetTextEntry(1);
                var entry2 = info.GetTextEntry(2);
                var entry3 = info.GetTextEntry(3);

                if (entry1 != null)
                    Str = Math.Min(125, Math.Max(10, Utility.ToInt32(entry1.Text)));

                if (entry2 != null)
                    Dex = Math.Min(125, Math.Max(10, Utility.ToInt32(entry2.Text)));

                if (entry3 != null)
                    Int = Math.Min(125, Math.Max(10, Utility.ToInt32(entry3.Text)));
            }

            private bool CanSelect(SkillName skill)
            {
                foreach (var sk in Selected)
                {
                    if (User.Skills[skill] == sk)
                        return false;
                }

                if (skill == SkillName.Spellweaving && !User.Spellweaving)
                    return false;

                if (skill == SkillName.Throwing && User.Race != Race.Gargoyle)
                    return false;

                if (skill == SkillName.Archery && User.Race == Race.Gargoyle)
                    return false;

                return true;
            }
        }
    }
}
