using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Nelson : HumilityQuestMobile
    {
        public override int Greeting => 1075749;

        [Constructable]
        public Nelson()
            : base("Nelson", "the Shepherd")
        {
        }

        public Nelson(Serial serial)
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
            base.InitOutfit();

            AddItem(new Items.Robe(Utility.RandomGreenHue()));
            AddItem(new Items.ShepherdsCrook());
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