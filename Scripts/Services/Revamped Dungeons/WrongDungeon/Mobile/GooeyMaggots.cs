using System;

namespace Server.Mobiles
{
    [CorpseName("gooey maggots corpse")]
    public class GooeyMaggots : BaseCreature
    {
        [Constructable]
        public GooeyMaggots()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Gooey Maggots";
            this.Body = 319;
            this.BaseSoundID = 898;

            this.SetStr(738, 763);
            this.SetDex(61, 70);
            this.SetInt(10);

            this.SetMana(0);

            this.SetDamage(3, 9);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 60, 70);
            this.SetResistance(ResistanceType.Energy, 60, 70);

            this.SetSkill(SkillName.Tactics, 80.2, 89.7);
            this.SetSkill(SkillName.Wrestling, 80.2, 87.5);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 24;
        }

        public GooeyMaggots(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

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