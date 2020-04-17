namespace Server.Mobiles
{
    public class CorruptedSoul : BaseCreature
    {
        [Constructable]
        public CorruptedSoul()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .1, 5)
        {
            Name = "a corrupted soul";
            Body = 0x3CA;
            Hue = 0x453;

            SetStr(102, 115);
            SetDex(101, 115);
            SetInt(203, 215);

            SetHits(61, 69);

            SetDamage(4, 40);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 61, 74);
            SetResistance(ResistanceType.Fire, 22, 48);
            SetResistance(ResistanceType.Cold, 73, 100);
            SetResistance(ResistanceType.Poison, 0);
            SetResistance(ResistanceType.Energy, 51, 60);

            SetSkill(SkillName.MagicResist, 80.2, 89.4);
            SetSkill(SkillName.Tactics, 81.3, 89.9);
            SetSkill(SkillName.Wrestling, 80.1, 88.7);

            Fame = 5000;
            Karma = -5000;
        }

        public CorruptedSoul(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath => true;
        public override bool AlwaysAttackable => true;
        public override bool BleedImmune => true;// NEED TO VERIFY

        // NEED TO VERIFY SOUNDS! Known: No Idle Sound.

        /*public override int GetDeathSound()
        {
        return 0x0;
        }*/
        public override bool AlwaysMurderer => true;
        public override int GetAttackSound()
        {
            return 0x233;
        }

        // TODO: Proper OnDeath Effect
        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
                return false;

            // 1 in 20 chance that a Thread of Fate will appear in the killer's pack

            Effects.SendLocationEffect(Location, Map, 0x376A, 10, 1);
            return true;
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