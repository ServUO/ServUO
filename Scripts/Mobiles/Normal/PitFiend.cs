/* Copied from deamon, still have to get detailed information on Pit Fiend */
using System;
using Server.Factions;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a pit fiend corpse")]
    public class PitFiend : BaseCreature
    {
        [Constructable]
        public PitFiend()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Pit fiend";
            this.Body = 43;
            this.Hue = 1863;
            this.BaseSoundID = 357;

            this.SetStr(376, 405);
            this.SetDex(176, 195);
            this.SetInt(201, 225);

            this.SetHits(226, 243);

            this.SetDamage(15, 20);

            this.SetSkill(SkillName.EvalInt, 80.1, 90.0);
            this.SetSkill(SkillName.Magery, 80.1, 90.0);
            this.SetSkill(SkillName.MagicResist, 75.1, 85.0);
            this.SetSkill(SkillName.Tactics, 80.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 100.0);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.Fame = 18000;
            this.Karma = -18000;

            this.VirtualArmor = 60;
        }

        public PitFiend(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty
        {
            get
            {
                return 125.0;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 45.0;
            }
        }
        public override Faction FactionAllegiance
        {
            get
            {
                return Shadowlords.Instance;
            }
        }
        public override Ethics.Ethic EthicAllegiance
        {
            get
            {
                return Ethics.Ethic.Evil;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Regular;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average, 2);
            this.AddLoot(LootPack.MedScrolls, 2);
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