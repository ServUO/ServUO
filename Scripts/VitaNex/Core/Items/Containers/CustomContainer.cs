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
#endregion

namespace VitaNex.Items
{
	public abstract class CustomContainer : BaseContainer
	{
		private int _ContainerItemID;

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

				if (_ContainerItemID == value)
				{
					return;
				}

				_ContainerItemID = value;
				Update();
			}
		}

		public virtual int DefaultContainerItemID => 0xE77;

		public CustomContainer(int itemID)
			: this(itemID, 0xE76)
		{ }

		public CustomContainer(int itemID, int containerID)
			: base(itemID)
		{
			ItemID = ContainerItemID = containerID;
		}

		public CustomContainer(Serial serial)
			: base(serial)
		{ }

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

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(_ContainerItemID);
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
					_ContainerItemID = reader.ReadInt();
					break;
			}

			Update();
		}
	}
}