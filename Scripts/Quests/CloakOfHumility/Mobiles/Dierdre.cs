using Server.Engines.Quests;
using Server.Items;

namespace Server.Mobiles
{
    public class Dierdre : HumilityQuestMobile
    {
        public override int Greeting => 1075744;

        [Constructable]
        public Dierdre()
            : base("Dierdre", "the Beggar")
        {
        }

        public Dierdre(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;
            Body = 0x191;

            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(true);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new Sandals(), dropChance: 1);
            SetWearable(new FancyShirt(), dropChance: 1);
			SetWearable(new PlainDress(), dropChance: 1);
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