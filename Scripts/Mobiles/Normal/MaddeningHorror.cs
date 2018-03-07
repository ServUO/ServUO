using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a maddening horror corpse")]
    public class MaddeningHorror : BaseCreature
    {
        [Constructable]
        public MaddeningHorror()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a maddening horror";
            this.Body = 721;

            this.SetStr(270, 290);
            this.SetDex(80, 100);
            this.SetInt(850);

            this.SetHits(660);

            this.SetDamage(15, 27);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 40);
            this.SetDamageType(ResistanceType.Energy, 40);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.EvalInt, 120.0, 130.0);
            this.SetSkill(SkillName.Magery, 120.0, 130.0);
            this.SetSkill(SkillName.Meditation, 100.0, 110.0);
            this.SetSkill(SkillName.MagicResist, 180.0, 195.0);
            this.SetSkill(SkillName.Tactics, 95.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.0, 85.0);
            this.SetSkill(SkillName.Poisoning, 110.0);
            this.SetSkill(SkillName.DetectHidden, 100.0);
            this.SetSkill(SkillName.Necromancy, 120.0);
            this.SetSkill(SkillName.SpiritSpeak, 120.0);

            this.Fame = 23000;
            this.Karma = -23000;
        }

        public MaddeningHorror(Serial serial)
            : base(serial)
        {
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.25 >= Utility.RandomDouble())
                this.DrainMana();
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.25 >= Utility.RandomDouble())
                this.DrainMana();
        }

        public void DrainMana()
        {
            if (this.Map == null)
                return;

            ArrayList list = new ArrayList();
            IPooledEnumerable eable = GetMobilesInRange(8);

            foreach (Mobile m in eable)
            {
                if (m == this || !this.CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            eable.Free();

            foreach (Mobile m in list)
            {
                this.DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                m.SendMessage("You feel the mana drain out of you!");

                int toDrain = Utility.RandomMinMax(40, 60);

                this.Mana += toDrain;
                m.Mana -= toDrain;
            }
        }

        public override int GetIdleSound()
        {
            return 1553;
        }

        public override int GetAngerSound()
        {
            return 1550;
        }

        public override int GetHurtSound()
        {
            return 1552;
        }

        public override int GetDeathSound()
        {
            return 1551;
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