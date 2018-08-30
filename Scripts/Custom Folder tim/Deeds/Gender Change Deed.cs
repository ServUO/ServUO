// By Celisuis 
// D.O.S
using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Multis;

namespace Server.Items
{
	public class GenderChangeDeed : Item
	{
		[Constructable]
		public GenderChangeDeed() : base( 0x14F0 )
		{
			Name = "Gender Change Deed";
			Weight = 1.0;
			//LootType = LootType.Blessed;
			Hue = 2067;
		}

		public GenderChangeDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); //version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}

			else
			{

				if ( from.Body == 0x190 )
                {
                    from.Body = 0x191;
                    from.SendMessage("You change your gender to female!");
                    World.Broadcast( 0x35, true, "{0} has had a sex change!", from.Name );
                    this.Delete();
				}
				else if ( from.Body == 0x191 )
                  {
                    from.Body = 0x190;
                    from.SendMessage("You change your gender to male!");
                    World.Broadcast( 0x35, true, "{0} has had a sex change!", from.Name );
                    this.Delete();
                }
                else if ( from.Body == 0x25D )
                {
                    from.Body = 0x25E;
                    from.SendMessage("You change your gender to female!");
                    World.Broadcast( 0x35, true, "{0} has had a sex change!", from.Name );
                    this.Delete();
                }
                else if ( from.Body == 0x25E)
                {
                    from.Body = 0x25D;
                    from.SendMessage("You change your gender to male!");
                    World.Broadcast( 0x35, true, "{0} has had a sex change!", from.Name );
                    this.Delete();
                }
                else if ( from.Body == 0x29A)
                {
                    from.Body = 0x29B;
                    from.SendMessage("You change your gender to female!");
                    World.Broadcast( 0x35, true, "{0} has had a sex change!", from.Name );
                    this.Delete();
                }
                else if ( from.Body == 0x29B)
                {
                    from.Body = 0x29A;
                    from.SendMessage("You change your gender to male!");
                    World.Broadcast( 0x35, true, "{0} has had a sex change!", from.Name );
                    this.Delete();
                }
                else
				{
					from.SendMessage( "You can't use this deed with your current race." );
				}
			}
		}
	}
}