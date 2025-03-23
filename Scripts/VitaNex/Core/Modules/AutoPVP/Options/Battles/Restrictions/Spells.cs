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
using System.Collections.Generic;

using Server;
using Server.Spells;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public class PvPBattleSpellRestrictions : PvPBattleRestrictionsBase<Type>
	{
		private static readonly Type _TypeOf = typeof(Spell);

		private static Type FindType(string name, bool full = false, bool ignoreCase = true)
		{
			return Type.GetType(name, false, ignoreCase) ?? (full
					   ? ScriptCompiler.FindTypeByFullName(name, ignoreCase)
					   : ScriptCompiler.FindTypeByName(name, ignoreCase));
		}

		public PvPBattleSpellRestrictions()
		{ }

		public PvPBattleSpellRestrictions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Spell Restrictions";
		}

		public override void Invalidate()
		{
			foreach (var t in SpellRegistry.Types)
			{
				SetRestricted(t, IsRestricted(t));
			}
		}

		public virtual void SetRestricted(Spell spell, bool restrict)
		{
			if (spell != null)
			{
				SetRestricted(spell.GetType(), restrict);
			}
		}

		public virtual void SetRestricted(string spell, bool restrict)
		{
			if (!String.IsNullOrWhiteSpace(spell))
			{
				SetRestricted(FindType(spell), restrict);
			}
		}

		public override void SetRestricted(Type key, bool val)
		{
			if (key == null)
			{
				return;
			}

			if (key.IsEqualOrChildOf(_TypeOf))
			{
				base.SetRestricted(key, val);
			}
		}

		public virtual bool IsRestricted(Spell spell)
		{
			return spell != null && IsRestricted(spell.GetType());
		}

		public override bool IsRestricted(Type key)
		{
			if (key == null)
			{
				return false;
			}

			if (key.IsEqualOrChildOf(_TypeOf))
			{
				return base.IsRestricted(_TypeOf) || base.IsRestricted(key);
			}

			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}

		public override void SerializeEntry(GenericWriter writer, Type key, bool val)
		{
			writer.WriteType(key);
			writer.Write(val);
		}

		public override KeyValuePair<Type, bool> DeserializeEntry(GenericReader reader)
		{
			var k = reader.ReadType();
			var v = reader.ReadBool();
			return new KeyValuePair<Type, bool>(k, v);
		}
	}
}