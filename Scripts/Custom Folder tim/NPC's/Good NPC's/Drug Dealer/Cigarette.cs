using System; 
using Server; 
using Server.Items;
using Server.Network;
//using Server.Scripts; 
using System.Threading;

namespace Server.Items 
{ 
   public abstract class Drugs : Item 
   { 
     
      public float Puffing;// 

      

      public Drugs( int itemID ) : base( itemID ) 
      { 
      } 

      public Drugs( Serial serial ) : base( serial ) 
      { 
      } 

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 

         writer.Write( (int) 0 ); // version 
          
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 

         int version = reader.ReadInt(); 
      
      } 
   }

   [FlipableAttribute( 0x1AA2, 0xF00 )] 
   public class Cigarette : Drugs
   { 
//      int hi = 0;   // 
      [Constructable] 
      public Cigarette() : base( 0x1420 ) 
      { 
         Name = "a Cigarette"; 
         this.Weight = 0.01;            //this is for future additions... 
         this.Hue = 1153; 
      } 
	
      public Cigarette( Serial serial ) : base( serial ) 
      { 
      } 

            public override void OnDoubleClick( Mobile from ) 
            { 
               Container pack = from.Backpack; 
      
               if (pack != null && pack.ConsumeTotal( typeof( Cigarette ), 1 ) ) 
               { 
                  if ( from.Body.IsHuman && !from.Mounted ) 
                  { 
                     from.Animate( 34, 5, 1, true, false, 0 ); 
                  } 
                  from.SendMessage( "You light your cigarette and begin smoking." ); 
		  from.Meditating = true;
		  from.SendMessage( "You feel more relaxed and calm.");
                  from.PlaySound( 0x226 ); 
                  Puffing=60; //One point = one second
		  new PuffingTimer(from,Puffing).Start();
				  
				  
               } 
               else 
               { 
                  from.SendMessage( "Your must have the cigarette in your pack to smoke it!" ); 
                  return; 
               } 
            } 

      public override void Serialize( GenericWriter writer ) 
      { 
         base.Serialize( writer ); 

         writer.Write( (int) 0 ); // version 
      } 

      public override void Deserialize( GenericReader reader ) 
      { 
         base.Deserialize( reader ); 

         int version = reader.ReadInt(); 
      } 
   } 

	//------------Drugs Effect Timer
	public class PuffingTimer : Timer
	{
		private Mobile m_Drunk;
		private float Puffing;
		  
		public PuffingTimer(Mobile from, float High) : base( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds(0.5)) 
		{
			
			m_Drunk=from;
			Puffing=High;
			//Fill Stam and Mana Lower HitPoints (Smoking makes you feel better but is bad for you =)
			m_Drunk.Stam-=5;
			m_Drunk.Mana+=10;
			m_Drunk.Hits-=5;
			Priority = TimerPriority.OneSecond;
		}

		protected override void OnTick()
		{
			if ( m_Drunk.Deleted || m_Drunk.Map == Map.Internal )
			{
				Stop();
				
			}
			if(Puffing%10==0)
			{
				m_Drunk.SendMessage( "You take another puff." ); 
			   	m_Drunk.PlaySound( 0x420 );   // coughing
			}
			Puffing--;
			if (Puffing<=0) 
			{
				
				m_Drunk.SendMessage("You finish smoking your cigarette");	
				m_Drunk.Int+=1;// Raise Intelligence 1 Point
				m_Drunk.Dex-=1;//Drop Dexterity 1 Point
				m_Drunk.Str-=1;//Drop Strength 1 Point
				m_Drunk.SendMessage("You feel more calm but less like fighting");
				Stop();
				
			 }
		}
	}
	
} 
