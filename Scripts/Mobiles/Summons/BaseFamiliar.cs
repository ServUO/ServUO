#region Header
// **********
// ServUO - BaseFamiliar.cs
// **********
#endregion

#region References
using System.Collections.Generic;
using System;
using Server.ContextMenus;
using Server.Items;
using Server.Spells.Necromancy;
#endregion

namespace Server.Mobiles
{
	public abstract class BaseFamiliar : BaseCreature
	{
		private bool m_LastHidden;
        private long m_NextMove;

        private DateTime m_SeperationStart;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime SeperationStart
        {
            get { return m_SeperationStart; }
            set { m_SeperationStart = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override IDamageable Combatant
        {
            get { return null; }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override OrderType ControlOrder
        {
            get { return OrderType.Come; }
            set { }
        }

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

        public virtual bool RangeCheck()
        {
            Mobile master = ControlMaster;

            if (Deleted || master == null || master.Deleted)
                return false;

            int dist = (int)master.GetDistanceToSqrt(Location);

            if (master.Map != Map || dist > 15)
            {
                if (m_SeperationStart == DateTime.MinValue)
                {
                    m_SeperationStart = DateTime.UtcNow + TimeSpan.FromMinutes(60);
                }
                else if (m_SeperationStart < DateTime.UtcNow)
                {
                    Delete();
                }

                return false;
            }

            if (m_SeperationStart != DateTime.MinValue)
            {
                m_SeperationStart = DateTime.MinValue;
            }

            int range = (RangeHome / 2);

            if (!InRange(ControlMaster.Location, RangeHome))
            {
                Point3D loc = Point3D.Zero;

                if (Map == master.Map)
                {
                    int x = (X > master.X) ? (master.X + range) : (master.X - range);
                    int y = (Y > master.Y) ? (master.Y + range) : (master.Y - range);

                    for (int i = 0; i < 10; i++)
                    {
                        loc.X = x + Utility.RandomMinMax(-1, 1);
                        loc.Y = y + Utility.RandomMinMax(-1, 1);

                        loc.Z = Map.GetAverageZ(loc.X, loc.Y);

                        if (Map.CanSpawnMobile(loc))
                        {
                            break;
                        }

                        loc = master.Location;
                    }

                    if (!Deleted)
                    {
                        SetLocation(loc, true);
                    }
                }
            }

            return true;
        }

		public override void OnThink()
		{
            if (Deleted || Map == null)
			{
				return;
			}

            Mobile master = ControlMaster;

			if (master == null || master.Deleted)
			{
				DropPackContents();
				EndRelease(null);
				return;
			}

			if (m_LastHidden != master.Hidden)
			{
				Hidden = m_LastHidden = master.Hidden;
			}

            if (RangeCheck())
            {
                if (AIObject != null && AIObject.WalkMobileRange(master, 5, true, 1, 1))
                {
                    if (master.Combatant != null && master.InRange(master.Combatant, 1) && Core.TickCount > m_NextMove)
                    {
                        IDamageable combatant = master.Combatant;

                        if (!InRange(combatant.Location, 1))
                        {
                            for (int x = combatant.X - 1; x <= combatant.X + 1; x++)
                            {
                                for (int y = combatant.Y - 1; y <= combatant.Y + 1; y++)
                                {
                                    if (x == combatant.X && y == combatant.Y)
                                    {
                                        continue;
                                    }

                                    Point2D p = new Point2D(x, y);

                                    if (InRange(p, 1) && master.InRange(p, 1) && Map != null)
                                    {
                                        CurrentSpeed = .01;
                                        AIObject.MoveTo(new Point3D(x, y, Map.GetAverageZ(x, y)), false, 0);
                                        m_NextMove = Core.TickCount + 500;
                                    }
                                }
                            }
                        }
                        else
                        {
                            CurrentSpeed = .1;
                        }
                    }
                    else if (master.Combatant == null)
                    {
                        CurrentSpeed = .1;
                    }
                }
                else
                {
                    CurrentSpeed = .1;
                }
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

        public static void OnHit(Mobile attacker, IDamageable defender)
        {
            BaseCreature check = (BaseCreature)SummonFamiliarSpell.Table[attacker];

            if (check != null && check is BaseFamiliar && check.Weapon != null && check.InRange(defender.Location, check.Weapon.MaxRange))
            {
                check.Weapon.OnSwing(check, defender);
            }
        }

        public static void OnLogout(PlayerMobile pm)
        {
            if (pm == null)
                return;

            BaseCreature check = (BaseCreature)SummonFamiliarSpell.Table[pm];

            if (check != null)
                check.Delete();
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