using System;

namespace Server.Mobiles
{
    [CorpseName("an acid elemental corpse")]
    public class ToxicElemental : BaseCreature
    {
        [Constructable]
        public ToxicElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an acid elemental";
            this.Body = 0x9E;
            this.BaseSoundID = 278;

            this.SetStr(326, 355);
            this.SetDex(66, 85);
            this.SetInt(271, 295);

            this.SetHits(196, 213);

            this.SetDamage(9, 15);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Fire, 50);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Anatomy, 30.3, 60.0);
            this.SetSkill(SkillName.EvalInt, 70.1, 85.0);
            this.SetSkill(SkillName.Magery, 70.1, 85.0);
            this.SetSkill(SkillName.MagicResist, 60.1, 75.0);
            this.SetSkill(SkillName.Tactics, 80.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 70.1, 90.0);

            this.Fame = 10000;
            this.Karma = -10000;

            this.VirtualArmor = 40;
        }

        public ToxicElemental(Serial serial)
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
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override double HitPoisonChance
        {
            get
            {
                return 0.6;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return Core.AOS ? 2 : 3;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
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

            if (this.BaseSoundID == 263)
                this.BaseSoundID = 278;

            if (this.Body == 13)
                this.Body = 0x9E;

            if (this.Hue == 0x4001)
                this.Hue = 0;
        }
    }
}