using Server;
using System;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class GraveDigger : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof(PaladinsOfTrinsic) }; } }

        public static GraveDigger TramInstance { get; set; }
        public static GraveDigger FelInstance { get; set; }

        public static void Initialize()
        {
            if (Core.TOL)
            {
                if (TramInstance == null)
                {
                    TramInstance = new GraveDigger();
                    TramInstance.MoveToWorld(new Point3D(2018, 2745, 30), Map.Trammel);
                    TramInstance.Direction = Direction.South;
                }

                if (FelInstance == null)
                {
                    FelInstance = new GraveDigger();
                    FelInstance.MoveToWorld(new Point3D(2018, 2745, 30), Map.Felucca);
                    FelInstance.Direction = Direction.South;
                }
            }
        }

        public GraveDigger()
            : base("Oscar", "the Grave Digger")
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
            
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m.Location, 5) && !InRange(oldLocation, 5))
            {
                GoingGumpshoeQuest2 quest = QuestHelper.GetQuest<GoingGumpshoeQuest2>((PlayerMobile)m);

                if (quest != null && !quest.SentMessage && quest.Completed)
                {
                    m.SendLocalizedMessage(1158606); // You've spoken to the Grave Digger and have paid your respects to those who perished in the fight against the titans. How someone could defile a grave stone, you have no idea. You decide to take a closer look...
                    quest.SentMessage = true;
                }
            }
        }

        public GraveDigger(Serial serial)
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
