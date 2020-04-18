using Server.Items;

namespace Server.Mobiles
{
    public class MorgBergen : BaseCreature
    {
        [Constructable]
        public MorgBergen()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Title = "the Cursed";

            Hue = 0x8596;
            Body = 0x190;
            Name = "Morg Bergen";

            AddItem(new ShortPants(0x59C));

            Bardiche bardiche = new Bardiche();
            LeatherGloves gloves = new LeatherGloves();
            LeatherArms arms = new LeatherArms();

            bardiche.Hue = 0x96F;
            bardiche.Movable = false;
            gloves.Hue = 0x96F;
            arms.Hue = 0x96F;

            AddItem(bardiche);
            AddItem(gloves);
            AddItem(arms);

            SetStr(111, 120);
            SetDex(111, 120);
            SetInt(51, 60);

            SetHits(180, 207);
            SetMana(0);

            SetDamage(9, 17);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 60);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.Swords, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 80.1, 90.0);
            SetSkill(SkillName.Anatomy, 90.1, 100.0);

            Fame = 5000;
            Karma = -1000;
        }

        public MorgBergen(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle => false;
        public override bool ShowFameTitle => false;
        public override bool DeleteCorpseOnDeath => true;
        public override bool AlwaysMurderer => true;
        public override int GetIdleSound()
        {
            return 0x1CE;
        }

        public override int GetAngerSound()
        {
            return 0x263;
        }

        public override int GetDeathSound()
        {
            return 0x1D1;
        }

        public override int GetHurtSound()
        {
            return 0x25E;
        }

        public override bool OnBeforeDeath()
        {
            Gold gold = new Gold(Utility.RandomMinMax(190, 230));
            gold.MoveToWorld(Location, Map);

            Effects.SendLocationEffect(Location, Map, 0x376A, 10, 1);
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}