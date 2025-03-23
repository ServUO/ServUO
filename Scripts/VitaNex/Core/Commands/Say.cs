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
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

using VitaNex.Targets;
#endregion

namespace VitaNex.Commands
{
	public class SayCommand : BaseCommand
	{
		public static void Initialize()
		{
			TargetCommands.Register(new SayCommand());
		}

		public SayCommand()
		{
			AccessLevel = AccessLevel.GameMaster;
			Supports = CommandSupport.All;
			Commands = new[] { "Say" };
			ObjectTypes = ObjectTypes.All;
			Usage = "Say <speech>";
			Description = "Causes an object to say the given speech.";
		}

		public override void Execute(CommandEventArgs e, object o)
		{
			HandleTarget(e.Mobile as PlayerMobile, o as IPoint3D, e.ArgString);
		}

		public static void BeginTarget(PlayerMobile m, string speech)
		{
			if (m != null && !String.IsNullOrWhiteSpace(speech))
			{
				GenericSelectTarget<IPoint3D>.Begin(m, (user, target) => HandleTarget(m, target, speech), null);
			}
		}

		public static bool HandleTarget(PlayerMobile m, IPoint3D target, string speech)
		{
			if (m == null || target == null || String.IsNullOrWhiteSpace(speech))
			{
				return false;
			}

			if (target is Item)
			{
				var item = (Item)target;

				item.PublicOverheadMessage(MessageType.Regular, m.SpeechHue, false, speech);

				return true;
			}

			if (target is Mobile)
			{
				var mobile = (Mobile)target;

				mobile.Say(speech);

				return true;
			}

			if (target is StaticTarget)
			{
				var t = (StaticTarget)target;

				Send(m.Map, t.Location, t.ItemID, m.SpeechHue, t.Name, speech);

				return true;
			}

			if (target is LandTarget)
			{
				var t = (LandTarget)target;

				Send(m.Map, t.Location, 0, m.SpeechHue, t.Name, speech);

				return true;
			}

			return false;
		}

		private static void Send(Map map, Point3D loc, int itemID, int hue, string name, string speech)
		{
			var fx = EffectItem.Create(loc, map, EffectItem.DefaultDuration);

			Packet p = null;

			var eable = map.GetClientsInRange(loc, Core.GlobalMaxUpdateRange);

			foreach (var state in eable)
			{
				if (p == null)
				{
					p = Packet.Acquire(new UnicodeMessage(fx.Serial, itemID, MessageType.Label, hue, 1, "ENU", name, speech));
				}

				state.Send(p);
			}

			Packet.Release(p);

			eable.Free();
		}
	}
}