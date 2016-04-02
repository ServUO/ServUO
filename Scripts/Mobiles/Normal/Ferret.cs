using System;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    [CorpseName("a ferret corpse")]
    public class Ferret : BaseCreature
    {
        private static readonly string[] m_Vocabulary = new string[]
        {
            "dook",
            "dook dook",
            "dook dook dook!"
        };
        private bool m_CanTalk;
        [Constructable]
        public Ferret()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a ferret";
            this.Body = 0x117;

            this.SetStr(41, 48);
            this.SetDex(55);
            this.SetInt(75);

            this.SetHits(45, 50);

            this.SetDamage(7, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45, 50);
            this.SetResistance(ResistanceType.Fire, 10, 14);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 21, 25);
            this.SetResistance(ResistanceType.Energy, 20, 25);

            this.SetSkill(SkillName.MagicResist, 4.0);
            this.SetSkill(SkillName.Tactics, 4.0);
            this.SetSkill(SkillName.Wrestling, 4.0);

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -21.3;

            this.m_CanTalk = true;
        }

        public Ferret(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation) 
        {
            if (m is Ferret && m.InRange(this, 3) && m.Alive)
                this.Talk((Ferret)m);
        }

        public void Talk()
        {
            this.Talk(null);
        }

        public void Talk(Ferret to)
        {
            if (this.m_CanTalk)
            {
                if (to != null)
                    QuestSystem.FocusTo(this, to);

                this.Say(m_Vocabulary[Utility.Random(m_Vocabulary.Length)]);
			
                if (to != null && Utility.RandomBool())
                    Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(5, 8)), new TimerCallback(delegate() { to.Talk(); }));

                this.m_CanTalk = false;

                Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30)), new TimerCallback(delegate() { this.m_CanTalk = true; }));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_CanTalk = true;
        }
    }
}