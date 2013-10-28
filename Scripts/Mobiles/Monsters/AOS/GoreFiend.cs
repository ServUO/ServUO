using System;

namespace Server.Mobiles
{
    [CorpseName("a gore fiend corpse")]
    public class GoreFiend : BaseCreature
    {
        [Constructable]
        public GoreFiend()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a gore fiend";
            this.Body = 305;
            this.BaseSoundID = 224;

            this.SetStr(161, 185);
            this.SetDex(41, 65);
            this.SetInt(46, 70);

            this.SetHits(97, 111);

            this.SetDamage(15, 21);

            this.SetDamageType(ResistanceType.Physical, 85);
            this.SetDamageType(ResistanceType.Poison, 15);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 5, 15);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 40.1, 55.0);
            this.SetSkill(SkillName.Tactics, 45.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 70.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 24;
        }

        public GoreFiend(Serial serial)
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override int GetDeathSound()
        {
            return 1218;
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