using Server;
using System;
using System.Collections.Generic;

namespace Server.Multis
{
	public class ShipTrackingContext 
	{
		private static Dictionary<Mobile, ShipTrackingContext> m_Table = new Dictionary<Mobile, ShipTrackingContext>();

		private Mobile m_Mobile;

        public List<BoatTrackingArrow> Arrows { get { return m_Arrows; } }
		private List<BoatTrackingArrow> m_Arrows = new List<BoatTrackingArrow>();

		public ShipTrackingContext(Mobile mobile, List<BoatTrackingArrow> arrows)
		{
			m_Mobile = mobile;
			m_Arrows = arrows;

			m_Table.Add(mobile, this);
		}

		public static bool RemoveContext(Mobile from)
		{
			if(!m_Table.ContainsKey(from))
			        return false;

			m_Table.Remove(from);
			return true;
		}

		public static bool CanAddContext(Mobile from)
		{
			if(!m_Table.ContainsKey(from))
				return true;

			if(m_Table[from] != null && m_Table[from].Arrows.Count > 5)
				return false;
			return true;
		}

		public static ShipTrackingContext GetContext(Mobile from)
		{
			if(!m_Table.ContainsKey(from))
				return null;

			return m_Table[from];
		}

        public bool IsTrackingBoat(Item item)
        {
            if (item is BaseBoat)
            {
                BaseBoat boat = item as BaseBoat;

                foreach (BoatTrackingArrow arrow in m_Arrows)
                {
                    if (arrow.Boat == boat)
                        return true;
                }
            }
            return false;
        }

		public void AddArrow(BoatTrackingArrow arrow)
		{
			m_Arrows.Add(arrow);
		}

        public void RemoveArrow(BoatTrackingArrow arrow)
        {
            if (m_Arrows.Contains(arrow))
                m_Arrows.Remove(arrow);

            if(m_Mobile == null)
                return;

            if (m_Arrows.Count == 0)
            {
                m_Mobile.QuestArrow = null;
                RemoveContext(m_Mobile);
            }
            else
            {
                if (m_Mobile.QuestArrow == arrow)
                    m_Mobile.QuestArrow = null;

                if (m_Arrows.Count > 0)
                    m_Mobile.QuestArrow = m_Arrows[0];
            }
        }
	}

	public class BoatTrackingArrow : QuestArrow
	{
		public static readonly int MaxRange = 200;
		public static readonly int MaxBoats = 5;

		private Mobile m_From;
		private Timer m_Timer;
        private BaseBoat m_Boat;

        public Mobile From { get { return m_From; } }
        public Timer Timer { get { return m_Timer; } }
        public BaseBoat Boat { get { return m_Boat; } }

		public BoatTrackingArrow(Mobile from, BaseBoat boat, int range) : base(from, boat)
		{
            m_Boat = boat;
            m_From = from;
			m_Timer = new BoatTrackingTimer(from, boat, range, this);
			m_Timer.Start();

            if (boat != null && from != null)
            {
                string name = "a ship with no name";
                if (boat.ShipName != null && boat.ShipName != "" && boat.ShipName != String.Empty)
                    name = boat.ShipName;

                from.SendMessage("You are now tracking the {0}.", name);
            }
		}

		public override void OnClick(bool rightClick)
		{
            if (rightClick && m_From != null)
            {
                ShipTrackingContext st = ShipTrackingContext.GetContext(m_From);

                if (st != null)
                    st.RemoveArrow(this);

                m_From = null;
                Stop();
            }
		}

		public override void OnStop()
		{
            if(m_Timer != null)
			    m_Timer.Stop();

            if(m_From != null)
                m_From.SendLocalizedMessage(503177); // You have lost your quarry.
		}

        public static void StopTracking(Mobile from)
        {
            ShipTrackingContext st = ShipTrackingContext.GetContext(from);

            if (st != null)
            {
                for (int i = 0; i < st.Arrows.Count; i++)
                    st.Arrows[i].Stop();

                ShipTrackingContext.RemoveContext(from);
            }

            from.QuestArrow = null;
        }

		public static void StartTracking(Mobile from)
		{
			if(!ShipTrackingContext.CanAddContext(from))
			{
                from.SendMessage("You are already tracking 5 boats.");
				return;
			}

			List<BaseBoat> targets = new List<BaseBoat>();
 			Map map = from.Map;
			
			if(map == null || map == Map.Internal)
				return;

			BaseBoat fromBoat = BaseBoat.FindBoatAt(from, map);
			ShipTrackingContext context = ShipTrackingContext.GetContext(from);

			if(fromBoat == null)
			{
                from.SendMessage("You must be on your boat to use this command.");
			}

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, MaxRange);
			foreach(Item item in eable)
			{
				if(context != null && context.IsTrackingBoat(item))
					continue;
				if(item is BaseBoat && (BaseBoat)item != fromBoat && !targets.Contains((BaseBoat)item))
					targets.Add((BaseBoat)item);
			}
		
			eable.Free();

			List<BoatTrackingArrow> arrows = new List<BoatTrackingArrow>();

			if(targets.Count == 0)
			{
                from.SendMessage("There are no boats in the area to track.");
                return;
			}

			for(int i = 0; i < targets.Count; i++)
			{
				if(i >= MaxBoats)
					break;

                BoatTrackingArrow arrow = new BoatTrackingArrow(from, targets[i], MaxRange);

				if(context == null)
					arrows.Add(arrow);
				else 
					context.AddArrow(arrow);
			}

            if (from.QuestArrow == null && arrows.Count > 0)
                from.QuestArrow = arrows[0];

			if(context == null)
				new ShipTrackingContext(from, arrows);
		}
	}
		
	public class BoatTrackingTimer : Timer
	{
        private Mobile m_From;
		private BaseBoat m_Target;
		private int m_Range;
		private int m_LastX, m_LastY;
		private QuestArrow m_Arrow;

		public BoatTrackingTimer(Mobile from, BaseBoat target, int range, QuestArrow arrow) : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 2.5 ) )
		{
            m_From = from;
			m_Target = target;
			m_Range = range;	
			m_Arrow = arrow;
		}

		protected override void OnTick()
		{
			if(!m_Arrow.Running)
			{
				this.Stop();
				return;
			}
			else if (m_From.NetState == null || m_From.Deleted || m_Target.Deleted || m_From.Map != m_Target.Map || !m_From.InRange(m_Target, m_Range))
			{
                    m_Arrow.Stop();
                    this.Stop();
                    return;
			}	
			
			if(m_LastX != m_Target.X || m_LastY != m_Target.Y)
			{
				m_LastX = m_Target.X;
				m_LastY = m_Target.Y;
		
				m_Arrow.Update();
			}
		}
	}
}
