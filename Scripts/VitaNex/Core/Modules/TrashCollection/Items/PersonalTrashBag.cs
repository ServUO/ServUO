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
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public class PersonalTrashBag : BaseTrashContainer
	{
		public bool ConfirmBind { get; protected set; }

		public override bool DisplayWeight => false;

		[Constructable]
		public PersonalTrashBag()
			: this(true)
		{ }

		[Constructable]
		public PersonalTrashBag(bool confirmBind)
			: base(0xE76, 0xE76)
		{
			ConfirmBind = confirmBind;
			Name = "Personal Trash Bag";
			GumpID = 0x2648;
		}

		public PersonalTrashBag(Serial serial)
			: base(serial)
		{ }

		public override bool OnDragDropInto(Mobile from, Item trashed, Point3D p)
		{
			if (from == null || trashed == null || trashed.Deleted || !IsAccessibleTo(from))
			{
				return false;
			}

			if (BlessedFor != null && BlessedFor != from)
			{
				from.SendMessage(0x22, "That does not belong to you.");
				return false;
			}

			if (BlessedFor == null)
			{
				BlessedFor = from;
			}

			return base.OnDragDropInto(from, trashed, p);
		}

		public override bool OnDragDrop(Mobile from, Item trashed)
		{
			if (from == null || trashed == null || trashed.Deleted || !IsAccessibleTo(from))
			{
				return false;
			}

			if (BlessedFor != null && BlessedFor != from)
			{
				from.SendMessage(0x22, "That does not belong to you.");
				return false;
			}

			if (BlessedFor == null)
			{
				BlessedFor = from;
			}

			return base.OnDragDrop(from, trashed);
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null)
			{
				return;
			}

			if (!from.CanSee(this))
			{
				OnDoubleClickCantSee(from);
				return;
			}

			if (!IsAccessibleTo(from))
			{
				OnDoubleClickNotAccessible(from);
				return;
			}

			if (BlessedFor != null && BlessedFor != from)
			{
				from.SendMessage(0x22, "That does not belong to you.");
				return;
			}

			if (BlessedFor == null)
			{
				BlessedFor = from;
			}

			base.OnDoubleClick(from);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(ConfirmBind);
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
				case 0:
				{
					ConfirmBind = reader.ReadBool();
				}
				break;
			}
		}
	}
}