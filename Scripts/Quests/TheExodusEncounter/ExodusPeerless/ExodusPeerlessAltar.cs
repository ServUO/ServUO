using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Mobiles;
using Server.Network;
using System.Linq;
using Server.ContextMenus;

namespace Server.Items
{
    public abstract class PeerlessExodusAltar : Item
    {
        public virtual TimeSpan DelayExit { get { return TimeSpan.FromMinutes(10); } }	
        public abstract BaseExodusPeerless Boss { get; }
				
        private BaseExodusPeerless m_Peerless;
        private Point3D m_BossLocation;
        private Point3D m_TeleportDest;
        private int m_Lifespan;
        private Timer m_Timer;

        public virtual int Lifespan { get { return 840; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseExodusPeerless Peerless
        {
            get
            {
                return this.m_Peerless;
            }
            set
            {
                this.m_Peerless = value;
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BossLocation
        {
            get
            {
                return this.m_BossLocation;
            }
            set
            {
                this.m_BossLocation = value;
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D TeleportDest
        {
            get
            {
                return this.m_TeleportDest;
            }
            set
            {
                this.m_TeleportDest = value;
            }
        }

        private List<Mobile> m_Fighters;
        private Dictionary<Mobile, List<Mobile>> m_Pets;
        public static List<RitualArray> m_Rituals;

        public List<Mobile> Fighters { get { return this.m_Fighters; } }		
        public Dictionary<Mobile, List<Mobile>> Pets { get { return this.m_Pets; } }       
        public List<RitualArray> Rituals { get { return m_Rituals; } }	
        public Mobile Summoner { get { return this.m_Fighters[0]; } }
	
        public PeerlessExodusAltar(int itemID)
            : base(itemID)
        {
            this.Movable = false;
			
            this.m_Fighters = new List<Mobile>();
            this.m_Pets = new Dictionary<Mobile, List<Mobile>>();            
            m_Rituals = new List<RitualArray>();

            if (this.Lifespan > 0)
            {
                this.m_Lifespan = this.Lifespan;
                this.StartTimer();
            }
        }

        public PeerlessExodusAltar(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.Lifespan > 0)
            {
                TimeSpan t = TimeSpan.FromSeconds(this.m_Lifespan);
                int minutes = t.Minutes;

                if (minutes > 0)
                    list.Add(string.Format("Lifespan: {0} {1}", minutes, minutes == 1 ? "minute" : "minutes"));
                else
                    list.Add(1072517, this.m_Lifespan.ToString()); // Lifespan: ~1_val~ seconds
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimeLeft
        {
            get
            {
                return this.m_Lifespan;
            }
            set
            {
                this.m_Lifespan = value;
                this.InvalidateProperties();
            }
        }

        public virtual void StartTimer()
        {
            if (this.m_Timer != null)
                return;

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), new TimerCallback(Slice));
            this.m_Timer.Priority = TimerPriority.OneSecond;
        }

        public virtual void StopTimerAltar()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;
        }

        public virtual void Slice()
        {
            this.m_Lifespan -= 10;

            this.InvalidateProperties();

            if (this.m_Lifespan <= 0)
                this.Decay();
        }

        public virtual void Decay()
        {
            if (this.RootParent is Mobile)
            {
                Mobile parent = (Mobile)this.RootParent;

                if (this.Name == null)
                    parent.SendLocalizedMessage(1072515, "#" + this.LabelNumber); // The ~1_name~ expired...
                else
                    parent.SendLocalizedMessage(1072515, this.Name); // The ~1_name~ expired...

                Effects.SendLocationParticles(EffectItem.Create(parent.Location, parent.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(parent.Location, parent.Map, 0x201);
            }
            else
            {
                Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(this.Location, this.Map, 0x201);
            }            

            this.StopTimerAltar();
            this.Delete();
        }

        private class BeginTheRitual : ContextMenuEntry
        {
            private Mobile m_Mobile;
            private PeerlessExodusAltar m_altar;

            public BeginTheRitual(PeerlessExodusAltar altar, Mobile from) : base(1153608, 3) // Begin the Ritual
            {
                m_Mobile = from;
                m_altar = altar;

                if (m_Rituals != null)
                    if (!AllRitualCheck())
                        Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                m_altar.SendConfirmationsExodus(m_Mobile);
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (m_Rituals != null && m_Rituals.First().RitualMobile == from)
                list.Add(new BeginTheRitual(this, from));
        }             

        public static bool AllRitualCheck()
        {
            int alllistcount = m_Rituals.Count();

            int ritual1list = m_Rituals.Count(n => n.Ritual1 == true);
            int ritual2list = m_Rituals.Count(n => n.Ritual2 == true);

            if (alllistcount > 1 && alllistcount == ritual1list && alllistcount == ritual2list)
                return true;

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.Delete();
        }

        private int toConfirm;

        public virtual void AddFighter(Mobile fighter, bool confirmed)
        {
            if (confirmed)
                this.AddFighter(fighter);

            this.toConfirm -= 1;

            if (this.toConfirm == 0)
                this.BeginSequence(this.Summoner);
        }

        public virtual void AddFighter(Mobile fighter)
        {
            this.m_Fighters.Add(fighter);		
				
            foreach (Mobile m in fighter.GetMobilesInRange(5))
            {
                if (m is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)m;
					
                    if (pet.Controlled && pet.ControlMaster == fighter)
                    {
                        if (!this.m_Pets.ContainsKey(fighter))
                            this.m_Pets.Add(fighter, new List<Mobile>());
							
                        this.m_Pets[fighter].Add(pet);
                    }
                }
            }
			
            if (fighter.Mounted)
            {
                if (!this.m_Pets.ContainsKey(fighter))
                    this.m_Pets.Add(fighter, new List<Mobile>());
				
                if (fighter.Mount is Mobile)
                    this.m_Pets[fighter].Add((Mobile)fighter.Mount);						
            }
        }

        public virtual void SendConfirmationsExodus(Mobile from)
        {
            if (m_Rituals != null && m_Rituals.Count != 0)
            {
                this.toConfirm = 0;

                foreach (var ritualmobiles in m_Rituals)
                {
                    if (ritualmobiles.RitualMobile.InRange(from.Location, 5) && this.CanEnter(ritualmobiles.RitualMobile))
                    {
                        if (from == ritualmobiles.RitualMobile && ritualmobiles.Ritual1 && ritualmobiles.Ritual2)
                        {
                            this.AddFighter(from);
                        }
                        else if (ritualmobiles.Ritual1 && ritualmobiles.Ritual2)
                        {
                            this.toConfirm += 1;

                            this.AddFighter(ritualmobiles.RitualMobile, true);
                        }
                    }
                    else
                        from.SendLocalizedMessage(1153611);
                }            
            }
        }

        public static bool CanBossCheck()
        {
            foreach (Mobile m in World.Mobiles.Values)
            {
                Region r = m.Region;

                if (r.IsPartOf("Ver Lor Reg"))
                    if (m is ClockworkExodus)
                        return true;
            }

            return false;
        }

        public virtual void BeginSequence(Mobile from)
        { 
            if (this.m_Peerless == null)
            {
                if (!CanBossCheck())
                {
                    // spawn boss
                    this.m_Peerless = this.Boss;

                    if (this.m_Peerless != null)
                    {
                        this.m_Peerless.Home = this.m_BossLocation;
                        this.m_Peerless.RangeHome = 4;
                        this.m_Peerless.MoveToWorld(this.m_BossLocation, Map.Ilshenar);
                        this.m_Peerless.Altar = this;
                    }
                    else
                        return;
                }
            }
				
            // teleport figters
            for (int i = 0; i < this.m_Fighters.Count; i ++)
            {
                Mobile fighter = this.m_Fighters[i];
                int counter = 1;
				
                if (from.InRange(fighter.Location, 5) && this.CanEnter(fighter))
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(counter), new TimerStateCallback(Enter_Callback), fighter);
											
                    counter += 1;
                }
            }
        }
		
        private void Enter_Callback(object state)
        {
            if (state is Mobile)
                this.Enter((Mobile)state);
        }
		
        public virtual void Enter(Mobile fighter)
        { 
            if (this.CanEnter(fighter))
            {
                // teleport party member's pets
                if (this.m_Pets.ContainsKey(fighter))
                {
                    for (int i = 0; i < this.m_Pets[fighter].Count; i ++)
                    {
                        BaseCreature pet = this.m_Pets[fighter][i] as BaseCreature;
						
                        if (pet != null && pet.Alive && pet.InRange(fighter.Location, 5) && !(pet is BaseMount && ((BaseMount)pet).Rider != null) && this.CanEnter(pet))
                        { 
                            pet.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                            pet.PlaySound(0x1FE);
                            pet.MoveToWorld(this.m_TeleportDest, Map.Ilshenar);
                        }
                    }
                }
				
                // teleport party member
                fighter.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                fighter.PlaySound(0x1FE);
                fighter.MoveToWorld(this.m_TeleportDest, Map.Ilshenar);

                Timer.DelayCall(TimeSpan.FromMinutes(1), new TimerCallback(AltarDelete));
            }
        }

        public void AltarDelete()
        {
            this.Delete();
        }


        public virtual bool CanEnter(Mobile fighter)
        {
	        return fighter != null && !Deleted && Map != null && Map != Map.Internal;
        }

	    public virtual bool CanEnter(BaseCreature pet)
	    {
		    return pet != null && !Deleted && Map != null && Map != Map.Internal;
	    }   		

        public class ExitTimer : Timer
        {
            private static TimeSpan m_Delay = TimeSpan.FromMinutes(2);
            private static TimeSpan m_Warning = TimeSpan.FromMinutes(8);

            public ExitTimer() : base(m_Warning)
            {
            }
            protected override void OnTick()
            {
                SendMessage(1010589);

                Timer.DelayCall(m_Delay, new TimerCallback(VerLorRegExit));                               
            }
        }

        public static void VerLorRegExit()
        {
            foreach (Mobile m in World.Mobiles.Values)
            {
                Region r = m.Region;

                if (r.IsPartOf("Ver Lor Reg") && m.IsPlayer() && !CanBossCheck())
                {
                    switch (Utility.Random(8))
                    {
                        case 0:
                            {
                                m.MoveToWorld(new Point3D(1217, 469, -13), m.Map); // Compassion
                                break;
                            }
                        case 1:
                            {
                                m.MoveToWorld(new Point3D(720, 1356, -60), m.Map); // Honesty
                                break;
                            }
                        case 2:
                            {
                                m.MoveToWorld(new Point3D(748, 728, -29), m.Map); // Honor
                                break;
                            }
                        case 3:
                            {
                                m.MoveToWorld(new Point3D(287, 1016, 0), m.Map); // Humility
                                break;
                            }
                        case 4:
                            {
                                m.MoveToWorld(new Point3D(987, 1007, -35), m.Map); // Justice
                                break;
                            }
                        case 5:
                            {
                                m.MoveToWorld(new Point3D(1175, 1287, -30), m.Map); // Sacrifice
                                break;
                            }
                        case 6:
                            {
                                m.MoveToWorld(new Point3D(1532, 1341, -3), m.Map); // Spirituality
                                break;
                            }
                        case 7:
                            {
                                m.MoveToWorld(new Point3D(527, 218, -44), m.Map); // Valor
                                break;
                            }
                    }
                }
            }
        }

        public static void SendMessage(int message)
        {
            foreach (Mobile m in World.Mobiles.Values)
            {
                Region r = m.Region;

                if (r.IsPartOf("Ver Lor Reg") && m.IsPlayer() && !CanBossCheck())
                {
                    m.SendLocalizedMessage(message);
                }
            }
        }
    }
}
