using System;
using Server.Items;
using Server.Multis;

namespace Server.Mobiles
{
    public class PetParrot : BaseCreature
    { 
        private DateTime m_Birth;
        [Constructable]
        public PetParrot()
            : this(DateTime.MinValue, null, 0)
        {
        }

        [Constructable]
        public PetParrot(DateTime birth, string name, int hue)
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        {
            this.Name = "a pet parrot";
            this.Title = "the parrot";			
            this.Body = 0x11A;
            this.BaseSoundID = 0xBF;
			
            this.SetStr(1, 5);
            this.SetDex(25, 30);
            this.SetInt(2);
			
            this.SetHits(1, this.Str);
            this.SetStam(25, this.Dex);
            this.SetMana(0);

            this.SetResistance(ResistanceType.Physical, 2);

            this.SetSkill(SkillName.MagicResist, 4);
            this.SetSkill(SkillName.Tactics, 4);
            this.SetSkill(SkillName.Wrestling, 4);

            this.CantWalk = true;
            this.Blessed = true;
			
            if (birth != DateTime.MinValue)
                this.m_Birth = birth;
            else
                this.m_Birth = DateTime.UtcNow;
				
            if (name != null)
                this.Name = name;
				
            if (hue > 0)
                this.Hue = hue;
        }

        public PetParrot(Serial serial)
            : base(serial)
        {
        }

        public override bool NoHouseRestrictions
        {
            get
            {
                return true;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Birth
        {
            get
            {
                return this.m_Birth;
            }
            set
            {
                this.m_Birth = value;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies;
            }
        }
        public static int GetWeeks(DateTime birth)
        {
            TimeSpan span = DateTime.UtcNow - birth;
			
            return (int)(span.TotalDays / 7);		
        }

        public override void OnStatsQuery(Mobile from)
        {
            if (from.Map == this.Map && Utility.InUpdateRange(this, from) && from.CanSee(this))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null && house.IsCoOwner(from) && from.AccessLevel == AccessLevel.Player)
                    from.SendLocalizedMessage(1072625); // As the house owner, you may rename this Parrot.
					
                from.Send(new Server.Network.MobileStatus(from, this));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            int weeks = GetWeeks(this.m_Birth);
			
            if (weeks == 1)
                list.Add(1072626); // 1 week old
            else if (weeks > 1)
                list.Add(1072627, weeks.ToString()); // ~1_AGE~ weeks old
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            if ((int)from.AccessLevel > (int)AccessLevel.Player)
                return true;
		
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsCoOwner(from))
                return true;
            else
                return false;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);
			
            if (Utility.RandomDouble() < 0.05)
            {
                this.Say(e.Speech);
                this.PlaySound(0xC0);
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is ParrotWafer)
            {
                dropped.Delete();
				
                switch ( Utility.Random(6) )
                {
                    case 0:
                        this.Say(1072602, "#" + Utility.RandomMinMax(1012003, 1012010));
                        break; // I just flew in from ~1_CITYNAME~ and boy are my wings tired!
                    case 1:
                        this.Say(1072603);
                        break; // Wind in the sails!  Wind in the sails!
                    case 2:
                        this.Say(1072604);
                        break; // Arrrr, matey!
                    case 3:
                        this.Say(1072605);
                        break; // Loot and plunder!  Loot and plunder!
                    case 4:
                        this.Say(1072606);
                        break; // I want a cracker!
                    case 5:
                        this.Say(1072607);
                        break; // I'm just a house pet!
                }
				
                this.PlaySound(Utility.RandomMinMax(0xBF, 0xC3));
				
                return true;
            }
            else
                return false;			
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((DateTime)this.m_Birth);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_Birth = reader.ReadDateTime();
        }
    }
}
