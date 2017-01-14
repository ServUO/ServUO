using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("the remains of Grim")]
    public class Grim : Drake 
    {
        [Constructable]
        public Grim()
            : base()
        {
            this.Name = "Grim";
            this.Hue = 1744;

            this.SetStr(527, 580);
            this.SetDex(284, 322);
            this.SetInt(249, 386);

            this.SetHits(1762, 2502);

            this.SetDamage(17, 25);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Fire, 20);

            this.SetResistance(ResistanceType.Physical, 55, 60);
            this.SetResistance(ResistanceType.Fire, 62, 68);
            this.SetResistance(ResistanceType.Cold, 52, 57);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 40, 44);

            this.SetSkill(SkillName.MagicResist, 105.8, 115.6);
            this.SetSkill(SkillName.Tactics, 102.8, 120.8);
            this.SetSkill(SkillName.Wrestling, 111.7, 119.2);
            this.SetSkill(SkillName.Anatomy, 105.0, 128.4);

            this.Fame = 17500;
            this.Karma = -5500;

            this.VirtualArmor = 54;

            this.Tamable = false;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Grim(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override int Meat
        {
            get
            {
                return 10;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        // Varchild's
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.CrushingBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            this.AddLoot(LootPack.MedScrolls);
            this.AddLoot(LootPack.HighScrolls, 2);
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