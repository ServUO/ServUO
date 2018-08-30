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
    [CorpseName( "an evil elf corpse" )]
    public class EvilElf : BaseCreature
    {
        [Constructable]
        public EvilElf() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "Evil Elf";
            Body = 400;
            Hue = 1002;
			

            SetStr( 796, 825 );
            SetDex( 86, 105 );
            SetInt( 436, 475 );
            SetHits( 478, 495 );
            SetDamage( 20, 30 );


            Item longpants;
            longpants = new LongPants();
            longpants.Hue = 1368;
            AddItem( longpants);
            longpants.LootType = LootType.Newbied;

            Item tunic;
			tunic = new Tunic();
			tunic.Hue = 1368;
			AddItem( tunic );
            tunic.LootType = LootType.Newbied;

            
            Item JesterHat;
            JesterHat = new JesterHat();
            JesterHat.Hue = 1368;
            AddItem(JesterHat);
            JesterHat.LootType = LootType.Newbied;

            Item boots;
            boots = new Boots();
            boots.Hue = 1109;
            AddItem( boots );
            boots.LootType = LootType.Newbied;

            
         FacialHairItemID = 0x204B;
            FacialHairHue = 996;


              HairItemID = 0x203C;
              HairHue = 996;

            Item leatherninjabelt;
            leatherninjabelt = new LeatherNinjaBelt();
            leatherninjabelt.Hue = 1109;
            AddItem( leatherninjabelt );
            leatherninjabelt.LootType = LootType.Newbied;

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
 
     
			switch(Utility.Random(3))
			{
				case 0: PackItem(new RedPaintBarrel()); break;
			}
			
			return base.OnBeforeDeath();
		}
        public override bool AlwaysMurderer { get { return true; } }



        public EvilElf(Serial serial)
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
    
 