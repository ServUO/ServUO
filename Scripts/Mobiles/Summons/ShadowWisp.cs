using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a shadow wisp corpse")]
    public class ShadowWispFamiliar : BaseFamiliar
    {
        private DateTime m_NextFlare;
        public ShadowWispFamiliar()
        {
            this.Name = "a shadow wisp";
            this.Body = 165;
            this.Hue = 0x901;
            this.BaseSoundID = 466;

            this.SetStr(50);
            this.SetDex(60);
            this.SetInt(100);

            this.SetHits(50);
            this.SetStam(60);
            this.SetMana(0);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Energy, 100);

            this.SetResistance(ResistanceType.Physical, 10, 15);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Cold, 10, 15);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 99);

            this.SetSkill(SkillName.Wrestling, 40.0);
            this.SetSkill(SkillName.Tactics, 40.0);

            this.ControlSlots = 1;
        }

        public ShadowWispFamiliar(Serial serial)
            : base(serial)
        {
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow < this.m_NextFlare)
                return;

            this.m_NextFlare = DateTime.UtcNow + TimeSpan.FromSeconds(5.0 + (25.0 * Utility.RandomDouble()));

            this.FixedEffect(0x37C4, 1, 12, 1109, 6);
            this.PlaySound(0x1D3);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), new TimerCallback(Flare));
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

        private void Flare()
        {
            Mobile caster = this.ControlMaster;

            if (caster == null)
                caster = this.SummonMaster;

            if (caster == null)
                return;

            ArrayList list = new ArrayList();
            IPooledEnumerable eable = GetMobilesInRange(5);

            foreach (Mobile m in eable)
            {
                if (m.Player && m.Alive && !m.IsDeadBondedPet && m.Karma <= 0 && m.IsPlayer())
                    list.Add(m);
            }
            eable.Free();

            for (int i = 0; i < list.Count; ++i)
            {
                Mobile m = (Mobile)list[i];
                bool friendly = true;

                for (int j = 0; friendly && j < caster.Aggressors.Count; ++j)
                    friendly = (caster.Aggressors[j].Attacker != m);

                for (int j = 0; friendly && j < caster.Aggressed.Count; ++j)
                    friendly = (caster.Aggressed[j].Defender != m);

                if (friendly)
                {
                    m.FixedEffect(0x37C4, 1, 12, 1109, 3); // At player
                    m.Mana += 1 - (m.Karma / 1000);
                }
            }
        }
    }
}