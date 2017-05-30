using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a clan scratch henchrat corpse")]
    public class ClanSH : BaseCreature
    {
        [Constructable]
        public ClanSH()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Clan Scratch Henchrat";
            this.Body = 42;
            this.BaseSoundID = 437;

            this.SetStr(227);
            this.SetDex(183);
            this.SetInt(93);

            this.SetHits(2065);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 26, 30);
            this.SetResistance(ResistanceType.Fire, 29, 35);
            this.SetResistance(ResistanceType.Cold, 30, 35);
            this.SetResistance(ResistanceType.Poison, 15, 20);
            this.SetResistance(ResistanceType.Energy, 13, 15);

            this.SetSkill(SkillName.MagicResist, 35.4, 40.0);
            this.SetSkill(SkillName.Tactics, 61.1, 65.0);
            this.SetSkill(SkillName.Wrestling, 64.0, 65.0);
            this.SetSkill(SkillName.Anatomy, 74.0, 75.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 48;
        }

        public ClanSH(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int Hides
        {
            get
            {
                return 8;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 3);	
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