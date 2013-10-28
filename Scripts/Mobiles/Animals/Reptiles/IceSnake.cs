using System;

namespace Server.Mobiles
{
    [CorpseName("an ice snake corpse")]
    [TypeAlias("Server.Mobiles.Icesnake")]
    public class IceSnake : BaseCreature
    {
        [Constructable]
        public IceSnake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an ice snake";
            this.Body = 52;
            this.Hue = 0x480;
            this.BaseSoundID = 0xDB;

            this.SetStr(42, 54);
            this.SetDex(36, 45);
            this.SetInt(26, 30);

            this.SetMana(0);

            this.SetDamage(4, 12);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Cold, 25);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Cold, 80, 90);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 39.3, 54.0);
            this.SetSkill(SkillName.Wrestling, 39.3, 54.0);

            this.Fame = 900;
            this.Karma = -900;

            this.VirtualArmor = 30;
        }

        public IceSnake(Serial serial)
            : base(serial)
        {
        }

        public override bool DeathAdderCharmable
        {
            get
            {
                return true;
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
            this.AddLoot(LootPack.Meager);
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