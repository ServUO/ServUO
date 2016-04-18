using System;
using Server.Mobiles;

namespace Server.Items
{
    public class XenrrFishingPole : FishingPole
    {
		public override bool IsArtifact { get { return true; } } 
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
            from.BodyValue = 723;

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
		public override int LabelNumber{ get{ return 1095066; } }  
		        
        [Constructable]
        public XenrrFishingPole() : base()
        {          
            LootType = LootType.Blessed;
            Weight = 1;
        } 

        public XenrrFishingPole( Serial serial ) : base( serial )
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
