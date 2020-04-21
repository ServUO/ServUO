using Server.Engines.CannedEvil;
using Server.Items;
using System;

namespace Server.Mobiles
{
    public class Mephitis : BaseChampion
    {
        [Constructable]
        public Mephitis()
            : base(AIType.AI_Melee)
        {
            Body = 173;
            Name = "Mephitis";

            BaseSoundID = 0x183;

            SetStr(505, 1000);
            SetDex(102, 300);
            SetInt(402, 600);

            SetHits(12000);
            SetStam(105, 600);

            SetDamage(21, 33);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 75, 80);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.MagicResist, 70.7, 140.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);

            Fame = 22500;
            Karma = -22500;

            SetSpecialAbility(SpecialAbility.Webbing);

            ForceActiveSpeed = 0.3;
            ForcePassiveSpeed = 0.6;
        }

        public Mephitis(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType => ChampionSkullType.Venom;
        public override Type[] UniqueList => new[] { typeof(Calm) };
        public override Type[] SharedList => new[] { typeof(OblivionsNeedle), typeof(ANecromancerShroud) };
        public override Type[] DecorativeList => new[] { typeof(Web), typeof(MonsterStatuette) };
        public override MonsterStatuetteType[] StatueTypes => new[] { MonsterStatuetteType.Spider };
        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 4);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
