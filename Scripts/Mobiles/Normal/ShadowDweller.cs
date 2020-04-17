namespace Server.Mobiles
{
    [CorpseName("a shadow dweller corpse")]
    public class ShadowDweller : BaseCreature
    {
        [Constructable]
        public ShadowDweller()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a shadow dweller";
            this.Body = 740;
            this.Hue = 1;
            this.BaseSoundID = 0x5F1;

            this.SetStr(171, 200);
            this.SetDex(126, 145);
            this.SetInt(276, 305);

            this.SetHits(103, 120);

            this.SetDamage(24, 26);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Cold, 40);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 100.0);
            this.SetSkill(SkillName.Magery, 70.1, 80.0);
            this.SetSkill(SkillName.Meditation, 85.1, 95.0);
            this.SetSkill(SkillName.MagicResist, 80.1, 100.0);
            this.SetSkill(SkillName.Tactics, 70.1, 90.0);

            this.Fame = 8000;
            this.Karma = -8000;

            this.PackNecroReg(17, 24);

            SetSpecialAbility(SpecialAbility.LifeLeech);
        }

        public ShadowDweller(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 3;
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 3);
            this.AddLoot(LootPack.MedScrolls, 2);
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
