#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Items
{
    public abstract class BaseSweet : Food
    {
        private static readonly Dictionary<Mobile, ToothAcheTimer> _ToothAches = new Dictionary<Mobile, ToothAcheTimer>();

        public static Dictionary<Mobile, ToothAcheTimer> ToothAches => _ToothAches;

        private static ToothAcheTimer EnsureTimer(Mobile m, int acidity)
        {
            if (!_ToothAches.ContainsKey(m))
            {
                _ToothAches.Add(m, new ToothAcheTimer(m, acidity));
            }
            else
            {
                _ToothAches[m].Acidity += acidity;
            }

            _ToothAches[m].Running = true;

            return _ToothAches[m];
        }

        public static void SetToothAche(Mobile from, int acidity, bool direct)
        {
            ToothAcheTimer t = EnsureTimer(from, acidity);

            if (direct)
            {
                t.Acidity = acidity;
            }
        }

        public static int GetToothAche(Mobile m)
        {
            ToothAcheTimer t;

            return _ToothAches.TryGetValue(m, out t) ? t.Acidity : 0;
        }

        public static bool CureToothAche(Mobile m)
        {
            return m != null && _ToothAches.Remove(m);
        }

        public virtual bool GivesToothAche => true;
        public virtual int Acidity => 32;

        public BaseSweet(int itemID)
            : base(itemID)
        { }

        public BaseSweet(int amount, int itemID)
            : base(amount, itemID)
        { }

        public BaseSweet(Serial serial)
            : base(serial)
        { }

        public override bool CheckHunger(Mobile from)
        {
            if (GivesToothAche)
            {
                EnsureTimer(from, Acidity);
            }

            from.SendLocalizedMessage(1077387); // You feel as if you could eat as much as you wanted!
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

            reader.ReadInt(); // version
        }

        public class ToothAcheTimer : Timer
        {
            public Mobile ConsumedBy { get; private set; }
            public int Acidity { get; set; }

            public ToothAcheTimer(Mobile consumedBy, int acidity)
                : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
            {
                ConsumedBy = consumedBy;
                Acidity = acidity;

                Priority = TimerPriority.FiveSeconds;

                Start();
            }

            private bool Invalidate()
            {
                return ConsumedBy != null && _ToothAches.ContainsKey(ConsumedBy) && !ConsumedBy.Deleted && Acidity > 0 &&
                       ConsumedBy.Map != null && ConsumedBy.Map != Map.Internal && ConsumedBy.Alive;
            }

            protected override void OnTick()
            {
                --Acidity;

                if (!Invalidate())
                {
                    Stop();

                    if (ConsumedBy != null)
                    {
                        _ToothAches.Remove(ConsumedBy);
                    }

                    return;
                }

                if (Acidity == 60)
                {
                    ConsumedBy.SendLocalizedMessage(1077393); // The extreme pain in your teeth subsides.
                    return;
                }

                if (Acidity <= 60)
                {
                    return;
                }

                /* 
				* ARRGH! My tooth hurts sooo much!
				* You just can't find a good Britannian dentist these days...
				* My teeth!
				* MAKE IT STOP!
				* AAAH! It feels like someone kicked me in the teeth!
				*/
                ConsumedBy.Say(1077388 + Utility.Random(5));

                if (Utility.RandomBool() && ConsumedBy.Body.IsHuman && !ConsumedBy.Mounted)
                {
                    ConsumedBy.Animate(32, 5, 1, true, false, 0);
                }
            }
        }
    }

    public class CandyCane : BaseSweet
    {
        [Constructable]
        public CandyCane()
            : this(0x2bdd + Utility.Random(4))
        { }

        public CandyCane(int itemID)
            : base(itemID)
        {
            Stackable = false;
            LootType = LootType.Blessed;
        }

        public CandyCane(Serial serial)
            : base(serial)
        { }

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
    }

    public class GingerBreadCookie : BaseSweet
    {
        private readonly int[] m_Messages =
        {
            0, 1077396, // Noooo!
			1077397, // Please don't eat me... *whimper*
			1077405, // Not the face!
			1077406, // Ahhhhhh! My foot’s gone!
			1077407, // Please. No! I have gingerkids!
			1077408, // No, no! I’m really made of poison. Really.
			1077409 // Run, run as fast as you can! You can't catch me! I'm the gingerbread man!
		};

        [Constructable]
        public GingerBreadCookie()
            : base(Utility.RandomBool() ? 0x2be1 : 0x2be2)
        {
            Stackable = false;
            LootType = LootType.Blessed;
        }

        public GingerBreadCookie(Serial serial)
            : base(serial)
        { }

        public override bool Eat(Mobile from)
        {
            int message = m_Messages[Utility.Random(m_Messages.Length)];

            if (message != 0)
            {
                SendLocalizedMessageTo(from, message);
                return false;
            }

            return base.Eat(from);
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
    }
}