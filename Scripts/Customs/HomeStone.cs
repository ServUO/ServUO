
// Created by Dunhill, based on the original idea of Murzin
// Version 1.1, released in runuo.com forums
// Version 1.2, fixed for ServUO by Ixtabay
using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Regions;
using Server.Spells;
using Server.Network;
using Server.Multis;
 
namespace Server.Items
{
    public class HomeStone : Item
    {
        private static readonly TimeSpan delay = TimeSpan.FromMinutes( 15.0 );
        private static readonly bool oneStonePerMobile = true;
        private static readonly TimeSpan totalDelay = TimeSpan.FromMinutes( 15.0 );
       
        private static List<HomeStoneUse> useRegistry = new List<HomeStoneUse>();
       
        private bool noWaitTime;
        private DateTime lastUsed;
        private DateTime lastMarked;
        private Point3D home;
        private Map homeMap;       
        private Mobile owner;
       
 
        #region Commands
        [CommandProperty( AccessLevel.GameMaster )]
        public bool NoWaitTime
        {
            get{ return noWaitTime; }
            set{ noWaitTime = value; }
        }
       
        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastUsed
        {
            get{ return lastUsed; }
            set{ lastUsed = value; }
        }
 
        [CommandProperty( AccessLevel.GameMaster )]
        public Point3D Home
        {
            get{ return home; }
            set{ home = value; }
        }
 
        [CommandProperty( AccessLevel.GameMaster )]
        public Map HomeMap
        {
            get{ return homeMap; }
            set{ homeMap = value; }
        }
       
        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner
        {
            get{ return owner; }
            set{ owner = value; }
        }

// ------------------------------------------------------------------------------------------------------- added per zerodowned		
		/* public override TimeSpan CastDelayBase
         {
             get
             {
                 return TimeSpan.FromSeconds(1.5);
             }
         } */
 
		
        #endregion
       
        #region constructors
        [Constructable]
        public HomeStone() : this( null )
        {
        }
       
        [Constructable]
        public HomeStone(Mobile from) : base( 0x1726) //2AAA )
        {
            this.owner = from;         
            Weight = 0.0;
            LootType = LootType.Blessed;                       
            RenameThisStone();
        }
        #endregion
       
        public static void RegisterUse( Mobile from )
        {
            useRegistry.Add(new HomeStoneUse( from ));
        }
       
        public static TimeSpan GetRemainingTimeToUseForMobile( Mobile from )
        {
            List<HomeStoneUse> innerUseRegistry = useRegistry.FindAll(delegate( HomeStoneUse hsu ) { return hsu.User == from
                                      && hsu.UseTime > DateTime.Now - totalDelay; });  
           
            if ( innerUseRegistry.Count > 0 )
            {
                foreach( HomeStoneUse hsu in innerUseRegistry) // TODO: Better way to solve this?
                {
                    // delay - ( now - used )
                    return  totalDelay -  (DateTime.Now - hsu.UseTime) ;
                }
            }
                   
            return TimeSpan.Zero;
        }
       
        public static void cleanUseList()
        {
            useRegistry.RemoveAll(delegate(HomeStoneUse hsu) { return hsu.UseTime < DateTime.Now - totalDelay; });
        }
       
        public override void OnDoubleClick( Mobile from )
        {          
           
            if ( !IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "This must be in your backpack to use." );
                return;
            }  
           
            if ( owner == null  )
            {
                SwitchOwner(from);
                return;
            }      
           
            if ( IsInMarkAbleRegion( from ))
            {
                Mark( from );
            }
            else
            {
                if ( Validate( from ) )
                {
                    new HomeStoneSpell( this, from ).Cast();
                }
            }
        }
 
        public override void OnSingleClick( Mobile from )
        {
            // why not base? we dont like to see the [blessed] tag, just like on runebooks,spellbooks etc
            // + we dont want it to be called coconut or whatever
            //base.OnSingleClick( from );
           
            LabelTo( from, this.Name );
                       
            string label;
           
            TimeSpan timetouse = GetRemainingTimeToUse();
            //TimeSpan timetouseTotal = GetRemainingTimeToUseForMobile( from );
           
            if ( owner == null )
                return;
           
           
            if ( timetouse.TotalSeconds <= 0.0)
                label = "[ready]";             
            else
                label = "[" + timetouse.Minutes + " minutes]";                 
 
 
           
            if ( this.homeMap != null )
                LabelTo( from, label );
            else
                LabelTo( from, "[unmarked]" );
 
        }
       
        private void Mark( Mobile from )
        {          
            if ( this.lastMarked > DateTime.Now - TimeSpan.FromSeconds(5))
            {
                from.SendMessage("You have to wait until you can mark your stone again.");
            }
            else
            {
                this.home = from.Location;
                this.homeMap = from.Map;
                this.lastMarked = DateTime.Now;
                from.PlaySound( 0x1E9 );       
                from.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist );
                from.SendMessage( "You have marked this as your home." );
            }          
        }
       
        private void SwitchOwner(Mobile from)
        {
            if ( owner == null ) // double check..
            {                  
                owner = from;
                from.SendMessage("You take possession of this hearthstone!");
                RenameThisStone();
            }
            else
                from.SendMessage( "This is not your homestone!" );
        }
       
        private void RenameThisStone()
        {
            if ( owner != null )
            {
                this.Name = owner.Name + "s homestone";
                this.Hue = 0x501;
            }
            else
            {
                this.Name = "a homestone with no owner";
                this.Hue = 0;
            }      
        }
 
        private bool Validate( Mobile from )  
        {
 
            if ( from != owner )
            {
                from.SendMessage( "This is not your homestone!" );
                return false;
            }
           
            else if ( this.homeMap == null )
            {
                from.SendMessage( "This homestone is not yet marked anywhere!" );
                return false;
            }
            else if ( from.Criminal )
            {
                from.SendLocalizedMessage( 1005561, "", 0x22 ); // your criminal and cannot escape so easily
                return false;
            }
            else if ( Server.Spells.SpellHelper.CheckCombat( from ) )
            {
                from.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if ( Server.Factions.Sigil.ExistsOn( from ) )
            {
                from.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
                return false;
            }
            else if ( Server.Misc.WeightOverloading.IsOverloaded( from ) )
            {
                from.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
                return false;
            }
            else if ( GetRemainingTimeToUse() > TimeSpan.Zero && !noWaitTime )
            {
                from.SendMessage( "Your homestone will be ready again in {0} minutes!", (int)HomeStone.GetRemainingTimeToUseForMobile( from ).TotalMinutes );
                return false;
            }
            else if ( HomeStone.GetRemainingTimeToUseForMobile( from ) > TimeSpan.Zero && oneStonePerMobile )
            {
                from.SendMessage( "You must wait {0} minutes before using another homestone.", (int)HomeStone.GetRemainingTimeToUseForMobile( from ).TotalMinutes );
                return false;
            }          
            else
            {
                return true;
            }
 
        }
       
        private bool IsInMarkAbleRegion(Mobile from)
        {
            bool house = false;
            if ( from.Region is HouseRegion )
                if (((HouseRegion)from.Region).House.IsOwner(from))
                    house = true;
           
            if ( from.Region.GetLogoutDelay( from ) == TimeSpan.Zero || house )
                return true;
 
            return false;
        }
       
        private TimeSpan GetRemainingTimeToUse()
        {
            return lastUsed + delay - DateTime.Now;
        }
       
        public HomeStone( Serial serial ) : base( serial )
        {
        }
       
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
            writer.Write( (Mobile)owner);
            writer.Write( (Point3D) home );
            writer.Write( (DateTime) lastUsed );
            writer.Write( (Map) homeMap );
            RenameThisStone();
            cleanUseList();
        }
       
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
            owner = reader.ReadMobile();
            home = reader.ReadPoint3D();
            lastUsed = reader.ReadDateTime();
            homeMap = reader.ReadMap();
            RenameThisStone();
            cleanUseList();
        }
       
        public class HomeStoneSpell : Spell
        {
            private static SpellInfo m_Info = new SpellInfo( "Home Stone", "", 230 );
			public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 2.5 ); } }
            private HomeStone m_homeStone;
 
            public HomeStoneSpell( HomeStone homeStone, Mobile caster ) : base( caster, null, m_Info )
            {
                m_homeStone = homeStone;
               
//          Effects.SendLocationEffect( new Point3D( caster.X + 1, caster.Y, caster.Z + 4 ), caster.Map, 0x3709, 30, 1281, 2 );
//          Effects.SendLocationEffect( new Point3D( caster.X + 1, caster.Y, caster.Z ), caster.Map, 0x3709, 30, 1281, 2 );
//          Effects.SendLocationEffect( new Point3D( caster.X + 1, caster.Y, caster.Z - 4 ), caster.Map, 0x3709, 30, 1281, 2 );
//          Effects.SendLocationEffect( new Point3D( caster.X, caster.Y + 1, caster.Z + 4 ), caster.Map, 0x3709, 30, 1281, 2 );
//          Effects.SendLocationEffect( new Point3D( caster.X, caster.Y + 1, caster.Z ), caster.Map, 0x3709, 30, 1281, 2 );
//          Effects.SendLocationEffect( new Point3D( caster.X, caster.Y + 1, caster.Z - 4 ), caster.Map, 0x3709, 30, 1281, 2 );
//
//          Effects.SendLocationEffect( new Point3D( caster.X + 1, caster.Y + 1, caster.Z + 11 ), caster.Map, 0x3709, 30, 1281, 2 );
//          Effects.SendLocationEffect( new Point3D( caster.X + 1, caster.Y + 1, caster.Z + 7 ), caster.Map, 0x3709, 30, 1281, 2 );
//          Effects.SendLocationEffect( new Point3D( caster.X + 1, caster.Y + 1, caster.Z + 3 ), caster.Map, 0x3709, 30, 1281, 2 );
//          Effects.SendLocationEffect( new Point3D( caster.X + 1, caster.Y + 1, caster.Z - 1 ), caster.Map, 0x3709, 30, 1281, 2 );
            caster.FixedParticles( 0x3709, 10, 30 ,1281,1, 5037, EffectLayer.Waist );
            }
 
            public override bool ClearHandsOnCast{ get{ return false; } }
            public override bool RevealOnCast{ get{ return false; } }
            public override TimeSpan GetCastRecovery()
            {
                return TimeSpan.Zero;
            }
 
            public override TimeSpan GetCastDelay()
            {
                return TimeSpan.FromSeconds(3);//( (m_Mount.IsDonationItem && RewardSystem.GetRewardLevel( m_Rider ) < 3 )? 12.5 : 5.0 ) );
            }
 
            public override int GetMana()
            {
                return 10;
            }
 
            public override bool ConsumeReagents()
            {
                return false;
            }
 
            public override bool CheckFizzle()
            {
                return true;
            }
 
            private bool m_Stop;
 
            public void Stop()
            {
                m_Stop = true;
                Disturb( DisturbType.Hurt, false, false );
            }
 
            public override bool CheckDisturb( DisturbType type, bool checkFirst, bool resistable )
            {
                //if ( type == DisturbType.EquipRequest || type == DisturbType.UseRequest/* || type == DisturbType.Hurt*/ )
                //  return false;
                if ( type == DisturbType.Hurt )
                    return false;
                else
                    return true;
            }
 
            public override void DoHurtFizzle()
            {
                if ( !m_Stop )
                    base.DoHurtFizzle();
            }
 
            public override void DoFizzle()
            {
                if ( !m_Stop )
                    base.DoFizzle();
            }
 
            public override void OnDisturb( DisturbType type, bool message )
            {
                if ( message && !m_Stop )
                    Caster.SendMessage( "You have been disrupted while attempting to use your homestone" );
 
                //m_Mount.UnmountMe();
            }
 
            public override void OnCast()
            {          
               
                HomeStoneTeleport();
                FinishSequence();
            }
           
            private void HomeStoneTeleport( )
            {
                if ( m_homeStone.Validate( Caster ) )
                {          
                    HomeStone.RegisterUse( Caster);
                    BaseCreature.TeleportPets( Caster, m_homeStone.home, m_homeStone.homeMap, true );
                    Caster.Location = new Point3D( m_homeStone.home );
                    Caster.Map = m_homeStone.homeMap;                  
                    m_homeStone.LastUsed = DateTime.Now;   
                    Caster.PlaySound( 0x1FC );
                    Caster.FixedParticles( 0x3709, 10, 30 ,1281,1, 5037, EffectLayer.Waist );
                }
               
            }
        }      
   
        private class HomeStoneUse
        {
            private DateTime useTime;
            private Mobile user;
            public DateTime UseTime{get{return this.useTime;}}
            public Mobile User{get{return this.user;}}
           
            public HomeStoneUse( Mobile from )
            {
                useTime = DateTime.Now;
                user = from;
            }
        }
    }
}
 