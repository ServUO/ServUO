using System;

namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class Spellbinder : BaseCreature
    {
        [Constructable]
        public Spellbinder()
            : base(AIType.AI_Spellbinder, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Spectral Spellbinder";
            this.Body = 153;
            this.BaseSoundID = 0x482;

            this.SetStr(46, 70);
            this.SetDex(47, 65);
            this.SetInt(187, 210);

            this.SetHits(36, 50);

            this.SetDamage(3, 6);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 30);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 10, 20);

            this.SetSkill(SkillName.MagicResist, 35.1, 45.0);
            this.SetSkill(SkillName.Tactics, 35.1, 50.0);
            this.SetSkill(SkillName.Wrestling, 35.1, 50.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 28;
        }

        public Spellbinder(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
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
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
            this.PackItem(Loot.RandomWeapon());
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