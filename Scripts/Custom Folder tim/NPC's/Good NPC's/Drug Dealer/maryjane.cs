using System; 
using Server; 
using Server.Items;
using Server.Network;
//using Server.Scripts; 
using System.Threading;

namespace Server.Items 
{ 
   public abstract class Drug : Item 
   { 
     
      public float Highness;// 

      

      public Drug( int itemID ) : base( itemID ) 
      { 
      } 

      public Drug( Serial serial ) : base( serial ) 
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

   
    [FlipableAttribute( 0xD16, 0xF00 )] 
    public class Shroom : Drug 
    { 

    [Constructable] 
        public Shroom() : base( 0xD16 ) 
        { 
            Name = "a psychadelic mushroom"; 
            this.Weight = 0.0; 
            this.Hue = 1289; 
        } 

        public Shroom( Serial serial ) : base( serial ) 
        { 
        } 

        public override void OnDoubleClick( Mobile from ) 
        { 
            Container pack = from.Backpack; 
            if (pack != null && pack.ConsumeTotal( typeof( Shroom ), 1 ) ) 
            { 
                if ( from.Body.IsHuman && !from.Mounted ) 
                { 
                    from.Animate( 34, 5, 1, true, false, 0 ); 
                    from.PlaySound( Utility.Random( 0x3A, 3 ) ); // random eating noise 
                    from.SendMessage( "You break off the stem and eat the funky colored cap." ); 
                    from.SendMessage( "You feel yourself beginning to trip mildly!"); 
                } 
                else 
                { 
                    from.SendMessage( "Your out of shrooms bro!" ); 
                    return; 
                } 
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
//============== 
    [FlipableAttribute( 0xE7A, 0xF00 )] 
    public class BoxOfPhillies : Drug 
    { 

    [Constructable] 
        public BoxOfPhillies() : base( 0xE7A ) 
        { 
            Name = "A Box of Phillie Blunts"; 
            this.Weight = 0.0; 
            this.Hue = 542; 
        } 

        public BoxOfPhillies( Serial serial ) : base( serial ) 
        { 
        } 

        public override void OnDoubleClick( Mobile from ) 
        { 
            Container pack = from.Backpack; 
            if (pack != null && pack.ConsumeTotal( typeof( BoxOfPhillies ), 1 ) ) 
            { 
                from.SendMessage( "You take the blunts out of the box." ); 
                for ( int i = 0; i < 5; ++i ) 
                from.AddToBackpack( new PhillieBlunt() ); 
            } 
            else 
            { 
                from.SendMessage( "You need more blunts!" ); 
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

//=========== 
  
  [FlipableAttribute( 0x1BE0, 0xF00 )]
   public class Blunt : Drug 
   { 
//      int hi = 0;   // 
      [Constructable] 
      public Blunt() : base( 0x1BE0 ) 
      { 
         Name = "a Huge Blunt"; 
         this.Weight = 0.0;            //this is for future additions... 
         this.Hue = 542; 
      } 
	
      public Blunt( Serial serial ) : base( serial ) 
      { 
      } 

            public override void OnDoubleClick( Mobile from ) 
            { 
               Container pack = from.Backpack; 
      
               if (pack != null && pack.ConsumeTotal( typeof( Blunt ), 1 ) ) 
               { 
                  if ( from.Body.IsHuman && !from.Mounted ) 
                  { 
                     from.Animate( 34, 5, 1, true, false, 0 ); 
                  } 
                  from.SendMessage( "You whip out your lighter and spark up the doobie." ); 
                  from.PlaySound( 0x226 ); 
                  Highness=30; //One point = one second
				   if ( Core.AOS )
					   from.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
				   else
					   from.FixedEffect( 0x3735, 6, 30 );	
				  new StonedTimer(from,Highness).Start();
				  
				  
               } 
               else 
               { 
                  from.SendMessage( "Your must have the doobie in your pack to spark it!" ); 
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

   
//=====================================


    [FlipableAttribute( 0x1915, 0xF00 )] 
    public class PhillieBluntWrapper : Drug 
    { 

    [Constructable] 
        public PhillieBluntWrapper() : base( 0x1915 ) 
        { 
            Name = "a cut up blunt"; 
            this.Weight = 0.0; 
            this.Hue = 542; 
        } 

        public PhillieBluntWrapper( Serial serial ) : base( serial ) 
        { 
        } 

        public override void OnDoubleClick( Mobile from ) 
        { 
            Container pack = from.Backpack; 
            if (pack != null && pack.ConsumeTotal( typeof( Marijuana ), 6 ) && pack.ConsumeTotal( typeof( PhillieBluntWrapper ), 1 ) ) 
            { 
                from.SendMessage( "You fill the broken blunt with pot, therefore fixing it." ); 
                from.AddToBackpack( new Blunt() ); 
            } 
            else 
            { 
                from.SendMessage( "You need more green bro!" ); 
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

//=============== 

    [FlipableAttribute( 0xF8A, 0xF00 )] 
    public class PhillieBlunt : Drug 
    { 

    [Constructable] 
        public PhillieBlunt() : base( 0xF8A ) 
        { 
            Name = "a Phillie Blunt"; 
            this.Weight = 0.0; 
            this.Hue = 542; 
        } 

        public PhillieBlunt( Serial serial ) : base( serial ) 
        { 
        } 

        public override void OnDoubleClick( Mobile from ) 
        { 
            Container pack = from.Backpack; 
            if (pack != null && pack.ConsumeTotal( typeof( PhillieBlunt ), 1 ) ) 
            { 
                from.SendMessage( "You break open the blunt and are left with the exterior." ); 
                from.AddToBackpack( new PhillieBluntWrapper() ); 
            } 
            else 
            { 
                from.SendMessage( "Your out of blunts bro!" ); 
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


//========== 
//Fix from here on 

   

    public class Crack : Drug 
    { 

    [Constructable] 
        public Crack() : base( 0x26B8 ) 
        { 
            Name = "Crack"; 
            Stackable = true; 
            this.Weight = 0.1; 
            Hue = 1153; 
        } 

        public Crack( Serial serial ) : base( serial ) 
        { 
        } 

        public override void Serialize( GenericWriter writer ) 
        { 
            base.Serialize( writer ); 
            writer.Write( (int) 0 ); // version 
        } 

        public override void OnDoubleClick( Mobile from ) 
        { 
            Container pack = from.Backpack; 

            if (pack != null && pack.ConsumeTotal( typeof( Crack ), 1 ) ) 
            { 
                if ( from.Body.IsHuman && !from.Mounted ) 
                from.Animate( 34, 5, 1, true, false, 0 ); 
                from.SendMessage( "You snort the drug." ); 
                from.SendMessage( "You feel extremely paronoid. You start to shake." ); 
                from.SendMessage( "*Sniff* *Sniff*"); 
                from.Animate( 34, 5, 1, true, false, 0 ); 
                from.Animate( 34, 5, 1, true, false, 0 ); 
                from.Animate( 34, 5, 1, true, false, 0 ); 
                from.Say( "*Sniff* *Sniff*" ); 
                from.Say( "IT BURNS!!!" ); 
            } 
        } 
        public override void Deserialize( GenericReader reader ) 
        { 
            base.Deserialize( reader ); 
            int version = reader.ReadInt(); 
        } 
    } 

    public class CrystalMeth : Drug 
    { 

    [Constructable] 
        public CrystalMeth() : base( 0x2215 ) 
        { 
            Name = "Crystal Meth"; 
            this.Weight = 0.1; 
        } 

        public CrystalMeth( Serial serial ) : base( serial ) 
        {   
        } 

        public override void Serialize( GenericWriter writer ) 
        { 
            base.Serialize( writer ); 
            writer.Write( (int) 0 ); // version 
        } 
     
        public override void OnDoubleClick( Mobile from ) 
        { 
            Container pack = from.Backpack; 
            if (pack != null && pack.ConsumeTotal( typeof( CrystalMeth ), 1 ) ) 
            { 
                if ( from.Body.IsHuman && !from.Mounted ) 
                from.Animate( 34, 5, 1, true, false, 0 ); 
                from.SendMessage( "You snort the drug." ); 
                from.SendMessage( "You feel extremely confident and strong." ); 
                //if ( m_Poison != null ) 
                { 
                    from.SendMessage( "*GaG* *GaG*"); 
                    from.Say( "*GaG*" ); 
                } 
            } 
        } 
        public override void Deserialize( GenericReader reader ) 
        { 
            base.Deserialize( reader ); 
            int version = reader.ReadInt(); 
        } 
    } 
    [FlipableAttribute( 0x1A9E, 0xF00 )] 
    public class cokeplant : Drug 
    { 
    [Constructable] 
        public cokeplant() : base( 0x1A9E ) 
        { 
            Name = "cocaine plant"; 
            this.Weight = 0.0; 
        } 

        public cokeplant( Serial serial ) : base( serial ) 
        { 
        } 

        public override void OnDoubleClick( Mobile from ) 
        { 
            from.SendMessage( "You harvest some cocaine from the plant." ); 
            for ( int i = 0; i < 6; ++i ) 
            from.AddToBackpack( new Crack () ); 
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
//===================================== 

    [FlipableAttribute( 0x1915, 0xF00 )] 
    public class Roller : Drug 
    { 

    [Constructable] 
        public Roller() : base( 0x1915 ) 
        { 
            Name = "a Roller"; 
            Stackable = true; 
            this.Weight = 0.0; 
            this.Hue = 542; 
        } 

        public Roller( Serial serial ) : base( serial ) 
        { 
        } 

        public override void OnDoubleClick( Mobile from ) 
        { 
            Container pack = from.Backpack; 
            if (pack != null && pack.ConsumeTotal( typeof( Marijuana ), 6 ) && pack.ConsumeTotal( typeof( Roller ), 1 ) ) 
            { 
                from.SendMessage( "You fill the roller with pot, then lick it closed." ); 
                from.AddToBackpack( new Joint() ); 
            } 
            else 
            { 
                from.SendMessage( "You need more green bro!" ); 
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
   
   
   
   [FlipableAttribute( 0xE28, 0xF00 )]
   public class WaterBong : Drug 
   { 

      [Constructable] 
      public WaterBong() : base( 0xE28 ) 
      { 
         Name = "a water bong";         // chills and graphix? 
         this.Weight = 0.0;            // 
         this.Hue = 1289;            // sweet color 
      } 

      public WaterBong( Serial serial ) : base( serial ) 
      { 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 
         Container pack = from.Backpack; 

		  if (pack != null && pack.ConsumeTotal( typeof( Marijuana ), 1 ) ) 
		  { 
			  if ( from.Body.IsHuman && !from.Mounted ) 
			  { 
				  from.Animate( 34, 5, 1, true, false, 0 );
               
			  } 
			  from.PlaySound( Utility.Random( 0x20, 2 ) ); 
			  from.SendMessage( "You pack a bowl and light it." );
			  from.Meditating = true; 
			  from.SendMessage( "You feel light very light headed." ); 
			  from.PlaySound( from.Female ? 798 : 1070 );
			  from.Say( "*hiccup!*" );
			  Highness=120;    //one point = one sec 
			  new StonedTimer(from,Highness).Start();
		  }
        
		  else 
		  { 
			  from.SendMessage( "Your out of pot yo!" ); 
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

   //================================= 
   [FlipableAttribute( 0x1AA2, 0xF00 )] 
   public class MarijuanaSeeds : Drug 
   { 

      [Constructable] 
      public MarijuanaSeeds() : base( 0x1AA2 ) 
      { 
         Name = "marijuana seeds"; 
         this.Weight = 0.0;            //this is for future additions... 
      } 

      public MarijuanaSeeds( Serial serial ) : base( serial ) 
      { 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 
         Container pack = from.Backpack; 

         if (pack != null && pack.ConsumeTotal( typeof( MarijuanaSeeds ), 3 ) ) 
         { 
            from.SendMessage( "You plant some seeds in the ground and the gods that run this shard donate a plant to you!" ); 
               switch ( Utility.Random( 4 ) ) 
               { 
                  case 0: from.AddToBackpack( new GanjaPlantA() ); break; 
                  case 1: from.AddToBackpack( new GanjaPlantB() ); break; 
                  case 2: from.AddToBackpack( new GanjaPlantC() ); break; 
                  case 3: from.AddToBackpack( new GanjaPlantD() ); break; 
               } 
         } 
         else 
         { 
            from.SendMessage( "Your need more seeds than that if you want some plants to survive!" ); 
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
   //===================================== 

   [FlipableAttribute( 0x1AA2, 0xF00 )] 
   public class Joint : Drug 
   { 
//      int hi = 0;   // 
      [Constructable] 
      public Joint() : base( 0x1420 ) 
      { 
         Name = "a fat handrolled joint"; 
         this.Weight = 0.0;            //this is for future additions... 
         this.Hue = 1153; 
      } 
	
      public Joint( Serial serial ) : base( serial ) 
      { 
      } 

            public override void OnDoubleClick( Mobile from ) 
            { 
               Container pack = from.Backpack; 
      
               if (pack != null && pack.ConsumeTotal( typeof( Joint ), 1 ) ) 
               { 
                  if ( from.Body.IsHuman && !from.Mounted ) 
                  { 
                     from.Animate( 34, 5, 1, true, false, 0 ); 
                  } 
                  from.SendMessage( "You whip out your lighter and spark up the doobie." ); 
                  from.PlaySound( 0x226 ); 
                  Highness=30; //One point = one second
				   if ( Core.AOS )
					   from.FixedParticles( 0x3735, 1, 30, 9503, EffectLayer.Waist );
				   else
					   from.FixedEffect( 0x3735, 6, 30 );	
				  new StonedTimer(from,Highness).Start();
				  
				  
               } 
               else 
               { 
                  from.SendMessage( "Your must have the doobie in your pack to spark it!" ); 
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

   //================================= 
   [FlipableAttribute( 0x1AA2, 0xF00 )] 
   public class RollingPaper : Drug 
   { 

      [Constructable] 
      public RollingPaper() : base( 0xFEF ) 
      { 
         Name = "a pack of rastafarian rolling papers"; 
         this.Weight = 0.0; 
         this.Hue = 1153; 
      } 

      public RollingPaper( Serial serial ) : base( serial ) 
      { 
      } 

            public override void OnDoubleClick( Mobile from ) 
            { 
               Container pack = from.Backpack; 
      
               if (pack != null && pack.ConsumeTotal( typeof( Marijuana ), 3 ) ) 
               { 
                  from.SendMessage( "You roll up some pot into a fatty." ); 
                  from.AddToBackpack( new Joint() ); 
               } 
               else 
               { 
                  from.SendMessage( "Your need more pot bro!" ); 
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
 

   [FlipableAttribute( 0x1A9E, 0xF00 )] 
   public class GanjaPlantA : Drug 
   { 

      [Constructable] 
      public GanjaPlantA() : base( 0x1A9E ) 
      { 
         Name = "A Marijuana Plant"; 
         this.Weight = 0.0; 
      } 

      public GanjaPlantA( Serial serial ) : base( serial ) 
      { 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 
         from.SendMessage( "You harvest some weed from the plant." ); 
         for ( int i = 0; i < 6; ++i ) 
            from.AddToBackpack( new Marijuana() );
		  Delete();
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
   //===================================== 

   [FlipableAttribute( 0x1A9F, 0xF00 )] 
   public class GanjaPlantB : Drug 
   { 

      [Constructable] 
      public GanjaPlantB() : base( 0x1A9F ) 
      { 
         Name = "A Marijuana Plant"; 
         this.Weight = 0.0; 
      } 

      public GanjaPlantB( Serial serial ) : base( serial ) 
      { 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 
         from.SendMessage( "You harvest some weed from the plant." ); 
         for ( int i = 0; i < 6; ++i ) 
            from.AddToBackpack( new Marijuana() ); 
		  Delete();
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
   //===================================== 

   [FlipableAttribute( 0x1AA0, 0xF00 )] 
   public class GanjaPlantC : Drug 
   { 

      [Constructable] 
      public GanjaPlantC() : base( 0x1AA0 ) 
      { 
         Name = "A Marijuana Plant"; 
         this.Weight = 0.0; 
      } 

      public GanjaPlantC( Serial serial ) : base( serial ) 
      { 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 
         from.SendMessage( "You harvest some weed from the plant." ); 
         for ( int i = 0; i < 6; ++i ) 
            from.AddToBackpack( new Marijuana() ); 
		  Delete();
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
   //===================================== 

   [FlipableAttribute( 0x1AA1, 0xF00 )] 
   public class GanjaPlantD : Drug 
   { 

      [Constructable] 
      public GanjaPlantD() : base( 0x1AA1 ) 
      { 
         Name = "A Marijuana Plant"; 
         this.Weight = 0.0; 
      } 

      public GanjaPlantD( Serial serial ) : base( serial ) 
      { 
      } 

      public override void OnDoubleClick( Mobile from ) 
      { 
         from.SendMessage( "You harvest some weed from the plant." ); 
         for ( int i = 0; i < 6; ++i ) 
            from.AddToBackpack( new Marijuana() ); 
		  Delete();
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
   //===================================== 

   
   public class Marijuana : Item
   { 
	  

	   [Constructable]
	   public Marijuana() : this( 1 )
	   {
	   }
   


      [Constructable] 
      public Marijuana(int amount) : base( 0xF88) 
      { 
	   Stackable = true;
         Name="Marijuana";
         this.Weight = 0.1; 
	   Amount = amount;
		
      } 

      public Marijuana( Serial serial ) : base( serial ) 
      { 
      } 

	   //public override Item Dupe( int amount )
	   //{
		   //return base.Dupe( new Marijuana( amount ), amount );
	   //}
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
	//------------Drug Effect Timer
	public class StonedTimer : Timer
	{
		private Mobile m_Drunk;
		private float Highness;
		  
		public StonedTimer(Mobile from, float High) : base( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds(0.5)) 
		{
			
			m_Drunk=from;
			Highness=High;
			Priority = TimerPriority.OneSecond;
		}

		protected override void OnTick()
		{
			if ( m_Drunk.Deleted || m_Drunk.Map == Map.Internal )
			{
				Stop();
				
			}
			else if ( !m_Drunk.Alive )
			{
			}
			
			m_Drunk.Direction = (Direction)Utility.Random( 8 );			
			
			if(Highness%10==0)
			{
				m_Drunk.SendMessage( "You are stoned!" ); 
			    m_Drunk.PlaySound( 0x420 );   // coughing
				int enlight = Utility.Random( 4 );
				switch(enlight) //Randomly generated enlightning things to say
				{
					case 1: 
					{
				            m_Drunk.Say( "I'm high as a kite..." );
						m_Drunk.PlaySound( m_Drunk.Female ? 794 : 1066 );
						m_Drunk.Say( "mooooo! hehehehe" );
						break;
					}

					case 2:
					{
						m_Drunk.Say( "This is some good stuff ..." );
						m_Drunk.PlaySound( m_Drunk.Female ? 794 : 1066 );
						m_Drunk.Say( "whoaaa...huh huh huh" );
						break;
					}

					case 3:
					{
						m_Drunk.Say( "uh..huh huh huh.." ); 
						m_Drunk.Say( "I'm freakin wasted" );
						m_Drunk.Say( "I smoke weed everyday hey" );			
		
					break;
					}

				}
			}
			Highness--;
			if (Highness<=0) 
			{
				
				m_Drunk.SendMessage("You are no longer stoned.");	
				Stop();
				
			 }
		}
	}
	
} 

