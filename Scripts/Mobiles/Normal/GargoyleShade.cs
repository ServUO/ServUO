namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class GargoyleShade : BaseCreature
    {
        [Constructable]
        public GargoyleShade()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a gargoyle shade";
            this.Body = 0x4;
            this.Hue = 16385;
            this.BaseSoundID = 0x482;

            this.SetStr(76, 78);
            this.SetDex(76, 81);
            this.SetInt(36, 48);

            this.SetHits(60, 64);
            this.SetStam(80, 81);
            this.SetMana(75, 78);

            this.SetDamage(7, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 60);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 20, 35);

            this.SetSkill(SkillName.EvalInt, 57.1, 65.5);
            this.SetSkill(SkillName.Magery, 60.6, 70.1);
            this.SetSkill(SkillName.MagicResist, 52.6, 70.0);
            this.SetSkill(SkillName.Tactics, 52.7, 60.0);
            this.SetSkill(SkillName.Wrestling, 47.7, 55.0);
            this.SetSkill(SkillName.DetectHidden, 30.0, 40.0);

            this.Fame = 4000;
            this.Karma = -4000;

            this.PackReg(10);
        }

        public GargoyleShade(Serial serial)
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
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override void GenerateLoot()
        {
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
