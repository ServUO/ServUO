using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Maribel : HumilityQuestMobile
    {
        public override int Greeting => 1075754;

        public override bool IsActiveVendor => true;

        [Constructable]
        public Maribel()
            : base("Maribel", "the Waitress")
        {
        }

        public override void InitSBInfo()
        {
            SBInfos.Add(new SBWaiter());
        }

        public Maribel(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;
            Body = 0x191;

            Hue = 0x83EA;
            HairItemID = 0x2049;
        }

        public override void InitOutfit()
        {
            AddItem(new Items.Backpack());
            AddItem(new Items.Sandals());
            AddItem(new Items.FancyDress(2205));
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