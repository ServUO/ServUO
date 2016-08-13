using System;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Walton : HumilityQuestMobile
    {
        public override int Greeting { get { return 1075739; } }

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
            this.InitStats(100, 100, 25);

            this.Female = false;
            this.Race = Race.Human;
            this.Body = 0x190;

            this.Hue = Race.RandomSkinHue();
            this.HairItemID = Race.RandomHair(false);
            this.HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new FancyShirt());
            this.AddItem(new Doublet(1109));
            this.AddItem(new LongPants(Utility.RandomBlueHue()));
            this.AddItem(new Boots());
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