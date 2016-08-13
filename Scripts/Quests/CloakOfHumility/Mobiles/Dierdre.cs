using System;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Dierdre : HumilityQuestMobile
    {
        public override int Greeting { get { return 1075744; } }

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
            this.InitStats(100, 100, 25);

            this.Female = true;
            this.Race = Race.Human;
            this.Body = 0x191;

            this.Hue = Race.RandomSkinHue();
            this.HairItemID = Race.RandomHair(true);
            this.HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Sandals());
            this.AddItem(new FancyShirt());
            this.AddItem(new PlainDress());
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
        }
    }
}