using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a poison elementals corpse")]
    public class GreaterPoisonElemental : BaseCreature
    {
        [Constructable]
        public GreaterPoisonElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Greater Poison Elemental";
            this.Body = 162;
            this.BaseSoundID = 263;

            this.SetStr(700, 771);
            this.SetDex(195, 203);
            this.SetInt(650, 691);

            this.SetHits(650, 702);
            this.SetStam(300, 322);
            this.SetMana(500, 530);

            this.SetDamage(12, 18);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Poison, 90);

            this.SetResistance(ResistanceType.Physical, 60, 61);
            this.SetResistance(ResistanceType.Fire, 20, 24);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 49);

            this.SetSkill(SkillName.EvalInt, 80.1, 88.3);
            this.SetSkill(SkillName.Magery, 80.1, 97.0);
            this.SetSkill(SkillName.Meditation, 80.2, 105.8);
            this.SetSkill(SkillName.Poisoning, 100.1, 114.9);
            this.SetSkill(SkillName.MagicResist, 85.2, 93.2);
            this.SetSkill(SkillName.Tactics, 80.1, 87.3);
            this.SetSkill(SkillName.Wrestling, 80.1, 88.3);

            this.Fame = 12500;
            this.Karma = -12500;

            this.VirtualArmor = 70;

            this.PackItem(new Nightshade(4));
            this.PackItem(new LesserPoisonPotion());
        }

        public GreaterPoisonElemental(Serial serial)
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
                return 5;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.03)            
                c.DropItem(new LuckyCoin());           
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