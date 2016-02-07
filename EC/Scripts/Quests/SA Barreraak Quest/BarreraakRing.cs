using System;
using Server.Mobiles;

/*
** Allows staff to quickly switch between player and their assigned staff levels by equipping or removing the cloak
** Also allows instant teleportation to a specified destination when double-clicked by the staff member.
*/

namespace Server.Items
{
    public class BarreraakRing : GoldRing
    {            
        private int m_BodyInit;

        [CommandProperty( AccessLevel.Administrator )]
        public int BodyInit
		{ 
            get 
            { 
                return m_BodyInit;
            }
            set 
            { 
                m_BodyInit = value;
                InvalidateProperties();
            }
        }
        
        public override bool OnEquip( Mobile from )
	{
            BodyInit = from.BodyValue;
            from.BodyValue = 334;

	    return base.OnEquip( from );
	}

        public override void OnRemoved( object parent )
        {
            base.OnRemoved( parent );

            if ( parent is Mobile && !Deleted)
            {
                Mobile m = (Mobile) parent;               
                
                m.BodyValue = BodyInit;
            }
        }        
		public override int LabelNumber{ get{ return 1095049; } }  
		        
        [Constructable]
        public BarreraakRing() : base()
        {          
            LootType = LootType.Blessed;
            Weight = 1;
        } 

        public BarreraakRing( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        { 
            base.Serialize( writer );
			
            writer.Write( (int) 0 );
            writer.Write( (int) m_BodyInit );
        } 

        public override void Deserialize(GenericReader reader) 
        { 
                base.Deserialize( reader );
                int version = reader.ReadInt();                
                m_BodyInit = reader.ReadInt();        
        }
    }
}
