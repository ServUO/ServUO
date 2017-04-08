using System;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Commands;
using Server.Network;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Exodus;
using Server.Mobiles;
using System.Linq;

namespace Server.Items
{
    public class ExodusTomeAltar : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1153602; } } // Exodus Summoning Tome 
        public static ExodusTomeAltar Altar { get; set; }
        public TimeSpan DelayExit { get { return TimeSpan.FromMinutes(10); } }
        private Point3D m_TeleportDest = new Point3D(764, 640, 0);
        public override int Lifespan { get { return 420; } }
        public override bool UseSeconds { get { return false; } }
        private List<RitualArray> m_Rituals;
        private Mobile m_Owner;
        private Item m_ExodusAlterAddon;

        public List<RitualArray> Rituals { get { return m_Rituals; } }
        public Mobile Owner
        {
            get { return this.m_Owner; }
            set { this.m_Owner = value; }
        }

        [Constructable]
        public ExodusTomeAltar(Mobile from) 
            : base(0x1C11)
        {
            this.Hue = 1943;
            this.Movable = false;
            this.LootType = LootType.Regular;
            this.Weight = 0.0;

            this.m_Rituals = new List<RitualArray>();
            this.m_ExodusAlterAddon = new ExodusAlterAddon();
            this.m_ExodusAlterAddon.Movable = false;
        }        

        public ExodusTomeAltar(Serial serial) : base(serial)
        {
        }

        private class BeginTheRitual : ContextMenuEntry
        {
            private Mobile m_Mobile;
            private ExodusTomeAltar m_altar;

            public BeginTheRitual(ExodusTomeAltar altar, Mobile from) : base(1153608, 2) // Begin the Ritual
            {
                m_Mobile = from;
                m_altar = altar;

                if (altar.Owner != from)
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                if (m_altar.Owner == m_Mobile)
                {
                    m_altar.SendConfirmationsExodus(m_Mobile);
                }
                else
                {
                    m_Mobile.SendLocalizedMessage(1153610); // Only the altar owner can commence with the ritual. 
                }
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new BeginTheRitual(this, from));
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(AltarGump)))
            {
                from.SendGump(new AltarGump(from));
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return false;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_ExodusAlterAddon != null)
                this.m_ExodusAlterAddon.Delete();

            if (Altar != null)
                Altar = null;
        }

        public override void OnMapChange()
        {
            if (this.Deleted)
                return;

            if (this.m_ExodusAlterAddon != null)
                this.m_ExodusAlterAddon.Map = this.Map;
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (this.Deleted)
                return;

            if (this.m_ExodusAlterAddon != null)
                this.m_ExodusAlterAddon.Location = new Point3D(this.X - 1, this.Y - 1, this.Z - 18);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((Item)m_ExodusAlterAddon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_ExodusAlterAddon = reader.ReadItem();
        }

        public bool CheckParty(Mobile from, Mobile m)
        {
            Party party = Party.Get(from);

            if (party != null)
            {
                foreach (PartyMemberInfo info in party.Members)
                {
                    if (info.Mobile == m)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual void SendConfirmationsExodus(Mobile from)
        {
            Party party = Party.Get(from);

            if (party != null)
            {
                int MemberRange = party.Members.Where(x => !from.InRange(x.Mobile, 5)).Count();

                if (MemberRange != 0)
                {
                    from.SendLocalizedMessage(1153611); // One or more members of your party are not close enough to you to perform the ritual.
                    return;
                }

                RobeofRite robe;

                foreach (PartyMemberInfo info in party.Members)
                {
                    robe = info.Mobile.FindItemOnLayer(Layer.OuterTorso) as RobeofRite;

                    if (!m_Rituals.Where(z => z.RitualMobile == info.Mobile && z.Ritual1 && z.Ritual2).Any() || robe == null)
                    {
                        from.SendLocalizedMessage(1153609, info.Mobile.Name); // ~1_PLAYER~ has not fulfilled all the requirements of the Ritual! You cannot commence until they do.
                        return;
                    }
                }

                foreach (PartyMemberInfo info in party.Members)
                {
                    this.SendBattleground(info.Mobile);
                }
            }
            else
            {
                from.SendLocalizedMessage(1153596); // You must join a party with the players you wish to perform the ritual with. 
            }
        }

        public virtual void SendBattleground(Mobile from)
        {
            if (VerLorRegController.Active && VerLorRegController.Mobile != null && ExodusSummoningAlter.CheckExodus())
            {
                // teleport party member
                from.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                from.PlaySound(0x1FE);
                from.MoveToWorld(this.m_TeleportDest, Map.Ilshenar);
                BaseCreature.TeleportPets(from, m_TeleportDest, Map.Ilshenar);

                // Robe of Rite Delete
                RobeofRite robe = from.FindItemOnLayer(Layer.OuterTorso) as RobeofRite;

                if (robe != null)
                {
                    robe.Delete();
                }

                // Altar Delete
                Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerCallback(Delete));
            }
            else
            {
                from.SendLocalizedMessage(1075213); // The master of this realm has already been summoned and is engaged in combat.  Your opportunity will come after he has squashed the current batch of intruders!
            }
        }
    }

    public class RitualArray
    {
        public Mobile RitualMobile { get; set; }
        public bool Ritual1 { get; set; }
        public bool Ritual2 { get; set; }
    }

    public class AltarGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register("TomeAltarGump", AccessLevel.Administrator, new CommandEventHandler(TomeAltarGump_OnCommand));
        }

        [Usage("TomeAltarGump")]
        public static void TomeAltarGump_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!from.HasGump(typeof(AltarGump)))
            {
                from.SendGump(new AltarGump(from));
            }
        }

        public AltarGump(Mobile owner) : base(100, 100)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;

            AddPage(0);
            AddBackground(0, 0, 447, 195, 5120);
            AddHtmlLocalized(17, 14, 412, 161, 1153607, 0x7FFF, false, false); // Contained within this Tome is the ritual by which Lord Exodus may once again be called upon Britannia in his physical form, summoned from deep within the Void.  Only when the Summoning Rite has been rejoined with the tome and only when the Robe of Rite covers the caster can the Sacrificial Dagger be used to seal thy fate.  Stab into this book the dagger and declare thy quest for Valor as thou stand to defend Britannia from this evil, or sacrifice thy blood unto this altar to declare thy quest for greed and wealth...only thou can judge thyself...
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0: {
                        //Cancel
                        break;
                    }
            }
        }
    }
}