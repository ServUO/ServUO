using Server.Items;
using Server.Mobiles;
using Server.Spells.SkillMasteries;

namespace Server.Gumps
{
    public class MasterySelectionGump : BaseGump
    {
        public const int Red = 0x8e2525;
        public const int Blue = 0x000066;

        public BookOfMasteries Book { get; private set; }

        public MasterySelectionGump(PlayerMobile user, BookOfMasteries book)
            : base(user, 75, 25)
        {
            Book = book;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 404, 550, 9380);

            AddHtmlLocalized(0, 40, 404, 16, CenterLoc, "#1151948", 0, false, false); // Switch Mastery

            int y = 58;
            SkillName current = User.Skills.CurrentMastery;

            foreach (SkillName skName in MasteryInfo.Skills)
            {
                Skill sk = User.Skills[skName];

                if (sk != null && sk.IsMastery && sk.VolumeLearned > 0)
                {
                    AddButton(30, y, 4005, 4007, (int)skName + 1, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(72, y, 200, 16, MasteryInfo.GetLocalization(skName), skName == current ? C32216(Red) : C32216(Blue), false, false);
                    AddHtmlLocalized(265, y, 100, 16, 1156052, MasteryInfo.GetMasteryLevel(User, skName).ToString(), 0, false, false);

                    y += 24;
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            SkillName n = (SkillName)info.ButtonID - 1;
            SkillName current = User.Skills.CurrentMastery;

            if (n == current)
            {
                User.Skills.CurrentMastery = SkillName.Alchemy;
                MasteryInfo.OnMasteryChanged(User, current);
            }
            else if (User.Skills[n].Base >= MasteryInfo.MinSkillRequirement)
            {
                User.SendLocalizedMessage(1155886, User.Skills[n].Info.Name); // Your active skill mastery is now set to ~1_MasterySkill~!
                User.Skills.CurrentMastery = n;

                MasteryInfo.OnMasteryChanged(User, current);

                BookOfMasteries.AddToCooldown(User);
            }
            else
                User.SendLocalizedMessage(1156236, string.Format("{0}\t{1}", MasteryInfo.MinSkillRequirement.ToString(), User.Skills[n].Info.Name)); // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that mastery.
        }
    }
}
