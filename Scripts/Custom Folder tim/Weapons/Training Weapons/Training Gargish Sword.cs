/*********************************************************************
 * 
 * The original weapon pack was posted to RunUO on 18AUG2005 by Triple. 
 * http://www.runuo.com/community/threads/training-weapons.57430/ 
 * 
 * It included: Training Katana, Training Kryss, Training Mace, and Training Bow.
 * 
 * I added the Training Buckler, Training Gargish Boomerang, Training Gargish Kryss,
 * Training Gargish Mace, Training Gargish Shield, and Training Gargish Sword. 
 * 
 * I had to work on them some to get all the Special Abilities to work. Now you can use 
 * your specials to get your mana down, so you can work your Meditation and Focus while 
 * training fighting skills. When used in conjunction with Training Elementals you can 
 * set up a nice training area. Hopefully you will find this script in a package with the set.
 * 
 * Tukaram 21MAY2016
 * 
 * ********************************************************************/
 
using System;
using Server;

namespace Server.Items
{
    public class GargishTrainingSword : Katana
    {

        public override int LabelNumber{ get{ return 1061097; } } // GargishTraining Katana


            public override int InitMinHits { get { return 2600; } }
            public override int InitMaxHits { get { return 2600; } }

            public override int AosMinDamage { get { return 1; } }
            public override int AosMaxDamage { get { return 1; } }
            public override int AosSpeed { get { return 56; } }

            public override int OldStrengthReq { get { return 10; } }
            public override int OldMinDamage { get { return 1; } }
            public override int OldMaxDamage { get { return 1; } }
            public override int OldSpeed { get { return 68; } }

            public override Race RequiredRace { get { return Race.Gargoyle; } }
            public override bool CanBeWornByGargoyles { get { return true; } }

            [Constructable]
            public GargishTrainingSword()
            {

                Name = "A Gargish Training Sword";
                ItemID = 2312;
            Hue = 220;
            //Layer = Layer.OneHanded;
                Attributes.WeaponSpeed = 20;
            
            }

            public override WeaponAbility PrimaryAbility { get { return WeaponAbility.WhirlwindAttack; } }
            public override WeaponAbility SecondaryAbility { get { return WeaponAbility.Dismount; } }

            public GargishTrainingSword( Serial serial ) : base( serial )
		{
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
