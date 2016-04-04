using System;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.Quests
{ 
    public class BaseObjective
    { 
        private BaseQuest m_Quest;
        private int m_MaxProgress;
        private int m_CurProgress;
        private int m_Seconds;
        private bool m_Timed;
        public BaseObjective()
            : this(1, 0)
        {
        }

        public BaseObjective(int maxProgress)
            : this(maxProgress, 0)
        {
        }

        public BaseObjective(int maxProgress, int seconds)
        { 
            this.m_MaxProgress = maxProgress;
            this.m_Seconds = seconds;
			
            if (seconds > 0)
                this.Timed = true;
            else
                this.Timed = false;	
        }

        public BaseQuest Quest
        {
            get
            {
                return this.m_Quest;
            }
            set
            {
                this.m_Quest = value;
            }
        }
        public int MaxProgress
        {
            get
            {
                return this.m_MaxProgress;
            }
            set
            {
                this.m_MaxProgress = value;
            }
        }
        public int CurProgress
        {
            get
            {
                return this.m_CurProgress;
            }
            set
            { 
                this.m_CurProgress = value; 
				
                if (this.Completed)
                    this.OnCompleted();
					
                if (this.m_CurProgress == -1)
                    this.OnFailed();
					
                if (this.m_CurProgress < -1)
                    this.m_CurProgress = -1;
            }
        }
        public int Seconds
        {
            get
            {
                return this.m_Seconds;
            }
            set
            { 
                this.m_Seconds = value;
				
                if (this.m_Seconds < 0)
                    this.m_Seconds = 0;
            }
        }
        public bool Timed
        {
            get
            {
                return this.m_Timed;
            }
            set
            {
                this.m_Timed = value;
            }
        }
        public bool Completed
        {
            get
            {
                return this.CurProgress >= this.MaxProgress;
            }
        }
        public bool Failed
        { 
            get
            {
                return this.CurProgress == -1;
            }
        }
        public virtual void Complete()
        {
            this.CurProgress = this.MaxProgress;
        }

        public virtual void Fail()
        {
            this.CurProgress = -1;
        }

        public virtual void OnAccept()
        {
        }

        public virtual void OnCompleted()
        {
        }

        public virtual void OnFailed()
        {
        }

        public virtual Type Type()
        {
            return null;
        }

        public virtual bool Update(object obj)
        {
            return false;
        }

        public virtual void UpdateTime()
        {
            if (!this.Timed || this.Failed)
                return;
		
            if (this.Seconds > 0)
            {
                this.Seconds -= 1;
            }
            else if (!this.Completed)
            {
                this.m_Quest.Owner.SendLocalizedMessage(1072258); // You failed to complete an objective in time!
				
                this.Fail();
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version
			
            writer.Write((int)this.m_CurProgress);
            writer.Write((int)this.m_Seconds);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
			
            this.m_CurProgress = reader.ReadInt();
            this.m_Seconds = reader.ReadInt();
        }
    }

    public class SlayObjective : BaseObjective
    {
        private Type m_Creature;
        private string m_Name;
        private Region m_Region;
        public SlayObjective(Type creature, string name, int amount)
            : this(creature, name, amount, null)
        {
        }

        public SlayObjective(Type creature, string name, int amount, string region)
            : this(creature, name, amount, region, 0)
        {
        }

        public SlayObjective(Type creature, string name, int amount, int seconds)
            : this(creature, name, amount, null, seconds)
        {
        }

        public SlayObjective(Type creature, string name, int amount, string region, int seconds)
            : base(amount, seconds)
        { 
            this.m_Creature = creature;
            this.m_Name = name;

            if (region != null)
            {
                this.m_Region = QuestHelper.FindRegion(region);

                if (this.m_Region == null)
                    Console.WriteLine(String.Format("Invalid region name ('{0}') in '{1}' objective!", region, this.GetType()));
            }
        }

        public Type Creature
        { 
            get
            {
                return this.m_Creature;
            }
            set
            {
                this.m_Creature = value;
            }
        }
        public string Name
        { 
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        public Region Region
        { 
            get
            {
                return this.m_Region;
            }
            set
            {
                this.m_Region = value;
            }
        }
        public virtual void OnKill(Mobile killed)
        {
            if (this.Completed)
                this.Quest.Owner.SendLocalizedMessage(1075050); // You have killed all the required quest creatures of this type.
            else
                this.Quest.Owner.SendLocalizedMessage(1075051, (this.MaxProgress - this.CurProgress).ToString()); // You have killed a quest creature. ~1_val~ more left.
        }

        public virtual bool IsObjective(Mobile mob)
        { 
            if (this.m_Creature == null)
                return false;
		
            if (this.m_Creature.IsAssignableFrom(mob.GetType()))
            {
                if (this.m_Region != null && !this.m_Region.Contains(mob.Location))
                    return false;
					
                return true;
            }
			
            return false;
        }

        public override bool Update(object obj)
        {
            if (obj is Mobile)
            {
                Mobile mob = (Mobile)obj;
			
                if (this.IsObjective(mob))
                { 
                    if (!this.Completed)
                        this.CurProgress += 1;
						
                    this.OnKill(mob);					
                    return true;
                }
            }
			
            return false;
        }

        public override Type Type()
        {
            return this.m_Creature;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class ObtainObjective : BaseObjective
    {
        private Type m_Obtain;
        private string m_Name;
        private int m_Image;
        public ObtainObjective(Type obtain, string name, int amount)
            : this(obtain, name, amount, 0, 0)
        {
        }

        public ObtainObjective(Type obtain, string name, int amount, int image)
            : this(obtain, name, amount, image, 0)
        {
        }

        public ObtainObjective(Type obtain, string name, int amount, int image, int seconds)
            : base(amount, seconds)
        { 
            this.m_Obtain = obtain;
            this.m_Name = name;
            this.m_Image = image;
        }

        public Type Obtain
        { 
            get
            {
                return this.m_Obtain;
            }
            set
            {
                this.m_Obtain = value;
            }
        }
        public string Name
        { 
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        public int Image
        { 
            get
            {
                return this.m_Image;
            }
            set
            {
                this.m_Image = value;
            }
        }
        public override bool Update(object obj)
        { 
            if (obj is Item)
            {
                Item obtained = (Item)obj;
				
                if (this.IsObjective(obtained))
                { 
                    if (!obtained.QuestItem)
                    { 
                        this.CurProgress += obtained.Amount;
							
                        obtained.QuestItem = true;
                        this.Quest.Owner.SendLocalizedMessage(1072353); // You set the item to Quest Item status
                    }
                    else
                    {
                        this.CurProgress -= obtained.Amount;
							
                        obtained.QuestItem = false;
                        this.Quest.Owner.SendLocalizedMessage(1072354); // You remove Quest Item status from the item
                    }
						
                    return true;
                }
            }
			
            return false;
        }

        public virtual bool IsObjective(Item item)
        {
            if (this.m_Obtain == null)
                return false;
		
            if (this.m_Obtain.IsAssignableFrom(item.GetType()))
                return true;
			
            return false;
        }

        public override Type Type()
        {
            return this.m_Obtain;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class DeliverObjective : BaseObjective
    {
        private Type m_Delivery;
        private String m_DeliveryName;
        private Type m_Destination;
        private String m_DestName;
        public DeliverObjective(Type delivery, String deliveryName, int amount, Type destination, String destName)
            : this(delivery, deliveryName, amount, destination, destName, 0)
        {
        }

        public DeliverObjective(Type delivery, String deliveryName, int amount, Type destination, String destName, int seconds)
            : base(amount, seconds)
        { 
            this.m_Delivery = delivery;
            this.m_DeliveryName = deliveryName;
			
            this.m_Destination = destination;
            this.m_DestName = destName;
        }

        public Type Delivery
        { 
            get
            {
                return this.m_Delivery;
            }
            set
            {
                this.m_Delivery = value;
            }
        }
        public String DeliveryName
        { 
            get
            {
                return this.m_DeliveryName;
            }
            set
            {
                this.m_DeliveryName = value;
            }
        }
        public Type Destination
        { 
            get
            {
                return this.m_Destination;
            }
            set
            {
                this.m_Destination = value;
            }
        }
        public String DestName
        { 
            get
            {
                return this.m_DestName;
            }
            set
            {
                this.m_DestName = value;
            }
        }
        public override void OnAccept()
        {
            if (this.Quest.StartingItem != null)
            {
                this.Quest.StartingItem.QuestItem = true;
                return;
            }
			
            int amount = this.MaxProgress;		
			
            while (amount > 0 && !this.Failed)
            { 
                Item item = QuestHelper.Construct(this.m_Delivery) as Item;
				
                if (item == null)
                {
                    this.Fail();
                    break;
                }
					
                if (item.Stackable)
                {
                    item.Amount = amount;
                    amount = 1;
                }
						
                if (!this.Quest.Owner.PlaceInBackpack(item))
                {
                    this.Quest.Owner.SendLocalizedMessage(503200); // You do not have room in your backpack for this.
                    this.Quest.Owner.SendLocalizedMessage(1075574); // Could not create all the necessary items. Your quest has not advanced.
					
                    this.Fail();
					
                    break;
                }
                else
                    item.QuestItem = true;
						
                amount -= 1;
            }
			
            if (this.Failed)
            {
                QuestHelper.DeleteItems(this.Quest.Owner, this.m_Delivery, this.MaxProgress - amount, false);
			
                this.Quest.RemoveQuest();	
            }
        }

        public override bool Update(object obj)
        { 
            if (this.m_Delivery == null || this.m_Destination == null)
                return false;
				
            if (this.Failed)
            {
                this.Quest.Owner.SendLocalizedMessage(1074813);  // You have failed to complete your delivery.
                return false;
            }
			
            if (obj is BaseVendor)
            {
                if (this.Quest.StartingItem != null)
                {
                    this.Complete();					
                    return true;
                }
                else if (this.m_Destination.IsAssignableFrom(obj.GetType()))
                { 
                    if (this.MaxProgress < QuestHelper.CountQuestItems(this.Quest.Owner, this.Delivery))
                    {
                        this.Quest.Owner.SendLocalizedMessage(1074813);  // You have failed to complete your delivery.						
                        this.Fail();
                    }
                    else
                        this.Complete();
					
                    return true;
                }
            }
			
            return false;
        }

        public override Type Type()
        {
            return this.m_Delivery;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class EscortObjective : BaseObjective
    {
        private Region m_Region;
        private int m_Fame;
        private int m_Compassion;
        public EscortObjective(string region)
            : this(region, 10, 200, 0)
        {
        }

        public EscortObjective(string region, int fame)
            : this(region, fame, 200)
        {
        }

        public EscortObjective(string region, int fame, int compassion)
            : this(region, fame, compassion, 0)
        {
        }

        public EscortObjective(string region, int fame, int compassion, int seconds)
            : base(1, seconds)
        {
            this.m_Region = QuestHelper.FindRegion(region);
            this.m_Fame = fame;
            this.m_Compassion = compassion;

            if (this.m_Region == null)
                Console.WriteLine(String.Format("Invalid region name ('{0}') in '{1}' objective!", region, this.GetType()));
        }

        public Region Region
        { 
            get
            {
                return this.m_Region;
            }
            set
            {
                this.m_Region = value;
            }
        }
        public int Fame
        {
            get
            {
                return this.m_Fame;
            }
            set
            {
                this.m_Fame = value;
            }
        }
        public int Compassion
        {
            get
            {
                return this.m_Compassion;
            }
            set
            {
                this.m_Compassion = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }

    public class ApprenticeObjective : BaseObjective
    {
        private SkillName m_Skill;
        private Region m_Region;
        private object m_Enter;
        private object m_Leave;
        public ApprenticeObjective(SkillName skill, int cap)
            : this(skill, cap, null, null, null)
        {
        }

        public ApprenticeObjective(SkillName skill, int cap, string region, object enterRegion, object leaveRegion)
            : base(cap)
        {
            this.m_Skill = skill;
		
            if (region != null)
            {
                this.m_Region = QuestHelper.FindRegion(region);					
                this.m_Enter = enterRegion;	
                this.m_Leave = leaveRegion;
				
                if (this.m_Region == null)
                    Console.WriteLine(String.Format("Invalid region name ('{0}') in '{1}' objective!", region, this.GetType()));
            }
        }

        public SkillName Skill
        {
            get
            {
                return this.m_Skill;
            }
            set
            {
                this.m_Skill = value;
            }
        }
        public Region Region
        {
            get
            {
                return this.m_Region;
            }
            set
            {
                this.m_Region = value;
            }
        }
        public object Enter
        {
            get
            {
                return this.m_Enter;
            }
            set
            {
                this.m_Enter = value;
            }
        }
        public object Leave
        {
            get
            {
                return this.m_Leave;
            }
            set
            {
                this.m_Leave = value;
            }
        }
        public override bool Update(object obj)
        {
            if (this.Completed)
                return false;
		
            if (obj is Skill)
            {
                Skill skill = (Skill)obj;				
				
                if (skill.SkillName != this.m_Skill)
                    return false;				
				
                if (this.Quest.Owner.Skills[this.m_Skill].Base >= this.MaxProgress)
                {
                    this.Complete();
                    return true;
                }
            }
			
            return false;
        }

        public override void OnAccept()
        {
            Region region = this.Quest.Owner.Region;
			
            while (region != null)
            { 
                if (region is ApprenticeRegion)
                    region.OnEnter(this.Quest.Owner);									
				
                region = region.Parent;
            }
        }

        public override void OnCompleted()
        {
            QuestHelper.RemoveAcceleratedSkillgain(this.Quest.Owner);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadEncodedInt();
        }
    }
}