#region References
using System;

using Server.Items;
using Server.Targeting;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class HandSiegeAttack : XmlAttachment
	{
		// multiplier of weapon min/max damage used to calculate siege damage.
		private const double DamageScaleFactor = 0.5;

		// base delay in seconds between attacks.  Actual delay will be reduced by weapon speed.
		private const double BaseWeaponDelay = 9.0;

		//private readonly int _CurrentDirection;
		private Item _AttackTarget; // target of the attack
		private InternalTimer m_Timer;

		[CommandProperty(AccessLevel.GameMaster)]
		public Item AttackTarget
		{
			get { return _AttackTarget; }
			set
			{
				_AttackTarget = value;

				if (_AttackTarget != null)
				{
					// immediate attack unless already attacking
					DoTimer(TimeSpan.Zero, true);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxDistance { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D CurrentLoc { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Map CurrentMap { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Point3D TargetLoc { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Map TargetMap { get; set; }

		[Attachable]
		public HandSiegeAttack()
		{
			MaxDistance = 2;
		}

		// a serial constructor is REQUIRED
		public HandSiegeAttack(ASerial serial)
			: base(serial)
		{
			MaxDistance = 2;
		}

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments
		public static void SelectTarget(Mobile from, Item weapon)
		{
			if (from == null || weapon == null)
			{
				return;
			}

			// does this weapon have a HandSiegeAttack attachment on it already?
			HandSiegeAttack a = (HandSiegeAttack)XmlAttach.FindAttachment(weapon, typeof(HandSiegeAttack));

			if (a == null || a.Deleted)
			{
				XmlAttach.AttachTo(weapon, a = new HandSiegeAttack());
			}

			from.Target = new HandSiegeTarget(weapon, a);
		}

		public void BeginAttackTarget(Mobile from, Item target, Point3D targetloc)
		{
			if (from == null || target == null)
			{
				return;
			}

			// check the target line of sight
			Point3D adjustedloc = new Point3D(targetloc.X, targetloc.Y, targetloc.Z + target.ItemData.Height);
			Point3D fromloc = new Point3D(from.Location.X, from.Location.Y, from.Location.Z + 14);

			if (!from.Map.LineOfSight(fromloc, adjustedloc))
			{
				from.SendMessage("Cannot see target.");
				return;
			}

			int distance = (int)XmlSiege.GetDistance(from.Location, targetloc);

			if (distance <= MaxDistance)
			{
				CurrentLoc = from.Location;
				CurrentMap = from.Map;
				TargetLoc = target.Location;
				TargetMap = target.Map;

				AttackTarget = target;
			}
			else
			{
				from.SendLocalizedMessage(500446); // That is too far away.
			}
		}

		public override void OnAttach()
		{
			base.OnAttach();

			if (!(AttachedTo is Item))
			{
				Delete();
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if (m_Timer != null)
			{
				m_Timer.Stop();
			}
		}

		public void DoTimer(TimeSpan delay, bool wait)
		{
			// is there a timer already running?  Then let it finish
			if (m_Timer != null && m_Timer.Running && wait)
			{
				return;
			}

			if (m_Timer != null)
			{
				m_Timer.Stop();
			}

			m_Timer = new InternalTimer(this, delay);
			m_Timer.Start();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);

			// version 0
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			// version 0
		}

		private class HandSiegeTarget : Target
		{
			private readonly Item _Weapon;
			private readonly HandSiegeAttack _Attachment;

			public HandSiegeTarget(Item weapon, HandSiegeAttack attachment)
				: base(30, true, TargetFlags.None)
			{
				_Weapon = weapon;
				_Attachment = attachment;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (from == null || _Weapon == null || _Attachment == null || from.Map == null)
				{
					return;
				}

				if (targeted is StaticTarget)
				{
					int staticID = ((StaticTarget)targeted).ItemID;
					int staticx = ((StaticTarget)targeted).Location.X;
					int staticy = ((StaticTarget)targeted).Location.Y;

					Item multiItem = null;

					// find the possible multi owner of the static tile
					foreach (Item item in from.Map.GetItemsInRange(((StaticTarget)targeted).Location, 50))
					{
						if (!(item is BaseMulti))
						{
							continue;
						}

						// search the component list for a match
						MultiComponentList mcl = ((BaseMulti)item).Components;
						bool found = false;

						if (mcl != null && mcl.List != null)
						{
							// ReSharper disable LoopCanBeConvertedToQuery
							foreach (MultiTileEntry t in mcl.List)
							// ReSharper restore LoopCanBeConvertedToQuery
							{
								int x = t.m_OffsetX + item.X;
								int y = t.m_OffsetY + item.Y;
								//int z = t.m_OffsetZ + item.Z;
								int itemID = t.m_ItemID & 0x3FFF;

								if (itemID != staticID || x != staticx || y != staticy)
								{
									continue;
								}

								found = true;
								break;
							}
						}

						if (!found)
						{
							continue;
						}

						multiItem = item;
						break;
					}

					if (multiItem != null)
					{
						//Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
						// may have to reconsider the use tileloc vs target loc
						//m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);
						//m_weapon.AttackTarget(from, multiitem, multiitem.Map.GetPoint(targeted, true), m_checklos);
						_Attachment.BeginAttackTarget(from, multiItem, multiItem.Map.GetPoint(targeted, true));
					}
				}
				else if (targeted is AddonComponent)
				{
					// if the addon doesnt have an xmlsiege attachment, then attack the addon
					XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(targeted, typeof(XmlSiege));

					if (a == null || a.Deleted)
					{
						_Attachment.BeginAttackTarget(from, ((AddonComponent)targeted).Addon, ((Item)targeted).Location);
					}
					else
					{
						_Attachment.BeginAttackTarget(from, (Item)targeted, ((Item)targeted).Location);
					}
				}
				else if (targeted is Item)
				{
					_Attachment.BeginAttackTarget(from, (Item)targeted, ((Item)targeted).Location);
				}
			}
		}

		// added the duration timer that begins on spawning
		private class InternalTimer : Timer
		{
			private readonly HandSiegeAttack _Attachment;

			public InternalTimer(HandSiegeAttack attachment, TimeSpan delay)
				: base(delay)
			{
				Priority = TimerPriority.TwoFiftyMS;
				_Attachment = attachment;
			}

			protected override void OnTick()
			{
				if (_Attachment == null)
				{
					return;
				}

				Item weapon = _Attachment.AttachedTo as Item;
				Item target = _Attachment.AttackTarget;

				if (weapon == null || weapon.Deleted || target == null || target.Deleted)
				{
					Stop();
					return;
				}

				// the weapon must be equipped
				Mobile attacker = weapon.Parent as Mobile;

				if (attacker == null || attacker.Deleted)
				{
					Stop();
					return;
				}

				// the attacker cannot be fighting

				if (attacker.Combatant != null)
				{
					attacker.SendMessage("Cannot siege while fighting.");
					Stop();
					return;
				}

				// get the location of the attacker
				Point3D attackerLoc = attacker.Location;
				Map attackerMap = attacker.Map;

				//Point3D targetloc = target.Location;
				Map targetMap = target.Map;

				if (targetMap == null || targetMap == Map.Internal || attackerMap == null || attackerMap == Map.Internal ||
					targetMap != attackerMap)
				{
					// if the attacker or target has an invalid map, then stop
					Stop();
					return;
				}

				// compare it against previous locations.  If they have moved then break off the attack
				if (attackerLoc != _Attachment.CurrentLoc || attackerMap != _Attachment.CurrentMap)
				{
					Stop();
					return;
				}

				// attack the target
				// Animate( int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay )
				int action = 26; // 1-H bash animation, 29=2-H mounted

				// get the layer
				switch (weapon.Layer)
				{
					case Layer.OneHanded:
						// unmounted animation (left number)
						action = attacker.Mount == null ? 9 : 26;
						break;
					case Layer.TwoHanded:
						// unmounted animation (left number)
						action = attacker.Mount == null ? 12 : 29;
						break;
				}

				// attack animation
				attacker.Animate(action, 7, 1, true, false, 0);

				int baseDamage = 1;
				double baseDelay = BaseWeaponDelay;

				if (weapon is BaseWeapon)
				{
					BaseWeapon b = (BaseWeapon)weapon;

					// calculate the siege damage based on the weapon min/max damage and the overall damage scale factor
					baseDamage = (int)(Utility.RandomMinMax(b.MinDamage, b.MaxDamage) * DamageScaleFactor);

					// reduce the actual delay by the weapon speed
					baseDelay -= b.Speed / 10;
				}

				baseDelay = Math.Max(1, baseDelay);
				baseDamage = Math.Max(1, baseDamage);

				// apply siege damage, all physical
				XmlSiege.Attack(attacker, target, baseDamage, 0);

				// prepare for the next attack
				_Attachment.DoTimer(TimeSpan.FromSeconds(baseDelay), false);
			}
		}
	}
}