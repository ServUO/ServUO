//Created by Milva
using System;
using Server;
using Server.Items;
using System.Collections; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles
{
    [CorpseName( "a deer trapper corpse" )]
    public class DeerTrapper : BaseCreature
    {
        [Constructable]
        public DeerTrapper() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "Deer Trapper";
            Body = 400;
            Hue = 1002;
			

            SetStr( 796, 825 );
            SetDex( 86, 105 );
            SetInt( 436, 475 );
            SetHits( 478, 495 );
            SetDamage( 20, 30 );


            Item longpants;
            longpants = new LongPants();
            longpants.Hue = 944;
            AddItem( longpants);
            longpants.LootType = LootType.Newbied;

            Item shirt;
		shirt = new Tunic();
		shirt.Hue = 605;
		AddItem( shirt );
            shirt.LootType = LootType.Newbied;

            
            Item StrawHat;
            StrawHat = new StrawHat();
            StrawHat.Hue = 944;
            AddItem(StrawHat);
            StrawHat.LootType = LootType.Newbied;

            Item boots;
            boots = new Boots();
            boots.Hue = 1109;
            AddItem( boots );
            boots.LootType = LootType.Newbied;

            
            this.FacialHairItemID = 0x203F;
            this.FacialHairHue = 1001;


              this.HairItemID = 0x2049;
              this.HairHue = 1001;

            

             DoubleAxe weapon = new DoubleAxe();
             weapon.Movable = false;
             AddItem(weapon);

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 55, 60 );
            SetResistance( ResistanceType.Fire, 61, 71 );
            SetResistance( ResistanceType.Cold, 30, 40 );
            SetResistance( ResistanceType.Poison, 25, 35 );
            SetResistance( ResistanceType.Energy, 35, 45 );

            SetSkill( SkillName.MagicResist, 99.2, 100 );
            SetSkill( SkillName.Tactics, 97.6, 100 );
            SetSkill( SkillName.Wrestling, 90.1, 92.5 );
            SetSkill( SkillName.Anatomy, 75.1, 78 );

                 

            
			PackGold( 250, 350 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
        }
           
		
		public override bool OnBeforeDeath()
		{
 
     
			switch(Utility.Random(2))
			{
				case 0: PackItem(new ReindeerFood()); break;
			}
			
			return base.OnBeforeDeath();
		}
        public override bool AlwaysMurderer { get { return true; } }



        public DeerTrapper(Serial serial)
            : base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int) 0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
 
		}
	}
}
    
 