#define RUNUO_2 //Comment this out to enable RunUO 1.0 Mode

using System;
using System.IO;
using System.Collections;

#if(RUNUO_2)
using System.Collections.Generic;
#endif

using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Touring
{
	public delegate void DestinationChangedEventHandler(DestinationChangedEventArgs e);

	public static partial class Tour
	{
		public static void Initialize()
		{
			DestinationConfig.Init();
			CheckGuideIntegrity();

			EventSink.Logout += new LogoutEventHandler(OnLogout);
			EventSink.Disconnected += new DisconnectedEventHandler(OnDisconnect);
			EventSink.ServerStarted += new ServerStartedEventHandler(OnLoad);
			EventSink.WorldSave += new WorldSaveEventHandler(OnSave);
		}

		private static void OnLoad()
		{
			/*string path = "\\Saves\\Touring\\";
			string fileName = "guides.bin";

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			if (!File.Exists(path + fileName))
				File.Create(path + fileName).Close();

			FileStream stream = File.OpenRead(path + fileName);

			if(stream.Length > 0)
			{
				using (BinaryFileReader reader = new BinaryFileReader(new BinaryReader(stream)))
				{
					int count = reader.ReadInt();

					for(int i = 0; i < count; i++)
					{
						int destID = reader.ReadInt();
						TourGuide guide = (TourGuide)reader.ReadMobile();

						if (guide != null && !guide.Deleted && m_Destinations[destID] != null)
						{
							Destination dest = m_Destinations[destID];

							m_GuideLocations.Add(dest, guide);
						}
					}

					reader.Close();
				}
			}

			stream.Close();*/
		}

		private static void OnSave(WorldSaveEventArgs e)
		{
			/*string path = "\\Saves\\Touring\\";
			string fileName = "guides.bin";

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			if (File.Exists(path + fileName))
				File.Delete(path + fileName);

			if (!File.Exists(path + fileName))
				File.Create(path + fileName).Close();

			FileStream stream = File.OpenWrite(path + fileName);

			using (BinaryFileWriter writer = new BinaryFileWriter(stream, true))
			{
				writer.Write(m_GuideLocations.Count);

				for (int i = 0; i < m_GuideLocations.Count; i++)
				{
					for (int destID = 0; destID < m_Destinations.Count; destID++)
					{
						if (m_Destinations[destID] != null)
						{
							Destination test = (Destination)m_Destinations[destID];

							if (m_GuideLocations.ContainsKey(test))
							{
								writer.Write(destID);
								writer.Write((Mobile)m_GuideLocations[destID]);
								break;
							}
						}
					}
				}

				writer.Close();
			}

			stream.Close();*/
		}

		public static void CheckGuideIntegrity()
		{
#if(RUNUO_2)
			List<Destination> toRemove = new List<Destination>();
#else
			ArrayList toRemove = new ArrayList();
#endif

#if(RUNUO_2)
			foreach (KeyValuePair<Destination, TourGuide> kvp in m_GuideLocations)
			{
				Destination dest = kvp.Key;
				TourGuide guide = kvp.Value;

				if (guide == null || guide.Deleted || !TourConfig.UseTourGuide)
					toRemove.Add(dest);
			}
#else
			foreach(Destination dest in m_GuideLocations.Keys)
			{
				TourGuide guide = (TourGuide)m_GuideLocations[dest];

				if (guide == null || guide.Deleted || !TourConfig.UseTourGuide)
					toRemove.Add(dest);
			}
#endif

			foreach (Destination xDest in toRemove)
			{
				if (m_GuideLocations.ContainsKey(xDest))
				{
					TourGuide guide = (TourGuide)m_GuideLocations[xDest];

					if (guide != null && !guide.Deleted)
						guide.Delete();

					m_GuideLocations.Remove(xDest);
				}
			}
		}

		public static void AddDestination(Map map, Point3D location, string name, string description, TimeSpan delay)
		{
			Destination dest = new Destination(map, location, name, description, delay);

			if (dest.IsValid())
			{
				m_Destinations.Add(dest);

				if (TourConfig.UseTourGuide)
				{
					TourGuide guide = new TourGuide(dest);

					if (guide != null && !guide.Deleted)
						m_GuideLocations.Add(dest, guide);
				}
			}
		}

		private static void OnLogout(LogoutEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
				return;

			Tour.Finish(e.Mobile);
		}

		private static void OnDisconnect(DisconnectedEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
				return;

			Tour.Finish(e.Mobile);
		}

		public static event DestinationChangedEventHandler
			DestinationChanged = new DestinationChangedEventHandler(OnDestinationChanged);

		public static void DestinationChangedInvoke(DestinationChangedEventArgs e)
		{
			try { DestinationChanged.Invoke(e); }
			catch { }
		}

		private static void OnDestinationChanged(DestinationChangedEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted)
				return;

			if (m_Stages.ContainsKey(e.Mobile))
			{
				int curStage = (int)m_Stages[e.Mobile];
				Destination curDest = e.Destination;

				if (curDest.IsValid())
				{
					if (TourConfig.UseTourGuide)
					{
						TourGuide guide = FindTourGuide(curDest);

						if (guide != null && !guide.Deleted)
						{
							guide.ShowTo(e.Mobile);
							guide.SayTo(e.Mobile, String.Format("Hier! {0}!", e.Destination.Name));
							guide.SayTo(e.Mobile, String.Format("{0}", e.Destination.Description));
						}
						else
						{
							e.Mobile.SendMessage(0x55, String.Format("Hier! {0}! {1}", e.Destination.Description));
						}
					}
					else
					{
						e.Mobile.SendMessage(0x55, String.Format("Hier! {0}! {1}", e.Destination.Description));
					}

					Timer.DelayCall(e.Destination.Delay, new TimerStateCallback(NextDestination), new object[] { e.Mobile });
				}
			}
		}


	}

	public sealed class DestinationChangedEventArgs : EventArgs
	{
		private Destination m_Destination;
		private Mobile m_Mobile;

		public Destination Destination { get { return m_Destination; } }
		public Mobile Mobile { get { return m_Mobile; } }

		public DestinationChangedEventArgs(Destination destination, Mobile m)
		{
			m_Destination = destination;
			m_Mobile = m;
		}
	}
}
