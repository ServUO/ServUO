using System;
using System.Collections;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a slasher of veils corpse")]
    public class SlasherOfVeils : BaseSABosses
    {
        private static readonly int[] m_North = new int[]
        {
            -1, -1,
            1, -1,
            -1, 2,
            1, 2
        };
        private static readonly int[] m_East = new int[]
        {
            -1, 0,
            2, 0
        };
        private DateTime m_NextTerror;
        [Constructable]
        public SlasherOfVeils()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "The Slasher of Veils";
            this.Body = 741;

            this.SetStr(967, 1145);
            this.SetDex(129, 139);
            this.SetInt(967, 1145);

            this.SetHits(100000);
            this.SetMana(10000);

            this.SetDamage(10, 15);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 70, 80);
            this.SetResistance(ResistanceType.Poison, 70, 80);
            this.SetResistance(ResistanceType.Energy, 70, 80);

            this.SetSkill(SkillName.Anatomy, 116.1, 120.6);
            this.SetSkill(SkillName.EvalInt, 113.8, 124.7);
            this.SetSkill(SkillName.Magery, 110.1, 123.2);
            this.SetSkill(SkillName.Spellweaving, 110.1, 123.2);
            this.SetSkill(SkillName.Meditation, 118.2, 127.8);
            this.SetSkill(SkillName.MagicResist, 110.0, 123.2);
            this.SetSkill(SkillName.Tactics, 112.2, 122.6);
            this.SetSkill(SkillName.Wrestling, 118.9, 128.6);
        }

        public SlasherOfVeils(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList
        {
            get
            {
                return new Type[] { typeof(ClawsOfTheBerserker), typeof(Lavaliere), typeof(Mangler), typeof(HumanSignOfChaos), typeof(GargishSignOfChaos), typeof(StandardOfChaosG), typeof(StandardOfChaos) };
            }
        }
        public override Type[] SharedSAList
        {
            get
            {
                return new Type[] { typeof(AxesOfFury), typeof(BladeOfBattle), typeof(DemonBridleRing), typeof(PetrifiedSnake), typeof(PillarOfStrength), typeof(SwordOfShatteredHopes), typeof(SummonersKilt), typeof(BreastplateOfTheBerserker) };
            }
        }
        // public override bool GivesSAArtifact { get { return true; } }
        public override bool Unprovokable
        {
            get
            {
                return false;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return false;
            }
        }
        public override int GetIdleSound()
        {
            return 1589;
        }

        public override int GetAngerSound()
        {
            return 1586;
        }

        public override int GetHurtSound()
        {
            return 1588;
        }

        public override int GetDeathSound()
        {
            return 1587;
        }

		public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosSuperBoss, 4);
            this.AddLoot(LootPack.Gems, 8);
        }


        public override void OnThink()
        {
            base.OnThink();

            if (this.Combatant == null)
                return;

            if (this.Hits > 0.6 * this.HitsMax && Utility.RandomDouble() < 0.05)
                this.FireRing();
        }

        public override void FireRing()
        {
            for (int i = 0; i < m_North.Length; i += 2)
            {
                Point3D p = this.Location;

                p.X += m_North[i];
                p.Y += m_North[i + 1];

                IPoint3D po = p as IPoint3D;

                SpellHelper.GetSurfaceTop(ref po);

                Effects.SendLocationEffect(po, this.Map, 0x3E27, 50);
            }

            for (int i = 0; i < m_East.Length; i += 2)
            {
                Point3D p = this.Location;

                p.X += m_East[i];
                p.Y += m_East[i + 1];

                IPoint3D po = p as IPoint3D;

                SpellHelper.GetSurfaceTop(ref po);

                Effects.SendLocationEffect(po, this.Map, 0x3E31, 50);
            }
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            if (this.Map != null && caster != this && 0.70 > Utility.RandomDouble())
            {
                this.Map = caster.Map;
                this.Location = caster.Location;
                this.Combatant = caster;
                Effects.PlaySound(this.Location, this.Map, 0x1FE);
            }

            base.OnDamagedBySpell(caster);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (this.m_NextTerror < DateTime.UtcNow && m != null && this.InRange(m.Location, 10) && m.IsPlayer())
            {
                m.Frozen = true;
                m.SendLocalizedMessage(1080342, this.Name, 33); // Terror slices into your very being, destroying any chance of resisting ~1_name~ you might have had

                Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerStateCallback(Terrorize), m);
            }
        }

        public void DrainMana()
        {
            if (this.Map == null)
                return;

            ArrayList list = new ArrayList();

            foreach (Mobile m in this.GetMobilesInRange(8))
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

                m.SendMessage("You feel the mana drain out of you!");

                int toDrain = Utility.RandomMinMax(40, 60);

                this.Mana += toDrain;
                m.Mana -= toDrain;
                //m.Damage(toDrain, this);
            }
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

        private void Terrorize(object o)
        {
            if (o is Mobile)
            {
                Mobile m = (Mobile)o;

                m.Frozen = false;
                m.SendLocalizedMessage(1005603); // You can move again!

                this.m_NextTerror = DateTime.UtcNow + TimeSpan.FromMinutes(1);
            }
        }
    }
}