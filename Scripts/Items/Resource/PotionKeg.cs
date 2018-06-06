using System;

namespace Server.Items
{
    public class PotionKeg : Item
    {
        private PotionEffect m_Type;
        private int m_Held;
        [Constructable]
        public PotionKeg()
            : base(0x1940)
        {
            UpdateWeight();
        }

        public PotionKeg(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Held
        {
            get
            {
                return m_Held;
            }
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
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }
        public override int LabelNumber
        { 
            get
            {
                if (m_Held > 0 && (int)m_Type >= (int)PotionEffect.Conflagration)
                {
                    return 1072658 + (int)m_Type - (int)PotionEffect.Conflagration;
                }

                return (m_Held > 0 ? 1041620 + (int)m_Type : 1041641); 
            }
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

            writer.Write((int)1); // version

            writer.Write((int)m_Type);
            writer.Write((int)m_Held);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        m_Type = (PotionEffect)reader.ReadInt();
                        m_Held = reader.ReadInt();

                        break;
                    }
            }

            if (version < 1)
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(UpdateWeight));
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

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

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

            LabelTo(from, number);
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
                                from.SendLocalizedMessage(502245); // The keg is now empty.
                        }
                        else
                        {
                            from.SendLocalizedMessage(502244); // ...but there is no room for the bottle in your backpack.
                            pot.Delete();
                        }
                    }
                    else
                    {
                        // TODO: Target a bottle
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

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (item is BasePotion)
            {
                BasePotion pot = (BasePotion)item;
                int toHold = Math.Min(100 - m_Held, pot.Amount);

                if (pot.PotionEffect == PotionEffect.Darkglow || pot.PotionEffect == PotionEffect.Parasitic)
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
            switch ( m_Type )
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
            }
        }
    }
}