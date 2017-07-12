using System;

namespace Server.Mobiles
{
    [CorpseName("a vampire bat corpse")]
    public class VampireBatFamiliar : BaseFamiliar
    {
        public VampireBatFamiliar()
        {
            this.Name = "a vampire bat";
            this.Body = 317;
            this.BaseSoundID = 0x270;

            this.SetStr(120);
            this.SetDex(120);
            this.SetInt(100);

            this.SetHits(90);
            this.SetStam(120);
            this.SetMana(0);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10, 15);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Cold, 10, 15);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.Wrestling, 95.1, 100.0);
            this.SetSkill(SkillName.Tactics, 50.0);

            this.ControlSlots = 1;
        }

        public VampireBatFamiliar(Serial serial)
            : base(serial)
        {
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