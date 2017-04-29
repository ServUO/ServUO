using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a clan ribbon supplicant corpse")]
    public class ClanRS : BaseCreature
    {
        //public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Ratman; } }
        [Constructable]
        public ClanRS()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Clan Ribbon Supplicant";
            this.Body = 42;
            this.Hue = 2952;
            this.BaseSoundID = 437;

            this.SetStr(173);
            this.SetDex(117);
            this.SetInt(207);

            this.SetHits(127);

            this.SetDamage(7, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 60);
            this.SetResistance(ResistanceType.Fire, 30, 35);
            this.SetResistance(ResistanceType.Cold, 80, 85);
            this.SetResistance(ResistanceType.Poison, 45, 50);
            this.SetResistance(ResistanceType.Energy, 25, 30);

            this.SetSkill(SkillName.MagicResist, 78.5, 80.0);
            this.SetSkill(SkillName.Tactics, 62.1, 65.0);
            this.SetSkill(SkillName.Wrestling, 56.5, 60.0);

            this.Fame = 1500;
            this.Karma = -1500;

            this.VirtualArmor = 48;
        }

        public ClanRS(Serial serial)
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