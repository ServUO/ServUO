using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Khaldun
{
    public class SageHumbolt : BaseVendor
    {
        public static SageHumbolt TramInstance { get; set; }

        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override bool IsActiveVendor => false;

        public override void InitSBInfo()
        {
        }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new SageHumbolt();
                TramInstance.MoveToWorld(new Point3D(5808, 3270, -15), Map.Trammel);
                TramInstance.Direction = Direction.North;
            }
        }

        public SageHumbolt()
            : base("the Ghost")
        {
            IsDeadPet = true;
        }

        public override void InitBody()
        {
            Name = "Sage Humbolt";
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Race = Race.Human;
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            DeathRobe robe = new DeathRobe
            {
                ItemID = 9863
            };
            SetWearable(robe);
        }

        public bool OnSpiritSpeak(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm != null)
            {
                GoingGumshoeQuest4 quest = QuestHelper.GetQuest<GoingGumshoeQuest4>(pm);

                if (quest != null && !quest.IsComplete)
                {
                    /*You have successfully found Sage Humbolt who has opened you eyes to the entire conspiracy, and the danger that looms ahead 
                     * if no steps are taken to alter the current course of events. Return to Inspector Jasper to report your findings.*/
                    m.SendLocalizedMessage(1158636, null, 0x23);
                    m.SendGump(new InternalGump());

                    m.PlaySound(quest.UpdateSound);
                    quest.IsComplete = true;

                    return true;
                }
            }

            return false;
        }

        private class InternalGump : Gump
        {
            public InternalGump()
                : base(50, 50)
            {
                AddBackground(0, 0, 400, 600, 9300);
                AddImage(58, 30, 1746);

                AddHtmlLocalized(0, 340, 400, 20, 1154645, "#1158623", 0x0, false, false); // The Prophecy
                AddHtmlLocalized(5, 365, 390, 200, 1158622, BaseGump.C32216(0x0D0D0D), false, true);

                /**The ghostly figure looks at you with disappointment* You've brought the cheese haven't you? Gah, 
                 * I can never find it! *the ghost goes back to waving its hands through the barrels* You explain
                 * who you are, and the circumstances that have lead you to this moment. With each word the ghost 
                 * becomes increasingly alarmed, yet an expression of expectation and satisfaction is apparent from
                 * their reaction. The ghost nods and begins to speak in a tongue you can understand,<br><br> "That's
                 * right. I am Sage Humbolt, or I was. What you speak of is especially concerning. The events you
                 * describe - the invasion by otherworldly cultist, the titans, all of it - it was something foretold
                 * long ago. But these most recent revelations, I was hopeful that, like most prophecies, this was a
                 * bit of embellishment by sages through the millenia. Alas, it seems this prophecy has come full 
                 * circle.<br><br>Long ago, a great warrior named Khal Ankur lead a cult devoted to death and sacrifice
                 * . Like most zealots of such a twisted dogma, Khal Ankur met his end and was sealed inside a tomb deep 
                 * in the Lost Lands. Not until four explorers uncovered the tomb did we even know for sure it existed. 
                 * The prophecy tells of a fallen star that would allow Khal Ankur to rise again and lead an army of 
                 * zealots against those who imprisoned him. With the strength of this fallen star Khal Ankur would be
                 * impossible to kill, save for with the very power the fallen star gives to Khal Ankur. You must never
                 * allow Khal Ankur to rise again, lest the cultists may use their power to recall the Titans to this
                 * world! The fallen star is rich with a material called Caddellite, which gives Khal Ankur unmatched
                 * power. I hope you know a good tinker, as Caddellite is a fiercely strong material that is otherwise 
                 * impossible to harvest. With Caddellite infused resources you will be able to supply an army of the
                 * willing with weapons, arcana, and provisions to dispatch this threat once and for all. Go now, there
                 * is little time to lose. Now I've got to get back to my cheese, what a new and exciting place Papua is...  */
            }
        }

        public SageHumbolt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
        }
    }
}
