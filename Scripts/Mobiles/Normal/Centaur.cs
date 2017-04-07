using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a centaur corpse")]
    public class Centaur : BaseCreature
    {
        [Constructable]
        public Centaur()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("centaur");
            this.Body = 101;
            this.BaseSoundID = 679;

            this.SetStr(202, 300);
            this.SetDex(104, 260);
            this.SetInt(91, 100);

            this.SetHits(130, 172);

            this.SetDamage(13, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 35, 45);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 45, 55);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.Anatomy, 95.1, 115.0);
            this.SetSkill(SkillName.Archery, 95.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 50.3, 80.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 95.1, 100.0);

            this.Fame = 6500;
            this.Karma = 0;

            this.VirtualArmor = 50;
            this.AddItem(new Bow());
            this.PackItem(new Arrow(Utility.RandomMinMax(80, 90))); // OSI it is different: in a sub backpack, this is probably just a limitation of their engine
        }

        public Centaur(Serial serial)
            : base(serial)
        {
        }

        public override TribeType Tribe { get { return TribeType.Fey; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
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
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Gems);
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

            if (this.BaseSoundID == 678)
                this.BaseSoundID = 679;
        }
    }
}
