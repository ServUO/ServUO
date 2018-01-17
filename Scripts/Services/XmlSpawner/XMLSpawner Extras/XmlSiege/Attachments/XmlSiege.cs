using System;
using Server;
using Server.Items;
using Server.Network;
using System.Collections;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlSiege : XmlAttachment
	{
		private const int LightDamageColor = 165; // color at 67-99% of hitsmax
		private const int MediumDamageColor = 53; // color at 34-66% of hitsmax
		private const int HeavyDamageColor = 33; // color at 1-33% of hitsmax
		private const int ShowSiegeColor = 0; // color used to flag items with siege attachments

		public virtual int LightDamageEffectID { get { return 14732; } } // 14732 = flame effect
		public virtual int MediumDamageEffectID { get { return 14732; } }
		public virtual int HeavyDamageEffectID { get { return 14732; } }

		public virtual int DamagedItemEffectDuration { get { return 110; } }

		public virtual bool UseEffectsDamageIndicator { get { return true; } }	// show damage using location effects
		public virtual bool UseColorDamageIndicator { get { return false; } }	// show damage using item rehueing

		public virtual int WhenToAutoRepair { get { return 0; } }  // 0=never, 1=after any damage, 2=after being destroyed
		public virtual double AutoRepairFactor { get { return 0.1; } } // fraction of HitsMax to repair on each autorepair OnTick. A value of 1 will fully repair.

		private int m_Hits = 1000;		// current hits
		private int m_HitsMax = 1000;	// max hits
		private int m_ResistFire = 30;	// percentage resistance to fire attacks
		private int m_ResistPhysical = 30;	// percentage resistance to physical attacks
		private int m_Stone = 1;	// amount of stone required per repair
		private int m_Iron = 20;	// amount of iron required per repair
		private int m_Wood = 20;	// amount of wood required per repair
		private int m_DestroyedItemID = 10984;	// itemid used when hits go to zero. 2322=dirt patch, 10984 pulsing pool.  Specifying a value of zero will cause the object to be permanently destroyed.
		private TimeSpan m_AutoRepairTime = TimeSpan.Zero;  // autorepair disabled by default

		private bool m_Enabled = true; // allows enabling/disabling siege damage and its effects
		private DateTime m_AutoRepairEnd;
		private AutoRepairTimer m_AutoRepairTimer;
		public bool NeedsEffectsUpdate = true;
		private ArrayList m_OriginalItemIDList = new ArrayList();		// original itemids of parent item
		private ArrayList m_OriginalHueList = new ArrayList();		// original hues of parent item
		public bool BeingRepaired;
		private EffectsTimer m_EffectsTimer;

		private class TileEntry
		{
			public int ID;
			public int X;
			public int Y;
			public int Z;

			public TileEntry(int id, int x, int y, int z)
			{
				ID = id;
				X = x;
				Y = y;
				Z = z;
			}
		}

		public static void SendMovingProjectileEffect(IEntity from, IEntity to, int itemID, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue)
		{
			if (from is Mobile)
				((Mobile)from).ProcessDelta();

			if (to is Mobile)
				((Mobile)to).ProcessDelta();

			Effects.SendPacket(from.Location, from.Map, new MovingProjectileEffect(from, to, itemID, fromPoint, toPoint, speed, duration, fixedDirection, explode, hue));
		}

		public class MovingProjectileEffect : HuedEffect
		{
			public MovingProjectileEffect(IEntity from, IEntity to, int itemID, Point3D fromPoint, Point3D toPoint, int speed, int duration, bool fixedDirection, bool explode, int hue)
				: base(EffectType.Moving, from.Serial, to == null ? (Serial)(-1) : to.Serial, itemID, fromPoint, toPoint, speed, duration, fixedDirection, explode, hue, 0)
			{
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan AutoRepairTime
		{
			get { return m_AutoRepairTime; }
			set
			{
				m_AutoRepairTime = value;

				// see if the object is already destroyed
				if ((Hits == 0 && WhenToAutoRepair == 2) || (Hits < HitsMax && WhenToAutoRepair == 1))
				{
					DoAutoRepairTimer(value);
				}
			}
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextAutoRepair
		{
			get
			{
				if (m_AutoRepairTimer != null && m_AutoRepairTimer.Running)
					return m_AutoRepairEnd - DateTime.UtcNow;
				else
					return TimeSpan.FromSeconds(0);
			}
			set
			{
				DoAutoRepairTimer(value);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool Enabled
		{
			get {
				// disable outside of Felucca
				//if (AttachedTo is Item && ((Item)AttachedTo).Map != Map.Felucca) return false;
				return m_Enabled; 
			}
			set
			{
				m_Enabled = value;
				AdjustItemID();
				UpdateDamageIndicators();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int DestroyedItemID
		{
			get { return m_DestroyedItemID; }
			set
			{
				if (value >= 0)
				{
					m_DestroyedItemID = value;
					if (Hits == 0)
						AdjustItemID();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Iron
		{
			get { return m_Iron; }
			set
			{
				if (value >= 0) m_Iron = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Stone
		{
			get { return m_Stone; }
			set
			{
				if (value >= 0) m_Stone = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Wood
		{
			get { return m_Wood; }
			set
			{
				if (value >= 0) m_Wood = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int ResistFire
		{
			get { return m_ResistFire; }
			set
			{
				if (value >= 0) m_ResistFire = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int ResistPhysical
		{
			get { return m_ResistPhysical; }
			set
			{
				if (value >= 0) m_ResistPhysical = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int HitsMax
		{
			get { return m_HitsMax; }
			set
			{
				if (value >= 0 && value != HitsMax)
				{
					m_HitsMax = value;
					if (m_Hits > m_HitsMax)
					{
						Hits = m_HitsMax;
					}
					// recalibrate damage indicators
					UpdateDamageIndicators();
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Hits
		{
			get { return m_Hits; }
			set
			{
				int newvalue = value;
				int oldvalue = m_Hits;

				if (newvalue > HitsMax) newvalue = HitsMax;
				if (newvalue < 0) newvalue = 0;

				if (oldvalue != newvalue)
				{
					m_Hits = newvalue;

					// it is being destroyed 
					if (newvalue == 0)
					{
						OnDestroyed();
					}
					else
						// it was destroyed and is being repaired
						if (oldvalue == 0)
						{
							// if autorepair was active then stop it
							if (m_AutoRepairTimer != null)
								m_AutoRepairTimer.Stop();

							// restore the itemids from the destroyed state
							AdjustItemID();

							// it is being restored from destroyed state so also refresh nearby mobile locations
							// which may have to be changed do to the new itemids
							AdjustMobileLocations();
						}

					// if it has taken damage and the autorepair feature is set to repair on damage
					// then start the autorepair timer
					if (WhenToAutoRepair == 1 && AutoRepairTime > TimeSpan.Zero && m_Hits != HitsMax)
					{
						DoAutoRepairTimer(AutoRepairTime);
					}

					// adjust the damage indicators on change in hits
					UpdateDamageIndicators();
				}
			}
		}

		public void DoAutoRepairTimer(TimeSpan delay)
		{
			m_AutoRepairEnd = DateTime.UtcNow + delay;

			if (m_AutoRepairTimer != null)
				m_AutoRepairTimer.Stop();

			m_AutoRepairTimer = new AutoRepairTimer(this, delay);
			m_AutoRepairTimer.Start();
		}

		private class AutoRepairTimer : Timer
		{
			private XmlSiege m_attachment;

			public AutoRepairTimer(XmlSiege attachment, TimeSpan delay)
				: base(delay)
			{
				Priority = TimerPriority.FiveSeconds;
				m_attachment = attachment;
			}

			protected override void OnTick()
			{
				if (m_attachment != null && !m_attachment.Deleted)
				{
					// incrementally repair the object.  This will also restart the timer if not fully repaired and WhenToAutoRepair is set to repair on damage.
					int repair = (int)(m_attachment.HitsMax * m_attachment.AutoRepairFactor);
					if (repair == 0) repair = 1;

					m_attachment.Hits += repair;
				}
			}
		}

		public void DoEffectsTimer(TimeSpan delay)
		{

			if (m_EffectsTimer != null)
				m_EffectsTimer.Stop();

			m_EffectsTimer = new EffectsTimer(this, delay);
			m_EffectsTimer.Start();
		}

		private class EffectsTimer : Timer
		{
			private XmlSiege m_Siege;

			public EffectsTimer(XmlSiege siege, TimeSpan delay)
				: base(delay)
			{

				Priority = TimerPriority.OneSecond;

				m_Siege = siege;
			}

			protected override void OnTick()
			{
				if (m_Siege != null && !m_Siege.Deleted && m_Siege.NeedsEffectsUpdate)
				{
					m_Siege.AdjustDamageEffects();

					int nplayers = 0;
					if (m_Siege.AttachedTo is Item)
					{
						// check to see if anyone is around
                        IPooledEnumerable eable = ((Item)m_Siege.AttachedTo).GetMobilesInRange(24);

						foreach (Mobile p in eable)
						{
							if (p.Player /*&& p.AccessLevel == AccessLevel.Player */)
							{
								nplayers++;
								break;
							}
						}
                        eable.Free();
					}

					// if not, the no need to update
					if (nplayers == 0)
					{
						m_Siege.NeedsEffectsUpdate = false;
					}
				}

			}
		}

		public override bool HandlesOnMovement { get { return (Hits != HitsMax) && Enabled; } }

		public override void OnMovement(MovementEventArgs e)
		{
			base.OnMovement(e);

			if (UseEffectsDamageIndicator)
			{
				NeedsEffectsUpdate = true;

				// if the effects timer is not running
				if (m_EffectsTimer == null || !m_EffectsTimer.Running)
				{
					// then update effects damage display
					AdjustDamageEffects();

				}
			}
		}

		public static void Attack(Mobile from, object target, int firedamage, int physicaldamage)
		{
			// find the XmlSiege attachment on the target
			XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(target, typeof(XmlSiege));

			if (a != null && !a.Deleted)
			{
				a.ApplyScaledDamage(from, firedamage, physicaldamage);
			}
		}

		public static int GetHits(object target)
		{
			// find the XmlSiege attachment on the target
			XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(target, typeof(XmlSiege));

			if (a != null && !a.Deleted)
			{
				return a.Hits;
			}

			return -1;
		}

		public static int GetHitsMax(object target)
		{
			// find the XmlSiege attachment on the target
			XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(target, typeof(XmlSiege));

			if (a != null && !a.Deleted)
			{
				return a.HitsMax;
			}

			return -1;
		}

		public virtual void ApplyScaledDamage(Mobile from, int firedamage, int physicaldamage)
		{
			if (!Enabled || Hits == 0) return;

			int firescale = 100 - ResistFire;
			int physicalscale = 100 - ResistPhysical;
			if (firescale < 0) firescale = 0;
			if (physicalscale < 0) physicalscale = 0;

			int scaleddamage = (firedamage * firescale + physicaldamage * physicalscale) / 100;

			// subtract the scaled damage from the current hits
			Hits -= scaleddamage;

			Item i = AttachedTo as Item;

			if (i != null)
			{
				// display the damage over the target

				// if it is an addon and invisible, then try displaying over a visible component
				if (i is BaseAddon && !i.Visible && ((BaseAddon)i).Components != null && ((BaseAddon)i).Components.Count > 0)
				{
					foreach (AddonComponent c in ((BaseAddon)i).Components)
					{
						if (c != null && c.Visible)
						{
							c.PublicOverheadMessage(MessageType.Regular, 33, true, String.Format("{0}", scaleddamage));
							break;
						}
					}
				}
				else
				{
					i.PublicOverheadMessage(MessageType.Regular, 33, true, String.Format("{0}", scaleddamage));
				}

				if (from != null)
				{
					from.SendMessage("You deliver {0} siege damage.", scaleddamage);
				}
			}
		}

		public virtual void OnDestroyed()
		{
			// change the itemid to reflect destroyed state
			AdjustItemID();

			// if autorepair is enabled, then start the autorepair timer
			if (WhenToAutoRepair > 0 && AutoRepairTime > TimeSpan.Zero)
			{
				DoAutoRepairTimer(AutoRepairTime);
			}
		}

		public static double GetDistance(Point3D p1, Point3D p2)
		{
			int xDelta = p1.X - p2.X;
			int yDelta = p1.Y - p2.Y;

			return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}

		private void AdjustItemID()
		{
			Item targetitem = AttachedTo as Item;

			if (targetitem == null || targetitem.Deleted) return;

			if (m_Hits == 0 && Enabled)
			{
				if (m_DestroyedItemID == 0)
				{
					// blow it up
					Effects.SendLocationEffect(targetitem, targetitem.Map, 0x36B0, 16, 1);
					Effects.PlaySound(targetitem, targetitem.Map, 0x11D);

					// and permanently destroy it
					targetitem.Delete();
				}
				else
				{
					if (targetitem is BaseMulti)
					{
						// map it into a valid multi id
						targetitem.ItemID = m_DestroyedItemID | 0x4000;
					}
					else
					{
						// change the target item id
						targetitem.ItemID = m_DestroyedItemID;
					}

					// deal with addons
					if (targetitem is BaseAddon)
					{
						BaseAddon addon = (BaseAddon)targetitem;
						if (addon.Components != null)
						{
							// change the ids of all of the components
							foreach (AddonComponent i in addon.Components)
							{
								i.ItemID = m_DestroyedItemID;
							}
						}
					}
				}
			}
			else
			{
				RestoreOriginalItemID(targetitem);
			}

		}

		public void RestoreOriginalItemID(Item targetitem)
		{
			if (targetitem == null || targetitem.Deleted) return;

			if (m_OriginalItemIDList != null && m_OriginalItemIDList.Count > 0)
			{
				targetitem.ItemID = (int)m_OriginalItemIDList[0];
				if (targetitem is BaseAddon)
				{
					BaseAddon addon = (BaseAddon)targetitem;
					if (addon.Components != null)
					{
						int j = 1;
						foreach (AddonComponent i in addon.Components)
						{
							if (j < m_OriginalItemIDList.Count)
							{
								i.ItemID = (int)m_OriginalItemIDList[j++];
							}
						}
					}
				}
			}
		}

		public void StoreOriginalItemID(Item targetitem)
		{
			if (targetitem == null || targetitem.Deleted) return;

			m_OriginalItemIDList = new ArrayList();

			m_OriginalItemIDList.Add(targetitem.ItemID);

			if (targetitem is BaseAddon)
			{
				BaseAddon addon = (BaseAddon)targetitem;
				if (addon.Components != null)
				{
					foreach (AddonComponent i in addon.Components)
					{
						m_OriginalItemIDList.Add(i.ItemID);
					}
				}
			}
		}

		public virtual void AssignItemHue(Item targetitem, int hue)
		{
			if (targetitem == null || targetitem.Deleted) return;

			// change the target item hue
			targetitem.Hue = hue;

			// deal with addons
			if (targetitem is BaseAddon)
			{
				BaseAddon addon = (BaseAddon)targetitem;
				if (addon.Components != null)
				{
					// change the ids of all of the components if they dont already have xmlsiege attachments
					foreach (AddonComponent i in addon.Components)
					{
						if (XmlAttach.FindAttachment(i, typeof(XmlSiege)) == null)
						{
							i.Hue = hue;
						}
					}
				}
			}
		}

		public virtual void AssignItemEffect(Item targetitem, int effectid, int hue, int fraction)
		{
			if (targetitem == null || targetitem.Deleted) return;

			// deal with addons
			if (targetitem is BaseAddon)
			{
				BaseAddon addon = (BaseAddon)targetitem;
				if (addon.Components != null)
				{
					int count = 0;
					// change the ids of all of the components if they dont already have xmlsiege attachments
					foreach (AddonComponent i in addon.Components)
					{
						if (XmlAttach.FindAttachment(i, typeof(XmlSiege)) == null)
						{
							// put the effect on a fraction of the components, but make sure you have at least one
							if (Utility.Random(100) < fraction || count == 0)
							{
								Effects.SendLocationEffect(i.Location, i.Map, effectid, DamagedItemEffectDuration, hue, 0);
								//Effects.SendTargetEffect(i, DamagedItemEffectID, DamagedItemEffectDuration, hue, 0);
								count++;
							}
						}
					}
				}
			}
			else
				if (targetitem is BaseMulti)
				{
					// place an effect at the location of the target item
					Effects.SendLocationEffect(targetitem.Location, targetitem.Map, effectid, DamagedItemEffectDuration, hue, 0);

					ArrayList tilelist = new ArrayList();
					// go through all of the multi components
					MultiComponentList mcl = ((BaseMulti)targetitem).Components;
					int count = 0;
					if (mcl != null && mcl.List != null)
					{
						for (int i = 0; i < mcl.List.Length; i++)
						{
							MultiTileEntry t = mcl.List[i];

							int x = t.m_OffsetX + targetitem.X;
							int y = t.m_OffsetY + targetitem.Y;
							int z = t.m_OffsetZ + targetitem.Z;
							int itemID = t.m_ItemID & 0x3FFF;

							if (Utility.Random(100) < fraction || count == 0)
							{
								tilelist.Add(new TileEntry(itemID, x, y, z));
								count++;
							}
						}

						foreach (TileEntry s in tilelist)
						{
							Effects.SendLocationEffect(new Point3D(s.X, s.Y, s.Z), targetitem.Map, effectid, DamagedItemEffectDuration, hue, 0);
						}

					}
				}
				else
				{
					// place an effect at the location of the target item
					Effects.SendLocationEffect(targetitem.Location, targetitem.Map, effectid, DamagedItemEffectDuration, hue, 0);
					//Effects.SendTargetEffect(targetitem, DamagedItemEffectID, DamagedItemEffectDuration, hue, 0);
				}
		}


		public void RestoreOriginalHue(Item targetitem)
		{
			if (targetitem == null || targetitem.Deleted) return;

			if (m_OriginalHueList != null && m_OriginalHueList.Count > 0)
			{
				targetitem.Hue = (int)m_OriginalHueList[0];
				if (targetitem is BaseAddon)
				{
					BaseAddon addon = (BaseAddon)targetitem;
					if (addon.Components != null)
					{
						int j = 1;
						foreach (AddonComponent i in addon.Components)
						{
							if (j < m_OriginalHueList.Count)
							{
								i.Hue = (int)m_OriginalHueList[j++];
							}
						}
					}
				}
			}
		}

		public void StoreOriginalHue(Item targetitem)
		{
			if (targetitem == null || targetitem.Deleted) return;

			m_OriginalHueList = new ArrayList();

			m_OriginalHueList.Add(targetitem.Hue);

			if (targetitem is BaseAddon)
			{
				BaseAddon addon = (BaseAddon)targetitem;
				if (addon.Components != null)
				{
					foreach (AddonComponent i in addon.Components)
					{
						m_OriginalHueList.Add(i.Hue);
					}
				}
			}
		}

		public virtual void UpdateDamageIndicators()
		{
			// add colored effects at the item location to reflect damage
			if (UseEffectsDamageIndicator)
			{
				AdjustDamageEffects();
			}

			// set the item hue to reflect damage
			if (UseColorDamageIndicator)
			{
				AdjustDamageColor();
			}
		}

		public virtual void AdjustDamageEffects()
		{
			Item targetitem = AttachedTo as Item;

			if (targetitem == null) return;

			// set the color based on damage
			if (Hits == HitsMax || !Enabled)
			{
				// no effects
			}
			else
			{
				// start the timer and apply effects if a timer is not already running
				if (m_EffectsTimer == null || !m_EffectsTimer.Running)
				{
					DoEffectsTimer(TimeSpan.FromSeconds(5.0));


					// linear scaling of effects density 1-30% based on damage
					int density = 29 * (HitsMax - Hits) / HitsMax + 1;

					if (Hits < HitsMax && Hits > HitsMax * 0.66)
					{
						AssignItemEffect(targetitem, LightDamageEffectID, LightDamageColor, density);
					}
					else
						if (Hits <= HitsMax * 0.66 && Hits > HitsMax * 0.33)
						{
							AssignItemEffect(targetitem, MediumDamageEffectID, MediumDamageColor, density);
						}
						else
							if (Hits <= HitsMax * 0.33)
							{
								AssignItemEffect(targetitem, HeavyDamageEffectID, HeavyDamageColor, density);
							}
				}
			}

		}

		public virtual void AdjustDamageColor()
		{
			Item targetitem = AttachedTo as Item;

			if (targetitem == null) return;

			// set the color based on damage
			if (Hits == HitsMax || !Enabled)
			{
				RestoreOriginalHue(targetitem);
			}
			else
				if (Hits < HitsMax && Hits > HitsMax * 0.66)
				{
					AssignItemHue(targetitem, LightDamageColor);
				}
				else
					if (Hits <= HitsMax * 0.66 && Hits > HitsMax * 0.33)
					{
						AssignItemHue(targetitem, MediumDamageColor);
					}
					else
						if (Hits <= HitsMax * 0.33)
						{
							AssignItemHue(targetitem, HeavyDamageColor);
						}
		}

		private void AdjustMobileLocations()
		{
			Item targetitem = AttachedTo as Item;

			if (targetitem == null) return;

			// make sure nearby mobiles are in valid locations
			ArrayList mobilelist = new ArrayList();
            IPooledEnumerable eable = targetitem.GetMobilesInRange(0);

			foreach (Mobile p in eable)
			{
				mobilelist.Add(p);
			}
            eable.Free();

			if (targetitem is BaseAddon)
			{
				BaseAddon addon = (BaseAddon)targetitem;
				if (addon.Components != null)
				{
					foreach (AddonComponent i in addon.Components)
					{
						if (i != null)
						{
                            IPooledEnumerable eable2 = i.GetMobilesInRange(0);
							foreach (Mobile p in eable2)
							{
								if (!mobilelist.Contains(p))
									mobilelist.Add(p);
							}
                            eable2.Free();
						}
					}
				}
			}

			if (targetitem is BaseMulti && targetitem.Map != null)
			{
				// check all locations covered by the multi
				// go through all of the multi components
				MultiComponentList mcl = ((BaseMulti)targetitem).Components;

				if (mcl != null && mcl.List != null)
				{
					for (int i = 0; i < mcl.List.Length; i++)
					{
						MultiTileEntry t = mcl.List[i];

						int x = t.m_OffsetX + targetitem.X;
						int y = t.m_OffsetY + targetitem.Y;
						int z = t.m_OffsetZ + targetitem.Z;
                        IPooledEnumerable eable2 = targetitem.Map.GetMobilesInRange(new Point3D(x, y, z), 0);

						foreach (Mobile p in eable2)
						{
							if (!mobilelist.Contains(p))
								mobilelist.Add(p);
						}
                        eable2.Free();

					}
				}
			}

			// relocate all mobiles found
			foreach (Mobile p in mobilelist)
			{
				if (p != null && p.Map != null)
				{
					int x = p.Location.X;
					int y = p.Location.Y;
					int z = p.Location.Z;

					// check the current location
					if (!p.Map.CanFit(x, y, z, 16, true, false, true))
					{
						bool found = false;


						for (int dx = 0; dx <= 10 && !found; dx++)
						{
							for (int dy = 0; dy <= 10 && !found; dy++)
							{
								// try moving it up in z to find a valid spot
								for (int h = 1; h <= 39; h++)
								{
									if (p.Map.CanFit(x + dx, y + dy, z + h, 16, true, false, true))
									{
										z += h;
										x += dx;
										y += dy;
										found = true;
										break;
									}
								}
							}

						}
					}

					// move them to the new location
					p.MoveToWorld(new Point3D(x, y, z), p.Map);

				}
			}


		}

		// These are the various ways in which the message attachment can be constructed.  
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlSiege(ASerial serial)
			: base(serial)
		{
		}

		[Attachable]
		public XmlSiege()
		{
		}

		[Attachable]
		public XmlSiege(int hitsmax)
		{
			HitsMax = hitsmax;
			m_Hits = HitsMax;
		}

		[Attachable]
		public XmlSiege(int hitsmax, int destroyeditemid)
		{
			HitsMax = hitsmax;
			m_Hits = HitsMax;
			DestroyedItemID = destroyeditemid;
		}

		[Attachable]
		public XmlSiege(int hitsmax, int resistfire, int resistphysical)
		{
			HitsMax = hitsmax;
			m_Hits = HitsMax;
			ResistPhysical = resistphysical;
			ResistFire = resistfire;
		}

		[Attachable]
		public XmlSiege(int hitsmax, int resistfire, int resistphysical, int destroyeditemid)
		{
			HitsMax = hitsmax;
			m_Hits = HitsMax;
			ResistPhysical = resistphysical;
			ResistFire = resistfire;
			DestroyedItemID = destroyeditemid;
		}

		[Attachable]
		public XmlSiege(int hitsmax, int resistfire, int resistphysical, int wood, int iron, int stone)
		{
			HitsMax = hitsmax;
			m_Hits = HitsMax;
			ResistPhysical = resistphysical;
			ResistFire = resistfire;
			Wood = wood;
			Iron = iron;
			Stone = stone;
		}

		[Attachable]
		public XmlSiege(int hitsmax, int resistfire, int resistphysical, int wood, int iron, int stone, int destroyeditemid)
		{
			HitsMax = hitsmax;
			m_Hits = HitsMax;
			ResistPhysical = resistphysical;
			ResistFire = resistfire;
			Wood = wood;
			Iron = iron;
			Stone = stone;
			DestroyedItemID = destroyeditemid;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)2);
			// version 2
			writer.Write(m_AutoRepairTime);
			if (m_AutoRepairEnd > DateTime.UtcNow)
			{
				writer.Write(m_AutoRepairEnd - DateTime.UtcNow);
			}
			else
			{
				writer.Write(TimeSpan.Zero);
			}
			// version 1
			writer.Write(m_Enabled);
			// version 0
			writer.Write(m_Hits);
			writer.Write(m_HitsMax);
			writer.Write(m_ResistFire);
			writer.Write(m_ResistPhysical);
			writer.Write(m_Stone);
			writer.Write(m_Iron);
			writer.Write(m_Wood);
			writer.Write(m_DestroyedItemID);
			if (m_OriginalItemIDList != null)
			{
				writer.Write(m_OriginalItemIDList.Count);
				for (int i = 0; i < m_OriginalItemIDList.Count; i++)
				{
					writer.Write((int)m_OriginalItemIDList[i]);
				}
			}
			else
			{
				writer.Write((int)0);
			}
			if (m_OriginalHueList != null)
			{
				writer.Write(m_OriginalHueList.Count);
				for (int i = 0; i < m_OriginalHueList.Count; i++)
				{
					writer.Write((int)m_OriginalHueList[i]);
				}
			}
			else
			{
				writer.Write((int)0);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 2:
					m_AutoRepairTime = reader.ReadTimeSpan();
					TimeSpan delay = reader.ReadTimeSpan();
					if (delay > TimeSpan.Zero)
					{
						DoAutoRepairTimer(delay);
					}
					goto case 1;
				case 1:
					m_Enabled = reader.ReadBool();
					goto case 0;
				case 0:
					// version 0
					m_Hits = reader.ReadInt();
					m_HitsMax = reader.ReadInt();
					m_ResistFire = reader.ReadInt();
					m_ResistPhysical = reader.ReadInt();
					m_Stone = reader.ReadInt();
					m_Iron = reader.ReadInt();
					m_Wood = reader.ReadInt();
					m_DestroyedItemID = reader.ReadInt();
					int count = reader.ReadInt();
					for (int i = 0; i < count; i++)
					{
						m_OriginalItemIDList.Add(reader.ReadInt());
					}
					count = reader.ReadInt();
					for (int i = 0; i < count; i++)
					{
						m_OriginalHueList.Add(reader.ReadInt());
					}
					break;
			}

			// force refresh of the Enabled status
			Enabled = m_Enabled;
		}

		public override void OnDelete()
		{
			base.OnDelete();

			// restore the original itemid and color
			if (AttachedTo is Item)
			{
				RestoreOriginalItemID((Item)AttachedTo);
				RestoreOriginalHue((Item)AttachedTo);
			}
		}

		public override void OnAttach()
		{
			base.OnAttach();

			if (AttachedTo is Item)
			{
				StoreOriginalItemID((Item)AttachedTo);
				StoreOriginalHue((Item)AttachedTo);
				// temporarily adjust hue to indicate attachment
				((Item)AttachedTo).Hue = ShowSiegeColor;
			}
			else
				Delete();
		}

		public override string OnIdentify(Mobile from)
		{
			if (from == null || from.AccessLevel == AccessLevel.Player) return null;

			if (Expiration > TimeSpan.Zero)
			{
				return String.Format("XmlSiege: Hits {0} expires in {1} mins", Hits, Expiration.TotalMinutes);
			}
			else
			{
				return String.Format("XmlSiege: Hits {0} out of {1}", Hits, HitsMax);
			}
		}
	}
}
