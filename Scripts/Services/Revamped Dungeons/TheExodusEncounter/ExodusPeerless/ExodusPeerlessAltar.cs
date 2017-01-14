using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Mobiles;
using Server.Network;
using System.Linq;
using Server.ContextMenus;
using Server.Engines.Exodus;

namespace Server.Items
{
    public abstract class PeerlessExodusAltar : BaseDecayingItem
    {
        public virtual TimeSpan DelayExit { get { return TimeSpan.FromMinutes(10); } }	
       
        private Point3D m_TeleportDest;
        public override int Lifespan { get { return 840; } }
        public override bool UseSeconds { get { return false; } }        
		
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
        }

        public PeerlessExodusAltar(Serial serial)
            : base(serial)
        {
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

        public virtual void BeginSequence(Mobile from)
        {
            if (VerLorRegController.Active && VerLorRegController.Mobile != null && ExodusSummoningAlter.CheckExodus())
            {
                // teleport figters
                for (int i = 0; i < this.m_Fighters.Count; i++)
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
            else
                from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
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
    }
}
