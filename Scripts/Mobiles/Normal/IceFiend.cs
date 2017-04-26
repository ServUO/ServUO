using System;

namespace Server.Mobiles
{
    [CorpseName("an ice fiend corpse")]
    public class IceFiend : BaseCreature
    {
        [Constructable]
        public IceFiend()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an ice fiend";
            this.Body = 43;
            this.BaseSoundID = 357;

            this.SetStr(376, 405);
            this.SetDex(176, 195);
            this.SetInt(201, 225);

            this.SetHits(226, 243);

            this.SetDamage(8, 19);

            this.SetSkill(SkillName.EvalInt, 80.1, 90.0);
            this.SetSkill(SkillName.Magery, 80.1, 90.0);
            this.SetSkill(SkillName.MagicResist, 75.1, 85.0);
            this.SetSkill(SkillName.Tactics, 80.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 100.0);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.Fame = 18000;
            this.Karma = -18000;

            this.VirtualArmor = 60;
        }

        public IceFiend(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
            }
        }

        public override bool HasAura { get { return true; } }
        public override int AuraBaseDamage { get { return 15; } }
        public override int AuraRange { get { return 2; } }
        public override int AuraFireDamage { get { return 0; } }
        public override int AuraColdDamage { get { return 100; } }

        public override void AuraEffect(Mobile m)
        {
            m.SendLocalizedMessage(1008111, false, this.Name); //  : The intense cold is damaging you!
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.MedScrolls, 2);
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