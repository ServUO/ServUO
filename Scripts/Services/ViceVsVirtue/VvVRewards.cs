using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;
using Server.Guilds;
using System.Linq;
using Server.Engines.Points;

namespace Server.Engines.VvV
{
    public enum TileType
    {
        North = 0,
        West = 1,
    }

    public static class VvVRewards
    {
        public static List<CollectionItem> Rewards { get; set; }

        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(VvVPotionKeg), 6870, 1094764, 437, 500));  // Greater Stam
            Rewards.Add(new CollectionItem(typeof(VvVPotionKeg), 6870, 1094718, 13, 500));   // Supernova
            Rewards.Add(new CollectionItem(typeof(VvVPotionKeg), 6870, 1155541, 2500, 500)); // Stat Loss Removal
            Rewards.Add(new CollectionItem(typeof(VvVPotionKeg), 6870, 1155543, 2543, 500)); // ANti Paralysis

            Rewards.Add(new CollectionItem(typeof(EssenceOfCourage), 3838, 1155554, 2718, 250)); // Essence of Courage

            Rewards.Add(new CollectionItem(typeof(VvVSteedStatuette), 8484, 1155545, ViceVsVirtueSystem.VirtueHue, 500)); // Virtue War Horse
            Rewards.Add(new CollectionItem(typeof(VvVSteedStatuette), 8484, 1155545, ViceVsVirtueSystem.ViceHue, 500));   // Vice War Horse
            Rewards.Add(new CollectionItem(typeof(VvVSteedStatuette), 8501, 1155546, ViceVsVirtueSystem.VirtueHue, 500)); // Virtue War Ostard
            Rewards.Add(new CollectionItem(typeof(VvVSteedStatuette), 8501, 1155546, ViceVsVirtueSystem.ViceHue, 500));   // Vice War Ostard

            Rewards.Add(new CollectionItem(typeof(VvVHairDye), 3838, 1155538, ViceVsVirtueSystem.VirtueHue, 2500)); // Virtue Hair Dye
            Rewards.Add(new CollectionItem(typeof(VvVHairDye), 3838, 1155539, ViceVsVirtueSystem.ViceHue, 2500));   // Vice Hair DYe

            Rewards.Add(new CollectionItem(typeof(VvVTrapKit), 7866, 1155527, 0, 250));    // Poison Trap Kit
            Rewards.Add(new CollectionItem(typeof(VvVTrapKit), 7866, 1155528, 0, 250));    // Freezing Trap Kit
            Rewards.Add(new CollectionItem(typeof(VvVTrapKit), 7866, 1155529, 0, 250));    // Shocking Trap Kit
            Rewards.Add(new CollectionItem(typeof(VvVTrapKit), 7866, 1155530, 0, 250));    // Blades Trap Kit
            Rewards.Add(new CollectionItem(typeof(VvVTrapKit), 7866, 1155531, 0, 250));    // Explosion Trap Kit

            Rewards.Add(new CollectionItem(typeof(CannonTurretPlans), 5360, 1155503, 0, 3000));    // Cannon Turret
            Rewards.Add(new CollectionItem(typeof(ManaSpike), 2308, 1155508, 0, 1000));            // Mana Spike

            Rewards.Add(new CollectionItem(typeof(ForgedRoyalPardon), 18098, 1155524, 0, 10000));        // Royal Forged Pardon
            Rewards.Add(new CollectionItem(typeof(ScrollofTranscendence), 5360, 1094934, 0x490, 10000));   // Scroll of Transcendence

            Rewards.Add(new CollectionItem(typeof(VvVRobe), 9859, 1155532, ViceVsVirtueSystem.VirtueHue, 5000)); // virtue robe
            Rewards.Add(new CollectionItem(typeof(VvVRobe), 9859, 1155533, ViceVsVirtueSystem.ViceHue, 5000)); // virtue robe

            Rewards.Add(new CollectionItem(typeof(CovetousTileDeed), 5360, 1155516, 0, 10000)); // Covetous Tile
            Rewards.Add(new CollectionItem(typeof(DeceitTileDeed), 5360, 1155517, 0, 10000)); // Deceit Tile
            Rewards.Add(new CollectionItem(typeof(DespiseTileDeed), 5360, 1155518, 0, 10000)); // Depise Tile
            Rewards.Add(new CollectionItem(typeof(DestardTileDeed), 5360, 1155519, 0, 10000)); // Destard Tile
            Rewards.Add(new CollectionItem(typeof(HythlothTileDeed), 5360, 1155520, 0, 10000)); // Hythloth Tile
            Rewards.Add(new CollectionItem(typeof(PrideTileDeed), 5360, 1155521, 0, 10000)); // Pride Tile
            Rewards.Add(new CollectionItem(typeof(ShameTileDeed), 5360, 1155522, 0, 10000)); // Shame Tile
            Rewards.Add(new CollectionItem(typeof(WrongTileDeed), 5360, 1155523, 0, 10000)); // Wrong Tile

            Rewards.Add(new CollectionItem(typeof(MorphEarrings), 4231, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(MaceAndShieldGlasses), 12216, 0, 477, 500));
            Rewards.Add(new CollectionItem(typeof(InquisitorsResolution), 5140, 0, 1266, 500));
            Rewards.Add(new CollectionItem(typeof(OrnamentOfTheMagician), 4230, 0, 1364, 500));
            Rewards.Add(new CollectionItem(typeof(VesperOrderShield), 7108, 0, 0, 500)); // Needs 0 FC
            Rewards.Add(new CollectionItem(typeof(ClaininsSpellbook), 3834, 0, 2125, 500));
            Rewards.Add(new CollectionItem(typeof(FoldedSteelGlasses), 12216, 0, 1150, 500));
            Rewards.Add(new CollectionItem(typeof(CrystallineRing), 4234, 0, 1152, 500));
            Rewards.Add(new CollectionItem(typeof(SpiritOfTheTotem), 5445, 0, 1109, 500));
            Rewards.Add(new CollectionItem(typeof(WizardsCrystalGlasses), 12216, 0, 688, 500));
            Rewards.Add(new CollectionItem(typeof(PrimerOnArmsTalisman), 12121, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(TomeOfLostKnowledge), 3834, 0, 1328, 500));
            Rewards.Add(new CollectionItem(typeof(HuntersHeaddress), 5447, 0, 1428, 500));
            Rewards.Add(new CollectionItem(typeof(HeartOfTheLion), 5141, 0, 1281, 500));
            Rewards.Add(new CollectionItem(typeof(CrimsonCincture), 5435, 0, 1157, 500));
            Rewards.Add(new CollectionItem(typeof(RingOfTheVile), 4234, 0, 1271, 500));
            Rewards.Add(new CollectionItem(typeof(FeyLeggings), 5054, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(Stormgrip), 10130, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(RuneBeetleCarapace), 10109, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(KasaOfTheRajin), 10136, 0, 0, 500));

            Rewards.Add(new CollectionItem(typeof(VvVWand1), 3571, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVWand2), 3571, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVWizardsHat), 5912, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVWand1), 3571, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVWoodlandArms), 11116, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVDragonArms), 9815, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVGargishPlateArms), 776, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVPlateArms), 5136, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVEpaulette), 0x9985, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVGargishEpaulette), 0x9986, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVGargishStoneChest), 0x286, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVStuddedChest), 5083, 0, 0, 500));
            Rewards.Add(new CollectionItem(typeof(VvVGargishEarrings), 16915, 0, 0, 500));

            Rewards.Add(new CollectionItem(typeof(CompassionBanner), 39351, 1123375, 0, 10000)); // Compassion Banner
            Rewards.Add(new CollectionItem(typeof(HonestyBanner), 39353, 1123377, 0, 10000)); // Honesty Banner
            Rewards.Add(new CollectionItem(typeof(HonorBanner), 39355, 1123379, 0, 10000)); // Honor Banner
            Rewards.Add(new CollectionItem(typeof(HumilityBanner), 39357, 1123381, 0, 10000)); // Humility Banner
            Rewards.Add(new CollectionItem(typeof(JusticeBanner), 39359, 1123383, 0, 10000)); // Justice Banner
            Rewards.Add(new CollectionItem(typeof(SacraficeBanner), 39361, 1123385, 0, 10000)); // Sacrafice Banner
            Rewards.Add(new CollectionItem(typeof(SpiritualityBanner), 39363, 1123387, 0, 10000)); // Spirituality Banner
            Rewards.Add(new CollectionItem(typeof(ValorBanner), 1123389, 39365, 0, 10000)); // Valor Banner

            Rewards.Add(new CollectionItem(typeof(CovetousBanner), 39335, 1123359, 0, 10000)); // Covetous Banner
            Rewards.Add(new CollectionItem(typeof(DeceitBanner), 39337, 1123361, 0, 10000)); // Deceit Banner
            Rewards.Add(new CollectionItem(typeof(DespiseBanner), 39339, 1123363, 0, 10000)); // Depise Banner
            Rewards.Add(new CollectionItem(typeof(DestardBanner), 39341, 1123365, 0, 10000)); // Destard Banner
            Rewards.Add(new CollectionItem(typeof(HythlothBanner), 39343, 1123367, 0, 10000)); // Hythloth Banner
            Rewards.Add(new CollectionItem(typeof(PrideBanner), 39345, 1123369, 0, 10000)); // Pride Banner
            Rewards.Add(new CollectionItem(typeof(ShameBanner), 39347, 1123371, 0, 10000)); // Shame Banner
            Rewards.Add(new CollectionItem(typeof(WrongBanner), 39349, 1123373, 0, 10000)); // Wrong Banner
        }
    }
}