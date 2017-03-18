using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an dimetrosaur corpse")]
    public class Dimetrosaur : BaseCreature
    {
        public override bool AttacksFocus { get { return true; } }

        [Constructable]
        public Dimetrosaur()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, .2, .4)
		{
			Name = "a dimetrosaur";
            Body = 1285;
			
            SetStr(526, 601);
			
			SetDex(166, 184);
			SetInt(373, 435);
			
			SetDamage( 18, 21 );
            SetHits(7304, 7622);
			
			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 65, 75 );
			SetResistance( ResistanceType.Energy, 65, 75 );
			
			SetDamageType( ResistanceType.Physical, 90 );
			SetDamageType( ResistanceType.Poison, 10 );
			
			SetSkill( SkillName.MagicResist, 30.1, 43.5 );
			SetSkill( SkillName.Tactics, 30.1, 49.0 );
			SetSkill( SkillName.Wrestling, 40, 50 );
			
			Fame = 17000;
			Karma = -17000;
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
            SetStr(271, 299);
            SetHits(360, 380);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            if (Utility.RandomBool())
                return WeaponAbility.MortalStrike;
            return WeaponAbility.Dismount;
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 11; } }
        public override HideType HideType { get { return HideType.Spined; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies; } }

        public override bool CanAreaPoison { get { return true; } }
        public override Poison HitAreaPoison { get { return Poison.Lethal; } }
        public override int AreaPoisonDamage { get { return 0; } }
        public override double AreaPosionChance { get { return 1.0; } }
        public override TimeSpan AreaPoisonDelay { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40)); } }

        public override void GenerateLoot()
        {
			if(IsChampionSpawn)
                this.AddLoot(LootPack.FilthyRich, 2);
			else
                this.AddLoot(LootPack.UltraRich, 2);
        }

        public Dimetrosaur(Serial serial)
            : base(serial)
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