using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
    public class SpecialScrollBookGump : BaseGump
    {
        public BaseSpecialScrollBook Book { get; private set; }

        public SkillCat Category { get; set; }
        public SkillName Skill { get; set; }

        public SpecialScrollBookGump(PlayerMobile pm, BaseSpecialScrollBook book)
            : base(pm, 150, 200)
        {
            Book = book;

            pm.CloseGump(typeof(SpecialScrollBookGump));
        }

        public override void AddGumpLayout()
        {
            AddImage(0, 0, 2200);

            for (int i = 0; i < 2; ++i)
            {
                int xOffset = 25 + (i * 165);

                AddImage(xOffset, 45, 57);
                xOffset += 20;

                for (int j = 0; j < 6; ++j, xOffset += 15)
                    AddImage(xOffset, 45, 58);

                AddImage(xOffset - 5, 45, 59);
            }

            if (Category != SkillCat.None)
            {
                if (Skill != SkillName.Alchemy)
                {
                    BuildSkillPage();

                    AddButton(20, 5, 2205, 2205, 1, GumpButtonType.Reply, 0);
                }
                else
                {
                    BuildSkillsPage();

                    AddButton(20, 5, 2205, 2205, 2, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                BuildCategoriesPage();
            }
        }

        public virtual void BuildCategoriesPage()
        {
            AddHtmlLocalized(0, 15, 175, 20, CenterLoc, String.Format("#{0}", Book.GumpTitle), 0, false, false);

            if (Book == null || Book.Deleted || Book.SkillInfo == null)
                return;

            int index = 0;
            foreach (var kvp in Book.SkillInfo)
            {
                AddHtmlLocalized(45, 55 + (index * 15), 100, 20, BaseSpecialScrollBook.GetCategoryLocalization(kvp.Key), false, false);

                if (HasScroll(kvp.Value))
                {
                    AddButton(30, 59 + (index * 15), 2103, 2104, 10 + (int)kvp.Key, GumpButtonType.Reply, 0);
                }

                index++;
            }
        }

        public virtual void BuildSkillsPage()
        {
            AddHtmlLocalized(0, 15, 175, 20, CenterLoc, String.Format("#{0}", BaseSpecialScrollBook.GetCategoryLocalization(Category)), 0, false, false); // Power Scrolls

            if (Category == SkillCat.None || Book == null || Book.Deleted || Book.SkillInfo == null)
                return;

            List<SkillName> list = Book.SkillInfo[Category];

            int x = 45;
            int y = 55;
            int buttonX = 30;
            int split = list.Count >= 9 ? list.Count / 2 : -1;

            for (int i = 0; i < list.Count; i++)
            {
                SkillName skill = list[i];

                if (split > -1 && i == split)
                {
                    x = 205;
                    y = 55;
                    buttonX = 190;
                }

                AddHtmlLocalized(x, y, 110, 20, SkillInfo.Table[(int)skill].Localization, false, false);

                if (HasScroll(skill))
                {
                    AddButton(buttonX, y + 4, 2103, 2104, 100 + i, GumpButtonType.Reply, 0);
                }

                y += 15;
            }
        }

        public virtual void BuildSkillPage()
        {
            AddHtmlLocalized(0, 15, 175, 20, CenterLoc, String.Format("#{0}", SkillInfo.Table[(int)Skill].Localization), 0, false, false);

            if (Skill == SkillName.Alchemy || Book == null || Book.Deleted || Book.ValueInfo == null)
                return;

            int x = 40;
            int buttonX = 30;
            int y = 55;
            int index = 0;
            int split = Book.ValueInfo.Count >= 9 ? Book.ValueInfo.Count / 2 : -1;

            foreach(var kvp in Book.ValueInfo)
            {
                if (split > -1 && index == split)
                {
                    x = 205;
                    buttonX = 195;
                    y = 55;
                }

                int total = GetTotalScrolls(Skill, kvp.Value);

                AddHtmlLocalized(x, y, 100, 20, kvp.Key, false, false);
                AddLabel(x + 100, y, 0, total.ToString());

                if (total > 0)
                {
                    AddButton(buttonX, y + 4, 2437, 2438, 1000 + (int)(kvp.Value * 10), GumpButtonType.Reply, 0);
                }

                y += 15;
                index++;
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                Skill = SkillName.Alchemy;

                Refresh();
            }
            else if (info.ButtonID == 2)
            {
                Category = SkillCat.None;
                Skill = SkillName.Alchemy;

                Refresh();
            }
            else
            {
                int id = info.ButtonID;

                if (id < 100)
                {
                    id -= 10;

                    if (id >= 0 && id <= 7)
                    {
                        Category = (SkillCat)id;
                        Refresh();
                    }
                }
                else if (id < 1000)
                {
                    id -= 100;

                    if (Category > SkillCat.None)
                    {
                        List<SkillName> list = Book.SkillInfo[Category];

                        if (id >= 0 && id < list.Count)
                        {
                            Skill = list[id];
                            Refresh();
                        }
                    }
                }
                else
                {
                    double value = id - 1000;
                    value /= 10;

                    Book.Construct(User, Skill, value);
                }
            }
        }

        public bool HasScroll(List<SkillName> skills)
        {
            return Book.Items.FirstOrDefault(i => i is SpecialScroll && skills.Contains(((SpecialScroll)i).Skill)) != null;
        }

        public bool HasScroll(SkillName skill)
        {
            return Book.Items.FirstOrDefault(i => i is SpecialScroll && skill == ((SpecialScroll)i).Skill) != null;
        }

        public int GetTotalScrolls(SkillName skill, double value)
        {
            int count = 0;

            foreach (var scroll in Book.Items.OfType<SpecialScroll>().Where(s => s.Skill == skill && value == s.Value))
                count++;

            return count;
        }
    }
}