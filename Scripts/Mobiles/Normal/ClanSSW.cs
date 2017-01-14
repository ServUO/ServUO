using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a clan scratch savage wolf corpse")]
    public class ClanSSW : BaseCreature
    {
        [Constructable]
        public ClanSSW()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Clan Scratch Savage Wolf";
            this.Body = 98;
            this.Hue = 0x2C;
            this.BaseSoundID = 229;

            this.SetStr(170);
            this.SetDex(244);
            this.SetInt(57);

            this.SetHits(65);

            this.SetDamage(8, 10);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 80);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Cold, 40, 45);
            this.SetResistance(ResistanceType.Poison, 25, 30);
            this.SetResistance(ResistanceType.Energy, 20, 25);
			
            this.SetSkill(SkillName.Swords, 99.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 41.5, 42.5);
            this.SetSkill(SkillName.Tactics, 65.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 42.3, 45.5);

            this.Fame = 3400;
            this.Karma = -3400;

            this.VirtualArmor = 50;
        }

        public ClanSSW(Serial serial)
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
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Canine;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ParalyzingBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Meager);
        }

        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (0.25 > Utility.RandomDouble() && reg.Name == "Cavern of the Discarded")
            {
                c.DropItem(new ReflectiveWolfEye());
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