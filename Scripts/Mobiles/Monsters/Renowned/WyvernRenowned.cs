using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Wyvern [Renowned] corpse")] 
    public class WyvernRenowned : BaseRenowned
    {
        [Constructable]
        public WyvernRenowned()
            : base(AIType.AI_Mage)
        {
            this.Name = "Wyvern";
            this.Title = "[Renowned]";
            this.Body = 62;
            this.Hue = 243;
            this.BaseSoundID = 362;

            this.SetStr(1364, 1544);
            this.SetDex(144, 160);
            this.SetInt(861, 1081);

            this.SetHits(2782, 2848);

            this.SetDamage(29, 35);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);

            this.SetResistance(ResistanceType.Physical, 61, 66);
            this.SetResistance(ResistanceType.Fire, 67, 89);
            this.SetResistance(ResistanceType.Cold, 61, 77);
            this.SetResistance(ResistanceType.Poison, 56, 62);
            this.SetResistance(ResistanceType.Energy, 53, 63);

            this.SetSkill(SkillName.Magery, 90.1, 94.3);
            this.SetSkill(SkillName.Meditation, 64.1, 70.1);
            this.SetSkill(SkillName.EvalInt, 90.1, 94.3);
            //SetSkill( SkillName.Wrestling, 65.1, 80.0 );

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 70;
			
            this.PackItem(new EssenceDiligence());
        }

        public WyvernRenowned(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(AnimatedLegsoftheInsaneTinker), typeof(PillarOfStrength) };
            }
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override int Meat
        {
            get
            {
                return 10;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Horned;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            this.AddLoot(LootPack.Gems, 5);
        }

        public override int GetAttackSound()
        {
            return 713;
        }

        public override int GetAngerSound()
        {
            return 718;
        }

        public override int GetDeathSound()
        {
            return 716;
        }

        public override int GetHurtSound()
        {
            return 721;
        }

        public override int GetIdleSound()
        {
            return 725;
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