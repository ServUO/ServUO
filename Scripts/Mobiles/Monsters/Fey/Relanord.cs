using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a relanord corpse")]
    public class Relanord : BaseCreature
    {
        [Constructable]
        public Relanord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Relanord";
            this.Body = 0x2F4;
            this.Hue = 2071;

            this.SetStr(561, 650);
            this.SetDex(76, 95);
            this.SetInt(61, 90);

            this.SetHits(331, 390);

            this.SetDamage(13, 19);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 40, 60);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.MagicResist, 80.2, 98.0);
            this.SetSkill(SkillName.Tactics, 80.2, 98.0);
            this.SetSkill(SkillName.Wrestling, 80.2, 98.0);

            this.Fame = 10000;
            this.Karma = -10000;
            this.VirtualArmor = 50;

            this.PackItem(new DaemonBone(5));
        }

        public Relanord(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool AutoDispel
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
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 1);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new VoidOrb());
        }

        public override int GetIdleSound()
        {
            return 0xFD;
        }

        public override int GetAngerSound()
        {
            return 0x26C;
        }

        public override int GetDeathSound()
        {
            return 0x211;
        }

        public override int GetAttackSound()
        {
            return 0x23B;
        }

        public override int GetHurtSound()
        {
            return 0x140;
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