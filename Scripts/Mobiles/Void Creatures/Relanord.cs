using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a relanord corpse")]
    public class Relanord : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.Survival; } }
        public override int Stage { get { return 2; } }

        [Constructable]
        public Relanord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a relanord";
            this.Body = 0x2F4;
            this.Hue = 2071;

            this.SetStr(700, 800);
            this.SetDex(60, 100);
            this.SetInt(60, 100);

            this.SetHits(400, 500);

            this.SetDamage(10, 15);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 45, 60);
            this.SetResistance(ResistanceType.Fire, 40, 60);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 30, 60);

            this.SetSkill(SkillName.MagicResist, 30.2, 50.0);
            this.SetSkill(SkillName.Tactics, 40.2, 60.0);
            this.SetSkill(SkillName.Wrestling, 50.2, 70.0);

            this.Fame = 10000;
            this.Karma = -10000;
            this.VirtualArmor = 50;

            this.PackItem(new DaemonBone(15));
        }

        public Relanord(Serial serial)
            : base(serial)
        {
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