using System;
using System.Collections;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public delegate void InstrumentPickedCallback(Mobile from, BaseInstrument instrument);

    public enum InstrumentQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public abstract class BaseInstrument : Item, ICraftable, ISlayer
    {
        private int m_WellSound, m_BadlySound;
        private SlayerName m_Slayer, m_Slayer2;
        private InstrumentQuality m_Quality;
        private Mobile m_Crafter;
        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SuccessSound
        {
            get
            {
                return this.m_WellSound;
            }
            set
            {
                this.m_WellSound = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FailureSound
        {
            get
            {
                return this.m_BadlySound;
            }
            set
            {
                this.m_BadlySound = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer
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

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer2
        {
            get
            {
                return this.m_Slayer2;
            }
            set
            {
                this.m_Slayer2 = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public InstrumentQuality Quality
        {
            get
            {
                return this.m_Quality;
            }
            set
            {
                this.UnscaleUses();
                this.m_Quality = value;
                this.InvalidateProperties();
                this.ScaleUses();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }

        public virtual int InitMinUses
        {
            get
            {
                return 350;
            }
        }
        public virtual int InitMaxUses
        {
            get
            {
                return 450;
            }
        }

        public virtual TimeSpan ChargeReplenishRate
        {
            get
            {
                return TimeSpan.FromMinutes(5.0);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                this.CheckReplenishUses();
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }

        private DateTime m_LastReplenished;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastReplenished
        {
            get
            {
                return this.m_LastReplenished;
            }
            set
            {
                this.m_LastReplenished = value;
                this.CheckReplenishUses();
            }
        }

        private bool m_ReplenishesCharges;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ReplenishesCharges
        {
            get
            {
                return this.m_ReplenishesCharges;
            }
            set 
            {
                if (value != this.m_ReplenishesCharges && value)
                    this.m_LastReplenished = DateTime.UtcNow;

                this.m_ReplenishesCharges = value; 
            }
        }

        public void CheckReplenishUses()
        {
            this.CheckReplenishUses(true);
        }

        public void CheckReplenishUses(bool invalidate)
        {
            if (!this.m_ReplenishesCharges || this.m_UsesRemaining >= this.InitMaxUses)
                return;

            if (this.m_LastReplenished + this.ChargeReplenishRate < DateTime.UtcNow)
            {
                TimeSpan timeDifference = DateTime.UtcNow - this.m_LastReplenished;

                this.m_UsesRemaining = Math.Min(this.m_UsesRemaining + (int)(timeDifference.Ticks / this.ChargeReplenishRate.Ticks), this.InitMaxUses);	//How rude of TimeSpan to not allow timespan division.
                this.m_LastReplenished = DateTime.UtcNow;

                if (invalidate)
                    this.InvalidateProperties();
            }
        }

        public void ScaleUses()
        {
            this.UsesRemaining = (this.UsesRemaining * this.GetUsesScalar()) / 100;
            //InvalidateProperties();
        }

        public void UnscaleUses()
        {
            this.UsesRemaining = (this.UsesRemaining * 100) / this.GetUsesScalar();
        }

        public int GetUsesScalar()
        {
            if (this.m_Quality == InstrumentQuality.Exceptional)
                return 200;

            return 100;
        }

        public void ConsumeUse(Mobile from)
        {
            // TODO: Confirm what must happen here?
            if (this.UsesRemaining > 1)
            {
                --this.UsesRemaining;
            }
            else
            {
                if (from != null)
                    from.SendLocalizedMessage(502079); // The instrument played its last tune.

                this.Delete();
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

            return bc.HasBreath;
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

            if (val > 700)
                val = 700 + (int)((val - 700) * (3.0 / 11));

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

            val /= 10;

            if (bc != null && bc.IsParagon)
                val += 40.0;

            if (Core.SE && val > 160.0)
                val = 160.0;

            return val;
        }

        public double GetDifficultyFor(Mobile targ)
        {
            double val = GetBaseDifficulty(targ);

            if (this.m_Quality == InstrumentQuality.Exceptional)
                val -= 5.0; // 10%

            if (this.m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(this.m_Slayer);

                if (entry != null)
                {
                    if (entry.Slays(targ))
                        val -= 10.0; // 20%
                    else if (entry.Group.OppositionSuperSlays(targ))
                        val += 10.0; // -20%
                }
            }

            if (this.m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(this.m_Slayer2);

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

        public BaseInstrument(int itemID, int wellSound, int badlySound)
            : base(itemID)
        {
            this.m_WellSound = wellSound;
            this.m_BadlySound = badlySound;
            this.UsesRemaining = Utility.RandomMinMax(this.InitMinUses, this.InitMaxUses);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            int oldUses = this.m_UsesRemaining;
            this.CheckReplenishUses(false);

            base.GetProperties(list);

            if (this.m_Crafter != null)
                list.Add(1050043, this.m_Crafter.Name); // crafted by ~1_NAME~

            if (this.m_Quality == InstrumentQuality.Exceptional)
                list.Add(1060636); // exceptional

            list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~

            if (this.m_ReplenishesCharges)
                list.Add(1070928); // Replenish Charges

            if (this.m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(this.m_Slayer);
                if (entry != null)
                    list.Add(entry.Title);
            }

            if (this.m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(this.m_Slayer2);
                if (entry != null)
                    list.Add(entry.Title);
            }

            if (this.m_UsesRemaining != oldUses)
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(InvalidateProperties));
        }

        public override void OnSingleClick(Mobile from)
        {
            ArrayList attrs = new ArrayList();

            if (this.DisplayLootType)
            {
                if (this.LootType == LootType.Blessed)
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                else if (this.LootType == LootType.Cursed)
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            }

            if (this.m_Quality == InstrumentQuality.Exceptional)
                attrs.Add(new EquipInfoAttribute(1018305 - (int)this.m_Quality));

            if (this.m_ReplenishesCharges)
                attrs.Add(new EquipInfoAttribute(1070928)); // Replenish Charges

            // TODO: Must this support item identification?
            if (this.m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(this.m_Slayer);
                if (entry != null)
                    attrs.Add(new EquipInfoAttribute(entry.Title));
            }

            if (this.m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(this.m_Slayer2);
                if (entry != null)
                    attrs.Add(new EquipInfoAttribute(entry.Title));
            }

            int number;

            if (this.Name == null)
            {
                number = this.LabelNumber;
            }
            else
            {
                this.LabelTo(from, this.Name);
                number = 1041000;
            }

            if (attrs.Count == 0 && this.Crafter == null && this.Name != null)
                return;

            EquipmentInfo eqInfo = new EquipmentInfo(number, this.m_Crafter, false, (EquipInfoAttribute[])attrs.ToArray(typeof(EquipInfoAttribute)));

            from.Send(new DisplayEquipmentInfo(this, eqInfo));
        }

        public BaseInstrument(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version

            writer.Write(this.m_ReplenishesCharges);
            if (this.m_ReplenishesCharges)
                writer.Write(this.m_LastReplenished);

            writer.Write(this.m_Crafter);

            writer.WriteEncodedInt((int)this.m_Quality);
            writer.WriteEncodedInt((int)this.m_Slayer);
            writer.WriteEncodedInt((int)this.m_Slayer2);

            writer.WriteEncodedInt((int)this.UsesRemaining);

            writer.WriteEncodedInt((int)this.m_WellSound);
            writer.WriteEncodedInt((int)this.m_BadlySound);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 3:
                    {
                        this.m_ReplenishesCharges = reader.ReadBool();

                        if (this.m_ReplenishesCharges)
                            this.m_LastReplenished = reader.ReadDateTime();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_Crafter = reader.ReadMobile();

                        this.m_Quality = (InstrumentQuality)reader.ReadEncodedInt();
                        this.m_Slayer = (SlayerName)reader.ReadEncodedInt();
                        this.m_Slayer2 = (SlayerName)reader.ReadEncodedInt();

                        this.UsesRemaining = reader.ReadEncodedInt();

                        this.m_WellSound = reader.ReadEncodedInt();
                        this.m_BadlySound = reader.ReadEncodedInt();
					
                        break;
                    }
                case 1:
                    {
                        this.m_Crafter = reader.ReadMobile();

                        this.m_Quality = (InstrumentQuality)reader.ReadEncodedInt();
                        this.m_Slayer = (SlayerName)reader.ReadEncodedInt();

                        this.UsesRemaining = reader.ReadEncodedInt();

                        this.m_WellSound = reader.ReadEncodedInt();
                        this.m_BadlySound = reader.ReadEncodedInt();

                        break;
                    }
                case 0:
                    {
                        this.m_WellSound = reader.ReadInt();
                        this.m_BadlySound = reader.ReadInt();
                        this.UsesRemaining = Utility.RandomMinMax(this.InitMinUses, this.InitMaxUses);

                        break;
                    }
            }

            this.CheckReplenishUses();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (from.BeginAction(typeof(BaseInstrument)))
            {
                SetInstrument(from, this);

                // Delay of 7 second before beign able to play another instrument again
                new InternalTimer(from).Start();

                if (CheckMusicianship(from))
                    this.PlayInstrumentWell(from);
                else
                    this.PlayInstrumentBadly(from);
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

        public void PlayInstrumentWell(Mobile from)
        {
            from.PlaySound(this.m_WellSound);
        }

        public void PlayInstrumentBadly(Mobile from)
        {
            from.PlaySound(this.m_BadlySound);
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;

            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(6.0))
            {
                this.m_From = from;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                this.m_From.EndAction(typeof(BaseInstrument));
            }
        }
        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (InstrumentQuality)quality;

            if (makersMark)
                this.Crafter = from;

            return quality;
        }
        #endregion
    }
}