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
using System.Collections.Generic;

using Server;

using VitaNex.Text;
#endregion

namespace VitaNex.Modules.AntiAdverts
{
	public sealed class AntiAdvertsOptions : CoreModuleOptions
	{
		/// <summary>
		///     Log the info in a log file?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool LogEnabled { get; set; }

		/// <summary>
		///     Write info to the console?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool ConsoleWrite { get; set; }

		/// <summary>
		///     Page staff with offense?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool PageStaff { get; set; }

		/// <summary>
		///     Send a warning broadcast to staff?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool NotifyStaff { get; set; }

		/// <summary>
		///     Send a warning to offending player
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool NotifyPlayer { get; set; }

		/// <summary>
		///     Send a warning broadcast to staff of this access and above.
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public AccessLevel NotifyAccess { get; set; }

		/// <summary>
		///     AccessLevels higher than this value will not have their speech handled.
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public AccessLevel HandleAccess { get; set; }

		/// <summary>
		///     Jail the offender?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool Jail { get; set; }

		/// <summary>
		///     The location and map of the jail area.
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public MapPoint JailPoint { get; set; }

		/// <summary>
		///     Squelch the offender?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool Squelch { get; set; }

		/// <summary>
		///     Kick the offender?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool Kick { get; set; }

		/// <summary>
		///     Ban the offender?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool Ban { get; set; }

		/// <summary>
		///     The mode used to find matching key words in text.
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public StringSearchFlags SearchMode { get; set; }

		/// <summary>
		///     Should the search be case-insensitive?
		/// </summary>
		[CommandProperty(AntiAdverts.Access)]
		public bool SearchCapsIgnore { get; set; }

		/// <summary>
		///     A list of common symbols used to replace whitespaces to avoid detection.
		/// </summary>
		public List<char> WhitespaceAliases { get; private set; }

		/// <summary>
		///     A case-insensitive list of all disallowed keywords and phrases.
		///     Any whitespaces in keywords will also be tested with their aliases.
		/// </summary>
		public List<string> KeyWords { get; private set; }

		public AntiAdvertsOptions()
			: base(typeof(AntiAdverts))
		{
			WhitespaceAliases = new List<char>();
			KeyWords = new List<string>();

			EnsureDefaults();
		}

		public AntiAdvertsOptions(GenericReader reader)
			: base(reader)
		{ }

		public void EnsureDefaults()
		{
			LogEnabled = true;
			ConsoleWrite = true;

			PageStaff = true;

			NotifyStaff = true;
			NotifyAccess = AccessLevel.Counselor;

			HandleAccess = AccessLevel.Player;

			Jail = false;
			JailPoint = new MapPoint(Map.Felucca, new Point3D(5275, 1174, 0));

			Squelch = false;
			Kick = false;
			Ban = false;

			SearchMode = StringSearchFlags.Contains;
			SearchCapsIgnore = true;

			WhitespaceAliases.AddRange(
				new[]
				{
					'_', ':', ';', '@', '#', '=', '-', '+', '*', '/', // 
					'\\', '!', '"', '£', '$', '%', '^', '&', '"' // 
				});

			KeyWords.AddRange(
				new[]
				{
					"port: 2593", "port 2593", "port: 2595", "port 2595", "paypal", "no-ip", "joinuo", "uoisnotdead", "shard.li",
					"easyuo"
				});
		}

		public override void Clear()
		{
			base.Clear();

			EnsureDefaults();
		}

		public override void Reset()
		{
			base.Reset();

			EnsureDefaults();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(2);

			switch (version)
			{
				case 2:
					writer.Write(NotifyPlayer);
					goto case 1;
				case 1:
				{
					writer.WriteFlag(SearchMode);
					writer.Write(SearchCapsIgnore);
				}
				goto case 0;
				case 0:
				{
					writer.WriteBlockList(WhitespaceAliases, (w, a) => w.Write(a));
					writer.WriteBlockList(KeyWords, (w, k) => w.Write(k));

					writer.Write(LogEnabled);
					writer.Write(ConsoleWrite);

					writer.Write(PageStaff);

					writer.Write(NotifyStaff);
					writer.WriteFlag(NotifyAccess);

					writer.Write(Jail);
					JailPoint.Serialize(writer);

					writer.Write(Squelch);
					writer.Write(Kick);
					writer.Write(Ban);
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
				case 2:
					NotifyPlayer = reader.ReadBool();
					goto case 1;
				case 1:
				{
					SearchMode = reader.ReadFlag<StringSearchFlags>();
					SearchCapsIgnore = reader.ReadBool();
				}
				goto case 0;
				case 0:
				{
					if (version < 1)
					{
						SearchMode = StringSearchFlags.Contains;
						SearchCapsIgnore = true;
					}

					WhitespaceAliases = reader.ReadBlockList(r => r.ReadChar());
					KeyWords = reader.ReadBlockList(r => r.ReadString());

					LogEnabled = reader.ReadBool();
					ConsoleWrite = reader.ReadBool();

					PageStaff = reader.ReadBool();

					NotifyStaff = reader.ReadBool();
					NotifyAccess = reader.ReadFlag<AccessLevel>();

					Jail = reader.ReadBool();
					JailPoint = new MapPoint(reader);

					Squelch = reader.ReadBool();
					Kick = reader.ReadBool();
					Ban = reader.ReadBool();
				}
				break;
			}
		}
	}
}