using Server.Items;

namespace Server.Mobiles
{
    public class SpectralArmour : BaseCreature
    {
        [Constructable]
        public SpectralArmour()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 637;
            Hue = 0x8026;
            Name = "spectral armour";

            Buckler buckler = new Buckler();
            ChainCoif coif = new ChainCoif();
            PlateGloves gloves = new PlateGloves();

            buckler.Hue = 0x835;
            buckler.Movable = false;
            coif.Hue = 0x835;
            gloves.Hue = 0x835;

            AddItem(buckler);
            AddItem(coif);
            AddItem(gloves);

            SetStr(101, 110);
            SetDex(101, 110);
            SetInt(101, 110);

            SetHits(178, 201);
            SetStam(191, 200);

            SetDamage(10, 22);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Cold, 25);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.Wrestling, 75.1, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 90.1, 100);

            Fame = 7000;
            Karma = -7000;
        }

        public SpectralArmour(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath => true;
        public override Poison PoisonImmune => Poison.Regular;
        public override int GetIdleSound()
        {
            return 0x200;
        }

        public override int GetAngerSound()
        {
            return 0x56;
        }

        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
                return false;

            Gold gold = new Gold(Utility.RandomMinMax(240, 375));
            gold.MoveToWorld(Location, Map);

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