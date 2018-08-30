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
    [CorpseName( "an gingerbread thief corpse" )]
    public class GingerbreadThief : BaseCreature
    {
        [Constructable]
        public GingerbreadThief() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "Gingerbread Thief";
            Body = 400;
            Hue = 1002;
			

            SetStr( 796, 825 );
            SetDex( 86, 105 );
            SetInt( 436, 475 );
            SetHits( 478, 495 );
            SetDamage( 20, 30 );


            Item shortpants;
            shortpants = new ShortPants();
            shortpants.Hue = 1108;
            AddItem(shortpants);
            shortpants.LootType = LootType.Newbied;

            Item shirt; 
            shirt = new Shirt();
            shirt.Hue = 1899;
            AddItem(shirt);
            shirt.LootType = LootType.Newbied;

            Item skullcap;
            skullcap = new SkullCap();
            skullcap.Hue = 1899;
            AddItem(skullcap);
            skullcap.LootType = LootType.Newbied;

            Item shoes;
            shoes = new Shoes();
            shoes.Hue = 1108;
            AddItem(shoes);
            shoes.LootType = LootType.Newbied;

          
              HairItemID = 0x203D;
              HairHue = 1118;

             FacialHairItemID = 0x204D;
            FacialHairHue = 1118;



             Cleaver weapon = new Cleaver();
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
 
     
			switch(Utility.Random(3))
			{
				case 0: PackItem(new GingerbreadDough()); break;
			}
			
			return base.OnBeforeDeath();
		}
        public override bool AlwaysMurderer { get { return true; } }
		


        public GingerbreadThief(Serial serial)
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
    
 