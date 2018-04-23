//BaleFire//

using System;
using Server;

namespace Server.Items
{
    public class ToxicLight : MetalShield, ITokunoDyable
    {
        //public override int BasePhysicalResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        //public override int BaseColdResistance { get { return 15; } }
        //public override int BaseEnergyResistance { get { return 15; } }
        //public override int BaseFireResistance { get { return 15; } }
        public override int ArtifactRarity{ get{ return 13; } }
        public override int InitMinHits{ get{ return 300; } }
        public override int InitMaxHits{ get{ return 300; } }

        [Constructable]
        public ToxicLight()
        {
            Name = "-Toxic Light-";
			ItemID = 2597;
            Hue = Utility.RandomList( 1267, 1268, 1269, 1270, 1271, 1271, 1372, 1167 );
            StrRequirement = 15;
            //Attributes.BonusStr = 10;
            Attributes.BonusInt = 10;
            //Attributes.BonusDex = 10;
            Attributes.SpellChanneling = 1;
            //Attributes.NightSight = 1;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 10;
			Attributes.ReflectPhysical = 30;
            Attributes.Luck = 150;
            ArmorAttributes.SelfRepair = 3;
            //SkillBonuses.SetValues(0, SkillName.blah, 20.0);
            Light = LightType.Circle300;
            
        }

        public ToxicLight(Serial serial) : base( serial )
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
				}
				else if ( this.ItemID == 2594 )
				{
					this.ItemID = 2597;
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
