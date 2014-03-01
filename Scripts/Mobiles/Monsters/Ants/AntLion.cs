using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ant lion corpse")]
    public class AntLion : BaseCreature
    {
        [Constructable]
        public AntLion()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an ant lion";
            this.Body = 787;
            this.BaseSoundID = 1006;

            this.SetStr(296, 320);
            this.SetDex(81, 105);
            this.SetInt(36, 60);

            this.SetHits(151, 162);

            this.SetDamage(7, 21);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Poison, 30);

            this.SetResistance(ResistanceType.Physical, 45, 60);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 30, 35);

            this.SetSkill(SkillName.MagicResist, 70.0);
            this.SetSkill(SkillName.Tactics, 90.0);
            this.SetSkill(SkillName.Wrestling, 90.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 45;

            this.PackItem(new Bone(3));
            this.PackItem(new FertileDirt(Utility.RandomMinMax(1, 5)));

            if (Core.ML && Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomPeculiarSeed(2));

            Item orepile = null; /* no trust, no love :( */

            switch (Utility.Random(4))
            {
                case 0:
                    orepile = new DullCopperOre();
                    break;
                case 1:
                    orepile = new ShadowIronOre();
                    break;
                case 2:
                    orepile = new CopperOre();
                    break;
                default:
                    orepile = new BronzeOre();
                    break;
            }
            orepile.Amount = Utility.RandomMinMax(1, 10);
            orepile.ItemID = 0x19B9;
            this.PackItem(orepile);
            // TODO: skeleton
			
			if ( 0.07 >= Utility.RandomDouble() )
			{
				switch ( Utility.Random( 3 ) )
				{
					case 0: PackItem( new UnknownBardSkeleton() ); break;
					case 1: PackItem( new UnknownMageSkeleton() ); break;
					case 2: PackItem( new UnknownRogueSkeleton() ); break;
				}
			}					
        }		

        public AntLion(Serial serial)
            : base(serial)
        {
        }

        public override int GetAngerSound()
        {
            return 0x5A;
        }

        public override int GetIdleSound()
        {
            return 0x5A;
        }

        public override int GetAttackSound()
        {
            return 0x164;
        }

        public override int GetHurtSound()
        {
            return 0x187;
        }

        public override int GetDeathSound()
        {
            return 0x1BA;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}