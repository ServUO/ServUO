using System;

namespace Server.Mobiles
{
    [CorpseName("a bulbous putrification corpse")]
    public class BulbousPutrification : BaseCreature
    {
        [Constructable]
        public BulbousPutrification()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a bulbous putrification";
            this.Body = 0x307;
            this.Hue = 0x55C;
            this.BaseSoundID = 0x165;

            this.SetStr(755, 800);
            this.SetDex(53, 60);
            this.SetInt(51, 59);

            this.SetHits(1211, 1231);

            this.SetDamage(22, 29);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 55, 70);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.Wrestling, 104.8, 114.7);
            this.SetSkill(SkillName.Tactics, 111.9, 119.1);
            this.SetSkill(SkillName.MagicResist, 55.5, 64.1);
            this.SetSkill(SkillName.Anatomy, 110.0);
            this.SetSkill(SkillName.Poisoning, 80.0);	
        }

        public BulbousPutrification(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosFilthyRich, 5);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}