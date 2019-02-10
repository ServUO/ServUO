using Server;
using System;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Jasper : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof(PaladinsOfTrinsic) }; } }

        public static Jasper TramInstance { get; set; }
        public static Jasper FelInstance { get; set; }

        public static void Initialize()
        {
            if (Core.TOL)
            {
                if (TramInstance == null)
                {
                    TramInstance = new Jasper();
                    TramInstance.MoveToWorld(new Point3D(2018, 2745, 30), Map.Trammel);
                    TramInstance.Direction = Direction.South;
                }

                if (FelInstance == null)
                {
                    FelInstance = new Jasper();
                    FelInstance.MoveToWorld(new Point3D(2018, 2745, 30), Map.Felucca);
                    FelInstance.Direction = Direction.South;
                }
            }
        }

        public Jasper()
            : base("Jasper", "the Inspector")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Body = 0x190;
            Hue = Race.RandomSkinHue();
            HairItemID = 0;
            FacialHairItemID = 0x204D;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new PlateChest(), 0x8A5);
            SetWearable(new PlateLegs(), 0x8A5);
            SetWearable(new PlateArms(), 0x8A5);
            SetWearable(new PlateGloves(), 0x8A5);
            SetWearable(new BodySash(), 1158);
            SetWearable(new Cloak(), 1158);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(Location, 5))
            {
                PaladinsOfTrinsic quest = QuestHelper.GetQuest((PlayerMobile)m, typeof(PaladinsOfTrinsic)) as PaladinsOfTrinsic;

                if (quest != null)
                {
                    quest.GiveRewards();
                }
                else
                {
                    PaladinsOfTrinsic2 quest2 = QuestHelper.GetQuest((PlayerMobile)m, typeof(PaladinsOfTrinsic2)) as PaladinsOfTrinsic2;

                    if (quest2 != null)
                    {
                        if (quest2.Completed)
                        {
                            quest2.CompleteQuest();
                        }
                        else
                        {
                            m.SendGump(new MondainQuestGump(quest2, MondainQuestGump.Section.InProgress, false));
                            quest2.InProgress();
                        }
                    }
                }
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m.Location, 5) && !InRange(oldLocation, 5))
            {
                PaladinsOfTrinsic2 quest = QuestHelper.GetQuest<PaladinsOfTrinsic2>((PlayerMobile)m);

                if (quest != null && !quest.SentMessage && quest.Completed)
                {
                    m.SendLocalizedMessage(1158111); // You have proven yourself Honorable, the Lord Commander looks overjoyed as you approach him triumphantly! Speak to him to claim your reward!
                    quest.SentMessage = true;
                }
            }
        }

        public Jasper(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
            else if (Map == Map.Felucca)
            {
                FelInstance = this;
            }

            if (!Core.TOL)
                Delete();
        }
    }
}
