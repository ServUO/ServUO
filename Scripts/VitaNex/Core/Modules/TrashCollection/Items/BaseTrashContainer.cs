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
using Server.Items;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public abstract class BaseTrashContainer : BaseContainer, ITrashTokenProperties, IDyable
	{
		private int _ContainerItemID;

		public virtual int DefaultContainerItemID => 0xE77;
		public override bool DisplayWeight => false;
		public override bool DisplaysContent => false;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public virtual int ContainerItemID
		{
			get
			{
				if (_ContainerItemID <= 0)
				{
					_ContainerItemID = DefaultContainerItemID;
				}

				return _ContainerItemID;
			}
			set
			{
				if (value <= 0)
				{
					value = DefaultContainerItemID;
				}

				if (_ContainerItemID != value)
				{
					_ContainerItemID = value;
					Update();
				}
			}
		}

		public BaseTrashContainer(int itemID, int containerID)
			: base(itemID)
		{
			Weight = 1.0;

			ItemID = ContainerItemID = containerID;
		}

		public BaseTrashContainer(Serial serial)
			: base(serial)
		{ }

		public virtual bool Dye(Mobile mob, DyeTub sender)
		{
			if (mob == null || sender == null || !mob.CanSee(sender) || !sender.IsAccessibleTo(mob))
			{
				return false;
			}

			Hue = sender.DyedHue;
			return true;
		}

		public virtual void Update()
		{
			UpdateContainerData();
		}

		public override void UpdateContainerData()
		{
			if (ContainerItemID > 0)
			{
				ContainerData = ContainerData.GetData(ContainerItemID);
			}
			else
			{
				base.UpdateContainerData();
			}

			GumpID = -1;
			DropSound = -1;
			Delta(ItemDelta.Update);
		}

		public bool Trash(Mobile mob, Item trashed, bool message = true)
		{
			var tokens = 0;
			return Trash(mob, trashed, ref tokens, message);
		}

		public bool Trash(Mobile mob, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (mob == null || trashed == null || trashed.Deleted)
			{
				return false;
			}

			if (!mob.InRange(GetWorldLocation(), 5))
			{
				if (message)
				{
					mob.SendMessage("You must be within 5 paces to use that trash container.");
				}

				OnTrashRejected(mob, trashed, false);
				return false;
			}

			if (!CanTrash(mob, trashed, message))
			{
				OnTrashRejected(mob, trashed, message);
				return false;
			}

			var tokenTmp = tokens;
			var success = TrashCollection.Handlers.Values.Where(h => h != null && h.Enabled)
										 .Any(h => h.Trash(mob, trashed, ref tokenTmp, message));

			tokens = tokenTmp;

			if (success)
			{
				OnTrashed(mob, trashed, ref tokens, message);
			}
			else
			{
				OnTrashRejected(mob, trashed, message);
			}

			return success;
		}

		protected virtual void OnTrashed(Mobile mob, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);
		}

		protected virtual void OnTrashRejected(Mobile mob, Item trashed, bool message = true)
		{
			if (mob == null || trashed == null || trashed.Deleted)
			{
				return;
			}

			if (message && !TrashCollection.CMOptions.ModuleEnabled)
			{
				mob.SendMessage(0x22, "Trash Collection is currently out of service.");
			}
		}

		public virtual bool CanTrash(Mobile mob, Item trash, bool message = true)
		{
			if (trash == null || trash.Deleted || !trash.Movable || !trash.IsAccessibleTo(mob) ||
				!TrashCollection.CMOptions.ModuleEnabled)
			{
				return false;
			}

			return TrashCollection.Handlers.Values.Any(h => h != null && h.CanTrash(mob, trash, message));
		}

		public override bool TryDropItem(Mobile mob, Item trashed, bool sendFullMessage)
		{
			return mob != null && trashed != null && !trashed.Deleted && IsAccessibleTo(mob) && Trash(mob, trashed) &&
				   trashed.Deleted;
		}

		public override bool OnDragDropInto(Mobile mob, Item trashed, Point3D p)
		{
			return mob != null && trashed != null && !trashed.Deleted && IsAccessibleTo(mob) && Trash(mob, trashed) &&
				   trashed.Deleted;
		}

		public override bool OnDragDrop(Mobile mob, Item trashed)
		{
			if (mob == null || trashed == null || trashed.Deleted || !IsAccessibleTo(mob))
			{
				return false;
			}

			if (!(RootParent is Mobile))
			{
				return Trash(mob, trashed) && trashed.Deleted;
			}

			mob.SendMessage(0x22, "Open the {0} to trash your items.", this.ResolveName(mob.GetLanguage()));
			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.Write(_ContainerItemID);
					goto case 0;
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
				case 1:
					_ContainerItemID = reader.ReadInt();
					goto case 0;
				case 0:
					break;
			}

			Update();
		}
	}
}