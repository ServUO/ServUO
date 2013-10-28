using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anlorzen corpse")]
    public class Anlorzen : BaseCreature
    {
        [Constructable]
        public Anlorzen()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Anlorzen";
            this.Body = 11;
            this.BaseSoundID = 1170;

            this.SetStr(196, 220);
            this.SetDex(126, 145);
            this.SetInt(286, 310);

            this.SetHits(1118, 1132);

            this.SetDamage(15, 17);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Poison, 80);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 90, 100);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.EvalInt, 65.1, 80.0);
            this.SetSkill(SkillName.Magery, 65.1, 80.0);
            this.SetSkill(SkillName.Meditation, 65.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 55.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 75.0);

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

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.10)
                c.DropItem(new VoidOrb());

            if (Utility.RandomDouble() < 0.20)
                c.DropItem(new VoidEssence());
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