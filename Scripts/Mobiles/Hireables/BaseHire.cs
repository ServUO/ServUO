using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;

namespace Server.Mobiles
{
    public class BaseHire : BaseCreature
    {
        private int m_Pay = 1;
        private bool m_IsHired;
        private int m_HoldGold = 8;
        private Timer m_PayTimer;

        public BaseHire(AIType AI)
            : base(AI, FightMode.Aggressor, 10, 1, 0.1, 4.0)
        {
        }

        public BaseHire()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 4.0)
        {
        }

        public BaseHire(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);// version

            writer.Write((bool)this.m_IsHired);
            writer.Write((int)this.m_HoldGold);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_IsHired = reader.ReadBool();
            this.m_HoldGold = reader.ReadInt();

            this.m_PayTimer = new PayTimer(this);
            this.m_PayTimer.Start();
        }

        private static readonly Hashtable m_HireTable = new Hashtable();

        public static Hashtable HireTable 
        { 
            get
            {
                return m_HireTable;
            }
        }

        public override bool KeepsItemsOnDeath
        {
            get
            {
                return true;
            }
        }
        private int m_GoldOnDeath = 0;
        public override bool OnBeforeDeath() 
        { 
            // Stop the pay timer if its running 
            if (this.m_PayTimer != null) 
                this.m_PayTimer.Stop();

            this.m_PayTimer = null;

            // Get all of the gold on the hireling and add up the total amount 
            if (this.Backpack != null)
            {
                Item[] AllGold = this.Backpack.FindItemsByType(typeof(Gold), true);
                if (AllGold != null)
                {
                    foreach (Gold g in AllGold)
                        this.m_GoldOnDeath += g.Amount;
                }
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        { 
            if (this.m_GoldOnDeath > 0) 
                c.DropItem(new Gold(this.m_GoldOnDeath));

            base.OnDeath(c);
        }

        [CommandProperty(AccessLevel.Player)]
        public bool IsHired
        {
            get
            {
                return this.m_IsHired;
            }
            set
            {
                if (this.m_IsHired == value)
                    return;

                this.m_IsHired = value;
                this.Delta(MobileDelta.Noto);
                this.InvalidateProperties();
            }
        }

        #region [ GetOwner ]
        public virtual Mobile GetOwner()
        {
            if (!this.Controlled)
                return null;
            Mobile Owner = this.ControlMaster;
	  
            this.m_IsHired = true;
	  
            if (Owner == null)
                return null;
	  
            if (Owner.Deleted || Owner.Map != this.Map || !Owner.InRange(this.Location, 30))
            {
                this.Say(1005653);// Hmmm.  I seem to have lost my master.
                BaseHire.HireTable.Remove(Owner);
                this.SetControlMaster(null);
                return null;
            }

            return Owner;
        }

        #endregion 

        #region [ AddHire ] 
        public virtual bool AddHire(Mobile m) 
        { 
            Mobile owner = this.GetOwner();

            if (owner != null) 
            { 
                m.SendLocalizedMessage(1043283, owner.Name);// I am following ~1_NAME~. 
                return false;
            }

            if (this.SetControlMaster(m)) 
            { 
                this.m_IsHired = true;
                return true;
            }
	  
            return false;
        }

        #endregion 

        #region [ Payday ] 
        public virtual bool Payday(BaseHire m) 
        { 
            this.m_Pay = (int)m.Skills[SkillName.Anatomy].Value + (int)m.Skills[SkillName.Tactics].Value;
            this.m_Pay += (int)m.Skills[SkillName.Macing].Value + (int)m.Skills[SkillName.Swords].Value;
            this.m_Pay += (int)m.Skills[SkillName.Fencing].Value + (int)m.Skills[SkillName.Archery].Value;
            this.m_Pay += (int)m.Skills[SkillName.MagicResist].Value + (int)m.Skills[SkillName.Healing].Value;
            this.m_Pay += (int)m.Skills[SkillName.Magery].Value + (int)m.Skills[SkillName.Parry].Value;
            this.m_Pay /= 35;
            this.m_Pay += 1;
            return true;
        }

        #endregion 

        #region [ OnDragDrop ]
        public override bool OnDragDrop(Mobile from, Item item)
        { 
            if (this.m_Pay != 0)
            { 
                // Is the creature already hired
                if (this.Controlled == false)
                { 
                    // Is the item the payment in gold
                    if (item is Gold)
                    { 
                        // Is the payment in gold sufficient
                        if (item.Amount >= this.m_Pay)
                        { 
                            // Check if this mobile already has a hire
                            BaseHire hire = (BaseHire)m_HireTable[from];

                            if (hire != null && !hire.Deleted && hire.GetOwner() == from)
                            {
                                this.SayTo(from, 500896);// I see you already have an escort.
                                return false;
                            }

                            // Try to add the hireling as a follower
                            if (this.AddHire(from) == true)
                            { 
                                this.SayTo(from, 1043258, string.Format("{0}", (int)item.Amount / this.m_Pay));//"I thank thee for paying me. I will work for thee for ~1_NUMBER~ days.", (int)item.Amount / m_Pay );
                                m_HireTable[from] = this;
                                this.m_HoldGold += item.Amount;
                                this.m_PayTimer = new PayTimer(this);
                                this.m_PayTimer.Start();
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else 
                        { 
                            this.SayHireCost();
                        }
                    }
                    else 
                    {
                        this.SayTo(from, 1043268);// Tis crass of me, but I want gold
                    }
                }
                else 
                { 
                    this.Say(1042495);// I have already been hired.
                }
            }
            else 
            {
                this.SayTo(from, 500200);// I have no need for that.
            }

            return base.OnDragDrop(from, item);
        }

        #endregion 

        #region [ OnSpeech ] 
        internal void SayHireCost() 
        { 
            this.Say(1043256, string.Format("{0}", this.m_Pay));// "I am available for hire for ~1_AMOUNT~ gold coins a day. If thou dost give me gold, I will work for thee."
        }

        public override void OnSpeech(SpeechEventArgs e) 
        { 
            if (!e.Handled && e.Mobile.InRange(this, 6)) 
            { 
                int[] keywords = e.Keywords;
                string speech = e.Speech;

                // Check for a greeting, a 'hire', or a 'servant'
                if ((e.HasKeyword(0x003B) == true) || (e.HasKeyword(0x0162) == true) || (e.HasKeyword(0x000C) == true))
                {
                    if (this.Controlled == false)
                    {
                        e.Handled = this.Payday(this);
                        this.SayHireCost();
                    }
                    else
                    {
                        this.Say(1042495);// I have already been hired.
                    }
                }
            }

            base.OnSpeech(e);
        }

        #endregion	

        #region [ GetContextMenuEntries ] 
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (this.Deleted)
                return;

            if (this.CanPaperdollBeOpenedBy(from))
                list.Add(new PaperdollEntry(this));

            if (from == this && this.Backpack != null && this.CanSee(this.Backpack) && this.CheckAlive(false))
                list.Add(new OpenBackpackEntry(this));

            if (this.Controlled == false)
                list.Add(new HireEntry(from, this));
        }

        #endregion 
	
        #region [ Class PayTimer ] 
        private class PayTimer : Timer 
        { 
            private readonly BaseHire m_Hire;
	  
            public PayTimer(BaseHire vend)
                : base(TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(30.0))
            { 
                this.m_Hire = vend;
                this.Priority = TimerPriority.OneMinute;
            }
	  
            protected override void OnTick() 
            { 
                int m_Pay = this.m_Hire.m_Pay;
                if (this.m_Hire.m_HoldGold <= m_Pay) 
                { 
                    // Get the current owner, if any (updates HireTable) 
                    Mobile owner = this.m_Hire.GetOwner();

                    this.m_Hire.Say(503235);// I regret nothing!postal 
                    this.m_Hire.Delete();
                }
                else 
                { 
                    this.m_Hire.m_HoldGold -= m_Pay;
                }
            }
        }
        #endregion 

        #region [ Class HireEntry ]
        public class HireEntry : ContextMenuEntry
        { 
            private readonly Mobile m_Mobile;
            private readonly BaseHire m_Hire;

            public HireEntry(Mobile from, BaseHire hire)
                : base(6120, 3)
            { 
                this.m_Hire = hire;
                this.m_Mobile = from;
            }
	  
            public override void OnClick()
            {
                this.m_Hire.Payday(this.m_Hire);
                this.m_Hire.SayHireCost();
            }
        }
        #endregion
    }
}