using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Server.Items
{
    public enum PotionEventType
    {
        Khaldun,
        Soulbinder
    }

    public class PotionArray
    {
        public Mobile Mobile { get; set; }
        public DateTime Date { get; set; }
        public PotionEventType Type { get; set; }
    }

    public class PotionOfGloriousFortune : Item
    {
        public override int LabelNumber => 1158688;  // Potion of Glorious Fortune        

        [CommandProperty(AccessLevel.GameMaster)]
        public PotionEventType _Type { get; set; }

        [Constructable]
        public PotionOfGloriousFortune()
            : this(PotionEventType.Soulbinder)
        {
        }

        [Constructable]
        public PotionOfGloriousFortune(PotionEventType type)
            : base(0xA1E6)
        {
            _Type = type;
            Hue = 1195;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) && TryAddEffects(from))
            {
                Consume();
            }
        }

        public PotionOfGloriousFortune(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }

        public static List<PotionArray> Table { get; set; }
        public static Timer Timer { get; set; }

        public static void OnTick()
        {
            if (Table == null)
            {
                return;
            }

            var list = new List<PotionArray>(Table);

            foreach (var l in list)
            {
                UnderEffects(l.Mobile, l.Type);
            }

            ColUtility.Free(list);
        }

        public static int GetBonus(Mobile m, PotionEventType type)
        {
            if (UnderEffects(m, type))
            {
                return GetScaler(type) / 100;
            }

            return 1;
        }

        public static int GetScaler(PotionEventType type)
        {
            return type == PotionEventType.Khaldun ? 400 : 50;
        }

        public bool TryAddEffects(Mobile m)
        {
            if (Table == null)
            {
                Table = new List<PotionArray>();
            }

            if (!UnderEffects(m, _Type))
            {
                Table.Add(new PotionArray { Mobile = m, Date = DateTime.UtcNow + TimeSpan.FromHours(1), Type = _Type });

                m.SendLocalizedMessage(1158719); // You are now under the effect of the Potion of Glorious Fortune.

                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.PotionGloriousFortune, 1158688, 1158720, TimeSpan.FromMinutes(60), m, GetScaler(_Type).ToString(), true));

                m.FixedEffect(0x375A, 10, 15);
                m.PlaySound(0x1E7);

                if (Timer == null)
                {
                    Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnTick);
                    Timer.Start();
                }

                return true;
            }
            else
            {
                m.SendLocalizedMessage(1158718); // You are already under the effect of the Potion of Glorious Fortune.
            }

            return false;
        }

        public static bool UnderEffects(Mobile m, PotionEventType type)
        {
            if (Table != null)
            {
                var list = Table.FirstOrDefault(x => x.Mobile == m && x.Type == type);

                if (list != null)
                {
                    if (list.Date < DateTime.UtcNow)
                    {
                        ExpireBuff(m);

                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public static void ExpireBuff(Mobile m)
        {
            Table.RemoveAll(x => x.Mobile == m);

            if (Table.Count == 0)
            {
                Table = null;

                if (Timer != null)
                {
                    Timer.Stop();
                    Timer = null;
                }
            }
        }

        private static readonly string FilePath = Path.Combine("Saves/Misc", "PotionOfGloriousFortune.bin");

        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(0);

                    if (Table != null)
                    {
                        writer.Write(Table.Count);

                        Table.ForEach(s =>
                        {
                            writer.Write(s.Mobile);
                            writer.Write(s.Date);
                            writer.Write((int)s.Type);
                        });
                    }
                    else
                    {
                        writer.Write(0);
                    }
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
                        var m = reader.ReadMobile();
                        var dt = reader.ReadDateTime();
                        var et = (PotionEventType)reader.ReadInt();

                        if (m != null && dt > DateTime.UtcNow)
                        {
                            if (Table == null)
                                Table = new List<PotionArray>();

                            Table.Add(new PotionArray { Mobile = m, Date = dt, Type = et });

                            if (Timer == null)
                            {
                                Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnTick);
                                Timer.Start();
                            }
                        }
                    }
                });            
        }

        public static void OldLoad(GenericReader reader)
        {
            reader.ReadInt(); // version

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Mobile bc = reader.ReadMobile();
                DateTime dt = reader.ReadDateTime();

                if (bc != null && dt > DateTime.UtcNow)
                {
                    if (Table == null)
                        Table = new List<PotionArray>();

                    Table.Add(new PotionArray { Mobile = bc, Date = dt, Type = PotionEventType.Khaldun });

                    if (Timer == null)
                    {
                        Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnTick);
                        Timer.Start();
                    }
                }
            }
        }

        public static void Initialize()
        {
            EventSink.Login += OnLogin;
        }

        public static void OnLogin(LoginEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm && Table != null)
            {
                var list = Table.FirstOrDefault(x => x.Mobile == pm);

                if (list != null && UnderEffects(pm, list.Type))
                {
                    BuffInfo.AddBuff(pm, new BuffInfo(BuffIcon.PotionGloriousFortune, 1158688, 1158720, list.Date - DateTime.UtcNow, pm, GetScaler(list.Type).ToString(), true));
                }
            }
        }
    }
}
