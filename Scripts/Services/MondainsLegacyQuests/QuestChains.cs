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
        public static Type[][] Chains { get; }

        static BaseChain()
        {
            Chains = new Type[30][];

            Chains[(int)QuestChain.None] = new Type[] { };

            Chains[(int)QuestChain.Aemaeth] = new[] { typeof(AemaethOneQuest), typeof(AemaethTwoQuest) };
            Chains[(int)QuestChain.AncientWorld] = new[] { typeof(TheAncientWorldQuest), typeof(TheGoldenHornQuest), typeof(BullishQuest), typeof(LostCivilizationQuest) };
            Chains[(int)QuestChain.BlightedGrove] = new[] { typeof(VilePoisonQuest), typeof(RockAndHardPlaceQuest), typeof(SympatheticMagicQuest), typeof(AlreadyDeadQuest), typeof(EurekaQuest), typeof(SubContractingQuest) };
            Chains[(int)QuestChain.CovetousGhost] = new[] { typeof(GhostOfCovetousQuest), typeof(SaveHisDadQuest), typeof(FathersGratitudeQuest) };
            Chains[(int)QuestChain.GemkeeperWarriors] = new[] { typeof(WarriorsOfTheGemkeeperQuest), typeof(CloseEnoughQuest), typeof(TakingTheBullByTheHornsQuest), typeof(EmissaryToTheMinotaurQuest) };
            Chains[(int)QuestChain.HonestBeggar] = new[] { typeof(HonestBeggarQuest), typeof(ReginasThanksQuest) };
            Chains[(int)QuestChain.LibraryFriends] = new[] { typeof(FriendsOfTheLibraryQuest), typeof(BureaucraticDelayQuest), typeof(TheSecretIngredientQuest), typeof(SpecialDeliveryQuest), typeof(AccessToTheStacksQuest) };
            Chains[(int)QuestChain.Marauders] = new[] { typeof(MaraudersQuest), typeof(TheBrainsOfTheOperationQuest), typeof(TheBrawnQuest), typeof(TheBiggerTheyAreQuest) };
            Chains[(int)QuestChain.MiniBoss] = new[] { typeof(MougGuurMustDieQuest), typeof(LeaderOfThePackQuest), typeof(SayonaraSzavetraQuest) };
            Chains[(int)QuestChain.SummonFey] = new[] { typeof(FirendOfTheFeyQuest), typeof(TokenOfFriendshipQuest), typeof(AllianceQuest) };
            Chains[(int)QuestChain.SummonFiend] = new[] { typeof(FiendishFriendsQuest), typeof(CrackingTheWhipQuest), typeof(IronWillQuest) };
            Chains[(int)QuestChain.TuitionReimbursement] = new[] { typeof(MistakenIdentityQuest), typeof(YouScratchMyBackQuest), typeof(FoolingAernyaQuest), typeof(NotQuiteThatEasyQuest), typeof(ConvinceMeQuest), typeof(TuitionReimbursementQuest) };
            Chains[(int)QuestChain.Spellweaving] = new[] { typeof(PatienceQuest), typeof(NeedsOfManyHeartwoodQuest), typeof(NeedsOfManyPartHeartwoodQuest), typeof(MakingContributionHeartwoodQuest), typeof(UnnaturalCreationsQuest) };
            Chains[(int)QuestChain.SpellweavingS] = new[] { typeof(DisciplineQuest), typeof(NeedsOfTheManySanctuaryQuest), typeof(MakingContributionSanctuaryQuest), typeof(SuppliesForSanctuaryQuest), typeof(TheHumanBlightQuest) };
            Chains[(int)QuestChain.UnfadingMemories] = new[] { typeof(UnfadingMemoriesOneQuest), typeof(UnfadingMemoriesTwoQuest), typeof(UnfadingMemoriesThreeQuest) };
            Chains[(int)QuestChain.Empty] = new Type[] { };
            Chains[(int)QuestChain.KingVernixQuests] = new Type[] { };
            Chains[(int)QuestChain.DoughtyWarriors] = new[] { typeof(DoughtyWarriorsQuest), typeof(DoughtyWarriors2Quest), typeof(DoughtyWarriors3Quest) };
            Chains[(int)QuestChain.HonorOfDeBoors] = new[] { typeof(HonorOfDeBoorsQuest), typeof(JackTheVillainQuest), typeof(SavedHonorQuest) };
            Chains[(int)QuestChain.LaifemTheWeaver] = new[] { typeof(ShearingKnowledgeQuest), typeof(WeavingFriendshipsQuest), typeof(NewSpinQuest)};
            Chains[(int)QuestChain.CloakOfHumility] = new[] { typeof(TheQuestionsQuest), typeof(CommunityServiceMuseumQuest), typeof(CommunityServiceZooQuest), typeof(CommunityServiceLibraryQuest), typeof(WhosMostHumbleQuest) };
            Chains[(int)QuestChain.ValleyOfOne] = new[] { typeof(TimeIsOfTheEssenceQuest), typeof(UnitingTheTribesQuest) };
            Chains[(int)QuestChain.MyrmidexAlliance] = new[] { typeof(TheZealotryOfZipactriotlQuest), typeof(DestructionOfZipactriotlQuest) };
            Chains[(int)QuestChain.EodonianAlliance] = new[] { typeof(ExterminatingTheInfestationQuest), typeof(InsecticideAndRegicideQuest) };
            Chains[(int)QuestChain.FlintTheQuartermaster] = new[] { typeof(ThievesBeAfootQuest), typeof(BibliophileQuest) };
            Chains[(int)QuestChain.AnimalTraining] = new[] { typeof(TamingPetQuest), typeof(UsingAnimalLoreQuest), typeof(LeadingIntoBattleQuest), typeof(TeachingSomethingNewQuest) };
            Chains[(int)QuestChain.PaladinsOfTrinsic] = new[] { typeof(PaladinsOfTrinsic), typeof(PaladinsOfTrinsic2) };
            Chains[(int)QuestChain.RightingWrong] = new[] { typeof(RightingWrongQuest2), typeof(RightingWrongQuest3), typeof(RightingWrongQuest4) };
            Chains[(int)QuestChain.Ritual] = new[] { typeof(RitualQuest.ScalesOfADreamSerpentQuest), typeof(RitualQuest.TearsOfASoulbinderQuest), typeof(RitualQuest.PristineCrystalLotusQuest) };
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
