using System;

namespace Server.Mobiles
{
    [CorpseName("a Chiikkaha the Toothed corpse")]
    public class Chiikkaha : RatmanMage
    {
        [Constructable]
        public Chiikkaha()
        {
            this.Name = "Chiikkaha the Toothed";

            this.SetStr(450, 476);
            this.SetDex(157, 179);
            this.SetInt(251, 275);

            this.SetHits(400, 425);

            this.SetDamage(10, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 45);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 100);

            this.SetSkill(SkillName.EvalInt, 70.1, 80.0);
            this.SetSkill(SkillName.Magery, 70.1, 90.0);
            this.SetSkill(SkillName.MagicResist, 65.1, 96.0);
            this.SetSkill(SkillName.Tactics, 50.1, 75.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 75.0);

            this.Fame = 7500;
            this.Karma = -7500;
        }

        public Chiikkaha(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
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