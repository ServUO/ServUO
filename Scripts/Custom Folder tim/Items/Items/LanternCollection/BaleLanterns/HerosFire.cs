//BaleFire//

using System;
using Server;

namespace Server.Items
{
    public class HeroFlame : MetalShield, ITokunoDyable
    {
        //public override int BasePhysicalResistance { get { return 15; } }
        //public override int BasePoisonResistance { get { return 15; } }
        //public override int BaseColdResistance { get { return 15; } }
        //public override int BaseEnergyResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int ArtifactRarity{ get{ return 13; } }
        public override int InitMinHits{ get{ return 300; } }
        public override int InitMaxHits{ get{ return 300; } }

        [Constructable]
        public HeroFlame()
        {
            Name = "-Heros Fury-";
			ItemID = 2597;
            Hue = Utility.RandomList( 1355, 1356, 1357, 1358, 1359, 1360, 1161, 1260 );
            StrRequirement = 15;
            Attributes.BonusStr = 10;
            //Attributes.BonusInt = 10;
            //Attributes.BonusDex = 10;
            Attributes.SpellChanneling = 1;
            Attributes.AttackChance = 15;
            //Attributes.DefendChance = 10;
			Attributes.ReflectPhysical = 20;
            Attributes.Luck = 150;
            ArmorAttributes.SelfRepair = 3;
            SkillBonuses.SetValues(0, SkillName.Fencing, 10.0);
            //SkillBonuses.SetValues(0, SkillName.MaceFighting, 10.0);
            //SkillBonuses.SetValues(0, SkillName.Swordsmanship, 10.0);
            Light = LightType.Circle300;
            
        }

        public HeroFlame(Serial serial): base(serial)
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
