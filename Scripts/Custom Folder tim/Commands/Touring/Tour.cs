#define RUNUO_2 //Comment this out to enable RunUO 1.0 Mode

using System;
using System.Collections;

#if(RUNUO_2)
using System.Collections.Generic;
#endif

using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Touring
{
	public static partial class Tour
	{
#if(RUNUO_2)
		private static List<Destination> m_Destinations = new List<Destination>();
		private static Dictionary<Mobile, int> m_Stages = new Dictionary<Mobile, int>();
		private static Dictionary<Mobile, Destination> m_StartLocations = new Dictionary<Mobile, Destination>();
		private static Dictionary<Destination, TourGuide> m_GuideLocations = new Dictionary<Destination, TourGuide>();

		public static List<Destination> Destinations { get { return m_Destinations; } }
		public static Dictionary<Mobile, int> Stages { get { return m_Stages; } }
		public static Dictionary<Mobile, Destination> StartLocations { get { return m_StartLocations; } }
		public static Dictionary<Destination, TourGuide> GuideLocations { get { return m_GuideLocations; } }
#else
		private static ArrayList m_Destinations = new ArrayList();
		private static Hashtable m_Stages = new Hashtable();
		private static Hashtable m_StartLocations = new Hashtable();
		private static Hashtable m_GuideLocations = new Hashtable();

		public static ArrayList Destinations { get { return m_Destinations; } }
		public static Hashtable Stages { get { return m_Stages; } }
		public static Hashtable StartLocations { get { return m_StartLocations; } }
		public static Hashtable GuideLocations { get { return m_GuideLocations; } }
#endif

		public static void Start(Mobile m)
		{
			if (m == null || m.Deleted)
				return;

			if (m_Stages.ContainsKey(m))
				m_Stages.Remove(m);

			if (m_StartLocations.ContainsKey(m))
				m_StartLocations.Remove(m);

			m_Stages.Add(m, 0);
			m_StartLocations.Add(m, new Destination(m.Map, m.Location, m.Region == null ? "Home" : m.Region.Name, "Du wurdest zu deiner ursprünglichen Position zurückgeschickt.", TimeSpan.Zero));

			m.SendMessage(0x55, "Willkommen zu der Tour auf Utgard, Setzen Sie sich, entspannen und geniessen Sie den Ritt...");
			m.SendMessage(0x55, "Die Tour beginnt in 5 Sekunden...");

			Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerStateCallback(NextDestination), new object[] { m });
		}

		public static void Finish(Mobile m)
		{
			if (m == null || m.Deleted)
				return;

			if (m_Stages.ContainsKey(m))
				m_Stages.Remove(m);

			if (m_StartLocations.ContainsKey(m))
			{
				Destination startDest = (Destination)m_StartLocations[m];
				startDest.DoTeleport(m);

				m_StartLocations.Remove(m);
			}

			m.SendMessage(0x55, "Wir hoffen Ihnen hat die Tour gefallen!");

			m.Frozen = false;
			m.Blessed = false;
		}

		private static void NextDestination(object state)
		{
			object[] states = (object[])state;
			Mobile m = (Mobile)states[0];

			if (m == null || m.Deleted)
				return;

			if (m_Stages.ContainsKey(m))
			{
				int curStage = (int)m_Stages[m];

				if (curStage >= m_Destinations.Count)
				{
					Tour.Finish(m);
					return;
				}

				Destination dest = (Destination)m_Destinations[curStage];

				if (dest.IsValid())
					dest.DoTeleport(m);

				m_Stages[m] = (int)m_Stages[m] + 1;
			}
		}

		public static TourGuide FindTourGuide(Destination dest)
		{
			if (dest.IsValid())
			{
				if (m_GuideLocations.ContainsKey(dest))
					return (TourGuide)m_GuideLocations[dest];
			}

			TourGuide guide = new TourGuide(dest);

			if (guide != null && !guide.Deleted)
			{
				m_GuideLocations.Add(dest, guide);
				return guide;
			}

			return null;
		}
	}
}
