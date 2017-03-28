using System;
using Server.Items;

namespace Server.Mobiles 
{
	[CorpseName( "a human corpse" )]
	public class DupresSquire : BaseCreature
	{		
		[Constructable]
		public DupresSquire() : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName("male");
            Title = "The Squire";
            Body = 0x190;
			Hue = Utility.RandomSkinHue();
            Female = false;

            SetStr( 190, 200 );
			SetDex( 50, 75 );
			SetInt( 150, 250  );
			SetHits( 3900, 4100 );
			SetDamage( 22, 28 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 50, 70 );
			SetResistance( ResistanceType.Fire, 50, 70 );
			SetResistance( ResistanceType.Cold, 50, 70 );
			SetResistance( ResistanceType.Poison, 50, 70 );
			SetResistance( ResistanceType.Energy, 50, 70 );

			SetSkill( SkillName.EvalInt, 195.0, 220.0 );
			SetSkill( SkillName.Magery, 195.0, 220.0 );
			SetSkill( SkillName.Meditation, 195.0, 200.0 );
			SetSkill( SkillName.MagicResist, 100.0, 120.0 );
			SetSkill( SkillName.Tactics, 195.0, 220.0 );
			SetSkill( SkillName.Wrestling, 195.0, 220.0 );

			VirtualArmor = 70;

            Item vikingsword = new VikingSword();
            vikingsword.LootType = LootType.Blessed;
            SetWearable(vikingsword);

            Item cc = new ChainChest();
            cc.LootType = LootType.Blessed;
            SetWearable(cc);

            Item cl = new ChainLegs();
            cl.LootType = LootType.Blessed;
            SetWearable(cl);

            Item ch = new CloseHelm();
            ch.LootType = LootType.Blessed;
            SetWearable(ch);

            Item boots = new Boots(1);
            boots.LootType = LootType.Blessed;
            SetWearable(boots);

            Item pgl = new PlateGloves();
            pgl.LootType = LootType.Blessed;
            SetWearable(pgl);

            Item mks = new MetalKiteShield();
            mks.LootType = LootType.Blessed;
            mks.Hue = 0x776;
            SetWearable(mks);

            Item bs = new BodySash(0x794); // dark purple
            bs.LootType = LootType.Blessed;
            SetWearable(bs);

            PackGold( 400, 600 );            
		}

        public override void OnKilledBy(Mobile m)
        {
            base.OnKilledBy(m);

            if (Utility.RandomDouble() < 0.1)
            {
                ExodusChest.GiveRituelItem(m);
            }
        }

        public override bool CanBeParagon { get { return false; } }
        public override bool InitialInnocent { get { return true; } }
        public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 5; } }		

        public DupresSquire(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
