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
            Name = "a lava serpent";
            Body = 90;
            BaseSoundID = 219;

            SetStr(386, 415);
            SetDex(56, 80);
            SetInt(66, 85);

            SetHits(232, 249);
            SetMana(0);

            SetDamage(10, 22);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 80);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 25.3, 70.0);
            SetSkill(SkillName.Tactics, 65.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 40;

            PackItem(new SulfurousAsh(3));
            PackItem(new Bone());
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
            AddLoot(LootPack.Average);
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

            if (BaseSoundID == -1)
                BaseSoundID = 219;
        }
    }
}