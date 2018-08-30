using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a volt spider corpse" )]
	public class VoltSpider : BaseCreature
	{

		[Constructable]
		public VoltSpider () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            int i_Resource = 0;
            i_Resource = Utility.RandomMinMax(1, 25);

            Body = 173;
			BaseSoundID = 362;
            Hue = 0x4fd;
            Name = "Volt Spider";

			SetStr( 721, 760 );
			SetDex( 101, 130 );
			SetInt( 386, 425 );

			SetHits( 433, 456 );

			SetDamage( 17, 25 );

			SetDamageType( ResistanceType.Physical, 50 );
            SetDamageType(ResistanceType.Energy, 50);

			SetResistance( ResistanceType.Physical, 55, 70 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 25, 45 );
			SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance(ResistanceType.Energy, 80, 90);

			SetSkill( SkillName.EvalInt, 89.1, 97.0 );
			SetSkill( SkillName.Magery, 89.1, 98.0 );
			SetSkill( SkillName.MagicResist, 84.1, 94.0 );
			SetSkill( SkillName.Tactics, 87.6, 92.0 );
			SetSkill( SkillName.Wrestling, 83.1, 90.1);



			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 64;

			Tamable = false;
			ControlSlots = 3;
			MinTameSkill = 96.3;



			AddLoot( LootPack.FilthyRich, 2 );
            PackItem(new Gold(250, 400));
			AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
            if (i_Resource > 24) PackItem(new vseggs());
		}

		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 19; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Gold; } }

		public VoltSpider( Serial serial ) : base( serial )
		{
		}
        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (attacker != null && attacker.Alive && attacker.Weapon is BaseRanged && 0.4 > Utility.RandomDouble())
            {
                SendEBolt(attacker);
            }
        }

        public void SendEBolt(Mobile to)
        {
            this.MovingParticles(to, 0x379F, 7, 0, false, true, 0xBE3, 0xFCB, 0x211);
            to.PlaySound(0x229);
            this.DoHarmful(to);
            AOS.Damage(to, this, 50, 0, 0, 0, 0, 100);
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