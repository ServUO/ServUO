using Server.Items;

namespace Server.Mobiles
{
    public class Sculptor : BaseCreature
    {
        [Constructable]
        public Sculptor()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            InitStats(31, 41, 51);

            SpeechHue = Utility.RandomDyedHue();
            Title = "the sculptor";
            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
				SetWearable(new Kilt(), Utility.RandomNeutralHue(), 1);
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
				SetWearable(new LongPants(), Utility.RandomNeutralHue(), 1);
            }

			SetWearable(new Doublet(), Utility.RandomNeutralHue(), 1);
			SetWearable(new HalfApron(), dropChance: 1);

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

			SetWearable(pack);
        }

        public Sculptor(Serial serial)
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
