using System;
using Server.Mobiles;

namespace Server.Items
{ 
	public class MasterCraftsmanTalisman : BaseTalisman
    {
		private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get 
			{ 
				return m_Charges; 
			}
            set 
			{ 
				m_Charges = value; 
				InvalidateProperties(); 
			}
		}
		
        [Constructable]
        public MasterCraftsmanTalisman(int charges)
            : base(0x2F58)
        { 
            this.SuccessBonus = GetRandomSuccessful();
            this.Blessed = GetRandomBlessed();	
			m_Charges = charges;
		}
		
		public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
			 list.Add(1157213); // Crafting Failure Protection
			 list.Add(1049116, Charges.ToString()); // [ Charges: ~1_CHARGES~ ]
		}

        public MasterCraftsmanTalisman(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1157217;
            }
        }// MasterCraftsmanTalisman
        public override bool ForceShowName
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
			writer.Write( m_Charges );
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			m_Charges = reader.ReadInt();
        }
    }
}
