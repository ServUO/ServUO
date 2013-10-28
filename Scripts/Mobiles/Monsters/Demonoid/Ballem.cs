using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Ballem corpse")]
    public class Ballem : BaseCreature
    {
        [Constructable]
        public Ballem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Ballem";
            this.Body = 304;
            this.Hue = 2071;
            this.BaseSoundID = 684;

            this.SetStr(376, 300);
            this.SetDex(151, 175);
            this.SetInt(46, 70);

            this.SetHits(1106, 1120);

            this.SetDamage(18, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 50.1, 75.0);
            this.SetSkill(SkillName.Tactics, 55.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 70.0);

            this.Fame = 1000;
            this.Karma = -1800;

            this.VirtualArmor = 54;

            this.PackItem(new DaemonBone(15));
        }

        public Ballem(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
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
                return true;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.CrushingBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new VoidOrb());

            if (Utility.RandomDouble() < 0.10)
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