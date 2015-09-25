using Server;
using System;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class QueenZhah : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof(JourneyToTheAthenaeumIsleQuest) }; } }

        [Constructable]
        public QueenZhah() : base("Zhah", "the Gargoyle Queen")
        {
        }

        public override void InitBody()
        {
            Female = true;
            Race = Race.Gargoyle;
            Body = 667;

            InitStats(100, 100, 25);
            SpeechHue = Utility.RandomDyedHue();
            Hue = Race.RandomSkinHue();

            HairItemID = 0x42AB; // Get tiare looking kind
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            ColorItem(new LeatherTalons()); // Bright Blue
            ColorItem(new GargishLeatherChest()); // Bright Blue
            ColorItem(new GargishLeatherLegs()); // Bright Blue
            ColorItem(new GargishClothWingArmor()); // Bright Blue
            ColorItem(new GargishLeatherArms()); // Bright Blue
            ColorItem(new GargishLeatherKilt()); // Bright Blue

            AddItem(new SerpentStoneStaff());
        }

        private void ColorItem(Item item)
        {
            item.Hue = 0x4F2;
            AddItem(item);
        }

        public override void Advertise()
        {
            Say(1150932);
        }

        public QueenZhah(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}