using Server.Items;

namespace Server.Mobiles
{
    public class GargishRefugee : BaseCreature
    {
        [Constructable]
        public GargishRefugee()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Refugee";
            if (Female = Utility.RandomBool())
            {
                Body = 667;
                HairItemID = 17067;
                HairHue = 1762;
                SetWearable(new GargishClothChest(), dropChance: 1);
                SetWearable(new GargishClothKilt(), Utility.RandomNeutralHue(), 1);
            }
            else
            {
                Body = 666;
                HairItemID = 16987;
                HairHue = 1801;
                SetWearable(new GargishClothChest(), dropChance: 1);
                SetWearable(new GargishClothKilt(), dropChance: 1);
				SetWearable(new GargishClothLegs(), Utility.RandomNeutralHue(), 1);
            }
        }

        public GargishRefugee(Serial serial)
            : base(serial)
        {
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
