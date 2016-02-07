using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lava serpent corpse")]
    [TypeAlias("Server.Mobiles.Lavaserpant")]
    public class LavaSerpent : BaseCreature
    {
        [Constructable]
        public LavaSerpent()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a lava serpent";
            this.Body = 90;
            this.BaseSoundID = 219;

            this.SetStr(386, 415);
            this.SetDex(56, 80);
            this.SetInt(66, 85);

            this.SetHits(232, 249);
            this.SetMana(0);

            this.SetDamage(10, 22);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 80);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 25.3, 70.0);
            this.SetSkill(SkillName.Tactics, 65.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 40;

            this.PackItem(new SulfurousAsh(3));
            this.PackItem(new Bone());
            // TODO: body parts, armour
        }

        public LavaSerpent(Serial serial)
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
            this.AddLoot(LootPack.Average);
        }
        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (0.25 > Utility.RandomDouble() && reg.Name == "Fire Temple Ruins")
            {
                switch (Utility.Random(2))
                {
                    case 0: c.DropItem(new EssenceOrder()); break;
                    case 1: c.DropItem(new LavaSerpentCrust()); break;
                }
                if (0.25 > Utility.RandomDouble() && reg.Name == "Lava Caldera")
                {
                    switch (Utility.Random(2))
                    {
                        case 0: c.DropItem(new EssenceOrder()); break;
                        case 1: c.DropItem(new LavaSerpentCrust()); break;
                    }
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

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 219;
        }
    }
}