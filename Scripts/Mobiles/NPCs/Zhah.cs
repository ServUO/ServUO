using Server.Engines.Quests;
using Server.Items;
using System;

namespace Server.Mobiles
{
    public class QueenZhah : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(JourneyToTheAthenaeumIsleQuest) };

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
            SetWearable(new LeatherTalons(), 0x4F2, 1); // Bright Blue
            SetWearable(new GargishLeatherChest(), 0x4F2, 1); // Bright Blue
            SetWearable(new GargishLeatherLegs(), 0x4F2, 1); // Bright Blue
            SetWearable(new GargishClothWingArmor(), 0x4F2, 1); // Bright Blue
            SetWearable(new GargishLeatherArms(), 0x4F2, 1); // Bright Blue
			SetWearable(new GargishLeatherKilt(), 0x4F2, 1); // Bright Blue

			SetWearable(new SerpentStoneStaff(), dropChance: 1);
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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}