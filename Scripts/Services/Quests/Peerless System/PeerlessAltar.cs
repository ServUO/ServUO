using System;
using System.Collections.Generic;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public abstract class PeerlessAltar : Container
    {
        public override bool IsPublicContainer
        {
            get
            {
                return true;
            }
        }
        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
        }
	
        public virtual TimeSpan TimeToSlay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public virtual TimeSpan DelayAfterBossSlain
        {
            get
            {
                return TimeSpan.FromMinutes(15);
            }
        }
	
        public abstract int KeyCount { get; }
        public abstract MasterKey MasterKey { get; }
		
        public abstract Type[] Keys { get; }
        public abstract BasePeerless Boss { get; }
				
        private BasePeerless m_Peerless;
        private Point3D m_BossLocation;
        private Point3D m_TeleportDest;
        private Point3D m_ExitDest;
        private DateTime m_Deadline;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public BasePeerless Peerless
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
		
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D ExitDest
        {
            get
            {
                return this.m_ExitDest;
            }
            set
            {
                this.m_ExitDest = value;
            }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Deadline
        {
            get
            {
                return this.m_Deadline;
            }
            set
            {
                this.m_Deadline = value;
            }
        }
					
        private List<Mobile> m_Fighters;
        private Dictionary<Mobile, List<Mobile>> m_Pets;
        private List<Item> m_MasterKeys;	
		
        public List<Mobile> Fighters
        {
            get
            {
                return this.m_Fighters;
            }
        }
		
        public Dictionary<Mobile, List<Mobile>> Pets
        {
            get
            {
                return this.m_Pets;
            }
        }
		
        public List<Item> MasterKeys
        {
            get
            {
                return this.m_MasterKeys;
            }
        }
		
        public bool Activated
        {
            get
            {
                return (this.m_Fighters.Count > 0 || this.Items.Count == this.Keys.Length) ? true : false;
            }
        }
		
        public Mobile Summoner
        {
            get
            {
                return this.m_Fighters[0];
            }
        }
	
        public PeerlessAltar(int itemID)
            : base(itemID)
        {
            this.Movable = false;
			
            this.m_Fighters = new List<Mobile>();
            this.m_Pets = new Dictionary<Mobile, List<Mobile>>();
            this.m_MasterKeys = new List<Item>();
        }
	
        public PeerlessAltar(Serial serial)
            : base(serial)
        {
        }
		
        public override bool OnDragDrop(Mobile from, Item dropped)
        { 
            if (this.Activated)
            { 
                from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
                return false;
            }
			
            if (!this.IsKey(dropped))
            {
                from.SendLocalizedMessage(1072682); // This is not the proper key.
                return false;
            }
			
            if (this.Items.Count + 1 == this.Keys.Length)
            {
                from.SendLocalizedMessage(1072680); // You have been given the key to the boss.
				
                for (int i = 0; i < this.KeyCount; i ++)
                {
                    MasterKey key = this.MasterKey;
					
                    if (key != null)
                    {
                        key.Altar = this;
						
                        if (!from.AddToBackpack(key))
                            key.MoveToWorld(from.Location, from.Map);
							
                        this.m_MasterKeys.Add(key);
                    }
                }
				
                dropped.Delete();				
                this.ClearContainer();
                this.StopTimer();
            }
            else
                this.StartTimer(from);
							
            return base.OnDragDrop(from, dropped);
        }
		
        public override void OnDoubleClick(Mobile from)
        {
            if ((int)from.AccessLevel > (int)AccessLevel.Player)
                base.OnDoubleClick(from);
        }
		
        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if ((int)from.AccessLevel > (int)AccessLevel.Player)
                return base.CheckLift(from, item, ref reject);
            else
                reject = LRReason.CannotLift;
				
            return false;
        }
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            // version 1
            writer.Write((bool)(this.m_Helpers != null));
			
            if (this.m_Helpers != null)
                writer.WriteMobileList<BaseCreature>(this.m_Helpers);

            // version 0			
            writer.Write((Mobile)this.m_Peerless);
            writer.Write((Point3D)this.m_BossLocation);
            writer.Write((Point3D)this.m_TeleportDest);
            writer.Write((Point3D)this.m_ExitDest);
			
            writer.Write((DateTime)this.m_Deadline);
			
            // serialize master keys						
            writer.WriteItemList(this.m_MasterKeys);
			
            // serialize fighters							
            writer.WriteMobileList(this.m_Fighters);
				
            // serialize pets
            writer.Write((int)this.m_Pets.Count);
			
            foreach (KeyValuePair<Mobile, List<Mobile>> pair in this.m_Pets)
            {
                writer.Write((Mobile)pair.Key);
				
                writer.WriteMobileList(pair.Value);
            }
        }
		
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            switch ( version )
            {
                case 2:
                case 1:
                    if (reader.ReadBool())
                        this.m_Helpers = reader.ReadStrongMobileList<BaseCreature>();
                    goto case 0;
                case 0:
                    this.m_Peerless = reader.ReadMobile() as BasePeerless;
                    this.m_BossLocation = reader.ReadPoint3D();
                    this.m_TeleportDest = reader.ReadPoint3D();
                    this.m_ExitDest = reader.ReadPoint3D();
					
                    this.m_Deadline = reader.ReadDateTime();
					
                    // deserialize master keys
                    this.m_MasterKeys = reader.ReadStrongItemList();		
						
                    // deserialize fightes			
                    this.m_Fighters = reader.ReadStrongMobileList();
						
                    // deserialize pets
                    this.m_Pets = new Dictionary<Mobile, List<Mobile>>();
                    int count = reader.ReadInt();
					
                    for (int i = 0; i < count; i ++)
                        this.m_Pets.Add(reader.ReadMobile(), reader.ReadStrongMobileList());
					
                    if (version < 2)
                        reader.ReadBool();
					
                    break;
            }
			
            this.FinishSequence();
        }
		
        public virtual bool IsKey(Item item)
        {
            if (this.Keys == null || item == null)
                return false;
				
            bool isKey = false;
			
            // check if item is key	
            for (int i = 0; i < this.Keys.Length && !isKey; i ++)
                if (this.Keys[i].IsAssignableFrom(item.GetType()))
                    isKey = true;
			
            // check if item is already in container			
            for (int i = 0; i < this.Items.Count && isKey; i ++)
                if (this.Items[i].GetType() == item.GetType())
                    return false;
					
            return isKey;
        }
		
        public virtual void ClearContainer()
        {
            while (this.Items.Count > 0)
                this.Items[0].Delete();
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
		
        public virtual void SendConfirmations(Mobile from)
        {
            Party party = Party.Get(from);	
			
            if (party != null)
            {
                this.toConfirm = 0;
				
                foreach (PartyMemberInfo info in party.Members)
                {
                    Mobile m = info.Mobile;
				
                    if (m.InRange(from.Location, 5) && this.CanEnter(m))
                    {
                        if (m == from)
                            this.AddFighter(from);
                        else
                        {
                            this.toConfirm += 1;						
							
                            m.CloseGump(typeof(ConfirmEntranceGump));		
                            m.SendGump(new ConfirmEntranceGump(this));				
                        }
                    }
                }
            }
            else
            { 
                this.AddFighter(from);
                this.BeginSequence(this.Summoner);
            }
        }
		
        public virtual void BeginSequence(Mobile from)
        { 
            if (this.m_Peerless == null)
            {
                // spawn boss
                this.m_Peerless = this.Boss;
					
                if (this.m_Peerless != null)
                {
                    this.m_Peerless.Home = this.m_BossLocation;
                    this.m_Peerless.RangeHome = 4;
                    this.m_Peerless.MoveToWorld(this.m_BossLocation, this.Map);
                    this.m_Peerless.Altar = this;
                }
                else
                    return;
						
                // set deadline								
                if (this.m_Timer != null)
                    this.m_Timer.Stop();
				
                if (this.TimeToSlay != TimeSpan.Zero)
                {
                    this.m_Deadline = DateTime.UtcNow + this.TimeToSlay;
                    this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), new TimerCallback(DeadlineCheck));	
                    this.m_Timer.Priority = TimerPriority.OneMinute;
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
                            pet.MoveToWorld(this.m_TeleportDest, this.Map);
                        }
                    }
                }
				
                // teleport party member
                fighter.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                fighter.PlaySound(0x1FE);
                fighter.MoveToWorld(this.m_TeleportDest, this.Map);						
            }
        }
		
        public virtual bool CanEnter(Mobile fighter)
        {
            return true;
        }
		
        public virtual bool CanEnter(BaseCreature pet)
        {
            return true;
        }
		
        public virtual void FinishSequence()
        { 
            this.StopTimer();
			
            // delete perless
            if (this.m_Peerless != null)
            {
                if (this.m_Peerless.Corpse != null)
                    this.m_Peerless.Corpse.Delete();
			
                this.m_Peerless.Delete();
            }
			
            // teleport pary to exit if not already there				
            for (int i = this.m_Fighters.Count - 1; i >= 0; i --)
                this.Exit(this.m_Fighters[i]);
			
            // delete master keys				
            for (int i = this.m_MasterKeys.Count - 1; i >= 0; i --) 
                this.m_MasterKeys[i].Delete();
				
            this.m_MasterKeys.Clear();				
            this.m_Fighters.Clear();		
            this.m_Pets.Clear();		
			
            // delete any remaining helpers
            this.CleanupHelpers();
			
            // reset summoner, boss		
            this.m_Peerless = null;
        }
		
        public virtual void Exit(Mobile fighter)
        {
            // teleport fighter
            if (fighter.NetState == null)
                fighter.LogoutLocation = this.m_ExitDest;
            else
            { 
                fighter.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                fighter.PlaySound(0x1FE);
				
                if (this is CitadelAltar)
                    fighter.MoveToWorld(this.m_ExitDest, Map.Tokuno);
                else
                    fighter.MoveToWorld(this.m_ExitDest, this.Map);
            }
			
            // teleport his pets
            if (this.m_Pets.ContainsKey(fighter))
            {
                for (int i = 0; i < this.m_Pets[fighter].Count; i ++)
                {
                    BaseCreature pet = this.m_Pets[fighter][i] as BaseCreature;
					
                    if (pet != null && (pet.Alive || pet.IsBonded) && pet.Map != Map.Internal)
                    { 
                        if (pet is BaseMount)
                        {
                            BaseMount mount = (BaseMount)pet;
						
                            if (mount.Rider != null && mount.Rider != fighter)
                            { 
                                mount.Rider.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                                mount.Rider.PlaySound(0x1FE);								
								
                                if (this is CitadelAltar)
                                    mount.Rider.MoveToWorld(this.m_ExitDest, Map.Tokuno);
                                else
                                    mount.Rider.MoveToWorld(this.m_ExitDest, this.Map);
							
                                continue;
                            }
                            else if (mount.Rider != null)
                                continue;
                        }
						
                        pet.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                        pet.PlaySound(0x1FE);									
								
                        if (this is CitadelAltar)
                            pet.MoveToWorld(this.m_ExitDest, Map.Tokuno);
                        else
                            pet.MoveToWorld(this.m_ExitDest, this.Map);
                    }
                }
            }
			
            this.m_Fighters.Remove(fighter);
            this.m_Pets.Remove(fighter);
			
            fighter.SendLocalizedMessage(1072677); // You have been transported out of this room.
			
            if (this.m_Fighters.Count == 0)
                this.FinishSequence();
        }
		
        public virtual void OnPeerlessDeath()
        {
            this.SendMessage(1072681); // The master of this realm has been slain! You may only stay here so long.
			
            if (this.DelayAfterBossSlain != null)
                this.SendMessage(1075611, this.DelayAfterBossSlain.TotalSeconds); // Time left: ~1_time~ seconds
				
            this.StopTimer();
			
            // delete master keys				
            for (int i = this.m_MasterKeys.Count - 1; i >= 0; i --) 
                this.m_MasterKeys[i].Delete();
				
            this.m_MasterKeys.Clear();
			
            this.m_Timer = Timer.DelayCall(this.DelayAfterBossSlain, new TimerCallback(FinishSequence));
        }
		
        public virtual void SendMessage(int message)
        {
            for (int i = 0; i < this.m_Fighters.Count; i ++)
                this.m_Fighters[i].SendLocalizedMessage(message);
        }
		
        public virtual void SendMessage(int message, object param)
        { 
            for (int i = 0; i < this.m_Fighters.Count; i ++)
                this.m_Fighters[i].SendLocalizedMessage(message, param.ToString());
        }
		
        private Timer m_Timer;				
		
        public virtual void StartTimer(Mobile from)
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), new TimerStateCallback(KeyTimeout_Callback), from);
        }
		
        public virtual void StopTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;
        }
		
        private void KeyTimeout_Callback(object state)
        {
            if (state is Mobile)
                this.KeyTimeout((Mobile)state);
        }
		
        public virtual void KeyTimeout(Mobile from)
        { 
            this.ClearContainer();
			
            from.SendLocalizedMessage(1072679); // Your realm offering has reset. You will need to start over.
        }
		
        public virtual void DeadlineCheck()
        {
            if (DateTime.UtcNow > this.m_Deadline)
            {
                this.SendMessage(1072258); // You failed to complete an objective in time!
                this.FinishSequence();
                return;
            }
			
            TimeSpan timeLeft = this.m_Deadline - DateTime.UtcNow;
			
            if (timeLeft < TimeSpan.FromMinutes(30))
                this.SendMessage(1075611, timeLeft.TotalSeconds);
				
            for (int i = this.m_Fighters.Count - 1; i >= 0; i --)
            {
                if (this.m_Fighters[i] is PlayerMobile)
                {
                    PlayerMobile player = (PlayerMobile)this.m_Fighters[i];
					
                    if (player.NetState == null)
                    { 
                        TimeSpan offline = DateTime.UtcNow - player.LastOnline;
						
                        if (offline > TimeSpan.FromMinutes(10))
                            this.Exit(player);
                    }
                }
            }
        }
		
        #region Helpers
        private List<BaseCreature> m_Helpers = new List<BaseCreature>();

        public List<BaseCreature> Helpers
        {
            get
            {
                return this.m_Helpers;
            }
        }
		
        public void AddHelper(BaseCreature helper)
        { 
            if (helper != null && helper.Alive && !helper.Deleted)
                this.m_Helpers.Add(helper);
        }

        public bool AllHelpersDead()
        {
            for (int i = this.m_Helpers.Count - 1; i >= 0; i--)
            {
                BaseCreature c = this.m_Helpers[i];

                if (c.Alive)
                    return false;
            }

            return true;
        }
		
        public void CleanupHelpers()
        {
            for (int i = this.m_Helpers.Count - 1; i >= 0; i --)
            {
                BaseCreature c = this.m_Helpers[i];
				
                if (c.Alive)
                    c.Delete();
            }
			
            this.m_Helpers.Clear();
        }
        #endregion
    }
}