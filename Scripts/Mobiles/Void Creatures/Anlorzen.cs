using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anlorzen corpse")]
    public class Anlorzen : BaseVoidCreature
    {
        public override VoidEvolution Evolution { get { return VoidEvolution.Grouping; } }
        public override int Stage { get { return 1; } }

        [Constructable]
        public Anlorzen()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an anlorzen";
            this.Body = 11;
            this.BaseSoundID = 1170;

            this.SetStr(600, 750);
            this.SetDex(666, 800);
            this.SetInt(850, 1000);

            this.SetHits(300, 400);

            this.SetDamage(15, 18);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Cold, 20);
            this.SetDamageType(ResistanceType.Poison, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 30, 50);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 40, 60);

            this.SetSkill(SkillName.MagicResist, 30.1, 60.0);
            this.SetSkill(SkillName.Tactics, 30.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 70.0);

            this.Fame = 5000;
            this.Karma = -5000;

            this.VirtualArmor = 56;

            this.PackItem(new DaemonBone(5));
        }

        public Anlorzen(Serial serial)
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
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
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

            if (this.BaseSoundID == 263)
                this.BaseSoundID = 1170;
        }
    }
}