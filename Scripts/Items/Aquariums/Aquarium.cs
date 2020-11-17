using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class AquariumAddonComponent : AddonContainerComponent
    {
        public override int LabelNumber => 1125918;  // aquarium

        public AquariumAddonComponent(int id)
            : base(id)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            Addon.GetProperties(list);
        }

        public AquariumAddonComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadEncodedInt();
        }
    }

    public class Aquarium : BaseAddonContainer
    {
        public override int LabelNumber => 1125918;  // aquarium

        public static readonly TimeSpan EvaluationInterval = TimeSpan.FromDays(1);

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiveCreatures { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DeadCreatures
        {
            get
            {
                int dead = 0;

                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] is BaseFish)
                    {
                        BaseFish fish = (BaseFish)Items[i];

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
                int state = (m_Food.State == (int)FoodState.Overfed) ? 1 : (int)FoodState.Full - m_Food.State;

                state += (int)WaterState.Strong - m_Water.State;

                state = (int)Math.Pow(state, 1.75);

                return MaxItems - state;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsFull => (Items.Count >= MaxItems);

        // vacation info
        private int m_VacationLeft;

        [CommandProperty(AccessLevel.GameMaster)]
        public int VacationLeft
        {
            get { return m_VacationLeft; }
            set
            {
                m_VacationLeft = value;
                InvalidateProperties();
            }
        }

        // aquarium state
        private AquariumState m_Food;

        [CommandProperty(AccessLevel.GameMaster)]
        public AquariumState Food
        {
            get { return m_Food; }
            set
            {
                m_Food = value;
                InvalidateProperties();
            }
        }

        private AquariumState m_Water;

        [CommandProperty(AccessLevel.GameMaster)]
        public AquariumState Water
        {
            get { return m_Water; }
            set
            {
                m_Water = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool OptimalState => (m_Food.State == (int)FoodState.Full && m_Water.State == (int)WaterState.Strong);

        private bool m_EvaluateDay;

        public List<int> Events { get; private set; }

        private bool m_RewardAvailable;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RewardAvailable
        {
            get { return m_RewardAvailable; }
            set
            {
                m_RewardAvailable = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextEvaluate { get; set; }

        public override BaseAddonContainerDeed Deed => null;

        public override void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsOwner(from))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.         

                AddonContainerBarrel barrel;

                if (ItemID == 0x3062)
                    barrel = new AquariumEastBarrel();
                else if (ItemID == 0x3060)
                    barrel = new AquariumNorthBarrel();
                else if (ItemID == 0xA3A6)
                    barrel = new SmallElegantAquariumBarrel();
                else if (ItemID == 41909 || ItemID == 41924)
                    barrel = new WallMountedAquariumBarrel();
                else
                    barrel = new LargeElegantAquariumBarrel();

                for (int i = 0; i < Items.Count; i++)
                {
                    barrel.DropItem(Items[i]);
                }

                if (barrel != null)
                {
                    from.AddToBackpack(barrel);
                }

                Delete();

                house.Addons.Remove(this);
            }
        }

        public override double DefaultWeight => 10.0;
        public override bool DisplaysContent => true;

        public Aquarium(int itemID)
            : base(itemID)
        {
            Movable = false;

            if (itemID == 0x3060)
                AddComponent(new AquariumAddonComponent(0x3061), -1, 0, 0);

            if (itemID == 0x3062)
                AddComponent(new AquariumAddonComponent(0x3063), 0, -1, 0);

            if (itemID == 41909)
            {
                AddComponent(new AquariumAddonComponent(41904), 1, 0, 0);
                AddComponent(new AquariumAddonComponent(41914), -1, 0, 0);
            }

            if (itemID == 41924)
            {
                AddComponent(new AquariumAddonComponent(41919), 0, 1, 0);
                AddComponent(new AquariumAddonComponent(41929), 0, -1, 0);
            }

            if (itemID == 41934)
            {
                AddComponent(new AquariumAddonComponent(41940), -1, 0, 0);
                AddComponent(new AquariumAddonComponent(41941), -2, 0, 0);
                AddComponent(new AquariumAddonComponent(41939), 0, -1, 0);
            }

            if (itemID == 41942)
            {
                AddComponent(new AquariumAddonComponent(41947), -1, 0, 0);
                AddComponent(new AquariumAddonComponent(41948), 0, -1, 0);
                AddComponent(new AquariumAddonComponent(41949), 0, -2, 0);
            }

            MaxItems = 30;

            m_Food = new AquariumState();
            m_Water = new AquariumState();

            m_Food.State = (int)FoodState.Full;
            m_Water.State = (int)WaterState.Strong;

            m_Food.Maintain = Utility.RandomMinMax(1, 2);
            m_Food.Improve = m_Food.Maintain + Utility.RandomMinMax(1, 2);

            m_Water.Maintain = Utility.RandomMinMax(1, 3);

            Events = new List<int>();
            NextEvaluate = DateTime.UtcNow + EvaluationInterval;

            EventTimer.AddTimer(this);
        }

        public Aquarium(Serial serial)
            : base(serial)
        {
        }

        public override void Delete()
        {
            base.Delete();

            EventTimer.RemoveTimer(this);
        }

        public override void OnDoubleClick(Mobile from)
        {
            ExamineAquarium(from);
        }

        public virtual bool HasAccess(Mobile from)
        {
            if (from == null || from.Deleted)
                return false;
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            return (house != null && house.IsCoOwner(from));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!HasAccess(from))
            {
                from.SendLocalizedMessage(1073821); // You do not have access to that item for use with the aquarium.
                return false;
            }

            if (m_VacationLeft > 0)
            {
                from.SendLocalizedMessage(1074427); // The aquarium is in vacation mode.
                return false;
            }

            bool takeItem = true;

            if (dropped is FishBowl)
            {
                FishBowl bowl = (FishBowl)dropped;

                if (bowl.Empty || !AddFish(from, bowl.Fish))
                    return false;

                bowl.InvalidateProperties();

                takeItem = false;
            }
            else if (dropped is BaseFish)
            {
                BaseFish fish = (BaseFish)dropped;

                if (!AddFish(from, fish))
                    return false;
            }
            else if (dropped is VacationWafer)
            {
                m_VacationLeft = VacationWafer.VacationDays;
                dropped.Delete();

                from.SendLocalizedMessage(1074428, m_VacationLeft.ToString()); // The aquarium will be in vacation mode for ~1_DAYS~ days
            }
            else if (dropped is AquariumFood)
            {
                m_Food.Added += 1;
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

                m_Water.Added += 1;
                beverage.Quantity -= 1;

                from.PlaySound(0x4E);
                from.SendLocalizedMessage(1074260, "1"); // ~1_NUM~ unit(s) of water have been added to the aquarium.

                takeItem = false;
            }
            else if (!AddDecoration(from, dropped))
            {
                takeItem = false;
            }

            from.CloseGump(typeof(AquariumGump));

            InvalidateProperties();

            if (takeItem)
                from.PlaySound(0x42);

            return takeItem;
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

            if (m_VacationLeft > 0)
                list.Add(1074430, m_VacationLeft.ToString()); // Vacation days left: ~1_DAYS

            if (Events.Count > 0)
                list.Add(1074426, Events.Count.ToString()); // ~1_NUM~ event(s) to view!

            if (m_RewardAvailable)
                list.Add(1074362); // A reward is available!

            list.Add(1074247, "{0}\t{1}", LiveCreatures, MaxLiveCreatures); // Live Creatures: ~1_NUM~ / ~2_MAX~

            int dead = DeadCreatures;

            if (dead > 0)
                list.Add(1074248, dead.ToString()); // Dead Creatures: ~1_NUM~

            int decorations = Items.Count - LiveCreatures - dead;

            if (decorations > 0)
                list.Add(1074249, decorations.ToString()); // Decorations: ~1_NUM~

            list.Add(1074250, "#{0}", FoodNumber()); // Food state: ~1_STATE~
            list.Add(1074251, "#{0}", WaterNumber()); // Water state: ~1_STATE~

            if (m_Food.State == (int)FoodState.Dead)
                list.Add(1074577, "{0}\t{1}", m_Food.Added, m_Food.Improve); // Food Added: ~1_CUR~ Needed: ~2_NEED~
            else if (m_Food.State == (int)FoodState.Overfed)
                list.Add(1074577, "{0}\t{1}", m_Food.Added, m_Food.Maintain); // Food Added: ~1_CUR~ Needed: ~2_NEED~
            else
                list.Add(1074253, "{0}\t{1}\t{2}", m_Food.Added, m_Food.Maintain, m_Food.Improve); // Food Added: ~1_CUR~ Feed: ~2_NEED~ Improve: ~3_GROW~

            if (m_Water.State == (int)WaterState.Dead)
                list.Add(1074578, "{0}\t{1}", m_Water.Added, m_Water.Improve); // Water Added: ~1_CUR~ Needed: ~2_NEED~
            else if (m_Water.State == (int)WaterState.Strong)
                list.Add(1074578, "{0}\t{1}", m_Water.Added, m_Water.Maintain); // Water Added: ~1_CUR~ Needed: ~2_NEED~
            else
                list.Add(1074254, "{0}\t{1}\t{2}", m_Water.Added, m_Water.Maintain, m_Water.Improve); // Water Added: ~1_CUR~ Maintain: ~2_NEED~ Improve: ~3_GROW~
        }

        public override bool DisplayWeight => false;

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
            {
                list.Add(new ExamineEntry(this));

                if (HasAccess(from))
                {
                    if (m_RewardAvailable)
                        list.Add(new CollectRewardEntry(this));

                    if (Events.Count > 0)
                        list.Add(new ViewEventEntry(this));

                    if (m_VacationLeft > 0)
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

            writer.Write(4); // Version

            // version 4
            writer.Write(m_EvaluateDay);
            writer.Write(NextEvaluate);

            // version 0
            writer.Write(LiveCreatures);
            writer.Write(m_VacationLeft);

            m_Food.Serialize(writer);
            m_Water.Serialize(writer);

            writer.Write(Events.Count);

            for (int i = 0; i < Events.Count; i++)
                writer.Write(Events[i]);

            writer.Write(m_RewardAvailable);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    m_EvaluateDay = reader.ReadBool();
                    NextEvaluate = reader.ReadDateTime();
                    goto case 0;
                case 3:
                case 2:
                case 1:
                    {
                        if (version < 4)
                        {
                            reader.ReadDateTime();

                            NextEvaluate = DateTime.UtcNow + TimeSpan.FromMinutes(5);
                        }

                        goto case 0;
                    }
                case 0:
                    {
                        LiveCreatures = reader.ReadInt();
                        m_VacationLeft = reader.ReadInt();

                        m_Food = new AquariumState();
                        m_Water = new AquariumState();

                        m_Food.Deserialize(reader);
                        m_Water.Deserialize(reader);

                        Events = new List<int>();

                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                            Events.Add(reader.ReadInt());

                        m_RewardAvailable = reader.ReadBool();

                        break;
                    }
            }

            if (version < 2)
            {
                Weight = DefaultWeight;
                Movable = false;
            }

            if (version < 3)
                ValidationQueue<Aquarium>.Add(this);

            EventTimer.AddTimer(this);
        }

        private void RecountLiveCreatures()
        {
            LiveCreatures = 0;
            List<BaseFish> fish = FindItemsByType<BaseFish>();

            foreach (BaseFish f in fish)
            {
                if (!f.Dead)
                    ++LiveCreatures;
            }
        }

        public void Validate()
        {
            RecountLiveCreatures();
        }

        #region Members
        public int FoodNumber()
        {
            if (m_Food.State == (int)FoodState.Full)
                return 1074240;

            if (m_Food.State == (int)FoodState.Overfed)
                return 1074239;

            return 1074236 + m_Food.State;
        }

        public int WaterNumber()
        {
            return 1074242 + m_Water.State;
        }

        #endregion

        #region Virtual members
        public virtual void KillFish(int amount)
        {
            List<BaseFish> toKill = new List<BaseFish>();

            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is BaseFish)
                {
                    BaseFish fish = (BaseFish)Items[i];

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
                LiveCreatures -= 1;

                if (LiveCreatures < 0)
                    LiveCreatures = 0;

                Events.Add(1074366); // An unfortunate accident has left a creature floating upside-down.  It is starting to smell.
            }
        }

        public virtual void Evaluate()
        {
            NextEvaluate = DateTime.UtcNow + EvaluationInterval;

            if (m_VacationLeft > 0)
            {
                m_VacationLeft -= 1;
            }
            else if (m_EvaluateDay)
            {
                // reset events
                Events = new List<int>();

                // food events
                if (
                    (m_Food.Added < m_Food.Maintain && m_Food.State != (int)FoodState.Overfed && m_Food.State != (int)FoodState.Dead) ||
                    (m_Food.Added >= m_Food.Improve && m_Food.State == (int)FoodState.Full)
                )
                    Events.Add(1074368); // The tank looks worse than it did yesterday.

                if (
                    (m_Food.Added >= m_Food.Improve && m_Food.State != (int)FoodState.Full && m_Food.State != (int)FoodState.Overfed) ||
                    (m_Food.Added < m_Food.Maintain && m_Food.State == (int)FoodState.Overfed)
                )
                    Events.Add(1074367); // The tank looks healthier today.

                // water events
                if (m_Water.Added < m_Water.Maintain && m_Water.State != (int)WaterState.Dead)
                    Events.Add(1074370); // This tank can use more water.

                if (m_Water.Added >= m_Water.Improve && m_Water.State != (int)WaterState.Strong)
                    Events.Add(1074369); // The water looks clearer today.

                UpdateFoodState();
                UpdateWaterState();

                // reward
                if (LiveCreatures > 0)
                    m_RewardAvailable = true;
            }
            else
            {
                // new fish
                if (OptimalState && LiveCreatures < MaxLiveCreatures)
                {
                    if (Utility.RandomDouble() < 0.005 * LiveCreatures)
                    {
                        BaseFish fish = null;
                        int message = 0;

                        switch (Utility.Random(6))
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

                        if (AddFish(fish))
                            Events.Add(message);
                        else
                            fish.Delete();
                    }
                }

                // kill fish *grins*
                if (LiveCreatures < MaxLiveCreatures)
                {
                    if (Utility.RandomDouble() < 0.01)
                        KillFish(1);
                }
                else
                {
                    KillFish(LiveCreatures - MaxLiveCreatures);
                }
            }

            m_EvaluateDay = !m_EvaluateDay;
            InvalidateProperties();
        }

        public virtual void GiveReward(Mobile to)
        {
            if (!m_RewardAvailable)
                return;

            int max = (int)(((double)LiveCreatures / 30) * m_Decorations.Length);

            int random = (max <= 0) ? 0 : Utility.Random(max);

            if (random >= m_Decorations.Length)
                random = m_Decorations.Length - 1;

            Item item = null;

            try
            {
                item = Activator.CreateInstance(m_Decorations[random]) as Item;
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }

            if (item == null)
                return;

            if (!to.PlaceInBackpack(item))
            {
                item.Delete();
                to.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                return;
            }

            to.SendLocalizedMessage(1074360, string.Format("#{0}", item.LabelNumber)); // You receive a reward: ~1_REWARD~
            to.PlaySound(0x5A3);

            m_RewardAvailable = false;

            InvalidateProperties();
        }

        public virtual void UpdateFoodState()
        {
            if (m_Food.Added < m_Food.Maintain)
                m_Food.State = (m_Food.State <= 0) ? 0 : m_Food.State - 1;
            else if (m_Food.Added >= m_Food.Improve)
                m_Food.State = (m_Food.State >= (int)FoodState.Overfed) ? (int)FoodState.Overfed : m_Food.State + 1;

            m_Food.Maintain = Utility.Random((int)FoodState.Overfed + 1 - m_Food.State, 2);

            if (m_Food.State == (int)FoodState.Overfed)
                m_Food.Improve = 0;
            else
                m_Food.Improve = m_Food.Maintain + 2;

            m_Food.Added = 0;
        }

        public virtual void UpdateWaterState()
        {
            if (m_Water.Added < m_Water.Maintain)
                m_Water.State = (m_Water.State <= 0) ? 0 : m_Water.State - 1;
            else if (m_Water.Added >= m_Water.Improve)
                m_Water.State = (m_Water.State >= (int)WaterState.Strong) ? (int)WaterState.Strong : m_Water.State + 1;

            m_Water.Maintain = Utility.Random((int)WaterState.Strong + 2 - m_Water.State, 2);

            if (m_Water.State == (int)WaterState.Strong)
                m_Water.Improve = 0;
            else
                m_Water.Improve = m_Water.Maintain + 2;

            m_Water.Added = 0;
        }

        public virtual bool RemoveItem(Mobile from, int at)
        {
            if (at < 0 || at >= Items.Count)
                return false;

            Item item = Items[at];

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

                    from.SendLocalizedMessage(1074512); // You put the gasping creature into your pack.
                }

                if (!fish.Dead)
                    LiveCreatures -= 1;
            }
            else
            {
                if (!from.PlaceInBackpack(item))
                {
                    from.SendLocalizedMessage(1074514); // You have no place to put it.
                    return false;
                }

                from.SendLocalizedMessage(1074513); // You put the item into your pack.
            }

            InvalidateProperties();
            return true;
        }

        public virtual void ExamineAquarium(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            from.CloseGump(typeof(AquariumGump));
            from.SendGump(new AquariumGump(this, HasAccess(from)));

            from.PlaySound(0x5A4);
        }

        public virtual bool AddFish(BaseFish fish)
        {
            return AddFish(null, fish);
        }

        public virtual bool AddFish(Mobile from, BaseFish fish)
        {
            if (fish == null)
                return false;

            if (IsFull || LiveCreatures >= MaxLiveCreatures || fish.Dead)
            {
                if (from != null)
                    from.SendLocalizedMessage(1073633); // The aquarium can not hold the creature.

                return false;
            }

            AddItem(fish);
            fish.StopTimer();

            LiveCreatures += 1;

            if (from != null)
                from.SendLocalizedMessage(1073632, string.Format("#{0}", fish.LabelNumber)); // You add the following creature to your aquarium: ~1_FISH~

            InvalidateProperties();
            return true;
        }

        public virtual bool AddDecoration(Item item)
        {
            return AddDecoration(null, item);
        }

        public virtual bool AddDecoration(Mobile from, Item item)
        {
            if (item == null)
                return false;

            if (IsFull)
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

            AddItem(item);

            if (from != null)
                from.SendLocalizedMessage(1073635, (item.LabelNumber != 0) ? string.Format("#{0}", item.LabelNumber) : item.Name); // You add the following decoration to your aquarium: ~1_NAME~

            InvalidateProperties();
            return true;
        }

        #endregion

        #region Static members
        public static FishBowl GetEmptyBowl(Mobile from)
        {
            if (from == null || from.Backpack == null)
                return null;

            Item[] items = from.Backpack.FindItemsByType(typeof(FishBowl));

            for (int i = 0; i < items.Length; i++)
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

        private static readonly Type[] m_Decorations =
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

            for (int i = 0; i < m_Decorations.Length; i++)
            {
                if (type == m_Decorations[i])
                    return true;
            }

            return false;
        }

        private static readonly int[] m_FishHues =
        {
            0x1C2, 0x1C3, 0x2A3, 0x47E, 0x51D
        };

        public static int[] FishHues => m_FishHues;
        #endregion

        #region Context entries
        private class ExamineEntry : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public ExamineEntry(Aquarium aquarium)
                : base(6235, 2)// Examine Aquarium
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted)
                    return;

                m_Aquarium.ExamineAquarium(Owner.From);
            }
        }

        private class CollectRewardEntry : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public CollectRewardEntry(Aquarium aquarium)
                : base(6237, 2)// Collect Reward
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted || !m_Aquarium.HasAccess(Owner.From))
                    return;

                m_Aquarium.GiveReward(Owner.From);
            }
        }

        private class ViewEventEntry : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public ViewEventEntry(Aquarium aquarium)
                : base(6239, 2)// View events
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted || !m_Aquarium.HasAccess(Owner.From) || m_Aquarium.Events.Count == 0)
                    return;

                Owner.From.SendLocalizedMessage(m_Aquarium.Events[0]);

                if (m_Aquarium.Events[0] == 1074366)
                    Owner.From.PlaySound(0x5A2);

                m_Aquarium.Events.RemoveAt(0);
                m_Aquarium.InvalidateProperties();
            }
        }

        private class CancelVacationMode : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public CancelVacationMode(Aquarium aquarium)
                : base(6240, 2)// Cancel vacation mode
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted || !m_Aquarium.HasAccess(Owner.From))
                    return;

                Owner.From.SendLocalizedMessage(1074429); // Vacation mode has been cancelled.
                m_Aquarium.VacationLeft = 0;
                m_Aquarium.InvalidateProperties();
            }
        }

        // GM context entries
        private class GMAddFood : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMAddFood(Aquarium aquarium)
                : base(6231, -1)// GM Add Food
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted)
                    return;

                m_Aquarium.Food.Added += 1;
                m_Aquarium.InvalidateProperties();
            }
        }

        private class GMAddWater : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMAddWater(Aquarium aquarium)
                : base(6232, -1)// GM Add Water
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted)
                    return;

                m_Aquarium.Water.Added += 1;
                m_Aquarium.InvalidateProperties();
            }
        }

        private class GMForceEvaluate : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMForceEvaluate(Aquarium aquarium)
                : base(6233, -1)// GM Force Evaluate
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted)
                    return;

                m_Aquarium.Evaluate();
            }
        }

        private class GMOpen : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMOpen(Aquarium aquarium)
                : base(6234, -1)// GM Open Container
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted)
                    return;

                Owner.From.SendGump(new AquariumGump(m_Aquarium, true));
            }
        }

        private class GMFill : ContextMenuEntry
        {
            private readonly Aquarium m_Aquarium;

            public GMFill(Aquarium aquarium)
                : base(6236, -1)// GM Fill Food and Water
            {
                m_Aquarium = aquarium;
            }

            public override void OnClick()
            {
                if (m_Aquarium.Deleted)
                    return;

                m_Aquarium.Food.Added = m_Aquarium.Food.Maintain;
                m_Aquarium.Water.Added = m_Aquarium.Water.Maintain;
                m_Aquarium.InvalidateProperties();
            }
        }
        #endregion
    }

    public class AquariumEastDeed : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon => new Aquarium(0x3062);
        public override int LabelNumber => 1074501;// Large Aquarium (east)

        [Constructable]
        public AquariumEastDeed()
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
            reader.ReadInt();
        }
    }

    public class AquariumEastBarrel : AddonContainerBarrel
    {
        public override BaseAddonContainer Addon => new Aquarium(0x3062);
        public override int LabelNumber => 1074501;// Large Aquarium (east)

        [Constructable]
        public AquariumEastBarrel()
        {
        }

        public AquariumEastBarrel(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class AquariumNorthDeed : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon => new Aquarium(0x3060);
        public override int LabelNumber => 1074497;// Large Aquarium (north)

        [Constructable]
        public AquariumNorthDeed()
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
            reader.ReadInt();
        }
    }

    public class AquariumNorthBarrel : AddonContainerBarrel
    {
        public override BaseAddonContainer Addon => new Aquarium(0x3060);
        public override int LabelNumber => 1074497;// Large Aquarium (north)

        [Constructable]
        public AquariumNorthBarrel()
        {
        }

        public AquariumNorthBarrel(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class SmallElegantAquariumDeed : BaseAddonContainerDeed
    {
        public override BaseAddonContainer Addon => new Aquarium(0xA3A6);
        public override int LabelNumber => 1159134; // small elegant aquarium

        [Constructable]
        public SmallElegantAquariumDeed()
        {
        }

        public SmallElegantAquariumDeed(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class SmallElegantAquariumBarrel : AddonContainerBarrel
    {
        public override BaseAddonContainer Addon => new Aquarium(0xA3A6);
        public override int LabelNumber => 1159134; // small elegant aquarium

        [Constructable]
        public SmallElegantAquariumBarrel()
        {
        }

        public SmallElegantAquariumBarrel(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class WallMountedAquariumDeed : BaseAddonContainerDeed, IRewardOption
    {
        public override BaseAddonContainer Addon => new Aquarium(_Direction == DirectionType.South ? 41909 : 41924);
        public override int LabelNumber => 1159135; // wall mounted aquarium

        private DirectionType _Direction;

        [Constructable]
        public WallMountedAquariumDeed()
        {
        }

        public WallMountedAquariumDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class WallMountedAquariumBarrel : AddonContainerBarrel, IRewardOption
    {
        public override BaseAddonContainer Addon => new Aquarium(_Direction == DirectionType.South ? 41909 : 41924);
        public override int LabelNumber => 1159135; // wall mounted aquarium

        private DirectionType _Direction;

        [Constructable]
        public WallMountedAquariumBarrel()
        {
        }

        public WallMountedAquariumBarrel(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class LargeElegantAquariumDeed : BaseAddonContainerDeed, IRewardOption
    {
        public override BaseAddonContainer Addon => new Aquarium(_Direction == DirectionType.South ? 41934 : 41942);
        public override int LabelNumber => 1159136; // large elegant aquarium

        private DirectionType _Direction;

        [Constructable]
        public LargeElegantAquariumDeed()
        {
        }

        public LargeElegantAquariumDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class LargeElegantAquariumBarrel : AddonContainerBarrel, IRewardOption
    {
        public override BaseAddonContainer Addon => new Aquarium(_Direction == DirectionType.South ? 41934 : 41942);
        public override int LabelNumber => 1159136; // large elegant aquarium

        private DirectionType _Direction;

        [Constructable]
        public LargeElegantAquariumBarrel()
        {
        }

        public LargeElegantAquariumBarrel(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public abstract class AddonContainerBarrel : Container
    {
        public abstract BaseAddonContainer Addon { get; }

        public override bool DisplaysContent => false;

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return false;
        }

        public AddonContainerBarrel()
            : base(0xFAE)
        {
            Weight = 2;
        }

        public AddonContainerBarrel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1153883, "0");
        }

        private class InternalTarget : Target
        {
            private readonly AddonContainerBarrel _Barrel;

            public InternalTarget(AddonContainerBarrel barrel)
                : base(-1, true, TargetFlags.None)
            {
                _Barrel = barrel;

                CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                Map map = from.Map;

                if (p == null || map == null || _Barrel.Deleted)
                    return;

                if (_Barrel.IsChildOf(from.Backpack))
                {
                    var addon = _Barrel.Addon as Aquarium;

                    Spells.SpellHelper.GetSurfaceTop(ref p);

                    BaseHouse house = null;

                    AddonFitResult res = addon.CouldFit(p, map, from, ref house);

                    if (res == AddonFitResult.Valid)
                        addon.MoveToWorld(new Point3D(p), map);
                    else if (res == AddonFitResult.Blocked)
                        from.SendLocalizedMessage(500269); // You cannot build that there.
                    else if (res == AddonFitResult.NotInHouse)
                        from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                    else if (res == AddonFitResult.DoorTooClose)
                        from.SendLocalizedMessage(500271); // You cannot build near the door.
                    else if (res == AddonFitResult.NoWall)
                        from.SendLocalizedMessage(500268); // This object needs to be mounted on something.

                    if (res == AddonFitResult.Valid)
                    {
                        house.Addons[addon] = from;

                        if (addon.Security)
                        {
                            house.AddSecure(from, addon);
                        }

                        for (int i = 0; i < _Barrel.Items.Count; i++)
                        {
                            var item = _Barrel.Items[i];

                            if (item is BaseFish bf)
                            {
                                addon.AddFish(bf);
                            }
                            else
                            {
                                addon.AddDecoration(item);
                            }
                        }

                        _Barrel.Delete();
                    }
                    else
                    {
                        addon.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                }
            }
        }
    }

    public class EventTimer : Timer
    {
        public static List<Aquarium> Aquariums { get; } = new List<Aquarium>();

        public static EventTimer Instance { get; set; }

        public static void AddTimer(Aquarium aq)
        {
            if (!Aquariums.Contains(aq))
            {
                Aquariums.Add(aq);
            }

            if (Instance == null)
            {
                Instance = new EventTimer();
                Instance.Start();
            }
        }

        public static void RemoveTimer(Aquarium aq)
        {
            if (Aquariums.Contains(aq))
            {
                Aquariums.Remove(aq);
            }

            if (Aquariums.Count == 0 && Instance != null)
            {
                Instance.Stop();
                Instance = null;
            }
        }

        public EventTimer()
            : base(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        {
        }

        protected override void OnTick()
        {
            List<Aquarium> list = Aquariums.Where(a => a.NextEvaluate <= DateTime.UtcNow).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Evaluate();
            }

            ColUtility.Free(list);
        }
    }
}
