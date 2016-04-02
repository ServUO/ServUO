using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ice serpent corpse")]
    [TypeAlias("Server.Mobiles.Iceserpant")]
    public class IceSerpent : BaseCreature
    {
        [Constructable]
        public IceSerpent()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a giant ice serpent";
            this.Body = 89;
            this.BaseSoundID = 219;

            this.SetStr(216, 245);
            this.SetDex(26, 50);
            this.SetInt(66, 85);

            this.SetHits(130, 147);
            this.SetMana(0);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Cold, 90);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Cold, 80, 90);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Anatomy, 27.5, 50.0);
            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 75.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 32;

        }

        public IceSerpent(Serial serial)
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
                return 4;
            }
        }
        public override int Hides
        {
            get
            {
                return 15;
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
            this.AddLoot(LootPack.Meager);
            this.PackItem(Loot.RandomArmorOrShieldOrWeapon());

            switch (Utility.Random(10))
            {
                case 0:
                    this.PackItem(new LeftArm());
                    break;
                case 1:
                    this.PackItem(new RightArm());
                    break;
                case 2:
                    this.PackItem(new Torso());
                    break;
                case 3:
                    this.PackItem(new Bone());
                    break;
                case 4:
                    this.PackItem(new RibCage());
                    break;
                case 5:
                    this.PackItem(new RibCage());
                    break;
                case 6:
                    this.PackItem(new BonePile());
                    break;
                case 7:
                    this.PackItem(new BonePile());
                    break;
                case 8:
                    this.PackItem(new BonePile());
                    break;
                case 9:
                    this.PackItem(new BonePile());
                    break;
            }

            if (0.025 > Utility.RandomDouble())
                this.PackItem(new GlacialStaff());
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

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 219;
        }
    }
}