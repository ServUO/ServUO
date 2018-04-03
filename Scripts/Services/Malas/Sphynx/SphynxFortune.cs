using Server;
using Server.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Engines.SphynxFortune
{
    public class SphynxFortuneArray
    {
        public Mobile Mobile { get; set; }
        public DateTime Date { get; set; }
        public EnumType Type { get; set; }
        public EnumTypeValue TypeValue { get; set; }   
        public int Value { get; set; }
    }

    public enum EnumTypeValue
    {
        Physical,
        Fire,
        Cold,
        Poison,
        Energy,
        Luck,
        EnhancePotions,
        DefendChance,
        RegenMana
    }

    public enum EnumType
    {
        ResistanceType,
        AosAttribute
    }

    public class SphynxFortune
	{
        public static string FilePath = Path.Combine("Saves/Misc/SphynxFortune", "Persistence.bin");
        private static List<SphynxFortuneArray> Fountains = new List<SphynxFortuneArray>();
        private static Timer m_Timer;

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void ApplyFortune(Mobile from, Mobile m)
        {
            int cliloc = 0;

            switch (Utility.Random(20))
            {
               case 0:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Physical, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060886; // Your endurance shall protect you from your enemies blows.
                        break;
                    }
                case 1:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Physical, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060901; // Your wounds in battle shall run deep.
                        break;
                    }
                case 2:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Fire, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060887; // A smile will be upon your lips, as you gaze into the infernos.
                        break;
                    }
                case 3:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Fire, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060902; // The fires of the abyss shall tear asunder your flesh!
                        break;
                    }
                case 4:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Cold, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060888; // The ice of ages will embrace you, and you will embrace it alike.
                        break;
                    }
                case 5:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Cold, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060903; // Winter’s touch shall be your undoing.
                        break;
                    }
                case 6:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Poison, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060889; // Your blood runs pure and strong.
                        break;
                    }
                case 7:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Poison, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060904; // Your veins will freeze with poison’s chill.
                        break;
                    }
                case 8:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Energy, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060890; // Your flesh shall endure the power of storms.
                        break;
                    }
                case 9:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.ResistanceType, TypeValue = EnumTypeValue.Energy, Value = Utility.RandomMinMax(-1, -15) });
                        cliloc = 1060905; // The wise will seek to avoid the anger of storms.
                        break;
                    }
                case 10:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.Luck, Value = Utility.RandomMinMax(10, 50) });
                        cliloc = 1060891; // Seek riches and they will seek you.
                        break;
                    }
                case 11:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.Luck, Value = Utility.RandomMinMax(-10, -50) });
                        cliloc = 1060901; // Your wounds in battle shall run deep.
                        break;
                    }
                case 12:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.EnhancePotions, Value = Utility.RandomMinMax(5, 25) });
                        cliloc = 1060892; // The power of alchemy shall thrive within you.
                        break;
                    }
                case 13:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.EnhancePotions, Value = Utility.RandomMinMax(-5, -25) });
                        cliloc = 1060907; // The strength of alchemy will fail you.
                        break;
                    }
                case 14:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.Luck, Value = Utility.RandomMinMax(10, 100) });
                        cliloc = 1060893; // Fate smiles upon you this day.
                        break;
                    }
                case 15:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.Luck, Value = Utility.RandomMinMax(-10, -100) });
                        cliloc = 1060908; // Only fools take risks in fate’s shadow.
                        break;
                    }
                case 16:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.DefendChance, Value = Utility.RandomMinMax(1, 10) });
                        cliloc = 1060894; // A keen mind in battle will help you avoid injury.
                        break;
                    }
                case 17:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.DefendChance, Value = Utility.RandomMinMax(-1, -10) });
                        cliloc = 1060909; // Your lack of focus in battle shall be your undoing.
                        break;
                    }
                case 18:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.RegenMana, Value = Utility.RandomMinMax(1, 3) });
                        cliloc = 1060895; // The flow of the ether is strong within you.
                        break;
                    }
                case 19:
                    {
                        Fountains.Add(new SphynxFortuneArray { Mobile = from, Date = DateTime.UtcNow, Type = EnumType.AosAttribute, TypeValue = EnumTypeValue.RegenMana, Value = Utility.RandomMinMax(-1, -3) });
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
            return Fountains.Where(x => x.Mobile == from && (x.Type == EnumType.ResistanceType) && Enum.GetName(typeof(ResistanceType), type) == x.TypeValue.ToString()).Sum(y => y.Value);
        }

        public static int GetAosAttributeBonus(Mobile from, AosAttribute type)
        {
            return Fountains.Where(x => x.Mobile == from && (x.Type == EnumType.AosAttribute) && Enum.GetName(typeof(AosAttribute), type) == x.TypeValue.ToString()).Sum(y => y.Value);
        }
				
		public static void DefragTables()
		{
            Fountains.ForEach(x =>
            {
                if (DateTime.UtcNow > x.Date + TimeSpan.FromHours(24))
                {
                    x.Mobile.SendLocalizedMessage(1060859); // The effects of the Sphynx have worn off.
                }
            });
            Fountains.RemoveAll(x => DateTime.UtcNow > x.Date + TimeSpan.FromHours(24));
        }

        public static void StartTimer()
        {
            if (m_Timer != null && m_Timer.Running)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(DefragTables));
            m_Timer.Start();
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write((int)0);

                    writer.Write(Fountains.Count);

                    Fountains.ForEach(s =>
                    {
                        writer.Write(s.Mobile);
                        writer.Write(s.Date);
                        writer.Write((int)s.Type);
                        writer.Write((int)s.TypeValue);
                        writer.Write(s.Value);
                    });
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    int count = reader.ReadInt();

                    for (int i = count; i > 0; i--)
                    {
                        Mobile m = reader.ReadMobile();
                        DateTime dt = reader.ReadDateTime();
                        EnumType et = (EnumType)reader.ReadInt();
                        EnumTypeValue etv = (EnumTypeValue)reader.ReadInt();
                        int value = reader.ReadInt();

                        if (m != null)
                        {
                            Fountains.Add(new SphynxFortuneArray { Mobile = m, Date = dt, Type = et, TypeValue = etv, Value = value });
                        }
                    }
                });

            StartTimer();
        }
	}
}
