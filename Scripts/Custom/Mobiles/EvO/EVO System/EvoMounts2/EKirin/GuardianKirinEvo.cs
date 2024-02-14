using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;


namespace Xanthos.Evo
{
    [CorpseName("guardian kirin corpse")]
	public class GuardianKirin : EvoKirin
	{
		public override bool AddPointsOnDamage { get { return false; } }
		public override bool AddPointsOnMelee { get { return true; } }

        [Constructable]
		public GuardianKirin() : base( "A Guardian Kirin" )
		{
		

		Name = "Guardian Evo Kirin";
            Body = 0X84;
            Hue = 0;


            SetStr( 260 );
            SetDex( 160 );
            SetInt( 252 );
                                               
            SetHits( 1800 );
            SetMana( 240 );
            SetStam( 100);
                                               
           	SetDamage(  33 );
                                               
            SetDamageType( ResistanceType.Physical, 50 );
            SetDamageType( ResistanceType.Cold, 50 );
            SetDamageType( ResistanceType.Energy, 50 );

            SetResistance( ResistanceType.Physical, 55 );
            SetResistance( ResistanceType.Cold, 81 );
            SetResistance( ResistanceType.Fire, 35 );
            SetResistance( ResistanceType.Energy, 77 );
            SetResistance( ResistanceType.Poison, 44 );
                                               
            SetSkill( SkillName.Anatomy, 70 );
            SetSkill( SkillName.Magery, 70 );
            SetSkill( SkillName.Meditation, 90 );
            SetSkill( SkillName.MagicResist, 85 );
            SetSkill( SkillName.Wrestling, 95 );
            SetSkill( SkillName.Tactics, 95 );
                                               
            VirtualArmor = 30;

			

				{
				}
			
			Tamable = false;	// Not appropriate as a pet
			
		

		switch ( Utility.Random( 20 ))
            {
                case 0: AddItem(new KirinEvoEgg()); break;
            }	
		
		}
			
          public override void GenerateLoot()
            {
            PackGold( 100 );
			AddLoot( LootPack.Gems, Utility.Random( 1, 5));
             }

          public GuardianKirin(Serial serial)
              : base(serial)
          {
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write( (int)0 );			
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
