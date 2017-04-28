using System;

namespace Server.Mobiles
{
    [CorpseName("a fire daemon corpse")]
    public class FireDaemon : BaseCreature
    {
        [Constructable]
        public FireDaemon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a fire daemon";
            this.Body = 9;
            this.BaseSoundID = 0x47D;
            this.Hue = 1636;

            this.SetStr(504, 539);
            this.SetDex(126, 145);
            this.SetInt(329, 364);

            this.SetHits(1026, 1174);

            this.SetDamage(7, 14);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 80);

            this.SetResistance(ResistanceType.Physical, 45, 60);
            this.SetResistance(ResistanceType.Fire, 100);
            this.SetResistance(ResistanceType.Cold, -10, 0);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Anatomy, 75.5, 84.9);
            this.SetSkill(SkillName.MagicResist, 95.7, 109.8);
            this.SetSkill(SkillName.Tactics, 81.0, 98.6);
            this.SetSkill(SkillName.Wrestling, 40.2, 78.7);
            this.SetSkill(SkillName.EvalInt, 91.1, 104.5);
            this.SetSkill(SkillName.Magery, 91.3, 105.0);
            this.SetSkill(SkillName.Meditation, 90.1, 103.7);
            this.SetSkill(SkillName.DetectHidden, 66.0);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 58;
        }        

        public FireDaemon(Serial serial)
            : base(serial)
        {
        }

        public override bool HasAura { get { return true; } }
        public override int AuraRange { get { return 5; } }
        public override bool HasBreath { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return 1; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
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
