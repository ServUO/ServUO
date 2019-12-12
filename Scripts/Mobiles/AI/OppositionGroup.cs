#region References
using System;

using Server.Mobiles;
#endregion

namespace Server
{
	public class OppositionGroup
	{
		private static readonly OppositionGroup m_TerathansAndOphidians = new OppositionGroup(
			new[]
			{
				new[] {typeof(TerathanAvenger), typeof(TerathanDrone), typeof(TerathanMatriarch), typeof(TerathanWarrior)},
				new[]
				{
					typeof(OphidianArchmage), typeof(OphidianKnight), typeof(OphidianMage), typeof(OphidianMatriarch),
					typeof(OphidianWarrior)
				}
			});

		private static readonly OppositionGroup m_SavagesAndOrcs = new OppositionGroup(
			new[]
			{
				new[]
				{
					typeof(Orc), typeof(OrcBomber), typeof(OrcBrute), typeof(OrcCaptain), typeof(OrcChopper), typeof(OrcishLord),
					typeof(OrcishMage), typeof(OrcScout), typeof(SpawnedOrcishLord)
				},
				new[] {typeof(Savage), typeof(SavageRider), typeof(SavageRidgeback), typeof(SavageShaman)}
			});

		private static readonly OppositionGroup m_FeyAndUndead = new OppositionGroup(
			new[]
			{
				new[]
				{
					typeof(Centaur), typeof(EtherealWarrior), typeof(Kirin), typeof(LordOaks), typeof(Pixie), typeof(Silvani),
					typeof(Unicorn), typeof(Wisp), typeof(Treefellow), typeof(MLDryad), typeof(Satyr)
				},
				new[]
				{
					typeof(AncientLich), typeof(Bogle), typeof(BoneKnight), typeof(BoneMagi), typeof(DarknightCreeper), typeof(Ghoul),
					typeof(LadyOfTheSnow), typeof(Lich), typeof(LichLord), typeof(Mummy), typeof(RevenantLion), typeof(RottingCorpse),
					typeof(Shade), typeof(ShadowKnight), typeof(SkeletalDragon), typeof(SkeletalDrake), typeof(SkeletalKnight),
					typeof(SkeletalMage), typeof(Skeleton), typeof(Spectre), typeof(Wraith), typeof(Zombie)
				}
			});

		private readonly Type[][] m_Types;

		public OppositionGroup(Type[][] types)
		{
			m_Types = types;
		}

		public static OppositionGroup TerathansAndOphidians { get { return m_TerathansAndOphidians; } }
		public static OppositionGroup SavagesAndOrcs { get { return m_SavagesAndOrcs; } }
		public static OppositionGroup FeyAndUndead { get { return m_FeyAndUndead; } }

		public bool IsEnemy(object from, object target)
		{
			var fromGroup = IndexOf(from);
			var targGroup = IndexOf(target);

			return (fromGroup != -1 && targGroup != -1 && fromGroup != targGroup);
		}

		public int IndexOf(object obj)
		{
			if (obj == null)
				return -1;

			var type = obj.GetType();

			for (var i = 0; i < m_Types.Length; ++i)
			{
				var group = m_Types[i];

				var contains = false;

				for (var j = 0; !contains && j < group.Length; ++j)
					contains = group[j].IsAssignableFrom(type);

				if (contains)
					return i;
			}

			return -1;
		}
	}
}