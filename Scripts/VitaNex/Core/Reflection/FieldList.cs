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
	///     Represents a collection of object field names and values.
	/// </summary>
	/// <typeparam name="TObj">The type instance associated with the field names and values.</typeparam>
	public class FieldList<TObj> : Dictionary<string, object>, IDisposable
	{
		public Func<FieldInfo, bool> Filter { get; set; }

		public FieldList()
		{ }

		public FieldList(TObj instance, BindingFlags flags = BindingFlags.Default, Func<FieldInfo, bool> filter = null)
		{
			Filter = filter;

			Deserialize(instance, flags);
		}

		/// <summary>
		///     Gets or sets the value associated with the specified field name.
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
		///     Write this list of fields and values to the specified object.
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

			var fields = o.GetType().GetFields(flags);

			foreach (var field in fields.Where(field => ContainsKey(field.Name) && (Filter == null || Filter(field))))
			{
				field.SetValue(o, this[field.Name]);
			}
		}

		/// <summary>
		///     Reads a list of fields from the specified object into this list.
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

			var fields = o.GetType().GetFields(flags);

			foreach (var field in fields.Where(field => Filter == null || Filter(field)))
			{
				this[field.Name] = field.GetValue(o);
			}
		}

		/// <summary>
		///     Selects all fields of which value types match the specified generic type
		/// </summary>
		public Dictionary<string, TField> Select<TField>()
		{
			return this.Where(kvp => kvp.Value is TField).ToDictionary(kvp => kvp.Key, kvp => (TField)kvp.Value);
		}

		/// <summary>
		///     Gets the value for the specified field name and converts it to the specified generic type
		/// </summary>
		public TField Get<TField>(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				return default(TField);
			}

			return this[name] is TField ? (TField)this[name] : default(TField);
		}

		/// <summary>
		///     Sets the cached value for the specified field name
		/// </summary>
		public void Set(string name, object value)
		{
			this[name] = value;
		}
	}
}