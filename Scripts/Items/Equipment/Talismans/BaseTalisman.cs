using System;
using Server.Commands;
using Server.Mobiles;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Spells.Second;
using Server.Targeting;
using Server.Engines.Craft;
using Server.Factions;

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

    public enum TalismanSkill
    {
        Alchemy,
        Blacksmithy,
        Fletching,
        Carpentry,
        Cartography,
        Cooking,
        Glassblowing,
        Inscription,
        Masonry,
        Tailoring,
        Tinkering
    }

    public class BaseTalisman : Item, IWearableDurability, IVvVItem, IOwnerRestricted, ITalismanProtection, ITalismanKiller, IFactionItem
    {
        #region Factions
        private FactionItem m_FactionState;

        public FactionItem FactionItemState
        {
            get { return m_FactionState; }
            set
            {
                m_FactionState = value;
            }
        }
        #endregion

        private bool _VvVItem;
        private Mobile _Owner;
        private string _OwnerName;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsVvVItem
        {
            get { return _VvVItem; }
            set { _VvVItem = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return _Owner; }
            set { _Owner = value; if (_Owner != null) _OwnerName = _Owner.Name; InvalidateProperties(); }
        }

        public virtual string OwnerName
        {
            get { return _OwnerName; }
            set { _OwnerName = value; InvalidateProperties(); }
        }

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

        public override bool DisplayWeight
        {
            get
            {
                if (IsVvVItem)
                    return true;

                return base.DisplayWeight;
            }
        }

        public virtual bool ForceShowName
        {
            get
            {
                return false;
            }
        }// used to override default summoner/removal name

        private int m_MaxHitPoints;
        private int m_HitPoints;

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
                return m_AosAttributes.IncreasedKarmaLoss;
            }
            set
            {
                m_AosAttributes.IncreasedKarmaLoss = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get
            {
                return m_MaxCharges;
            }
            set
            {
                m_MaxCharges = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get
            {
                return m_Charges;
            }
            set
            {
                m_Charges = value;

                if (m_ChargeTime > 0)
                    StartTimer();

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxChargeTime
        {
            get
            {
                return m_MaxChargeTime;
            }
            set
            {
                m_MaxChargeTime = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ChargeTime
        {
            get
            {
                return m_ChargeTime;
            }
            set
            {
                m_ChargeTime = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Blessed
        {
            get
            {
                return m_Blessed;
            }
            set
            {
                m_Blessed = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get
            {
                return m_MaxHitPoints;
            }
            set
            {
                m_MaxHitPoints = value;

                if (m_MaxHitPoints > 255)
                    m_MaxHitPoints = 255;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get
            {
                return m_HitPoints;
            }
            set
            {
                if (value != m_HitPoints && MaxHitPoints > 0)
                {
                    m_HitPoints = value;

                    if (m_HitPoints < 0)
                        Delete();
                    else if (m_HitPoints > MaxHitPoints)
                        m_HitPoints = MaxHitPoints;

                    InvalidateProperties();
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

        public virtual bool CanRepair { get { return true; } }
        public virtual bool CanFortify { get { return NegativeAttributes.Antique < 4; } }

        #region Slayer
        private TalismanSlayerName m_Slayer;

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanSlayerName Slayer
        {
            get
            {
                return m_Slayer;
            }
            set
            {
                m_Slayer = value;
                InvalidateProperties();
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
                return m_Summoner;
            }
            set
            {
                m_Summoner = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanRemoval Removal
        {
            get
            {
                return m_Removal;
            }
            set
            {
                m_Removal = value;
                InvalidateProperties();
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
                return m_Protection;
            }
            set
            {
                m_Protection = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanAttribute Killer
        {
            get
            {
                return m_Killer;
            }
            set
            {
                m_Killer = value;
                InvalidateProperties();
            }
        }
        #endregion

        #region Craft bonuses
        private TalismanSkill m_Skill;
        private int m_SuccessBonus;
        private int m_ExceptionalBonus;

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanSkill Skill
        {
            get
            {
                return m_Skill;
            }
            set
            {
                m_Skill = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName CraftSkill
        {
            get { return GetMainSkill(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SuccessBonus
        {
            get
            {
                return m_SuccessBonus;
            }
            set
            {
                m_SuccessBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExceptionalBonus
        {
            get
            {
                return m_ExceptionalBonus;
            }
            set
            {
                m_ExceptionalBonus = value;
                InvalidateProperties();
            }
        }
        #endregion

        #region AOS bonuses
        private AosAttributes m_AosAttributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private NegativeAttributes m_NegativeAttributes;

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes
        {
            get
            {
                return m_AosAttributes;
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
                return m_AosSkillBonuses;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NegativeAttributes NegativeAttributes
        {
            get
            {
                return m_NegativeAttributes;
            }
            set
            {
            }
        }
        #endregion

        private SAAbsorptionAttributes m_SAAbsorptionAttributes;

        [CommandProperty(AccessLevel.GameMaster)]
        public SAAbsorptionAttributes SAAbsorptionAttributes
        {
            get
            {
                return m_SAAbsorptionAttributes;
            }
            set
            {
            }
        }

        public BaseTalisman()
            : this(GetRandomItemID())
        {
        }

        public BaseTalisman(int itemID)
            : base(itemID)
        {
            Layer = Layer.Talisman;
            Weight = 1.0;

            m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            m_Protection = new TalismanAttribute();
            m_Killer = new TalismanAttribute();
            m_Summoner = new TalismanAttribute();
            m_AosAttributes = new AosAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);
            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);
            m_NegativeAttributes = new NegativeAttributes(this);
        }

        public BaseTalisman(Serial serial)
            : base(serial)
        {
        }

        public virtual int OnHit(BaseWeapon weap, int damage)
        {
            if (m_MaxHitPoints == 0)
                return damage;

            int chance = m_NegativeAttributes.Antique > 0 ? 50 : 25;
            if (chance > Utility.Random(100)) // 25% chance to lower durability
            {
                if (m_HitPoints >= 1)
                {
                    HitPoints--;
                }
                else if (m_MaxHitPoints > 0)
                {
                    MaxHitPoints--;

                    if (Parent is Mobile)
                        ((Mobile)Parent).LocalOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.

                    if (m_MaxHitPoints == 0)
                    {
                        Delete();
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

            talisman.m_Summoner = new TalismanAttribute(m_Summoner);
            talisman.m_Protection = new TalismanAttribute(m_Protection);
            talisman.m_Killer = new TalismanAttribute(m_Killer);
            talisman.m_AosAttributes = new AosAttributes(newItem, m_AosAttributes);
            talisman.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
            talisman.m_SAAbsorptionAttributes = new SAAbsorptionAttributes(newItem, m_SAAbsorptionAttributes);
            talisman.m_NegativeAttributes = new NegativeAttributes(newItem, m_NegativeAttributes);
        }

        public override bool CanEquip(Mobile from)
        {
            if (from.IsPlayer())
            {
                if (_Owner != null && _Owner != from)
                {
                    from.SendLocalizedMessage(501023); // You must be the owner to use this item.
                    return false;
                }

                if (this is IAccountRestricted && ((IAccountRestricted)this).Account != null)
                {
                    Accounting.Account acct = from.Account as Accounting.Account;

                    if (acct == null || acct.Username != ((IAccountRestricted)this).Account)
                    {
                        from.SendLocalizedMessage(1071296); // This item is Account Bound and your character is not bound to it. You cannot use this item.
                        return false;
                    }
                }

                if (IsVvVItem && !Engines.VvV.ViceVsVirtueSystem.IsVvV(from))
                {
                    from.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                    return false;
                }
            }

            if (BlessedFor != null && BlessedFor != from)
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

                m_AosSkillBonuses.AddTo(from);
                m_AosAttributes.AddStatBonuses(from);

                if (m_Blessed && BlessedFor == null)
                {
                    BlessedFor = from;
                    LootType = LootType.Blessed;
                }

                if (m_ChargeTime > 0)
                {
                    m_ChargeTime = m_MaxChargeTime;
                    StartTimer();
                }
            }

            InvalidateProperties();
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.Remove();
                m_AosAttributes.RemoveStatBonuses(from);

                if (m_Creature != null && !m_Creature.Deleted)
                {
                    Effects.SendLocationParticles(EffectItem.Create(m_Creature.Location, m_Creature.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                    Effects.PlaySound(m_Creature, m_Creature.Map, 0x201);

                    m_Creature.Delete();
                    m_Creature = null;
                }

                StopTimer();
            }

            InvalidateProperties();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Talisman != this)
                from.SendLocalizedMessage(502641); // You must equip this item to use it.
            else if (m_ChargeTime > 0)
                from.SendLocalizedMessage(1074882, m_ChargeTime.ToString()); // You must wait ~1_val~ seconds for this to recharge.
            else if (m_Charges == 0 && m_MaxCharges > 0)
                from.SendLocalizedMessage(1042544); // This item is out of charges.
            else
            {
                Type type = GetSummoner();

                if (m_Summoner != null && !m_Summoner.IsEmpty)
                    type = m_Summoner.Type;

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

                        if (m_Summoner != null && m_Summoner.Amount > 1)
                        {
                            if (item.Stackable)
                                item.Amount = m_Summoner.Amount;
                            else
                                count = m_Summoner.Amount;
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
                        else if (m_Summoner != null && m_Summoner.Name != null)
                            from.SendLocalizedMessage(1074853, m_Summoner.Name.ToString()); // You have been given ~1_name~
                    }
                    else if (obj is BaseCreature)
                    {
                        BaseCreature mob = (BaseCreature)obj;

                        if ((m_Creature != null && !m_Creature.Deleted) || from.Followers + mob.ControlSlots > from.FollowersMax)
                        {
                            from.SendLocalizedMessage(1074270); // You have too many followers to summon another one.
                            mob.Delete();
                            return;
                        }

                        BaseCreature.Summon(mob, from, from.Location, mob.BaseSoundID, TimeSpan.FromMinutes(10));
                        Effects.SendLocationParticles(EffectItem.Create(mob.Location, mob.Map, EffectItem.DefaultDuration), 0x3728, 1, 10, 0x26B6);

                        mob.Summoned = false;
                        mob.ControlOrder = OrderType.Friend;

                        m_Creature = mob;
                    }

                    OnAfterUse(from);
                }

                if (m_Removal != TalismanRemoval.None)
                {
                    from.Target = new TalismanTarget(this);
                }
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (ForceShowName)
                base.AddNameProperty(list);
            else if (m_Summoner != null && !m_Summoner.IsEmpty)
                list.Add(1072400, m_Summoner.Name != null ? m_Summoner.Name.ToString() : "Unknown"); // Talisman of ~1_name~ Summoning
            else if (m_Removal != TalismanRemoval.None)
                list.Add(1072389, "#" + (1072000 + (int)m_Removal)); // Talisman of ~1_name~
            else
                base.AddNameProperty(list);
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            if (IsVvVItem)
                list.Add(1154937); // VvV Item
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (OwnerName != null)
            {
                list.Add(1153213, OwnerName);
            }

            #region Factions
            FactionEquipment.AddFactionProperties(this, list);
            #endregion

            if(Attributes.Brittle > 0)
                list.Add(1116209); // Brittle

            if (Blessed)
            {
                if (BlessedFor != null)
                    list.Add(1072304, !String.IsNullOrEmpty(BlessedFor.Name) ? BlessedFor.Name : "Unnamed Warrior"); // Owned by ~1_name~
                else
                    list.Add(1072304, "Nobody"); // Owned by ~1_name~
            }

            if (Parent is Mobile && m_MaxChargeTime > 0)
            {
                if (m_ChargeTime > 0)
                    list.Add(1074884, m_ChargeTime.ToString()); // Charge time left: ~1_val~
                else
                    list.Add(1074883); // Fully Charged
            }

            if (m_Killer != null && !m_Killer.IsEmpty && m_Killer.Amount > 0)
                list.Add(1072388, "{0}\t{1}", m_Killer.Name != null ? m_Killer.Name.ToString() : "Unknown", m_Killer.Amount); // ~1_NAME~ Killer: +~2_val~%

            if (m_Protection != null && !m_Protection.IsEmpty && m_Protection.Amount > 0)
                list.Add(1072387, "{0}\t{1}", m_Protection.Name != null ? m_Protection.Name.ToString() : "Unknown", m_Protection.Amount); // ~1_NAME~ Protection: +~2_val~%

            if (m_ExceptionalBonus != 0)
                list.Add(1072395, "#{0}\t{1}", GetSkillLabel(), m_ExceptionalBonus); // ~1_NAME~ Exceptional Bonus: ~2_val~%

            if (m_SuccessBonus != 0)
                list.Add(1072394, "#{0}\t{1}", GetSkillLabel(), m_SuccessBonus); // ~1_NAME~ Bonus: ~2_val~%

            if (m_NegativeAttributes != null)
                m_NegativeAttributes.GetProperties(list, this);

            m_AosSkillBonuses.GetProperties(list);

            int prop;

            if ((prop = m_AosAttributes.WeaponDamage) != 0)
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%

            if ((prop = m_AosAttributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = m_AosAttributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = m_AosAttributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = m_AosAttributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = m_AosAttributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = m_AosAttributes.AttackChance) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = m_AosAttributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = m_AosAttributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = m_AosAttributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = m_AosAttributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%

            if ((prop = m_AosAttributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = m_AosAttributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = m_AosAttributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = m_AosAttributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = m_AosAttributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = m_AosAttributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = m_AosAttributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = m_AosAttributes.SpellChanneling) != 0)
                list.Add(1060482); // spell channeling

            if ((prop = m_AosAttributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = m_AosAttributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = m_AosAttributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = m_AosAttributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            if (Core.ML && (prop = m_AosAttributes.IncreasedKarmaLoss) != 0)
                list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%

            if (m_MaxCharges > 0)
                list.Add(1060741, m_Charges.ToString()); // charges: ~1_val~

            #region SA
            if ((prop = m_SAAbsorptionAttributes.CastingFocus) != 0)
                list.Add(1113696, prop.ToString()); // Casting Focus ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.EaterFire) != 0)
                list.Add(1113593, prop.ToString()); // Fire Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterCold) != 0)
                list.Add(1113594, prop.ToString()); // Cold Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterPoison) != 0)
                list.Add(1113595, prop.ToString()); // Poison Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterEnergy) != 0)
                list.Add(1113596, prop.ToString()); // Energy Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterKinetic) != 0)
                list.Add(1113597, prop.ToString()); // Kinetic Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterDamage) != 0)
                list.Add(1113598, prop.ToString()); // Damage Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.ResonanceFire) != 0)
                list.Add(1113691, prop.ToString()); // Fire Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.ResonanceCold) != 0)
                list.Add(1113692, prop.ToString()); // Cold Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.ResonancePoison) != 0)
                list.Add(1113693, prop.ToString()); // Poison Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.ResonanceEnergy) != 0)
                list.Add(1113694, prop.ToString()); // Energy Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.ResonanceKinetic) != 0)
                list.Add(1113695, prop.ToString()); // Kinetic Resonance ~1_val~%
            #endregion

            base.AddResistanceProperties(list);

            if (this is ManaPhasingOrb)
                list.Add(1116158); //Mana Phase

            if (m_Slayer != TalismanSlayerName.None)
            {
                if (m_Slayer == TalismanSlayerName.Goblin)
                    list.Add(1095010);
                else if (m_Slayer == TalismanSlayerName.Undead)
                    list.Add(1060479);
                else if (m_Slayer <= TalismanSlayerName.Wolf)
                    list.Add(1072503 + (int)m_Slayer);
                else
                {
                    switch (m_Slayer)
                    {
                        case TalismanSlayerName.Repond: list.Add(1079750); break;
                        case TalismanSlayerName.Elemental: list.Add(1079749); break;
                        case TalismanSlayerName.Demon: list.Add(1079748); break;
                        case TalismanSlayerName.Arachnid: list.Add(1079747); break;
                        case TalismanSlayerName.Reptile: list.Add(1079751); break;
                        case TalismanSlayerName.Fey: list.Add(1154652); break;
                    }
                }
            }

            if (m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints); // durability ~1_val~ / ~2_val~
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
            SAAbsorptionAttributes = 0x00020000,
            NegativeAttributes = 0x00040000,
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)4); // version

            writer.Write(m_Creature);

            writer.Write(_VvVItem);
            writer.Write(_Owner);
            writer.Write(_OwnerName);

            writer.Write(m_MaxHitPoints);
            writer.Write(m_HitPoints);

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.Attributes, !m_AosAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SkillBonuses, !m_AosSkillBonuses.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Protection, m_Protection != null && !m_Protection.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Killer, m_Killer != null && !m_Killer.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Summoner, m_Summoner != null && !m_Summoner.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Removal, m_Removal != TalismanRemoval.None);
            SetSaveFlag(ref flags, SaveFlag.Skill, (int)m_Skill != 0);
            SetSaveFlag(ref flags, SaveFlag.SuccessBonus, m_SuccessBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.ExceptionalBonus, m_ExceptionalBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxCharges, m_MaxCharges != 0);
            SetSaveFlag(ref flags, SaveFlag.Charges, m_Charges != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxChargeTime, m_MaxChargeTime != 0);
            SetSaveFlag(ref flags, SaveFlag.ChargeTime, m_ChargeTime != 0);
            SetSaveFlag(ref flags, SaveFlag.Blessed, m_Blessed);
            SetSaveFlag(ref flags, SaveFlag.Slayer, m_Slayer != TalismanSlayerName.None);
            SetSaveFlag(ref flags, SaveFlag.SAAbsorptionAttributes, !m_SAAbsorptionAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.NegativeAttributes, !m_NegativeAttributes.IsEmpty);

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.Attributes))
                m_AosAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                m_AosSkillBonuses.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Protection))
                m_Protection.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Killer))
                m_Killer.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Summoner))
                m_Summoner.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Removal))
                writer.WriteEncodedInt((int)m_Removal);

            if (GetSaveFlag(flags, SaveFlag.Skill))
                writer.WriteEncodedInt((int)m_Skill);

            if (GetSaveFlag(flags, SaveFlag.SuccessBonus))
                writer.WriteEncodedInt(m_SuccessBonus);

            if (GetSaveFlag(flags, SaveFlag.ExceptionalBonus))
                writer.WriteEncodedInt(m_ExceptionalBonus);

            if (GetSaveFlag(flags, SaveFlag.MaxCharges))
                writer.WriteEncodedInt(m_MaxCharges);

            if (GetSaveFlag(flags, SaveFlag.Charges))
                writer.WriteEncodedInt(m_Charges);

            if (GetSaveFlag(flags, SaveFlag.MaxChargeTime))
                writer.WriteEncodedInt(m_MaxChargeTime);

            if (GetSaveFlag(flags, SaveFlag.ChargeTime))
                writer.WriteEncodedInt(m_ChargeTime);

            if (GetSaveFlag(flags, SaveFlag.Slayer))
                writer.WriteEncodedInt((int)m_Slayer);

            if (GetSaveFlag(flags, SaveFlag.SAAbsorptionAttributes))
                m_SAAbsorptionAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.NegativeAttributes))
                m_NegativeAttributes.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4: // version 4 converts SkillName to CraftSystem (thanks glassblowing and stone crafting!)
                case 3:
                    {
                        m_Creature = reader.ReadMobile();
                        goto case 2;
                    }
                case 2:
                    {
                        _VvVItem = reader.ReadBool();
                        _Owner = reader.ReadMobile();
                        _OwnerName = reader.ReadString();
                        goto case 1;
                    }
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
                            m_AosAttributes = new AosAttributes(this, reader);
                        else
                            m_AosAttributes = new AosAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                            m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            m_AosSkillBonuses = new AosSkillBonuses(this);

                        // Backward compatibility
                        if (GetSaveFlag(flags, SaveFlag.Owner))
                            BlessedFor = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.Protection))
                            m_Protection = new TalismanAttribute(reader);
                        else
                            m_Protection = new TalismanAttribute();

                        if (GetSaveFlag(flags, SaveFlag.Killer))
                            m_Killer = new TalismanAttribute(reader);
                        else
                            m_Killer = new TalismanAttribute();

                        if (GetSaveFlag(flags, SaveFlag.Summoner))
                            m_Summoner = new TalismanAttribute(reader);
                        else
                            m_Summoner = new TalismanAttribute();

                        if (GetSaveFlag(flags, SaveFlag.Removal))
                            m_Removal = (TalismanRemoval)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.OldKarmaLoss))
                            m_AosAttributes.IncreasedKarmaLoss = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Skill))
                        {
                            if (version <= 3)
                            {
                                m_Skill = GetTalismanSkill((SkillName)reader.ReadEncodedInt());
                            }
                            else
                            {
                                m_Skill = (TalismanSkill)reader.ReadEncodedInt();
                            }
                        }

                        if (GetSaveFlag(flags, SaveFlag.SuccessBonus))
                            m_SuccessBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.ExceptionalBonus))
                            m_ExceptionalBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.MaxCharges))
                            m_MaxCharges = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Charges))
                            m_Charges = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.MaxChargeTime))
                            m_MaxChargeTime = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.ChargeTime))
                            m_ChargeTime = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Slayer))
                            m_Slayer = (TalismanSlayerName)reader.ReadEncodedInt();

                        m_Blessed = GetSaveFlag(flags, SaveFlag.Blessed);

                        if (GetSaveFlag(flags, SaveFlag.SAAbsorptionAttributes))
                            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this, reader);
                        else
                            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.NegativeAttributes))
                            m_NegativeAttributes = new NegativeAttributes(this, reader);
                        else
                            m_NegativeAttributes = new NegativeAttributes(this);
                        break;
                    }
            }

            if (Parent is Mobile)
            {
                Mobile m = (Mobile)Parent;

                m_AosAttributes.AddStatBonuses(m);
                m_AosSkillBonuses.AddTo(m);

                if (m_ChargeTime > 0)
                    StartTimer();
            }

            if (IsVvVItem && m_MaxHitPoints == 0)
            {
                m_NegativeAttributes.Antique = 1;

                m_MaxHitPoints = 255;
                m_HitPoints = 255;
            }
        }

        public virtual void OnAfterUse(Mobile m)
        {
            m_ChargeTime = m_MaxChargeTime;

            if (m_Charges > 0 && m_MaxCharges > 0)
                m_Charges -= 1;

            if (m_ChargeTime > 0)
                StartTimer();

            InvalidateProperties();
        }

        public virtual Type GetSummoner()
        {
            return null;
        }

        public virtual void SetSummoner(Type type, TextDefinition name)
        {
            m_Summoner = new TalismanAttribute(type, name);
        }

        public virtual void SetProtection(Type type, TextDefinition name, int amount)
        {
            m_Protection = new TalismanAttribute(type, name, amount);
        }

        public virtual void SetKiller(Type type, TextDefinition name, int amount)
        {
            m_Killer = new TalismanAttribute(type, name, amount);
        }

        #region Timer
        private Timer m_Timer;

        public virtual void StartTimer()
        {
            if (m_Timer == null || !m_Timer.Running)
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), new TimerCallback(Slice));
        }

        public virtual void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public virtual void Slice()
        {
            if (m_ChargeTime - 10 > 0)
                m_ChargeTime -= 10;
            else
            {
                m_ChargeTime = 0;

                StopTimer();
            }

            InvalidateProperties();
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

        private static readonly SkillName[] m_SkillsOld = new SkillName[]
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

        private static readonly TalismanSkill[] m_Skills = new TalismanSkill[]
        {
            TalismanSkill.Alchemy,
            TalismanSkill.Blacksmithy,
            TalismanSkill.Fletching,
            TalismanSkill.Carpentry,
            TalismanSkill.Cartography,
            TalismanSkill.Cooking,
            TalismanSkill.Glassblowing,
            TalismanSkill.Inscription,
            TalismanSkill.Masonry,
            TalismanSkill.Tailoring,
            TalismanSkill.Tinkering,
        };

        public static TalismanSkill GetRandomSkill()
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

        #region Crafting Bonuses
        /// <summary>
        /// This should only be called for version 4 conversion from SkillName to CraftSystem
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public TalismanSkill GetTalismanSkill(SkillName skill)
        {
            switch (skill)
            {
                default:
                case SkillName.Alchemy: return TalismanSkill.Alchemy;
                case SkillName.Blacksmith: return TalismanSkill.Blacksmithy;
                case SkillName.Carpentry: return TalismanSkill.Carpentry;
                case SkillName.Cartography: return TalismanSkill.Cartography;
                case SkillName.Cooking: return TalismanSkill.Cooking;
                case SkillName.Fletching: return TalismanSkill.Fletching;
                case SkillName.Inscribe: return TalismanSkill.Inscription;
                case SkillName.Tailoring: return TalismanSkill.Tailoring;
                case SkillName.Tinkering: return TalismanSkill.Tinkering;
            }
        }

        public SkillName GetMainSkill()
        {
            switch (m_Skill)
            {
                default:
                case TalismanSkill.Alchemy: return SkillName.Alchemy;
                case TalismanSkill.Blacksmithy: return SkillName.Blacksmith;
                case TalismanSkill.Fletching: return SkillName.Fletching;
                case TalismanSkill.Carpentry: return SkillName.Carpentry;
                case TalismanSkill.Cartography: return SkillName.Cartography;
                case TalismanSkill.Cooking: return SkillName.Cooking;
                case TalismanSkill.Glassblowing: return SkillName.Alchemy;
                case TalismanSkill.Inscription: return SkillName.Inscribe;
                case TalismanSkill.Masonry: return SkillName.Carpentry;
                case TalismanSkill.Tailoring: return SkillName.Tailoring;
                case TalismanSkill.Tinkering: return SkillName.Tinkering;
            }
        }

        public int GetSkillLabel()
        {
            switch (m_Skill)
            {
                case TalismanSkill.Glassblowing: return 1072393;
                case TalismanSkill.Masonry: return 1072392;
                default: return AosSkillBonuses.GetLabel(GetMainSkill());
            }
        }

        public bool CheckSkill(CraftSystem system)
        {
            int idx = (int)m_Skill;

            return idx >= 0 && idx < CraftContext.Systems.Length && CraftContext.Systems[idx] == system;
        }
        #endregion

        private class TalismanTarget : Target
        {
            private readonly BaseTalisman m_Talisman;

            public TalismanTarget(BaseTalisman talisman)
                : base(12, false, TargetFlags.Beneficial)
            {
                m_Talisman = talisman;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (m_Talisman == null || m_Talisman.Deleted)
                    return;

                Mobile target = o as Mobile;

                if (from.Talisman != m_Talisman)
                    from.SendLocalizedMessage(502641); // You must equip this item to use it.
                else if (target == null)
                    from.SendLocalizedMessage(1046439); // That is not a valid target.
                else if (m_Talisman.ChargeTime > 0)
                    from.SendLocalizedMessage(1074882, m_Talisman.ChargeTime.ToString()); // You must wait ~1_val~ seconds for this to recharge.
                else if (m_Talisman.Charges == 0 && m_Talisman.MaxCharges > 0)
                    from.SendLocalizedMessage(1042544); // This item is out of charges.
                else
                {
                    switch (m_Talisman.Removal)
                    {
                        case TalismanRemoval.Curse:
                            target.PlaySound(0xF6);
                            target.PlaySound(0x1F7);
                            target.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);

                            IEntity mfrom = new Entity(Serial.Zero, new Point3D(target.X, target.Y, target.Z - 10), from.Map);
                            IEntity mto = new Entity(Serial.Zero, new Point3D(target.X, target.Y, target.Z + 50), from.Map);
                            Effects.SendMovingParticles(mfrom, mto, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

                            WeakenSpell.RemoveEffects(target);
                            FeeblemindSpell.RemoveEffects(target);
                            ClumsySpell.RemoveEffects(target);
                            CurseSpell.RemoveEffect(target);

                            target.Paralyzed = false;

                            EvilOmenSpell.TryEndEffect(target);
                            StrangleSpell.RemoveCurse(target);
                            CorpseSkinSpell.RemoveCurse(target);
                            CurseSpell.RemoveEffect(target);

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

                            EodonianPotion.RemoveEffects(target, PotionEffect.Barrab);
                            EodonianPotion.RemoveEffects(target, PotionEffect.Urali);

                            target.SendLocalizedMessage(1072402); // Your wards have been removed!

                            if (target != from)
                                from.SendLocalizedMessage(1072403); // Your target's wards have been removed!

                            break;
                        case TalismanRemoval.Wildfire:
                            // TODO
                            break;
                    }

                    m_Talisman.OnAfterUse(from);
                }
            }
        }
    }
}