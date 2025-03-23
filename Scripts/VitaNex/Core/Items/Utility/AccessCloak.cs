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
using System.Drawing;

using Server;
using Server.Items;
#endregion

namespace VitaNex.Items
{
	public class AccessCloak : BaseCloak
	{
		private AccessLevel _AccessTemp;
		private AccessLevel _AccessMask;

		[CommandProperty(AccessLevel.GameMaster)]
		public AccessLevel AccessMask
		{
			get => _AccessMask;
			set
			{
				if (value <= _AccessTemp)
				{
					_AccessMask = value;
				}
			}
		}

		public override bool DisplayLootType => false;
		public override bool DisplayWeight => false;

		[Constructable]
		public AccessCloak()
			: this(AccessLevel.Player)
		{ }

		public AccessCloak(Mobile owner)
			: this(owner, AccessLevel.Player)
		{ }

		public AccessCloak(Mobile owner, AccessLevel mask)
			: this(mask)
		{
			_AccessTemp = owner != null ? owner.AccessLevel : AccessLevel.Player;
		}

		public AccessCloak(AccessLevel mask)
			: base(0x1515)
		{
			_AccessMask = mask;

			Name = "Access Cloak";
			Hue = Utility.RandomDyedHue();
			LootType = LootType.Blessed;
			StrRequirement = 0;
			Weight = 0;
		}

		public AccessCloak(Serial serial)
			: base(serial)
		{ }

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			return DeathMoveResult.MoveToBackpack;
		}

		public override bool OnEquip(Mobile from)
		{
			if (BlessedFor == null)
			{
				BlessedFor = from;
				_AccessTemp = BlessedFor.AccessLevel;
			}

			if (BlessedFor != from)
			{
				from.SendMessage("That does not belong to you.");
				return false;
			}

			return base.OnEquip(from);
		}

#if NEWPARENT
		public override void OnAdded(IEntity parent)
#else
		public override void OnAdded(object parent)
#endif
		{
			if (BlessedFor != null)
			{
				BlessedFor.AccessLevel = _AccessMask;

				BlessedFor.Hits = BlessedFor.HitsMax;
				BlessedFor.Stam = BlessedFor.StamMax;
				BlessedFor.Mana = BlessedFor.ManaMax;
			}

			base.OnAdded(parent);
		}

#if NEWPARENT
		public override void OnRemoved(IEntity parent)
#else
		public override void OnRemoved(object parent)
#endif
		{
			if (BlessedFor != null)
			{
				BlessedFor.AccessLevel = _AccessTemp;

				BlessedFor.Hits = BlessedFor.HitsMax;
				BlessedFor.Stam = BlessedFor.StamMax;
				BlessedFor.Mana = BlessedFor.ManaMax;
			}

			base.OnRemoved(parent);
		}

		public override void OnDelete()
		{
			if (BlessedFor != null)
			{
				BlessedFor.AccessLevel = _AccessTemp;

				BlessedFor.Hits = BlessedFor.HitsMax;
				BlessedFor.Stam = BlessedFor.StamMax;
				BlessedFor.Mana = BlessedFor.ManaMax;
			}

			base.OnDelete();
		}

		public override void AddNameProperty(ObjectPropertyList list)
		{
			base.AddNameProperty(list);

			list.Add(
				"<basefont color=#{0:X6}>Cloak: {1} => {2}<basefont color=#FFFFFF>",
				Color.Gold.ToRgb(),
				_AccessTemp,
				_AccessMask);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(BlessedFor);
					writer.WriteFlag(_AccessMask);
					writer.WriteFlag(_AccessTemp);
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
					BlessedFor = reader.ReadMobile();

					_AccessMask = reader.ReadFlag<AccessLevel>();
					_AccessTemp = reader.ReadFlag<AccessLevel>();
				}
				break;
			}
		}
	}
}