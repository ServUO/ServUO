using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class Aquarium : BaseAddonContainer
    {
        public static readonly TimeSpan EvaluationInterval = TimeSpan.FromDays(1);

        // items info
        private int m_LiveCreatures;

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiveCreatures
        {
            get
            {
                return this.m_LiveCreatures;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DeadCreatures
        {
            get
            {
                int dead = 0;

                for (int i = 0; i < this.Items.Count; i ++)
                {
                    if (this.Items[i] is BaseFish)
                    {
                        BaseFish fish = (BaseFish)this.Items[i];

                        if (fish.Dead)
                            dead += 1;
                    }
                }

                return dead;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLiveCreatures
        {
            get
            {
                int state = (this.m_Food.State == (int)FoodState.Overfed) ? 1 : (int)FoodState.Full - this.m_Food.State;

                state += (int)WaterState.Strong - this.m_Water.State;

                state = (int)Math.Pow(state, 1.75);

                return this.MaxItems - state;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsFull
        {
            get
            {
                return (this.Items.Count >= this.MaxItems);
            }
        }

        // vacation info
        private int m_VacationLeft;

        [CommandProperty(AccessLevel.GameMaster)]
        public int VacationLeft
        {
            get
            {
                return this.m_VacationLeft;
            }
            set
            {
                this.m_VacationLeft = value;
                this.InvalidateProperties();
            }
        }

        // aquarium state
        private AquariumState m_Food;
        private AquariumState m_Water;

        [CommandProperty(AccessLevel.GameMaster)]
        public AquariumState Food
        {
            get
            {
                return this.m_Food;
            }
            set
            {
                this.m_Food = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AquariumState Water
        {
            get
            {
                return this.m_Water;
            }
            set
            {
                this.m_Water = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool OptimalState
        {
            get
            {
                return (this.m_Food.State == (int)FoodState.Full && this.m_Water.State == (int)WaterState.Strong);
            }
        }

        // events
        private List<int> m_Events;
        private bool m_RewardAvailable;
        private bool m_EvaluateDay;

        public List<int> Events
        {
            get
            {
                return this.m_Events;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RewardAvailable
        {
            get
            {
                return this.m_RewardAvailable;
            }
            set
            {
                this.m_RewardAvailable = value;
                this.InvalidateProperties();
            }
        }

        // evaluate timer
        private Timer m_Timer;

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                if (this.ItemID == 0x3062)
                    return new AquariumEastDeed();
                else
                    return new AquariumNorthDeed();
            }
        }

        public override double DefaultWeight
        {
            get
            {
                return 10.0;
            }
        }

        public Aquarium(int itemID)
            : base(itemID)
        {
            this.Movable = false;

            if (itemID == 0x3060)
                this.AddComponent(new AddonContainerComponent(0x3061), -1, 0, 0);

            if (itemID == 0x3062)
                this.AddComponent(new AddonContainerComponent(0x3063), 0, -1, 0);

            this.MaxItems = 30;

            this.m_Food = new AquariumState();
            this.m_Water = new AquariumState();

            this.m_Food.State = (int)FoodState.Full;
            this.m_Water.State = (int)WaterState.Strong;

            this.m_Food.Maintain = Utility.RandomMinMax(1, 2);
            this.m_Food.Improve = this.m_Food.Maintain + Utility.RandomMinMax(1, 2);

            this.m_Water.Maintain = Utility.RandomMinMax(1, 3);

            this.m_Events = new List<int>();

            this.m_Timer = Timer.DelayCall(EvaluationInterval, EvaluationInterval, new TimerCallback(Evaluate));
        }

        public Aquarium(Serial serial)
            : base(serial)
        {
        }

        public override void OnDelete()
        {
            if (this.m_Timer != null)
            {
                this.m_Timer.Stop();
                this.m_Timer = null;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            this.ExamineAquarium(from);
        }

        public virtual bool HasAccess(Mobile from)
        {
            if (from == null || from.Deleted)
                return false;
            else if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            return (house != null && house.IsCoOwner(from));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!this.HasAccess(from))
            {
                from.SendLocalizedMessage(1073821); // You do not have access to that item for use with the aquarium.
                return false;
            }

            if (this.m_VacationLeft > 0)
            {
                from.SendLocalizedMessage(1074427); // The aquarium is in vacation mode.
                return false;
            }

            bool takeItem = true;

            if (dropped is FishBowl)
            {
                FishBowl bowl = (FishBowl)dropped;

                if (bowl.Empty || !this.AddFish(from, bowl.Fish))
                    return false;

                bowl.InvalidateProperties();

                takeItem = false;
            }
            else if (dropped is BaseFish)
            {
                BaseFish fish = (BaseFish)dropped;

                if (!this.AddFish(from, fish))
                    return false;
            }
            else if (dropped is VacationWafer)
            {
                this.m_VacationLeft = VacationWafer.VacationDays;
                dropped.Delete();

                from.SendLocalizedMessage(1074428, this.m_VacationLeft.ToString()); // The aquarium will be in vacation mode for ~1_DAYS~ days
            }
            else if (dropped is AquariumFood)
            {
                this.m_Food.Added += 1;
                dropped.Delete();

                from.SendLocalizedMessage(1074259, "1"); // ~1_NUM~ unit(s) of food have been added to the aquarium.
            }
            else if (dropped is BaseBeverage)
            {
                BaseBeverage beverage = (BaseBeverage)dropped;

                if (beverage.IsEmpty || !beverage.Pourable || beverage.Content != BeverageType.Water)
                {
                    from.SendLocalizedMessage(500840); // Can't pour that in there.
                    return false;
                }

                this.m_Water.Added += 1;
                beverage.Quantity -= 1;

                from.PlaySound(0x4E);
                from.SendLocalizedMessage(1074260, "1"); // ~1_NUM~ unit(s) of water have been added to the aquarium.

                takeItem = false;
            }
            else if (!this.AddDecoration(from, dropped))
            {
                takeItem = false;
            }

            from.CloseGump(typeof(AquariumGump));

            this.InvalidateProperties();

            if (takeItem)
                from.PlaySound(0x42);

            return takeItem;
        }

        public override void DropItemsToGround()
        {
            Point3D loc = this.GetWorldLocation();

            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                Item item = this.Items[i];

                item.MoveToWorld(loc, this.Map);

                if (item is BaseFish)
                {
                    BaseFish fish = (BaseFish)item;

                    if (!fish.Dead)
                        fish.StartTimer();
                }
            }
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (item != this)
                return false;

            return base.CheckItemUse(from, item);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (item != this)
            {
                reject = LRReason.CannotLift;
                return false;
            }

            return base.CheckLift(from, item, ref reject);
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (this.m_VacationLeft > 0)
                list.Add(1074430, this.m_VacationLeft.ToString()); // Vacation days left: ~1_DAYS

            if (this.m_Events.Count > 0)
                list.Add(1074426, this.m_Events.Count.ToString()); // ~1_NUM~ event(s) to view!

            if (this.m_RewardAvailable)
                list.Add(1074362); // A reward is available!

            list.Add(1074247, "{0}\t{1}", this.m_LiveCreatures, this.MaxLiveCreatures); // Live Creatures: ~1_NUM~ / ~2_MAX~

            int dead = this.DeadCreatures;

            if (dead > 0)
                list.Add(1074248, dead.ToString()); // Dead Creatures: ~1_NUM~

            int decorations = this.Items.Count - this.m_LiveCreatures - dead;

            if (decorations > 0)
                list.Add(1074249, decorations.ToString()); // Decorations: ~1_NUM~

            list.Add(1074250, "#{0}", this.FoodNumber()); // Food state: ~1_STATE~
            list.Add(1074251, "#{0}", this.WaterNumber()); // Water state: ~1_STATE~

            if (this.m_Food.State == (int)FoodState.Dead)
                list.Add(1074577, "{0}\t{1}", this.m_Food.Added, this.m_Food.Improve); // Food Added: ~1_CUR~ Needed: ~2_NEED~
            else if (this.m_Food.State == (int)FoodState.Overfed)
                list.Add(1074577, "{0}\t{1}", this.m_Food.Added, this.m_Food.Maintain); // Food Added: ~1_CUR~ Needed: ~2_NEED~
            else
                list.Add(1074253, "{0}\t{1}\t{2}", this.m_Food.Added, this.m_Food.Maintain, this.m_Food.Improve); // Food Added: ~1_CUR~ Feed: ~2_NEED~ Improve: ~3_GROW~

            if (this.m_Water.State == (int)WaterState.Dead)
                list.Add(1074578, "{0}\t{1}", this.m_Water.Added, this.m_Water.Improve); // Water Added: ~1_CUR~ Needed: ~2_NEED~
            else if (this.m_Water.State == (int)WaterState.Strong)
                list.Add(1074578, "{0}\t{1}", this.m_Water.Added, this.m_Water.Maintain); // Water Added: ~1_CUR~ Needed: ~2_NEED~
            else
                list.Add(1074254, "{0}\t{1}\t{2}", this.m_Water.Added, this.m_Water.Maintain, this.m_Water.Improve); // Water Added: ~1_CUR~ Maintain: ~2_NEED~ Improve: ~3_GROW~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
            {
                list.Add(new ExamineEntry(this));

                if (this.HasAccess(from))
                {
                    if (this.m_RewardAvailable)
                        list.Add(new CollectRewardEntry(this));

                    if (this.m_Events.Count > 0)
                        list.Add(new ViewEventEntry(this));

                    if (this.m_VacationLeft > 0)
                        list.Add(new CancelVacationMode(this));
                }
            }

            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                list.Add(new GMAddFood(this));
                list.Add(new GMAddWater(this));
                list.Add(new GMForceEvaluate(this));
                list.Add(new GMOpen(this));
                list.Add(new GMFill(this));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(3); // Version

            // version 1
            if (this.m_Timer != null)
                writer.Write(this.m_Timer.Next);
            else
                writer.Write(DateTime.UtcNow + EvaluationInterval);

            // version 0
            writer.Write((int)this.m_LiveCreatures);
            writer.Write((int)this.m_VacationLeft);

            this.m_Food.Serialize(writer);
            this.m_Water.Serialize(writer);

            writer.Write((int)this.m_Events.Count);

            for (int i = 0; i < this.m_Events.Count; i ++)
                writer.Write((int)this.m_Events[i]);

            writer.Write((bool)this.m_RewardAvailable);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                case 2:
                case 1:
                    {
                        DateTime next = reader.ReadDateTime();

                        if (next < DateTime.UtcNow)
                            next = DateTime.UtcNow;

                        this.m_Timer = Timer.DelayCall(next - DateTime.UtcNow, EvaluationInterval, new TimerCallback(Evaluate));

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_LiveCreatures = reader.ReadInt();
                        this.m_VacationLeft = reader.ReadInt();

                        this.m_Food = new AquariumState();
                        this.m_Water = new AquariumState();

                        this.m_Food.Deserialize(reader);
                        this.m_Water.Deserialize(reader);

                        this.m_Events = new List<int>();

                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i ++)
                            this.m_Events.Add(reader.ReadInt());

                        this.m_RewardAvailable = reader.ReadBool();

                        break;
                    }
            }

            if (version < 2)
            {
                this.Weight = this.DefaultWeight;
                this.Movable = false;
            }

            if (version < 3)
                ValidationQueue<Aquarium>.Add(this);
        }

        private void RecountLiveCreatures()
        {
            this.m_LiveCreatures = 0;
            List<BaseFish> fish = this.FindItemsByType<BaseFish>();

            foreach (BaseFish f in fish)
            {
                if (!f.Dead)
                    ++this.m_LiveCreatures;
            }
        }

        public void Validate()
        {
            this.RecountLiveCreatures();
        }

        #region Members
        public int FoodNumber()
        {
            if (this.m_Food.State == (int)FoodState.Full)
                return 1074240;

            if (this.m_Food.State == (int)FoodState.Overfed)
                return 1074239;

            return 1074236 + this.m_Food.State;
        }

        public int WaterNumber()
        {
            return 1074242 + this.m_Water.State;
        }

        #endregion

        #region Virtual members
        public virtual void KillFish(int amount)
        {
            List<BaseFish> toKill = new List<BaseFish>();

            for (int i = 0; i < this.Items.Count; i ++)
            {
                if (this.Items[i] is BaseFish)
                {
                    BaseFish fish = (BaseFish)this.Items[i];

                    if (!fish.Dead)
                        toKill.Add(fish);
                }
            }

            while (amount > 0 && toKill.Count > 0)
            {
                int kill = Utility.Random(toKill.Count);

                toKill[kill].Kill();

                toKill.RemoveAt(kill);

                amount -= 1;
                this.m_LiveCreatures -= 1;

                if (this.m_LiveCreatures < 0)
                    this.m_LiveCreatures = 0;

                this.m_Events.Add(1074366); // An unfortunate accident has left a creature floating upside-down.  It is starting to smell.
            }
        }

        public virtual void Evaluate()
        {
            if (this.m_VacationLeft > 0)
            {
                this.m_VacationLeft -= 1;
            }
            else if (this.m_EvaluateDay)
            {
                // reset events
                this.m_Events = new List<int>();

                // food events
                if (
                    (this.m_Food.Added < this.m_Food.Maintain && this.m_Food.State != (int)FoodState.Overfed && this.m_Food.State != (int)FoodState.Dead) ||
                    (this.m_Food.Added >= this.m_Food.Improve && this.m_Food.State == (int)FoodState.Full)
                )
                    this.m_Events.Add(1074368); // The tank looks worse than it did yesterday.

                if (
                    (this.m_Food.Added >= this.m_Food.Improve && this.m_Food.State != (int)FoodState.Full && this.m_Food.State != (int)FoodState.Overfed) ||
                    (this.m_Food.Added < this.m_Food.Maintain && this.m_Food.State == (int)FoodState.Overfed)
                )
                    this.m_Events.Add(1074367); // The tank looks healthier today.

                // water events
                if (this.m_Water.Added < this.m_Water.Maintain && this.m_Water.State != (int)WaterState.Dead)
                    this.m_Events.Add(1074370); // This tank can use more water.

                if (this.m_Water.Added >= this.m_Water.Improve && this.m_Water.State != (int)WaterState.Strong)
                    this.m_Events.Add(1074369); // The water looks clearer today.

                this.UpdateFoodState();
                this.UpdateWaterState();

                // reward
                if (this.m_LiveCreatures > 0)
                    this.m_RewardAvailable = true;
            }
            else
            {
                // new fish
                if (this.OptimalState && this.m_LiveCreatures < this.MaxLiveCreatures)
                {
                    if (Utility.RandomDouble() < 0.005 * this.m_LiveCreatures)
                    {
                        BaseFish fish = null;
                        int message = 0;

                        switch ( Utility.Random(6) )
                        {
                            case 0:
                                {
                                    message = 1074371; // Brine shrimp have hatched overnight in the tank.
                                    fish = new BrineShrimp();
                                    break;
                                }
                            case 1:
                                {
                                    message = 1074365; // A new creature has hatched overnight in the tank.
                                    fish = new Coral();
                                    break;
                                }
                            case 2:
                                {
                                    message = 1074365; // A new creature has hatched overnight in the tank.
                                    fish = new FullMoonFish();
                                    break;
                                }
                            case 3:
                                {
                                    message = 1074373; // A sea horse has hatched overnight in the tank.
                                    fish = new SeaHorseFish();
                                    break;
                                }
                            case 4:
                                {
                                    message = 1074365; // A new creature has hatched overnight in the tank.
                                    fish = new StrippedFlakeFish();
                                    break;
                                }
                            case 5:
                                {
                                    message = 1074365; // A new creature has hatched overnight in the tank.
                                    fish = new StrippedSosarianSwill();
                                    break;
                                }
                        }

                        if (Utility.RandomDouble() < 0.05)
                            fish.Hue = m_FishHues[Utility.Random(m_FishHues.Length)];
                        else if (Utility.RandomDouble() < 0.5)
                            fish.Hue = Utility.RandomMinMax(0x100, 0x3E5);

                        if (this.AddFish(fish))
                            this.m_Events.Add(message);
                        else
                            fish.Delete();
                    }
                }

                // kill fish *grins*
                if (this.m_LiveCreatures < this.MaxLiveCreatures)
                {
                    if (Utility.RandomDouble() < 0.01)
                        this.KillFish(1);
                }
                else
                {
                    this.KillFish(this.m_LiveCreatures - this.MaxLiveCreatures);
                }
            }

            this.m_EvaluateDay = !this.m_EvaluateDay;
            this.InvalidateProperties();
        }

        public virtual void GiveReward(Mobile to)
        {
            if (!this.m_RewardAvailable)
                return;

            int max = (int)(((double)this.m_LiveCreatures / 30) * m_Decorations.Length);

            int random = (max <= 0) ? 0 : Utility.Random(max);

            if (random >= m_Decorations.Length)
                random = m_Decorations.Length - 1;

            Item item;

            try
            {
                item = Activator.CreateInstance(m_Decorations[random]) as Item;
            }
            catch
            {
                return;
            }

            if (item == null)
                return;

            if (!to.PlaceInBackpack(item))
            {
                item.Delete();
                to.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                return;
            }

            to.SendLocalizedMessage(1074360, String.Format("#{0}", item.LabelNumber)); // You receive a reward: ~1_REWARD~
            to.PlaySound(0x5A3);

            this.m_RewardAvailable = false;

            this.InvalidateProperties();
        }

        public virtual void UpdateFoodState()
        {
            if (this.m_Food.Added < this.m_Food.Maintain)
                this.m_Food.State = (this.m_Food.State <= 0) ? 0 : this.m_Food.State - 1;
            else if (this.m_Food.Added >= this.m_Food.Improve)
                this.m_Food.State = (this.m_Food.State >= (int)FoodState.Overfed) ? (int)FoodState.Overfed : this.m_Food.State + 1;

            this.m_Food.Maintain = Utility.Random((int)FoodState.Overfed + 1 - this.m_Food.State, 2);

            if (this.m_Food.State == (int)FoodState.Overfed)
                this.m_Food.Improve = 0;
            else
                this.m_Food.Improve = this.m_Food.Maintain + 2;

            this.m_Food.Added = 0;
        }

        public virtual void UpdateWaterState()
        {
            if (this.m_Water.Added < this.m_Water.Maintain)
                this.m_Water.State = (this.m_Water.State <= 0) ? 0 : this.m_Water.State - 1;
            else if (this.m_Water.Added >= this.m_Water.Improve)
                this.m_Water.State = (this.m_Water.State >= (int)WaterState.Strong) ? (int)WaterState.Strong : this.m_Water.State + 1;

            this.m_Water.Maintain = Utility.Random((int)WaterState.Strong + 2 - this.m_Water.State, 2);

            if (this.m_Water.State == (int)WaterState.Strong)
                this.m_Water.Improve = 0;
            else
                this.m_Water.Improve = this.m_Water.Maintain + 2;

            this.m_Water.Added = 0;
        }

        public virtual bool RemoveItem(Mobile from, int at)
        {
            if (at < 0 || at >= this.Items.Count)
                return false;

            Item item = this.Items[at];

            if (item.IsLockedDown) // for legacy aquariums
            {
                from.SendLocalizedMessage(1010449); // You may not use this object while it is locked down.
                return false;
            }

            if (item is BaseFish)
            {
                BaseFish fish = (BaseFish)item;

                FishBowl bowl;

                if ((bowl = GetEmptyBowl(from)) != null)
                {
                    bowl.AddItem(fish);

                    from.SendLocalizedMessage(1074511); // You put the creature into a fish bowl.
                }
                else
                {
                    if (!from.PlaceInBackpack(fish))
                    {
                        from.SendLocalizedMessage(1074514); // You have no place to put it.
                        return false;
                    }
                    else
                    {
                        from.SendLocalizedMessage(1074512); // You put the gasping creature into your pack.
                    }
                }

                if (!fish.Dead)
                    this.m_LiveCreatures -= 1;
            }
            else
            {
                if (!from.PlaceInBackpack(item))
                {
                    from.SendLocalizedMessage(1074514); // You have no place to put it.
                    return false;
                }
                else
                {
                    from.SendLocalizedMessage(1074513); // You put the item into your pack.
                }
            }

            this.InvalidateProperties();
            return true;
        }

        public virtual void ExamineAquarium(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            from.CloseGump(typeof(AquariumGump));
            from.SendGump(new AquariumGump(this, this.HasAccess(from)));

            from.PlaySound(0x5A4);
        }

        public virtual bool AddFish(BaseFish fish)
        {
            return this.AddFish(null, fish);
        }

        public virtual bool AddFish(Mobile from, BaseFish fish)
        {
            if (fish == null)
                return false;

            if (this.IsFull || this.m_LiveCreatures >= this.MaxLiveCreatures || fish.Dead)
            {
                if (from != null)
                    from.SendLocalizedMessage(1073633); // The aquarium can not hold the creature.

                return false;
            }

            this.AddItem(fish);
            fish.StopTimer();

            this.m_LiveCreatures += 1;

            if (from != null)
                from.SendLocalizedMessage(1073632, String.Format("#{0}", fish.LabelNumber)); // You add the following creature to your aquarium: ~1_FISH~

            this.InvalidateProperties();
            return true;
        }

        public virtual bool AddDecoration(Item item)
        {
            return this.AddDecoration(null, item);
        }

        public virtual bool AddDecoration(Mobile from, Item item)
        {
            if (item == null)
                return false;

            if (this.IsFull)
            {
                if (from != null)
                    from.SendLocalizedMessage(1073636); // The decoration will not fit in the aquarium.

                return false;
            }

            if (!Accepts(item))
            {
                if (from != null)
                    from.SendLocalizedMessage(1073822); // The aquarium can not hold that item.

                return false;
            }

            this.AddItem(item);

            if (from != null)
                from.SendLocalizedMessage(1073635, (item.LabelNumber != 0) ? String.Format("#{0}", item.LabelNumber) : item.Name); // You add the following decoration to your aquarium: ~1_NAME~

            this.InvalidateProperties();
            return true;
        }

        #endregion

        #region Static members
        public static FishBowl GetEmptyBowl(Mobile from)
        {
            if (from == null || from.Backpack == null)
                return null;

            Item[] items = from.Backpack.FindItemsByType(typeof(FishBowl));

            for (int i = 0; i < items.Length; i ++)
            {
                if (items[i] is FishBowl)
                {
                    FishBowl bowl = (FishBowl)items[i];

                    if (bowl.Empty)
                        return bowl;
                }
            }

            return null;
        }

        private static readonly Type[] m_Decorations = new Type[]
        {
            typeof(FishBones),
            typeof(WaterloggedBoots),
            typeof(CaptainBlackheartsFishingPole),
            typeof(CraftysFishingHat),
            typeof(AquariumFishNet),
            typeof(AquariumMessage),
            typeof(IslandStatue),
            typeof(Shell),
            typeof(ToyBoat)
        };

        public static bool Accepts(Item item)
        {
            if (item == null)
                return false;

            Type type = item.GetType();

            for (int i = 0; i < m_Decorations.Length; i ++)
            {
                if (type == m_Decorations[i])
                    return true;
            }

            return false;
        }

        private static readonly int[] m_FishHues = new int[]
        {
            0x1C2, 0x1C3, 0x2A3, 0x47E, 0x51D
        };

        public static int[] FishHues
        {
            get
            {
                return m_FishHues;
            }
        }
        #endregion

        #region Context entries
        private class ExamineEntry : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public ExamineEntry(Aquarium aquarium)
                : base(6235, 2)// Examine Aquarium
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted)
                    return;

                this.m_Aquarium.ExamineAquarium(this.Owner.From);
            }
        }

        private class CollectRewardEntry : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public CollectRewardEntry(Aquarium aquarium)
                : base(6237, 2)// Collect Reward
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted || !this.m_Aquarium.HasAccess(this.Owner.From))
                    return;

                this.m_Aquarium.GiveReward(this.Owner.From);
            }
        }

        private class ViewEventEntry : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public ViewEventEntry(Aquarium aquarium)
                : base(6239, 2)// View events
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted || !this.m_Aquarium.HasAccess(this.Owner.From) || this.m_Aquarium.Events.Count == 0)
                    return;

                this.Owner.From.SendLocalizedMessage(this.m_Aquarium.Events[0]);

                if (this.m_Aquarium.Events[0] == 1074366)
                    this.Owner.From.PlaySound(0x5A2);

                this.m_Aquarium.Events.RemoveAt(0);
                this.m_Aquarium.InvalidateProperties();
            }
        }

        private class CancelVacationMode : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public CancelVacationMode(Aquarium aquarium)
                : base(6240, 2)// Cancel vacation mode
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted || !this.m_Aquarium.HasAccess(this.Owner.From))
                    return;

                this.Owner.From.SendLocalizedMessage(1074429); // Vacation mode has been cancelled.
                this.m_Aquarium.VacationLeft = 0;
                this.m_Aquarium.InvalidateProperties();
            }
        }

        // GM context entries
        private class GMAddFood : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMAddFood(Aquarium aquarium)
                : base(6231, -1)// GM Add Food
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted)
                    return;

                this.m_Aquarium.Food.Added += 1;
                this.m_Aquarium.InvalidateProperties();
            }
        }

        private class GMAddWater : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMAddWater(Aquarium aquarium)
                : base(6232, -1)// GM Add Water
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted)
                    return;

                this.m_Aquarium.Water.Added += 1;
                this.m_Aquarium.InvalidateProperties();
            }
        }

        private class GMForceEvaluate : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMForceEvaluate(Aquarium aquarium)
                : base(6233, -1)// GM Force Evaluate
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted)
                    return;

                this.m_Aquarium.Evaluate();
            }
        }

        private class GMOpen : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMOpen(Aquarium aquarium)
                : base(6234, -1)// GM Open Container
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted)
                    return;

                this.Owner.From.SendGump(new AquariumGump(this.m_Aquarium, true));
            }
        }

        private class GMFill : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMFill(Aquarium aquarium)
                : base(6236, -1)// GM Fill Food and Water
            {
                this.m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (this.m_Aquarium.Deleted)
                    return;

                this.m_Aquarium.Food.Added = this.m_Aquarium.Food.Maintain;
                this.m_Aquarium.Water.Added = this.m_Aquarium.Water.Maintain;
                this.m_Aquarium.InvalidateProperties();
            }
        }
        #endregion
    }

    public class AquariumEastDeed : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon
        {
            get
            {
                return new Aquarium(0x3062);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074501;
            }
        }// Large Aquarium (east)

        [Constructable]
        public AquariumEastDeed()
            : base()
        {
        }

        public AquariumEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class AquariumNorthDeed : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon
        {
            get
            {
                return new Aquarium(0x3060);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074497;
            }
        }// Large Aquarium (north)

        [Constructable]
        public AquariumNorthDeed()
            : base()
        {
        }

        public AquariumNorthDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}