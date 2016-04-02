using System;

namespace Server.Mobiles
{
    [CorpseName("a phoenix corpse")]
    public class Phoenix : BaseCreature
    {
        [Constructable]
        public Phoenix()
            : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a phoenix";
            this.Body = 5;
            this.Hue = 0x674;
            this.BaseSoundID = 0x8F;

            this.SetStr(504, 700);
            this.SetDex(202, 300);
            this.SetInt(504, 700);

            this.SetHits(340, 383);

            this.SetDamage(25);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 50);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 90.2, 100.0);
            this.SetSkill(SkillName.Magery, 90.2, 100.0);
            this.SetSkill(SkillName.Meditation, 75.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 86.0, 135.0);
            this.SetSkill(SkillName.Tactics, 80.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 15000;
            this.Karma = 0;

            this.VirtualArmor = 60;
        }

        public Phoenix(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override MeatType MeatType
        {
            get
            {
                return MeatType.Bird;
            }
        }
        public override int Feathers
        {
            get
            {
                return 36;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
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