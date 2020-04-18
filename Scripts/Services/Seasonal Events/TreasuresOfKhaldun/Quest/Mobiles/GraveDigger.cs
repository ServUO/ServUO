using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Khaldun
{
    public class GraveDigger : BaseVendor
    {
        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override bool IsActiveVendor => false;

        public override void InitSBInfo()
        {
        }

        public static GraveDigger TramInstance { get; set; }
        //public static GraveDigger FelInstance { get; set; }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new GraveDigger();
                TramInstance.MoveToWorld(new Point3D(1382, 1447, 10), Map.Trammel);
                TramInstance.Direction = Direction.South;
            }
        }

        public GraveDigger()
            : base("the Grave Digger")
        {
        }

        public override void InitBody()
        {
            Name = NameList.RandomName("male");
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Race = Race.Human;
            Hue = Race.RandomSkinHue();
            HairHue = Race.RandomHairHue();
            FacialHairHue = Race.RandomHairHue();
            HairItemID = 0x203C;
            FacialHairItemID = 0x203E;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new Surcoat(), 1634);
            SetWearable(new Kilt(), 946);
            SetWearable(new FancyShirt(), 1411);
            SetWearable(new ThighBoots(), 2013);
            SetWearable(new GoldBracelet());
            SetWearable(new GoldRing());
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && InRange(m.Location, 5))
            {
                GoingGumshoeQuest2 quest = QuestHelper.GetQuest<GoingGumshoeQuest2>((PlayerMobile)m);

                if (quest != null)
                {
                    m.SendLocalizedMessage(1158606, null, 0x23); /* You've spoken to the Grave Digger and have paid your respects to those who 
                                                                    perished in the fight against the titans. How someone could defile a grave 
                                                                    stone, you have no idea. You decide to take a closer look... */

                    m.PlaySound(quest.UpdateSound);

                    m.SendGump(new InternalGump());
                }
            }
        }

        private class InternalGump : Gump
        {
            public InternalGump()
                : base(50, 50)
            {
                AddBackground(0, 0, 720, 285, 9300);
                AddImage(0, 0, 1743);
                AddHtmlLocalized(300, 25, 408, 250, 1158570, false, false);

                /*Solemn job you know, but someone has to do it. Been pretty busy since the invasions, hardly a day goes by we don't
                 * have the kin of someone who embraced Sacrifice and gave themselves in defense of the realm against the Titans.
                 * Ah well, makes me feel good knowing me shovel gives em a good final resting place. Worst part ya know though -
                 * plenty of people been in and out of the cemetery for sure, but I don't know why they insist on messing up the 
                 * headstones!? To think, some people have no Shame!*/
            }
        }

        public GraveDigger(Serial serial)
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
