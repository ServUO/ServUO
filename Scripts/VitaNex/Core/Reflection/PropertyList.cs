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
using System.Linq;
using System.Reflection;
#endregion

namespace VitaNex.Reflection
{
	/// <summary>
	///     Represents a collection of object property names and values.
	/// </summary>
	/// <typeparam name="TObj">The type instance associated with the property names and values.</typeparam>
	public class PropertyList<TObj> : Dictionary<string, object>, IDisposable
	{
		public Func<PropertyInfo, bool> Filter { get; set; }

		public PropertyList()
		{ }

		public PropertyList(TObj instance, BindingFlags flags = BindingFlags.Default, Func<PropertyInfo, bool> filter = null)
		{
			Filter = filter;

			Deserialize(instance, flags);
		}

		/// <summary>
		///     Gets or sets the cached value associated with the specified property name.
		/// </summary>
		public new object this[string name]
		{
			get => ContainsKey(name) ? base[name] : null;
			set
			{
				if (ContainsKey(name))
				{
					base[name] = value;
				}
				else
				{
					Add(name, value);
				}
			}
		}

		public void Dispose()
		{
			Clear();
		}

		/// <summary>
		///     Write this list of properties and values to the specified object.
		/// </summary>
		public void Serialize(TObj o, BindingFlags flags = BindingFlags.Default)
		{
			if (o == null)
			{
				return;
			}

			if (flags == BindingFlags.Default)
			{
				flags = BindingFlags.Instance | BindingFlags.Public;
			}

			var props = o.GetType().GetProperties(flags);

			foreach (var prop in props.Where(
				prop => prop.CanWrite && ContainsKey(prop.Name) && (Filter == null || Filter(prop))))
			{
				prop.SetValue(o, this[prop.Name], null);
			}
		}

		/// <summary>
		///     Reads a list of properties from the specified object into this list.
		/// </summary>
		public void Deserialize(TObj o, BindingFlags flags = BindingFlags.Default)
		{
			if (o == null)
			{
				return;
			}

			Clear();

			if (flags == BindingFlags.Default)
			{
				flags = BindingFlags.Instance | BindingFlags.Public;
			}

			var props = o.GetType().GetProperties(flags);

			foreach (var prop in props.Where(prop => prop.CanRead && (Filter == null || Filter(prop))))
			{
				this[prop.Name] = prop.GetValue(o, null);
			}
		}

		/// <summary>
		///     Selects all properties of which value types match the specified generic type.
		/// </summary>
		public Dictionary<string, TProp> Select<TProp>()
		{
			return this.Where(kvp => kvp.Value is TProp).ToDictionary(kvp => kvp.Key, kvp => (TProp)kvp.Value);
		}

		/// <summary>
		///     Gets the value for the specified property name and converts it to the specified generic type.
		/// </summary>
		public TProp Get<TProp>(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				return default(TProp);
			}

			return this[name] is TProp ? (TProp)this[name] : default(TProp);
		}

		/// <summary>
		///     Sets the cached value for the specified property name.
		/// </summary>
		public void Set(string name, object value)
		{
			this[name] = value;
		}
	}
}