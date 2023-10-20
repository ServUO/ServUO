using System;
using Server;

namespace Xanthos.Evo
{
	public sealed class RidableLlamaEvoSpec : BaseEvoSpec
	{
		// This class implements a singleton pattern; meaning that no matter how many times the
		// Instance attribute is used, there will only ever be one of these created in the entire system.
		// Copy this template and give it a new name.  Assign all of the data members of the EvoSpec
		// base class in the constructor.  Your subclass must not be abstract.
		// Never call new on this class, use the Instance attribute to get the instance instead.

        RidableLlamaEvoSpec()
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
			m_MaxTrainingStage = 3;
			m_MountStage = 4;

			m_Skills = new SkillName[4] { SkillName.MagicResist, SkillName.Tactics, SkillName.Wrestling, SkillName.Anatomy };
			m_MinSkillValues = new int[4] { 50, 50, 50, 15, };
			m_MaxSkillValues = new int[4] { 100, 110, 120, 110 };


            m_Stages = new BaseEvoStage[] { new RidableLlamaStageOne(), new RidableLlamaStageTwo(), new RidableLlamaStageThree(),
											  new RidableLlamaStageFour(), new RidableLlamaStageFive() };
		}

		// These next 2 lines facilitate the singleton pattern.  In your subclass only change the
		// BaseEvoSpec class name to your subclass of BaseEvoSpec class name and uncomment both lines.
        public static RidableLlamaEvoSpec Instance { get { return Nested.instance; } }
        class Nested { static Nested() { } internal static readonly RidableLlamaEvoSpec instance = new RidableLlamaEvoSpec();}
	}	

	// Define a subclass of BaseEvoStage for each stage in your creature and place them in the
	// array in your subclass of BaseEvoSpec.  See the example classes for how to do this.
	// Your subclass must not be abstract.

	public class RidableLlamaStageOne : BaseEvoStage
	{
        public RidableLlamaStageOne()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 25000; EpMinDivisor = 10; EpMaxDivisor = 5; DustMultiplier = 20;
			BaseSoundID = 0x3F5;
			BodyValue = 220; ControlSlots = 4; MinTameSkill = 99.9; VirtualArmor = 30;

			DamagesTypes = new ResistanceType[1] { ResistanceType.Physical };
			MinDamages = new int[1] { 100 };
			MaxDamages = new int[1] { 100 };

			ResistanceTypes = new ResistanceType[1] { ResistanceType.Physical };
			MinResistances = new int[1] { 15 };
			MaxResistances = new int[1] { 15 };

			DamageMin = 11; DamageMax = 15; HitsMin = 150; HitsMax = 200;
			StrMin = 200; StrMax = 250; DexMin = 95; DexMax = 105; IntMin = 80; IntMax = 100;
		}
	}

	public class RidableLlamaStageTwo : BaseEvoStage
	{
        public RidableLlamaStageTwo()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 75000; EpMinDivisor = 20; EpMaxDivisor = 10; DustMultiplier = 20;
			BaseSoundID = 0x3F5;
			BodyValue = 220; VirtualArmor = 40;
		
			DamagesTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
													ResistanceType.Poison, ResistanceType.Energy };
			MinDamages = new int[5] { 20, 20, 20, 20, 20 };
			MaxDamages = new int[5] { 20, 20, 20, 20, 20 };

			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 25, 25, 25, 25, 25 };
			MaxResistances = new int[5] { 25, 25, 25, 25, 25 };

			DamageMin = 2; DamageMax = 2; HitsMin= 450; HitsMax = 450;
			StrMin = 150; StrMax = 150; DexMin = 40; DexMax = 40; IntMin = 50; IntMax = 50;
		}
	}

	public class RidableLlamaStageThree : BaseEvoStage
	{
        public RidableLlamaStageThree()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 1250000; EpMinDivisor = 30; EpMaxDivisor = 20; DustMultiplier = 20;
			BaseSoundID = 0x3F5;
			BodyValue = 220; VirtualArmor = 50;
		
			DamagesTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
													 ResistanceType.Poison, ResistanceType.Energy };
			MinDamages = new int[5] { 100, 20, 20, 20, 20 };
			MaxDamages = new int[5] { 100, 20, 20, 20, 20 };

			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 35, 35, 35, 35, 35 };
			MaxResistances = new int[5] { 35, 35, 35, 35, 35 };

			DamageMin = 5; DamageMax = 5; HitsMin= 100; HitsMax = 100;
			StrMin = 100; StrMax = 100; DexMin = 40; DexMax = 40; IntMin = 40; IntMax = 40;
		}
	}

	public class RidableLlamaStageFour : BaseEvoStage
	{
        public RidableLlamaStageFour()
		{
			EvolutionMessage = "has evolved";
			NextEpThreshold = 7750000; EpMinDivisor = 50; EpMaxDivisor = 40; DustMultiplier = 20;
			BaseSoundID = 0x3F5;
			BodyValue = 220; ControlSlots = 4; MinTameSkill = 115.0; VirtualArmor = 60;
		
			DamagesTypes = null;
			MinDamages = null;
			MaxDamages = null;

			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 40, 40, 40, 40, 40 };
			MaxResistances = new int[5] { 40, 40, 40, 40, 40 };	

			DamageMin = 5; DamageMax = 5; HitsMin= 100; HitsMax = 100;
			StrMin = 150; StrMax = 150; DexMin = 40; DexMax = 40; IntMin = 100; IntMax = 100;
		}
	}

	public class RidableLlamaStageFive : BaseEvoStage
	{
        public RidableLlamaStageFive()
		{
            Title = "The Ancient Ridable Llama";
            EvolutionMessage = "has evolved to its highest form and is now an Ancient Ridable Llama";
			NextEpThreshold = 0; EpMinDivisor = 160; EpMaxDivisor = 40; DustMultiplier = 20;
			BaseSoundID = 0x3F5; ControlSlots = 4;
			BodyValue = 0xDC; VirtualArmor = 100;
		
			ResistanceTypes = new ResistanceType[5] { ResistanceType.Physical, ResistanceType.Fire, ResistanceType.Cold,
														ResistanceType.Poison, ResistanceType.Energy };
			MinResistances = new int[5] { 55, 70, 25, 40, 40 };
			MaxResistances = new int[5] { 70, 80, 45, 50, 50 };	

			DamageMin = 10; DamageMax = 10; HitsMin= 300; HitsMax = 300;
			StrMin = 200; StrMax = 200; DexMin = 60; DexMax = 60; IntMin = 200; IntMax = 200;
		}
	}
}