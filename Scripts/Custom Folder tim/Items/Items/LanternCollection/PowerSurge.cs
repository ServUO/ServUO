using System;
using Server;

namespace Server.Items
{
    public class PowerSurge : MetalShield, ITokunoDyable
    {
        public override int BaseEnergyResistance{ get{ return 15; } }
		public override int BaseFireResistance{ get{ return 0; } }
        public override int ArtifactRarity{ get{ return 15; } }
        public override int InitMinHits{ get{ return 255; } }
        public override int InitMaxHits{ get{ return 255; } }

        [Constructable]
        public PowerSurge()
        {
            Name = "Power Surge";
			ItemID = 2597;
            Hue = Utility.RandomList( 1158, 1159, 1163, 1168, 1170, 16 );
            StrRequirement = 45;
            Attributes.SpellChanneling = 1;
            Attributes.NightSight = 1;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 10;
			Attributes.ReflectPhysical = 15;
            Attributes.Luck = 150;
            ArmorAttributes.SelfRepair = 3;
			Attributes.NightSight = 1;
            
        }

        public PowerSurge(Serial serial) : base( serial )
        {
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}

			else
			{

				if ( this.ItemID == 2597 )
				{
					this.ItemID = 2594;
					this.Attributes.NightSight = 1;
				}
				else if ( this.ItemID == 2594 )
				{
					this.ItemID = 2597;
					this.Attributes.NightSight = 1;
				}
				else if (this.ItemID != 2597 || this.ItemID != 2594 )
				{
					from.SendMessage("There was a problem lighting your lantern. Please contact a staff member");				
				}
				else
				{
					from.SendMessage( "Your lantern is broken. Please contact a staff member to repair it!" );
				}
			}
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
    } // End Class
} // End Namespace
