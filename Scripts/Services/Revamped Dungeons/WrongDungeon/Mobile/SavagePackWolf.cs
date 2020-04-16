namespace Server.Mobiles
{
    [CorpseName("a wolf corpse")]
    public class SavagePackWolf : BaseCreature
    {
        [Constructable]
        public SavagePackWolf()
            : base(AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.2, 0.4)
        {
            Name = "a savage pack wolf";
            Body = 0xE1;
            BaseSoundID = 0xE5;

            SetStr(100, 116);
            SetDex(51, 61);
            SetInt(11, 21);

            SetHits(650, 671);
            SetMana(0);

            SetDamage(9, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 60.7, 74.0);
            SetSkill(SkillName.Tactics, 80.9, 94.4);
            SetSkill(SkillName.Wrestling, 89.0, 97.1);

            Fame = 450;
            Karma = -450;
        }

        public SavagePackWolf(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer => true;

        public override int Meat => 1;

        public override int Hides => 5;

        public override PackInstinct PackInstinct => PackInstinct.Canine;

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