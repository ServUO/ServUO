#region Header
// **********
// ServUO - BaseFamiliar.cs
// **********
#endregion

#region References
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public abstract class BaseFamiliar : BaseCreature
	{
		private bool m_LastHidden;

		public BaseFamiliar()
			: base(AIType.AI_Melee, FightMode.Closest, 10, 1, -1, -1)
		{ }

		public BaseFamiliar(Serial serial)
			: base(serial)
		{ }

		public override bool BardImmune { get { return true; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override bool Commandable { get { return false; } }
		public override bool PlayerRangeSensitive { get { return false; } }

		public virtual void RangeCheck()
		{
			if (!Deleted && ControlMaster != null && !ControlMaster.Deleted)
			{
				int range = (RangeHome - 2);

				if (!InRange(ControlMaster.Location, RangeHome))
				{
					Mobile master = ControlMaster;

					Point3D m_Loc = Point3D.Zero;

					if (Map == master.Map)
					{
						int x = (X > master.X) ? (master.X + range) : (master.X - range);
						int y = (Y > master.Y) ? (master.Y + range) : (master.Y - range);

						for (int i = 0; i < 10; i++)
						{
							m_Loc.X = x + Utility.RandomMinMax(-1, 1);
							m_Loc.Y = y + Utility.RandomMinMax(-1, 1);

							m_Loc.Z = Map.GetAverageZ(m_Loc.X, m_Loc.Y);

							if (Map.CanSpawnMobile(m_Loc))
							{
								break;
							}

							m_Loc = master.Location;
						}

						if (!Deleted)
						{
							SetLocation(m_Loc, true);
						}
					}
				}
			}
		}

		public override void OnThink()
		{
			Mobile master = ControlMaster;

			if (Deleted)
			{
				return;
			}

			if (master == null || master.Deleted)
			{
				DropPackContents();
				EndRelease(null);
				return;
			}

			RangeCheck();

			if (m_LastHidden != master.Hidden)
			{
				Hidden = m_LastHidden = master.Hidden;
			}

			if (AIObject != null && AIObject.WalkMobileRange(master, 5, true, 1, 1))
			{
                if ((master.Combatant !=null) && (InRange(master.Combatant, 1)))
                {
		            Warmode = master.Warmode;
		            Combatant = master.Combatant;

		            CurrentSpeed = 0.10;
			    }

                if ((master.Combatant !=null) && (!InRange(master.Combatant, 1)))
                {
                    Warmode = false;
                    FocusMob = Combatant = null;
                    CurrentSpeed = 0.01;
                }

                if (master.Combatant == null)
                {
                    Warmode = false;
                    FocusMob = Combatant = null;
                    CurrentSpeed = 0.01;
                }
 
            }
			else
			{
				Warmode = false;
				FocusMob = Combatant = null;

				CurrentSpeed = .01;
			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			if (from.Alive && Controlled && from == ControlMaster && from.InRange(this, 14))
			{
				list.Add(new ReleaseEntry(from, this));
			}
		}

		public virtual void BeginRelease(Mobile from)
		{
			if (!Deleted && Controlled && from == ControlMaster && from.CheckAlive())
			{
				EndRelease(from);
			}
		}

		public virtual void EndRelease(Mobile from)
		{
			if (from == null || (!Deleted && Controlled && from == ControlMaster && from.CheckAlive()))
			{
				Effects.SendLocationParticles(
					EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 2100, 3, 5042, 0);
				PlaySound(0x201);
				Delete();
			}
		}

		public virtual void DropPackContents()
		{
			Map map = Map;
			Container pack = Backpack;

			if (map != null && map != Map.Internal && pack != null)
			{
				var list = new List<Item>(pack.Items);

				for (int i = 0; i < list.Count; ++i)
				{
					list[i].MoveToWorld(Location, map);
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			ValidationQueue<BaseFamiliar>.Add(this);
		}

		public void Validate()
		{
			DropPackContents();
			Delete();
		}

		private class ReleaseEntry : ContextMenuEntry
		{
			private readonly Mobile m_From;
			private readonly BaseFamiliar m_Familiar;

			public ReleaseEntry(Mobile from, BaseFamiliar familiar)
				: base(6118, 14)
			{
				m_From = from;
				m_Familiar = familiar;
			}

			public override void OnClick()
			{
				if (!m_Familiar.Deleted && m_Familiar.Controlled && m_From == m_Familiar.ControlMaster && m_From.CheckAlive())
				{
					m_Familiar.BeginRelease(m_From);
				}
			}
		}
	}
}