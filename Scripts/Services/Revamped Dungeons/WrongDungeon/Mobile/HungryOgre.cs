using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hungry ogre corpse")]
    public class HungryOgre : BaseCreature
    {
        [Constructable]
        public HungryOgre()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Hungry Ogre";
            this.Body = 0x1;
            this.BaseSoundID = 427;

            this.SetStr(188, 223);
            this.SetDex(62, 79);
            this.SetInt(49, 59);

            this.SetHits(1107, 1205);
            this.SetMana(49, 59);

            this.SetDamage(15, 20);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 70, 80);
            this.SetResistance(ResistanceType.Energy, 60, 70);

            this.SetSkill(SkillName.MagicResist, 61.1, 69.9);
            this.SetSkill(SkillName.Tactics, 102.3, 109.6);
            this.SetSkill(SkillName.Wrestling, 100.9, 108.7);

            this.Fame = 12000;
            this.Karma = -12000;

            this.VirtualArmor = 32;

            this.PackItem(new Club());
        }

        public HungryOgre(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses { get { return true; } }
        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 2; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Potions);
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