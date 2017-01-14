using System;
using Server.Items;

namespace Server.Mobiles
{
    public interface IAcidCreature
    {
    }

    [CorpseName("an acid elementals corpse")]
    public class AcidElemental : BaseCreature, IAcidCreature
    {
        [Constructable]
        public AcidElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an acid elemental";
            this.Body = 158;
            this.BaseSoundID = 263;

            this.SetStr(326, 355);
            this.SetDex(66, 85);
            this.SetInt(271, 295);

            this.SetHits(196, 213);

            this.SetDamage(9, 15);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Poison, 90);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 30.3, 60.0);
            this.SetSkill(SkillName.EvalInt, 80.1, 95.0);
            this.SetSkill(SkillName.Magery, 70.1, 85.0);
            this.SetSkill(SkillName.Meditation, 0.0, 0.0);
            this.SetSkill(SkillName.MagicResist, 60.1, 85.0);
            this.SetSkill(SkillName.Tactics, 80.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 70.1, 90.0);

            this.Fame = 10000;
            this.Karma = -10000;

            this.VirtualArmor = 70;

            this.PackItem(new Nightshade(4));
            //			PackItem( new LesserPoisonPotion() );
        }

        public AcidElemental(Serial serial)
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
                return Poison.Lethal;
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
                return 0.75;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 2;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    Body = 158;
                    break;
            }
        }
    }
}