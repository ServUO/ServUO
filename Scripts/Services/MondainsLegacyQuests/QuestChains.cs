#region Header
// **********
// ServUO - QuestChains.cs
// **********
#endregion

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
		PercolemTheHunter = 16,
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
        RightingWrong = 28
	}

	public class BaseChain
	{
		public Type CurrentQuest { get; set; }
		public Type Quester { get; set; }

		public BaseChain(Type currentQuest, Type quester)
		{
			CurrentQuest = currentQuest;
			Quester = quester;
		}
	}
}