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
using Server.Items;
using Server.Mobiles;
using Server.Movement;
using Server.Targeting;
#endregion

namespace VitaNex.Items
{
	public class HauntedMomentoEntity : BaseCreature
	{
		private HauntedMomento _Momento;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public HauntedMomento Momento => _Momento;

		public override bool ClickTitle => false;
		public override bool AlwaysMurderer => true;
		public override bool BardImmune => true;
		public override bool BleedImmune => true;
		public override bool NoHouseRestrictions => true;
		public override bool ShowFameTitle => false;
		public override bool Unprovokable => true;
		public override bool CanMoveOverObstacles => true;
		public override bool CanOpenDoors => true;
		public override bool CanTarget => false;
		//public override bool CanDrop => false;
		public override bool Commandable => false;
		public override bool DeleteCorpseOnDeath => true;
		public override bool IsScaryToPets => true;
		public override bool IsDispellable => false;
		public override bool PlayerRangeSensitive => false;
		public override bool CanFlee => false;

		public HauntedMomentoEntity(HauntedMomento momento)
			: base(AIType.AI_Use_Default, FightMode.None, 10, 0, 0.2, 0.2)
		{
			_Momento = momento;

			Name = "?";

			Hue = 0x4001;
			BaseSoundID = 0;

			Hidden = true;
			Blessed = true;
			Tamable = false;
			CanSwim = true;

			Fame = 0;
			Karma = 0;

			VirtualArmor = 1337;

			SetStr(0);
			SetDex(0);
			SetInt(0);
			SetHits(1);
			SetDamage(0);

			SetDamageType(ResistanceType.Cold, 0);
			SetDamageType(ResistanceType.Energy, 0);
			SetDamageType(ResistanceType.Fire, 0);
			SetDamageType(ResistanceType.Physical, 0);
			SetDamageType(ResistanceType.Poison, 0);

			SetResistance(ResistanceType.Cold, 100);
			SetResistance(ResistanceType.Energy, 100);
			SetResistance(ResistanceType.Fire, 100);
			SetResistance(ResistanceType.Physical, 100);
			SetResistance(ResistanceType.Poison, 100);

			this.SetAllSkills(0, 0);

			ChangeBody();
			OnThink();
		}

		public HauntedMomentoEntity(Serial serial)
			: base(serial)
		{ }

		public static bool IsBeingLookedAt(Direction facing, Direction to)
		{
			if (facing.HasFlag(Direction.Running))
			{
				facing &= ~Direction.Running;
			}

			switch (facing)
			{
				case Direction.Up:
					return (to == Direction.Left || to == Direction.West || to == Direction.Up || to == Direction.North ||
							to == Direction.Right);
				case Direction.North:
					return (to == Direction.West || to == Direction.Up || to == Direction.North || to == Direction.Right ||
							to == Direction.East);
				case Direction.Right:
					return (to == Direction.Up || to == Direction.North || to == Direction.Right || to == Direction.East ||
							to == Direction.Down);
				case Direction.East:
					return (to == Direction.North || to == Direction.Right || to == Direction.East || to == Direction.Down ||
							to == Direction.South);
				case Direction.Down:
					return (to == Direction.Right || to == Direction.East || to == Direction.Down || to == Direction.South ||
							to == Direction.Left);
				case Direction.South:
					return (to == Direction.East || to == Direction.Down || to == Direction.South || to == Direction.Left ||
							to == Direction.West);
				case Direction.Left:
					return (to == Direction.Down || to == Direction.South || to == Direction.Left || to == Direction.West ||
							to == Direction.Up);
				case Direction.West:
					return (to == Direction.South || to == Direction.Left || to == Direction.West || to == Direction.Up ||
							to == Direction.North);
			}

			return true;
		}

		public virtual void ChangeBody()
		{
			Body = Utility.RandomList(26, 50, 146, 148, 196, 747, 748, 970);

			PlaySound(1383);
		}

		public override void OnThink()
		{
			base.OnThink();

			if (Deleted || _Momento == null || _Momento.Deleted || _Momento.Map == null || _Momento.Map == Map.Internal ||
				!(_Momento.RootParent is Mobile))
			{
				Delete();
				return;
			}

			var parent = (Mobile)_Momento.RootParent;

			if (parent.Deleted || parent.Map == null || parent.Map == Map.Internal)
			{
				Delete();
				return;
			}

			Hidden = true;

			Home = parent.Location;
			RangeHome = RangePerception;

			var map = parent.Map;

			if (Map == null || Map == Map.Internal || Map != map || !InRange(parent, 20))
			{
				MoveToWorld(parent.GetRandomPoint3D(RangeHome + (RangeHome / 2)).GetWorldTop(map), map);

				ChangeBody();
			}

			var to = parent.GetDirectionTo(this);

			var stop = (parent.InRange(this, RangeHome / 2));

			if (parent is PlayerMobile)
			{
				var hide = (IsBeingLookedAt(parent.Direction, to) || !parent.InRange(this, RangeHome + (RangeHome / 2)));

				if (hide)
				{
					((PlayerMobile)parent).VisibilityList.Update(this);
				}
				else
				{
					((PlayerMobile)parent).VisibilityList.Remove(this);
				}
			}

			if (stop)
			{
				Frozen = true;
				return;
			}

			Frozen = false;

			int x = X, y = Y, z = Z;

			Movement.Offset(GetDirectionTo(Home), ref x, ref y);

			var p = new Point3D(x, y, z).GetWorldTop(map);

			if (!map.CanFit(p, 16))
			{
				SetLocation(p, true);
			}
			else
			{
				Move(GetDirectionTo(p));
			}

			Direction = GetDirectionTo(parent);
		}

		public override void OnAfterDelete()
		{
			if (_Momento != null)
			{
				var parent = _Momento.RootParent as Mobile;

				if (parent is PlayerMobile)
				{
					((PlayerMobile)parent).VisibilityList.Remove(this);
				}

				if (_Momento.Entity == this)
				{
					_Momento.Entity = null;
				}
			}

			base.OnAfterDelete();
		}

		public override bool CanBeDamaged()
		{
			return false;
		}

		public override bool CanBeRenamedBy(Mobile from)
		{
			return false;
		}

		public override bool CanBeControlledBy(Mobile m)
		{
			return false;
		}

		public override bool CheckFlee()
		{
			return false;
		}

		protected override bool OnMove(Direction d)
		{
			return true;
		}

		public override bool OnMoveOver(Mobile m)
		{
			return true;
		}

		public override bool OnMoveOff(Mobile m)
		{
			return true;
		}

		public override bool CheckShove(Mobile shoved)
		{
			return true;
		}

		public override void OnDoubleClick(Mobile m)
		{ }

		public override void OnAosSingleClick(Mobile m)
		{ }

		public override void OnSingleClick(Mobile m)
		{ }

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			return false;
		}

		public override bool OnDragLift(Item item)
		{
			return false;
		}

		public override void OnStatsQuery(Mobile from)
		{ }

		public override void AddNameProperties(ObjectPropertyList list)
		{ }

		public override void GetProperties(ObjectPropertyList list)
		{ }

		public override void GenerateLoot()
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			writer.Write(_Momento);

			if (version < 1)
			{
				writer.Write(false);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			_Momento = reader.ReadItem<HauntedMomento>();

			if (version < 1)
			{
				reader.ReadBool();
			}
		}
	}

	public class HauntedMomento : BaseThrowableAtMobile<Mobile>
	{
		private static bool _Initialized;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public HauntedMomentoEntity Entity { get; set; }

		[Constructable]
		public HauntedMomento()
			: base(0x3679, 1)
		{
			Name = "Haunted Momento";
			Token = "Mysterious Energies Surround This Momento";

			Weight = 1.0;
			Stackable = false;
			LootType = LootType.Blessed;

			AllowCombat = false;
			TargetFlags = TargetFlags.None;

			Delivery = ThrowableAtMobileDelivery.AddToPack;
			DismountUser = false;

			ThrowSound = 0x180;
			ImpactSound = 0x181;

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.Zero;
		}

		public HauntedMomento(Serial serial)
			: base(serial)
		{ }

		public static void Initialize()
		{
			if (_Initialized)
			{
				return;
			}

			EventSink.Login += e =>
			{
				if (e.Mobile == null || e.Mobile.Deleted || e.Mobile.Backpack == null)
				{
					return;
				}

				VitaNexCore.TryCatch(
					() =>
					{
						var momentos = e.Mobile.Backpack.FindItemsByType<HauntedMomento>(true);

						momentos.ForEach(
							m =>
							{
								if (m != null && !m.Deleted)
								{
									m.InvalidateEntity();
								}
							});

						momentos.Clear();
					});
			};

			_Initialized = true;
		}

		public override bool CanThrowAt(Mobile from, Mobile target, bool message)
		{
			if (!base.CanThrowAt(from, target, message))
			{
				return false;
			}

			if (!target.Player)
			{
				if (message)
				{
					from.SendMessage(37, "You can only throw the {0} at other players.", Name);
				}

				return false;
			}

			return true;
		}

		public override void OnLocationChange(Point3D oldLocation)
		{
			base.OnLocationChange(oldLocation);

			InvalidateEntity();
		}

		protected virtual HauntedMomentoEntity CreateEntity()
		{
			return new HauntedMomentoEntity(this);
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			InvalidateEntity();

			if (Entity != null)
			{
				Entity.ChangeBody();
			}
		}

#if NEWPARENT
		public override void OnAdded(IEntity parent)
#else
		public override void OnAdded(object parent)
#endif
		{
			base.OnAdded(parent);

			InvalidateEntity();
		}

		protected virtual void InvalidateEntity()
		{
			if (Map == null || Map == Map.Internal || !(RootParent is Mobile) || Parent is BankBox)
			{
				if (Entity != null)
				{
					Entity.Delete();
					Entity = null;
				}
			}
			else if (Entity == null || Entity.Deleted)
			{
				Entity = CreateEntity();
			}
		}

		public override void OnAfterDelete()
		{
			if (Entity != null)
			{
				Entity.Delete();
			}

			base.OnAfterDelete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(Entity);
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
					Entity = reader.ReadMobile<HauntedMomentoEntity>();
					break;
			}
		}
	}
}
