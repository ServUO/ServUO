using System;

namespace Server.Mobiles
{
    [CorpseName("a dark wolf corpse")]
    public class DarkWolfFamiliar : BaseFamiliar
    {
        private DateTime m_NextRestore;
        public DarkWolfFamiliar()
        {
            this.Name = "a dark wolf";
            this.Body = 99;
            this.Hue = 0x901;
            this.BaseSoundID = 0xE5;

            this.SetStr(100);
            this.SetDex(90);
            this.SetInt(90);

            this.SetHits(60);
            this.SetStam(90);
            this.SetMana(0);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 25, 40);
            this.SetResistance(ResistanceType.Cold, 25, 40);
            this.SetResistance(ResistanceType.Poison, 25, 40);
            this.SetResistance(ResistanceType.Energy, 25, 40);

            this.SetSkill(SkillName.Wrestling, 85.1, 90.0);
            this.SetSkill(SkillName.Tactics, 50.0);

            this.ControlSlots = 1;
        }

        public DarkWolfFamiliar(Serial serial)
            : base(serial)
        {
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow < this.m_NextRestore)
                return;

            this.m_NextRestore = DateTime.UtcNow + TimeSpan.FromSeconds(2.0);

            Mobile caster = this.ControlMaster;

            if (caster == null)
                caster = this.SummonMaster;

            if (caster != null)
                ++caster.Stam;
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