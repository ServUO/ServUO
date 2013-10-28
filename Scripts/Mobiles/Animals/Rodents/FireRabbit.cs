using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hare corpse")]
    public class FireRabbit : VorpalBunny
    {
        [Constructable]
        public FireRabbit()
        {
            this.Name = "a fire rabbit";

            this.Hue = 0x550; // guessed

            this.SetStr(130);
            this.SetDex(4500);
            this.SetInt(2500);

            this.SetHits(2500);
            this.SetStam(1500);
            this.SetMana(1500);

            this.SetDamage(10, 15);

            this.SetDamageType(ResistanceType.Fire, 100);
            this.SetDamageType(ResistanceType.Physical, 0);

            this.SetResistance(ResistanceType.Physical, 45);
            this.SetResistance(ResistanceType.Fire, 100);
            this.SetResistance(ResistanceType.Cold, 40);
            this.SetResistance(ResistanceType.Poison, 46);
            this.SetResistance(ResistanceType.Energy, 46);

            this.SetSkill(SkillName.MagicResist, 200);
            this.SetSkill(SkillName.Tactics, 0.0);
            this.SetSkill(SkillName.Wrestling, 80.0);
            this.SetSkill(SkillName.Anatomy, 0.0);
        }

        public FireRabbit(Serial serial)
            : base(serial)
        {
        }

        public override bool IsScaryToPets
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
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new AnimalPheromone());
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich, 3);
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