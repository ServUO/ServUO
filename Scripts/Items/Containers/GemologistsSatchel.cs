using System;

namespace Server.Items
{
    public class GemologistsSatchel : Bag
	{
        public override int LabelNumber { get { return 1113378; } } // Gemologist's Satchel
		
        [Constructable]
		public GemologistsSatchel()
		{
            Hue = 1177;

            DropItem(new Amber(Utility.RandomMinMax(10, 25)));
            DropItem(new Citrine(Utility.RandomMinMax(10, 25)));
            DropItem(new Ruby(Utility.RandomMinMax(10, 25)));
            DropItem(new Tourmaline(Utility.RandomMinMax(10, 25)));
            DropItem(new Amethyst(Utility.RandomMinMax(10, 25)));
            DropItem(new Emerald(Utility.RandomMinMax(10, 25)));
            DropItem(new Sapphire(Utility.RandomMinMax(10, 25)));
            DropItem(new StarSapphire(Utility.RandomMinMax(10, 25)));
            DropItem(new Diamond(Utility.RandomMinMax(10, 25)));

            for (int i = 0; i < 5; i++)
            {
                Type type = m_Types[Utility.Random(m_Types.Length)];

                if (type != null)
                {
                    Item item = Loot.Construct(type);

                    if (item != null)
                    {
                        item.Amount = Utility.RandomMinMax(5, 12);
                        DropItem(item);
                    }
                }
            }
		}

        public GemologistsSatchel(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}

        private static Type[] m_Types = new Type[]
		{
			typeof(MagicalResidue), 	typeof(EnchantEssence), 		typeof(RelicFragment),
			
			typeof(SeedRenewal), 		typeof(ChagaMushroom), 			typeof(CrystalShards),
			typeof(BottleIchor), 		typeof(ReflectiveWolfEye), 		typeof(FaeryDust),
			typeof(BouraPelt), 			typeof(SilverSnakeSkin), 		typeof(ArcanicRuneStone),
			typeof(SlithTongue), 		typeof(VoidOrb), 				typeof(RaptorTeeth),
			typeof(SpiderCarapace), 	typeof(DaemonClaw), 			typeof(VialOfVitriol),
			typeof(GoblinBlood), 		typeof(LavaSerpentCrust), 		typeof(UndyingFlesh),
			typeof(CrushedGlass), 		typeof(CrystallineBlackrock), 	typeof(PowderedIron),
			typeof(ElvenFletchings),    typeof(DelicateScales),
			
			typeof(EssenceSingularity), typeof(EssenceBalance), 		typeof(EssencePassion),
			typeof(EssenceDirection), 	typeof(EssencePrecision), 		typeof(EssenceControl),
			typeof(EssenceDiligence), 	typeof(EssenceAchievement), 	typeof(EssenceFeeling), 
			typeof(EssenceOrder),
			
			typeof(ParasiticPlant), 	typeof(LuminescentFungi), 		typeof(BrilliantAmber), 
			typeof(FireRuby), 			typeof(WhitePearl), 			typeof(BlueDiamond), 
			typeof(Turquoise)
		};
	}
}