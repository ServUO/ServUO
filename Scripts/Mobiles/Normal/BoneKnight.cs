using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a skeletal corpse")]
    public class BoneKnight : BaseCreature
    {
        [Constructable]
        public BoneKnight()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a bone knight";
            this.Body = 57;
            this.BaseSoundID = 451;

            this.SetStr(196, 250);
            this.SetDex(76, 95);
            this.SetInt(36, 60);

            this.SetHits(118, 150);

            this.SetDamage(8, 18);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Cold, 60);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 65.1, 80.0);
            this.SetSkill(SkillName.Tactics, 85.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 85.1, 95.0);

            this.Fame = 3000;
            this.Karma = -3000;

            this.VirtualArmor = 40;
			
            switch ( Utility.Random(6) )
            {
                case 0:
                    this.PackItem(new PlateArms());
                    break;
                case 1:
                    this.PackItem(new PlateChest());
                    break;
                case 2:
                    this.PackItem(new PlateGloves());
                    break;
                case 3:
                    this.PackItem(new PlateGorget());
                    break;
                case 4:
                    this.PackItem(new PlateLegs());
                    break;
                case 5:
                    this.PackItem(new PlateHelm());
                    break;
            }

            this.PackItem(new Scimitar());
            this.PackItem(new WoodenShield());
        }

        public BoneKnight(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }

        public override TribeType Tribe { get { return TribeType.Undead; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Meager);
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
