using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells.SkillMasteries;
using System.Linq;
using Server.Network;

namespace Server.Gumps
{
    public class MasterySelectionGump : Gump
    {
        public const int Red = 0x4800;
        public const int Blue = 0x000F;

        public PlayerMobile User { get; set; }
        public BookOfMasteries Book { get; private set; }

        public MasterySelectionGump(PlayerMobile user, BookOfMasteries book) : base(75, 25)
        {
            Book = book;
            User = user;

            AddGumpLayout();
        }

        public void AddGumpLayout()
        {
            AddImage(0, 0, 8000);
            AddImage(20, 37, 8001);
            AddImage(20, 107, 8002);
            AddImage(20, 177, 8001);
            AddImage(20, 247, 8002);
            AddImage(20, 317, 8001);
            AddImage(20, 387, 8002);
            AddImage(20, 457, 8003);

            AddHtmlLocalized(125, 40, 345, 16, 1151948, false, false); // Switch Mastery

            int y = 60;
            SkillName current = User.Skills.CurrentMastery;

            foreach (SkillName skName in MasteryInfo.Skills)
            {
                Skill sk = User.Skills[skName];

                if (sk != null && sk.IsMastery && sk.KnownMasteries != 0)
                {
                    if (skName != current)
                        AddButton(30, y, 4005, 4007, (int)skName + 1, GumpButtonType.Reply, 0);

                    AddHtmlLocalized(75, y, 200, 16, MasteryInfo.GetLocalization(skName), skName == current ? Red : Blue, false, false);
                    AddHtmlLocalized(250, y, 100, 16, 1156052, MasteryInfo.GetMasteryLevel(User, skName).ToString(), 0, false, false);

                    y += 20;
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            SkillName n = (SkillName)info.ButtonID - 1;
            SkillName current = User.Skills.CurrentMastery;

            if (User.Skills[n].Value >= MasteryInfo.MinSkillRequirement)
            {
                User.SendLocalizedMessage(1155886, User.Skills[n].Info.Name); // Your active skill mastery is now set to ~1_MasterySkill~!
                User.Skills.CurrentMastery = n;

                MasteryInfo.OnMasteryChanged(User, current);

                BookOfMasteries.AddToCooldown(User);
            }
            else
                User.SendLocalizedMessage(1156236, String.Format("{0}\t{1}", MasteryInfo.MinSkillRequirement.ToString(), User.Skills[n].Info.Name)); // You need at least ~1_SKILL_REQUIREMENT~ ~2_SKILL_NAME~ skill to use that mastery.
        }
    }
}