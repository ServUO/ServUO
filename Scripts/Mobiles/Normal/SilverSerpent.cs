using System;
using Server.Factions;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a silver serpent corpse")]
    [TypeAlias("Server.Mobiles.Silverserpant")]
    public class SilverSerpent : BaseCreature
    {
        [Constructable]
        public SilverSerpent()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Body = 92;
            this.Name = "a silver serpent";
            this.BaseSoundID = 219;

            this.SetStr(161, 360);
            this.SetDex(151, 300);
            this.SetInt(21, 40);

            this.SetHits(97, 216);

            this.SetDamage(5, 21);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 5, 10);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.Poisoning, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 95.1, 100.0);
            this.SetSkill(SkillName.Tactics, 80.1, 95.0);
            this.SetSkill(SkillName.Wrestling, 85.1, 100.0);

            this.Fame = 7000;
            this.Karma = -7000;

            this.VirtualArmor = 40;
        }

        public SilverSerpent(Serial serial)
            : base(serial)
        {
        }

        public override Faction FactionAllegiance
        {
            get
            {
                return TrueBritannians.Instance;
            }
        }
        public override Ethics.Ethic EthicAllegiance
        {
            get
            {
                return Ethics.Ethic.Hero;
            }
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Gems, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            Region reg = Region.Find(c.GetWorldLocation(), c.Map);

            if (reg.Name == "Tomb of Kings")
            {
                if (Utility.RandomDouble() < 0.05)
                    c.DropItem(new SilverSnakeSkin());

                if (Utility.RandomDouble() < 0.1)
                    c.DropItem(new SilverSerpentVenom());
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

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 219;
        }
    }
}