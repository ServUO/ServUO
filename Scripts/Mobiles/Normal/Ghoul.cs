namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class Ghoul : BaseCreature
    {
        [Constructable]
        public Ghoul()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a ghoul";
            this.Body = 153;
            this.BaseSoundID = 0x482;

            this.SetStr(76, 100);
            this.SetDex(76, 95);
            this.SetInt(36, 60);

            this.SetHits(46, 60);
            this.SetMana(0);

            this.SetDamage(7, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 45.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 55.0);

            this.Fame = 2500;
            this.Karma = -2500;
        }

        public Ghoul(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Regular;

        public override TribeType Tribe => TribeType.Undead;

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
            this.PackItem(Loot.RandomWeapon());
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
