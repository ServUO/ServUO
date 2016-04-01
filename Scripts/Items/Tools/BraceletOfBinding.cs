using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using Server.Spells;
using Server.Targeting;

namespace Server.Items
{
    public class BraceletOfBinding : BaseBracelet, TranslocationItem
    {
        private int m_Charges;
        private int m_Recharges;
        private string m_Inscription;
        private BraceletOfBinding m_Bound;
        private TransportTimer m_Timer;
        [Constructable]
        public BraceletOfBinding()
            : base(0x1086)
        {
            this.Hue = 0x489;
            this.Weight = 1.0;

            this.m_Inscription = "";
        }

        public BraceletOfBinding(Serial serial)
            : base(serial)
        {
        }

        private delegate void BraceletCallback(Mobile from);
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return this.m_Charges;
            }
            set
            {
                if (value > this.MaxCharges)
                    this.m_Charges = this.MaxCharges;
                else if (value < 0)
                    this.m_Charges = 0;
                else
                    this.m_Charges = value;

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Recharges
        {
            get
            {
                return this.m_Recharges;
            }
            set
            {
                if (value > this.MaxRecharges)
                    this.m_Recharges = this.MaxRecharges;
                else if (value < 0)
                    this.m_Recharges = 0;
                else
                    this.m_Recharges = value;

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get
            {
                return 20;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRecharges
        {
            get
            {
                return 255;
            }
        }
        public string TranslocationItemName
        {
            get
            {
                return "bracelet of binding";
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Inscription
        {
            get
            {
                return this.m_Inscription;
            }
            set
            {
                this.m_Inscription = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public BraceletOfBinding Bound
        {
            get
            {
                if (this.m_Bound != null && this.m_Bound.Deleted)
                    this.m_Bound = null;

                return this.m_Bound;
            }
            set
            {
                this.m_Bound = value;
            }
        }
        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1054000, this.m_Charges.ToString() + (this.m_Inscription.Length == 0 ? "\t " : " :\t" + this.m_Inscription)); // a bracelet of binding : ~1_val~ ~2_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, 1054000, this.m_Charges.ToString() + (this.m_Inscription.Length == 0 ? "\t " : " :\t" + this.m_Inscription)); // a bracelet of binding : ~1_val~ ~2_val~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive && this.IsChildOf(from))
            {
                BraceletOfBinding bound = this.Bound;

                list.Add(new BraceletEntry(new BraceletCallback(Activate), 6170, bound != null));
                list.Add(new BraceletEntry(new BraceletCallback(Search), 6171, bound != null));
                list.Add(new BraceletEntry(new BraceletCallback(Bind), bound == null ? 6173 : 6174, true));
                list.Add(new BraceletEntry(new BraceletCallback(Inscribe), 6175, true));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            BraceletOfBinding bound = this.Bound;

            if (this.Bound == null)
            {
                this.Bind(from);
            }
            else
            {
                this.Activate(from);
            }
        }

        public void Activate(Mobile from)
        {
            BraceletOfBinding bound = this.Bound;

            if (this.Deleted || bound == null)
                return;

            if (!this.IsChildOf(from))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
            }
            else if (this.m_Timer != null)
            {
                from.SendLocalizedMessage(1054013); // The bracelet is already attempting contact. You decide to wait a moment.
            }
            else
            {
                from.PlaySound(0xF9);
                from.LocalOverheadMessage(MessageType.Regular, 0x5D, true, "* You concentrate on the bracelet to summon its power *");

                from.Frozen = true;

                this.m_Timer = new TransportTimer(this, from);
                this.m_Timer.Start();
            }
        }

        public void Search(Mobile from)
        {
            BraceletOfBinding bound = this.Bound;

            if (this.Deleted || bound == null)
                return;

            if (!this.IsChildOf(from))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
            }
            else
            {
                this.CheckUse(from, true);
            }
        }

        public void Bind(Mobile from)
        {
            if (this.Deleted)
                return;

            if (!this.IsChildOf(from))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
            }
            else
            {
                from.SendLocalizedMessage(1054001); // Target the bracelet of binding you wish to bind this bracelet to.
                from.Target = new BindTarget(this);
            }
        }

        public void Inscribe(Mobile from)
        {
            if (this.Deleted)
                return;

            if (!this.IsChildOf(from))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
            }
            else
            {
                from.SendLocalizedMessage(1054009); // Enter the text to inscribe upon the bracelet :
                from.Prompt = new InscribePrompt(this);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)1); // version

            writer.WriteEncodedInt((int)this.m_Recharges);

            writer.WriteEncodedInt((int)this.m_Charges);
            writer.Write((string)this.m_Inscription);
            writer.Write((Item)this.Bound);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Recharges = reader.ReadEncodedInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Charges = Math.Min(reader.ReadEncodedInt(), this.MaxCharges);
                        this.m_Inscription = reader.ReadString();
                        this.Bound = (BraceletOfBinding)reader.ReadItem();
                        break;
                    }
            }
        }

        private bool CheckUse(Mobile from, bool successMessage)
        {
            BraceletOfBinding bound = this.Bound;

            if (bound == null)
                return false;

            Mobile boundRoot = bound.RootParent as Mobile;

            if (this.Charges == 0)
            {
                from.SendLocalizedMessage(1054005); // The bracelet glows black. It must be charged before it can be used again.
                return false;
            }
            else if (from.FindItemOnLayer(Layer.Bracelet) != this)
            {
                from.SendLocalizedMessage(1054004); // You must equip the bracelet in order to use its power.
                return false;
            }
            else if (boundRoot == null || boundRoot.NetState == null || boundRoot.FindItemOnLayer(Layer.Bracelet) != bound)
            {
                from.SendLocalizedMessage(1054006); // The bracelet emits a red glow. The bracelet's twin is not available for transport.
                return false;
            }
            else if (!Core.AOS && from.Map != boundRoot.Map)
            {
                from.SendLocalizedMessage(1054014); // The bracelet glows black. The bracelet's target is on another facet.
                return false;
            }
            else if (Factions.Sigil.ExistsOn(from))
            {
                from.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }
            else if (!SpellHelper.CheckTravel(from, TravelCheckType.RecallFrom))
            {
                return false;
            }
            else if (!SpellHelper.CheckTravel(from, boundRoot.Map, boundRoot.Location, TravelCheckType.RecallTo))
            {
                return false;
            }
            else if (boundRoot.Map == Map.Felucca && from is PlayerMobile && ((PlayerMobile)from).Young)
            {
                from.SendLocalizedMessage(1049543); // You decide against traveling to Felucca while you are still young.
                return false;
            }
            else if (from.Kills >= 5 && boundRoot.Map != Map.Felucca)
            {
                from.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                return false;
            }
            else if (from.Criminal)
            {
                from.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            else if (SpellHelper.CheckCombat(from))
            {
                from.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(from))
            {
                from.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }
            else if (from.Region.IsPartOf(typeof(Server.Regions.Jail)))
            {
                from.SendLocalizedMessage(1114345, "", 0x35); // You'll need a better jailbreak plan than that!
                return false;
            }
            else if (boundRoot.Region.IsPartOf(typeof(Server.Regions.Jail)))
            {
                from.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                return false;
            }
            else
            {
                if (successMessage)
                    from.SendLocalizedMessage(1054015); // The bracelet's twin is available for transport.

                return true;
            }
        }

        private class BraceletEntry : ContextMenuEntry
        {
            private readonly BraceletCallback m_Callback;
            public BraceletEntry(BraceletCallback callback, int number, bool enabled)
                : base(number)
            {
                this.m_Callback = callback;

                if (!enabled)
                    this.Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (from.CheckAlive())
                    this.m_Callback(from);
            }
        }

        private class TransportTimer : Timer
        {
            private readonly BraceletOfBinding m_Bracelet;
            private readonly Mobile m_From;
            public TransportTimer(BraceletOfBinding bracelet, Mobile from)
                : base(TimeSpan.FromSeconds(2.0))
            {
                this.m_Bracelet = bracelet;
                this.m_From = from;
            }

            protected override void OnTick()
            {
                this.m_Bracelet.m_Timer = null;
                this.m_From.Frozen = false;

                if (this.m_Bracelet.Deleted || this.m_From.Deleted)
                    return;

                if (this.m_Bracelet.CheckUse(this.m_From, false))
                {
                    Mobile boundRoot = this.m_Bracelet.Bound.RootParent as Mobile;

                    if (boundRoot != null)
                    {
                        this.m_Bracelet.Charges--;

                        BaseCreature.TeleportPets(this.m_From, boundRoot.Location, boundRoot.Map, true);

                        this.m_From.PlaySound(0x1FC);
                        this.m_From.MoveToWorld(boundRoot.Location, boundRoot.Map);
                        this.m_From.PlaySound(0x1FC);
                    }
                }
            }
        }

        private class BindTarget : Target
        {
            private readonly BraceletOfBinding m_Bracelet;
            public BindTarget(BraceletOfBinding bracelet)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Bracelet = bracelet;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Bracelet.Deleted)
                    return;

                if (!this.m_Bracelet.IsChildOf(from))
                {
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                }
                else if (targeted is BraceletOfBinding)
                {
                    BraceletOfBinding bindBracelet = (BraceletOfBinding)targeted;

                    if (bindBracelet == this.m_Bracelet)
                    {
                        from.SendLocalizedMessage(1054012); // You cannot bind a bracelet of binding to itself!
                    }
                    else if (!bindBracelet.IsChildOf(from))
                    {
                        from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1054003); // You bind the bracelet to its counterpart. The bracelets glow with power.
                        from.PlaySound(0x1FA);

                        this.m_Bracelet.Bound = bindBracelet;
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1054002); // You can only bind this bracelet to another bracelet of binding!
                }
            }
        }

        private class InscribePrompt : Prompt
        {
            private readonly BraceletOfBinding m_Bracelet;
            public InscribePrompt(BraceletOfBinding bracelet)
            {
                this.m_Bracelet = bracelet;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (this.m_Bracelet.Deleted)
                    return;

                if (!this.m_Bracelet.IsChildOf(from))
                {
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                }
                else
                {
                    from.SendLocalizedMessage(1054011); // You mark the bracelet with your inscription.
                    this.m_Bracelet.Inscription = text;
                }
            }

            public override void OnCancel(Mobile from)
            {
                from.SendLocalizedMessage(1054010); // You decide not to inscribe the bracelet at this time.
            }
        }
    }
}