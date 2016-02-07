using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an efreet corpse")]
    public class Efreet : BaseCreature
    {
        [Constructable]
        public Efreet()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an efreet";
            this.Body = 131;
            this.BaseSoundID = 768;

            this.SetStr(326, 355);
            this.SetDex(266, 285);
            this.SetInt(171, 195);

            this.SetHits(196, 213);

            this.SetDamage(11, 13);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Fire, 50);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 60.1, 75.0);
            this.SetSkill(SkillName.Magery, 60.1, 75.0);
            this.SetSkill(SkillName.MagicResist, 60.1, 75.0);
            this.SetSkill(SkillName.Tactics, 60.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 10000;
            this.Karma = -10000;

            this.VirtualArmor = 56;
        }

        public Efreet(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel
        {
            get
            {
                return Core.AOS ? 4 : 5;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Gems);

            if (0.02 > Utility.RandomDouble())
            {
                switch ( Utility.Random(5) )
                {
                    case 0:
                        this.PackItem(new DaemonArms());
                        break;
                    case 1:
                        this.PackItem(new DaemonChest());
                        break;
                    case 2:
                        this.PackItem(new DaemonGloves());
                        break;
                    case 3:
                        this.PackItem(new DaemonLegs());
                        break;
                    case 4:
                        this.PackItem(new DaemonHelm());
                        break;
                }
            }
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