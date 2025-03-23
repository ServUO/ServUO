#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;

using Server;

using VitaNex.Reflection;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleRules : PropertyObject
	{
		private readonly object _CopyLock = new object();
		private PropertyList<PvPBattleRules> _CopyStore;

		[CommandProperty(AutoPvP.Access)]
		public bool AllowSpeech { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool AllowBeneficial { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool AllowHarmful { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool AllowHousing { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool AllowSpawn { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanDie { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanHeal { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanBeDamaged { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanDamageOwnTeam { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanDamageEnemyTeam { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanHealOwnTeam { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanHealEnemyTeam { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool AllowPets { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanMount { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanFly { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanMountEthereal { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanMoveThrough { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanResurrect { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanUseStuckMenu { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public bool CanEquip { get; set; }

		public PvPBattleRules()
		{
			SetDefaults();
		}

		public PvPBattleRules(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Battle Rules";
		}

		public void SetDefaults()
		{
			AllowBeneficial = true;
			AllowHarmful = true;
			AllowHousing = false;
			AllowPets = false;
			AllowSpawn = false;
			AllowSpeech = true;
			CanBeDamaged = true;
			CanDamageEnemyTeam = true;
			CanDamageOwnTeam = false;
			CanDie = false;
			CanEquip = true;
			CanFly = false;
			CanHeal = true;
			CanHealEnemyTeam = false;
			CanHealOwnTeam = true;
			CanMount = false;
			CanMountEthereal = false;
			CanMoveThrough = false;
			CanResurrect = false;
			CanUseStuckMenu = false;
		}

		public override void Clear()
		{
			SetDefaults();
		}

		public override void Reset()
		{
			SetDefaults();
		}

		public void CopyFrom(PvPBattleRules source)
		{
			source.CopyTo(this);
		}

		public void CopyTo(PvPBattleRules target)
		{
			lock (_CopyLock)
			{
				if (_CopyStore == null)
				{
					_CopyStore = new PropertyList<PvPBattleRules>
					{
						Filter = p => p.Name != "InvokeReset" && p.Name != "InvokeClear" && p.PropertyType.IsEqual<bool>()
					};
				}

				_CopyStore.Deserialize(this);
				_CopyStore.Serialize(target);
				_CopyStore.Clear();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(3);

			switch (version)
			{
				case 3:
					writer.Write(CanEquip);
					goto case 2;
				case 2:
					writer.Write(CanMoveThrough);
					goto case 1;
				case 1:
					writer.Write(CanFly);
					goto case 0;
				case 0:
				{
					writer.Write(AllowBeneficial);
					writer.Write(AllowHarmful);
					writer.Write(AllowHousing);
					writer.Write(AllowPets);
					writer.Write(AllowSpawn);
					writer.Write(AllowSpeech);
					writer.Write(CanBeDamaged);
					writer.Write(CanDamageEnemyTeam);
					writer.Write(CanDamageOwnTeam);
					writer.Write(CanDie);
					writer.Write(CanHeal);
					writer.Write(CanHealEnemyTeam);
					writer.Write(CanHealOwnTeam);
					writer.Write(CanMount);
					writer.Write(CanMountEthereal);
					writer.Write(CanResurrect);
					writer.Write(CanUseStuckMenu);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			SetDefaults();

			var version = reader.GetVersion();

			switch (version)
			{
				case 3:
					CanEquip = reader.ReadBool();
					goto case 2;
				case 2:
					CanMoveThrough = reader.ReadBool();
					goto case 1;
				case 1:
					CanFly = reader.ReadBool();
					goto case 0;
				case 0:
				{
					AllowBeneficial = reader.ReadBool();
					AllowHarmful = reader.ReadBool();
					AllowHousing = reader.ReadBool();
					AllowPets = reader.ReadBool();
					AllowSpawn = reader.ReadBool();
					AllowSpeech = reader.ReadBool();
					CanBeDamaged = reader.ReadBool();
					CanDamageEnemyTeam = reader.ReadBool();
					CanDamageOwnTeam = reader.ReadBool();
					CanDie = reader.ReadBool();
					CanHeal = reader.ReadBool();
					CanHealEnemyTeam = reader.ReadBool();
					CanHealOwnTeam = reader.ReadBool();
					CanMount = reader.ReadBool();
					CanMountEthereal = reader.ReadBool();
					CanResurrect = reader.ReadBool();
					CanUseStuckMenu = reader.ReadBool();
				}
				break;
			}
		}
	}
}