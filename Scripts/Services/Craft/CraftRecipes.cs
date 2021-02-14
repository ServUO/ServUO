namespace Server.Engines.Craft
{
    public enum CraftRecipes
    {
        // DefCarpentry
        WarriorStatueSouth = 100,
        WarriorStatueEast = 101,
        SquirrelStatueSouth = 102,
        SquirrelStatueEast = 103,
        AcidProofRope = 104,
        OrnateElvenChair = 105,
        ArcaneBookshelfSouth = 106,
        ArcaneBookshelfEast = 107,
        OrnateElvenChestSouth = 108,
        ElvenDresserSouth = 109,
        ElvenDresserEast = 110,
        FancyElvenArmoire = 111,
        ArcanistsWildStaff = 112,
        AncientWildStaff = 113,
        ThornedWildStaff = 114,
        HardenedWildStaff = 115,
        TallElvenBedSouth = 116,
        TallElvenBedEast = 117,
        StoneAnvilSouth = 118,
        StoneAnvilEast = 119,
        OrnateElvenChestEast = 120,
        PhantomStaff = 150,
        IronwoodCrown = 151,
        BrambleCoat = 152,
        SmallElegantAquarium = 153,
        WallMountedAquarium = 154,
        LargeElegantAquarium = 155,
        KotlBlackRod = 170,
        KotlAutomaton = 171,

        // DefBowFletching
        BarbedLongbow = 200,
        SlayerLongbow = 201,
        FrozenLongbow = 202,
        LongbowOfMight = 203,
        RangersShortbow = 204,
        LightweightShortbow = 205,
        MysticalShortbow = 206,
        AssassinsShortbow = 207,
        BlightGrippedLongbow = 250,
        FaerieFire = 251,
        SilvanisFeywoodBow = 252,
        MischiefMaker = 253,
        TheNightReaper = 254,

        // DefBlacksmithy
        TrueSpellblade = 300,
        IcySpellblade = 301,
        FierySpellblade = 302,
        SpellbladeOfDefense = 303,
        TrueAssassinSpike = 304,
        ChargedAssassinSpike = 305,
        MagekillerAssassinSpike = 306,
        WoundingAssassinSpike = 307,
        TrueLeafblade = 308,
        Luckblade = 309,
        MagekillerLeafblade = 310,
        LeafbladeOfEase = 311,
        KnightsWarCleaver = 312,
        ButchersWarCleaver = 313,
        SerratedWarCleaver = 314,
        TrueWarCleaver = 315,
        AdventurersMachete = 316,
        OrcishMachete = 317,
        MacheteOfDefense = 318,
        DiseasedMachete = 319,
        Runesabre = 320,
        MagesRuneBlade = 321,
        RuneBladeOfKnowledge = 322,
        CorruptedRuneBlade = 323,
        TrueRadiantScimitar = 324,
        DarkglowScimitar = 325,
        IcyScimitar = 326,
        TwinklingScimitar = 327,
        GuardianAxe = 328,
        SingingAxe = 329,
        ThunderingAxe = 330,
        HeavyOrnateAxe = 331,
        RubyMace = 332, 
        EmeraldMace = 333, 
        SapphireMace = 334, 
        SilverEtchedMace = 335, 
        BoneMachete = 336,
        RuneCarvingKnife = 350,
        ColdForgedBlade = 351,
        OverseerSunderedBlade = 352,
        LuminousRuneBlade = 353,
        ShardTrasher = 354, 
        BritchesOfWarding = 355,
        GlovesOfFeudalGrip = 356,

        // DefTinkering
        InvisibilityPotion = 400,
        DarkglowPotion = 401,
        ParasiticPotion = 402,
        EssenceOfBattle = 450,
        PendantOfTheMagi = 451,
        ResilientBracer = 452,
        ScrappersCompendium = 453,
        HoveringWisp = 454, // Removed at OSI Publish 103
        KotlPowerCore = 455,
        BraceletOfPrimalConsumption = 456,
        DrSpectorLenses = 457,
        KotlAutomatonHead = 458,
        WeatheredBronzeArcherSculpture = 459,
        WeatheredBronzeFairySculpture = 460,
        WeatheredBronzeGlobeSculpture = 461,
        WeatheredBronzeManOnABench = 462,
        KrampusMinionEarrings = 463,
        EnchantedPicnicBasket = 464,
        Telescope = 465,
        BarbedWhip = 466,
        SpikedWhip = 467,
        BladedWhip = 468,
       
        RotWormStew = 500, // DefCooking
        ElvenQuiver = 501, // DefTailoring
        QuiverOfFire = 502, // DefTailoring
        QuiverOfIce = 503, // DefTailoring
        QuiverOfBlight = 504, // DefTailoring
        QuiverOfLightning = 505, // DefTailoring
        SongWovenMantle = 550, // DefTailoring
        SpellWovenBritches = 551, // DefTailoring
        StitchersMittens = 552, // DefTailoring
        JesterShoes = 560, // DefTailoring
        ChefsToque = 561, // DefTailoring
        GuildedKilt = 562, // DefTailoring
        CheckeredKilt = 563, // DefTailoring
        FancyKilt = 564, // DefTailoring
        FloweredDress = 565, // DefTailoring
        EveningGown = 566, // DefTailoring
        TigerPeltChest = 570, // DefTailoring
        TigerPeltCollar = 571, // DefTailoring
        TigerPeltHelm = 572, // DefTailoring
        TigerPeltLegs = 573, // DefTailoring
        TigerPeltShorts = 574, // DefTailoring
        TigerPeltBustier = 575, // DefTailoring
        TigerPeltLongSkirt = 576, // DefTailoring
        TigerPeltSkirt = 577, // DefTailoring
        DragonTurtleHideArms = 580, // DefTailoring
        DragonTurtleHideChest = 581, // DefTailoring
        DragonTurtleHideHelm = 582, // DefTailoring
        DragonTurtleHideLegs = 583, // DefTailoring
        DragonTurtleHideBustier = 584, // DefTailoring
        CuffsOfTheArchmage = 585, // DefTailoring
        KrampusMinionHat = 586, // DefTailoring
        KrampusMinionBoots = 587, // DefTailoring
        KrampusMinionTalons = 588, // DefTailoring
        GingerbreadCookie = 599, // DefCooking
        DarkChocolateNutcracker = 600, // DefCooking
        MilkChocolateNutcracker = 601, // DefCooking
        WhiteChocolateNutcracker = 602, // DefCooking
        ThreeTieredCake = 603, // DefCooking
        BlackrockStew = 604, // DefCooking
        Hamburger = 605, // DefCooking
        HotDog = 606, // DefCooking
        Sausage = 607, // DefCooking
        GrilledSerpentSteak = 608, // DefCooking
        BBQDinoRibs = 609, // DefCooking
        WakuChicken = 610, // DefCooking

        AnniversaryVaseShort = 701, // DefMasonry
        AnniversaryVaseTall = 702, // DefMasonry

        RunicAtlas = 800, // DefInscription

        BarrabHemolymphConcentrate = 900, // DefAlchemy
        JukariBurnPoiltice = 901, // DefAlchemy
        KurakAmbushersEssence = 902, // DefAlchemy
        BarakoDraftOfMight = 903, // DefAlchemy
        UraliTranceTonic = 904, // DefAlchemy
        SakkhraProphylaxisPotion = 905, // DefAlchemy

        EodonianWallMap = 1000, // DefCartography

        MaceBelt = 1100, // DefTailoring
        SwordBelt = 1101, // DefTailoring
        DaggerBelt = 1102, // DefTailoring
        ElegantCollar = 1103, // DefTailoring
        CrimsonMaceBelt = 1104, // DefTailoring
        CrimsonSwordBelt = 1105, // DefTailoring
        CrimsonDaggerBelt = 1106, // DefTailoring
        ElegantCollarOfFortune = 1107, // DefTailoring
        AssassinsCowl = 1108, // DefTailoring
        MagesHood = 1109, // DefTailoring
        CowlOfTheMaceAndShield = 1110, // DefTailoring
        MagesHoodOfScholarlyInsight = 1111 // DefTailoring
    }
}
