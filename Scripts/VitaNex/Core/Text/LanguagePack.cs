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
#endregion

namespace VitaNex.Text
{
	public class IndependentLanguagePack
	{
		private readonly Dictionary<int, string> _Definitions = new Dictionary<int, string>();

		/// <summary>
		///     Gets the string at the given index, or String.Empty if undefined
		/// </summary>
		public virtual string this[int index]
		{
			get => _Definitions.ContainsKey(index) ? _Definitions[index] ?? String.Empty : String.Empty;
			set
			{
				if (_Definitions.ContainsKey(index))
				{
					_Definitions[index] = value;
				}
				else
				{
					_Definitions.Add(index, value);
				}
			}
		}

		/// <summary>
		///     Checks if the given index has a defined entry.
		/// </summary>
		public bool IsDefined(int index)
		{
			return !String.IsNullOrEmpty(this[index]);
		}

		/// <summary>
		///     Sets the value of the given index
		/// </summary>
		public void Define(int index, string value)
		{
			this[index] = value;
		}

		/// <summary>
		///     Sets the value of the given index
		/// </summary>
		public string ToString(int index)
		{
			return this[index];
		}

		/// <summary>
		///     Gets a formatted message string at the given index using the specified optional parameters to be included during
		///     formatting
		/// </summary>
		public string Format(int index, params object[] args)
		{
			return String.Format(this[index], args);
		}
	}

	public class LanguagePack
	{
		/// <summary>
		///     A table used for overriding cliloc text for each language
		/// </summary>
		private readonly Dictionary<ClilocLNG, Dictionary<int, string>> _TableMutations =
			new Dictionary<ClilocLNG, Dictionary<int, string>>
			{
				{ClilocLNG.ENU, new Dictionary<int, string>()},
				{ClilocLNG.DEU, new Dictionary<int, string>()},
				{ClilocLNG.ESP, new Dictionary<int, string>()},
				{ClilocLNG.FRA, new Dictionary<int, string>()},
				{ClilocLNG.JPN, new Dictionary<int, string>()},
				{ClilocLNG.KOR, new Dictionary<int, string>()},
				{ClilocLNG.CHT, new Dictionary<int, string>()}
			};

		/// <summary>
		///     Default constructor
		/// </summary>
		public LanguagePack(ClilocLNG lng = ClilocLNG.ENU)
		{
			Language = lng;
		}

		/// <summary>
		///     Current language used by this instance
		/// </summary>
		public virtual ClilocLNG Language { get; protected set; }

		/// <summary>
		///     Gets a cliloc table using this instance' selected language
		/// </summary>
		protected virtual ClilocTable Table => Clilocs.Tables[Language];

		/// <summary>
		///     Gets the string at the given index, or String.Empty if undefined
		/// </summary>
		public virtual string this[int index]
		{
			get => !Table.IsNullOrWhiteSpace(index)
					? (_TableMutations[Language].ContainsKey(index) ? _TableMutations[Language][index] : Table[index].Text)
					: (_TableMutations[Language].ContainsKey(index) ? _TableMutations[Language][index] : String.Empty);
			set
			{
				if (_TableMutations[Language].ContainsKey(index))
				{
					_TableMutations[Language][index] = value;
				}
				else
				{
					_TableMutations[Language].Add(index, value);
				}
			}
		}

		/// <summary>
		///     Checks if the given index has a defined entry.
		/// </summary>
		public bool IsDefined(int index)
		{
			return !String.IsNullOrEmpty(this[index]);
		}

		/// <summary>
		///     Sets the value of the given index
		/// </summary>
		public void Define(int index, string value)
		{
			this[index] = value;
		}

		/// <summary>
		///     Sets the value of the given index
		/// </summary>
		public string ToString(int index)
		{
			return this[index];
		}

		/// <summary>
		///     Gets a formatted message string at the given index using the specified optional parameters to be included during
		///     formatting
		/// </summary>
		public string Format(int index, params object[] args)
		{
			return String.Format(this[index], args);
		}
	}
}