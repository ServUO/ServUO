using System;

namespace Server.Mobiles
{
    [CorpseName("a maggoty corpse")] // TODO: Corpse name?
    public class MoundOfMaggots : BaseCreature
    {
        [Constructable]
        public MoundOfMaggots()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a mound of maggots";
            this.Body = 319;
            this.BaseSoundID = 898;

            this.SetStr(61, 70);
            this.SetDex(61, 70);
            this.SetInt(10);

            this.SetMana(0);

            this.SetDamage(3, 9);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 90);
            this.SetResistance(ResistanceType.Poison, 100);

            this.SetSkill(SkillName.Tactics, 50.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 60.0);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 24;
        }

        public MoundOfMaggots(Serial serial)
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
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.Gems);
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