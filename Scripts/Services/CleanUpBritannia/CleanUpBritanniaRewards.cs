using Server;
using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.CleanUpBritannia
{
    public static class CleanUpBritanniaRewards
    {
        public static List<CollectionItem> Rewards { get; set; }
        
        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(Mailbox), 0x4142, 1153729, 0, 1000));
            Rewards.Add(new CollectionItem(typeof(HumansAndElvesRobe), 0x1F03, 1023744, 0, 1000));
            Rewards.Add(new CollectionItem(typeof(GargoylesAreOurFriendsRobe), 0x1F03, 1023744, 0, 1000));
            Rewards.Add(new CollectionItem(typeof(WeArePiratesRobe), 0x1F03, 1023744, 0, 1000));
            Rewards.Add(new CollectionItem(typeof(FollowerOfBaneRobe), 0x1F03, 1023744, 0, 1000));
            Rewards.Add(new CollectionItem(typeof(QueenDawnForeverRobe), 0x1F03, 1023744, 0, 1000));
            Rewards.Add(new CollectionItem(typeof(LillyPad), 0xDBC, 1023744, 0, 5000));
            Rewards.Add(new CollectionItem(typeof(LillyPads), 0xDBE, 1023744, 0, 5000));
            Rewards.Add(new CollectionItem(typeof(NocturneEarrings), 0x1F07, 1023744, 0x3E5, 5000));
            Rewards.Add(new CollectionItem(typeof(SherryTheMouseStatue), 0x20D0, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(ChaosTileDeed), 0x14EF, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(HonestyVirtueTileDeed), 0x14EF, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(CompassionVirtueTileDeed), 0x14EF, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(ValorVirtueTileDeed), 0x14EF, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(SpiritualityVirtueTileDeed), 0x14EF, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(HonorVirtueTileDeed), 0x14EF, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(HumilityVirtueTileDeed), 0x14EF, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(SacrificeVirtueTileDeed), 0x14EF, 1023744, 0, 10000));
            Rewards.Add(new CollectionItem(typeof(StewardDeed), 0x14F0, 1023744, 0, 10000));

            Rewards.Add(new CollectionItem(typeof(KnightsBascinet), 0x140C, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsCloseHelm), 0x1408, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsFemalePlateChest), 0x1C04, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsNorseHelm), 0x140E, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsPlateArms), 0x1410, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsPlateChest), 0x1415, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsPlateGloves), 0x1414, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsPlateGorget), 0x1413, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsPlateHelm), 0x1412, 1023744, 1150, 10000));
            Rewards.Add(new CollectionItem(typeof(KnightsPlateLegs), 0x1411, 1023744, 1150, 10000));

            Rewards.Add(new CollectionItem(typeof(ScoutArms), 0x13DC, 1023744, 1148, 10000));
            Rewards.Add(new CollectionItem(typeof(ScoutBustier), 0x1C0C, 1023744, 1148, 10000));
            Rewards.Add(new CollectionItem(typeof(ScoutChest), 0x13DB, 1023744, 1148, 10000));
            Rewards.Add(new CollectionItem(typeof(ScoutCirclet), 0x2B6E, 1023744, 1148, 10000));
            Rewards.Add(new CollectionItem(typeof(ScoutFemaleChest), 0x1C02, 1023744, 1148, 10000));
            Rewards.Add(new CollectionItem(typeof(ScoutGloves), 0x13D5, 1023744, 1148, 10000));
            Rewards.Add(new CollectionItem(typeof(ScoutGorget), 0x13D6, 1023744, 1148, 10000));
            Rewards.Add(new CollectionItem(typeof(ScoutLegs), 0x13DA, 1023744, 1148, 10000));
            Rewards.Add(new CollectionItem(typeof(ScoutSmallPlateJingasa), 0x2784, 1023744, 1148, 10000));

            Rewards.Add(new CollectionItem(typeof(SorcererArms), 0x13CD, 1023744, 1165, 10000));
            Rewards.Add(new CollectionItem(typeof(SorcererChest), 0x13CC, 1023744, 1165, 10000));
            Rewards.Add(new CollectionItem(typeof(SorcererFemaleChest), 0x1C06, 1023744, 1165, 10000));
            Rewards.Add(new CollectionItem(typeof(SorcererGloves), 0x13C6, 1023744, 1165, 10000));
            Rewards.Add(new CollectionItem(typeof(SorcererGorget), 0x13C7, 1023744, 1165, 10000));
            Rewards.Add(new CollectionItem(typeof(SorcererHat), 0x1718, 1023744, 1165, 10000));
            Rewards.Add(new CollectionItem(typeof(SorcererLegs), 0x13CB, 1023744, 1165, 10000));
            Rewards.Add(new CollectionItem(typeof(SorcererSkirt), 0x1C08, 1023744, 1165, 10000));

            Rewards.Add(new CollectionItem(typeof(YuccaTree), 0x0D37, 1023744, 0, 15000));
            Rewards.Add(new CollectionItem(typeof(TableLamp), 0x49C1, 1023744, 0, 15000));
            Rewards.Add(new CollectionItem(typeof(Bamboo), 0x246D, 1023744, 0, 15000));

            Rewards.Add(new CollectionItem(typeof(HorseBardingDeed), 0x14EF, 1023744, 0, 20000));
            Rewards.Add(new CollectionItem(typeof(ScrollofAlacrity), 0x14EF, 1023744, 1195, 20000));

            Rewards.Add(new CollectionItem(typeof(SnakeSkinBoots), 0x170B, 1023744, 0x7D9, 20000));
            Rewards.Add(new CollectionItem(typeof(BootsOfTheLavaLizard), 0x170B, 1023744, 0x674, 20000));
            Rewards.Add(new CollectionItem(typeof(BootsOfTheIceWyrm), 0x170B, 1023744, 0x482, 20000));
            Rewards.Add(new CollectionItem(typeof(BootsOfTheCrystalHydra), 0x170B, 1023744, 0x47E, 20000));
            Rewards.Add(new CollectionItem(typeof(BootsOfTheThrasher), 0x170B, 1023744, 0x497, 20000));

            Rewards.Add(new CollectionItem(typeof(NaturesTears), 0x0E9C, 1023744, 2075, 20000));
            Rewards.Add(new CollectionItem(typeof(PrimordialDecay), 0x0E9C, 1023744, 1927, 20000));
            Rewards.Add(new CollectionItem(typeof(ArachnidDoom), 0x0E9C, 1023744, 1944, 20000));

            Rewards.Add(new CollectionItem(typeof(SophisticatedElvenTapestry), 0x2D70, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(OrnateElvenTapestry), 0x2D72, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(ChestOfDrawers), 0x0A2C, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(FootedChestOfDrawers), 0x0A30, 1023744, 0, 50000));

            Rewards.Add(new CollectionItem(typeof(DragonHeadDeed), 0x2234, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(NestWithEggs), 0x1AD4, 1023744, 2415, 50000));

            Rewards.Add(new CollectionItem(typeof(FishermansHat), 0x1716, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(FishermansTrousers), 0x13DA, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(FishermansVest), 0x13CC, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(FishermansEelskinGloves), 0x13C6, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(FishermansChestguard), 0x4052, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(FishermansKilt), 0x0408, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(FishermansArms), 0x0302, 1023744, 0, 50000));
            Rewards.Add(new CollectionItem(typeof(FishermansEarrings), 0x4213, 1023744, 0, 50000));

            Rewards.Add(new CollectionItem(typeof(FirePitDeed), 0x29FD, 1023744, 0, 75000));
            Rewards.Add(new CollectionItem(typeof(PresentationStone), 0x32F2, 1023744, 0, 75000));
            Rewards.Add(new CollectionItem(typeof(Beehive), 0x091A, 1023744, 0, 80000));
            Rewards.Add(new CollectionItem(typeof(ArcheryButteAddon), 0x100B, 1023744, 0, 80000));
        }
    }
}