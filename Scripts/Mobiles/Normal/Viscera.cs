namespace Server.Mobiles
{
    [CorpseName("a Visceral corpse")]
    public class Viscera : BaseCreature
    {
        [Constructable]
        public Viscera()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Viscera";
            Body = 775;
            Hue = 2718;
            BaseSoundID = 679;
            SpeechHue = 0x3B2;

            SetStr(250, 293);
            SetDex(120, 130);
            SetInt(100);

            SetHits(200, 230);

            SetDamage(8, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 30, 50);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 85.0, 100.0);
            SetSkill(SkillName.Wrestling, 85.0, 100.0);
            SetSkill(SkillName.Poisoning, 20.0, 30.0);
            SetSkill(SkillName.DetectHidden, 50.0);

            Fame = 2000;
            Karma = -2000;

            SetAreaEffect(AreaEffect.AuraOfNausea);
            SetSpecialAbility(SpecialAbility.StickySkin);
        }

        public Viscera(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Gems, Utility.Random(1, 3));
        }

        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;

        public override int GetIdleSound()
        {
            return 0x1BF;
        }

        public override int GetAttackSound()
        {
            return 0x1C0;
        }

        public override int GetHurtSound()
        {
            return 0x1C1;
        }

        public override int GetDeathSound()
        {
            return 0x1C2;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
