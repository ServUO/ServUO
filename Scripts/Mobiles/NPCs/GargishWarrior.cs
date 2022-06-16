using Server.Items;

namespace Server.Mobiles
{
    public class GargishWarrior : BaseCreature
    {
        [Constructable]
        public GargishWarrior()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Warrior";
            if (Female = Utility.RandomBool())
            {
                Body = 667;
                HairItemID = 17067;
                HairHue = 1762;
				SetWearable(new FemaleGargishPlateChest(), dropChance: 1);
                SetWearable(new FemaleGargishPlateKilt(), dropChance: 1);
                SetWearable(new FemaleGargishPlateLegs(), dropChance: 1);
                SetWearable(new FemaleGargishPlateArms(), dropChance: 1);
                SetWearable(new PlateTalons(), dropChance: 1);

                SetWearable(new GlassSword(), dropChance: 1);
            }
            else
            {
                Body = 666;
                HairItemID = 16987;
                HairHue = 1801;
				SetWearable(new GargishPlateChest(), dropChance: 1);
                SetWearable(new GargishPlateKilt(), dropChance: 1);
                SetWearable(new GargishPlateLegs(), dropChance: 1);
                SetWearable(new GargishPlateArms(), dropChance: 1);
                SetWearable(new PlateTalons(), dropChance: 1);

                SetWearable(new GlassSword(), dropChance: 1);
            }
        }

        public GargishWarrior(Serial serial)
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
