using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Skeletal Dragon [Renowned] corpse")]  
    public class SkeletalDragonRenowned : BaseRenowned
    {
        [Constructable]
        public SkeletalDragonRenowned()
            : base(AIType.AI_Mage)
        {
            Name = "Skeletal Dragon";
            Title = "[Renowned]";
            Body = 104;
            BaseSoundID = 0x488;

            Hue = 906;

            SetStr(898, 1030);
            SetDex(100, 200);
            SetInt(488, 620);

            SetHits(558, 599);

            SetDamage(29, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 75, 80);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 80.1, 100.0);
            SetSkill(SkillName.Magery, 80.1, 100.0);
            SetSkill(SkillName.MagicResist, 100.3, 130.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 80;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public SkeletalDragonRenowned(Serial serial)
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
                return new Type[] { typeof(AxeOfAbandon), typeof(DemonBridleRing), typeof(VoidInfusedKilt), typeof(BladeOfBattle) };
            }
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override double BonusPetDamageScalar
        {
            get
            {
                return (Core.SE) ? 3.0 : 1.0;
            }
        }
        // TODO: Undead summoning?
        public override bool AutoDispel
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
                return Poison.Lethal;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }// where's it hiding these? :)
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
                return HideType.Barbed;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 3);
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
