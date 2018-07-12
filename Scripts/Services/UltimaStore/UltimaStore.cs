using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.VendorSearching;
using Server.Gumps;
using Server.Network;
using Server.Engines.Points;
using System.IO;
using Server.Multis;

namespace Server.Engines.UOStore
{
    public enum StoreCategory
    {
        None,
        Featured,
        Character,
        Equipment,
        Decorations,
        Mounts,
        Misc,
        Cart
    }

    public enum SortBy
    {
        Name,
        PriceLower,
        PriceHigher,
        Newest,
        Oldest
    }

    public static class UltimaStore
    {
        public static int MaxCart = 10;
        public static int MaxPurchase = 125;

        public static void Initialize()
        {
            if (Core.TOL)
            {
                PacketHandlers.Register(0xFA, 1, true, UOStoreRequest);
                LoadEntries();
            }
        }

        public static void UOStoreRequest(NetState state, PacketReader pvSrc)
        {
            Mobile m = state.Mobile;
            bool canUse = true;

            if (Configuration.CurrencyType == CurrencyType.None)
            {
                m.SendLocalizedMessage(1062904); // The promo code redemption system is currently unavailable. Please try again later.
                canUse = false;
            }
            
            if (m.AccessLevel == AccessLevel.Player && !CanSearch(m))
            {
                m.SendLocalizedMessage(1156586);
                canUse = false;
                /*Before using the in game store, you must be in a safe log-out location
                 * (such as an inn or a house which has you on its Owner, Co-owner, or Friends list).*/
            }
            
            if (canUse && !m.HasGump(typeof(UltimaStoreGump)))
            {
                BaseGump.SendGump(new UltimaStoreGump((PlayerMobile)m));
            }
        }

        public static bool CanSearch(Mobile m)
        {
            return m.Region.GetLogoutDelay(m) == TimeSpan.Zero;
        }

        public static List<StoreEntry> Entries { get; private set; }
        public static Dictionary<Mobile, List<Item>> PendingItems { get; private set; }

        private static UltimaStoreContainer _UltimaStoreContainer;

        public static UltimaStoreContainer UltimaStoreContainer
        {
            get
            {
                if (_UltimaStoreContainer == null)
                {
                    _UltimaStoreContainer = new UltimaStoreContainer();
                    _UltimaStoreContainer.MoveToWorld(new Point3D(5426, 1082, 0), Map.Felucca);
                }

                return _UltimaStoreContainer;
            }
        }

        private static void LoadEntries()
        {
            Entries = new List<StoreEntry>();

            // Featured
            StoreCategory cat = StoreCategory.Featured;
            Register(new StoreEntry(typeof(VirtueShield), 1109616, 1158384, 0x7818, 0, 0, 1500, cat));
            Register(new StoreEntry(typeof(SoulstoneToken), 1158404, 1158405, 0x2A93, 0, 2598, 1000, cat, ConstructSoulstone));
            //Register(new StoreEntry(typeof(DeluxeStarterPackToken), 1158368, 1158369, 0, 0x9CCB, 0, 2000, cat));
            Register(new StoreEntry(typeof(GreenGoblinStatuette), 1125133, 1158015, 0xA095, 0, 0, 600, cat));
            //Register(new StoreEntry(typeof(TotemOfChromaticFortune), 1157606, 1157604, 0, 0x9CC9, 0, 300, cat));
            Register(new StoreEntry(typeof(MythicCharacterToken), new TextDefinition[] { 1156614, 1156615 }, 1156679, 0x2AAA, 0, 0, 2500, cat));

            // Character
            cat = StoreCategory.Character;
            //Register(new StoreEntry(typeof(DeluxeStarterPackToken), 1158368, 1158369, 0, 0x9CCB, 0, 2000, cat));
            Register(new StoreEntry(typeof(GreenGoblinStatuette), 1125133, 1158015, 0xA095, 0, 0, 600, cat));
            Register(new StoreEntry(typeof(GreyGoblinStatuette), 1125135, 1158015, 0xA097, 0, 0, 600, cat));
            Register(new StoreEntry(typeof(StableSlotIncreaseToken), 1157608, 1157609, 0x2AAA, 0, 0, 500, cat));
            Register(new StoreEntry(typeof(MythicCharacterToken), new TextDefinition[] { 1156614, 1156615 }, 1156679, 0x2AAA, 0, 0, 2500, cat));
            Register(new StoreEntry(typeof(CharacterReincarnationToken), new TextDefinition[] { 1156612, 1156615 }, 1156677, 0x2AAA, 0, 0, 2000, cat));

            Register(new StoreEntry(typeof(AbyssalHairDye), 1149822, 1156676, 0, 0x9C7A, 0, 400, cat));
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071439 }, 1156676, 0, 0x9C78, 0, 400, cat, ConstructHairDye)); // Lemon Lime
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071470 }, 1156676, 0, 0x9C6D, 0, 400, cat, ConstructHairDye)); // Yew Brown 
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071471 }, 1156676, 0, 0x9C6E, 0, 400, cat, ConstructHairDye)); // Bloodwood Red
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071438 }, 1156676, 0, 0x9C6F, 0, 400, cat, ConstructHairDye)); // Vivid Blue
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071469 }, 1156676, 0, 0x9C71, 0, 400, cat, ConstructHairDye)); // Ash Blonde
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071472 }, 1156676, 0, 0x9C72, 0, 400, cat, ConstructHairDye)); // Heartwood Green
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071472 }, 1156676, 0, 0x9C85, 0, 400, cat, ConstructHairDye)); // Oak Blonde
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071474 }, 1156676, 0, 0x9C70, 0, 400, cat, ConstructHairDye)); // Sacred White
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071473 }, 1156676, 0, 0x9C73, 0, 400, cat, ConstructHairDye)); // Frostwood Ice Green
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071440 }, 1156676, 0, 0x9C76, 0, 400, cat, ConstructHairDye)); // Fiery Blonde
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071437 }, 1156676, 0, 0x9C77, 0, 400, cat, ConstructHairDye)); // Bitter Brown
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071442 }, 1156676, 0, 0x9C74, 0, 400, cat, ConstructHairDye)); // Gnaw's Twisted Blue
            Register(new StoreEntry(typeof(SpecialHairDye), new TextDefinition[] { 1071387, 1071441 }, 1156676, 0, 0x9C75, 0, 400, cat, ConstructHairDye)); // Dusk Black

            Register(new StoreEntry(typeof(GenderChangeToken), new TextDefinition[] { 1156609, 1156615 }, 1156642, 0x2AAA, 0, 0, 1000, cat));
            Register(new StoreEntry(typeof(NameChangeToken), new TextDefinition[] { 1156608, 1156615 }, 1156641, 0x2AAA, 0, 0, 1000, cat));

            // Equipment
            cat = StoreCategory.Equipment;
            Register(new StoreEntry(typeof(VirtueShield), 1109616, 1158384, 0x7818, 0, 0, 1500, cat));
            Register(new StoreEntry(typeof(HoodedBritanniaRobe), 1125155, 1158016, 0xA0AB, 0, 0, 1500, cat, ConstructRobe));
            Register(new StoreEntry(typeof(HoodedBritanniaRobe), 1125155, 1158016, 0xA0AC, 0, 0, 1500, cat, ConstructRobe));
            Register(new StoreEntry(typeof(HoodedBritanniaRobe), 1125155, 1158016, 0xA0AD, 0, 0, 1500, cat, ConstructRobe));
            Register(new StoreEntry(typeof(HoodedBritanniaRobe), 1125155, 1158016, 0xA0AE, 0, 0, 1500, cat, ConstructRobe));
            Register(new StoreEntry(typeof(HoodedBritanniaRobe), 1125155, 1158016, 0xA0AF, 0, 0, 1500, cat, ConstructRobe));

            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1157275 }, 1156671, 0, 0x9CBF, 0, 400, cat, ConstructHaochisPigment)); // Heartwood Sienna
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1157274 }, 1156671, 0, 0x9CBD, 0, 400, cat, ConstructHaochisPigment)); // Campion White
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1157273 }, 1156671, 0, 0x9CC2, 0, 400, cat, ConstructHaochisPigment)); // Yewish Pine
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1157272 }, 1156671, 0, 0x9CC0, 0, 400, cat, ConstructHaochisPigment)); // Minocian Fire
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1157269 }, 1156671, 0, 0x9CC1, 0, 400, cat, ConstructHaochisPigment)); // Celtic Lime

            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070994 }, 1156906, 0, 0x9CA8, 0, 400, cat, ConstructPigments)); // Nox Green
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079584 }, 1156906, 0, 0x9CAF, 0, 400, cat, ConstructPigments)); // Midnight Coal
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070995 }, 1156906, 0, 0x9CA5, 0, 400, cat, ConstructPigments)); // Rum Red
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079580 }, 1156906, 0, 0x9CA4, 0, 400, cat, ConstructPigments)); // Coal
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079582 }, 1156906, 0, 0x9CA3, 0, 400, cat, ConstructPigments)); // Storm Bronze
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079581 }, 1156906, 0, 0x9CA2, 0, 400, cat, ConstructPigments)); // Faded Gold
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070988 }, 1156906, 0, 0x9CA1, 0, 400, cat, ConstructPigments)); // Violet Courage Purple
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079585 }, 1156906, 0, 0x9CA2, 0, 400, cat, ConstructPigments)); // Faded Bronze
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070996 }, 1156906, 0, 0x9C9F, 0, 400, cat, ConstructPigments)); // Fire Orange
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079586 }, 1156906, 0, 0x9C9E, 0, 400, cat, ConstructPigments)); // Faded Rose
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079583 }, 1156906, 0, 0x9CA7, 0, 400, cat, ConstructPigments)); // Rose
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079587 }, 1156906, 0, 0x9CA9, 0, 400, cat, ConstructPigments)); // Deep Rose
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070990 }, 1156906, 0, 0x9CAA, 0, 400, cat, ConstructPigments)); // Luna White

            Register(new StoreEntry(typeof(CommemorativeRobe), 1157009, 1156908, 0x4B9D, 0, 0, 500, cat));

            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070992 }, 1156906, 0, 0x9CAF, 0, 400, cat, ConstructPigments)); // Shadow Dancer Black
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070989 }, 1156906, 0, 0x9CAE, 0, 400, cat, ConstructPigments)); // Invulnerability Blue
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070991 }, 1156906, 0, 0x9CAD, 0, 400, cat, ConstructPigments)); // Dryad Green
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070993 }, 1156906, 0, 0x9CAC, 0, 400, cat, ConstructPigments)); // Berserker Red
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1079579 }, 1156906, 0, 0x9CAB, 0, 400, cat, ConstructPigments)); // Faded Coal
            Register(new StoreEntry(typeof(PigmentsOfTokuno), new TextDefinition[] { 1070933, 1070987 }, 1156906, 0, 0x9C9D, 0, 400, cat, ConstructPigments)); // Paragon Gold

            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1071246 }, 1156671, 0, 0x9CAF, 0, 400, cat, ConstructHaochisPigment)); // Ninja Black
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1018352 }, 1156671, 0, 0x9C83, 0, 400, cat, ConstructHaochisPigment)); // Olive
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1071247 }, 1156671, 0, 0x9C7D, 0, 400, cat, ConstructHaochisPigment)); // Dark Reddish Brown
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1071245 }, 1156671, 0, 0x9C85, 0, 400, cat, ConstructHaochisPigment)); // Yellow
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1071244 }, 1156671, 0, 0x9C80, 0, 400, cat, ConstructHaochisPigment)); // Pretty Pink
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1071248 }, 1156671, 0, 0x9C81, 0, 400, cat, ConstructHaochisPigment)); // Midnight Blue
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1023856 }, 1156671, 0, 0x9C7F, 0, 400, cat, ConstructHaochisPigment)); // Emerald
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1115467 }, 1156671, 0, 0x9C82, 0, 400, cat, ConstructHaochisPigment)); // Smoky Gold
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1115468 }, 1156671, 0, 0x9C7E, 0, 400, cat, ConstructHaochisPigment)); // Ghost's Grey
            Register(new StoreEntry(typeof(HaochisPigment), new TextDefinition[] { 1071249, 1115471 }, 1156671, 0, 0x9C84, 0, 400, cat, ConstructHaochisPigment)); // Ocean Blue   

            Register(new StoreEntry(typeof(SmugglersEdge), 1071499, 1156664, 0, 0x9C63, 0, 400, cat));
            Register(new StoreEntry(typeof(UndertakersStaff), 1071498, 1156663, 0x13F8, 0, 0, 500, cat));
            Register(new StoreEntry(typeof(ReptalonFormTalisman), new TextDefinition[] { 1157010, 1075202 }, 1156967, 0x2F59, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(QuiverOfInfinity), 1075201, 1156971, 0x2B02, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(CuSidheFormTalisman), new TextDefinition[] { 1157010, 1031670 }, 1156970, 0x2F59, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(FerretFormTalisman), new TextDefinition[] { 1157010, 1031672 }, 1156969, 0x2F59, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(LeggingsOfEmbers), 1062911, 1156956, 0x1411, 0, 0x2C, 100, cat));
            Register(new StoreEntry(typeof(ShaminoCrossbow), 1062915, 1156957, 0x26C3, 0, 0x504, 100, cat));
            Register(new StoreEntry(typeof(SamuraiHelm), 1062923, 1156959, 0x236C, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(HolySword), 1062921, 1156962, 0xF61, 0, 0x482, 100, cat));
            Register(new StoreEntry(typeof(DupresShield), 1075196, 1156963, 0x2B01, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(OssianGrimoire), 1078148, 1156965, 0x2253, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(SquirrelFormTalisman), new TextDefinition[] { 1157010, 1031671 }, 1156966, 0x2F59, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(EarringsOfProtection), new TextDefinition[] { 1156821, 1156822 }, 1156659, 0, 0x9C66, 0, 200, cat, ConstructEarrings)); // Physcial
            Register(new StoreEntry(typeof(EarringsOfProtection), 1071092, 1156659, 0, 0x9C66, 0, 200, cat, ConstructEarrings)); // Fire
            Register(new StoreEntry(typeof(EarringsOfProtection), 1071093, 1156659, 0, 0x9C66, 0, 200, cat, ConstructEarrings)); // Cold
            Register(new StoreEntry(typeof(EarringsOfProtection), 1071094, 1156659, 0, 0x9C66, 0, 200, cat, ConstructEarrings)); // Poison
            Register(new StoreEntry(typeof(EarringsOfProtection), 1071095, 1156659, 0, 0x9C66, 0, 200, cat, ConstructEarrings)); // Energy
            Register(new StoreEntry(typeof(HoodedShroudOfShadows), 1079727, 1156643, 0x2684, 0, 0x455, 1000, cat));

            // decorations
            cat = StoreCategory.Decorations;
            Register(new StoreEntry(typeof(DecorativeBlackwidowDeed), 1157897, 1157898, 0, 0x9CD7, 0, 600, cat));
            Register(new StoreEntry(typeof(HildebrandtDragonRugDeed), 1157889, 1157890, 0, 0x9CD8, 0, 700, cat));
            Register(new StoreEntry(typeof(SmallWorldTreeRugAddonDeed), 1157206, 1157898, 0, 0x9CBA, 0, 300, cat));
            Register(new StoreEntry(typeof(LargeWorldTreeRugAddonDeed), 1157207, 1157898, 0, 0x9CBA, 0, 500, cat));
            Register(new StoreEntry(typeof(MountedPixieWhiteDeed), new TextDefinition[] { 1074482, 1156915 }, 1156974, 0x2A79, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(MountedPixieLimeDeed), new TextDefinition[] { 1074482, 1156914 }, 1156974, 0x2A77, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(MountedPixieBlueDeed), new TextDefinition[] { 1074482, 1156913 }, 1156974, 0x2A75, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(MountedPixieOrangeDeed), new TextDefinition[] { 1074482, 1156912 }, 1156974, 0x2A73, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(MountedPixieGreenDeed), new TextDefinition[] { 1074482, 1156911 }, 1156974, 0x2A71, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(UnsettlingPortraitDeed), 1074480, 1156973, 0x2A65, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(CreepyPortraitDeed), 1074481, 1156972, 0x2A69, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(DisturbingPortraitDeed), 1074479, 1156955, 0x2A5D, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(DawnsMusicBox), 1075198, 1156968, 0x2AF9, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(BedOfNailsDeed), 1074801, 1156975, 0, 0x9C8D, 0, 100, cat));
            Register(new StoreEntry(typeof(BrokenCoveredChairDeed), 1076257, 1156950, 0xC17, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(BoilingCauldronDeed), 1076267, 1156949, 0, 0x9CB9, 0, 100, cat));
            Register(new StoreEntry(typeof(SuitOfGoldArmorDeed), 1076265, 1156943, 0x3DAA, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(BrokenBedDeed), 1076263, 1156945, 0, 0x9C8F, 0, 100, cat));
            Register(new StoreEntry(typeof(BrokenArmoireDeed), 1076262, 1156946, 0xC12, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(BrokenVanityDeed), 1076260, 1156947, 0, 0x9C90, 0, 100, cat));
            Register(new StoreEntry(typeof(BrokenBookcaseDeed), 1076258, 1156948, 0xC14, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(SacrificialAltarDeed), 1074818, 1156954, 0, 0x9C8E, 0, 100, cat));
            Register(new StoreEntry(typeof(HauntedMirrorDeed), 1074800, 1156953, 0x2A7B, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(BrokenChestOfDrawersDeed), 1076261, 1156951, 0xC24, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(StandingBrokenChairDeed), 1076259, 1156952, 0xC1B, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(FountainOfLifeDeed), 1075197, 1156964, 0x2AC0, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(TapestryOfSosaria), 1062917, 1156961, 0x234E, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(RoseOfTrinsic), 1062913, 1156960, 0x234D, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(HearthOfHomeFireDeed), 1062919, 1156958, 0, 0x9C97, 0, 100, cat));
            // TODO: Singing Ball
            // TODO: Secret Chest

            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1157015 }, 1156916, 0, 0x9CB5, 0, 200, cat, ConstructMiniHouseDeed)); // two story wood & plaster
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011317 }, 1156916, 0x22F5, 0, 0, 200, cat, ConstructMiniHouseDeed)); // small stone tower
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011307 }, 1156916, 0x22E0, 0, 0, 200, cat, ConstructMiniHouseDeed)); // wood and plaster house
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011308 }, 1156916, 0x22E1, 0, 0, 200, cat, ConstructMiniHouseDeed)); // thathed-roof cottage
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011312 }, 1156916, 0, 0x9CB2, 0, 200, cat, ConstructMiniHouseDeed)); // Tower
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011313 }, 1156916, 0, 0x9CB1, 0, 200, cat, ConstructMiniHouseDeed)); // Small stone keep
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011314 }, 1156916, 0, 0x9CB0, 0, 200, cat, ConstructMiniHouseDeed)); // Castle

            Register(new StoreEntry(typeof(HangingSwordsDeed), 1076272, 1156936, 0, 0x9C96, 0, 100, cat));
            Register(new StoreEntry(typeof(UnmadeBedDeed), 1076279, 1156935, 0, 0x9C9B, 0, 100, cat));
            Register(new StoreEntry(typeof(CurtainsDeed), 1076280, 1156934, 0, 0x9C93, 0, 100, cat));
            Register(new StoreEntry(typeof(TableWithOrangeClothDeed), new TextDefinition[] { 1157012, 1157013 }, 1156933, 0x118E, 0, 0, 100, cat));

            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011320 }, 1156916, 0x22F3, 0, 0, 200, cat, ConstructMiniHouseDeed)); // sanstone house with patio
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011316 }, 1156916, 0, 0x9CB3, 0, 200, cat, ConstructMiniHouseDeed)); // marble house with patio
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011319 }, 1156916, 0x2300, 0, 0, 200, cat, ConstructMiniHouseDeed)); // two story villa
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1157014 }, 1156916, 0, 0x9CB6, 0, 200, cat, ConstructMiniHouseDeed)); // two story stone & plaster
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011315 }, 1156916, 0, 0x9CB4, 0, 200, cat, ConstructMiniHouseDeed)); // Large house with patio
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011309 }, 1156916, 0, 0x9CB7, 0, 200, cat, ConstructMiniHouseDeed)); // brick house
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011304 }, 1156916, 0x22C9, 0, 0, 200, cat, ConstructMiniHouseDeed)); // field stone house
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011306 }, 1156916, 0x22DF, 0, 0, 200, cat, ConstructMiniHouseDeed)); // wooden house
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011305 }, 1156916, 0x22DE, 0, 0, 200, cat, ConstructMiniHouseDeed)); // small brick house
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011303 }, 1156916, 0x22E1, 0, 0, 200, cat, ConstructMiniHouseDeed)); // stone and plaster house
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011318 }, 1156916, 0x22FB, 0, 0, 200, cat, ConstructMiniHouseDeed)); // two-story log cabin
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011321 }, 1156916, 0x22F6, 0, 0, 200, cat, ConstructMiniHouseDeed)); // small stone workshop
            Register(new StoreEntry(typeof(MiniHouseDeed), new TextDefinition[] { 1062096, 1011322 }, 1156916, 0x22F4, 0, 0, 200, cat, ConstructMiniHouseDeed)); // small marble workshop

            Register(new StoreEntry(typeof(TableWithBlueClothDeed), 1076276, 1156932, 0x118C, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(CherryBlossomTreeDeed), 1076268, 1156940, 0, 0x9C91, 0, 100, cat));
            Register(new StoreEntry(typeof(IronMaidenDeed), 1076288, 1156924, 0x1249, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(SmallFishingNetDeed), 1076286, 1156923, 0x1EA3, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(StoneStatueDeed), 1076284, 1156922, 0, 0x9C9A, 0, 100, cat));
            Register(new StoreEntry(typeof(WallTorchDeed), 1076282, 1156921, 0x3D98, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(HouseLadderDeed), 1076287, 1156920, 0x2FDE, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(LargeFishingNetDeed), 1076285, 1156919, 0x3D8E, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(FountainDeed), 1076283, 1156918, 0, 0x9C94, 0, 100, cat));
            Register(new StoreEntry(typeof(ScarecrowDeed), 1076608, 1156917, 0x1E34, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(HangingAxesDeed), 1076271, 1156937, 0, 0x9C95, 0, 100, cat));
            Register(new StoreEntry(typeof(AppleTreeDeed), 1076269, 1156938, 0, 0x9C8C, 0, 100, cat));
            Register(new StoreEntry(typeof(GuillotineDeed), 1024656, 1156941, 0x125E, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(SuitOfSilverArmorDeed), 1076266, 1156942, 0x3D86, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(PeachTreeDeed), 1076270, 1156939, 0, 0x9C98, 0, 100, cat));
            Register(new StoreEntry(typeof(CherryBlossomTrunkDeed), 1076784, 1156925, 0x26EE, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(PeachTrunkDeed), 1076786, 1156926, 0xD9C, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(BrokenFallenChairDeed), 1076264, 1156944, 0xC19, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(TableWithRedClothDeed), 1076277, 1156930, 0x118E, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(VanityDeed), 1074027, 1156931, 0, 0x9C9C, 0, 100, cat));
            Register(new StoreEntry(typeof(AppleTrunkDeed), 1076785, 1156927, 0xD98, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(TableWithPurpleClothDeed), new TextDefinition[] { 1157011, 1157013 }, 1156929, 0x118B, 0, 0, 100, cat));
            Register(new StoreEntry(typeof(WoodenCoffinDeed), 1076274, 1156928 , 0, 0x9C92, 0, 100, cat));
            Register(new StoreEntry(typeof(RaisedGardenDeed), new TextDefinition[] { 1150359, 1156688 }, 1156680, 0, 0x9C8B, 0, 2000, cat, ConstructRaisedGarden));
            Register(new StoreEntry(typeof(HouseTeleporterTileBag), new TextDefinition[] { 1156683, 1156826 }, 1156668, 0x40B9, 0, 1201, 1000, cat));
            Register(new StoreEntry(typeof(WoodworkersBenchDeed), 1026641, 1156670, 0x14F0, 0, 0, 600, cat));
            Register(new StoreEntry(typeof(LargeGlowingLadyBug), 1026641, 1156660, 0x2CFD, 0, 0, 200, cat));
            Register(new StoreEntry(typeof(FreshGreenLadyBug), 1071401, 1156661, 0x2D01, 0, 0, 200, cat));
            Register(new StoreEntry(typeof(WillowTreeDeed), 1071105, 1156658, 0x224A, 0, 0, 200, cat));

            Register(new StoreEntry(typeof(FallenLogDeed), 1071088, 1156649, 0, 0x9C88, 0, 200, cat));
            Register(new StoreEntry(typeof(LampPost2), 1071089, 1156650, 0xB22, 0, 0, 200, cat, ConstructLampPost));
            Register(new StoreEntry(typeof(HitchingPost), 1071090, 1156651, 0x14E7, 0, 0, 200, cat));
            Register(new StoreEntry(typeof(AncestralGravestone), 1071096, 1156653, 0x1174, 0, 0, 200, cat));
            Register(new StoreEntry(typeof(WoodenBookcase), 1071102, 1156655, 0x0A9D, 0, 0, 200, cat));
            Register(new StoreEntry(typeof(SnowTreeDeed), 1071103, 1156656, 0, 0x9C8A, 0, 200, cat));
            Register(new StoreEntry(typeof(MapleTreeDeed), 1071104, 1156657, 0, 0x9C87, 0, 200, cat));

            // mounts
            cat = StoreCategory.Mounts;
            Register(new StoreEntry(typeof(WindrunnerStatue), 1124685, 1157373, 0x9ED5, 0, 0, 1000, cat));
            Register(new StoreEntry(typeof(LasherStatue), 1157214, 1157305, 0x9E35, 0, 0, 1000, cat));
            Register(new StoreEntry(typeof(ChargerOfTheFallen), 1075187, 1156646, 0x2D9C, 0, 0, 1000, cat));

            // misc
            cat = StoreCategory.Misc;
            Register(new StoreEntry(typeof(SoulstoneToken), 1158404, 1158405, 0x2A93, 0, 2598, 1000, cat, ConstructSoulstone));
            Register(new StoreEntry(typeof(BagOfBulkOrderCovers), 1071116, 1157603, 0, 0x9CC6, 0, 200, cat, ConstructBOBCoverOne));

            //TODO: UndeadWeddingBundle, TotemOfChromaticFortune, 

            Register(new StoreEntry(typeof(PetBrandingIron), 1157314, 1157372, 0, 0x9CC3, 0, 600, cat));
            Register(new StoreEntry(typeof(PetBondingPotion), 1152921, 1156678, 0, 0x9CBC, 0, 500, cat)); 

            Register(new StoreEntry(typeof(ForgedMetalOfArtifacts), new TextDefinition[] { 1149868, 1156686 }, 1156674, 0, 0x9C65, 0, 1000, cat, ConstructForgedMetal));
            Register(new StoreEntry(typeof(ForgedMetalOfArtifacts), new TextDefinition[] { 1149868, 1156687 }, 1156675, 0, 0x9C65, 0, 600, cat, ConstructForgedMetal));
            Register(new StoreEntry(typeof(PenOfWisdom), 1115358, 1156669, 0, 0x9C62, 0, 600, cat));

            Register(new StoreEntry(typeof(BritannianShipDeed), 1150100, 1156673, 0, 0x9C6A, 0, 1200, cat));

            Register(new StoreEntry(typeof(SoulstoneToken), 1078835, 1158405, 0x2ADC, 0, 0, 1000, cat, ConstructSoulstone));
            Register(new StoreEntry(typeof(SoulstoneToken), 1078834, 1158405, 0x2A93, 0, 0, 1000, cat, ConstructSoulstone));

            Register(new StoreEntry(typeof(MerchantsTrinket), new TextDefinition[] { 1156827, 1156681 }, 1156666, 0, 0x9C67, 0, 300, cat, ConstructMerchantsTrinket));
            Register(new StoreEntry(typeof(MerchantsTrinket), new TextDefinition[] { 1156828, 1156682 }, 1156667, 0, 0x9C67, 0, 500, cat, ConstructMerchantsTrinket));

            Register(new StoreEntry(typeof(ArmorEngravingToolToken), 1080547, 1156652, 0, 0x9C65, 0, 200, cat));
            Register(new StoreEntry(typeof(BagOfBulkOrderCovers), 1071116, 1156654, 0, 0x9CC6, 0, 200, cat, ConstructBOBCoverTwo));
        }

        public static void Register(StoreEntry entry)
        {
            Entries.Add(entry);
        }

        #region Constructors
        public static Item ConstructHairDye(Mobile m, StoreEntry entry)
        {
            var info = NaturalHairDye.Table.FirstOrDefault(x => x.Localization == entry.Name[1].Number);

            if(info != null)
            {
                return new NaturalHairDye(info.Type);
            }

            return null;
        }

        public static Item ConstructHaochisPigment(Mobile m, StoreEntry entry)
        {
            var info = HaochisPigment.Table.FirstOrDefault(x => x.Localization == entry.Name[1].Number);

            if (info != null)
            {
                return new HaochisPigment(info.Type, 50);
            }

            return null;
        }

        public static Item ConstructPigments(Mobile m, StoreEntry entry)
        {
            PigmentType type = PigmentType.None;

            for (int i = 0; i < PigmentsOfTokuno.Table.Length; i++)
            {
                if (PigmentsOfTokuno.Table[i][1] == entry.Name[1].Number)
                {
                    type = (PigmentType)i;
                    break;
                }
            }

            if (type != PigmentType.None)
            {
                return new PigmentsOfTokuno(type, 50);
            }

            return null;
        }

        public static Item ConstructEarrings(Mobile m, StoreEntry entry)
        {
            AosElementAttribute ele = AosElementAttribute.Physical;

            switch (entry.Name[0].Number)
            {
                case 1071092: ele = AosElementAttribute.Fire; break;
                case 1071093: ele = AosElementAttribute.Cold; break;
                case 1071094: ele = AosElementAttribute.Poison; break;
                case 1071095: ele = AosElementAttribute.Energy; break;
            }

            return new EarringsOfProtection(ele);
        }

        public static Item ConstructRobe(Mobile m, StoreEntry entry)
        {
            return new HoodedBritanniaRobe(entry.ItemID);
        }

        public static Item ConstructMiniHouseDeed(Mobile m, StoreEntry entry)
        {
            int label = entry.Name[1].Number;

            switch (label)
            {
                default:
                    for (int i = 0; i < MiniHouseInfo.Info.Length; i++)
                    {
                        if (MiniHouseInfo.Info[i].LabelNumber == entry.Name[1].Number)
                        {
                            var type = (MiniHouseType)i;

                            return new MiniHouseDeed(type);
                        }
                    }
                    return null;
                case 1157015: return new MiniHouseDeed(MiniHouseType.TwoStoryWoodAndPlaster);
                case 1157014: return new MiniHouseDeed(MiniHouseType.TwoStoryStoneAndPlaster);
            }
        }

        public static Item ConstructRaisedGarden(Mobile m, StoreEntry entry)
        {
            var bag = new Bag();

            bag.DropItem(new RaisedGardenDeed());
            bag.DropItem(new RaisedGardenDeed());
            bag.DropItem(new RaisedGardenDeed());

            return bag;
        }

        public static Item ConstructLampPost(Mobile m, StoreEntry entry)
        {
            var item = new LampPost2();

            item.Movable = true;
            item.LootType = LootType.Blessed;

            return item;
        }

        public static Item ConstructForgedMetal(Mobile m, StoreEntry entry)
        {
            switch (entry.Name[1].Number)
            {
                default:
                case 1156686: return new ForgedMetalOfArtifacts(10);
                case 1156687: return new ForgedMetalOfArtifacts(5);
            }
        }

        public static Item ConstructSoulstone(Mobile m, StoreEntry entry)
        {
            switch (entry.Name[0].Number)
            {
                case 1078835: return new SoulstoneToken(SoulstoneType.Blue);
                case 1078834: return new SoulstoneToken(SoulstoneType.Green);
                case 1158404: return new SoulstoneToken(SoulstoneType.Violet);
            }

            return null;
        }

        public static Item ConstructMerchantsTrinket(Mobile m, StoreEntry entry)
        {
            switch(entry.Name[0].Number)
            {
                case 1156827: return new MerchantsTrinket(false);
                case 1156828: return new MerchantsTrinket(true);
            }

            return null;
        }

        public static Item ConstructBOBCoverOne(Mobile m, StoreEntry entry)
        {
            return new BagOfBulkOrderCovers(12, 25);
        }

        public static Item ConstructBOBCoverTwo(Mobile m, StoreEntry entry)
        {
            return new BagOfBulkOrderCovers(1, 11);
        }
        #endregion

        public static void AddPendingItem(Mobile m, Item item)
        {
            if (PendingItems == null)
                PendingItems = new Dictionary<Mobile, List<Item>>();

            if (!PendingItems.ContainsKey(m))
                PendingItems[m] = new List<Item>();

            PendingItems[m].Add(item);
            UltimaStoreContainer.DropItem(item);
        }

        public static bool HasPendingItem(PlayerMobile pm)
        {
            return PendingItems != null && PendingItems.ContainsKey(pm);
        }

        public static void CheckPendingItem(Mobile m)
        {
            if (PendingItems == null)
                return;

            if (PendingItems.ContainsKey(m))
            {
                var list = new List<Item>(PendingItems[m]);

                foreach (Item item in list)
                {
                    if (item != null)
                    {
                        if (m.Backpack != null && m.Alive && m.Backpack.TryDropItem(m, item, false))
                        {
                            if (item is IPromotionalToken && ((IPromotionalToken)item).ItemName != null)
                            {
                                m.SendLocalizedMessage(1075248, ((IPromotionalToken)item).ItemName.ToString()); // A token has been placed in your backpack. Double-click it to redeem your ~1_PROMO~.
                            }
                            else if (item.LabelNumber > 0 || item.Name != null)
                            {
                                m.SendLocalizedMessage(1156844, item.LabelNumber > 0 ? String.Format("#{0}", item.LabelNumber.ToString()) : item.Name); 
                                // Your purchase of ~1_ITEM~ has been placed in your backpack.
                            }
                            else
                            {
                                m.SendLocalizedMessage(1156843); // Your purchased item has been placed in your backpack.
                            }

                            PendingItems[m].Remove(item);
                        }
                    }
                }

                ColUtility.Free(list);

                if (PendingItems[m].Count == 0)
                {
                    PendingItems.Remove(m);
                }
            }
        }

        public static bool IsInList(StoreEntry entry, List<StoreEntry> list)
        {
            return list.Any(e =>
                    entry.ItemType == e.ItemType &&
                    entry.Name[0].ToString() == e.Name[0].ToString() &&
                    ((entry.Name.Length == 1 && e.Name.Length == 1 && entry.Name[0].ToString() == e.Name[0].ToString()) || 
                    (entry.Name.Length == 2 && e.Name.Length == 2 && entry.Name[1].ToString() == e.Name[1].ToString())));

        }

        public static List<StoreEntry> GetSortedList(string searchString)
        {
            var list = new List<StoreEntry>();

            foreach (var entry in Entries.Where(e => GetStringName(e.Name).ToLower().IndexOf(searchString.ToLower()) >= 0))
            {
                if (!IsInList(entry, list))
                {
                    list.Add(entry);
                }
            }

            return list;
        }

        public static string GetStringName(TextDefinition[] text)
        {
            string str = string.Empty;

            foreach (var td in text)
            {
                if (td.Number > 0 && VendorSearch.StringList != null)
                {
                    str += String.Format("{0} ", VendorSearch.StringList.GetString(td.Number));
                }
                else if (!string.IsNullOrEmpty(td.String))
                {
                    str += String.Format("{0} ", td.String);
                }
            }

            return str;
        }

        public static string GetStringName(TextDefinition text)
        {
            string str = text.String;

            if (text.Number > 0 && VendorSearch.StringList != null)
            {
                str = VendorSearch.StringList.GetString(text.Number);
            }

            return str != null ? str : String.Empty;
        }

        public static List<StoreEntry> GetList(StoreCategory cat)
        {
            return Entries.Where(e => e.Category == cat).ToList();
        }

        public static void SortList(List<StoreEntry> list, SortBy sort)
        {
            switch (sort)
            {
                case SortBy.Name: 
                    {
                        list.Sort((a, b) => GetStringName(a.Name).CompareTo(GetStringName(b.Name)));
                    }
                    break;
                case SortBy.PriceLower:
                    {
                        list.Sort((a, b) => a.Price.CompareTo(b.Price));
                    }
                    break;
                case SortBy.PriceHigher:
                    {
                        list.Sort((a, b) => b.Price.CompareTo(a.Price));
                    }
                    break;
                case SortBy.Newest:
                    break;
                case SortBy.Oldest:
                    {
                        list.Reverse();
                    }
                    break;
            }
        }

        public static int CartCount(Mobile m)
        {
            var profile = GetProfile(m, false);

            if (profile != null && profile.Cart != null)
            {
                return profile.Cart.Count;
            }

            return 0;
        }

        public static int GetSubTotal(Dictionary<StoreEntry, int> cart)
        {
            if (cart == null || cart.Count == 0)
                return 0;

            double sub = 0;

            foreach (var kvp in cart)
            {
                sub += kvp.Key.Cost * (double)kvp.Value;
            }

            return (int)sub;
        }

        public static int GetCurrency(Mobile m, bool sendMessage = false)
        {
            switch (Configuration.CurrencyType)
            {
                case CurrencyType.None:
                    return 0;
                case CurrencyType.Sovereigns:
                    if (m is PlayerMobile)
                    {
                        return ((PlayerMobile)m).AccountSovereigns;
                    }
                    return 0;
                case CurrencyType.Gold:
                    return Banker.GetBalance(m);
                case CurrencyType.PointsSystem:
                    var sys = PointsSystem.GetSystemInstance(Configuration.PointsSystemCurrency);

                    if (sys != null)
                    {
                        return (int)sys.GetPoints(m);
                    }

                    return 0;
                case CurrencyType.Custom:
                    return (int)Configuration.GetCustomCurrency(m);
            }

            return 0;
        }

        public static void TryPurchase(Mobile m)
        {
            var cart = GetCart(m);
            int total = GetSubTotal(cart);
            
            if (cart == null || cart.Count == 0 || total == 0)
            {
                m.SendLocalizedMessage(1156842); // Purchase failed due to your cart being empty.
            }
            else if (total > GetCurrency(m, true))
            {
                if (m is PlayerMobile)
                    BaseGump.SendGump(new NoFundsGump((PlayerMobile)m));
            }
            else
            {
                int subtotal = 0;
                bool fail = false;

                var remove = new List<StoreEntry>();

                foreach (var entry in cart)
                {
                    for (int i = 0; i < entry.Value; i++)
                    {
                        if (!entry.Key.Construct(m))
                        {
                            fail = true;

                            try
                            {
                                using (StreamWriter op = new StreamWriter("UltimaStoreError.log", true))
                                {
                                    op.WriteLine("Bad Constructor: {0}", entry.Key.ItemType.Name);
                                    Utility.WriteConsoleColor(ConsoleColor.Red, String.Format("[Ultima Store]: Bad Constructor: {0}", entry.Key.ItemType.Name));
                                }
                            }
                            catch
                            { }
                        }
                        else
                        {
                            remove.Add(entry.Key);
                            subtotal += (int)entry.Key.Cost;
                        }
                    }
                }

                if (subtotal > 0)
                {
                    DeductCurrency(m, subtotal);
                }

                var profile = GetProfile(m);

                foreach (var entry in remove)
                {
                    profile.RemoveFromCart(entry);
                }

                if (fail)
                    m.SendLocalizedMessage(1156853); // Failed to process one of your items. Please check your cart and try again.
            }
        }

        /// <summary>
        /// Should have already passed GetCurrency
        /// </summary>
        /// <param name="m"></param>
        /// <param name="amount"></param>
        public static void DeductCurrency(Mobile m, int amount)
        {
            switch (Configuration.CurrencyType)
            {
                case CurrencyType.None:
                    break;
                case CurrencyType.Sovereigns:
                    if (m is PlayerMobile)
                    {
                        ((PlayerMobile)m).WithdrawSovereigns(amount);
                    }
                    break;
                case CurrencyType.Gold:
                    Banker.Withdraw(m, amount, true);
                    break;
                case CurrencyType.PointsSystem:
                    var sys = PointsSystem.GetSystemInstance(Configuration.PointsSystemCurrency);
                    if (sys != null)
                    {
                        sys.DeductPoints(m, amount, true);
                    }
                    break;
                case CurrencyType.Custom:
                    Configuration.DeductCustomCurrecy(m, amount);
                    break;
            }
        }

        #region Player Persistence
        public static List<PlayerProfile> PlayerProfiles { get; private set; }

        public static PlayerProfile GetProfile(Mobile m, bool create = true)
        {
            var profile = PlayerProfiles.FirstOrDefault(p => p.Player == m);

            if (profile == null && create)
            {
                profile = new PlayerProfile(m);
                PlayerProfiles.Add(profile);
            }

            return profile;
        }

        public static Dictionary<StoreEntry, int> GetCart(Mobile m)
        {
            var profile = GetProfile(m, false);

            if (profile != null && profile.Cart != null)
            {
                return profile.Cart;
            }

            return null;
        }

        public static string FilePath = Path.Combine("Saves/Misc", "UltimaStore.bin");

        public static void Configure()
        {
            if (Core.TOL)
            {
                PlayerProfiles = new List<PlayerProfile>();

                EventSink.WorldSave += OnSave;
                EventSink.WorldLoad += OnLoad;
            }
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    writer.Write(_UltimaStoreContainer);

                    writer.Write(PendingItems == null ? 0 : PendingItems.Count);

                    if (PendingItems != null)
                    {
                        foreach (var kvp in PendingItems)
                        {
                            writer.Write(kvp.Key);
                            writer.WriteItemList(kvp.Value, true);
                        }
                    }

                    writer.Write(PlayerProfiles.Count);

                    foreach (var pe in PlayerProfiles)
                    {
                        pe.Serialize(writer);
                    }
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    _UltimaStoreContainer = reader.ReadItem() as UltimaStoreContainer;

                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        Mobile m = reader.ReadMobile();
                        List<Item> list = reader.ReadStrongItemList<Item>();

                        if (m != null && list != null && list.Count > 0)
                        {
                            if (PendingItems == null)
                                PendingItems = new Dictionary<Mobile, List<Item>>();

                            PendingItems[m] = list;
                        }
                    }

                    count = reader.ReadInt();

                    for (int i = 0; i < count; i++)
                    {
                        var pe = new PlayerProfile(reader);

                        if (pe.Player != null)
                        {
                            PlayerProfiles.Add(pe);
                        }
                    }
                });
        }
        #endregion
    }

    public class UltimaStoreContainer : Container
    {
        private List<Item> _DisplayItems;

        public override string DefaultName
        {
            get { return "Ultima Store Container: DO NOT DELETE!"; }
        }

        public UltimaStoreContainer()
            : base(0x09AB)
        {
            Movable = false;
            Visible = false;
        }

        public void AddDisplayItem(Item item)
        {
            if (item == null)
                return;

            if (_DisplayItems == null)
                _DisplayItems = new List<Item>();

            _DisplayItems.Add(item);

            DropItem(item);
        }

        public Item FindDisplayItem(Type t)
        {
            Item item = GetDisplayItem(t);

            if (item == null)
            {
                item = Loot.Construct(t);

                if (item != null)
                {
                    AddDisplayItem(item);
                }
            }

            return item;
        }

        public Item GetDisplayItem(Type t)
        {
            return _DisplayItems != null ? _DisplayItems.FirstOrDefault(x => x.GetType() == t) : null;
        }

        public override void Delete()
        {
        }

        public UltimaStoreContainer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_DisplayItems == null ? 0 : _DisplayItems.Count);

            if (_DisplayItems != null)
            {
                foreach (var item in _DisplayItems)
                {
                    writer.Write(item);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Item item = reader.ReadItem();

                if (item != null)
                {
                    AddDisplayItem(item);
                }
            }
        }
    }
}
