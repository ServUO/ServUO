using System;
using System.Collections;
using Server.Engines.CannedEvil;
using Server.Items;

namespace Server.Mobiles
{
    public class Semidar : BaseChampion
    {
        [Constructable]
        public Semidar()
            : base(AIType.AI_Mage)
        {
            this.Name = "Semidar";
            this.Body = 174;
            this.BaseSoundID = 0x4B0;

            this.SetStr(502, 600);
            this.SetDex(102, 200);
            this.SetInt(601, 750);

            this.SetHits(10000);
            this.SetStam(103, 250);

            this.SetDamage(29, 35);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);

            this.SetResistance(ResistanceType.Physical, 75, 90);
            this.SetResistance(ResistanceType.Fire, 65, 75);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 65, 75);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetSkill(SkillName.EvalInt, 95.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 105.0);
            this.SetSkill(SkillName.Meditation, 95.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 120.2, 140.0);
            this.SetSkill(SkillName.Tactics, 90.1, 105.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 105.0);

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 20;
        }

        public Semidar(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType
        {
            get
            {
                return ChampionSkullType.Pain;
            }
        }
        public override Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(GladiatorsCollar) };
            }
        }
        public override Type[] SharedList
        {
            get
            {
                return new Type[] { typeof(RoyalGuardSurvivalKnife), typeof(TheMostKnowledgePerson), typeof(LieutenantOfTheBritannianRoyalGuard) };
            }
        }
        public override Type[] DecorativeList
        {
            get
            {
                return new Type[] { typeof(LavaTile), typeof(DemonSkull) };
            }
        }
        public override MonsterStatuetteType[] StatueTypes
        {
            get
            {
                return new MonsterStatuetteType[] { };
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 4);
            this.AddLoot(LootPack.FilthyRich);
        }

        public override int GetDrainAmount(Mobile m)
        {
            if (m.Female)
                return base.GetDrainAmount(m);

            return base.GetDrainAmount(m) * 2;
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            if (!caster.Female && !caster.IsBodyMod)
                reflect = true; // Always reflect if caster isn't female
        }

        /*public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            if (caster.Body.IsMale)
                scalar = 20; // Male bodies always reflect.. damage scaled 20x
        }*/

        public override bool DrainsLife { get { return false; } }
        public override double DrainsLifeChance { get { return 0.25; } }

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