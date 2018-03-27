using Server;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class SphynxFortuneArray
    {
        public Mobile Mobile { get; set; }
        public DateTime Date { get; set; }
        public AosAttribute attr1 { get; set; }
        public ResistanceType attr2 { get; set; }
        public int Value { get; set; }
    }

    public class SphynxFortune : Item
	{
        private static List<SphynxFortuneArray> Fountains = new List<SphynxFortuneArray>();
        private static Timer m_Timer;

        [Constructable]
		public SphynxFortune()
            :base(3796)
		{
            Name = "Sphynx Fortune Stone (do not remove)";
            Movable = false;
            Visible = false;

            StartTimer();
        }

        public static void ApplyFortune(Mobile from, Mobile m)
        {
            int cliloc = 0;

            switch (Utility.Random(20))
            {
               case 0:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Physical, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060886; // Your endurance shall protect you from your enemies blows.
                        break;
                    }
                case 1:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Physical, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060901; // Your wounds in battle shall run deep.
                        break;
                    }
                case 2:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Fire, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060887; // A smile will be upon your lips, as you gaze into the infernos.
                        break;
                    }
                case 3:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Fire, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060902; // The fires of the abyss shall tear asunder your flesh!
                        break;
                    }
                case 4:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Cold, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060888; // The ice of ages will embrace you, and you will embrace it alike.
                        break;
                    }
                case 5:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Cold, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060903; // Winter’s touch shall be your undoing.
                        break;
                    }
                case 6:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Poison, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060889; // Your blood runs pure and strong.
                        break;
                    }
                case 7:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Poison, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060904; // Your veins will freeze with poison’s chill.
                        break;
                    }
                case 8:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Energy, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060890; // Your flesh shall endure the power of storms.
                        break;
                    }
                case 9:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.None, attr2 = ResistanceType.Energy, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060905; // The wise will seek to avoid the anger of storms.
                        break;
                    }
                case 10:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.Luck, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(10, 50) });
                        cliloc = 1060891; // Seek riches and they will seek you.
                        break;
                    }
                case 11:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.Luck, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(-10, -50) });
                        cliloc = 1060901; // Your wounds in battle shall run deep.
                        break;
                    }
                case 12:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.EnhancePotions, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(5, 25) });
                        cliloc = 1060892; // The power of alchemy shall thrive within you.
                        break;
                    }
                case 13:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.EnhancePotions, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(-5, -25) });
                        cliloc = 1060907; // The strength of alchemy will fail you.
                        break;
                    }
                case 14:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.Luck, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(10, 100) });
                        cliloc = 1060893; // Fate smiles upon you this day.
                        break;
                    }
                case 15:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.Luck, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(-10, -100) });
                        cliloc = 1060908; // Only fools take risks in fate’s shadow.
                        break;
                    }
                case 16:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.DefendChance, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060894; // A keen mind in battle will help you avoid injury.
                        break;
                    }
                case 17:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.DefendChance, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(-1, -10) });
                        cliloc = 1060909; // Your lack of focus in battle shall be your undoing.
                        break;
                    }
                case 18:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.RegenMana, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(1, 3) });
                        cliloc = 1060895; // The flow of the ether is strong within you.
                        break;
                    }
                case 19:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, attr1 = AosAttribute.RegenMana, attr2 = ResistanceType.None, Value = Utility.RandomMinMax(-1, -3) });
                        cliloc = 1060910; // Your connection with the ether is weak, take heed.
                        break;
                    }                 
            }

            m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, cliloc, from.NetState);
        }

        public static bool UnderEffect(Mobile from)
        {
            return Fountains.Any(x => x.Mobile == from);
        }

        public static int GetResistanceBonus(Mobile from, ResistanceType type)
        {
            if (Fountains.Any(x => x.Mobile == from && x.attr2 == type))
            {
                return Fountains.Where(x => x.Mobile == from && x.attr2 == type).Sum(y => y.Value);
            }

            return 0;
        }

        public static int GetAosAttributeBonus(Mobile from, AosAttribute type)
        {
            if (Fountains.Any(x => x.Mobile == from && x.attr1 == type))
            {
                return Fountains.Where(x => x.Mobile == from && x.attr1 == type).Sum(y => y.Value);
            }

            return 0;
        }

        public override bool HandlesOnMovement { get { return true; } }
				
		public static void DefragTables()
		{
            Fountains.ForEach(x =>
            {
                if (DateTime.UtcNow > x.Date + TimeSpan.FromHours(24))
                {
                    x.Mobile.SendLocalizedMessage(1060859); // The effects of the Sphynx have worn off.
                    Fountains.Remove(x);
                }
            });
		}

        public static void StartTimer()
        {
            if (m_Timer != null && m_Timer.Running)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(DefragTables));
            m_Timer.Start();
        }
		
		public SphynxFortune(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

            writer.Write(Fountains.Count);

            foreach (var pair in Fountains)
            {
                writer.Write(pair.Mobile);
                writer.Write(pair.Date);
                writer.Write((int)pair.attr1);
                writer.Write((int)pair.attr2);
                writer.Write(pair.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            for (int i = reader.ReadInt(); i > 0; i--)
            {
                Fountains.Add(new SphynxFortuneArray { Mobile = reader.ReadMobile(), Date = reader.ReadDateTime(), attr1 = (AosAttribute)reader.ReadInt(), attr2 = (ResistanceType)reader.ReadInt(), Value = reader.ReadInt() });
            }

            StartTimer();
        }
	}
}
