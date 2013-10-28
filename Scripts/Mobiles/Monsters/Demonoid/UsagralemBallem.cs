using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an usagralem ballem corpse")]
    public class UsagralemBallem : BaseCreature
    {
        [Constructable]
        public UsagralemBallem()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Usagrallem Ballem";
            this.Hue = 2071;
            this.Body = 318;
            this.BaseSoundID = 0x165;

            this.SetStr(500);
            this.SetDex(100);
            this.SetInt(1000);

            this.SetHits(30000);
            this.SetMana(5000);

            this.SetDamage(17, 21);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 30);
            this.SetResistance(ResistanceType.Fire, 30);
            this.SetResistance(ResistanceType.Cold, 30);
            this.SetResistance(ResistanceType.Poison, 30);
            this.SetResistance(ResistanceType.Energy, 30);

            this.SetSkill(SkillName.DetectHidden, 80.0);
            this.SetSkill(SkillName.EvalInt, 100.0);
            this.SetSkill(SkillName.Magery, 100.0);
            this.SetSkill(SkillName.Meditation, 120.0);
            this.SetSkill(SkillName.MagicResist, 150.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 120.0);

            this.Fame = 28000;
            this.Karma = -28000;

            this.VirtualArmor = 64;
        }

        public UsagralemBallem(Serial serial)
            : base(serial)
        {
        }

        public override bool IgnoreYoungProtection
        {
            get
            {
                return Core.ML;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.SE;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return Core.SE;
            }
        }
        public override bool AreaPeaceImmune
        {
            get
            {
                return Core.SE;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            switch ( Utility.Random(3) )
            {
                default:
                case 0:
                    return WeaponAbility.DoubleStrike;
                case 1:
                    return WeaponAbility.WhirlwindAttack;
                case 2:
                    return WeaponAbility.CrushingBlow;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.SuperBoss, 2);
            this.AddLoot(LootPack.HighScrolls, Utility.RandomMinMax(6, 10));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.3)
                c.DropItem(new VoidOrb());

            if (Utility.RandomDouble() < 0.30)
            {
                switch (Utility.Random(2))
                {
                    case 0:
                        this.AddToBackpack(new VoidEssence());
                        break;
                    case 1:
                        this.AddToBackpack(new AncientPotteryFragments());
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