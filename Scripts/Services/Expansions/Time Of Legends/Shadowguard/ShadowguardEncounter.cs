using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Engines.PartySystem;
using System.Linq;

namespace Server.Engines.Shadowguard
{
    [PropertyObject]
	public abstract class ShadowguardEncounter
	{
        [CommandProperty(AccessLevel.GameMaster)]
		public ShadowguardController Controller { get { return ShadowguardController.Instance; } }

		public Rectangle2D[] Bounds { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowguardRegion Region { get { return Instance.Region; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D StartLoc { get { return Def.StartLoc; } }

        public Point3D[] SpawnPoints { get { return Def.SpawnPoints; } }
        public Rectangle2D[] SpawnRecs { get { return Def.SpawnRecs; } }

        [CommandProperty(AccessLevel.GameMaster)]
		public EncounterType Encounter { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public bool DoneWarning { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public DateTime StartTime { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public Mobile PartyLeader { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowguardInstance Instance { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceExpire
        {
            get { return false; }
            set
            {
                if (value)
                    Expire();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceComplete
        {
            get { return false; }
            set
            {
                if (value)
                    CompleteEncounter();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseAddon Addon { get; set; }

        public abstract Type AddonType { get; }
        public EncounterDef Def { get { return Defs[Encounter]; } }

        public virtual TimeSpan EncounterDuration { get { return TimeSpan.FromMinutes(30); } }
        public virtual TimeSpan ResetDuration { get { return TimeSpan.FromSeconds(60); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get { return Controller != null && Controller.Encounters.Contains(this); } }

		public ShadowguardEncounter(EncounterType encounter, ShadowguardInstance instance = null)
		{
			Encounter = encounter;
            Instance = instance;

            if(instance != null)
                instance.Encounter = this;
		}

        public override string ToString()
        {
            return Encounter.ToString();
        }

        public int PartySize()
        {
            if (PartyLeader == null || this.Region == null)
                return 0;

            int inRegion = this.Region.GetPlayerCount();

            if (inRegion > 0)
                return inRegion;

            Party p = Party.Get(PartyLeader);

            if (p == null)
                return 1;

            return p.Members.Count;
        }

        public void OnBeforeBegin(Mobile m)
        {
            PartyLeader = m;
            StartTime = DateTime.UtcNow;

            DoneWarning = false;
            Setup();

            SendPartyMessage(1156185);
            /*Please wait while your Shadowguard encounter is prepared. 
            Do no leave the area or logoff during this time. You will 
            teleported when the encounter is ready and if you leave 
            there is no way to enter once the encounter begins.*/
            CheckAddon();

            Timer.DelayCall(ShadowguardController.ReadyDuration, OnBeginEncounter);
        }

        public void CheckAddon()
        {
            Type addon = AddonType;

            if (addon == null)
                return;

            BaseAddon ad = Controller.Addons.FirstOrDefault(a => a.GetType() == addon && a.Map == Map.Internal);

            if (ad == null)
            {
                ad = Activator.CreateInstance(addon) as BaseAddon;
                Controller.Addons.Add(ad);
            }

            ad.MoveToWorld(new Point3D(Instance.Center.X - 1, Instance.Center.Y - 1, Instance.Center.Z), Map.TerMur);
            Addon = ad;
        }

        public void OnBeginEncounter()
        {
            AddPlayers(PartyLeader);

            SendPartyMessage(1156251, 0x20);
            //There is a 30 minute time limit for each encounter. You will receive a time limit warning at 5 minutes.
        }

		public void AddPlayers(Mobile m)
		{
			if(m == null || !m.Alive || !m.InRange(Controller.Location, 25) || m.NetState == null)
			{
				Reset(true);
			}
			else
			{
				Party p = Party.Get(m);
				
				if(p != null)
				{
                    ColUtility.ForEach(
                        p.Members.Where(
                        info => info.Mobile.Alive && 
                            Controller.Lobby.Contains(new Point2D(info.Mobile.X, info.Mobile.Y)) && 
                            info.Mobile.NetState != null), 
                        info => AddPlayer(info.Mobile));
				}
				
				AddPlayer(m);
			}
		}

        protected void SendPartyMessage(int cliloc, int hue = 0x3B2)
        {
            if (PartyLeader == null)
                return;

            Party p = Party.Get(PartyLeader);

            if (p != null)
                p.Members.ForEach(info => info.Mobile.SendLocalizedMessage(cliloc, null, hue));
            else
                PartyLeader.SendLocalizedMessage(cliloc, null, hue);
        }
		
		public void AddPlayer(Mobile m)
		{
            Point3D p = StartLoc;
            ConvertOffset(ref p);

			MovePlayer(m, p);
            m.CloseGump(typeof(ShadowguardGump));
		}
		
		public void DoWarning()
		{
            ColUtility.ForEach(this.Region.GetEnumeratedMobiles().Where(m => m is PlayerMobile), m =>
			{
				m.SendLocalizedMessage(1156252); // You have 5 minutes remaining in the encounter!
			});
			
			DoneWarning = true;
		}
		
		public virtual void Expire(bool message = true)
		{
			if(message) 
			{
                ColUtility.ForEach(this.Region.GetEnumeratedMobiles().Where(m => m is PlayerMobile), m =>
				{
					m.SendLocalizedMessage(1156253, "", 0x32); // The encounter timer has expired!
				});
			}
			
			Timer.DelayCall(TimeSpan.FromSeconds(5), () =>
                {
                    Reset(true);
                });
		}

        private bool _Completed;
		
		public virtual void CompleteEncounter()
		{
            if (_Completed)
                return;

			Timer.DelayCall(ResetDuration, () =>
                {
                    Reset();
                });

            if (this is RoofEncounter)
                SendPartyMessage(1156250); // Congratulations! You have bested Shadowguard and prevented Minax from exploiting the Time Gate! You will be teleported out in a few minutes.
            else
                SendPartyMessage(1156244); //You have bested this tower of Shadowguard! You will be teleported out of the tower in 60 seconds!

            _Completed = true;
		}
		
		public virtual void Reset(bool expired = false)
		{
            if (!Active)
                return;

            Controller.OnEncounterComplete(this, expired);

            RemovePlayers();
            PartyLeader = null;

            ClearItems();

            if (Addon != null)
            {
                Addon.Internalize();
                Addon = null;
            }

            Instance.ClearRegion();
            Instance.CompleteEncounter();
		}
		
		private void RemovePlayers()
		{
            ColUtility.ForEach(Region.GetEnumeratedMobiles().Where(
                m => m is PlayerMobile || 
                    (m is BaseCreature && 
                    ((BaseCreature)m).GetMaster() is PlayerMobile)),
                m =>
			    {
				    MovePlayer(m, Controller.KickLocation, false);
			    });
		}

        public static void MovePlayer(Mobile m, Point3D p, bool pets = true)
        {
            if (pets)
                BaseCreature.TeleportPets(m, p, m.Map);

            m.MoveToWorld(p, m.Map);
            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
            m.PlaySound(0x1FE);
        }

        public virtual void OnTick()
        {
        }
		
		public virtual void ClearItems()
		{
		}
		
		public virtual void Setup()
		{
		}
		
		public virtual void CheckEncounter()
		{
		}

        public virtual void OnCreatureKilled(BaseCreature bc)
        {
        }
		
		public void CheckPlayerStatus(Mobile m)
		{
			if(m is PlayerMobile)
			{
				Party p = Party.Get(m);
                bool aliveandconnectedandpresent = false;

				if(p != null)
				{
                    foreach (PartyMemberInfo info in p.Members)
					{
                        if (info.Mobile.Alive && info.Mobile.NetState != null && ShadowguardController.GetEncounter(m.Location, m.Map) == this)
						{
							aliveandconnectedandpresent = true;
						}
					}
				}

                if (!aliveandconnectedandpresent)
				{
					Expire(false);

					if(p != null)
						p.Members.ForEach(info => info.Mobile.SendLocalizedMessage(1156267)); // All members of your party are dead, have logged off, or have chosen to exit Shadowguard. You will be removed from the encounter shortly.
                    else
                        m.SendLocalizedMessage(1156267); // All members of your party are dead, have logged off, or have chosen to exit Shadowguard. You will be removed from the encounter shortly.
				}
			}
		}

        public void ConvertOffset(ref Point3D p)
        {
            p = new Point3D(Instance.Center.X + p.X, Instance.Center.Y + p.Y, Instance.Center.Z + p.Z); 
        }

        public void ConvertOffset(ref Rectangle2D rec)
        {
            rec = new Rectangle2D(Instance.Center.X + rec.X, Instance.Center.Y + rec.Y, rec.Width, rec.Height);
        }
		
		public virtual void Serialize(GenericWriter writer)
		{
			writer.Write(0);

            writer.Write(Instance.Index);
			writer.Write(PartyLeader);
            writer.Write(Addon);
		}
		
		public virtual void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

            Instance = Controller.Instances[reader.ReadInt()];
			PartyLeader = reader.ReadMobile();
            Addon = reader.ReadItem() as BaseAddon;

            if (Instance != null)
                Instance.Encounter = this;

			StartTime = DateTime.UtcNow;
		}

        public static Dictionary<EncounterType, EncounterDef> Defs { get; set; }

        public static void Configure()
        {
            Defs = new Dictionary<EncounterType, EncounterDef>();

            Defs[EncounterType.Bar] = new EncounterDef(
                            new Point3D(0, 0, 0),
                            new Point3D[] { new Point3D(-16, 8, 0), new Point3D(-16, 4, 0), new Point3D(-16, -6, 0), new Point3D(-16, -10, 0) },
                            new Rectangle2D[] { new Rectangle2D(-15, -12, 1, 8), new Rectangle2D(-15, 2, 1, 8) });

            Defs[EncounterType.Orchard] = new EncounterDef(
                            new Point3D(0, 0, 0),
                            new Point3D[] { new Point3D(-10, -11, 0), new Point3D(-18, -15, 0), new Point3D(-11, -19, 0), new Point3D(-17, -10, 0),
                                            new Point3D(-21, 10, 0), new Point3D(-17, 16, 0), new Point3D(-13, 12, 0), new Point3D(-11, 18, 0),
                                            new Point3D(10, -20, 0), new Point3D(10, -11, 0), new Point3D(14, -15, 0), new Point3D(17, -10, 0),
                                            new Point3D(10, 10, 0), new Point3D(9, 16, 0), new Point3D(13, 16, 0), new Point3D(15, 10, 0)},
                            new Rectangle2D[] { });

            Defs[EncounterType.Armory] = new EncounterDef(
                            new Point3D(0, 0, 0),
                            new Point3D[] { new Point3D(5, -7, 0), new Point3D(5, -9, 0), new Point3D(5, -11, 0), new Point3D(5, -13, 0), 
                                            new Point3D(5, -17, 0), new Point3D(5, -19, 0), new Point3D(5, -21, 0), new Point3D(5, 16, 0),
                                            new Point3D(5, 18, 0), new Point3D(5, 11, 0), new Point3D(5, 9, 0), 
                                            new Point3D(-23, -10, 0), new Point3D(-20, -15, 0), new Point3D(-16, -19, 0),

                                            new Point3D(9, 5, 0), new Point3D(11, 5, 0), new Point3D(16, 5, 0), new Point3D(18, 5, 0),
                                            new Point3D(-21, 5, 0), new Point3D(-19, 5, 0), new Point3D(-17, 5, 0), new Point3D(-12, 5, 0),
                                            new Point3D(-10, 5, 0), new Point3D(-8, 5, 0), new Point3D(-23, 5, 0), 
                                            new Point3D(-18, -17, 0), new Point3D(-10, -23, 0), new Point3D(-13, -21, 0)},
                            new Rectangle2D[] { new Rectangle2D(-25, -24, 18, 18), new Rectangle2D(-25, 4, 18, 18), new Rectangle2D(4, 20, 18, 18), new Rectangle2D(4, -6, 18, 18), });

            Defs[EncounterType.Fountain] = new EncounterDef(
                            new Point3D(11, 11, 0),
                            new Point3D[] { new Point3D(-6, 7, 0), new Point3D(5, 7, 0), new Point3D(7, 5, 0), new Point3D(7, -6, 0) },
                            new Rectangle2D[] { new Rectangle2D(-24, 8, 45, 17), new Rectangle2D(-24, -25, 45, 16), new Rectangle2D(-25, -8, 16, 15), new Rectangle2D(8, -8, 16, 15 ),
                                                new Rectangle2D(12, -4, 2, 6), new Rectangle2D(-4, 12, 6, 2)});

            Defs[EncounterType.Belfry] = new EncounterDef(
                            new Point3D(15, 1, 0),
                            new Point3D[] { new Point3D(0, 0, 22), new Point3D(-5, -5, 22) },
                            new Rectangle2D[] { new Rectangle2D(8, -9, 15, 15), new Rectangle2D(-24, -9, 15, 18) });

            Defs[EncounterType.Roof] = new EncounterDef(
                            new Point3D(-8, -8, 0),
                            new Point3D[] { new Point3D(0, 0, 30) },
                            new Rectangle2D[] { });
        }

        public static ShadowguardEncounter ConstructEncounter(EncounterType type)
        {
            switch (type)
            {
                default:
                case EncounterType.Bar: return new BarEncounter();
                case EncounterType.Orchard: return new OrchardEncounter();
                case EncounterType.Armory: return new ArmoryEncounter();
                case EncounterType.Fountain: return new FountainEncounter();
                case EncounterType.Belfry: return new BelfryEncounter();
                case EncounterType.Roof: return new RoofEncounter();
            }
        }
	}

    public class EncounterDef
    {
        public Point3D StartLoc { get; private set; }
        public Point3D[] SpawnPoints { get; private set; }
        public Rectangle2D[] SpawnRecs { get; private set; }

        public EncounterDef(Point3D start, Point3D[] points, Rectangle2D[] recs)
        {
            StartLoc = start;
            SpawnPoints = points;
            SpawnRecs = recs;
        }
    }
}