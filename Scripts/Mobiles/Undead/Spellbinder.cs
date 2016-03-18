using System;

namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class Spellbinder : BaseCreature
    {
        [Constructable]
        public Spellbinder()
            : base(AIType.AI_Spellbinder, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Spectral Spellbinder";
            this.Body = 153;
            this.BaseSoundID = 0x482;

            this.SetStr(76, 100);
            this.SetDex(76, 95);
            this.SetInt(36, 60);

            this.SetHits(46, 60);
            this.SetMana(100);

            this.SetDamage(0, 1);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 45.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 55.0);
            this.SetSkill(SkillName.Magery, 70.0, 80.0);
            this.SetSkill(SkillName.Meditation, 100.0, 120.0);
            this.SetSkill(SkillName.Necromancy, 100.0, 120.0);

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