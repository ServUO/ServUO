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
using Server.Mobiles;
#endregion

namespace VitaNex.Items
{
	public abstract class FloorTile<TMob> : Item
		where TMob : Mobile
	{
		public override double DefaultWeight => 0;
		public override bool DisplayWeight => false;
		public override bool DisplayLootType => false;
		public override bool Decays => false;

		public Func<TMob, bool> MoveOverHandler { get; set; }
		public Func<TMob, bool> MoveOffHandler { get; set; }

		public FloorTile()
			: this(null, null)
		{ }

		public FloorTile(Func<TMob, bool> moveOver, Func<TMob, bool> moveOff)
			: base(9272)
		{
			MoveOverHandler = moveOver;
			MoveOffHandler = moveOff;

			Name = "Floor Tile";
			Movable = false;
			Visible = false;
		}

		public FloorTile(Serial serial)
			: base(serial)
		{ }

		public override sealed bool OnMoveOver(Mobile m)
		{
			if (m == null || m.Deleted || !(m is TMob))
			{
				return base.OnMoveOver(m);
			}

			return OnMoveOver(m as TMob);
		}

		public virtual bool OnMoveOver(TMob mob)
		{
			if (base.OnMoveOver(mob))
			{
				if (MoveOverHandler != null)
				{
					return MoveOverHandler(mob);
				}

				return true;
			}

			return false;
		}

		public override sealed bool OnMoveOff(Mobile m)
		{
			if (m == null || m.Deleted || !(m is TMob))
			{
				return base.OnMoveOff(m);
			}

			return OnMoveOff(m as TMob);
		}

		public virtual bool OnMoveOff(TMob mob)
		{
			if (base.OnMoveOff(mob))
			{
				if (MoveOffHandler != null)
				{
					return MoveOffHandler(mob);
				}

				return true;
			}

			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public abstract class RestrictMobileTile<TMob> : FloorTile<TMob>
		where TMob : Mobile
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Restricted { get; set; }

		public RestrictMobileTile()
			: this(null, null)
		{ }

		public RestrictMobileTile(Func<TMob, bool> moveOver, Func<TMob, bool> moveOff)
			: base(moveOver, moveOff)
		{
			Restricted = true;
			Name = "Restrict Mobiles";
		}

		public RestrictMobileTile(Serial serial)
			: base(serial)
		{ }

		public override bool OnMoveOver(TMob m)
		{
			if (base.OnMoveOver(m))
			{
				return !Restricted;
			}

			return false;
		}

		public override bool OnMoveOff(TMob m)
		{
			if (base.OnMoveOff(m))
			{
				return !Restricted;
			}

			return false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(Restricted);
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
					Restricted = reader.ReadBool();
					break;
			}
		}
	}

	public class RestrictMobileTile : RestrictMobileTile<Mobile>
	{
		[Constructable]
		public RestrictMobileTile()
			: this(null, null)
		{ }

		public RestrictMobileTile(Func<Mobile, bool> moveOver, Func<Mobile, bool> moveOff)
			: base(moveOver, moveOff)
		{
			Name = "Restrict Mobiles";
		}

		public RestrictMobileTile(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class RestrictCreatureTile : RestrictMobileTile<BaseCreature>
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowControlled { get; set; }

		[Constructable]
		public RestrictCreatureTile()
			: this(null, null)
		{ }

		public RestrictCreatureTile(Func<BaseCreature, bool> moveOver, Func<BaseCreature, bool> moveOff)
			: base(moveOver, moveOff)
		{
			AllowControlled = true;
			Name = "Restrict Creatures";
		}

		public RestrictCreatureTile(Serial serial)
			: base(serial)
		{ }

		public override bool OnMoveOver(BaseCreature m)
		{
			if (!base.OnMoveOver(m))
			{
				if (m.Controlled && AllowControlled)
				{
					return true;
				}

				return false;
			}

			return true;
		}

		public override bool OnMoveOff(BaseCreature m)
		{
			if (!base.OnMoveOff(m))
			{
				if (m.Controlled && AllowControlled)
				{
					return true;
				}

				return false;
			}

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(AllowControlled);
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
					AllowControlled = reader.ReadBool();
					break;
			}
		}
	}

	public class RestrictPlayerTile : RestrictMobileTile<PlayerMobile>
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowDead { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowYoung { get; set; }

		[Constructable]
		public RestrictPlayerTile()
			: this(null, null)
		{ }

		public RestrictPlayerTile(Func<PlayerMobile, bool> moveOver, Func<PlayerMobile, bool> moveOff)
			: base(moveOver, moveOff)
		{
			AllowYoung = true;
			AllowDead = true;
			Name = "Restrict Players";
		}

		public RestrictPlayerTile(Serial serial)
			: base(serial)
		{ }

		public override bool OnMoveOver(PlayerMobile m)
		{
			if (m.Player && m.AccessLevel > AccessLevel.Counselor)
			{
				return true;
			}

			if (!base.OnMoveOver(m))
			{
				if (!m.Alive && AllowDead)
				{
					return true;
				}

				if (m.Young && AllowYoung)
				{
					return true;
				}

				return false;
			}

			return true;
		}

		public override bool OnMoveOff(PlayerMobile m)
		{
			if (m.Player && m.AccessLevel > AccessLevel.Counselor)
			{
				return true;
			}

			if (!base.OnMoveOff(m))
			{
				if (!m.Alive && AllowDead)
				{
					return true;
				}

				if (m.Young && AllowYoung)
				{
					return true;
				}

				return false;
			}

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(AllowDead);
					writer.Write(AllowYoung);
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
					AllowDead = reader.ReadBool();
					AllowYoung = reader.ReadBool();
				}
				break;
			}
		}
	}
}