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
using Server.Mobiles;
#endregion

namespace VitaNex
{
	public interface ITypeSelectProperty
	{
		Type ExpectedType { get; }
		Type InternalType { get; }

		bool IsNotNull { get; }

		[CommandProperty(AccessLevel.GameMaster)]
		string TypeName { get; set; }

		bool CheckType(Type t);

		object CreateInstanceObject(params object[] args);
	}

	public interface ITypeSelectProperty<TObj> : ITypeSelectProperty
		where TObj : class
	{
		TObj CreateInstance(params object[] args);

		T CreateInstance<T>(params object[] args)
			where T : TObj;
	}

	public class TypeSelectProperty<TObj> : PropertyObject, ITypeSelectProperty<TObj>
		where TObj : class
	{
		private readonly Type _ExpectedType = typeof(TObj);

		public Type ExpectedType => _ExpectedType;

		public Type InternalType { get; protected set; }

		public bool IsNotNull => InternalType != null;

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual string TypeName
		{
			get => IsNotNull ? InternalType.Name : String.Empty;
			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					InternalType = null;
					return;
				}

				VitaNexCore.TryCatch(
					() =>
					{
						var t = Type.GetType(value, false, true) ??
								ScriptCompiler.FindTypeByName(value, true) ?? ScriptCompiler.FindTypeByFullName(value, true);

						if (CheckType(t))
						{
							InternalType = t;
						}
					},
					VitaNexCore.ToConsole);
			}
		}

		public TypeSelectProperty(string type = "")
		{
			TypeName = type;
		}

		public TypeSelectProperty(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			TypeName = null;
		}

		public override void Reset()
		{
			TypeName = null;
		}

		public virtual bool CheckType(Type t)
		{
			return CheckType(t, true);
		}

		public virtual bool CheckType(Type t, bool children)
		{
			return t != null &&
				   (!children || ExpectedType.IsSealed ? t.IsEqual(ExpectedType) : t.IsEqualOrChildOf(ExpectedType));
		}

		public virtual object CreateInstanceObject(params object[] args)
		{
			return CreateInstance(args);
		}

		public virtual TObj CreateInstance(params object[] args)
		{
			return CreateInstance<TObj>(args);
		}

		public virtual T CreateInstance<T>(params object[] args)
			where T : TObj
		{
			return IsNotNull ? InternalType.CreateInstanceSafe<T>(args) : default(T);
		}

		public override string ToString()
		{
			return String.IsNullOrEmpty(TypeName) ? "null" : TypeName;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.WriteType(InternalType);
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
					InternalType = reader.ReadType();
					break;
			}
		}

		public static implicit operator TypeSelectProperty<TObj>(string typeName)
		{
			return new TypeSelectProperty<TObj>(typeName);
		}

		public static implicit operator string(TypeSelectProperty<TObj> t)
		{
			return ((t == null || t.InternalType == null) ? String.Empty : t.InternalType.FullName);
		}

		public static implicit operator TypeSelectProperty<TObj>(Type t)
		{
			return new TypeSelectProperty<TObj>((t == null) ? null : t.FullName);
		}

		public static implicit operator Type(TypeSelectProperty<TObj> t)
		{
			return ((t == null) ? null : t.InternalType);
		}
	}

	public class EntityTypeSelectProperty : TypeSelectProperty<IEntity>
	{
		public EntityTypeSelectProperty(string type = "")
			: base(type)
		{ }

		public EntityTypeSelectProperty(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}

		public static implicit operator EntityTypeSelectProperty(string typeName)
		{
			return new EntityTypeSelectProperty(typeName);
		}

		public static implicit operator string(EntityTypeSelectProperty t)
		{
			return ((t == null || t.InternalType == null) ? String.Empty : t.InternalType.FullName);
		}

		public static implicit operator EntityTypeSelectProperty(Type t)
		{
			return new EntityTypeSelectProperty((t == null) ? null : t.FullName);
		}

		public static implicit operator Type(EntityTypeSelectProperty t)
		{
			return ((t == null) ? null : t.InternalType);
		}
	}

	public class ItemTypeSelectProperty : TypeSelectProperty<Item>
	{
		public ItemTypeSelectProperty(string type = "")
			: base(type)
		{ }

		public ItemTypeSelectProperty(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}

		public static implicit operator ItemTypeSelectProperty(string typeName)
		{
			return new ItemTypeSelectProperty(typeName);
		}

		public static implicit operator string(ItemTypeSelectProperty t)
		{
			return ((t == null || t.InternalType == null) ? String.Empty : t.InternalType.FullName);
		}

		public static implicit operator ItemTypeSelectProperty(Type t)
		{
			return new ItemTypeSelectProperty((t == null) ? null : t.FullName);
		}

		public static implicit operator Type(ItemTypeSelectProperty t)
		{
			return ((t == null) ? null : t.InternalType);
		}
	}

	public class MobileTypeSelectProperty : TypeSelectProperty<Mobile>
	{
		public MobileTypeSelectProperty(string type = "")
			: base(type)
		{ }

		public MobileTypeSelectProperty(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}

		public static implicit operator MobileTypeSelectProperty(string typeName)
		{
			return new MobileTypeSelectProperty(typeName);
		}

		public static implicit operator string(MobileTypeSelectProperty t)
		{
			return ((t == null || t.InternalType == null) ? String.Empty : t.InternalType.FullName);
		}

		public static implicit operator MobileTypeSelectProperty(Type t)
		{
			return new MobileTypeSelectProperty((t == null) ? null : t.FullName);
		}

		public static implicit operator Type(MobileTypeSelectProperty t)
		{
			return ((t == null) ? null : t.InternalType);
		}
	}

	public class CreatureTypeSelectProperty : TypeSelectProperty<BaseCreature>
	{
		public CreatureTypeSelectProperty(string type = "")
			: base(type)
		{ }

		public CreatureTypeSelectProperty(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}

		public static implicit operator CreatureTypeSelectProperty(string typeName)
		{
			return new CreatureTypeSelectProperty(typeName);
		}

		public static implicit operator string(CreatureTypeSelectProperty t)
		{
			return ((t == null || t.InternalType == null) ? String.Empty : t.InternalType.FullName);
		}

		public static implicit operator CreatureTypeSelectProperty(Type t)
		{
			return new CreatureTypeSelectProperty((t == null) ? null : t.FullName);
		}

		public static implicit operator Type(CreatureTypeSelectProperty t)
		{
			return ((t == null) ? null : t.InternalType);
		}
	}

	public class VendorTypeSelectProperty : TypeSelectProperty<BaseVendor>
	{
		public VendorTypeSelectProperty(string type = "")
			: base(type)
		{ }

		public VendorTypeSelectProperty(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}

		public static implicit operator VendorTypeSelectProperty(string typeName)
		{
			return new VendorTypeSelectProperty(typeName);
		}

		public static implicit operator string(VendorTypeSelectProperty t)
		{
			return ((t == null || t.InternalType == null) ? String.Empty : t.InternalType.FullName);
		}

		public static implicit operator VendorTypeSelectProperty(Type t)
		{
			return new VendorTypeSelectProperty((t == null) ? null : t.FullName);
		}

		public static implicit operator Type(VendorTypeSelectProperty t)
		{
			return ((t == null) ? null : t.InternalType);
		}
	}

	public class SpellTypeSelectProperty : TypeSelectProperty<ISpell>
	{
		public SpellTypeSelectProperty(string type = "")
			: base(type)
		{ }

		public SpellTypeSelectProperty(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}

		public static implicit operator SpellTypeSelectProperty(string typeName)
		{
			return new SpellTypeSelectProperty(typeName);
		}

		public static implicit operator string(SpellTypeSelectProperty t)
		{
			return ((t == null || t.InternalType == null) ? String.Empty : t.InternalType.FullName);
		}

		public static implicit operator SpellTypeSelectProperty(Type t)
		{
			return new SpellTypeSelectProperty((t == null) ? null : t.FullName);
		}

		public static implicit operator Type(SpellTypeSelectProperty t)
		{
			return ((t == null) ? null : t.InternalType);
		}
	}
}