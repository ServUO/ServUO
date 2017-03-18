using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Ballem corpse")]
    public class Ballem : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.Killing; } }
        public override int Stage { get { return 2; } }

        [Constructable]
        public Ballem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Ballem";
            this.Body = 304;
            this.Hue = 2071;
            this.BaseSoundID = 684;

            this.SetStr(991);
            this.SetDex(1001);
            this.SetInt(243);

            this.SetHits(500, 600);

            this.SetDamage(10, 15);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 30, 50);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 70.0, 80.0);
            this.SetSkill(SkillName.Tactics, 50.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 70.1, 80.0);
            this.SetSkill(SkillName.Anatomy, 0.0, 10.0);

            this.Fame = 1800;
            this.Karma = -1800;

            this.VirtualArmor = 54;

            this.PackItem(new DaemonBone(15));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.10)
            {
                c.DropItem(new AncientPotteryFragments());
            }
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