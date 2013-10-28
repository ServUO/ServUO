using System;

namespace Server.Mobiles
{
    [CorpseName("a death adder corpse")]
    public class DeathAdder : BaseFamiliar
    {
        public DeathAdder()
        {
            this.Name = "a death adder";
            this.Body = 0x15;
            this.Hue = 0x455;
            this.BaseSoundID = 219;

            this.SetStr(70);
            this.SetDex(150);
            this.SetInt(100);

            this.SetHits(50);
            this.SetStam(150);
            this.SetMana(0);

            this.SetDamage(1, 4);
            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10);
            this.SetResistance(ResistanceType.Poison, 100);

            this.SetSkill(SkillName.Wrestling, 90.0);
            this.SetSkill(SkillName.Tactics, 50.0);
            this.SetSkill(SkillName.MagicResist, 100.0);
            this.SetSkill(SkillName.Poisoning, 150.0);

            this.ControlSlots = 1;
        }

        public DeathAdder(Serial serial)
            : base(serial)
        {
        }

        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}