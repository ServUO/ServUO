using System;

namespace Server.Items
{
    public enum TrapType
    {
        None,
        MagicTrap,
        ExplosionTrap,
        DartTrap,
        PoisonTrap
    }

    public abstract class TrapableContainer : BaseContainer, ITelekinesisable
    {
        private TrapType m_TrapType;
        private int m_TrapPower;
        private int m_TrapLevel;
        public TrapableContainer(int itemID)
            : base(itemID)
        {
        }

        public TrapableContainer(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TrapType TrapType
        {
            get
            {
                return m_TrapType;
            }
            set
            {
                m_TrapType = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int TrapPower
        {
            get
            {
                return m_TrapPower;
            }
            set
            {
                m_TrapPower = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int TrapLevel
        {
            get
            {
                return m_TrapLevel;
            }
            set
            {
                m_TrapLevel = value;
            }
        }
        public virtual bool TrapOnOpen => true;
        public virtual bool ExecuteTrap(Mobile from)
        {
            if (m_TrapType != TrapType.None)
            {
                Point3D loc = GetWorldLocation();
                Map facet = Map;

                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    SendMessageTo(from, "That is trapped, but you open it with your godly powers.", 0x3B2);
                    return false;
                }

                switch (m_TrapType)
                {
                    case TrapType.ExplosionTrap:
                        {
                            SendMessageTo(from, 502999, 0x3B2); // You set off a trap!

                            if (from.InRange(loc, 3))
                            {
                                int damage;

                                if (m_TrapLevel > 0)
                                    damage = Utility.RandomMinMax(10, 30) * m_TrapLevel;
                                else
                                    damage = m_TrapPower;

                                AOS.Damage(from, damage, 0, 100, 0, 0, 0);

                                // Your skin blisters from the heat!
                                from.LocalOverheadMessage(Network.MessageType.Regular, 0x2A, 503000);
                            }

                            Effects.SendLocationEffect(loc, facet, 0x36BD, 15, 10);
                            Effects.PlaySound(loc, facet, 0x307);

                            break;
                        }
                    case TrapType.MagicTrap:
                        {
                            if (from.InRange(loc, 1))
                                from.Damage(m_TrapPower);
                            //AOS.Damage( from, m_TrapPower, 0, 100, 0, 0, 0 );

                            Effects.PlaySound(loc, Map, 0x307);

                            Effects.SendLocationEffect(new Point3D(loc.X - 1, loc.Y, loc.Z), Map, 0x36BD, 15);
                            Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y, loc.Z), Map, 0x36BD, 15);

                            Effects.SendLocationEffect(new Point3D(loc.X, loc.Y - 1, loc.Z), Map, 0x36BD, 15);
                            Effects.SendLocationEffect(new Point3D(loc.X, loc.Y + 1, loc.Z), Map, 0x36BD, 15);

                            Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y + 1, loc.Z + 11), Map, 0x36BD, 15);

                            break;
                        }
                    case TrapType.DartTrap:
                        {
                            SendMessageTo(from, 502999, 0x3B2); // You set off a trap!

                            if (from.InRange(loc, 3))
                            {
                                int damage;

                                if (m_TrapLevel > 0)
                                    damage = Utility.RandomMinMax(5, 15) * m_TrapLevel;
                                else
                                    damage = m_TrapPower;

                                AOS.Damage(from, damage, 100, 0, 0, 0, 0);

                                // A dart imbeds itself in your flesh!
                                from.LocalOverheadMessage(Network.MessageType.Regular, 0x62, 502998);
                            }

                            Effects.PlaySound(loc, facet, 0x223);

                            break;
                        }
                    case TrapType.PoisonTrap:
                        {
                            SendMessageTo(from, 502999, 0x3B2); // You set off a trap!

                            if (from.InRange(loc, 3))
                            {
                                Poison poison;

                                if (m_TrapLevel > 0)
                                {
                                    poison = Poison.GetPoison(Math.Max(0, Math.Min(4, m_TrapLevel - 1)));
                                }
                                else
                                {
                                    AOS.Damage(from, m_TrapPower, 0, 0, 0, 100, 0);
                                    poison = Poison.Greater;
                                }

                                from.ApplyPoison(from, poison);

                                // You are enveloped in a noxious green cloud!
                                from.LocalOverheadMessage(Network.MessageType.Regular, 0x44, 503004);
                            }

                            Effects.SendLocationEffect(loc, facet, 0x113A, 10, 20);
                            Effects.PlaySound(loc, facet, 0x231);

                            break;
                        }
                }

                m_TrapType = TrapType.None;
                m_TrapPower = 0;
                m_TrapLevel = 0;
                return true;
            }

            return false;
        }

        public virtual void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
            Effects.PlaySound(Location, Map, 0x1F5);

            if (TrapOnOpen)
            {
                ExecuteTrap(from);
            }
        }

        public override void Open(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player || !TrapOnOpen || !ExecuteTrap(from))
                base.Open(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write(m_TrapLevel);

            writer.Write(m_TrapPower);
            writer.Write((int)m_TrapType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    {
                        m_TrapLevel = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_TrapPower = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_TrapType = (TrapType)reader.ReadInt();
                        break;
                    }
            }
        }

        private void SendMessageTo(Mobile to, int number, int hue)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new Network.MessageLocalized(Serial, ItemID, Network.MessageType.Regular, hue, 3, number, "", ""));
        }

        private void SendMessageTo(Mobile to, string text, int hue)
        {
            if (Deleted || !to.CanSee(this))
                return;

            to.Send(new Network.UnicodeMessage(Serial, ItemID, Network.MessageType.Regular, hue, 3, "ENU", "", text));
        }
    }
}
