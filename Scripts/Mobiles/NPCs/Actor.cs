using Server.Items;

namespace Server.Mobiles
{
    public class Actor : BaseCreature
    {
        [Constructable]
        public Actor()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            InitStats(31, 41, 51);

            SpeechHue = Utility.RandomDyedHue();

            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
				SetWearable(new FancyDress(), Utility.RandomDyedHue(), 1);
                Title = "the actress";
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
				SetWearable(new LongPants(), Utility.RandomNeutralHue(), 1);
				SetWearable(new FancyShirt(), Utility.RandomDyedHue(), 1);
                Title = "the actor";
            }

            SetWearable(new Boots(), Utility.RandomNeutralHue(), 1);

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

            SetWearable(pack);
        }

        public Actor(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle => false;
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
