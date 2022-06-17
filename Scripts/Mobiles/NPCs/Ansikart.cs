using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class Ansikart : MondainQuester
    {
        [Constructable]
        public Ansikart()
            : base("Ansikart", "the Artificer")
        {
            SetSkill(SkillName.ItemID, 60.0, 83.0);
            SetSkill(SkillName.Imbuing, 60.0, 83.0);
        }

        public Ansikart(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(MasteringtheSoulforge),
                    typeof(ALittleSomething)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            CantWalk = true;
            Race = Race.Gargoyle;

            Hue = 0x86DF;
            HairItemID = 0x425D;
            HairHue = 0x321;
        }

        public override void InitOutfit()
        {
            SetWearable(new SerpentStoneStaff(), dropChance: 1);
            SetWearable(new GargishClothChest(), 1428, 1);
            SetWearable(new GargishClothArms(), 1445, 1);
            SetWearable(new GargishClothKilt(), 1443, 1);
        }

        public override void Advertise()
        {
            Say(1112528);  // Master the art of unraveling magic.
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
        }
    }
}