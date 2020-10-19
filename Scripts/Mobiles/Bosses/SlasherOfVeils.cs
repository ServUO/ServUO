using Server.Items;
using Server.Spells;
using System;

namespace Server.Mobiles
{
    [CorpseName("a slasher of veils corpse")]
    public class SlasherOfVeils : BaseSABoss
    {
        private static readonly int[] m_North =
        {
            -1, -1,
            1, -1,
            -1, 2,
            1, 2
        };
        private static readonly int[] m_East =
        {
            -1, 0,
            2, 0
        };

        [Constructable]
        public SlasherOfVeils()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "The Slasher of Veils";
            Body = 741;

            SetStr(901, 1010);
            SetDex(127, 153);
            SetInt(1078, 1263);

            SetHits(50000, 65000);
            SetMana(10000);

            SetDamage(10, 15);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 65, 80);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.Anatomy, 110.8, 129.7);
            SetSkill(SkillName.EvalInt, 113.4, 130);
            SetSkill(SkillName.Magery, 111.7, 130);
            SetSkill(SkillName.Spellweaving, 111.1, 125);
            SetSkill(SkillName.Meditation, 113.5, 129.9);
            SetSkill(SkillName.MagicResist, 110, 129.8);
            SetSkill(SkillName.Tactics, 110.5, 126.3);
            SetSkill(SkillName.Wrestling, 110.1, 130);
            SetSkill(SkillName.DetectHidden, 127.1);

            Fame = 35000;
            Karma = -35000;

            SetSpecialAbility(SpecialAbility.AngryFire);
            SetSpecialAbility(SpecialAbility.ManaDrain);
            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
            SetSpecialAbility(SpecialAbility.TrueFear);
        }

        public SlasherOfVeils(Serial serial)
            : base(serial)
        {
        }

        public override Type[] UniqueSAList => new[] { typeof(ClawsOfTheBerserker), typeof(Lavaliere), typeof(Mangler), typeof(HumanSignOfChaos), typeof(GargishSignOfChaos), typeof(StandardOfChaosG), typeof(StandardOfChaos) };
        public override Type[] SharedSAList => new[] { typeof(AxesOfFury), typeof(BladeOfBattle), typeof(DemonBridleRing), typeof(PetrifiedSnake), typeof(PillarOfStrength), typeof(SwordOfShatteredHopes), typeof(SummonersKilt) };

        public override bool Unprovokable => false;
        public override bool BardImmune => false;

        [CommandProperty(AccessLevel.Counselor)]
        public override int PhysicalResistance => Weakened ? base.PhysicalResistance / 2 : base.PhysicalResistance;

        [CommandProperty(AccessLevel.Counselor)]
        public override int FireResistance => Weakened ? base.FireResistance / 2 : base.FireResistance;

        [CommandProperty(AccessLevel.Counselor)]
        public override int ColdResistance => Weakened ? base.ColdResistance / 2 : base.ColdResistance;

        [CommandProperty(AccessLevel.Counselor)]
        public override int PoisonResistance => Weakened ? base.PoisonResistance / 2 : base.PoisonResistance;

        [CommandProperty(AccessLevel.Counselor)]
        public override int EnergyResistance => Weakened ? base.EnergyResistance / 2 : base.EnergyResistance;

        private static readonly Rectangle2D _WeakBounds = new Rectangle2D(740, 466, 20, 20);

        public bool Weakened => Map == Map.TerMur && _WeakBounds.Contains(this);

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

        public override bool AlwaysMurderer => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 4);
            AddLoot(LootPack.Gems, 8);
        }

        public override void FireRing()
        {
            for (int i = 0; i < m_North.Length; i += 2)
            {
                Point3D p = Location;

                p.X += m_North[i];
                p.Y += m_North[i + 1];

                IPoint3D po = p;

                SpellHelper.GetSurfaceTop(ref po);

                Effects.SendLocationEffect(po, Map, 0x3E27, 50);
            }

            for (int i = 0; i < m_East.Length; i += 2)
            {
                Point3D p = Location;

                p.X += m_East[i];
                p.Y += m_East[i + 1];

                IPoint3D po = p;

                SpellHelper.GetSurfaceTop(ref po);

                Effects.SendLocationEffect(po, Map, 0x3E31, 50);
            }
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            if (0.5 > Utility.RandomDouble() && caster.InRange(Location, 10) && Map != null && caster.Alive && caster != this && caster.Map == Map)
            {
                MoveToWorld(caster.Location, Map);

                Timer.DelayCall(() =>
                {
                    Combatant = caster;
                });

                Effects.PlaySound(Location, Map, 0x1FE);
            }

            base.OnDamagedBySpell(caster);
        }

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
