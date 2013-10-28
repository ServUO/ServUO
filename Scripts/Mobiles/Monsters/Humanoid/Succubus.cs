using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a succubus corpse")]
    public class Succubus : BaseCreature
    {
        [Constructable]
        public Succubus()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a succubus";
            this.Body = 149;
            this.BaseSoundID = 0x4B0;

            this.SetStr(488, 620);
            this.SetDex(121, 170);
            this.SetInt(498, 657);

            this.SetHits(312, 353);

            this.SetDamage(18, 28);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 99.1, 100.0);
            this.SetSkill(SkillName.Meditation, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 100.5, 150.0);
            this.SetSkill(SkillName.Tactics, 80.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 90.0);

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 80;
        }

        public Succubus(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 2);
            this.AddLoot(LootPack.MedScrolls, 2);
        }

        public void DrainLife()
        {
            ArrayList list = new ArrayList();

            foreach (Mobile m in this.GetMobilesInRange(2))
            {
                if (m == this || !this.CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            foreach (Mobile m in list)
            {
                this.DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                //m.SendMessage( "You feel the life drain out of you!" );

                int toDrain = Utility.RandomMinMax(10, 40);

                this.Hits += toDrain;
                m.Damage(toDrain, this);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 >= Utility.RandomDouble())
                this.DrainLife();
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.1 >= Utility.RandomDouble())
                this.DrainLife();
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