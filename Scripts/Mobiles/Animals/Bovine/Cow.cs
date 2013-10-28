using System;

namespace Server.Mobiles
{
    [CorpseName("a cow corpse")]
    public class Cow : BaseCreature
    {
        private DateTime m_MilkedOn;
        private int m_Milk;
        [Constructable]
        public Cow()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a cow";
            this.Body = Utility.RandomList(0xD8, 0xE7);
            this.BaseSoundID = 0x78;

            this.SetStr(30);
            this.SetDex(15);
            this.SetInt(5);

            this.SetHits(18);
            this.SetMana(0);

            this.SetDamage(1, 4);

            this.SetDamage(1, 4);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 15);

            this.SetSkill(SkillName.MagicResist, 5.5);
            this.SetSkill(SkillName.Tactics, 5.5);
            this.SetSkill(SkillName.Wrestling, 5.5);

            this.Fame = 300;
            this.Karma = 0;

            this.VirtualArmor = 10;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 11.1;

            if (Core.AOS && Utility.Random(1000) == 0) // 0.1% chance to have mad cows
                this.FightMode = FightMode.Closest;
        }

        public Cow(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime MilkedOn
        {
            get
            {
                return this.m_MilkedOn;
            }
            set
            {
                this.m_MilkedOn = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Milk
        {
            get
            {
                return this.m_Milk;
            }
            set
            {
                this.m_Milk = value;
            }
        }
        public override int Meat
        {
            get
            {
                return 8;
            }
        }
        public override int Hides
        {
            get
            {
                return 12;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            int random = Utility.Random(100);

            if (random < 5)
                this.Tip();
            else if (random < 20)
                this.PlaySound(120);
            else if (random < 40)
                this.PlaySound(121);
        }

        public void Tip()
        {
            this.PlaySound(121);
            this.Animate(8, 0, 3, true, false, 0);
        }

        public bool TryMilk(Mobile from)
        {
            if (!from.InLOS(this) || !from.InRange(this.Location, 2))
                from.SendLocalizedMessage(1080400); // You can not milk the cow from this location.
            if (this.Controlled && this.ControlMaster != from)
                from.SendLocalizedMessage(1071182); // The cow nimbly escapes your attempts to milk it.
            if (this.m_Milk == 0 && this.m_MilkedOn + TimeSpan.FromDays(1) > DateTime.UtcNow)
                from.SendLocalizedMessage(1080198); // This cow can not be milked now. Please wait for some time.
            else
            {
                if (this.m_Milk == 0)
                    this.m_Milk = 4;

                this.m_MilkedOn = DateTime.UtcNow;
                this.m_Milk--;

                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            writer.Write((DateTime)this.m_MilkedOn);
            writer.Write((int)this.m_Milk);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version > 0)
            {
                this.m_MilkedOn = reader.ReadDateTime();
                this.m_Milk = reader.ReadInt();
            }
        }
    }
}