using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class CandyCane : Food
    {
        private static Dictionary<Mobile, CandyCaneTimer> m_ToothAches;
        [Constructable]
        public CandyCane()
            : this(0x2bdd + Utility.Random(4))
        {
        }

        public CandyCane(int itemID)
            : base(itemID)
        {
            this.Stackable = false;
            this.LootType = LootType.Blessed;
        }

        public CandyCane(Serial serial)
            : base(serial)
        {
        }

        public static Dictionary<Mobile, CandyCaneTimer> ToothAches
        {
            get
            {
                return m_ToothAches;
            }
            set
            {
                m_ToothAches = value;
            }
        }
        public static void Initialize()
        {
            m_ToothAches = new Dictionary<Mobile, CandyCaneTimer>();
        }

        public static int GetToothAche(Mobile from)
        {
            CandyCaneTimer timer;

            if (m_ToothAches.TryGetValue(from, out timer))
                return timer.Eaten;

            return 0;
        }

        public static void SetToothAche(Mobile from, int value)
        {
            EnsureTimer(from).Eaten = value;
        }

        public override bool CheckHunger(Mobile from)
        {
            EnsureTimer(from).Eaten += 32;

            from.SendLocalizedMessage(1077387); // You feel as if you could eat as much as you wanted!
            return true;
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

        private static CandyCaneTimer EnsureTimer(Mobile from)
        {
            CandyCaneTimer timer;

            if (!m_ToothAches.TryGetValue(from, out timer))
                m_ToothAches[from] = timer = new CandyCaneTimer(from);

            return timer;
        }

        public class CandyCaneTimer : Timer
        {
            private readonly Mobile m_Eater;
            private int m_Eaten;
            public CandyCaneTimer(Mobile eater)
                : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
            {
                this.m_Eater = eater;
                this.Priority = TimerPriority.FiveSeconds;
                this.Start();
            }

            public Mobile Eater
            {
                get
                {
                    return this.m_Eater;
                }
            }
            public int Eaten
            {
                get
                {
                    return this.m_Eaten;
                }
                set
                {
                    this.m_Eaten = value;
                }
            }
            protected override void OnTick()
            {
                --this.m_Eaten;

                if (this.m_Eater == null || this.m_Eater.Deleted || this.m_Eaten <= 0)
                {
                    this.Stop();
                    m_ToothAches.Remove(this.m_Eater);
                }
                else if (this.m_Eater.Map != Map.Internal && this.m_Eater.Alive)
                {
                    if (this.m_Eaten > 60)
                    {
                        this.m_Eater.Say(1077388 + Utility.Random(5));
                        /* ARRGH! My tooth hurts sooo much!
                        * You just can't find a good Britannian dentist these days...
                        * My teeth!
                        * MAKE IT STOP!
                        * AAAH! It feels like someone kicked me in the teeth!
                        */

                        if (Utility.RandomBool() && this.m_Eater.Body.IsHuman && !this.m_Eater.Mounted)
                            this.m_Eater.Animate(32, 5, 1, true, false, 0);
                    }
                    else if (this.m_Eaten == 60)
                    {
                        this.m_Eater.SendLocalizedMessage(1077393); // The extreme pain in your teeth subsides.
                    }
                }
            }
        }
    }

    public class GingerBreadCookie : Food
    {
        private readonly int[] m_Messages =
        {
            0,
            1077396, // Noooo!
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
            this.Stackable = false;
            this.LootType = LootType.Blessed;
        }

        public GingerBreadCookie(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            int message = this.m_Messages[Utility.Random(this.m_Messages.Length)];

            if (message != 0)
            {
                this.SendLocalizedMessageTo(from, message);
                return false;
            }

            return base.Eat(from);
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
    }
}