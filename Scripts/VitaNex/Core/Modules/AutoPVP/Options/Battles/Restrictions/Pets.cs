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
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public class PvPBattlePetRestrictions : PvPBattleRestrictionsBase<Type>
	{
		private static readonly Type _TypeOf = typeof(BaseCreature);

		private static Type FindType(string name, bool full = false, bool ignoreCase = true)
		{
			return Type.GetType(name, false, ignoreCase) ?? (full
					   ? ScriptCompiler.FindTypeByFullName(name, ignoreCase)
					   : ScriptCompiler.FindTypeByName(name, ignoreCase));
		}

		public PvPBattlePetRestrictions()
		{ }

		public PvPBattlePetRestrictions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Pet Restrictions";
		}

		public override void Invalidate()
		{ }

		public virtual void SetRestricted(BaseCreature pet, bool restrict)
		{
			if (pet != null)
			{
				SetRestricted(pet.GetType(), restrict);
			}
		}

		public virtual void SetRestricted(string pet, bool restrict)
		{
			if (!String.IsNullOrWhiteSpace(pet))
			{
				SetRestricted(FindType(pet), restrict);
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

		public virtual bool IsRestricted(BaseCreature pet)
		{
			return pet != null && IsRestricted(pet.GetType());
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