#region References
using Server.Commands;
using Server.Engines.Craft;
using Server.Multis;
using Server.Network;
using Server.Spells;
using Server.Spells.Mysticism;
using Server.Targeting;
using System;
using System.Collections.Generic;
#endregion

namespace Server.Items
{
    public enum SpellbookType
    {
        Invalid = -1,
        Regular,
        Necromancer,
        Paladin,
        Ninja,
        Samurai,
        Arcanist,
        Mystic,
        SkillMasteries,
    }

    public enum BookQuality
    {
        Regular,
        Exceptional
    }

    public class Spellbook : Item, ICraftable, ISlayer, IEngravable, IVvVItem, IOwnerRestricted, IWearableDurability
    {
        private static readonly Dictionary<Mobile, List<Spellbook>> m_Table = new Dictionary<Mobile, List<Spellbook>>();

        private static readonly int[] m_LegendPropertyCounts = new[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0 properties : 21/52 : 40%
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 1 property   : 15/52 : 29%
			2, 2, 2, 2, 2, 2, 2, 2, 2, 2, // 2 properties : 10/52 : 19%
			3, 3, 3, 3, 3, 3 // 3 properties :  6/52 : 12%
		};

        private static readonly int[] m_ElderPropertyCounts = new[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0 properties : 15/34 : 44%
			1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 1 property   : 10/34 : 29%
			2, 2, 2, 2, 2, 2, // 2 properties :  6/34 : 18%
			3, 3, 3 // 3 properties :  3/34 :  9%
		};

        private static readonly int[] m_GrandPropertyCounts = new[]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0 properties : 10/20 : 50%
			1, 1, 1, 1, 1, 1, // 1 property   :  6/20 : 30%
			2, 2, 2, // 2 properties :  3/20 : 15%
			3 // 3 properties :  1/20 :  5%
		};

        private static readonly int[] m_MasterPropertyCounts = new[]
        {
            0, 0, 0, 0, 0, 0, // 0 properties : 6/10 : 60%
			1, 1, 1, // 1 property   : 3/10 : 30%
			2 // 2 properties : 1/10 : 10%
		};

        private static readonly int[] m_AdeptPropertyCounts = new[]
        {
            0, 0, 0, // 0 properties : 3/4 : 75%
			1 // 1 property   : 1/4 : 25%
		};

        private string m_EngravedText;
        private BookQuality m_Quality;
        private AosAttributes m_AosAttributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private NegativeAttributes m_NegativeAttributes;
        private ulong m_Content;
        private int m_Count;
        private Mobile m_Crafter;
        private SlayerName m_Slayer;
        private SlayerName m_Slayer2;

        [Constructable]
        public Spellbook()
            : this((ulong)0)
        { }

        [Constructable]
        public Spellbook(ulong content)
            : this(content, 0xEFA)
        { }

        public Spellbook(ulong content, int itemID)
            : base(itemID)
        {
            m_AosAttributes = new AosAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);
            m_NegativeAttributes = new NegativeAttributes(this);

            Weight = 3.0;
            Layer = Layer.OneHanded;
            LootType = LootType.Blessed;

            Content = content;
        }

        public Spellbook(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set
            {
                m_EngravedText = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BookQuality Quality
        {
            get { return m_Quality; }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        public override bool DisplayWeight => false;

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes { get { return m_AosAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SkillBonuses { get { return m_AosSkillBonuses; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NegativeAttributes NegativeAttributes { get { return m_NegativeAttributes; } set { } }

        public virtual SpellbookType SpellbookType => SpellbookType.Regular;
        public virtual int BookOffset => 0;
        public virtual int BookCount => 64;

        [CommandProperty(AccessLevel.GameMaster)]
        public ulong Content
        {
            get { return m_Content; }
            set
            {
                if (m_Content != value)
                {
                    m_Content = value;

                    m_Count = 0;

                    while (value > 0)
                    {
                        m_Count += (int)(value & 0x1);
                        value >>= 1;
                    }

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpellCount => m_Count;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        public override bool DisplayLootType => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer
        {
            get { return m_Slayer; }
            set
            {
                m_Slayer = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer2
        {
            get { return m_Slayer2; }
            set
            {
                m_Slayer2 = value;
                InvalidateProperties();
            }
        }

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

        private int m_MaxHitPoints;
        private int m_HitPoints;

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_HitPoints; }
            set
            {
                if (m_HitPoints == value)
                {
                    return;
                }

                if (value > m_MaxHitPoints)
                {
                    value = m_MaxHitPoints;
                }

                m_HitPoints = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHitPoints; }
            set
            {
                m_MaxHitPoints = value;
                InvalidateProperties();
            }
        }

        public virtual bool CanFortify => false;

        public virtual int InitMinHits => 0;
        public virtual int InitMaxHits => 0;

        public virtual void ScaleDurability()
        {
        }

        public virtual void UnscaleDurability()
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
                        ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.

                    if (m_MaxHitPoints == 0)
                    {
                        Delete();
                    }
                }
            }

            return damage;
        }

        public static void Initialize()
        {
            EventSink.OpenSpellbookRequest += EventSink_OpenSpellbookRequest;
            EventSink.CastSpellRequest += EventSink_CastSpellRequest;
            EventSink.TargetedSpell += Targeted_Spell;

            CommandSystem.Register("AllSpells", AccessLevel.GameMaster, AllSpells_OnCommand);
        }

        #region Enhanced Client
        private static void Targeted_Spell(TargetedSpellEventArgs e)
        {
            try
            {
                Mobile from = e.Mobile;

                if (!DesignContext.Check(from))
                {
                    return; // They are customizing
                }

                int spellID = e.SpellID;

                Spellbook book = Find(from, spellID);

                if (book != null && book.HasSpell(spellID))
                {
                    SpecialMove move = SpellRegistry.GetSpecialMove(spellID);

                    if (move != null)
                    {
                        SpecialMove.SetCurrentMove(from, move);
                    }
                    else if (e.Target != null)
                    {
                        Mobile to = World.FindMobile(e.Target.Serial);
                        Item toI = World.FindItem(e.Target.Serial);
                        Spell spell = SpellRegistry.NewSpell(spellID, from, null);

                        if (spell != null && !Spells.SkillMasteries.MasteryInfo.IsPassiveMastery(spellID))
                        {
                            if (to != null)
                            {
                                spell.InstantTarget = to;
                            }
                            else if (toI != null)
                            {
                                spell.InstantTarget = toI as IDamageableItem;
                            }

                            spell.Cast();
                        }
                    }
                }
                else
                {
                    from.SendLocalizedMessage(500015); // You do not have that spell!
                }
            }
            catch (Exception ex)
            {
                Diagnostics.ExceptionLogging.LogException(ex);
            }
        }
        #endregion

        public static SpellbookType GetTypeForSpell(int spellID)
        {
            if (spellID >= 0 && spellID < 64)
            {
                return SpellbookType.Regular;
            }
            else if (spellID >= 100 && spellID < 117)
            {
                return SpellbookType.Necromancer;
            }
            else if (spellID >= 200 && spellID < 210)
            {
                return SpellbookType.Paladin;
            }
            else if (spellID >= 400 && spellID < 406)
            {
                return SpellbookType.Samurai;
            }
            else if (spellID >= 500 && spellID < 508)
            {
                return SpellbookType.Ninja;
            }
            else if (spellID >= 600 && spellID < 617)
            {
                return SpellbookType.Arcanist;
            }
            else if (spellID >= 677 && spellID < 693)
            {
                return SpellbookType.Mystic;
            }
            else if (spellID >= 700 && spellID < 746)
            {
                return SpellbookType.SkillMasteries;
            }

            return SpellbookType.Invalid;
        }

        public static Spellbook FindRegular(Mobile from)
        {
            return Find(from, -1, SpellbookType.Regular);
        }

        public static Spellbook FindNecromancer(Mobile from)
        {
            return Find(from, -1, SpellbookType.Necromancer);
        }

        public static Spellbook FindPaladin(Mobile from)
        {
            return Find(from, -1, SpellbookType.Paladin);
        }

        public static Spellbook FindSamurai(Mobile from)
        {
            return Find(from, -1, SpellbookType.Samurai);
        }

        public static Spellbook FindNinja(Mobile from)
        {
            return Find(from, -1, SpellbookType.Ninja);
        }

        public static Spellbook FindArcanist(Mobile from)
        {
            return Find(from, -1, SpellbookType.Arcanist);
        }

        public static Spellbook FindMystic(Mobile from)
        {
            return Find(from, -1, SpellbookType.Mystic);
        }

        public static Spellbook Find(Mobile from, int spellID)
        {
            return Find(from, spellID, GetTypeForSpell(spellID));
        }

        public static Spellbook Find(Mobile from, int spellID, SpellbookType type)
        {
            if (from == null)
            {
                return null;
            }

            if (from.Deleted)
            {
                m_Table.Remove(from);
                return null;
            }

            List<Spellbook> list = null;

            m_Table.TryGetValue(from, out list);

            bool searchAgain = false;

            if (list == null)
            {
                m_Table[from] = list = FindAllSpellbooks(from);
            }
            else
            {
                searchAgain = true;
            }

            Spellbook book = FindSpellbookInList(list, from, spellID, type);

            if (book == null && searchAgain)
            {
                m_Table[from] = list = FindAllSpellbooks(from);

                book = FindSpellbookInList(list, from, spellID, type);
            }

            return book;
        }

        public static Spellbook FindSpellbookInList(List<Spellbook> list, Mobile from, int spellID, SpellbookType type)
        {
            Container pack = from.Backpack;

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (i >= list.Count)
                {
                    continue;
                }

                Spellbook book = list[i];

                if (!book.Deleted && (book.Parent == from || (pack != null && book.Parent == pack)) &&
                    ValidateSpellbook(book, spellID, type))
                {
                    return book;
                }

                list.RemoveAt(i);
            }

            return null;
        }

        public static List<Spellbook> FindAllSpellbooks(Mobile from)
        {
            List<Spellbook> list = new List<Spellbook>();

            Item item = from.FindItemOnLayer(Layer.OneHanded);

            if (item is Spellbook)
            {
                list.Add((Spellbook)item);
            }

            Container pack = from.Backpack;

            if (pack == null)
            {
                return list;
            }

            for (int i = 0; i < pack.Items.Count; ++i)
            {
                item = pack.Items[i];

                if (item is Spellbook)
                {
                    list.Add((Spellbook)item);
                }
            }

            return list;
        }

        public static Spellbook FindEquippedSpellbook(Mobile from)
        {
            return (from.FindItemOnLayer(Layer.OneHanded) as Spellbook);
        }

        public static bool ValidateSpellbook(Spellbook book, int spellID, SpellbookType type)
        {
            return (book.SpellbookType == type && (spellID == -1 || book.HasSpell(spellID)));
        }

        public override bool CanEquip(Mobile from)
        {
            if (!from.CanBeginAction(typeof(BaseWeapon)))
            {
                return false;
            }
            else if (_Owner != null && _Owner != from)
            {
                from.SendLocalizedMessage(501023); // You must be the owner to use this item.
                return false;
            }
            else if (IsVvVItem && !Engines.VvV.ViceVsVirtueSystem.IsVvV(from))
            {
                from.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                return false;
            }

            return base.CanEquip(from);
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is SpellScroll && !(dropped is SpellStone))
            {
                SpellScroll scroll = (SpellScroll)dropped;

                SpellbookType type = GetTypeForSpell(scroll.SpellID);

                if (type != SpellbookType)
                {
                    return false;
                }
                else if (HasSpell(scroll.SpellID))
                {
                    from.SendLocalizedMessage(500179); // That spell is already present in that spellbook.
                    return false;
                }
                else
                {
                    int val = scroll.SpellID - BookOffset;

                    if (val >= 0 && val < BookCount)
                    {
                        from.Send(new PlaySound(0x249, GetWorldLocation()));

                        m_Content |= (ulong)1 << val;
                        ++m_Count;

                        if (dropped.Amount > 1)
                        {
                            dropped.Amount--;
                            return base.OnDragDrop(from, dropped);
                        }
                        else
                        {
                            InvalidateProperties();
                            scroll.Delete();
                            return true;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        public override void OnAfterDuped(Item newItem)
        {
            Spellbook book = newItem as Spellbook;

            if (book == null)
            {
                return;
            }

            book.m_AosAttributes = new AosAttributes(newItem, m_AosAttributes);
            book.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
            book.m_NegativeAttributes = new NegativeAttributes(newItem, m_NegativeAttributes);

            base.OnAfterDuped(newItem);
        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.AddTo(from);

                int strBonus = m_AosAttributes.BonusStr;
                int dexBonus = m_AosAttributes.BonusDex;
                int intBonus = m_AosAttributes.BonusInt;

                if (strBonus != 0 || dexBonus != 0 || intBonus != 0)
                {
                    string modName = Serial.ToString();

                    if (strBonus != 0)
                    {
                        from.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));
                    }

                    if (dexBonus != 0)
                    {
                        from.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));
                    }

                    if (intBonus != 0)
                    {
                        from.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
                    }
                }

                if (HasSocket<Caddellite>())
                {
                    Caddellite.UpdateBuff(from);
                }

                from.CheckStatTimers();
            }
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.Remove();

                if (HasSocket<Caddellite>())
                {
                    Caddellite.UpdateBuff(from);
                }

                string modName = Serial.ToString();

                from.RemoveStatMod(modName + "Str");
                from.RemoveStatMod(modName + "Dex");
                from.RemoveStatMod(modName + "Int");

                from.CheckStatTimers();
            }
        }

        public bool HasSpell(int spellID)
        {
            spellID -= BookOffset;

            return (spellID >= 0 && spellID < BookCount && (m_Content & ((ulong)1 << spellID)) != 0);
        }

        public void DisplayTo(Mobile to)
        {
            // The client must know about the spellbook or it will crash!
            NetState ns = to.NetState;

            if (ns == null)
            {
                return;
            }

            if (Parent == null)
            {
                to.Send(WorldPacket);
            }
            else if (Parent is Item)
            {
                to.Send(new ContainerContentUpdate(this));
            }
            else if (Parent is Mobile)
            {
                // What will happen if the client doesn't know about our parent?
                to.Send(new EquipUpdate(this));
            }

            to.Send(new DisplaySpellbook(this));

            to.Send(new SpellbookContent(this, ItemID, BookOffset + 1, m_Content));
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (m_Quality == BookQuality.Exceptional)
            {
                list.Add(1063341); // exceptional
            }

            if (m_EngravedText != null)
            {
                list.Add(1072305, Utility.FixHtml(m_EngravedText)); // Engraved: ~1_INSCRIPTION~
            }

            if (m_Crafter != null)
            {
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
            }

            if (IsVvVItem)
            {
                list.Add(1154937); // VvV Item
            }

            if (OwnerName != null)
            {
                list.Add(1153213, OwnerName);
            }

            if (m_NegativeAttributes != null)
            {
                m_NegativeAttributes.GetProperties(list, this);
            }

            m_AosSkillBonuses.GetProperties(list);

            if (m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
                if (entry != null)
                {
                    list.Add(entry.Title);
                }
            }

            if (m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
                if (entry != null)
                {
                    list.Add(entry.Title);
                }
            }

            if (HasSocket<Caddellite>())
            {
                list.Add(1158662); // Caddellite Infused
            }

            int prop;

            if ((prop = m_AosAttributes.SpellChanneling) != 0)
            {
                list.Add(1060482); // spell channeling
            }

            if ((prop = m_AosAttributes.NightSight) != 0)
            {
                list.Add(1060441); // night sight
            }

            if ((prop = m_AosAttributes.BonusStr) != 0)
            {
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~
            }

            if ((prop = m_AosAttributes.BonusDex) != 0)
            {
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~
            }

            if ((prop = m_AosAttributes.BonusInt) != 0)
            {
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~
            }

            if ((prop = m_AosAttributes.BonusHits) != 0)
            {
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~
            }

            if ((prop = m_AosAttributes.BonusStam) != 0)
            {
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~
            }

            if ((prop = m_AosAttributes.BonusMana) != 0)
            {
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~
            }

            if ((prop = m_AosAttributes.RegenHits) != 0)
            {
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~
            }

            if ((prop = m_AosAttributes.RegenStam) != 0)
            {
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~
            }

            if ((prop = m_AosAttributes.RegenMana) != 0)
            {
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~
            }

            if ((prop = m_AosAttributes.Luck) != 0)
            {
                list.Add(1060436, prop.ToString()); // luck ~1_val~
            }

            if ((prop = m_AosAttributes.EnhancePotions) != 0)
            {
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%
            }

            if ((prop = m_AosAttributes.ReflectPhysical) != 0)
            {
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%
            }

            if ((prop = m_AosAttributes.AttackChance) != 0)
            {
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%
            }

            if ((prop = m_AosAttributes.WeaponSpeed) != 0)
            {
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%
            }

            if ((prop = m_AosAttributes.WeaponDamage) != 0)
            {
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%
            }

            if ((prop = m_AosAttributes.DefendChance) != 0)
            {
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%
            }

            if ((prop = m_AosAttributes.CastRecovery) != 0)
            {
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~
            }

            if ((prop = m_AosAttributes.CastSpeed) != 0)
            {
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~
            }

            if ((prop = m_AosAttributes.SpellDamage) != 0)
            {
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%
            }

            if ((prop = m_AosAttributes.LowerManaCost) != 0)
            {
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%
            }

            if ((prop = m_AosAttributes.LowerRegCost) != 0)
            {
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%
            }

            if ((prop = m_AosAttributes.IncreasedKarmaLoss) != 0)
            {
                list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%
            }

            AddProperty(list);

            list.Add(1042886, m_Count.ToString()); // ~1_NUMBERS_OF_SPELLS~ Spells

            if (m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints); // durability ~1_val~ / ~2_val~
        }

        public virtual void AddProperty(ObjectPropertyList list)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            Container pack = from.Backpack;

            if (Parent == from || (pack != null && Parent == pack))
            {
                DisplayTo(from);
            }
            else
            {
                from.SendLocalizedMessage(500207);
                // The spellbook must be in your backpack (and not in a container within) to open.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(6); // version

            m_NegativeAttributes.Serialize(writer);

            writer.Write(m_HitPoints);
            writer.Write(m_MaxHitPoints);

            writer.Write(_VvVItem);
            writer.Write(_Owner);
            writer.Write(_OwnerName);

            writer.Write((byte)m_Quality);

            writer.Write(m_EngravedText);

            writer.Write(m_Crafter);

            writer.Write((int)m_Slayer);
            writer.Write((int)m_Slayer2);

            m_AosAttributes.Serialize(writer);
            m_AosSkillBonuses.Serialize(writer);

            writer.Write(m_Content);
            writer.Write(m_Count);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 6:
                    {
                        m_NegativeAttributes = new NegativeAttributes(this, reader);


                        m_MaxHitPoints = reader.ReadInt();
                        m_HitPoints = reader.ReadInt();

                        _VvVItem = reader.ReadBool();
                        _Owner = reader.ReadMobile();
                        _OwnerName = reader.ReadString();

                        goto case 5;
                    }
                case 5:
                    {
                        m_Quality = (BookQuality)reader.ReadByte();

                        goto case 4;
                    }
                case 4:
                    {
                        m_EngravedText = reader.ReadString();

                        goto case 3;
                    }
                case 3:
                    {
                        m_Crafter = reader.ReadMobile();
                        goto case 2;
                    }
                case 2:
                    {
                        m_Slayer = (SlayerName)reader.ReadInt();
                        m_Slayer2 = (SlayerName)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_AosAttributes = new AosAttributes(this, reader);
                        m_AosSkillBonuses = new AosSkillBonuses(this, reader);

                        goto case 0;
                    }
                case 0:
                    {
                        m_Content = reader.ReadULong();
                        m_Count = reader.ReadInt();

                        break;
                    }
            }

            if (m_AosAttributes == null)
            {
                m_AosAttributes = new AosAttributes(this);
            }

            if (m_AosSkillBonuses == null)
            {
                m_AosSkillBonuses = new AosSkillBonuses(this);
            }

            if (m_NegativeAttributes == null)
            {
                m_NegativeAttributes = new NegativeAttributes(this);
            }

            if (Parent is Mobile)
            {
                m_AosSkillBonuses.AddTo((Mobile)Parent);
            }

            int strBonus = m_AosAttributes.BonusStr;
            int dexBonus = m_AosAttributes.BonusDex;
            int intBonus = m_AosAttributes.BonusInt;

            if (Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0))
            {
                Mobile m = (Mobile)Parent;

                string modName = Serial.ToString();

                if (strBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));
                }

                if (dexBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));
                }

                if (intBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
                }
            }

            if (Parent is Mobile)
            {
                ((Mobile)Parent).CheckStatTimers();
            }
        }

        public virtual int OnCraft(
            int quality,
            bool makersMark,
            Mobile from,
            CraftSystem craftSystem,
            Type typeRes,
            ITool tool,
            CraftItem craftItem,
            int resHue)
        {
            int magery = from.Skills.Magery.BaseFixedPoint;

            if (magery >= 800)
            {
                int[] propertyCounts;
                int minIntensity;
                int maxIntensity;

                if (magery >= 1000)
                {
                    if (magery >= 1200)
                    {
                        propertyCounts = m_LegendPropertyCounts;
                    }
                    else if (magery >= 1100)
                    {
                        propertyCounts = m_ElderPropertyCounts;
                    }
                    else
                    {
                        propertyCounts = m_GrandPropertyCounts;
                    }

                    minIntensity = 55;
                    maxIntensity = 75;
                }
                else if (magery >= 900)
                {
                    propertyCounts = m_MasterPropertyCounts;
                    minIntensity = 25;
                    maxIntensity = 45;
                }
                else
                {
                    propertyCounts = m_AdeptPropertyCounts;
                    minIntensity = 0;
                    maxIntensity = 15;
                }

                int propertyCount = propertyCounts[Utility.Random(propertyCounts.Length)];

                GuaranteedSpellbookImprovementTalisman talisman = from.FindItemOnLayer(Layer.Talisman) as GuaranteedSpellbookImprovementTalisman;

                if (talisman != null && talisman.Charges > 0)
                {
                    propertyCount++;
                    talisman.Charges--;

                    from.SendLocalizedMessage(1157210); // Your talisman magically improves your spellbook.

                    if (talisman.Charges <= 0)
                    {
                        from.SendLocalizedMessage(1157211); // Your talisman has been destroyed.
                        talisman.Delete();
                    }
                }

                BaseRunicTool.ApplyAttributesTo(this, true, 0, propertyCount, minIntensity, maxIntensity);
            }

            if (makersMark)
            {
                Crafter = from;
            }

            m_Quality = (BookQuality)(quality - 1);

            return quality;
        }

        [Usage("AllSpells")]
        [Description("Completely fills a targeted spellbook with scrolls.")]
        private static void AllSpells_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, AllSpells_OnTarget);
            e.Mobile.SendMessage("Target the spellbook to fill.");
        }

        private static void AllSpells_OnTarget(Mobile from, object obj)
        {
            if (obj is Spellbook)
            {
                Spellbook book = (Spellbook)obj;

                if (book.BookCount == 64)
                {
                    book.Content = ulong.MaxValue;
                }
                else
                {
                    book.Content = (1ul << book.BookCount) - 1;
                }

                from.SendMessage("The spellbook has been filled.");

                CommandLogging.WriteLine(
                    from, "{0} {1} filling spellbook {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(book));
            }
            else
            {
                from.BeginTarget(-1, false, TargetFlags.None, AllSpells_OnTarget);
                from.SendMessage("That is not a spellbook. Try again.");
            }
        }

        private static void EventSink_OpenSpellbookRequest(OpenSpellbookRequestEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!DesignContext.Check(from))
            {
                return; // They are customizing
            }

            SpellbookType type;

            switch (e.Type)
            {
                default:
                case 1:
                    type = SpellbookType.Regular;
                    break;
                case 2:
                    type = SpellbookType.Necromancer;
                    break;
                case 3:
                    type = SpellbookType.Paladin;
                    break;
                case 4:
                    type = SpellbookType.Ninja;
                    break;
                case 5:
                    type = SpellbookType.Samurai;
                    break;
                case 6:
                    type = SpellbookType.Arcanist;
                    break;
                case 7:
                    type = SpellbookType.Mystic;
                    break;
            }

            Spellbook book = Find(from, -1, type);

            if (book != null)
            {
                book.DisplayTo(from);
            }
        }

        private static void EventSink_CastSpellRequest(CastSpellRequestEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!DesignContext.Check(from))
            {
                return; // They are customizing
            }

            Spellbook book = e.Spellbook as Spellbook;
            int spellID = e.SpellID;

            if (book == null || !book.HasSpell(spellID))
            {
                book = Find(from, spellID);
            }

            if (book != null && book.HasSpell(spellID))
            {
                SpecialMove move = SpellRegistry.GetSpecialMove(spellID);

                if (move != null)
                {
                    SpecialMove.SetCurrentMove(from, move);
                }
                else
                {
                    Spell spell = SpellRegistry.NewSpell(spellID, from, null);

                    if (spell != null)
                    {
                        spell.Cast();
                    }
                    else if (!Spells.SkillMasteries.MasteryInfo.IsPassiveMastery(spellID))
                    {
                        from.SendLocalizedMessage(502345); // This spell has been temporarily disabled.
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage(500015); // You do not have that spell!
            }
        }
    }
}
