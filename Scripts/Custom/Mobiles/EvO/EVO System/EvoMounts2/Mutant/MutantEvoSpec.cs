using System;
using Server;

namespace Xanthos.Evo
{
	public sealed class MutantEvoSpec : BaseEvoSpec
	{
		// This class implements a singleton pattern; meaning that no matter how many times the
		// Instance attribute is used, there will only ever be one of these created in the entire system.
		// Copy this template and give it a new name.  Assign all of the data members of the EvoSpec
		// base class in the constructor.  Your subclass must not be abstract.
		// Never call new on this class, use the Instance attribute to get the instance instead.

		MutantEvoSpec()
		{
			m_Tamable = true;
			m_MinTamingToHatch = 99.9;
			m_PercentFemaleChance = .04;
			m_GuardianEggOrDeedChance = .01;
			m_AlwaysHappy = false;
			m_ProducesYoung = true;
			m_PregnancyTerm = 14.00;
			m_AbsoluteStatValues = false;
			m_MaxEvoResistance = 90;
			m_MaxTrainingStage = 4;
			m_MountStage = 5;
			m_CanAttackPlayers = false;

			m_Skills = new SkillName[4] { SkillName.MagicResist, SkillName.Tactics, SkillName.Wrestling, SkillName.Anatomy };
			m_MinSkillValues = new int[4] { 50, 50, 50, 15, };
			m_MaxSkillValues = new int[4] { 100, 110, 120, 110 };


			m_Stages = new BaseEvoStage[] { new MutantStageOne(), new MutantStageTwo(), new MutantStageThree(),
											  new MutantStageFour(), new MutantStageFive(), new MutantStageSix() };
		}

		// These next 2 lines facilitate the singleton pattern.  In your subclass only change the
		// BaseEvoSpec class name to your subclass of BaseEvoSpec class name and uncomment both lines.
		public static MutantEvoSpec Instance { get { return Nested.instance; } }
		class Nested { static Nested() { } internal static readonly MutantEvoSpec instance = new MutantEvoSpec();}
	}	

	// Define a subclass of BaseEvoStage for each stage in your creature and place them in the
	// array in your subclass of BaseEvoSpec.  See the example classes for how to do this.
	// Your subclass must not be abstract.

	public class MutantStageOne : BaseEvoStage
	{
		public MutantStageOne()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 50000; EpMinDivisor = 10; EpMaxDivisor = 5; DustMultiplier = 20;
			BaseSoundID = 0x4FD;
			Hue = 1175;
			BodyValue = 201; ControlSlots = 3; MinTameSkill = 99.9; VirtualArmor = 30;

			DamagesTypes = new ResistanceType[1] { ResistanceType.Physical };
			MinDamages = new int[1] { 100 };
			MaxDamages = new int[1] { 100 };

			ResistanceTypes = new ResistanceType[1] { ResistanceType.Physical };
			MinResistances = new int[1] { 15 };
			MaxResistances = new int[1] { 15 };

			DamageMin = 11; DamageMax = 15; HitsMin = 5; HitsMax = 10;
			StrMin = 75; StrMax = 85; DexMin = 95; DexMax = 105; IntMin = 80; IntMax = 100;
		}
	}

	public class MutantStageTwo : BaseEvoStage
	{
		public MutantStageTwo()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 150000; EpMinDivisor = 20; EpMaxDivisor = 10; DustMultiplier = 20;
			BaseSoundID = 0x4FD;
			BodyValue = 217; VirtualArmor = 40;
			Hue = 1175;
		
			DamagesTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
													ResistanceType.Poison, ResistanceType.Energy };
			MinDamages = new int[5] { 20, 20, 20, 20, 20 };
			MaxDamages = new int[5] { 20, 20, 20, 20, 20 };

			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 25, 25, 25, 25, 25 };
			MaxResistances = new int[5] { 25, 25, 25, 25, 25 };

			DamageMin = 2; DamageMax = 2; HitsMin= 10; HitsMax =15;
			StrMin = 65; StrMax = 75; DexMin = 40; DexMax = 45; IntMin = 40; IntMax = 50;
		}
	}

	public class MutantStageThree : BaseEvoStage
	{
		public MutantStageThree()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 20000000; EpMinDivisor = 30; EpMaxDivisor = 20; DustMultiplier = 20;
			BaseSoundID = 0x5A;
			Hue = 1175;
			BodyValue = 214; VirtualArmor = 50;
		
			DamagesTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
													 ResistanceType.Poison, ResistanceType.Energy };
			MinDamages = new int[5] { 100, 20, 20, 20, 20 };
			MaxDamages = new int[5] { 100, 20, 20, 20, 20 };

			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 35, 35, 35, 35, 35 };
			MaxResistances = new int[5] { 35, 35, 35, 35, 35 };

			DamageMin = 5; DamageMax = 5; HitsMin= 15; HitsMax = 20;
			StrMin = 25; StrMax = 35; DexMin = 40; DexMax = 45; IntMin = 35; IntMax = 40;
		}
	}

	public class MutantStageFour : BaseEvoStage
	{
		public MutantStageFour()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 40000000; EpMinDivisor = 50; EpMaxDivisor = 40; DustMultiplier = 20;
			BaseSoundID = 0x4FD;
			Hue = 1175;
			BodyValue = 204; ControlSlots = 2; MinTameSkill = 99.0; VirtualArmor = 85;
		
			DamagesTypes = null;
			MinDamages = null;
			MaxDamages = null;

			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 40, 40, 40, 40, 40 };
			MaxResistances = new int[5] { 40, 40, 40, 40, 40 };	

			DamageMin = 5; DamageMax = 5; HitsMin= 20; HitsMax = 20;
			StrMin = 35; StrMax = 45; DexMin = 30; DexMax = 40; IntMin = 90; IntMax = 100;
		}
	}
	
	public class MutantStageFive : BaseEvoStage
	{
		public MutantStageFive()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 80000000; EpMinDivisor = 60; EpMaxDivisor = 50; DustMultiplier = 20;
			BaseSoundID = 362;
			Hue = 1175;
			//ItemID = 0x3EBB;
			BodyValue = 0x319; ControlSlots = 2; MinTameSkill = 99.0; VirtualArmor = 140;
		
			DamagesTypes = null;
			MinDamages = null;
			MaxDamages = null;

			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 55, 70, 25, 40, 40 };
			MaxResistances = new int[5] { 70, 80, 45, 50, 50 };	

			DamageMin = 10; DamageMax = 10; HitsMin= 150; HitsMax = 200;
			StrMin = 100; StrMax = 105; DexMin = 50; DexMax = 60; IntMin = 150; IntMax = 200;

		}
	}

	public class MutantStageSix : BaseEvoStage
	{
		public MutantStageSix()
		{
			Title = "The Mutant Steed";
			EvolutionMessage = "has evolved to its highest form and is now a Mutant Steed";
			NextEpThreshold = 0; EpMinDivisor = 160; EpMaxDivisor = 60; DustMultiplier = 20;
			BaseSoundID = 362; ControlSlots = 2;
			Hue = 1175;
			//ItemID = 0x3E92;
			BodyValue = 0x11C; VirtualArmor = 180;
		
			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 55, 70, 25, 40, 40 };
			MaxResistances = new int[5] { 70, 80, 45, 50, 50 };	

			DamageMin = 5; DamageMax = 5; HitsMin= 250; HitsMax = 325;
			StrMin = 30; StrMax = 40; DexMin = 15; DexMax = 25; IntMin = 50; IntMax = 60;

		
		}
	}
}