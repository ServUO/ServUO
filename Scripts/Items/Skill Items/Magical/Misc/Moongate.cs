using System;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.Items
{
    [DispellableFieldAttribute]
    public class Moongate : Item
    {
        private Point3D m_Target;
        private Map m_TargetMap;
        private bool m_bDispellable;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Target
        {
            get
            {
                return this.m_Target;
            }
            set
            {
                this.m_Target = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map TargetMap
        {
            get
            {
                return this.m_TargetMap;
            }
            set
            {
                this.m_TargetMap = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Dispellable
        {
            get
            {
                return this.m_bDispellable;
            }
            set
            {
                this.m_bDispellable = value;
            }
        }

        public virtual bool ShowFeluccaWarning
        {
            get
            {
                return false;
            }
        }

        [Constructable]
        public Moongate()
            : this(Point3D.Zero, null)
        {
            this.m_bDispellable = true;
        }

        [Constructable]
        public Moongate(bool bDispellable)
            : this(Point3D.Zero, null)
        {
            this.m_bDispellable = bDispellable;
        }

        [Constructable]
        public Moongate(Point3D target, Map targetMap)
            : base(0xF6C)
        {
            this.Movable = false;
            this.Light = LightType.Circle300;

            this.m_Target = target;
            this.m_TargetMap = targetMap;
        }

        public Moongate(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.Player)
                return;

            if (from.InRange(this.GetWorldLocation(), 1))
                this.CheckGate(from, 1);
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Player)
                this.CheckGate(m, 0);

            return true;
        }

        public virtual void CheckGate(Mobile m, int range)
        {
            #region Mondain's Legacy
            if (m.Hidden && m.IsPlayer() && Core.ML)
                m.RevealingAction();
            #endregion

            new DelayTimer(m, this, range).Start();
        }

        public virtual void OnGateUsed(Mobile m)
        {
        }

        public virtual void UseGate(Mobile m)
        {
            ClientFlags flags = m.NetState == null ? ClientFlags.None : m.NetState.Flags;

            if (Factions.Sigil.ExistsOn(m))
            {
                m.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (this.m_TargetMap == Map.Felucca && m is PlayerMobile && ((PlayerMobile)m).Young)
            {
                m.SendLocalizedMessage(1049543); // You decide against traveling to Felucca while you are still young.
            }
            else if ((m.Kills >= 5 && this.m_TargetMap != Map.Felucca) || (this.m_TargetMap == Map.Tokuno && (flags & ClientFlags.Tokuno) == 0) || (this.m_TargetMap == Map.Malas && (flags & ClientFlags.Malas) == 0) || (this.m_TargetMap == Map.Ilshenar && (flags & ClientFlags.Ilshenar) == 0))
            {
                m.SendLocalizedMessage(1019004); // You are not allowed to travel there.
            }
            else if (m.Spell != null)
            {
                m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
            }
            else if (m.Holding != null)
            {
                m.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
            }
            else if (this.m_TargetMap != null && this.m_TargetMap != Map.Internal)
            {
                BaseCreature.TeleportPets(m, this.m_Target, this.m_TargetMap);

                m.MoveToWorld(this.m_Target, this.m_TargetMap);

                if (m.IsPlayer() || !m.Hidden)
                    m.PlaySound(0x1FE);

                this.OnGateUsed(m);
            }
            else
            {
                m.SendMessage("This moongate does not seem to go anywhere.");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(this.m_Target);
            writer.Write(this.m_TargetMap);
			
            // Version 1
            writer.Write(this.m_bDispellable);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Target = reader.ReadPoint3D();
            this.m_TargetMap = reader.ReadMap();

            if (version >= 1)
                this.m_bDispellable = reader.ReadBool();
        }

        public virtual bool ValidateUse(Mobile from, bool message)
        {
            if (from.Deleted || this.Deleted)
                return false;

            if (from.Map != this.Map || !from.InRange(this, 1))
            {
                if (message)
                    from.SendLocalizedMessage(500446); // That is too far away.

                return false;
            }

            return true;
        }

        public virtual void BeginConfirmation(Mobile from)
        {
            if (IsInTown(from.Location, from.Map) && !IsInTown(this.m_Target, this.m_TargetMap) || (from.Map != Map.Felucca && this.TargetMap == Map.Felucca && this.ShowFeluccaWarning))
            {
                if (from.IsPlayer() || !from.Hidden)
                    from.Send(new PlaySound(0x20E, from.Location));
                from.CloseGump(typeof(MoongateConfirmGump));
                from.SendGump(new MoongateConfirmGump(from, this));
            }
            else
            {
                this.EndConfirmation(from);
            }
        }

        public virtual void EndConfirmation(Mobile from)
        {
            if (!this.ValidateUse(from, true))
                return;

            this.UseGate(from);
        }

        public virtual void DelayCallback(Mobile from, int range)
        {
            if (!this.ValidateUse(from, false) || !from.InRange(this, range))
                return;

            if (this.m_TargetMap != null)
                this.BeginConfirmation(from);
            else
                from.SendMessage("This moongate does not seem to go anywhere.");
        }

        public static bool IsInTown(Point3D p, Map map)
        {
            if (map == null)
                return false;

            GuardedRegion reg = (GuardedRegion)Region.Find(p, map).GetRegion(typeof(GuardedRegion));

            return (reg != null && !reg.IsDisabled());
        }

        private class DelayTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly Moongate m_Gate;
            private readonly int m_Range;

            public DelayTimer(Mobile from, Moongate gate, int range)
                : base(TimeSpan.FromSeconds(1.0))
            {
                this.m_From = from;
                this.m_Gate = gate;
                this.m_Range = range;
            }

            protected override void OnTick()
            {
                this.m_Gate.DelayCallback(this.m_From, this.m_Range);
            }
        }
    }

    public class ConfirmationMoongate : Moongate
    {
        private int m_GumpWidth;
        private int m_GumpHeight;

        private int m_TitleColor;
        private int m_MessageColor;

        private int m_TitleNumber;
        private int m_MessageNumber;

        private string m_MessageString;

        [CommandProperty(AccessLevel.GameMaster)]
        public int GumpWidth
        {
            get
            {
                return this.m_GumpWidth;
            }
            set
            {
                this.m_GumpWidth = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int GumpHeight
        {
            get
            {
                return this.m_GumpHeight;
            }
            set
            {
                this.m_GumpHeight = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TitleColor
        {
            get
            {
                return this.m_TitleColor;
            }
            set
            {
                this.m_TitleColor = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MessageColor
        {
            get
            {
                return this.m_MessageColor;
            }
            set
            {
                this.m_MessageColor = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TitleNumber
        {
            get
            {
                return this.m_TitleNumber;
            }
            set
            {
                this.m_TitleNumber = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MessageNumber
        {
            get
            {
                return this.m_MessageNumber;
            }
            set
            {
                this.m_MessageNumber = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string MessageString
        {
            get
            {
                return this.m_MessageString;
            }
            set
            {
                this.m_MessageString = value;
            }
        }

        [Constructable]
        public ConfirmationMoongate()
            : this(Point3D.Zero, null)
        {
        }

        [Constructable]
        public ConfirmationMoongate(Point3D target, Map targetMap)
            : base(target, targetMap)
        {
        }

        public ConfirmationMoongate(Serial serial)
            : base(serial)
        {
        }

        public virtual void Warning_Callback(Mobile from, bool okay, object state)
        {
            if (okay)
                this.EndConfirmation(from);
        }

        public override void BeginConfirmation(Mobile from)
        {
            if (this.m_GumpWidth > 0 && this.m_GumpHeight > 0 && this.m_TitleNumber > 0 && (this.m_MessageNumber > 0 || this.m_MessageString != null))
            {
                from.CloseGump(typeof(WarningGump));
                from.SendGump(new WarningGump(this.m_TitleNumber, this.m_TitleColor, this.m_MessageString == null ? (object)this.m_MessageNumber : (object)this.m_MessageString, this.m_MessageColor, this.m_GumpWidth, this.m_GumpHeight, new WarningGumpCallback(Warning_Callback), from));
            }
            else
            {
                base.BeginConfirmation(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.WriteEncodedInt(this.m_GumpWidth);
            writer.WriteEncodedInt(this.m_GumpHeight);

            writer.WriteEncodedInt(this.m_TitleColor);
            writer.WriteEncodedInt(this.m_MessageColor);

            writer.WriteEncodedInt(this.m_TitleNumber);
            writer.WriteEncodedInt(this.m_MessageNumber);

            writer.Write(this.m_MessageString);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_GumpWidth = reader.ReadEncodedInt();
                        this.m_GumpHeight = reader.ReadEncodedInt();

                        this.m_TitleColor = reader.ReadEncodedInt();
                        this.m_MessageColor = reader.ReadEncodedInt();

                        this.m_TitleNumber = reader.ReadEncodedInt();
                        this.m_MessageNumber = reader.ReadEncodedInt();

                        this.m_MessageString = reader.ReadString();

                        break;
                    }
            }
        }
    }

    public class MoongateConfirmGump : Gump
    {
        private readonly Mobile m_From;
        private readonly Moongate m_Gate;

        public MoongateConfirmGump(Mobile from, Moongate gate)
            : base(Core.AOS ? 110 : 20, Core.AOS ? 100 : 30)
        {
            this.m_From = from;
            this.m_Gate = gate;

            if (Core.AOS)
            {
                this.Closable = false;

                this.AddPage(0);

                this.AddBackground(0, 0, 420, 280, 5054);

                this.AddImageTiled(10, 10, 400, 20, 2624);
                this.AddAlphaRegion(10, 10, 400, 20);

                this.AddHtmlLocalized(10, 10, 400, 20, 1062051, 30720, false, false); // Gate Warning

                this.AddImageTiled(10, 40, 400, 200, 2624);
                this.AddAlphaRegion(10, 40, 400, 200);

                if (from.Map != Map.Felucca && gate.TargetMap == Map.Felucca && gate.ShowFeluccaWarning)
                    this.AddHtmlLocalized(10, 40, 400, 200, 1062050, 32512, false, true); // This Gate goes to Felucca... Continue to enter the gate, Cancel to stay here
                else
                    this.AddHtmlLocalized(10, 40, 400, 200, 1062049, 32512, false, true); // Dost thou wish to step into the moongate? Continue to enter the gate, Cancel to stay here

                this.AddImageTiled(10, 250, 400, 20, 2624);
                this.AddAlphaRegion(10, 250, 400, 20);

                this.AddButton(10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(40, 250, 170, 20, 1011036, 32767, false, false); // OKAY

                this.AddButton(210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(240, 250, 170, 20, 1011012, 32767, false, false); // CANCEL
            }
            else
            {
                this.AddPage(0);

                this.AddBackground(0, 0, 420, 400, 5054);
                this.AddBackground(10, 10, 400, 380, 3000);

                this.AddHtml(20, 40, 380, 60, @"Dost thou wish to step into the moongate? Continue to enter the gate, Cancel to stay here", false, false);

                this.AddHtmlLocalized(55, 110, 290, 20, 1011012, false, false); // CANCEL
                this.AddButton(20, 110, 4005, 4007, 0, GumpButtonType.Reply, 0);

                this.AddHtmlLocalized(55, 140, 290, 40, 1011011, false, false); // CONTINUE
                this.AddButton(20, 140, 4005, 4007, 1, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1)
                this.m_Gate.EndConfirmation(this.m_From);
        }
    }
}