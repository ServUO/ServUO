#region References
using System;
#endregion

namespace Server.Engines.Quests
{
    public enum QuestChain
    {
        None = 0,

        Aemaeth = 1,
        AncientWorld = 2,
        BlightedGrove = 3,
        CovetousGhost = 4,
        GemkeeperWarriors = 5,
        HonestBeggar = 6,
        LibraryFriends = 7,
        Marauders = 8,
        MiniBoss = 9,
        SummonFey = 10,
        SummonFiend = 11,
        TuitionReimbursement = 12,
        Spellweaving = 13,
        SpellweavingS = 14,
        UnfadingMemories = 15,
        Empty = 16,
        KingVernixQuests = 17,
        DoughtyWarriors = 18,
        HonorOfDeBoors = 19,
        LaifemTheWeaver = 20,
        CloakOfHumility = 21,
        ValleyOfOne = 22,
        MyrmidexAlliance = 23,
        EodonianAlliance = 24,
        FlintTheQuartermaster = 25,
        AnimalTraining = 26,
        PaladinsOfTrinsic = 27,
        RightingWrong = 28,
        Ritual = 29
    }

    public class BaseChain
    {
        public static Type[][] Chains { get; set; }

        static BaseChain()
        {
            Chains = new Type[30][];

            Chains[(int)QuestChain.None] = new Type[] { };

            Chains[(int)QuestChain.Aemaeth] = new Type[] { typeof(AemaethOneQuest), typeof(AemaethTwoQuest) };
            Chains[(int)QuestChain.AncientWorld] = new Type[] { typeof(TheAncientWorldQuest), typeof(TheGoldenHornQuest), typeof(BullishQuest), typeof(LostCivilizationQuest) };
            Chains[(int)QuestChain.BlightedGrove] = new Type[] { typeof(VilePoisonQuest), typeof(RockAndHardPlaceQuest), typeof(SympatheticMagicQuest), typeof(AlreadyDeadQuest), typeof(EurekaQuest), typeof(SubContractingQuest) };
            Chains[(int)QuestChain.CovetousGhost] = new Type[] { typeof(GhostOfCovetousQuest), typeof(SaveHisDadQuest), typeof(FathersGratitudeQuest) };
            Chains[(int)QuestChain.GemkeeperWarriors] = new Type[] { typeof(WarriorsOfTheGemkeeperQuest), typeof(CloseEnoughQuest), typeof(TakingTheBullByTheHornsQuest), typeof(EmissaryToTheMinotaurQuest) };
            Chains[(int)QuestChain.HonestBeggar] = new Type[] { typeof(HonestBeggarQuest), typeof(ReginasThanksQuest) };
            Chains[(int)QuestChain.LibraryFriends] = new Type[] { typeof(FriendsOfTheLibraryQuest), typeof(BureaucraticDelayQuest), typeof(TheSecretIngredientQuest), typeof(SpecialDeliveryQuest), typeof(AccessToTheStacksQuest) };
            Chains[(int)QuestChain.Marauders] = new Type[] { typeof(MaraudersQuest), typeof(TheBrainsOfTheOperationQuest), typeof(TheBrawnQuest), typeof(TheBiggerTheyAreQuest) };
            Chains[(int)QuestChain.MiniBoss] = new Type[] { typeof(MougGuurMustDieQuest), typeof(LeaderOfThePackQuest), typeof(SayonaraSzavetraQuest) };
            Chains[(int)QuestChain.SummonFey] = new Type[] { typeof(FirendOfTheFeyQuest), typeof(TokenOfFriendshipQuest), typeof(AllianceQuest) };
            Chains[(int)QuestChain.SummonFiend] = new Type[] { typeof(FiendishFriendsQuest), typeof(CrackingTheWhipQuest), typeof(IronWillQuest) };
            Chains[(int)QuestChain.TuitionReimbursement] = new Type[] { typeof(MistakenIdentityQuest), typeof(YouScratchMyBackQuest), typeof(FoolingAernyaQuest), typeof(NotQuiteThatEasyQuest), typeof(ConvinceMeQuest), typeof(TuitionReimbursementQuest) };
            Chains[(int)QuestChain.Spellweaving] = new Type[] { typeof(PatienceQuest), typeof(NeedsOfManyHeartwoodQuest), typeof(NeedsOfManyPartHeartwoodQuest), typeof(MakingContributionHeartwoodQuest), typeof(UnnaturalCreationsQuest) };
            Chains[(int)QuestChain.SpellweavingS] = new Type[] { typeof(DisciplineQuest), typeof(NeedsOfTheManySanctuaryQuest), typeof(MakingContributionSanctuaryQuest), typeof(SuppliesForSanctuaryQuest), typeof(TheHumanBlightQuest) };
            Chains[(int)QuestChain.UnfadingMemories] = new Type[] { typeof(UnfadingMemoriesOneQuest), typeof(UnfadingMemoriesTwoQuest), typeof(UnfadingMemoriesThreeQuest) };
            Chains[(int)QuestChain.Empty] = new Type[] { };
            Chains[(int)QuestChain.KingVernixQuests] = new Type[] { };
            Chains[(int)QuestChain.DoughtyWarriors] = new Type[] { typeof(DoughtyWarriorsQuest), typeof(DoughtyWarriors2Quest), typeof(DoughtyWarriors3Quest) };
            Chains[(int)QuestChain.HonorOfDeBoors] = new Type[] { typeof(HonorOfDeBoorsQuest), typeof(JackTheVillainQuest), typeof(SavedHonorQuest) };
            Chains[(int)QuestChain.LaifemTheWeaver] = new Type[] { typeof(ShearingKnowledgeQuest), typeof(WeavingFriendshipsQuest), typeof(NewSpinQuest), };
            Chains[(int)QuestChain.CloakOfHumility] = new Type[] { typeof(TheQuestionsQuest), typeof(CommunityServiceMuseumQuest), typeof(CommunityServiceZooQuest), typeof(CommunityServiceLibraryQuest), typeof(WhosMostHumbleQuest) };
            Chains[(int)QuestChain.ValleyOfOne] = new Type[] { typeof(TimeIsOfTheEssenceQuest), typeof(UnitingTheTribesQuest) };
            Chains[(int)QuestChain.MyrmidexAlliance] = new Type[] { typeof(TheZealotryOfZipactriotlQuest), typeof(DestructionOfZipactriotlQuest) };
            Chains[(int)QuestChain.EodonianAlliance] = new Type[] { typeof(ExterminatingTheInfestationQuest), typeof(InsecticideAndRegicideQuest) };
            Chains[(int)QuestChain.FlintTheQuartermaster] = new Type[] { typeof(ThievesBeAfootQuest), typeof(BibliophileQuest) };
            Chains[(int)QuestChain.AnimalTraining] = new Type[] { typeof(TamingPetQuest), typeof(UsingAnimalLoreQuest), typeof(LeadingIntoBattleQuest), typeof(TeachingSomethingNewQuest) };
            Chains[(int)QuestChain.PaladinsOfTrinsic] = new Type[] { typeof(PaladinsOfTrinsic), typeof(PaladinsOfTrinsic2) };
            Chains[(int)QuestChain.RightingWrong] = new Type[] { typeof(RightingWrongQuest2), typeof(RightingWrongQuest3), typeof(RightingWrongQuest4) };
            Chains[(int)QuestChain.Ritual] = new Type[] { typeof(RitualQuest.ScalesOfADreamSerpentQuest), typeof(RitualQuest.TearsOfASoulbinderQuest), typeof(RitualQuest.PristineCrystalLotusQuest) };
        }

        public Type CurrentQuest { get; set; }
        public Type Quester { get; set; }

        public BaseChain(Type currentQuest, Type quester)
        {
            CurrentQuest = currentQuest;
            Quester = quester;
        }
    }
}
