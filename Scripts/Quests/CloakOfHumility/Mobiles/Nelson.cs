using System;
using Server.Items;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Nelson : HumilityQuestMobile
    {
        public override int Greeting { get { return 1075749; } }

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
            base.InitOutfit();

            AddItem(new Server.Items.Robe(Utility.RandomGreenHue()));
            AddItem(new Server.Items.ShepherdsCrook());
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