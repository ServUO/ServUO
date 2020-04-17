using Server.Engines.Quests;
using Server.Items;

namespace Server.Mobiles
{
    public class Walton : HumilityQuestMobile
    {
        public override int Greeting => 1075739;

        [Constructable]
        public Walton()
            : base("Walton", "the Horse Trainer")
        {
        }

        public Walton(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;
            Body = 0x190;

            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new FancyShirt());
            AddItem(new Doublet(1109));
            AddItem(new LongPants(Utility.RandomBlueHue()));
            AddItem(new Boots());
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