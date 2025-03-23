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
using System.Linq;

using Server;

using VitaNex.Items;
using VitaNex.Mobiles;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public class TrashVendor : AdvancedVendor, ITrashTokenProperties
	{
		[Constructable]
		public TrashVendor()
			: base("the trash collector", typeof(TrashToken), "Trash Tokens", "TT")
		{ }

		public TrashVendor(Serial serial)
			: base(serial)
		{ }

		protected override void InitBuyInfo()
		{
			AddStock<TrashCan>(100);
			AddStock<PersonalTrashBag>(750);
			AddStock<SepticTank>(1000);

			AddStock<ThrowableStinkBomb>(20);
			AddStock<ThrowableRat>(20);
			AddStock<ThrowableRock>(20);

			AddStock<StrobeLantern>(10000);

			AddStock<BroadcastScroll>(10);
			AddStock<BroadcastScroll_3Uses>(30);
			AddStock<BroadcastScroll_5Uses>(50);
			AddStock<BroadcastScroll_10Uses>(100);
			AddStock<BroadcastScroll_30Uses>(300);
			AddStock<BroadcastScroll_50Uses>(500);
			AddStock<BroadcastScroll_100Uses>(1000);
		}

		public bool Trash(Mobile from, Item trashed, bool message = true)
		{
			var tokens = 0;
			return Trash(from, trashed, ref tokens, message);
		}

		public bool Trash(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (from == null || trashed == null || trashed.Deleted)
			{
				return false;
			}

			if (!from.InRange(Location, 5))
			{
				if (message)
				{
					from.SendMessage("You must be within 5 paces to use that trash collector.");
				}

				OnTrashRejected(from, trashed, false);
				return false;
			}

			if (!CanTrash(from, trashed, message))
			{
				OnTrashRejected(from, trashed, message);
				return false;
			}

			foreach (var h in TrashCollection.Handlers.Values)
			{
				if (h != null && h.Trash(from, trashed, ref tokens, message))
				{
					OnTrashed(from, trashed, ref tokens, message);
					return true;
				}
			}

			OnTrashRejected(from, trashed, message);
			return false;
		}

		protected virtual void OnTrashed(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (from == null || trashed == null || trashed.Deleted)
			{
				return;
			}

			if (message && Utility.RandomDouble() < 0.20)
			{
				Say(TrashCan.SuccessEmotes.GetRandom());
			}
		}

		protected virtual void OnTrashRejected(Mobile from, Item trashed, bool message = true)
		{
			if (from == null || trashed == null || trashed.Deleted)
			{
				return;
			}

			if (message && !TrashCollection.CMOptions.ModuleEnabled)
			{
				Say("Sorry, I'm on my lunch break.");
			}
			else if (message && Utility.RandomDouble() < 0.20)
			{
				Say(TrashCan.FailEmotes.GetRandom());
			}
		}

		public virtual bool CanTrash(Mobile from, Item trash, bool message = true)
		{
			if (trash == null || trash.Deleted || !trash.Movable || !trash.IsAccessibleTo(from) ||
				!TrashCollection.CMOptions.ModuleEnabled)
			{
				return false;
			}

			return TrashCollection.Handlers.Values.Any(h => h != null && h.CanTrash(from, trash, message));
		}

		public override bool OnDragDrop(Mobile from, Item trashed)
		{
			if (from == null || trashed == null || trashed.Deleted)
			{
				return false;
			}

			if (Trash(from, trashed))
			{
				return true;
			}

			return base.OnDragDrop(from, trashed);
		}

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
	}
}