using System;
using System.Collections.Generic;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class CrystalRechargeInfo
    {
        public static readonly CrystalRechargeInfo[] Table = new CrystalRechargeInfo[]
        {
            new CrystalRechargeInfo(typeof(Citrine), 500),
            new CrystalRechargeInfo(typeof(Amber), 500),
            new CrystalRechargeInfo(typeof(Tourmaline), 750),
            new CrystalRechargeInfo(typeof(Emerald), 1000),
            new CrystalRechargeInfo(typeof(Sapphire), 1000),
            new CrystalRechargeInfo(typeof(Amethyst), 1000),
            new CrystalRechargeInfo(typeof(StarSapphire), 1250),
            new CrystalRechargeInfo(typeof(Diamond), 2000)
        };
        private readonly Type m_Type;
        private readonly int m_Amount;
        private CrystalRechargeInfo(Type type, int amount)
        {
            this.m_Type = type;
            this.m_Amount = amount;
        }

        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        public int Amount
        {
            get
            {
                return this.m_Amount;
            }
        }
        public static CrystalRechargeInfo Get(Type type)
        {
            foreach (CrystalRechargeInfo info in Table)
            {
                if (info.Type == type)
                    return info;
            }

            return null;
        }
    }

    public class BroadcastCrystal : Item
    {
        public static readonly int MaxCharges = 2000;
        private int m_Charges;
        private List<ReceiverCrystal> m_Receivers;
        [Constructable]
        public BroadcastCrystal()
            : this(2000)
        {
        }

        [Constructable]
        public BroadcastCrystal(int charges)
            : base(0x1ED0)
        {
            this.Light = LightType.Circle150;

            this.m_Charges = charges;

            this.m_Receivers = new List<ReceiverCrystal>();
        }

        public BroadcastCrystal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1060740;
            }
        }// communication crystal
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return this.ItemID == 0x1ECD;
            }
            set
            {
                this.ItemID = value ? 0x1ECD : 0x1ED0;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return this.m_Charges;
            }
            set
            {
                this.m_Charges = value;
                this.InvalidateProperties();
            }
        }
        public List<ReceiverCrystal> Receivers
        {
            get
            {
                return this.m_Receivers;
            }
        }
        public override bool HandlesOnSpeech
        {
            get
            {
                return this.Active && this.Receivers.Count > 0 && (this.RootParent == null || this.RootParent is Mobile);
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(this.Active ? 1060742 : 1060743); // active / inactive
            list.Add(1060745); // broadcast
            list.Add(1060741, this.Charges.ToString()); // charges: ~1_val~

            if (this.Receivers.Count > 0)
                list.Add(1060746, this.Receivers.Count.ToString()); // links: ~1_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            this.LabelTo(from, this.Active ? 1060742 : 1060743); // active / inactive
            this.LabelTo(from, 1060745); // broadcast
            this.LabelTo(from, 1060741, this.Charges.ToString()); // charges: ~1_val~

            if (this.Receivers.Count > 0)
                this.LabelTo(from, 1060746, this.Receivers.Count.ToString()); // links: ~1_val~
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!this.Active || this.Receivers.Count == 0 || (this.RootParent != null && !(this.RootParent is Mobile)))
                return;

            if (e.Type == MessageType.Emote)
                return;

            Mobile from = e.Mobile;
            string speech = e.Speech;

            foreach (ReceiverCrystal receiver in new List<ReceiverCrystal>(this.Receivers))
            {
                if (receiver.Deleted)
                {
                    this.Receivers.Remove(receiver);
                }
                else if (this.Charges > 0)
                {
                    receiver.TransmitMessage(from, speech);
                    this.Charges--;
                }
                else
                {
                    this.Active = false;
                    break;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            from.Target = new InternalTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(this.m_Charges);
            writer.WriteItemList<ReceiverCrystal>(this.m_Receivers);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Charges = reader.ReadEncodedInt();
            this.m_Receivers = reader.ReadStrongItemList<ReceiverCrystal>();
        }

        private class InternalTarget : Target
        {
            private readonly BroadcastCrystal m_Crystal;
            public InternalTarget(BroadcastCrystal crystal)
                : base(2, false, TargetFlags.None)
            {
                this.m_Crystal = crystal;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!this.m_Crystal.IsAccessibleTo(from))
                    return;

                if (from.Map != this.m_Crystal.Map || !from.InRange(this.m_Crystal.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }

                if (targeted == this.m_Crystal)
                {
                    if (this.m_Crystal.Active)
                    {
                        this.m_Crystal.Active = false;
                        from.SendLocalizedMessage(500672); // You turn the crystal off.
                    }
                    else
                    {
                        if (this.m_Crystal.Charges > 0)
                        {
                            this.m_Crystal.Active = true;
                            from.SendLocalizedMessage(500673); // You turn the crystal on.
                        }
                        else
                        {
                            from.SendLocalizedMessage(500676); // This crystal is out of charges.
                        }
                    }
                }
                else if (targeted is ReceiverCrystal)
                {
                    ReceiverCrystal receiver = (ReceiverCrystal)targeted;

                    if (this.m_Crystal.Receivers.Count >= 10)
                    {
                        from.SendLocalizedMessage(1010042); // This broadcast crystal is already linked to 10 receivers.
                    }
                    else if (receiver.Sender == this.m_Crystal)
                    {
                        from.SendLocalizedMessage(500674); // This crystal is already linked with that crystal.
                    }
                    else if (receiver.Sender != null)
                    {
                        from.SendLocalizedMessage(1010043); // That receiver crystal is already linked to another broadcast crystal.
                    }
                    else
                    {
                        receiver.Sender = this.m_Crystal;
                        from.SendLocalizedMessage(500675); // That crystal has been linked to this crystal.
                    }
                }
                else if (targeted == from)
                {
                    foreach (ReceiverCrystal receiver in new List<ReceiverCrystal>(this.m_Crystal.Receivers))
                    {
                        receiver.Sender = null;
                    }

                    from.SendLocalizedMessage(1010046); // You unlink the broadcast crystal from all of its receivers.
                }
                else
                {
                    Item targItem = targeted as Item;

                    if (targItem != null && targItem.VerifyMove(from))
                    {
                        CrystalRechargeInfo info = CrystalRechargeInfo.Get(targItem.GetType());

                        if (info != null)
                        {
                            if (this.m_Crystal.Charges >= MaxCharges)
                            {
                                from.SendLocalizedMessage(500678); // This crystal is already fully charged.
                            }
                            else
                            {
                                targItem.Consume();

                                if (this.m_Crystal.Charges + info.Amount >= MaxCharges)
                                {
                                    this.m_Crystal.Charges = MaxCharges;
                                    from.SendLocalizedMessage(500679); // You completely recharge the crystal.
                                }
                                else
                                {
                                    this.m_Crystal.Charges += info.Amount;
                                    from.SendLocalizedMessage(500680); // You recharge the crystal.
                                }
                            }

                            return;
                        }
                    }

                    from.SendLocalizedMessage(500681); // You cannot use this crystal on that.
                }
            }
        }
    }

    public class ReceiverCrystal : Item
    {
        private BroadcastCrystal m_Sender;
        [Constructable]
        public ReceiverCrystal()
            : base(0x1ED0)
        {
            this.Light = LightType.Circle150;
        }

        public ReceiverCrystal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1060740;
            }
        }// communication crystal
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return this.ItemID == 0x1ED1;
            }
            set
            {
                this.ItemID = value ? 0x1ED1 : 0x1ED0;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BroadcastCrystal Sender
        {
            get
            {
                return this.m_Sender;
            }
            set
            {
                if (this.m_Sender != null)
                {
                    this.m_Sender.Receivers.Remove(this);
                    this.m_Sender.InvalidateProperties();
                }

                this.m_Sender = value;

                if (value != null)
                {
                    value.Receivers.Add(this);
                    value.InvalidateProperties();
                }
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(this.Active ? 1060742 : 1060743); // active / inactive
            list.Add(1060744); // receiver
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            this.LabelTo(from, this.Active ? 1060742 : 1060743); // active / inactive
            this.LabelTo(from, 1060744); // receiver
        }

        public void TransmitMessage(Mobile from, string message)
        {
            if (!this.Active)
                return;

            string text = String.Format("{0} says {1}", from.Name, message);

            if (this.RootParent is Mobile)
            {
                ((Mobile)this.RootParent).SendMessage(0x2B2, "Crystal: " + text);
            }
            else if (this.RootParent is Item)
            {
                ((Item)this.RootParent).PublicOverheadMessage(MessageType.Regular, 0x2B2, false, "Crystal: " + text);
            }
            else
            {
                this.PublicOverheadMessage(MessageType.Regular, 0x2B2, false, text);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            from.Target = new InternalTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteItem<BroadcastCrystal>(this.m_Sender);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Sender = reader.ReadItem<BroadcastCrystal>();
        }

        private class InternalTarget : Target
        {
            private readonly ReceiverCrystal m_Crystal;
            public InternalTarget(ReceiverCrystal crystal)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Crystal = crystal;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!this.m_Crystal.IsAccessibleTo(from))
                    return;

                if (from.Map != this.m_Crystal.Map || !from.InRange(this.m_Crystal.GetWorldLocation(), 2))
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                    return;
                }

                if (targeted == this.m_Crystal)
                {
                    if (this.m_Crystal.Active)
                    {
                        this.m_Crystal.Active = false;
                        from.SendLocalizedMessage(500672); // You turn the crystal off.
                    }
                    else
                    {
                        this.m_Crystal.Active = true;
                        from.SendLocalizedMessage(500673); // You turn the crystal on.
                    }
                }
                else if (targeted == from)
                {
                    if (this.m_Crystal.Sender != null)
                    {
                        this.m_Crystal.Sender = null;
                        from.SendLocalizedMessage(1010044); // You unlink the receiver crystal.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1010045); // That receiver crystal is not linked.
                    }
                }
                else
                {
                    Item targItem = targeted as Item;

                    if (targItem != null && targItem.VerifyMove(from))
                    {
                        CrystalRechargeInfo info = CrystalRechargeInfo.Get(targItem.GetType());

                        if (info != null)
                        {
                            from.SendLocalizedMessage(500677); // This crystal cannot be recharged.
                            return;
                        }
                    }

                    from.SendLocalizedMessage(1010045); // That receiver crystal is not linked.
                }
            }
        }
    }
}