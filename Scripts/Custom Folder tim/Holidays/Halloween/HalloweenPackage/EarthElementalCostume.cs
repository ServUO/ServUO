///////////////////////////////////////////////////
///// Created By Bad Karma aka Broadside///////////
///////////////////////////////////////////////////

using System;
using Server;

namespace Server.Items
{
	public class EarthElementalCostume : Item, IDyable
	{

		public bool m_Transformed;
		public Timer m_TransformTimer;
		private DateTime m_End;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Transformed
		{
			get{ return m_Transformed; }
			set{ m_Transformed = value; }
		}

		[Constructable]
		public EarthElementalCostume() : base( 0x2684 )
		{
                        Name = "EarthElemental Costume";
			
                        Hue = 2;
                        Layer = Layer.OuterTorso;
                        ItemID = 0x2684;

			Weight = 3.0;
		}

		public EarthElementalCostume( Serial serial ) : base( serial )
		{
		}

     		public override void OnDoubleClick( Mobile from ) 
		{ 
			
                        if ( Parent != from ) 
                        { 
                                from.SendMessage( "The costume must be equiped to be used." ); 
                        } 

			else if ( from.Mounted == true )
			{
				from.SendMessage( "You cannot be mounted while wearing your costume!" );
			}

                        else if ( this.Transformed == false )
                        { 
				
				LootType = LootType.Blessed;
               			from.SendMessage( "You pull the mask over your head." );
				from.PlaySound( 0x440 );
				//from.Title = "skeleton";
				from.BodyMod = 14;
				from.NameHue = 39;
				from.DisplayGuildTitle = false; 
				this.Transformed = true; 
				ItemID = 9860;
				from.RemoveItem(this);
              			from.EquipItem(this);
                        
			}
			else
			{
				from.SendMessage( "You lower the mask." );
				from.PlaySound( 0x440 );
				//from.Title = null;
				from.BodyMod = 0x0;
				from.NameHue = -1;
				from.HueMod = -1;
				from.DisplayGuildTitle = true;
				this.Transformed = false;
				ItemID = 0x1F03;
				from.RemoveItem(this);
              			from.EquipItem(this);
			}
		}

		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;
			else if ( RootParent is Mobile && from != RootParent )
				return false;

			Hue = sender.DyedHue;

			return true;
		}
			
		public override void OnRemoved( Object o )
      		{
      			
      			if( o is Mobile && ((Mobile)o).Kills >= 5)
               		{
               			( (Mobile)o).Criminal = true;
                	}
      			if( o is Mobile && ((Mobile)o).GuildTitle != null )
               		{
          			( (Mobile)o).DisplayGuildTitle = true;
                	}
				
      			base.OnRemoved( o );
      		}			

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
