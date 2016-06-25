using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class SnakeCharmerFlute : BaseInstrument
	{
        public override int LabelNumber { get { return 1112174; } } // Snake Charmer Flute
		
		[Constructable]
		public SnakeCharmerFlute() : base( 0x2805, 0, 0 )
		{
			this.LootType = LootType.Regular;
			this.Weight = 2.0;
			this.Hue = 0x187;
        }

        public override void OnDoubleClick ( Mobile from )
        {
			if ( IsChildOf( from.Backpack ) )
            {
				from.Target = new InternalTargetSnake( this );
                from.SendLocalizedMessage( 1112175 ); // Target the serpent you wish to entice.
			}
            else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
        }

        private class InternalTargetSnake : Target
		{
			private SnakeCharmerFlute m_Flute;

			public InternalTargetSnake( SnakeCharmerFlute flute ) : base( 2, false, TargetFlags.None )
			{
				m_Flute = flute;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
                if ( targeted is Snake || targeted is CoralSnake || targeted is SilverSerpent || targeted is GiantSerpent || targeted is LavaSnake )
                {
                    from.Target = new InternalTargetNest( m_Flute, targeted );
                    from.SendLocalizedMessage( 502475 ); // Click where you wish the animal to go.
                }
                else
                    from.SendLocalizedMessage( 1112176 ); // That is not a snake or serpent.
            }
        }
        	        
        private class InternalTargetNest : Target
		{
			private SnakeCharmerFlute m_Flute;

            private object m_Snake;

			public InternalTargetNest( SnakeCharmerFlute flute, object snake ) : base( 2, false, TargetFlags.None )
			{
				m_Flute = flute;
                m_Snake = snake;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
                if ( targeted is SerpentNest )
                {				
					SerpentNest nest = (SerpentNest)targeted;
                    BaseCreature snake = (BaseCreature)m_Snake;

                    from.SendLocalizedMessage( 502479 ); //The animal walks where it was instructed to.
                    snake.ActiveSpeed = 0.1;
                    snake.PassiveSpeed = 0.2;
                    snake.ControlOrder = OrderType.Follow;
                    snake.CurrentSpeed = 0.1;
                    snake.MoveToWorld( new Point3D( ((SerpentNest)targeted).X,((SerpentNest)targeted).Y,((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map );
                    snake.Frozen = true;
                    snake.Say( 1112588 ); // The snake begins searching for rare eggs.
                    m_Flute.ConsumeUse(from);

                    if (Utility.RandomDouble() < 0.25)  //% may be off, just a rough guess
                    {
                        switch (Utility.Random(4))
                        {
                            case 0:
                                {
                                    RareSerpentEgg4 RSEB = new RareSerpentEgg4();
                                    RSEB.MoveToWorld(new Point3D(((SerpentNest)targeted).X, ((SerpentNest)targeted).Y, ((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map);
                                    snake.Say(1112586); // The snake finds a rare egg and drags it out of the nest!		
                                    nest.Delete();
                                }
                                break;
                            case 1:
                                {
                                    RareSerpentEgg3 RSEW = new RareSerpentEgg3();
                                    RSEW.MoveToWorld(new Point3D(((SerpentNest)targeted).X, ((SerpentNest)targeted).Y, ((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map);
                                    snake.Say(1112586); // The snake finds a rare egg and drags it out of the nest!	
                                    nest.Delete();
                                }
                                break;
                            case 2:
                                {
                                    RareSerpentEgg2 RSER = new RareSerpentEgg2();
                                    RSER.MoveToWorld(new Point3D(((SerpentNest)targeted).X, ((SerpentNest)targeted).Y, ((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map);
                                    snake.Say(1112586); // The snake finds a rare egg and drags it out of the nest!	
                                    nest.Delete();
                                }
                                break;
                            case 3:
                                {
                                    RareSerpentEgg1 RSEY = new RareSerpentEgg1();
                                    RSEY.MoveToWorld(new Point3D(((SerpentNest)targeted).X, ((SerpentNest)targeted).Y, ((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map);
                                    snake.Say(1112586); // The snake finds a rare egg and drags it out of the nest!		
                                    nest.Delete();
                                }
                                break;
                        }
                    }
                    else if (Utility.RandomDouble() >= 0.25)
                    {
                        switch (Utility.Random(4))
                        {
                            case 0:
                                {
                                    snake.Say(1112584); // The snake searches the nest and finds nothing.	
                                }
                                break;
                            case 1:
                                {
                                    CoralSnake S1 = new CoralSnake();  //Not sure of what type or how many snakes it spawns
                                    CoralSnake S3 = new CoralSnake();  //Not sure of what type or how many snakes it spawns
                                    S1.MoveToWorld(new Point3D(((SerpentNest)targeted).X, ((SerpentNest)targeted).Y, ((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map);
                                    S3.MoveToWorld(new Point3D(((SerpentNest)targeted).X, ((SerpentNest)targeted).Y, ((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map);
                                    snake.Say(1112585); // Beware! The snake has hatched some of the eggs!!							
                                }
                                break;
                            case 2:
                                {
                                    LavaSnake S2 = new LavaSnake();  //Not sure of what type or how many snakes it spawns
                                    LavaSnake S4 = new LavaSnake();  //Not sure of what type or how many snakes it spawns
                                    S2.MoveToWorld(new Point3D(((SerpentNest)targeted).X, ((SerpentNest)targeted).Y, ((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map);
                                    S4.MoveToWorld(new Point3D(((SerpentNest)targeted).X, ((SerpentNest)targeted).Y, ((SerpentNest)targeted).Z), ((SerpentNest)targeted).Map);
                                    snake.Say(1112585); // Beware! The snake has hatched some of the eggs!!									
                                }
                                break;
                            case 3:
                                {
                                    snake.Say(1112583); // The nest collapses.		
                                    nest.Delete();
                                }
                                break;
                        }
                    }

                    snake.Frozen = false;
                    snake.Say( 1112181 ); // The charm seems to wear off.

                }
                else
                    return;
                    //from.SendLocalizedMessage( 1112176 ); // That is not a snake or serpent.
            }
        }
		
		public SnakeCharmerFlute( Serial serial ) : base( serial )
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
}