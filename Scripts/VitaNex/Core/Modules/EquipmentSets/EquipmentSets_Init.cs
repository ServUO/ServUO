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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Network;

using VitaNex.Network;
#endregion

namespace VitaNex.Modules.EquipmentSets
{
	[CoreModule("Equipment Sets", "3.0.0.1")]
	public static partial class EquipmentSets
	{
		static EquipmentSets()
		{
			CMOptions = new EquipmentSetsOptions();

			SetTypes = TypeOfEquipmentSet.GetConstructableChildren();

			Sets = new Dictionary<Type, EquipmentSet>(SetTypes.Length);

			foreach (var t in SetTypes)
			{
				Sets[t] = t.CreateInstanceSafe<EquipmentSet>();
			}
		}

		private static void CMConfig()
		{
			EquipItemParent = OutgoingPacketOverrides.GetHandler(0x2E);

			OutgoingPacketOverrides.Register(0x2E, EquipItem);

			EquipItemRequestParent = PacketHandlers.GetHandler(0x13);
			DropItemRequestParent = PacketHandlers.GetHandler(0x08);

			PacketHandlers.Register(EquipItemRequestParent.PacketID, EquipItemRequestParent.Length, EquipItemRequestParent.Ingame, EquipItemRequest);
			PacketHandlers.Register(DropItemRequestParent.PacketID, DropItemRequestParent.Length, DropItemRequestParent.Ingame, DropItemRequest);

#if !ServUOX
            EquipItemRequestParent6017 = PacketHandlers.Get6017Handler(0x13);
			DropItemRequestParent6017 = PacketHandlers.Get6017Handler(0x08);

			PacketHandlers.Register6017(EquipItemRequestParent6017.PacketID, EquipItemRequestParent6017.Length, EquipItemRequestParent6017.Ingame, EquipItemRequest6017);
			PacketHandlers.Register6017(DropItemRequestParent6017.PacketID, DropItemRequestParent6017.Length, DropItemRequestParent6017.Ingame, DropItemRequest6017);
#endif
		}

		private static void CMInvoke()
		{
			EventSink.Login += OnLogin;
			ExtendedOPL.OnItemOPLRequest += GetProperties;
		}

		private static void CMEnabled()
		{
			if (World.Loaded)
			{
				World.Mobiles.Values.AsParallel().Where(m => m.Items != null && m.Items.Any(i => i.Layer.IsEquip())).ForEach(Invalidate);
			}
		}

		private static void CMDisabled()
		{
			if (World.Loaded)
			{
				foreach (var set in Sets.Values)
				{
					set.ActiveOwners.ForEachReverse(Invalidate);
				}
			}
		}

		private static void CMDispose()
		{
			if (EquipItemRequestParent != null && EquipItemRequestParent.OnReceive != null)
			{
				PacketHandlers.Register(EquipItemRequestParent.PacketID, EquipItemRequestParent.Length, EquipItemRequestParent.Ingame, EquipItemRequestParent.OnReceive);
			}

			if (DropItemRequestParent != null && DropItemRequestParent.OnReceive != null)
			{
				PacketHandlers.Register(DropItemRequestParent.PacketID, DropItemRequestParent.Length, DropItemRequestParent.Ingame, DropItemRequestParent.OnReceive);
			}

#if !ServUOX
            if (EquipItemRequestParent6017 != null && EquipItemRequestParent6017.OnReceive != null)
			{
				PacketHandlers.Register(EquipItemRequestParent6017.PacketID, EquipItemRequestParent6017.Length, EquipItemRequestParent6017.Ingame, EquipItemRequestParent6017.OnReceive);
			}

			if (DropItemRequestParent6017 != null && DropItemRequestParent6017.OnReceive != null)
			{
				PacketHandlers.Register(DropItemRequestParent6017.PacketID, DropItemRequestParent6017.Length, DropItemRequestParent6017.Ingame, DropItemRequestParent6017.OnReceive);
			}
#endif
		}
	}
}
