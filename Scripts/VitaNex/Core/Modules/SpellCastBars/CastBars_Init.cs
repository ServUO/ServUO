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
using System.Drawing;
using System.Linq;

using Server;
using Server.Mobiles;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.CastBars
{
	[CoreModule("Spell Cast Bars", "2.0.0.0")]
	public static partial class SpellCastBars
	{
		static SpellCastBars()
		{
			CMOptions = new CastBarsOptions();

			_CastBarQueue = new Queue<PlayerMobile>();

			_InternalTimer = PollTimer.CreateInstance(TimeSpan.FromSeconds(0.1), PollCastBarQueue, _CastBarQueue.Any);

			Instances = new Dictionary<PlayerMobile, SpellCastBar>();

			States = new BinaryDataStore<PlayerMobile, CastBarState>(VitaNexCore.SavesDirectory + "/SpellCastBars", "States")
			{
				Async = true,
				OnSerialize = Serialize,
				OnDeserialize = Deserialize
			};
		}

		private static void CMInvoke()
		{
			EventSink.CastSpellRequest += OnSpellRequest;
		}

		private static void CMEnabled()
		{
			_InternalTimer.Start();
		}

		private static void CMDisabled()
		{
			_InternalTimer.Stop();
		}

		private static void CMSave()
		{
			States.Export();
		}

		private static void CMLoad()
		{
			States.Import();
		}

		private static bool Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{
					writer.WriteBlockDictionary(
						States,
						(w, k, v) =>
						{
							w.Write(k);
							v.Serialize(w);
						});
				}
				break;
				case 0:
				{
					writer.WriteBlockDictionary(
						States,
						(w, k, v) =>
						{
							w.Write(k);
							w.Write(v.Enabled);
							w.Write(v.Offset.X);
							w.Write(v.Offset.Y);
						});
				}
				break;
			}

			return true;
		}

		private static bool Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
				{
					reader.ReadBlockDictionary(
						r =>
						{
							var k = r.ReadMobile<PlayerMobile>();
							var v = new CastBarState(r);

							return new KeyValuePair<PlayerMobile, CastBarState>(k, v);
						},
						States);
				}
				break;
				case 0:
				{
					reader.ReadBlockDictionary(
						r =>
						{
							var k = r.ReadMobile<PlayerMobile>();
							var v = new CastBarState(r.ReadBool(), new Point(r.ReadInt(), r.ReadInt()));

							return new KeyValuePair<PlayerMobile, CastBarState>(k, v);
						},
						States);
				}
				break;
			}

			return true;
		}
	}
}