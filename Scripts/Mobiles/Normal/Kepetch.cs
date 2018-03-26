using System;
using Server.Items;
using Server.Network;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a kepetch corpse")]
    public class Kepetch : BaseCreature, ICarvable
    {
        public bool GatheredFur { get; set; }

        [Constructable]
        public Kepetch()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a kepetch";
            Body = 726;

            SetStr(337, 354);
            SetDex(184, 194);
            SetInt(32, 37);

            SetHits(308, 366);

            SetDamage(7, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 45);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.Anatomy, 119.7, 124.1);
            SetSkill(SkillName.MagicResist, 89.9, 97.4);
            SetSkill(SkillName.Tactics, 117.4, 123.5);
            SetSkill(SkillName.Wrestling, 107.7, 113.9);
        }

        public Kepetch(Serial serial)
            : base(serial)
        {
        }
        public override int Meat
        {
            get
            {
                return 5;
            }
        }
        public override int Hides
        {
            get
            {
                return 14;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }

        public bool Carve(Mobile from, Item item)
        {
            if (!GatheredFur)
            {
                var fur = new KepetchFur(30);

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, fur, false))
                {
                    from.SendLocalizedMessage(1112359); // You would not be able to place the gathered kepetch fur in your backpack!
                    fur.Delete();
                }
                else
                {
                    from.SendLocalizedMessage(1112360); // You place the gathered kepetch fur into your backpack.
                    GatheredFur = true;
                    return true;
                }
            }
            else
                from.SendLocalizedMessage(1112358); // The Kepetch nimbly escapes your attempts to shear its mane.

            return false;
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            base.OnCarve(from, corpse, with);

            if (!GatheredFur)
            {
                from.SendLocalizedMessage(1112765); // You shear it, and the fur is now on the corpse.
                corpse.AddCarvedItem(new KepetchFur(15), from);
                GatheredFur = true;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
        }

        public override int GetIdleSound()
        {
            return 1545;
        }

        public override int GetAngerSound()
        {
            return 1542;
        }

        public override int GetHurtSound()
        {
            return 1544;
        }

        public override int GetDeathSound()
        {
            return 1543;
        }

        // To Do: Infected Wound (5 +5(every 20 seconds)damage) 

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            if (Utility.RandomDouble() < 0.05)
            {
                //defender.SendLocalizedMessage(1113211); //The kepetch gives you a particularly vicious bite! 
                defender.LocalOverheadMessage(MessageType.Regular, 0x3B2, false,
                "The kepetch gives you a particularly vicious bite!");
                defender.PlaySound(0x133);
                defender.FixedParticles(0x377A, 244, 25, 9950, 31, 0, EffectLayer.Waist);

                BeginBleed(defender, this);
            }
        }

        private static Hashtable m_Table = new Hashtable();
        private static Hashtable m_BleedTable = new Hashtable();

        public static bool IsBleeding(Mobile m)
        {
            return m_BleedTable.Contains(m);
        }

        public static void BeginBleed(Mobile m, Mobile from)
        {
            Timer t = (Timer)m_BleedTable[m];

            if (t != null)
                t.Stop();

            t = new InternalBleedTimer(from, m);
            m_BleedTable[m] = t;

            t.Start();
        }

        public static void DoBleed(Mobile m, Mobile from, int level)
        {
            if (m.Alive)
            {
                int damage = Utility.RandomMinMax(level, level * 1);//2

                if (!m.Player)
                    damage *= 2;

                m.PlaySound(0x133);
                m.Damage(damage, from);

                Blood blood = new Blood();

                blood.ItemID = Utility.Random(0x122A, 5);

                blood.MoveToWorld(m.Location, m.Map);
            }
            else
            {
                EndBleed(m, false);
            }
        }

        public static void EndBleed(Mobile m, bool message)
        {
            Timer t = (Timer)m_BleedTable[m];

            if (t == null)
                return;

            t.Stop();
            m_Table.Remove(m);

            m.SendLocalizedMessage(1113365); // Your wounds have been mended.
        }

        private class InternalBleedTimer : Timer
        {
            private Mobile m_From;
            private Mobile m_Mobile;
            private int m_Count;

            public InternalBleedTimer(Mobile from, Mobile m)
                : base(TimeSpan.FromSeconds(20.0), TimeSpan.FromSeconds(20.0))
            {
                m_From = from;
                m_Mobile = m;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                DoBleed(m_Mobile, m_From, 5 + 5 * m_Count);

                if (++m_Count == 5)
                    EndBleed(m_Mobile, true);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2);
            writer.Write(GatheredFur);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            var version = reader.ReadInt();

            if (version == 1)
                reader.ReadDeltaTime();
            else
                GatheredFur = reader.ReadBool();
        }
    }
}