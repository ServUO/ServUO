using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a drake corpse" )]
	public class ColdDrake : BaseCreature
	{
		[Constructable]
		public ColdDrake () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a cold drake";
			Body = Utility.RandomList( 60, 61 );
			BaseSoundID = 362;

            Hue = Utility.RandomMinMax(1319, 1327);

            SetStr(610, 670);
			SetDex(130, 160);
			SetInt(150, 190);

			SetHits( 450, 500 );

			SetDamage( 17, 20 );

			SetDamageType( ResistanceType.Physical, 50 );
            SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 50, 65 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 75, 90 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.MagicResist, 95, 110 );
			SetSkill( SkillName.Tactics, 115, 140 );
			SetSkill( SkillName.Wrestling, 115, 126 );

			Fame = 12000;
			Karma = -12000;

			VirtualArmor = 60;

            PackReg(3);

            for (int i = 0; i <= 1; i++)
            {
                Item item;

                if (Utility.RandomBool())
                    item = Loot.RandomScroll(0, Loot.NecromancyScrollTypes.Length, SpellbookType.Necromancer);
                else
                    item = Loot.RandomScroll(0, Loot.RegularScrollTypes.Length, SpellbookType.Regular);

                PackItem(item);
            }
		}

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (corpse != null)
                corpse.DropItem(new DragonBlood(8));

            base.OnCarve(from, corpse, with);
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 22; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		//public override int Scales{ get{ return 3; } }
		//public override ScaleType ScaleType{ get{ return ScaleType.Blue; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish; } }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathColdDamage { get { return 100; } }
        public override int BreathEffectHue { get { return 1264; } }

        public override bool CanAreaDamage { get { return true; } }
        public override int AreaDamageRange { get { return 10; } }
        public override double AreaDamageScalar { get { return 1.0; } }
        public override double AreaDamageChance { get { return 1.0; } }
        public override TimeSpan AreaDamageDelay { get { return TimeSpan.FromSeconds(30); } }

        public override int AreaFireDamage { get { return 0; } }
        public override int AreaColdDamage { get { return 100; } }

        public override void AreaDamageEffect(Mobile m)
        {
            m.FixedParticles(0x374A, 10, 30, 5052, this.Hue, 0, EffectLayer.Waist);
            m.PlaySound(0x5C6);
        }	

		public ColdDrake( Serial serial ) : base( serial )
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
