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
#endregion

namespace VitaNex.Modules.AutoPvP
{
	[PropertyObject]
	public class PvPBattleItemRestrictions : PvPBattleRestrictionsBase<Type>
	{
		private static readonly Type _TypeOf = typeof(Item);

		private static Type FindType(string name, bool full = false, bool ignoreCase = true)
		{
			return Type.GetType(name, false, ignoreCase) ?? (full
					   ? ScriptCompiler.FindTypeByFullName(name, ignoreCase)
					   : ScriptCompiler.FindTypeByName(name, ignoreCase));
		}

		[CommandProperty(AutoPvP.Access)]
		public bool AllowNonExceptional { get; set; }

		public PvPBattleItemRestrictions()
		{
			AllowNonExceptional = true;
		}

		public PvPBattleItemRestrictions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Item Restrictions";
		}

		public override void Invalidate()
		{ }

		public virtual void SetRestricted(Item item, bool restrict)
		{
			if (item != null)
			{
				SetRestricted(item.GetType(), restrict);
			}
		}

		public virtual void SetRestricted(string item, bool restrict)
		{
			if (!String.IsNullOrWhiteSpace(item))
			{
				SetRestricted(FindType(item), restrict);
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

		public virtual bool IsRestricted(Item item)
		{
			return item != null && IsRestricted(item.GetType());
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

			var v = writer.SetVersion(1);

			if (v > 0)
			{
				var flags = 0UL;

				SetFlag(ref flags, 0x1, AllowNonExceptional);

				writer.Write(flags);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var v = reader.GetVersion();

			if (v > 0)
			{
				var flags = reader.ReadULong();

				AllowNonExceptional = GetFlag(flags, 0x1);
			}
			else
			{
				AllowNonExceptional = true;
			}
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