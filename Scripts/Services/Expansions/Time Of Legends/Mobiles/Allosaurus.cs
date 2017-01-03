using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an allosaurus corpse")]
	public class Allosaurus : BaseCreature
	{
        public override bool AttacksFocus { get { return true; } }

        [Constructable]
		public Allosaurus() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
		{
			Name = "allosaurus";
            Body = 1290;
			
			SetStr(699, 828);
			SetDex(200);
			SetInt(127, 150);
			
			SetDamage( 21, 23 );
			
			SetHits(18000);
			SetMana(48, 70);
			
			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 55, 65 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 90, 100 );
			SetResistance( ResistanceType.Energy, 60, 70 );
			
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 50 );
			
			SetSkill( SkillName.MagicResist, 100, 110 );
			SetSkill( SkillName.Tactics, 120, 140 );
			SetSkill( SkillName.Wrestling, 120, 150 );
			
			Fame = 21000;
			Karma = -21000;
		}
		
		public override void GenerateLoot()
        {
			if(IsChampionSpawn)
                this.AddLoot(LootPack.FilthyRich, 3);
			else
                this.AddLoot(LootPack.UltraRich, 3);
        }
		
		public override int GetIdleSound()
        {
            return 0x2C4;
        }

        public override int GetAttackSound()
        {
            return 0x2C0;
        }

        public override int GetDeathSound()
        {
            return 0x2C1;
        }

        public override int GetAngerSound()
        {
            return 0x2C4;
        }

        public override int GetHurtSound()
        {
            return 0x2C3;
        }

		public override void SetToChampionSpawn()
		{
			SetStr(347, 387);
			SetHits(940, 1000);
		}
		
		public override WeaponAbility GetWeaponAbility()
        {
            if(Utility.RandomBool())
				return WeaponAbility.Disarm;
			return WeaponAbility.ArmorPierce;
        }
		
		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 11; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		
		public Allosaurus(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}