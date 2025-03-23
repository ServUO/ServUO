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
using Server;
using Server.Items;
using Server.Network;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	[Furniture]
	public class TrashCan : BaseTrashContainer
	{
		public static string[] SuccessEmotes =
		{
			"*Nom Nom Nom*", "*Burp*", "I Like Trash", "*Brofist*", "That's What SHE Said!", "I Have No Bottom, I Do Not Poop",
			"That Tickles!", "I Cast Thy trash Into The Abyss!", "Trash, Be Gone!", "*Boing*", "*Poof*", "Ha Ha, Made You Look",
			"It's Gone, Into The Void", "Have You Seen My Brothers?", "Your Trash Was Trashed, Heh", "*Weeeeeeeeeee... Thud*"
		};

		public static string[] FailEmotes =
		{
			"Ewww", "Thanks, But No Thanks", "Are You Hitting On Me?", "Disgusting!", "Not My Kind Of Food",
			"I've Had That Before, It's Terrible", "Please! No!", "Pfft, You Eat It, If It's Sooo Good",
			"The Void Doesn't Want That", "That Gives Me Indigestion", "Risk Heartwood-Burn? No Thanks",
			"The Abyss Frowns At Your Offering"
		};

		[Constructable]
		public TrashCan()
			: base(0xE77, 0xE77)
		{
			Name = "Trash Can";
			Weight = 5;
		}

		public TrashCan(Serial serial)
			: base(serial)
		{ }

		public override bool Dye(Mobile from, DyeTub sender)
		{
			if (from == null || sender == null || !from.CanSee(sender) || !sender.IsAccessibleTo(from))
			{
				return false;
			}

			if (!(sender is FurnitureDyeTub))
			{
				return false;
			}

			return base.Dye(from, sender);
		}

		protected override void OnTrashed(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			base.OnTrashed(from, trashed, ref tokens, message);

			if (message && Utility.RandomDouble() < 0.20)
			{
				PublicOverheadMessage(MessageType.Emote, 0x55, true, SuccessEmotes[Utility.Random(SuccessEmotes.Length)]);
			}
		}

		protected override void OnTrashRejected(Mobile from, Item trashed, bool message = true)
		{
			base.OnTrashRejected(from, trashed, message);

			if (message && Utility.RandomDouble() < 0.20)
			{
				PublicOverheadMessage(MessageType.Emote, 0x55, true, FailEmotes[Utility.Random(FailEmotes.Length)]);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{ }
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
				{ }
				break;
			}
		}
	}
}