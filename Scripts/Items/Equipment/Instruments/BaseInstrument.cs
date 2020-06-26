using Server.Engines.Craft;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections;

namespace Server.Items
{
    public delegate void InstrumentPickedCallback(Mobile from, BaseInstrument instrument);

    public abstract class BaseInstrument : Item, ISlayer, IQuality, IResource
    {
        public static readonly double MaxBardingDifficulty = 160.0;

        private int m_WellSound, m_BadlySound;
        private SlayerName m_Slayer, m_Slayer2;
        private ItemQuality m_Quality;
        private Mobile m_Crafter;
        private int m_UsesRemaining;
        private CraftResource m_Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SuccessSound
        {
            get
            {
                return m_WellSound;
            }
            set
            {
                m_WellSound = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FailureSound
        {
            get
            {
                return m_BadlySound;
            }
            set
            {
                m_BadlySound = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer
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

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer2
        {
            get
            {
                return m_Slayer2;
            }
            set
            {
                m_Slayer2 = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get
            {
                return m_Quality;
            }
            set
            {
                UnscaleUses();
                m_Quality = value;
                InvalidateProperties();
                ScaleUses();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed => m_Crafter != null;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return m_Crafter;
            }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                m_Resource = value;
                Hue = CraftResources.GetHue(m_Resource);
                InvalidateProperties();
            }
        }

        public virtual int InitMinUses => 350;
        public virtual int InitMaxUses => 450;

        public virtual TimeSpan ChargeReplenishRate => TimeSpan.FromMinutes(5.0);

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                CheckReplenishUses();
                return m_UsesRemaining;
            }
            set
            {
                m_UsesRemaining = value;
                InvalidateProperties();
            }
        }

        private DateTime m_LastReplenished;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastReplenished
        {
            get
            {
                return m_LastReplenished;
            }
            set
            {
                m_LastReplenished = value;
                CheckReplenishUses();
            }
        }

        private bool m_ReplenishesCharges;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ReplenishesCharges
        {
            get
            {
                return m_ReplenishesCharges;
            }
            set
            {
                if (value != m_ReplenishesCharges && value)
                    m_LastReplenished = DateTime.UtcNow;

                m_ReplenishesCharges = value;
            }
        }

        public void RandomInstrument()
        {
            switch (Utility.Random(3))
            {
                case 0:
                    {
                        ItemID = 0xEB2;
                        SuccessSound = 0x45;
                        FailureSound = 0x46;
                        break;
                    }
                case 1:
                    {
                        ItemID = 0xEB3;
                        SuccessSound = 0x4C;
                        FailureSound = 0x4D;
                        break;
                    }
                default:
                    {
                        ItemID = 0xE9C;
                        SuccessSound = 0x38;
                        FailureSound = 0x39;
                        break;
                    }
            }
        }

        public void CheckReplenishUses()
        {
            CheckReplenishUses(true);
        }

        public void CheckReplenishUses(bool invalidate)
        {
            if (!m_ReplenishesCharges || m_UsesRemaining >= InitMaxUses)
                return;

            if (m_LastReplenished + ChargeReplenishRate < DateTime.UtcNow)
            {
                TimeSpan timeDifference = DateTime.UtcNow - m_LastReplenished;

                m_UsesRemaining = Math.Min(m_UsesRemaining + (int)(timeDifference.Ticks / ChargeReplenishRate.Ticks), InitMaxUses);	//How rude of TimeSpan to not allow timespan division.
                m_LastReplenished = DateTime.UtcNow;

                if (invalidate)
                    InvalidateProperties();
            }
        }

        public void ScaleUses()
        {
            UsesRemaining = (UsesRemaining * GetUsesScalar()) / 100;
            //InvalidateProperties();
        }

        public void UnscaleUses()
        {
            UsesRemaining = (UsesRemaining * 100) / GetUsesScalar();
        }

        public int GetUsesScalar()
        {
            if (m_Quality == ItemQuality.Exceptional)
                return 200;

            return 100;
        }

        public void ConsumeUse(Mobile from)
        {
            // TODO: Confirm what must happen here?
            if (UsesRemaining > 1)
            {
                --UsesRemaining;
            }
            else
            {
                if (from != null)
                    from.SendLocalizedMessage(502079); // The instrument played its last tune.

                Delete();
            }
        }

        private static readonly Hashtable m_Instruments = new Hashtable();

        public static BaseInstrument GetInstrument(Mobile from)
        {
            BaseInstrument item = m_Instruments[from] as BaseInstrument;

            if (item == null)
                return null;

            if (!item.IsChildOf(from.Backpack))
            {
                m_Instruments.Remove(from);
                return null;
            }

            return item;
        }

        public static int GetBardRange(Mobile bard, SkillName skill)
        {
            return 8 + (int)(bard.Skills[skill].Value / 15);
        }

        public static void PickInstrument(Mobile from, InstrumentPickedCallback callback)
        {
            BaseInstrument instrument = GetInstrument(from);

            if (instrument != null)
            {
                if (callback != null)
                    callback(from, instrument);
            }
            else
            {
                from.SendLocalizedMessage(500617); // What instrument shall you play?
                from.BeginTarget(1, false, TargetFlags.None, new TargetStateCallback(OnPickedInstrument), callback);
            }
        }

        public static void OnPickedInstrument(Mobile from, object targeted, object state)
        {
            BaseInstrument instrument = targeted as BaseInstrument;

            if (instrument == null)
            {
                from.SendLocalizedMessage(500619); // That is not a musical instrument.
            }
            else
            {
                SetInstrument(from, instrument);

                InstrumentPickedCallback callback = state as InstrumentPickedCallback;

                if (callback != null)
                    callback(from, instrument);
            }
        }

        public static bool IsMageryCreature(BaseCreature bc)
        {
            return (bc != null && bc.AI == AIType.AI_Mage && bc.Skills[SkillName.Magery].Base > 5.0);
        }

        public static bool IsFireBreathingCreature(BaseCreature bc)
        {
            if (bc == null)
                return false;

            AbilityProfile profile = bc.AbilityProfile;

            if (profile != null)
            {
                return profile.HasAbility(SpecialAbility.DragonBreath);
            }

            return false;
        }

        public static bool IsPoisonImmune(BaseCreature bc)
        {
            return (bc != null && bc.PoisonImmune != null);
        }

        public static int GetPoisonLevel(BaseCreature bc)
        {
            if (bc == null)
                return 0;

            Poison p = bc.HitPoison;

            if (p == null)
                return 0;

            return p.Level + 1;
        }

        public static double GetBaseDifficulty(Mobile targ)
        {
            /* Difficulty TODO: Add another 100 points for each of the following abilities:
            - Radiation or Aura Damage (Heat, Cold etc.)
            - Summoning Undead
            */
            double val = (targ.HitsMax * 1.6) + targ.StamMax + targ.ManaMax;

            val += targ.SkillsTotal / 10;

            BaseCreature bc = targ as BaseCreature;

            if (IsMageryCreature(bc))
                val += 100;

            if (IsFireBreathingCreature(bc))
                val += 100;

            if (IsPoisonImmune(bc))
                val += 100;

            if (targ is VampireBat || targ is VampireBatFamiliar)
                val += 100;

            val += GetPoisonLevel(bc) * 20;

            if (val > 700)
                val = 700 + (int)((val - 700) * (3.0 / 11));

            val /= 10;

            if (bc != null && bc.IsParagon)
                val += 40.0;

            if (val > MaxBardingDifficulty)
                val = MaxBardingDifficulty;

            return val;
        }

        public double GetDifficultyFor(Mobile targ)
        {
            double val = GetBaseDifficulty(targ);

            if (m_Quality == ItemQuality.Exceptional)
                val -= 5.0; // 10%

            if (m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);

                if (entry != null)
                {
                    if (entry.Slays(targ))
                        val -= 10.0; // 20%
                    else if (entry.Group.OppositionSuperSlays(targ))
                        val += 10.0; // -20%
                }
            }

            if (m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);

                if (entry != null)
                {
                    if (entry.Slays(targ))
                        val -= 10.0; // 20%
                    else if (entry.Group.OppositionSuperSlays(targ))
                        val += 10.0; // -20%
                }
            }

            if (m_Slayer == SlayerName.None && m_Slayer2 == SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(SlayerSocket.GetSlayer(this));

                if (entry != null)
                {
                    if (entry.Slays(targ))
                        val -= 10.0; // 20%
                    else if (entry.Group.OppositionSuperSlays(targ))
                        val += 10.0; // -20%
                }
            }

            return val;
        }

        public static void SetInstrument(Mobile from, BaseInstrument item)
        {
            m_Instruments[from] = item;
        }

        public BaseInstrument()
        {
            RandomInstrument();

            UsesRemaining = Utility.RandomMinMax(InitMinUses, InitMaxUses);
        }

        public BaseInstrument(int itemID, int wellSound, int badlySound)
            : base(itemID)
        {
            m_WellSound = wellSound;
            m_BadlySound = badlySound;

            UsesRemaining = Utility.RandomMinMax(InitMinUses, InitMaxUses);
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Quality == ItemQuality.Exceptional)
                list.Add(1060636); // exceptional
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            list.Add(1060584, UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            int oldUses = m_UsesRemaining;
            CheckReplenishUses(false);

            base.GetProperties(list);

            if (m_ReplenishesCharges)
                list.Add(1070928); // Replenish Charges

            if (m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
                if (entry != null)
                    list.Add(entry.Title);
            }

            if (m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
                if (entry != null)
                    list.Add(entry.Title);
            }

            if (!CraftResources.IsStandard(m_Resource))
            {
                int num = CraftResources.GetLocalizationNumber(m_Resource);

                if (num > 0)
                    list.Add(num);
                else
                    list.Add(CraftResources.GetName(m_Resource));
            }

            if (m_UsesRemaining != oldUses)
                Timer.DelayCall(TimeSpan.Zero, InvalidateProperties);
        }

        public BaseInstrument(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(4); // version

            writer.Write((int)m_Resource);

            writer.Write(m_ReplenishesCharges);
            if (m_ReplenishesCharges)
                writer.Write(m_LastReplenished);

            writer.Write(m_Crafter);

            writer.WriteEncodedInt((int)m_Quality);
            writer.WriteEncodedInt((int)m_Slayer);
            writer.WriteEncodedInt((int)m_Slayer2);

            writer.WriteEncodedInt(UsesRemaining);

            writer.WriteEncodedInt(m_WellSound);
            writer.WriteEncodedInt(m_BadlySound);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    {
                        m_Resource = (CraftResource)reader.ReadInt();
                        goto case 3;
                    }
                case 3:
                    {
                        m_ReplenishesCharges = reader.ReadBool();

                        if (m_ReplenishesCharges)
                            m_LastReplenished = reader.ReadDateTime();

                        goto case 2;
                    }
                case 2:
                    {
                        m_Crafter = reader.ReadMobile();

                        m_Quality = (ItemQuality)reader.ReadEncodedInt();
                        m_Slayer = (SlayerName)reader.ReadEncodedInt();
                        m_Slayer2 = (SlayerName)reader.ReadEncodedInt();

                        UsesRemaining = reader.ReadEncodedInt();

                        m_WellSound = reader.ReadEncodedInt();
                        m_BadlySound = reader.ReadEncodedInt();

                        break;
                    }
                case 1:
                    {
                        m_Crafter = reader.ReadMobile();

                        m_Quality = (ItemQuality)reader.ReadEncodedInt();
                        m_Slayer = (SlayerName)reader.ReadEncodedInt();

                        UsesRemaining = reader.ReadEncodedInt();

                        m_WellSound = reader.ReadEncodedInt();
                        m_BadlySound = reader.ReadEncodedInt();

                        break;
                    }
                case 0:
                    {
                        m_WellSound = reader.ReadInt();
                        m_BadlySound = reader.ReadInt();
                        UsesRemaining = Utility.RandomMinMax(InitMinUses, InitMaxUses);

                        break;
                    }
            }

            CheckReplenishUses();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (from.BeginAction(typeof(BaseInstrument)))
            {
                SetInstrument(from, this);

                Timer.DelayCall(TimeSpan.FromMilliseconds(1000), () =>
                {
                    from.EndAction(typeof(BaseInstrument));
                });

                if (CheckMusicianship(from))
                    PlayInstrumentWell(from);
                else
                    PlayInstrumentBadly(from);
            }
            else
            {
                from.SendLocalizedMessage(500119); // You must wait to perform another action
            }
        }

        public static bool CheckMusicianship(Mobile m)
        {
            m.CheckSkill(SkillName.Musicianship, 0.0, 120.0);

            return ((m.Skills[SkillName.Musicianship].Value / 100) > Utility.RandomDouble());
        }

        public virtual void PlayInstrumentWell(Mobile from)
        {
            from.PlaySound(m_WellSound);
        }

        public void PlayInstrumentBadly(Mobile from)
        {
            from.PlaySound(m_BadlySound);
        }

        #region ICraftable Members

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                    typeRes = craftItem.Resources.GetAt(0).ItemType;

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }
        #endregion
    }
}
