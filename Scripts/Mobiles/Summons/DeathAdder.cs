namespace Server.Mobiles
{
    [CorpseName("a death adder corpse")]
    public class DeathAdder : BaseFamiliar
    {
        public DeathAdder()
        {
            Name = "a death adder";
            Body = 0x15;
            Hue = 0x455;
            BaseSoundID = 219;

            SetStr(70);
            SetDex(150);
            SetInt(100);

            SetHits(50);
            SetStam(150);
            SetMana(0);

            SetDamage(1, 4);
            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 10);
            SetResistance(ResistanceType.Poison, 100);

            SetSkill(SkillName.Wrestling, 90.0);
            SetSkill(SkillName.Tactics, 50.0);
            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Poisoning, 150.0);

            ControlSlots = 1;
        }

        public DeathAdder(Serial serial)
            : base(serial)
        {
        }

        public override Poison HitPoison => (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadEncodedInt();
        }
    }
}
