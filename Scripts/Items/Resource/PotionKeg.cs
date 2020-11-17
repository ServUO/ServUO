using Server.Targeting;
using System;

namespace Server.Items
{
    public class PotionKeg : Item
    {
        private PotionEffect m_Type;
        private int m_Held;
        private bool m_Unknown;

        [Constructable]
        public PotionKeg()
            : base(0x1940)
        {
            Unknown = true;
            UpdateWeight();
        }

        public PotionKeg(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Unknown
        {
            get { return m_Unknown; }
            set
            {
                m_Unknown = value; InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Held
        {
            get { return m_Held; }
            set
            {
                if (m_Held != value)
                {
                    m_Held = value;
                    UpdateWeight();
                    InvalidateProperties();
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PotionEffect Type
        {
            get { return m_Type; }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        public override int LabelNumber { get { return GetLabelNumber(); } }

        public int GetLabelNumber()
        {
            int number = 1041610; // A keg of strange liquid.

            if (m_Held > 0)
            {
                if (m_Unknown)
                {
                    switch (m_Type)
                    {
                        case PotionEffect.Nightsight: number = 1041611; break; // A keg of black liquid.
                        case PotionEffect.CureLesser: number = 1041612; break; // A keg of orange liquid.
                        case PotionEffect.Cure: number = 1041612; break; // A keg of orange liquid.
                        case PotionEffect.CureGreater: number = 1041612; break; // A keg of orange liquid.
                        case PotionEffect.Agility: number = 1041613; break; // A keg of blue liquid.
                        case PotionEffect.AgilityGreater: number = 1041613; break; // A keg of blue liquid.
                        case PotionEffect.Strength: number = 1041614; break; // A keg of white liquid.
                        case PotionEffect.StrengthGreater: number = 1041614; break; // A keg of white liquid.
                        case PotionEffect.PoisonLesser: number = 1041615; break; // A keg of green liquid.
                        case PotionEffect.Poison: number = 1041615; break; // A keg of green liquid.
                        case PotionEffect.PoisonGreater: number = 1041615; break; // A keg of green liquid.
                        case PotionEffect.Refresh: number = 1041616; break; // A keg of red liquid.
                        case PotionEffect.RefreshTotal: number = 1041616; break; // A keg of red liquid.
                        case PotionEffect.HealLesser: number = 1041617; break; // A keg of yellow liquid.
                        case PotionEffect.Heal: number = 1041617; break; // A keg of yellow liquid.
                        case PotionEffect.HealGreater: number = 1041617; break; // A keg of yellow liquid.
                        case PotionEffect.ExplosionLesser: number = 1041618; break; // A keg of purple liquid.
                        case PotionEffect.Explosion: number = 1041618; break; // A keg of purple liquid.
                        case PotionEffect.ExplosionGreater: number = 1041618; break; // A keg of purple liquid.
                        case PotionEffect.Conflagration: number = 1072654; break; // A keg of fiery liquid.
                        case PotionEffect.ConflagrationGreater: number = 1072654; break; // A keg of fiery liquid.
                        case PotionEffect.ConfusionBlast: number = 1072656; break; // A keg of muddled liquid.
                        case PotionEffect.ConfusionBlastGreater: number = 1072656; break; // A keg of muddled liquid.
                        case PotionEffect.Invisibility: number = 1080071; break; // A keg of Invisibility potions
                        case PotionEffect.Parasitic: number = 1080067; break; // A Keg of Murky Liquid.
                        case PotionEffect.Darkglow: number = 1080068; break; // A Keg of Baneful Liquid.
                    }
                }
                else
                {
                    switch (m_Type)
                    {
                        case PotionEffect.Conflagration: number = 1072658; break; // A keg of Conflagration potions
                        case PotionEffect.ConflagrationGreater: number = 1072659; break; // A keg of Greater Conflagration potions
                        case PotionEffect.ConfusionBlast: number = 1072662; break; // A keg of Confusion Blast potions
                        case PotionEffect.ConfusionBlastGreater: number = 1072663; break; // A keg of Greater Confusion Blast potions
                        case PotionEffect.Invisibility: number = 1080071; break; // A keg of Invisibility potions
                        case PotionEffect.Parasitic: number = 1080069; break; // A keg of Parasitic Poison potions
                        case PotionEffect.Darkglow: number = 1080070; break; // A keg of Darkglow Poison potions
                        default: number = 1041620 + (int)m_Type; break;
                    }
                }
            }
            else
            {
                number = 1041084; // A specially lined keg for potions.
            }

            return number;
        }

        public static void Initialize()
        {
            TileData.ItemTable[0x1940].Height = 4;
        }

        public virtual void UpdateWeight()
        {
            int held = Math.Max(0, Math.Min(m_Held, 100));

            Weight = 20 + ((held * 80) / 100);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2); // version

            writer.Write((bool)m_Unknown);
            writer.Write((int)m_Type);
            writer.Write(m_Held);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        m_Unknown = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                case 0:
                    {
                        m_Type = (PotionEffect)reader.ReadInt();
                        m_Held = reader.ReadInt();

                        break;
                    }
            }

            if (version < 1)
                Timer.DelayCall(TimeSpan.Zero, UpdateWeight);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int number;

            if (m_Held <= 0)
                number = 502246; // The keg is empty.
            else if (m_Held < 5)
                number = 502248; // The keg is nearly empty.
            else if (m_Held < 20)
                number = 502249; // The keg is not very full.
            else if (m_Held < 30)
                number = 502250; // The keg is about one quarter full.
            else if (m_Held < 40)
                number = 502251; // The keg is about one third full.
            else if (m_Held < 47)
                number = 502252; // The keg is almost half full.
            else if (m_Held < 54)
                number = 502254; // The keg is approximately half full.
            else if (m_Held < 70)
                number = 502253; // The keg is more than half full.
            else if (m_Held < 80)
                number = 502255; // The keg is about three quarters full.
            else if (m_Held < 96)
                number = 502256; // The keg is very full.
            else if (m_Held < 100)
                number = 502257; // The liquid is almost to the top of the keg.
            else
                number = 502258; // The keg is completely full.

            list.Add(number);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                if (m_Held > 0)
                {
                    Container pack = from.Backpack;

                    if (pack != null && pack.ConsumeTotal(typeof(Bottle), 1))
                    {
                        from.SendLocalizedMessage(502242); // You pour some of the keg's contents into an empty bottle...

                        BasePotion pot = FillBottle();

                        if (pack.TryDropItem(from, pot, false))
                        {
                            from.SendLocalizedMessage(502243); // ...and place it into your backpack.
                            from.PlaySound(0x240);

                            if (--Held == 0)
                            {
                                from.SendLocalizedMessage(502245); // The keg is now empty.							
                            }
                            else
                            {
                                Unknown = false;
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(502244); // ...but there is no room for the bottle in your backpack.
                            pot.Delete();
                        }
                    }
                    else
                    {
                        from.Target = new BottleTarget(this);

                        from.SendLocalizedMessage(502241); // Where is a container for your potion?
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502246); // The keg is empty.
                }
            }
            else
            {
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        private class BottleTarget : Target
        {
            private readonly PotionKeg m_Keg;

            public BottleTarget(PotionKeg keg)
                : base(12, true, TargetFlags.None)
            {
                m_Keg = keg;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Bottle bottle)
                {
                    Container pack = from.Backpack;

                    from.SendLocalizedMessage(502242); // You pour some of the keg's contents into an empty bottle...

                    BasePotion pot = m_Keg.FillBottle();

                    if (pack.TryDropItem(from, pot, false))
                    {
                        from.SendLocalizedMessage(502243); // ...and place it into your backpack.
                        from.PlaySound(0x240);

                        if (--m_Keg.Held == 0)
                            from.SendLocalizedMessage(502245); // The keg is now empty.							
                        else
                            m_Keg.Unknown = false;

                        bottle.Consume();
                    }
                    else
                    {
                        from.SendLocalizedMessage(502244); // ...but there is no room for the bottle in your backpack.

                        from.SendLocalizedMessage(502217); // Nothing comes out of the tap!
                        pot.Delete();
                    }

                }
                else
                {
                    from.SendLocalizedMessage(502227); // That cannot be used to hold a potion.
                }
            }
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (item is BasePotion pot)
            {
                int toHold = Math.Min(100 - m_Held, pot.Amount);

                if (pot.PotionEffect > PotionEffect.Darkglow)
                {
                    from.SendLocalizedMessage(502232); // The keg is not designed to hold that type of object.
                    return false;
                }
                else if (toHold <= 0)
                {
                    from.SendLocalizedMessage(502233); // The keg will not hold any more!
                    return false;
                }
                else if (m_Held == 0)
                {
                    if (GiveBottle(from, toHold))
                    {
                        m_Type = pot.PotionEffect;
                        Held = toHold;

                        from.PlaySound(0x240);

                        from.SendLocalizedMessage(502237); // You place the empty bottle in your backpack.

                        item.Consume(toHold);

                        if (!item.Deleted)
                            item.Bounce(from);

                        return true;
                    }
                    else
                    {
                        from.SendLocalizedMessage(502238); // You don't have room for the empty bottle in your backpack.
                        return false;
                    }
                }
                else if (pot.PotionEffect != m_Type)
                {
                    from.SendLocalizedMessage(502236); // You decide that it would be a bad idea to mix different types of potions.
                    return false;
                }
                else
                {
                    if (GiveBottle(from, toHold))
                    {
                        Held += toHold;

                        from.PlaySound(0x240);

                        from.SendLocalizedMessage(502237); // You place the empty bottle in your backpack.

                        item.Consume(toHold);

                        if (!item.Deleted)
                            item.Bounce(from);

                        return true;
                    }
                    else
                    {
                        from.SendLocalizedMessage(502238); // You don't have room for the empty bottle in your backpack.
                        return false;
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(502232); // The keg is not designed to hold that type of object.
                return false;
            }
        }

        public bool GiveBottle(Mobile m, int amount)
        {
            Container pack = m.Backpack;

            Bottle bottle = new Bottle(amount);

            if (pack == null || !pack.TryDropItem(m, bottle, false))
            {
                bottle.Delete();
                return false;
            }

            return true;
        }

        public BasePotion FillBottle()
        {
            switch (m_Type)
            {
                default:
                case PotionEffect.Nightsight:
                    return new NightSightPotion();

                case PotionEffect.CureLesser:
                    return new LesserCurePotion();
                case PotionEffect.Cure:
                    return new CurePotion();
                case PotionEffect.CureGreater:
                    return new GreaterCurePotion();

                case PotionEffect.Agility:
                    return new AgilityPotion();
                case PotionEffect.AgilityGreater:
                    return new GreaterAgilityPotion();

                case PotionEffect.Strength:
                    return new StrengthPotion();
                case PotionEffect.StrengthGreater:
                    return new GreaterStrengthPotion();

                case PotionEffect.PoisonLesser:
                    return new LesserPoisonPotion();
                case PotionEffect.Poison:
                    return new PoisonPotion();
                case PotionEffect.PoisonGreater:
                    return new GreaterPoisonPotion();
                case PotionEffect.PoisonDeadly:
                    return new DeadlyPoisonPotion();

                case PotionEffect.Refresh:
                    return new RefreshPotion();
                case PotionEffect.RefreshTotal:
                    return new TotalRefreshPotion();

                case PotionEffect.HealLesser:
                    return new LesserHealPotion();
                case PotionEffect.Heal:
                    return new HealPotion();
                case PotionEffect.HealGreater:
                    return new GreaterHealPotion();

                case PotionEffect.ExplosionLesser:
                    return new LesserExplosionPotion();
                case PotionEffect.Explosion:
                    return new ExplosionPotion();
                case PotionEffect.ExplosionGreater:
                    return new GreaterExplosionPotion();

                case PotionEffect.Conflagration:
                    return new ConflagrationPotion();
                case PotionEffect.ConflagrationGreater:
                    return new GreaterConflagrationPotion();

                case PotionEffect.ConfusionBlast:
                    return new ConfusionBlastPotion();
                case PotionEffect.ConfusionBlastGreater:
                    return new GreaterConfusionBlastPotion();

                case PotionEffect.Invisibility:
                    return new InvisibilityPotion();
                case PotionEffect.Parasitic:
                    return new ParasiticPotion();
                case PotionEffect.Darkglow:
                    return new DarkglowPotion();
            }
        }
    }
}
