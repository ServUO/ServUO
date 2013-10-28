#region Header
// **********
// ServUO - BaseModule.cs
// **********
#endregion

#region References
using System;

using Server;
using Server.Gumps;
#endregion

namespace CustomsFramework
{
	public class BaseModule : SaveData, ICustomsEntity, ISerializable
	{
		private Mobile _LinkedMobile;
		private Item _LinkedItem;
		private DateTime _CreatedTime;
		private DateTime _LastEditedTime;

		public BaseModule()
		{ }

		public BaseModule(Mobile from)
		{
			LinkMobile(from);
		}

		public BaseModule(Item item)
		{
			LinkItem(item);
		}

		public BaseModule(CustomSerial serial)
			: base(serial)
		{ }

		public override string Name { get { return @"Base Module"; } }

		public virtual string Description { get { return "Base Module, inherit from this class and override all interface items."; } }

		public virtual string Version { get { return "1.0"; } }

		public virtual AccessLevel EditLevel { get { return AccessLevel.Developer; } }

		public virtual Gump SettingsGump { get { return null; } }

		[CommandProperty(AccessLevel.Administrator)]
		public Mobile LinkedMobile { get { return _LinkedMobile; } set { LinkMobile(value); } }

		[CommandProperty(AccessLevel.Administrator)]
		public Item LinkedItem { get { return _LinkedItem; } set { LinkItem(value); } }

		[CommandProperty(AccessLevel.Administrator)]
		public DateTime CreatedTime { get { return _CreatedTime; } }

		[CommandProperty(AccessLevel.Administrator)]
		public DateTime LastEditedTime { get { return _LastEditedTime; } }

		public override string ToString()
		{
			return Name;
		}

		public override void Prep()
		{ }

		public override void Delete()
		{
			if (_LinkedMobile != null)
			{
				_LinkedMobile.Modules.Remove(this);
				_LinkedMobile = null;
			}

			if (_LinkedItem != null)
			{
				_LinkedItem.Modules.Remove(this);
				_LinkedItem = null;
			}
		}

		public void Update()
		{
			_LastEditedTime = DateTime.UtcNow;
		}

		public bool LinkMobile(Mobile from)
		{
			if (_LinkedMobile != null || _LinkedMobile == from)
			{
				return false;
			}
			else
			{
				if (!from.Modules.Contains(this))
				{
					from.Modules.Add(this);
				}

				_LinkedMobile = from;
				Update();
				return true;
			}
		}

		public bool LinkItem(Item item)
		{
			if (_LinkedItem != null || _LinkedItem == item)
			{
				return false;
			}
			else
			{
				if (!item.Modules.Contains(this))
				{
					item.Modules.Add(this);
				}

				_LinkedItem = item;
				Update();
				return true;
			}
		}

		public bool UnlinkMobile()
		{
			if (_LinkedMobile == null)
			{
				return false;
			}
			else
			{
				if (_LinkedMobile.Modules.Contains(this))
				{
					_LinkedMobile.Modules.Remove(this);
				}

				_LinkedMobile = null;
				Update();
				return true;
			}
		}

		public bool UnlinkItem()
		{
			if (_LinkedItem == null)
			{
				return false;
			}
			else
			{
				if (_LinkedItem.Modules.Contains(this))
				{
					_LinkedItem.Modules.Remove(this);
				}

				_LinkedItem = null;
				Update();
				return true;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			writer.WriteVersion(0);

			// Version 0
			writer.Write(_LinkedMobile);
			writer.Write(_LinkedItem);
			writer.Write(_CreatedTime);
			writer.Write(_LastEditedTime);
		}

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						LinkedMobile = reader.ReadMobile();
						LinkedItem = reader.ReadItem();
						_CreatedTime = reader.ReadDateTime();
						_LastEditedTime = reader.ReadDateTime();
						break;
					}
			}
		}
	}
}