namespace Server.Mobiles
{
    [CorpseName("a gargoyle corpse")]
    public class GargoylePet : BaseCreature
    {
        [Constructable]
        public GargoylePet()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gargoyle pet";
            Body = 730;

            SetStr(500, 512);
            SetDex(90, 94);
            SetInt(100, 107);

            SetHits(300, 313);

            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60);
            SetResistance(ResistanceType.Fire, 40);
            SetResistance(ResistanceType.Cold, 40);
            SetResistance(ResistanceType.Poison, 40);
            SetResistance(ResistanceType.Energy, 40);

            SetSkill(SkillName.MagicResist, 75.5, 89.0);
            SetSkill(SkillName.Tactics, 80.3, 93.8);
            SetSkill(SkillName.Wrestling, 66.9, 81.5);

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 65.1;
        }

        public GargoylePet(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 7;
        public override int Hides => 11;
        public override HideType HideType => HideType.Horned;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
        }

        public override int GetIdleSound()
        {
            return 1573;
        }

        public override int GetAngerSound()
        {
            return 1570;
        }

        public override int GetHurtSound()
        {
            return 1572;
        }

        public override int GetDeathSound()
        {
            return 1571;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}