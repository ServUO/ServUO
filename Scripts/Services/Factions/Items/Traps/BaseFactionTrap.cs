using System;
using Server.Items;
using Server.Network;

namespace Server.Factions
{
    public enum AllowedPlacing
    {
        Everywhere,

        AnyFactionTown,
        ControlledFactionTown,
        FactionStronghold
    }

    public abstract class BaseFactionTrap : BaseTrap
    {
        private Faction m_Faction;
        private Mobile m_Placer;
        private DateTime m_TimeOfPlacement;
        private Timer m_Concealing;
        public BaseFactionTrap(Faction f, Mobile m, int itemID)
            : base(itemID)
        {
            this.Visible = false;

            this.m_Faction = f;
            this.m_TimeOfPlacement = DateTime.UtcNow;
            this.m_Placer = m;
        }

        public BaseFactionTrap(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
            set
            {
                this.m_Faction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Placer
        {
            get
            {
                return this.m_Placer;
            }
            set
            {
                this.m_Placer = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfPlacement
        {
            get
            {
                return this.m_TimeOfPlacement;
            }
            set
            {
                this.m_TimeOfPlacement = value;
            }
        }
        public virtual int EffectSound
        {
            get
            {
                return 0;
            }
        }
        public virtual int SilverFromDisarm
        {
            get
            {
                return 100;
            }
        }
        public virtual int MessageHue
        {
            get
            {
                return 0;
            }
        }
        public virtual int AttackMessage
        {
            get
            {
                return 0;
            }
        }
        public virtual int DisarmMessage
        {
            get
            {
                return 0;
            }
        }
        public virtual AllowedPlacing AllowedPlacing
        {
            get
            {
                return AllowedPlacing.Everywhere;
            }
        }
        public virtual TimeSpan ConcealPeriod
        {
            get
            {
                return TimeSpan.FromMinutes(1.0);
            }
        }
        public virtual TimeSpan DecayPeriod
        {
            get
            {
                if (Core.AOS)
                    return TimeSpan.FromDays(1.0);

                return TimeSpan.MaxValue; // no decay
            }
        }
        public override void OnTrigger(Mobile from)
        {
            if (!this.IsEnemy(from))
                return;

            this.Conceal();

            this.DoVisibleEffect();
            Effects.PlaySound(this.Location, this.Map, this.EffectSound);
            this.DoAttackEffect(from);

            int silverToAward = (from.Alive ? 20 : 40);

            if (silverToAward > 0 && this.m_Placer != null && this.m_Faction != null)
            {
                PlayerState victimState = PlayerState.Find(from);

                if (victimState != null && victimState.CanGiveSilverTo(this.m_Placer) && victimState.KillPoints > 0)
                {
                    int silverGiven = this.m_Faction.AwardSilver(this.m_Placer, silverToAward);

                    if (silverGiven > 0)
                    {
                        // TODO: Get real message
                        if (from.Alive)
                            this.m_Placer.SendMessage("You have earned {0} silver pieces because {1} fell for your trap.", silverGiven, from.Name);
                        else
                            this.m_Placer.SendLocalizedMessage(1042736, String.Format("{0} silver\t{1}", silverGiven, from.Name)); // You have earned ~1_SILVER_AMOUNT~ pieces for vanquishing ~2_PLAYER_NAME~!
                    }

                    victimState.OnGivenSilverTo(this.m_Placer);
                }
            }

            from.LocalOverheadMessage(MessageType.Regular, this.MessageHue, this.AttackMessage);
        }

        public abstract void DoVisibleEffect();

        public abstract void DoAttackEffect(Mobile m);

        public virtual int IsValidLocation()
        {
            return this.IsValidLocation(this.GetWorldLocation(), this.Map);
        }

        public virtual int IsValidLocation(Point3D p, Map m)
        {
            if (m == null)
                return 502956; // You cannot place a trap on that.

            if (Core.ML)
            {
                foreach (Item item in m.GetItemsInRange(p, 0))
                {
                    if (item is BaseFactionTrap && ((BaseFactionTrap)item).Faction == this.Faction)
                        return 1075263; // There is already a trap belonging to your faction at this location.;
                }
            }

            switch( this.AllowedPlacing )
            {
                case AllowedPlacing.FactionStronghold:
                    {
                        StrongholdRegion region = (StrongholdRegion)Region.Find(p, m).GetRegion(typeof(StrongholdRegion));

                        if (region != null && region.Faction == this.m_Faction)
                            return 0;

                        return 1010355; // This trap can only be placed in your stronghold
                    }
                case AllowedPlacing.AnyFactionTown:
                    {
                        Town town = Town.FromRegion(Region.Find(p, m));

                        if (town != null)
                            return 0;

                        return 1010356; // This trap can only be placed in a faction town
                    }
                case AllowedPlacing.ControlledFactionTown:
                    {
                        Town town = Town.FromRegion(Region.Find(p, m));

                        if (town != null && town.Owner == this.m_Faction)
                            return 0;

                        return 1010357; // This trap can only be placed in a town your faction controls
                    }
            }

            return 0;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (!this.CheckDecay() && this.CheckRange(m.Location, oldLocation, 6))
            {
                if (Faction.Find(m) != null && ((m.Skills[SkillName.DetectHidden].Value - 80.0) / 20.0) > Utility.RandomDouble())
                    this.PrivateOverheadLocalizedMessage(m, 1010154, this.MessageHue, "", ""); // [Faction Trap]
            }
        }

        public void PrivateOverheadLocalizedMessage(Mobile to, int number, int hue, string name, string args)
        {
            if (to == null)
                return;

            NetState ns = to.NetState;

            if (ns != null)
                ns.Send(new MessageLocalized(this.Serial, this.ItemID, MessageType.Regular, hue, 3, number, name, args));
        }

        public virtual bool CheckDecay()
        {
            TimeSpan decayPeriod = this.DecayPeriod;

            if (decayPeriod == TimeSpan.MaxValue)
                return false;

            if ((this.m_TimeOfPlacement + decayPeriod) < DateTime.UtcNow)
            {
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
                return true;
            }

            return false;
        }

        public virtual void BeginConceal()
        {
            if (this.m_Concealing != null)
                this.m_Concealing.Stop();

            this.m_Concealing = Timer.DelayCall(this.ConcealPeriod, new TimerCallback(Conceal));
        }

        public virtual void Conceal()
        {
            if (this.m_Concealing != null)
                this.m_Concealing.Stop();

            this.m_Concealing = null;

            if (!this.Deleted)
                this.Visible = false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Faction.WriteReference(writer, this.m_Faction);
            writer.Write((Mobile)this.m_Placer);
            writer.Write((DateTime)this.m_TimeOfPlacement);

            if (this.Visible)
                this.BeginConceal();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Faction = Faction.ReadReference(reader);
            this.m_Placer = reader.ReadMobile();
            this.m_TimeOfPlacement = reader.ReadDateTime();

            if (this.Visible)
                this.BeginConceal();

            this.CheckDecay();
        }

        public override void OnDelete()
        {
            if (this.m_Faction != null && this.m_Faction.Traps.Contains(this))
                this.m_Faction.Traps.Remove(this);

            base.OnDelete();
        }

        public virtual bool IsEnemy(Mobile mob)
        {
            if (mob.Hidden && mob.IsStaff())
                return false;

            if (!mob.Alive || mob.IsDeadBondedPet)
                return false;

            Faction faction = Faction.Find(mob, true);

            if (faction == null && mob is BaseFactionGuard)
                faction = ((BaseFactionGuard)mob).Faction;

            if (faction == null)
                return false;

            return (faction != this.m_Faction);
        }
    }
}