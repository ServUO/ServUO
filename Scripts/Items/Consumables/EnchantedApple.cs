using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Mysticism;
using Server.Spells.Necromancy;
using System;

namespace Server.Items
{
    public class EnchantedApple : BaseMagicalFood
    {
        [Constructable]
        public EnchantedApple()
            : base(0x2FD8)
        {
            Weight = 1.0;
            Hue = 0x488;
            Stackable = true;
        }

        public EnchantedApple(Serial serial)
            : base(serial)
        {
        }

        public override MagicalFood FoodID => MagicalFood.EnchantedApple;
        public override TimeSpan Cooldown => TimeSpan.FromSeconds(30);
        public override int EatMessage => 1074846;// A tasty bite of the enchanted apple lifts all curses from your soul.

        public override bool Eat(Mobile from)
        {
            if (!IsUnderInfluence(from, FoodID))
            {
                if (CoolingDown(from, FoodID))
                {
                    from.SendLocalizedMessage(1151180); // You must wait a while before eating another enchanted apple.
                }
                else
                {
                    from.PlaySound(0xF6);
                    from.PlaySound(0x1F7);
                    from.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);

                    IEntity mfrom = new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z - 10), from.Map);
                    IEntity mto = new Entity(Serial.Zero, new Point3D(from.X, from.Y, from.Z + 50), from.Map);
                    Effects.SendMovingParticles(mfrom, mto, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

                    int power = CleansingWindsSpell.RemoveCurses(from);
                    power = Math.Min(power, 15);

                    from.SendLocalizedMessage(EatMessage);

                    StartInfluence(from, FoodID, Duration, TimeSpan.FromSeconds(30 + power));
                    Consume();

                    return true;
                }
            }

            return false;
        }

        public static int GetTotalCurses(Mobile m)
        {
            int curses = 0;

            if (EvilOmenSpell.UnderEffects(m))
            {
                curses++;
            }

            if (StrangleSpell.UnderEffects(m))
            {
                curses++;
            }

            if (CorpseSkinSpell.IsUnderEffects(m))
            {
                curses++;
            }

            if (BloodOathSpell.GetBloodOath(m) != null)
            {
                curses++;
            }

            if (MindRotSpell.HasMindRotScalar(m))
            {
                curses++;
            }

            if (SpellPlagueSpell.HasSpellPlague(m))
            {
                curses++;
            }

            if (SleepSpell.IsUnderSleepEffects(m))
            {
                curses++;
            }

            if (CurseSpell.UnderEffect(m))
            {
                curses++;
            }

            if (FeeblemindSpell.IsUnderEffects(m))
            {
                curses++;
            }

            if (ClumsySpell.IsUnderEffects(m))
            {
                curses++;
            }

            if (WeakenSpell.IsUnderEffects(m))
            {
                curses++;
            }

            return curses;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
