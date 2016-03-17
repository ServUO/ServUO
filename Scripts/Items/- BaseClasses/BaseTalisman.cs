using System;
using Server.Commands;
using Server.Mobiles;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Spells.Second;
using Server.Targeting;

namespace Server.Items
{
    public enum TalismanRemoval
    {
        None = 0,
        Ward = 390,
        Damage = 404,
        Curse = 407,
        Wildfire = 2843
    }

    public class BaseTalisman : Item, IWearableDurability
    {
        public static void Initialize()
        {
            CommandSystem.Register("RandomTalisman", AccessLevel.GameMaster, new CommandEventHandler(RandomTalisman_OnCommand));
        }

        [Usage("RandomTalisman <count>")]
        [Description("Generates random talismans in your backback.")]
        public static void RandomTalisman_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            int count = e.GetInt32(0);

            for (int i = 0; i < count; i++)
            {
                m.AddToBackpack(Loot.RandomTalisman());
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1071023;
            }
        }// Talisman
        public virtual bool ForceShowName
        {
            get
            {
                return false;
            }
        }// used to override default summoner/removal name

        private int m_MaxHitPoints;
        private int m_HitPoints;

        //private readonly int m_KarmaLoss;
        private int m_MaxCharges;
        private int m_Charges;
        private int m_MaxChargeTime;
        private int m_ChargeTime;
        private bool m_Blessed;

        [CommandProperty(AccessLevel.GameMaster)]
        public int KarmaLoss
        {
            get
            {
                return this.m_AosAttributes.IncreasedKarmaLoss;
            }
            set
            {
                this.m_AosAttributes.IncreasedKarmaLoss = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get
            {
                return this.m_MaxCharges;
            }
            set
            {
                this.m_MaxCharges = value;
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

                if (this.m_ChargeTime > 0)
                    this.StartTimer();

                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxChargeTime
        {
            get
            {
                return this.m_MaxChargeTime;
            }
            set
            {
                this.m_MaxChargeTime = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ChargeTime
        {
            get
            {
                return this.m_ChargeTime;
            }
            set
            {
                this.m_ChargeTime = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Blessed
        {
            get
            {
                return this.m_Blessed;
            }
            set
            {
                this.m_Blessed = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get
            {
                return this.m_MaxHitPoints;
            }
            set
            {
                this.m_MaxHitPoints = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get
            {
                return this.m_HitPoints;
            }
            set
            {
                if (value != this.m_HitPoints && this.MaxHitPoints > 0)
                {
                    this.m_HitPoints = value;

                    if (this.m_HitPoints < 0)
                        this.Delete();
                    else if (this.m_HitPoints > this.MaxHitPoints)
                        this.m_HitPoints = this.MaxHitPoints;

                    this.InvalidateProperties();
                }
            }
        }

        public virtual int InitMinHits
        {
            get
            {
                return 0;
            }
        }

        public virtual int InitMaxHits
        {
            get
            {
                return 0;
            }
        }

        public virtual bool CanRepair { get { return this is Server.Engines.Craft.IRepairable; } }
        public virtual bool CanFortify { get { return CanRepair; } }

        #region Slayer
        private TalismanSlayerName m_Slayer;

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanSlayerName Slayer
        {
            get
            {
                return this.m_Slayer;
            }
            set
            {
                this.m_Slayer = value;
                this.InvalidateProperties();
            }
        }
        #endregion

        #region Summoner/Removal
        private TalismanAttribute m_Summoner;
        private TalismanRemoval m_Removal;
        private Mobile m_Creature;

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanAttribute Summoner
        {
            get
            {
                return this.m_Summoner;
            }
            set
            {
                this.m_Summoner = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanRemoval Removal
        {
            get
            {
                return this.m_Removal;
            }
            set
            {
                this.m_Removal = value;
                this.InvalidateProperties();
            }
        }
        #endregion

        #region Protection/Killer
        private TalismanAttribute m_Protection;
        private TalismanAttribute m_Killer;

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanAttribute Protection
        {
            get
            {
                return this.m_Protection;
            }
            set
            {
                this.m_Protection = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanAttribute Killer
        {
            get
            {
                return this.m_Killer;
            }
            set
            {
                this.m_Killer = value;
                this.InvalidateProperties();
            }
        }
        #endregion

        #region Craft bonuses
        private SkillName m_Skill;
        private int m_SuccessBonus;
        private int m_ExceptionalBonus;

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get
            {
                return this.m_Skill;
            }
            set
            {
                this.m_Skill = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SuccessBonus
        {
            get
            {
                return this.m_SuccessBonus;
            }
            set
            {
                this.m_SuccessBonus = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExceptionalBonus
        {
            get
            {
                return this.m_ExceptionalBonus;
            }
            set
            {
                this.m_ExceptionalBonus = value;
                this.InvalidateProperties();
            }
        }
        #endregion

        #region AOS bonuses
        private AosAttributes m_AosAttributes;
        private AosSkillBonuses m_AosSkillBonuses;

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes
        {
            get
            {
                return this.m_AosAttributes;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SkillBonuses
        {
            get
            {
                return this.m_AosSkillBonuses;
            }
            set
            {
            }
        }
        #endregion

        public BaseTalisman()
            : this(GetRandomItemID())
        {
        }

        public BaseTalisman(int itemID)
            : base(itemID)
        {
            this.Layer = Layer.Talisman;
            this.Weight = 1.0;

            this.m_HitPoints = this.m_MaxHitPoints = Utility.RandomMinMax(this.InitMinHits, this.InitMaxHits);

            this.m_Protection = new TalismanAttribute();
            this.m_Killer = new TalismanAttribute();
            this.m_Summoner = new TalismanAttribute();
            this.m_AosAttributes = new AosAttributes(this);
            this.m_AosSkillBonuses = new AosSkillBonuses(this);
        }

        public BaseTalisman(Serial serial)
            : base(serial)
        {
        }

        public virtual int OnHit(BaseWeapon weap, int damage)
        {
            if (m_MaxHitPoints == 0)
                return damage;

            if (25 > Utility.Random(100)) // 25% chance to lower durability
            {
                int wear = Utility.Random(2);

                if (wear > 0)
                {
                    if (m_HitPoints >= wear)
                    {
                        HitPoints -= wear;
                        wear = 0;
                    }
                    else
                    {
                        wear -= HitPoints;
                        HitPoints = 0;
                    }

                    if (wear > 0)
                    {
                        if (m_MaxHitPoints > wear)
                        {
                            MaxHitPoints -= wear;

                            if (Parent is Mobile)
                                ((Mobile)Parent).LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                        }
                        else
                        {
                            Delete();
                        }
                    }
                }
            }

            return damage;
        }

        public virtual void UnscaleDurability()
        {
        }

        public virtual void ScaleDurability()
        {
        }

        public override void OnAfterDuped(Item newItem)
        {
            BaseTalisman talisman = newItem as BaseTalisman;

            if (talisman == null)
                return;

            talisman.m_Summoner = new TalismanAttribute(this.m_Summoner);
            talisman.m_Protection = new TalismanAttribute(this.m_Protection);
            talisman.m_Killer = new TalismanAttribute(this.m_Killer);
            talisman.m_AosAttributes = new AosAttributes(newItem, this.m_AosAttributes);
            talisman.m_AosSkillBonuses = new AosSkillBonuses(newItem, this.m_AosSkillBonuses);
        }

        public override bool CanEquip(Mobile from)
        {
            if (this.BlessedFor != null && this.BlessedFor != from)
            {
                from.SendLocalizedMessage(1010437); // You are not the owner.
                return false;
            }

            return base.CanEquip(from);
        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                this.m_AosSkillBonuses.AddTo(from);
                this.m_AosAttributes.AddStatBonuses(from);

                if (this.m_Blessed && this.BlessedFor == null)
                {
                    this.BlessedFor = from;
                    this.LootType = LootType.Blessed;
                }

                if (this.m_ChargeTime > 0)
                {
                    this.m_ChargeTime = this.m_MaxChargeTime;
                    this.StartTimer();
                }
            }

            this.InvalidateProperties();
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                this.m_AosSkillBonuses.Remove();
                this.m_AosAttributes.RemoveStatBonuses(from);

                if (this.m_Creature != null && !this.m_Creature.Deleted)
                {
                    Effects.SendLocationParticles(EffectItem.Create(this.m_Creature.Location, this.m_Creature.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                    Effects.PlaySound(this.m_Creature, this.m_Creature.Map, 0x201);

                    this.m_Creature.Delete();
                }

                this.StopTimer();
            }

            this.InvalidateProperties();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Talisman != this)
                from.SendLocalizedMessage(502641); // You must equip this item to use it.
            else if (this.m_ChargeTime > 0)
                from.SendLocalizedMessage(1074882, this.m_ChargeTime.ToString()); // You must wait ~1_val~ seconds for this to recharge.
            else if (this.m_Charges == 0 && this.m_MaxCharges > 0)
                from.SendLocalizedMessage(1042544); // This item is out of charges.
            else
            {
                Type type = this.GetSummoner();

                if (this.m_Summoner != null && !this.m_Summoner.IsEmpty)
                    type = this.m_Summoner.Type;

                if (type != null)
                {
                    object obj;

                    try
                    {
                        obj = Activator.CreateInstance(type);
                    }
                    catch
                    {
                        obj = null;
                    }

                    if (obj is Item)
                    {
                        Item item = (Item)obj;
                        int count = 1;

                        if (this.m_Summoner != null && this.m_Summoner.Amount > 1)
                        {
                            if (item.Stackable)
                                item.Amount = this.m_Summoner.Amount;
                            else
                                count = this.m_Summoner.Amount;
                        }

                        if (from.Backpack == null || count * item.Weight > from.Backpack.MaxWeight ||
                            from.Backpack.Items.Count + count > from.Backpack.MaxItems)
                        {
                            from.SendLocalizedMessage(500720); // You don't have enough room in your backpack!
                            item.Delete();
                            item = null;
                            return;
                        }

                        for (int i = 0; i < count; i++)
                        {
                            from.PlaceInBackpack(item);

                            if (i + 1 < count)
                                item = Activator.CreateInstance(type) as Item;
                        }

                        if (item is Board)
                            from.SendLocalizedMessage(1075000); // You have been given some wooden boards.
                        else if (item is IronIngot)
                            from.SendLocalizedMessage(1075001); // You have been given some ingots.
                        else if (item is Bandage)
                            from.SendLocalizedMessage(1075002); // You have been given some clean bandages.
                        else if (this.m_Summoner != null && this.m_Summoner.Name != null)
                            from.SendLocalizedMessage(1074853, this.m_Summoner.Name.ToString()); // You have been given ~1_name~
                    }
                    else if (obj is BaseCreature)
                    {
                        BaseCreature mob = (BaseCreature)obj;

                        if ((this.m_Creature != null && !this.m_Creature.Deleted) || from.Followers + mob.ControlSlots > from.FollowersMax)
                        {
                            from.SendLocalizedMessage(1074270); // You have too many followers to summon another one.
                            mob.Delete();
                            return;
                        }

                        BaseCreature.Summon(mob, from, from.Location, mob.BaseSoundID, TimeSpan.FromMinutes(10));
                        Effects.SendLocationParticles(EffectItem.Create(mob.Location, mob.Map, EffectItem.DefaultDuration), 0x3728, 1, 10, 0x26B6);

                        mob.Summoned = false;
                        mob.ControlOrder = OrderType.Friend;

                        this.m_Creature = mob;
                    }

                    this.OnAfterUse(from);
                }

                if (this.m_Removal != TalismanRemoval.None)
                {
                    from.Target = new TalismanTarget(this);
                }
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.ForceShowName)
                base.AddNameProperty(list);
            else if (this.m_Summoner != null && !this.m_Summoner.IsEmpty)
                list.Add(1072400, this.m_Summoner.Name != null ? this.m_Summoner.Name.ToString() : "Unknown"); // Talisman of ~1_name~ Summoning
            else if (this.m_Removal != TalismanRemoval.None)
                list.Add(1072389, "#" + (1072000 + (int)this.m_Removal)); // Talisman of ~1_name~
            else
                base.AddNameProperty(list);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if(Attributes.Brittle > 0)
                list.Add(1116209); // Brittle

            if (this.Blessed)
            {
                if (this.BlessedFor != null)
                    list.Add(1072304, !String.IsNullOrEmpty(this.BlessedFor.Name) ? this.BlessedFor.Name : "Unnamed Warrior"); // Owned by ~1_name~
                else
                    list.Add(1072304, "Nobody"); // Owned by ~1_name~
            }

            if (this.Parent is Mobile && this.m_MaxChargeTime > 0)
            {
                if (this.m_ChargeTime > 0)
                    list.Add(1074884, this.m_ChargeTime.ToString()); // Charge time left: ~1_val~
                else
                    list.Add(1074883); // Fully Charged
            }

            if (this.m_Killer != null && !this.m_Killer.IsEmpty && this.m_Killer.Amount > 0)
                list.Add(1072388, "{0}\t{1}", this.m_Killer.Name != null ? this.m_Killer.Name.ToString() : "Unknown", this.m_Killer.Amount); // ~1_NAME~ Killer: +~2_val~%

            if (this.m_Protection != null && !this.m_Protection.IsEmpty && this.m_Protection.Amount > 0)
                list.Add(1072387, "{0}\t{1}", this.m_Protection.Name != null ? this.m_Protection.Name.ToString() : "Unknown", this.m_Protection.Amount); // ~1_NAME~ Protection: +~2_val~%

            if (this.m_ExceptionalBonus != 0)
                list.Add(1072395, "#{0}\t{1}", AosSkillBonuses.GetLabel(this.m_Skill), this.m_ExceptionalBonus); // ~1_NAME~ Exceptional Bonus: ~2_val~%

            if (this.m_SuccessBonus != 0)
                list.Add(1072394, "#{0}\t{1}", AosSkillBonuses.GetLabel(this.m_Skill), this.m_SuccessBonus); // ~1_NAME~ Bonus: ~2_val~%

            this.m_AosSkillBonuses.GetProperties(list);

            int prop;

            if ((prop = this.m_AosAttributes.WeaponDamage) != 0)
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%

            if ((prop = this.m_AosAttributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = this.m_AosAttributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = this.m_AosAttributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = this.m_AosAttributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = this.m_AosAttributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = this.m_AosAttributes.AttackChance) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = this.m_AosAttributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = this.m_AosAttributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = this.m_AosAttributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = this.m_AosAttributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%

            if ((prop = this.m_AosAttributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = this.m_AosAttributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = this.m_AosAttributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = this.m_AosAttributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = this.m_AosAttributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = this.m_AosAttributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = this.m_AosAttributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = this.m_AosAttributes.SpellChanneling) != 0)
                list.Add(1060482); // spell channeling

            if ((prop = this.m_AosAttributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = this.m_AosAttributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = this.m_AosAttributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = this.m_AosAttributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            if (Core.ML && (prop = this.m_AosAttributes.IncreasedKarmaLoss) != 0)
                list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%

            if (this.m_MaxCharges > 0)
                list.Add(1060741, this.m_Charges.ToString()); // charges: ~1_val~

            if (this is ManaPhasingOrb)
                list.Add(1116158); //Mana Phase

            if (this.m_Slayer != TalismanSlayerName.None)
            {
                if (this.m_Slayer == TalismanSlayerName.Wolf)
                    list.Add(1075462);
                else if (this.m_Slayer == TalismanSlayerName.Goblin)
                    list.Add(1095010);
                else if (this.m_Slayer == TalismanSlayerName.Undead)
                    list.Add(1060479);
                else
                    list.Add(1072503 + (int)this.m_Slayer);
            }

            if (this.m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", this.m_HitPoints, this.m_MaxHitPoints); // durability ~1_val~ / ~2_val~
        }

        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Attributes = 0x00000001,
            SkillBonuses = 0x00000002,
            Owner = 0x00000004,
            Protection = 0x00000008,
            Killer = 0x00000010,
            Summoner = 0x00000020,
            Removal = 0x00000040,
            OldKarmaLoss = 0x00000080,
            Skill = 0x00000100,
            SuccessBonus = 0x00000200,
            ExceptionalBonus = 0x00000400,
            MaxCharges = 0x00000800,
            Charges = 0x00001000,
            MaxChargeTime = 0x00002000,
            ChargeTime = 0x00004000,
            Blessed = 0x00008000,
            Slayer = 0x00010000,
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(m_MaxHitPoints);
            writer.Write(m_HitPoints);

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.Attributes, !this.m_AosAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SkillBonuses, !this.m_AosSkillBonuses.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Protection, this.m_Protection != null && !this.m_Protection.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Killer, this.m_Killer != null && !this.m_Killer.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Summoner, this.m_Summoner != null && !this.m_Summoner.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Removal, this.m_Removal != TalismanRemoval.None);
            SetSaveFlag(ref flags, SaveFlag.Skill, (int)this.m_Skill != 0);
            SetSaveFlag(ref flags, SaveFlag.SuccessBonus, this.m_SuccessBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.ExceptionalBonus, this.m_ExceptionalBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxCharges, this.m_MaxCharges != 0);
            SetSaveFlag(ref flags, SaveFlag.Charges, this.m_Charges != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxChargeTime, this.m_MaxChargeTime != 0);
            SetSaveFlag(ref flags, SaveFlag.ChargeTime, this.m_ChargeTime != 0);
            SetSaveFlag(ref flags, SaveFlag.Blessed, this.m_Blessed);
            SetSaveFlag(ref flags, SaveFlag.Slayer, this.m_Slayer != TalismanSlayerName.None);

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.Attributes))
                this.m_AosAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                this.m_AosSkillBonuses.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Protection))
                this.m_Protection.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Killer))
                this.m_Killer.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Summoner))
                this.m_Summoner.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Removal))
                writer.WriteEncodedInt((int)this.m_Removal);

            if (GetSaveFlag(flags, SaveFlag.Skill))
                writer.WriteEncodedInt((int)this.m_Skill);

            if (GetSaveFlag(flags, SaveFlag.SuccessBonus))
                writer.WriteEncodedInt(this.m_SuccessBonus);

            if (GetSaveFlag(flags, SaveFlag.ExceptionalBonus))
                writer.WriteEncodedInt(this.m_ExceptionalBonus);

            if (GetSaveFlag(flags, SaveFlag.MaxCharges))
                writer.WriteEncodedInt(this.m_MaxCharges);

            if (GetSaveFlag(flags, SaveFlag.Charges))
                writer.WriteEncodedInt(this.m_Charges);

            if (GetSaveFlag(flags, SaveFlag.MaxChargeTime))
                writer.WriteEncodedInt(this.m_MaxChargeTime);

            if (GetSaveFlag(flags, SaveFlag.ChargeTime))
                writer.WriteEncodedInt(this.m_ChargeTime);

            if (GetSaveFlag(flags, SaveFlag.Slayer))
                writer.WriteEncodedInt((int)this.m_Slayer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_MaxHitPoints = reader.ReadInt();
                        m_HitPoints = reader.ReadInt();
                    }
                    goto case 0;
                case 0:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Attributes))
                            this.m_AosAttributes = new AosAttributes(this, reader);
                        else
                            this.m_AosAttributes = new AosAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                            this.m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            this.m_AosSkillBonuses = new AosSkillBonuses(this);

                        // Backward compatibility
                        if (GetSaveFlag(flags, SaveFlag.Owner))
                            this.BlessedFor = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.Protection))
                            this.m_Protection = new TalismanAttribute(reader);
                        else
                            this.m_Protection = new TalismanAttribute();

                        if (GetSaveFlag(flags, SaveFlag.Killer))
                            this.m_Killer = new TalismanAttribute(reader);
                        else
                            this.m_Killer = new TalismanAttribute();

                        if (GetSaveFlag(flags, SaveFlag.Summoner))
                            this.m_Summoner = new TalismanAttribute(reader);
                        else
                            this.m_Summoner = new TalismanAttribute();

                        if (GetSaveFlag(flags, SaveFlag.Removal))
                            this.m_Removal = (TalismanRemoval)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.OldKarmaLoss))
                            this.m_AosAttributes.IncreasedKarmaLoss = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Skill))
                            this.m_Skill = (SkillName)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.SuccessBonus))
                            this.m_SuccessBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.ExceptionalBonus))
                            this.m_ExceptionalBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.MaxCharges))
                            this.m_MaxCharges = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Charges))
                            this.m_Charges = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.MaxChargeTime))
                            this.m_MaxChargeTime = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.ChargeTime))
                            this.m_ChargeTime = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Slayer))
                            this.m_Slayer = (TalismanSlayerName)reader.ReadEncodedInt();

                        this.m_Blessed = GetSaveFlag(flags, SaveFlag.Blessed);

                        break;
                    }
            }

            if (this.Parent is Mobile)
            {
                Mobile m = (Mobile)this.Parent;

                this.m_AosAttributes.AddStatBonuses(m);
                this.m_AosSkillBonuses.AddTo(m);

                if (this.m_ChargeTime > 0)
                    this.StartTimer();
            }
        }

        public virtual void OnAfterUse(Mobile m)
        {
            this.m_ChargeTime = this.m_MaxChargeTime;

            if (this.m_Charges > 0 && this.m_MaxCharges > 0)
                this.m_Charges -= 1;

            if (this.m_ChargeTime > 0)
                this.StartTimer();

            this.InvalidateProperties();
        }

        public virtual Type GetSummoner()
        {
            return null;
        }

        public virtual void SetSummoner(Type type, TextDefinition name)
        {
            this.m_Summoner = new TalismanAttribute(type, name);
        }

        public virtual void SetProtection(Type type, TextDefinition name, int amount)
        {
            this.m_Protection = new TalismanAttribute(type, name, amount);
        }

        public virtual void SetKiller(Type type, TextDefinition name, int amount)
        {
            this.m_Killer = new TalismanAttribute(type, name, amount);
        }

        #region Timer
        private Timer m_Timer;

        public virtual void StartTimer()
        {
            if (this.m_Timer == null || !this.m_Timer.Running)
                this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), new TimerCallback(Slice));
        }

        public virtual void StopTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;
        }

        public virtual void Slice()
        {
            if (this.m_ChargeTime - 10 > 0)
                this.m_ChargeTime -= 10;
            else
            {
                this.m_ChargeTime = 0;

                this.StopTimer();
            }

            this.InvalidateProperties();
        }

        #endregion

        #region Randomize
        private static readonly int[] m_ItemIDs = new int[]
        {
            0x2F58, 0x2F59, 0x2F5A, 0x2F5B
        };

        public static int GetRandomItemID()
        {
            return Utility.RandomList(m_ItemIDs);
        }

        private static readonly Type[] m_Summons = new Type[]
        {
            typeof(SummonedAntLion),
            typeof(SummonedCow),
            typeof(SummonedLavaSerpent),
            typeof(SummonedOrcBrute),
            typeof(SummonedFrostSpider),
            typeof(SummonedPanther),
            typeof(SummonedDoppleganger),
            typeof(SummonedGreatHart),
            typeof(SummonedBullFrog),
            typeof(SummonedArcticOgreLord),
            typeof(SummonedBogling),
            typeof(SummonedBakeKitsune),
            typeof(SummonedSheep),
            typeof(SummonedSkeletalKnight),
            typeof(SummonedWailingBanshee),
            typeof(SummonedChicken),
            typeof(SummonedVorpalBunny),
            typeof(Board),
            typeof(IronIngot),
            typeof(Bandage),
        };

        private static readonly int[] m_SummonLabels = new int[]
        {
            1075211, // Ant Lion
            1072494, // Cow
            1072434, // Lava Serpent
            1072414, // Orc Brute
            1072476, // Frost Spider
            1029653, // Panther
            1029741, // Doppleganger
            1018292, // great hart
            1028496, // bullfrog
            1018227, // arctic ogre lord
            1029735, // Bogling
            1030083, // bake-kitsune
            1018285, // sheep
            1018239, // skeletal knight
            1072399, // Wailing Banshee
            1072459, // Chicken
            1072401, // Vorpal Bunny

            1015101, // Boards
            1044036, // Ingots
            1023817, // clean bandage
        };

        public static Type GetRandomSummonType()
        {
            return m_Summons[Utility.Random(m_Summons.Length)];
        }

        public static TalismanAttribute GetRandomSummoner()
        {
            if (0.025 > Utility.RandomDouble())
            {
                int num = Utility.Random(m_Summons.Length);

                if (num > 14)
                    return new TalismanAttribute(m_Summons[num], m_SummonLabels[num], 10);
                else
                    return new TalismanAttribute(m_Summons[num], m_SummonLabels[num]);
            }

            return new TalismanAttribute();
        }

        public static TalismanRemoval GetRandomRemoval()
        {
            if (0.65 > Utility.RandomDouble())
                return (TalismanRemoval)Utility.RandomList(390, 404, 407);

            return TalismanRemoval.None;
        }

        private static readonly Type[] m_Killers = new Type[]
        {
            typeof(OrcBomber), typeof(OrcBrute), typeof(Sewerrat), typeof(Rat), typeof(GiantRat),
            typeof(Ratman), typeof(RatmanArcher), typeof(GiantSpider), typeof(FrostSpider), typeof(GiantBlackWidow),
            typeof(DreadSpider), typeof(SilverSerpent), typeof(DeepSeaSerpent), typeof(GiantSerpent), typeof(Snake),
            typeof(IceSnake), typeof(IceSerpent), typeof(LavaSerpent), typeof(LavaSnake), typeof(Yamandon),
            typeof(StrongMongbat), typeof(Mongbat), typeof(VampireBat), typeof(Lich), typeof(EvilMage),
            typeof(LichLord), typeof(EvilMageLord), typeof(SkeletalMage), typeof(KhaldunZealot), typeof(AncientLich),
            typeof(JukaMage), typeof(MeerMage), typeof(Beetle), typeof(DeathwatchBeetle), typeof(RuneBeetle),
            typeof(FireBeetle), typeof(DeathwatchBeetleHatchling), typeof(Bird), typeof(Chicken), typeof(Eagle),
            typeof(TropicalBird), typeof(Phoenix), typeof(DesertOstard), typeof(FrenziedOstard), typeof(ForestOstard),
            typeof(Crane), typeof(SnowLeopard), typeof(IceFiend), typeof(FrostOoze), typeof(FrostTroll),
            typeof(IceElemental), typeof(SnowElemental), typeof(GiantIceWorm), typeof(LadyOfTheSnow), typeof(FireElemental),
            typeof(FireSteed), typeof(HellHound), typeof(HellCat), typeof(PredatorHellCat), typeof(LavaLizard),
            typeof(FireBeetle), typeof(Cow), typeof(Bull), typeof(Gaman)//,			typeof( Minotaur)
            // TODO Meraktus, Tormented Minotaur, Minotaur
        };

        private static readonly int[] m_KillerLabels = new int[]
        {
            1072413, 1072414, 1072418, 1072419, 1072420,
            1072421, 1072423, 1072424, 1072425, 1072426,
            1072427, 1072428, 1072429, 1072430, 1072431,
            1072432, 1072433, 1072434, 1072435, 1072438,
            1072440, 1072441, 1072443, 1072444, 1072445,
            1072446, 1072447, 1072448, 1072449, 1072450,
            1072451, 1072452, 1072453, 1072454, 1072455,
            1072456, 1072457, 1072458, 1072459, 1072461,
            1072462, 1072465, 1072468, 1072469, 1072470,
            1072473, 1072474, 1072477, 1072478, 1072479,
            1072480, 1072481, 1072483, 1072485, 1072486,
            1072487, 1072489, 1072490, 1072491, 1072492,
            1072493, 1072494, 1072495, 1072498,
        };

        public static TalismanAttribute GetRandomKiller()
        {
            return GetRandomKiller(true);
        }

        public static TalismanAttribute GetRandomKiller(bool includingNone)
        {
            if (includingNone && Utility.RandomBool())
                return new TalismanAttribute();

            int num = Utility.Random(m_Killers.Length);

            return new TalismanAttribute(m_Killers[num], m_KillerLabels[num], Utility.RandomMinMax(10, 100));
        }

        public static TalismanAttribute GetRandomProtection()
        {
            return GetRandomProtection(true);
        }

        public static TalismanAttribute GetRandomProtection(bool includingNone)
        {
            if (includingNone && Utility.RandomBool())
                return new TalismanAttribute();

            int num = Utility.Random(m_Killers.Length);

            return new TalismanAttribute(m_Killers[num], m_KillerLabels[num], Utility.RandomMinMax(5, 60));
        }

        private static readonly SkillName[] m_Skills = new SkillName[]
        {
            SkillName.Alchemy,
            SkillName.Blacksmith,
            SkillName.Carpentry,
            SkillName.Cartography,
            SkillName.Cooking,
            SkillName.Fletching,
            SkillName.Inscribe,
            SkillName.Tailoring,
            SkillName.Tinkering,
        };

        public static SkillName GetRandomSkill()
        {
            return m_Skills[Utility.Random(m_Skills.Length)];
        }

        public static int GetRandomExceptional()
        {
            if (0.3 > Utility.RandomDouble())
            {
                double num = 40 - Math.Log(Utility.RandomMinMax(7, 403)) * 5;

                return (int)Math.Round(num);
            }

            return 0;
        }

        public static int GetRandomSuccessful()
        {
            if (0.75 > Utility.RandomDouble())
            {
                double num = 40 - Math.Log(Utility.RandomMinMax(7, 403)) * 5;

                return (int)Math.Round(num);
            }

            return 0;
        }

        public static bool GetRandomBlessed()
        {
            if (0.02 > Utility.RandomDouble())
                return true;

            return false;
        }

        public static TalismanSlayerName GetRandomSlayer()
        {
            if (0.01 > Utility.RandomDouble())
                return (TalismanSlayerName)Utility.RandomMinMax(1, 9);

            return TalismanSlayerName.None;
        }

        public static int GetRandomCharges()
        {
            if (0.5 > Utility.RandomDouble())
                return Utility.RandomMinMax(10, 50);

            return 0;
        }

        #endregion

        private class TalismanTarget : Target
        {
            private readonly BaseTalisman m_Talisman;

            public TalismanTarget(BaseTalisman talisman)
                : base(12, false, TargetFlags.Beneficial)
            {
                this.m_Talisman = talisman;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (this.m_Talisman == null || this.m_Talisman.Deleted)
                    return;

                Mobile target = o as Mobile;

                if (from.Talisman != this.m_Talisman)
                    from.SendLocalizedMessage(502641); // You must equip this item to use it.
                else if (target == null)
                    from.SendLocalizedMessage(1046439); // That is not a valid target.
                else if (this.m_Talisman.ChargeTime > 0)
                    from.SendLocalizedMessage(1074882, this.m_Talisman.ChargeTime.ToString()); // You must wait ~1_val~ seconds for this to recharge.
                else if (this.m_Talisman.Charges == 0 && this.m_Talisman.MaxCharges > 0)
                    from.SendLocalizedMessage(1042544); // This item is out of charges.
                else
                {
                    switch (this.m_Talisman.Removal)
                    {
                        case TalismanRemoval.Curse:
                            target.PlaySound(0xF6);
                            target.PlaySound(0x1F7);
                            target.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);

                            IEntity mfrom = new Entity(Serial.Zero, new Point3D(target.X, target.Y, target.Z - 10), from.Map);
                            IEntity mto = new Entity(Serial.Zero, new Point3D(target.X, target.Y, target.Z + 50), from.Map);
                            Effects.SendMovingParticles(mfrom, mto, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

                            target.RemoveStatMod("[Magic] Str Curse");
							target.RemoveStatMod("[Magic] Dex Curse");
							target.RemoveStatMod("[Magic] Int Curse");

                            target.Paralyzed = false;

                            EvilOmenSpell.TryEndEffect(target);
                            StrangleSpell.RemoveCurse(target);
                            CorpseSkinSpell.RemoveCurse(target);
                            CurseSpell.RemoveEffect(target);

                            BuffInfo.RemoveBuff(target, BuffIcon.Clumsy);
                            BuffInfo.RemoveBuff(target, BuffIcon.FeebleMind);
                            BuffInfo.RemoveBuff(target, BuffIcon.Weaken);
                            BuffInfo.RemoveBuff(target, BuffIcon.MassCurse);

                            target.SendLocalizedMessage(1072408); // Any curses on you have been lifted

                            if (target != from)
                                from.SendLocalizedMessage(1072409); // Your targets curses have been lifted

                            break;
                        case TalismanRemoval.Damage:
                            target.PlaySound(0x201);
                            Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 0x834, 0, 0x13B2, 0);

                            BleedAttack.EndBleed(target, true);
                            MortalStrike.EndWound(target);

                            BuffInfo.RemoveBuff(target, BuffIcon.Bleed);
                            BuffInfo.RemoveBuff(target, BuffIcon.MortalStrike);

                            target.SendLocalizedMessage(1072405); // Your lasting damage effects have been removed!

                            if (target != from)
                                from.SendLocalizedMessage(1072406); // Your Targets lasting damage effects have been removed!

                            break;
                        case TalismanRemoval.Ward:
                            target.PlaySound(0x201);
                            Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 0x834, 0, 0x13B2, 0);

                            MagicReflectSpell.EndReflect(target);
                            ReactiveArmorSpell.EndArmor(target);
                            ProtectionSpell.EndProtection(target);

                            target.SendLocalizedMessage(1072402); // Your wards have been removed!

                            if (target != from)
                                from.SendLocalizedMessage(1072403); // Your target's wards have been removed!

                            break;
                        case TalismanRemoval.Wildfire:
                            // TODO
                            break;
                    }

                    this.m_Talisman.OnAfterUse(from);
                }
            }
        }
    }
}