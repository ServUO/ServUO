using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a battle ancient hellhound corpse" )]
    public class BattleAncientHellHound : BaseMount
	{
        public override bool InitialInnocent { get { return true; } }
        public override bool DeleteOnRelease { get { return true; } }

        [Constructable]
        public BattleAncientHellHound()
            : this("A Battle Ancient Hellhound")
        {
        }

		[Constructable]
        public BattleAncientHellHound(string name)
            : base(name, 0x42D, 0x3EC9, AIType.AI_Animal, FightMode.Good, 10, 1, 0.2, 0.4)
		{
            //Body = 1069;
            //ItemID = 16073;
			BaseSoundID = 229;

            SetStr( 589, 800);
            SetDex( 243, 322);
            SetInt( 678, 800);

			SetHits( 1001, 1400);

			SetDamage( 20, 30 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 56, 56 );
			SetResistance( ResistanceType.Fire, 83, 83 );
			SetResistance( ResistanceType.Cold, 40, 40 );
			SetResistance( ResistanceType.Poison, 75, 75 );
			SetResistance( ResistanceType.Energy, 67, 67 );

			SetSkill( SkillName.MagicResist, 120.0, 125.0 );
			SetSkill( SkillName.Anatomy, 120.0, 128.0 );
			SetSkill( SkillName.Tactics, 120.0, 120.0 );
			SetSkill( SkillName.Wrestling, 120.0, 120.0 );
			
			Fame = 1000;
			Karma = -1000;
            Tamable = false;
            ControlSlots = 4;
            MinTameSkill = 125;
		}

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public BattleAncientHellHound(Serial serial)
            : base(serial)
        {
        }

		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override int TreasureMapLevel { get { return 5; } }
		public override int Meat { get { return 16; } }
		public override int Hides { get { return 20; } }
		public override HideType HideType { get { return HideType.Horned; } }

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
