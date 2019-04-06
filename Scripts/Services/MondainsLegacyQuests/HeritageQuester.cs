using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Spells.Fifth;

namespace Server.Engines.Quests
{
    public class HeritageQuestInfo
    {
        private readonly Type m_Quest;
        private readonly object m_Title;
		
        public Type Quest
        {
            get
            {
                return this.m_Quest;
            }
        }
        public object Title
        {
            get
            {
                return this.m_Title;
            }
        }
		
        public HeritageQuestInfo(Type quest, object title)
        {
            this.m_Quest = quest;
            this.m_Title = title;
        }
		
        public bool Check(PlayerMobile player)
        {
            return this.Check(player, false);
        }
		
        public bool Check(PlayerMobile player, bool delete)
        {
            int j = 0;

            while (j < player.DoneQuests.Count && player.DoneQuests[j].QuestType != this.m_Quest)
            {
                //if(player.Murderer && this.m_Quest == typeof(ResponsibilityQuest)  && player.DoneQuests[j].QuestType.IsSubclassOf(typeof(

                j += 1;
            }
			
            if (j == player.DoneQuests.Count)
                return false;
            else if (delete)
                player.DoneQuests.RemoveAt(j);
				
            return true;
        }
    }
	
    public abstract class HeritageQuester : BaseVendor
    {
        #region Vendor stuff		
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override bool IsActiveVendor
        {
            get
            {
                return false;
            }
        }
        public override void InitSBInfo()
        {
        }

        #endregion
		
        public virtual int AutoSpeakRange
        {
            get
            {
                return 7;
            }
        }
        public virtual object ConfirmMessage
        {
            get
            {
                return 0;
            }
        }
        public virtual object IncompleteMessage
        {
            get
            {
                return 0;
            }
        }
			
        private List<HeritageQuestInfo> m_Quests;	
        private List<object> m_Objectives;	
        private List<object> m_Story;
        private bool m_Busy;
        private int m_Index;
		
        public List<HeritageQuestInfo> Quests
        {
            get
            {
                return this.m_Quests;
            }
        }
		
        public List<object> Objectives
        {
            get
            {
                return this.m_Objectives;
            }
        }
		
        public List<object> Story
        {
            get
            {
                return this.m_Story;
            }
        }
		
        public HeritageQuester()
            : this(null)
        {
        }
		
        public HeritageQuester(string name)
            : this(name, null)
        {
        }

        public HeritageQuester(string name, string title)
            : base(title)
        {
            this.m_Quests = new List<HeritageQuestInfo>();
            this.m_Objectives = new List<object>();
            this.m_Story = new List<object>();
			
            this.Initialize();
			
            this.Name = name;
            this.SpeechHue = 0x3B2;
        }
		
        public HeritageQuester(Serial serial)
            : base(serial)
        {
        }
		
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Alive && !m.Hidden && m is PlayerMobile)
            {
                int range = this.AutoSpeakRange;

                if (range >= 0 && this.InRange(m, range) && !this.InRange(oldLocation, range))
                    this.OnTalk(m);
            }
        }
		
        public override void OnDoubleClick(Mobile m)
        {
            Console.WriteLine(m.Items.Count);
		
            if (m.Alive)
                this.OnTalk(m);		
        }
		
        public virtual void OnTalk(Mobile m)
        { 
            if (m.Hidden || this.m_Busy || m.Race == this.Race)
            {
                m.SendLocalizedMessage(1074017); // He's too busy right now, so he ignores you.
                return;
            }
			
            this.m_Busy = true;
            this.m_Index = 0;
			
            this.SpeechHue = Utility.RandomDyedHue();
            this.Say(m.Name);
            this.SpeechHue = 0x3B2;
			
            if (this.CheckCompleted(m))
                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(10), this.Story.Count + 1, new TimerStateCallback(SayStory), m);
            else
            {
                List<object> incomplete = this.FindIncompleted(m);
                TimeSpan delay = TimeSpan.FromSeconds(2);
				
                if (incomplete.Count == this.m_Quests.Count + 1)
                {
                    incomplete = this.m_Objectives;
                    delay = TimeSpan.FromSeconds(10);
                }
				
                Timer.DelayCall(TimeSpan.Zero, delay, incomplete.Count, new TimerStateCallback(SayInstructions), incomplete); 
            }
        }
		
        public bool CheckCompleted(Mobile m)
        {
            return this.CheckCompleted(m, false);
        }
		
        public bool CheckCompleted(Mobile m, bool delete)
        { 
            for (int i = 0; i < this.m_Quests.Count; i += 1)
            {
                HeritageQuestInfo info = this.m_Quests[i];
				
                if (!info.Check((PlayerMobile)m, delete))
                    return false;
            }
			
            return true;
        }
		
        public List<object> FindIncompleted(Mobile m)
        { 
            List<object> incomplete = new List<object>();
						
            incomplete.Add(this.IncompleteMessage);
				
            for (int i = 0; i < this.m_Quests.Count; i += 1)
            {
                HeritageQuestInfo info = this.m_Quests[i];
				
                if (!info.Check((PlayerMobile)m))
                    incomplete.Add(info.Title);
            }
			
            return incomplete;
        }
		
        private void SayInstructions(object args)
        {
            if (args is List<object>)
                this.SayInstructions((List<object>)args);
        }
		
        public void SayInstructions(List<object> incomplete)
        {
            Say(this, incomplete[this.m_Index]);
				
            this.m_Index += 1;
			
            if (this.m_Index == incomplete.Count)
                this.m_Busy = false;
        }
		
        private void SayStory(object args)
        {
            if (args is Mobile)
                this.SayStory((Mobile)args);
        }
		
        public void SayStory(Mobile m)
        {
            if (this.m_Index < this.m_Story.Count)
                Say(this, this.m_Story[this.m_Index]);
            else
            {
                this.m_Busy = false;	
				
                m.CloseGump(typeof(ConfirmHeritageGump));
                m.SendGump(new ConfirmHeritageGump(this));
            }
			
            this.m_Index += 1;
        }
		
        #region Static
        private static readonly Dictionary<Mobile, HeritageQuester> m_Pending = new Dictionary<Mobile, HeritageQuester>();
		
        public static void AddPending(Mobile m, HeritageQuester quester)
        {
            m_Pending[m] = quester;
        }
		
        public static void RemovePending(Mobile m)
        {
            if (m_Pending.ContainsKey(m))
            {
                m_Pending.Remove(m);
            }
        }
		
        public static bool IsPending(Mobile m)
        {
            return m_Pending.ContainsKey(m) && m_Pending[m] != null;
        }
		
        public static HeritageQuester Pending(Mobile m)
        {
            return m_Pending.ContainsKey(m) ? m_Pending[m] as HeritageQuester : null;
        }
		
        public static void Say(Mobile m, object message)
        {
            if (message is int)
                m.Say((int)message);
            else if (message is string)
                m.Say((string)message);			
        }
		
        public static bool Check(Mobile m)
        { 
            if (!m.Alive)
                m.SendLocalizedMessage(1073646); // Only the living may proceed...			
            else if (m.Mounted)
                m.SendLocalizedMessage(1073647); // You may not continue while mounted...			
            else if (m.IsBodyMod || m.HueMod > 0 || !m.CanBeginAction(typeof(IncognitoSpell)))
                m.SendLocalizedMessage(1073648); // You may only proceed while in your original state...						
            else if (m.Spell != null && m.Spell.IsCasting)
                m.SendLocalizedMessage(1073649); // One may not proceed while embracing magic...			
            else if (IsUnburdened(m))
                m.SendLocalizedMessage(1073650); // To proceed you must be unburdened by equipment...
            else if (!m.NetState.SupportsExpansion(Expansion.ML))
                m.SendLocalizedMessage(1073651); // You must have Mondain's Legacy before proceeding...
            else if (m.Hits < m.HitsMax)
                m.SendLocalizedMessage(1073652); // You must be healthy to proceed...				
            else
                return true;
				
            return false;
        }
		
        public static bool IsUnburdened(Mobile m)
        {
            int count = m.Items.Count - 1;
				
            if (m.Backpack != null)
                count -= 1;
				
            return count > 0;
        }

        #endregion
		
        public virtual void Initialize()
        { 
        }
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();		
			
            this.m_Quests = new List<HeritageQuestInfo>();
            this.m_Objectives = new List<object>();
            this.m_Story = new List<object>();
			
            this.Initialize();
        }
    }
}