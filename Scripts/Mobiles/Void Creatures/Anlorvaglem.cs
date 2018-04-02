using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anlorvaglem corpse")]
    public class Anlorvaglem : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.Grouping; } }
        public override int Stage { get { return 3; } }

        [Constructable]
        public Anlorvaglem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.6, 1.2)
        {
            this.Name = "anlorvaglem";
            this.Hue = 2071;
            this.Body = 152;

            this.SetStr(1000, 1200);
            this.SetDex(1000, 1200);
            this.SetInt(100, 1200);

            this.SetHits(3205);

            this.SetDamage(11, 13);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 20, 50);
            this.SetResistance(ResistanceType.Fire, 20, 60);
            this.SetResistance(ResistanceType.Cold, 20, 58);
            this.SetResistance(ResistanceType.Poison, 80, 100);
            this.SetResistance(ResistanceType.Energy, 30, 50);

            this.SetSkill(SkillName.Wrestling, 75.8, 100.0);
            this.SetSkill(SkillName.Tactics, 50.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 50.9, 90.0);

            this.Fame = 8000;
            this.Karma = -8000;

            this.VirtualArmor = 48;

            this.PackItem(new DaemonBone(30));
        }

        public Anlorvaglem(Serial serial)
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
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}
