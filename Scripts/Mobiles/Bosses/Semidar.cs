using Server.Engines.CannedEvil;
using Server.Items;
using System;

namespace Server.Mobiles
{
    public class Semidar : BaseChampion
    {
        [Constructable]
        public Semidar()
            : base(AIType.AI_Mage)
        {
            Name = "Semidar";
            Body = 174;
            BaseSoundID = 0x4B0;

            SetStr(502, 600);
            SetDex(102, 200);
            SetInt(601, 750);

            SetHits(10000);
            SetStam(103, 250);

            SetDamage(29, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 75, 90);
            SetResistance(ResistanceType.Fire, 65, 75);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.EvalInt, 95.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 105.0);
            SetSkill(SkillName.Meditation, 95.1, 100.0);
            SetSkill(SkillName.MagicResist, 120.2, 140.0);
            SetSkill(SkillName.Tactics, 90.1, 105.0);
            SetSkill(SkillName.Wrestling, 90.1, 105.0);

            Fame = 24000;
            Karma = -24000;

            SetSpecialAbility(SpecialAbility.LifeDrain);

            ForceActiveSpeed = 0.3;
            ForcePassiveSpeed = 0.6;
        }

        public Semidar(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType => ChampionSkullType.Pain;
        public override Type[] UniqueList => new[] { typeof(GladiatorsCollar) };
        public override Type[] SharedList => new[] { typeof(RoyalGuardSurvivalKnife), typeof(TheMostKnowledgePerson), typeof(LieutenantOfTheBritannianRoyalGuard) };
        public override Type[] DecorativeList => new[] { typeof(LavaTile), typeof(DemonSkull) };
        public override MonsterStatuetteType[] StatueTypes => new MonsterStatuetteType[] { };
        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 4);
            AddLoot(LootPack.FilthyRich);
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            if (!caster.Female && !caster.IsBodyMod)
                reflect = true; // Always reflect if caster isn't female
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
