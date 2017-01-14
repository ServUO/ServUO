using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a satyr's corpse")]
    public class Satyr : BaseCreature
    {
        [Constructable]
        public Satyr()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a satyr";
            this.Body = 271;
            this.BaseSoundID = 0x586;

            this.SetStr(177, 195);
            this.SetDex(251, 269);
            this.SetInt(153, 170);

            this.SetHits(350, 400);

            this.SetDamage(13, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 60);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 55.0, 65.0);
            this.SetSkill(SkillName.Tactics, 80.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.0, 100.0);

            this.SetSkill(SkillName.Musicianship, 100);
            this.SetSkill(SkillName.Discordance, 100);
            this.SetSkill(SkillName.Provocation, 100);
            this.SetSkill(SkillName.Peacemaking, 100);

            this.Fame = 5000;
            this.Karma = 0;

            this.VirtualArmor = 28; // Don't know what it should be

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.MlRich);
            this.AddLoot(LootPack.MedScrolls);
        }

        public override bool CanDiscord { get { return true; } }
        public override bool CanPeace { get { return true; } }
        public override bool CanProvoke { get { return true; } }

        public override TimeSpan DiscordInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)); } }
        public override TimeSpan PeaceInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)); } }
        public override TimeSpan ProvokeInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)); } }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }

        public Satyr(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}