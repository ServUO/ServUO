using System;
using System.Collections.Generic;

using Server.Mobiles;

namespace Server.Items
{
    public class PotionOfGloriousFortune : Item
    {
        public override int LabelNumber { get { return 1158688; } } // Potion of Glorious Fortune

        public static int Bonus = 400;

        [Constructable]
        public PotionOfGloriousFortune()
            : base(0xA1E6)
        {
            Hue = 1195;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                if (TryAddEffects(m))
                {
                    Consume();
                }
            }
        }

        public PotionOfGloriousFortune(Serial serial)
            : base(serial)
        {
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
        }

        public static Dictionary<Mobile, DateTime> Table { get; set; }
        public static Timer Timer { get; set; }

        public static void OnTick()
        {
            if (Table == null)
            {
                return;
            }

            var list = new List<Mobile>(Table.Keys);

            foreach(var m in list)
            {
                UnderEffects(m);
            }

            ColUtility.Free(list);
        }

        public static int GetBonus(Mobile m)
        {
            if (UnderEffects(m))
            {
                return (int)(Bonus / 100);
            }

            return 1;
        }

        public static bool TryAddEffects(Mobile m)
        {
            if (Table == null)
            {
                Table = new Dictionary<Mobile, DateTime>();
            }

            if (!UnderEffects(m))
            {
                Table[m] = DateTime.UtcNow + TimeSpan.FromHours(1);

                m.SendLocalizedMessage(1158719); // You are now under the effect of the Potion of Glorious Fortune.

                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.PotionGloriousFortune, 1158688, 1158720, TimeSpan.FromMinutes(60), m, Bonus.ToString(), true));

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

        public static bool UnderEffects(Mobile m)
        {
            if (Table != null && Table.ContainsKey(m))
            {
                if (Table[m] < DateTime.UtcNow)
                {
                    ExpireBuff(m);

                    return false;
                }

                return true;
            }

            return false;
        }

        public static void ExpireBuff(Mobile m)
        {
            Table.Remove(m);

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

        public static void Save(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Table == null ? 0 : Table.Count);

            if (Table != null)
            {
                foreach (var kpv in Table)
                {
                    writer.Write(kpv.Key);
                    writer.Write(kpv.Value);
                }
            }
        }

        public static void Load(GenericReader reader)
        {
            reader.ReadInt(); // version

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                var bc = reader.ReadMobile();
                var dt = reader.ReadDateTime();

                if (bc != null && dt > DateTime.UtcNow)
                {
                    if (Table == null)
                        Table = new Dictionary<Mobile, DateTime>();

                    Table[bc] = dt;

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
            var pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                if (Table != null && UnderEffects(pm))
                {
                    BuffInfo.AddBuff(pm, new BuffInfo(BuffIcon.PotionGloriousFortune, 1158688, 1158720, Table[pm] - DateTime.UtcNow, pm, Bonus.ToString(), true));
                }
            }
        }
    }
}
