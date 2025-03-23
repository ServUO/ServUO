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
using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleRestrictions : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleItemRestrictions Items { get; protected set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattlePetRestrictions Pets { get; protected set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleSkillRestrictions Skills { get; protected set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPBattleSpellRestrictions Spells { get; protected set; }

		public PvPBattleRestrictions()
		{
			Items = new PvPBattleItemRestrictions();
			Pets = new PvPBattlePetRestrictions();
			Skills = new PvPBattleSkillRestrictions();
			Spells = new PvPBattleSpellRestrictions();
		}

		public PvPBattleRestrictions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Battle Restrictions";
		}

		public override void Clear()
		{
			Items.Clear();
			Pets.Clear();
			Skills.Clear();
			Spells.Clear();
		}

		public override void Reset()
		{
			Items.Reset(false);
			Pets.Reset(false);
			Skills.Reset(false);
			Spells.Reset(false);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteBlock(w => w.WriteType(Items, t => Items.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Pets, t => Pets.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Skills, t => Skills.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Spells, t => Spells.Serialize(w)));
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					reader.ReadBlock(r => Items = r.ReadTypeCreate<PvPBattleItemRestrictions>(r) ?? new PvPBattleItemRestrictions());
					reader.ReadBlock(r => Pets = r.ReadTypeCreate<PvPBattlePetRestrictions>(r) ?? new PvPBattlePetRestrictions());
					reader.ReadBlock(
						r => Skills = r.ReadTypeCreate<PvPBattleSkillRestrictions>(r) ?? new PvPBattleSkillRestrictions());
					reader.ReadBlock(
						r => Spells = r.ReadTypeCreate<PvPBattleSpellRestrictions>(r) ?? new PvPBattleSpellRestrictions());
				}
				break;
			}
		}
	}
}