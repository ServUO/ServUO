#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	class Flags
	{
		public const int kUnHueFlag		= -2;
		public const int kRandomHueFlag = -1;
	}

	public abstract class BaseEvoSpec
	{
		// This class implements a singleton pattern; meaning that no matter how many times the
		// Instance attribute is used, there will only ever be one of these created in the entire system.
		// Copy this template and give it a new name.  Assign all of the data members of the EvoSpec
		// base class in the constructor.  Your subclass must not be abstract.
		// Never call new on this class, use the Instance attribute to get the instance instead.

		public int MountStage { get { return m_MountStage; } }
		public int MaxTrainingStage { get { return m_MaxTrainingStage; } }
		public bool CanAttackPlayers { get { return m_CanAttackPlayers; } }
		public bool Tamable { get { return m_Tamable; } }
		public double MinTamingToHatch { get { return m_MinTamingToHatch; } }
		public double PercentFemaleChance { get{ return m_PercentFemaleChance; } }
		public double GuardianEggOrDeedChance { get { return m_GuardianEggOrDeedChance; } }
		public double PackSpecialItemChance { get { return m_PackSpecialItemChance; } }
		public bool AlwaysHappy { get { return m_AlwaysHappy; } }
		public bool ProducesYoung { get { return m_ProducesYoung; } }
		public double PregnancyTerm { get { return m_PregnancyTerm; } }
		public double HatchDuration { get { return m_HatchDuration; } }
		public bool AbsoluteStatValues { get { return m_AbsoluteStatValues; } }
		public int FameLevel { get { return m_FameLevel; } }
		public int KarmaLevel { get { return m_KarmaLevel; } }
		public SkillName [] Skills { get { return m_Skills; } }
		public int [] MinSkillValues { get { return m_MinSkillValues; } }
		public int [] MaxSkillValues { get { return m_MaxSkillValues; } }
		public int MaxEvoResistance { get { return m_MaxEvoResistance; } }
		public BaseEvoStage [] Stages { get { return m_Stages; } }
		public int[] RandomHues { get { return m_RandomHues; } }

		protected BaseEvoSpec() { }

		protected int m_MountStage;					// At what stage (zero based) can player mount (only BaseEvoMount)?
		protected int m_MaxTrainingStage;			// At what stage (zero based) can evo no longer train against training elementals?
		protected bool m_CanAttackPlayers;			// Keep things fair?
		protected bool m_Tamable;					// Is it or not?
		protected double m_MinTamingToHatch;		// Skill required - independent of requirement to tame one gone wild.
		protected double m_PercentFemaleChance;		// Chance to spawn as female - important for population control 
		protected double m_GuardianEggOrDeedChance;	// Chance to produce an egg or deed as loot - Guardians only
		protected double m_PackSpecialItemChance;	// Small chance to pack a special item in construction of the Evo
		protected bool m_AlwaysHappy;				// Keeps it wonderfully happy if true, otherwise needs food like other pets.
		protected bool m_ProducesYoung;				// Does the species produce offspring?
		protected double m_PregnancyTerm;			// Days between mating and producing an egg (1.00 = one day, 0.01 = 15 minutes).
		protected double m_HatchDuration;			// Days of egg incubation time (1.00 = one day, 0.01 = 15 minutes).
		protected bool m_AbsoluteStatValues;		// Set or add str, dex, int, hits, damage at each stage.
		protected int m_FameLevel;					// 1 - 5
		protected int m_KarmaLevel;					// 0 - 5
		protected BaseEvoStage [] m_Stages;			// The list of stages.
		protected SkillName [] m_Skills;			// List of skill names and min/max values
		protected int [] m_MinSkillValues;
		protected int [] m_MaxSkillValues;
		protected int m_MaxEvoResistance;			// The cap even with mods (i.e. armor, et.) on.
		protected int [] m_RandomHues;

		// These next 2 lines facilitate the singleton pattern.  In your subclass only change the
		// BaseEvoSpec class name to your subclass of BaseEvoSpec class name and uncomment both lines.
		// public static BaseEvoSpec Instance { get { return Nested.instance; } }
		// class Nested { static Nested() { } internal static readonly BaseEvoSpec instance = new BaseEvoSpec();}
	}

	// Define a subclass of BaseEvoStage for each stage in your creature and place them in the
	// array in your subclass of BaseEvoSpec.  See the example classes for how to do this.
	// Your subclass must not be abstract.

	public abstract class BaseEvoStage
	{
		public string Title;
		public string EvolutionMessage;

		public int NextEpThreshold;
		public int EpMinDivisor;
		public int EpMaxDivisor;
		public int DustMultiplier;

		public int BaseSoundID;
		public int BodyValue;
		public int Hue;
		public int ControlSlots;
		public double MinTameSkill;
		public int VirtualArmor;

		public ResistanceType [] DamagesTypes;
		public int [] MinDamages;
		public int [] MaxDamages;

		public ResistanceType [] ResistanceTypes;
		public int [] MinResistances;
		public int [] MaxResistances;

		// These next 10 can be relative or absolute depending on your EvoSpec.AbsoluteStatValues

		public int DamageMin;
		public int DamageMax;
		public int HitsMin;
		public int HitsMax;

		public int StrMin;
		public int StrMax;
		public int DexMin;
		public int DexMax;
		public int IntMin;
		public int IntMax;
	}
}