using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ortanord corpse")]
    public class Ortanord : BaseCreature
    {
        [Constructable]
        public Ortanord()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            this.Name = "Ortanord";
            this.Body = 58;
            this.Hue = 2071;
            this.BaseSoundID = 466;

            this.SetStr(50, 50);
            this.SetDex(50, 50);
            this.SetInt(51, 51);

            this.SetHits(100, 100);
            this.SetMana(1001, 1001);
            this.SetStam(50, 50);

            this.SetDamage(5, 8);

            this.SetDamageType(ResistanceType.Energy, 100);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 80, 90);
            this.SetResistance(ResistanceType.Cold, 80, 90);
            this.SetResistance(ResistanceType.Poison, 80, 90);
            this.SetResistance(ResistanceType.Energy, 80, 90);

            this.SetSkill(SkillName.MagicResist, 104.4, 108.0);
            this.SetSkill(SkillName.Tactics, 19.0, 19.8);
            this.SetSkill(SkillName.Anatomy, 15.6, 16.8);
            this.SetSkill(SkillName.Wrestling, 15.4, 16.6);
            this.SetSkill(SkillName.Magery, 104.7, 107.3);
            this.SetSkill(SkillName.Meditation, 20.0, 20.0);

            this.Fame = 8000;
            this.Karma = -8000;

            this.VirtualArmor = 40;

            if (0.25 > Utility.RandomDouble())
                this.PackItem(new DaemonBone(10));
        }

        public Ortanord(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
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
            this.AddLoot(LootPack.Average, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.05)
            {
                c.DropItem(new VoidOrb()); 
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