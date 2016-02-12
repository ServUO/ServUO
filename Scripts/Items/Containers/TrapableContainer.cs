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
        private bool m_TrapEnabled;

        public TrapableContainer(int itemID)
            : base(itemID)
        {
        }

        public TrapableContainer(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TrapType TrapType { get { return m_TrapType; } set { m_TrapType = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrapPower { get { return m_TrapPower; } set { m_TrapPower = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrapLevel { get { return m_TrapLevel; } set { m_TrapLevel = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TrapEnabled { get { return m_TrapEnabled; } set { m_TrapEnabled = value; } }

        /// <summary>
        /// Checks whether the given mobile will activate the container's trap or not.
        /// </summary>
        /// <param name="from">The mobile who triggered the trap.</param>
        /// <returns>true if the trap will be executed, false otherwise.</returns>
        public virtual bool CheckTrap(Mobile from)
        {
            if (!m_TrapEnabled || m_TrapType == TrapType.None)
            {
                return false;
            }

            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                SendMessageTo(from, "That is trapped, but you open it with your godly powers.", 0x3B2);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Executes the trap of this container, if any.
        /// </summary>
        /// <param name="from">The mobile who triggered the trap.</param>
        public virtual void ExecuteTrap(Mobile from)
        {
            Point3D loc = this.GetWorldLocation();
            Map facet = this.Map;

            switch (this.m_TrapType)
            {
                case TrapType.ExplosionTrap:
                    {
                        this.SendMessageTo(from, 502999, 0x3B2); // You set off a trap!

                        if (from.InRange(loc, 3))
                        {
                            int damage;

                            if (this.m_TrapLevel > 0)
                            {
                                damage = Utility.RandomMinMax(10, 30) * this.m_TrapLevel;
                            }
                            else
                            {
                                damage = this.m_TrapPower;
                            }

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
                        {
                            from.Damage(this.m_TrapPower);
                        }
                        //AOS.Damage( from, m_TrapPower, 0, 100, 0, 0, 0 );

                        Effects.PlaySound(loc, this.Map, 0x307);

                        Effects.SendLocationEffect(new Point3D(loc.X - 1, loc.Y, loc.Z), this.Map, 0x36BD, 15);
                        Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y, loc.Z), this.Map, 0x36BD, 15);

                        Effects.SendLocationEffect(new Point3D(loc.X, loc.Y - 1, loc.Z), this.Map, 0x36BD, 15);
                        Effects.SendLocationEffect(new Point3D(loc.X, loc.Y + 1, loc.Z), this.Map, 0x36BD, 15);

                        Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y + 1, loc.Z + 11), this.Map, 0x36BD, 15);

                        break;
                    }
                case TrapType.DartTrap:
                    {
                        this.SendMessageTo(from, 502999, 0x3B2); // You set off a trap!

                        if (from.InRange(loc, 3))
                        {
                            int damage;

                            if (this.m_TrapLevel > 0)
                            {
                                damage = Utility.RandomMinMax(5, 15) * this.m_TrapLevel;
                            }
                            else
                            {
                                damage = this.m_TrapPower;
                            }

                            AOS.Damage(from, damage, 100, 0, 0, 0, 0);

                            // A dart imbeds itself in your flesh!
                            from.LocalOverheadMessage(Network.MessageType.Regular, 0x62, 502998);
                        }

                        Effects.PlaySound(loc, facet, 0x223);

                        break;
                    }
                case TrapType.PoisonTrap:
                    {
                        this.SendMessageTo(from, 502999, 0x3B2); // You set off a trap!

                        if (from.InRange(loc, 3))
                        {
                            Poison poison;

                            if (this.m_TrapLevel > 0)
                            {
                                poison = Poison.GetPoison(Math.Max(0, Math.Min(4, this.m_TrapLevel - 1)));
                            }
                            else
                            {
                                AOS.Damage(from, this.m_TrapPower, 0, 0, 0, 100, 0);
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
        }

        public void DisarmTrap(Mobile from)
        {
            SendMessageTo(from, 502999, 0x3B2); // You set off a trap!

            this.m_TrapType = TrapType.None;
            this.m_TrapPower = 0;
            this.m_TrapLevel = 0;
            TrapEnabled = false;
        }

        public virtual void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
            Effects.PlaySound(Location, Map, 0x1F5);

            if (CheckTrap(from))
            {
                ExecuteTrap(from);
                DisarmTrap(from);
            }

            DisplayTo(from);
        }

        public override void Open(Mobile from)
        {
            if (CheckTrap(from))
            {
                ExecuteTrap(from);
                DisarmTrap(from);
            }

            base.Open(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            writer.Write((int)this.m_TrapLevel);

            writer.Write((int)this.m_TrapPower);
            writer.Write((int)this.m_TrapType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                    {
                        this.m_TrapLevel = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_TrapPower = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_TrapType = (TrapType)reader.ReadInt();
                        break;
                    }
            }
        }

        private void SendMessageTo(Mobile to, int number, int hue)
        {
            if (this.Deleted || !to.CanSee(this))
                return;

            to.Send(new Network.MessageLocalized(this.Serial, this.ItemID, Network.MessageType.Regular, hue, 3, number, "", ""));
        }

        private void SendMessageTo(Mobile to, string text, int hue)
        {
            if (this.Deleted || !to.CanSee(this))
                return;

            to.Send(new Network.UnicodeMessage(this.Serial, this.ItemID, Network.MessageType.Regular, hue, 3, "ENU", "", text));
        }
    }
}