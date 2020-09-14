using Server.Engines.Despise;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Services.Virtues;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class Bandage : Item, IDyable, ICommodity
    {
        public static void Initialize()
        {
            EventSink.BandageTargetRequest += BandageTargetRequest;
        }

        public static int Range = 2;

        public override double DefaultWeight => 0.1;

        [Constructable]
        public Bandage()
            : this(1)
        { }

        [Constructable]
        public Bandage(int amount)
            : base(0xE21)
        {
            Stackable = true;
            Amount = amount;
        }

        public Bandage(Serial serial)
            : base(serial)
        { }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
            {
                return false;
            }

            Hue = sender.DyedHue;

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), Range))
            {
                from.RevealingAction();

                from.SendLocalizedMessage(500948); // Who will you use the bandages on?

                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(500295); // You are too far away to do that.
            }
        }

        public static void BandageTargetRequest(BandageTargetRequestEventArgs e)
        {
            BandageTargetRequest(e.Bandage as Bandage, e.Mobile, e.Target);
        }

        public static void BandageTargetRequest(Bandage bandage, Mobile from, Mobile target)
        {
            if (bandage == null || bandage.Deleted)
                return;

            if (from.InRange(bandage.GetWorldLocation(), Range))
            {
                Target t = from.Target;

                if (t != null)
                {
                    Target.Cancel(from);
                    from.Target = null;
                }

                from.RevealingAction();
                from.SendLocalizedMessage(500948); // Who will you use the bandages on?

                new InternalTarget(bandage).Invoke(from, target);
            }
            else
            {
                from.SendLocalizedMessage(500295); // You are too far away to do that.
            }
        }

        private class InternalTarget : Target
        {
            private readonly Bandage m_Bandage;

            public InternalTarget(Bandage bandage)
                : base(Bandage.Range, false, TargetFlags.Beneficial)
            {
                m_Bandage = bandage;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Bandage.Deleted)
                {
                    return;
                }

                if (targeted is Mobile)
                {
                    if (from.InRange(m_Bandage.GetWorldLocation(), Bandage.Range))
                    {
                        if (BandageContext.BeginHeal(from, (Mobile)targeted, m_Bandage is EnhancedBandage) != null)
                        {
                            NegativeAttributes.OnCombatAction(from);
                            m_Bandage.Consume();
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(500295); // You are too far away to do that.
                    }
                }
                else if (targeted is PlagueBeastInnard)
                {
                    if (((PlagueBeastInnard)targeted).OnBandage(from))
                    {
                        NegativeAttributes.OnCombatAction(from);
                        m_Bandage.Consume();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(500970); // Bandages can not be used on that.
                }
            }

            protected override void OnNonlocalTarget(Mobile from, object targeted)
            {
                if (targeted is PlagueBeastInnard)
                {
                    if (((PlagueBeastInnard)targeted).OnBandage(from))
                    {
                        m_Bandage.Consume();
                    }
                }
                else
                {
                    base.OnNonlocalTarget(from, targeted);
                }
            }
        }
    }

    public class BandageContext
    {
        private readonly Mobile m_Healer;
        private readonly Mobile m_Patient;
        private int m_Slips;
        private int m_HealedPoisonOrBleed;
        private Timer m_Timer;
        private int m_HealingBonus;

        public Mobile Healer => m_Healer;
        public Mobile Patient => m_Patient;
        public int Slips { get { return m_Slips; } set { m_Slips = value; } }
        public int HealedPoisonOrBleed { get { return m_HealedPoisonOrBleed; } set { m_HealedPoisonOrBleed = value; } }
        public Timer Timer => m_Timer;
        public int HealingBonus => m_HealingBonus;

        public void Slip()
        {
            m_Healer.SendLocalizedMessage(500961); // Your fingers slip!
            ++m_Slips;
        }

        public BandageContext(Mobile healer, Mobile patient, TimeSpan delay)
            : this(healer, patient, delay, false)
        { }

        public BandageContext(Mobile healer, Mobile patient, TimeSpan delay, bool enhanced)
        {
            m_Healer = healer;
            m_Patient = patient;

            if (enhanced)
                m_HealingBonus += EnhancedBandage.HealingBonus;

            m_Timer = new InternalTimer(this, delay);
            m_Timer.Start();
        }

        public void StopHeal()
        {
            m_Table.Remove(m_Healer);

            if (m_Timer != null)
            {
                m_Timer.Stop();
            }

            m_Timer = null;
        }

        private static readonly Dictionary<Mobile, BandageContext> m_Table = new Dictionary<Mobile, BandageContext>();

        public static BandageContext GetContext(Mobile healer)
        {
            BandageContext bc = null;
            m_Table.TryGetValue(healer, out bc);
            return bc;
        }

        public static SkillName GetPrimarySkill(Mobile healer, Mobile m)
        {
            if (m is DespiseCreature)
            {
                return healer.Skills[SkillName.Healing].Value > healer.Skills[SkillName.Veterinary].Value ? SkillName.Healing : SkillName.Veterinary;
            }

            if (!m.Player && (m.Body.IsMonster || m.Body.IsAnimal))
            {
                return SkillName.Veterinary;
            }
            else
            {
                return SkillName.Healing;
            }
        }

        public static SkillName GetSecondarySkill(Mobile healer, Mobile m)
        {
            if (m is DespiseCreature)
            {
                return healer.Skills[SkillName.Healing].Value > healer.Skills[SkillName.Veterinary].Value ? SkillName.Anatomy : SkillName.AnimalLore;
            }

            if (!m.Player && (m.Body.IsMonster || m.Body.IsAnimal))
            {
                return SkillName.AnimalLore;
            }
            else
            {
                return SkillName.Anatomy;
            }
        }

        public void CheckPoisonOrBleed()
        {
            bool bleeding = BleedAttack.IsBleeding(m_Patient);
            bool poisoned = m_Patient.Poisoned;

            if (bleeding || poisoned)
            {
                double healing = m_Healer.Skills[SkillName.Healing].Value;
                double anatomy = m_Healer.Skills[SkillName.Anatomy].Value;
                double chance = ((healing + anatomy) - 120) * 25;

                if (poisoned)
                    chance /= m_Patient.Poison.RealLevel * 20;
                else
                    chance /= 3 * 20;

                if (chance >= Utility.Random(100))
                {
                    m_HealedPoisonOrBleed = poisoned ? m_Patient.Poison.RealLevel : 3;

                    if (poisoned && m_Patient.CurePoison(m_Healer))
                    {
                        m_Patient.SendLocalizedMessage(1010059); // You have been cured of all poisons.
                    }
                    else
                    {
                        if (BleedAttack.IsBleeding(m_Patient))
                        {
                            BleedAttack.EndBleed(m_Patient, false);
                        }

                        m_Patient.SendLocalizedMessage(1060088); // You bind the wound and stop the bleeding
                        m_Patient.SendLocalizedMessage(1060167); // The bleeding wounds have healed, you are no longer bleeding!
                    }
                }
            }
        }

        public void EndHeal()
        {
            StopHeal();

            int healerNumber = -1, patientNumber = -1;
            bool playSound = true;
            bool checkSkills = false;

            SkillName primarySkill = GetPrimarySkill(m_Healer, m_Patient);
            SkillName secondarySkill = GetSecondarySkill(m_Healer, m_Patient);

            BaseCreature petPatient = m_Patient as BaseCreature;

            if (!m_Healer.Alive)
            {
                healerNumber = 500962; // You were unable to finish your work before you died.
                patientNumber = -1;
                playSound = false;
            }
            else if (!m_Healer.InRange(m_Patient, Bandage.Range))
            {
                healerNumber = 500963; // You did not stay close enough to heal your target.
                patientNumber = -1;
                playSound = false;
            }
            else if (!m_Patient.Alive || (petPatient != null && petPatient.IsDeadPet))
            {
                double healing = m_Healer.Skills[primarySkill].Value;
                double anatomy = m_Healer.Skills[secondarySkill].Value;
                double chance = ((healing - 68.0) / 50.0) - (m_Slips * 0.02);

                if (((checkSkills = (healing >= 80.0 && anatomy >= 80.0)) && chance > Utility.RandomDouble()) ||
                    (Engines.VvV.ViceVsVirtueSystem.Enabled && petPatient is Engines.VvV.VvVMount && petPatient.ControlMaster == m_Healer))
                {
                    if (m_Patient.Map == null || !m_Patient.Map.CanFit(m_Patient.Location, 16, false, false))
                    {
                        healerNumber = 501042; // Target can not be resurrected at that location.
                        patientNumber = 502391; // Thou can not be resurrected there!
                    }
                    else if (m_Patient.Region != null && m_Patient.Region.IsPartOf("Khaldun"))
                    {
                        healerNumber = 1010395; // The veil of death in this area is too strong and resists thy efforts to restore life.
                        patientNumber = -1;
                    }
                    else
                    {
                        healerNumber = 500965; // You are able to resurrect your patient.
                        patientNumber = -1;

                        m_Patient.PlaySound(0x214);
                        m_Patient.FixedEffect(0x376A, 10, 16);

                        if (petPatient != null && petPatient.IsDeadPet)
                        {
                            Mobile master = petPatient.ControlMaster;

                            if (master != null && m_Healer == master)
                            {
                                petPatient.ResurrectPet();

                                for (int i = 0; i < petPatient.Skills.Length; ++i)
                                {
                                    petPatient.Skills[i].Base -= 0.1;
                                }
                            }
                            else if (master != null && master.InRange(petPatient, 3))
                            {
                                healerNumber = 1049658; // The owner has been asked to sanctify the resurrection.

                                master.CloseGump(typeof(PetResurrectGump));
                                master.SendGump(new PetResurrectGump(m_Healer, petPatient));
                            }
                            else
                            {
                                bool found = false;

                                List<Mobile> friends = petPatient.Friends;

                                for (int i = 0; friends != null && i < friends.Count; ++i)
                                {
                                    Mobile friend = friends[i];

                                    if (friend.InRange(petPatient, 3))
                                    {
                                        healerNumber = 1049658; // The owner has been asked to sanctify the resurrection.

                                        friend.CloseGump(typeof(PetResurrectGump));
                                        friend.SendGump(new PetResurrectGump(m_Healer, petPatient));

                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    healerNumber = 1049659; // Neither the owner or friends of the pet are nearby to sanctify the resurrection.
                                }
                            }
                        }
                        else
                        {
                            m_Patient.CloseGump(typeof(ResurrectGump));
                            m_Patient.SendGump(new ResurrectGump(m_Patient, m_Healer));
                        }
                    }
                }
                else
                {
                    if (petPatient != null && petPatient.IsDeadPet)
                    {
                        healerNumber = 503256; // You fail to resurrect the creature.
                    }
                    else
                    {
                        healerNumber = 500966; // You are unable to resurrect your patient.
                    }

                    patientNumber = -1;
                }
            }
            else if (m_Patient.Poisoned)
            {
                m_Healer.SendLocalizedMessage(500969); // You finish applying the bandages.

                double healing = m_Healer.Skills[primarySkill].Value;
                double anatomy = m_Healer.Skills[secondarySkill].Value;
                double chance = ((healing - 30.0) / 50.0) - (m_Patient.Poison.RealLevel * 0.1) - (m_Slips * 0.02);

                if ((checkSkills = (healing >= 60.0 && anatomy >= 60.0)) && chance > Utility.RandomDouble())
                {
                    if (m_Patient.CurePoison(m_Healer))
                    {
                        healerNumber = (m_Healer == m_Patient) ? -1 : 1010058; // You have cured the target of all poisons.
                        patientNumber = 1010059; // You have been cured of all poisons.
                    }
                    else
                    {
                        healerNumber = -1;
                        patientNumber = -1;
                    }
                }
                else
                {
                    healerNumber = 1010060; // You have failed to cure your target!
                    patientNumber = -1;
                }
            }
            else if (BleedAttack.IsBleeding(m_Patient))
            {
                healerNumber = 1060088; // You bind the wound and stop the bleeding
                patientNumber = 1060167; // The bleeding wounds have healed, you are no longer bleeding!

                BleedAttack.EndBleed(m_Patient, false);
            }
            else if (MortalStrike.IsWounded(m_Patient))
            {
                healerNumber = (m_Healer == m_Patient ? 1005000 : 1010398);
                patientNumber = -1;
                playSound = false;
            }
            else if (m_Patient.Hits == m_Patient.HitsMax)
            {
                healerNumber = 500967; // You heal what little damage your patient had.
                patientNumber = -1;
            }
            else
            {
                checkSkills = true;
                patientNumber = -1;

                double healing = m_Healer.Skills[primarySkill].Value;
                double anatomy = m_Healer.Skills[secondarySkill].Value;

                FirstAidBelt belt = m_Healer.FindItemOnLayer(Layer.Waist) as FirstAidBelt;

                if (belt != null)
                    m_HealingBonus += belt.HealingBonus;

                Item item = m_Healer.FindItemOnLayer(Layer.TwoHanded);

                if (item is Asclepius || item is GargishAsclepius)
                    m_HealingBonus += 15;

                if (m_HealingBonus > 0)
                    healing += m_HealingBonus;

                double chance = ((healing + 10.0) / 100.0) - (m_Slips * 0.02);

                if (chance > Utility.RandomDouble())
                {
                    healerNumber = 500969; // You finish applying the bandages.

                    double min = (anatomy / 8.0) + (healing / 5.0) + 4.0;
                    double max = (anatomy / 6.0) + (healing / 2.5) + 4.0;

                    double toHeal = min + (Utility.RandomDouble() * (max - min));

                    if (m_Patient.Body.IsMonster || m_Patient.Body.IsAnimal)
                    {
                        toHeal += m_Patient.HitsMax / 100;
                    }

                    toHeal -= toHeal * m_Slips * 0.35; // TODO: Verify algorithm

                    if (Engines.CityLoyalty.CityLoyaltySystem.HasTradeDeal(m_Healer, Engines.CityLoyalty.TradeDeal.GuildOfHealers))
                        toHeal += (int)Math.Ceiling(toHeal * 0.05);

                    if (m_HealedPoisonOrBleed > 0)
                    {
                        toHeal /= m_HealedPoisonOrBleed;
                    }

                    if (SearingWounds.IsUnderEffects(m_Patient))
                    {
                        toHeal /= 2;
                        m_Patient.SendLocalizedMessage(1151178); // The cauterized wound resists some of your healing.
                    }

                    if (toHeal < 1)
                    {
                        toHeal = 1;
                        healerNumber = 500968; // You apply the bandages, but they barely help.
                    }
                    else if (m_Patient != m_Healer && m_Patient is PlayerMobile && m_Healer is PlayerMobile)
                    {
                        SpiritualityVirtue.OnHeal(m_Healer, Math.Min((int)toHeal, m_Patient.HitsMax - m_Patient.Hits));
                    }

                    m_Patient.Heal((int)toHeal, m_Healer, false);
                }
                else
                {
                    healerNumber = 500968; // You apply the bandages, but they barely help.
                    playSound = false;
                }
            }

            if (healerNumber != -1)
            {
                m_Healer.SendLocalizedMessage(healerNumber);
            }

            if (patientNumber != -1)
            {
                m_Patient.SendLocalizedMessage(patientNumber);
            }

            if (playSound)
            {
                m_Patient.PlaySound(0x57);
            }

            if (checkSkills)
            {
                m_Healer.CheckSkill(secondarySkill, 0.0, 120.0);
                m_Healer.CheckSkill(primarySkill, 0.0, 120.0);
            }

            if (m_Patient is PlayerMobile)
                BuffInfo.RemoveBuff(m_Healer, BuffIcon.Healing);
            else
                BuffInfo.RemoveBuff(m_Healer, BuffIcon.Veterinary);
        }

        private class InternalTimer : Timer
        {
            private readonly BandageContext m_Context;
            private readonly long m_Begin;
            private readonly long m_Expires;
            private bool m_CheckedHealAndBleed;

            public bool CanCheckAtHalf => m_Context != null && m_Context.Healer == m_Context.Patient &&
                           m_Context.Healer.Skills[SkillName.Healing].Value >= 80 &&
                           m_Context.Healer.Skills[SkillName.Anatomy].Value >= 80;

            public InternalTimer(BandageContext context, TimeSpan delay)
                : base(TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(250))
            {
                m_Context = context;
                Priority = TimerPriority.FiftyMS;

                m_Begin = Core.TickCount;
                m_Expires = Core.TickCount + (int)delay.TotalMilliseconds;
            }

            protected override void OnTick()
            {
                if (Core.TickCount >= m_Expires)
                {
                    m_Context.EndHeal();
                    Stop();
                }
                else if (!m_CheckedHealAndBleed && CanCheckAtHalf && m_Begin + ((m_Expires - m_Begin) / 2) < Core.TickCount)
                {
                    m_Context.CheckPoisonOrBleed();
                    m_CheckedHealAndBleed = true;
                }
            }
        }

        public static BandageContext BeginHeal(Mobile healer, Mobile patient)
        {
            return BeginHeal(healer, patient, false);
        }

        public static BandageContext BeginHeal(Mobile healer, Mobile patient, bool enhanced)
        {
            bool isDeadPet = (patient is BaseCreature && ((BaseCreature)patient).IsDeadPet);

            if (patient is IRepairableMobile && ((IRepairableMobile)patient).RepairResource != typeof(Bandage))
            {
                healer.SendLocalizedMessage(500970); // Bandages cannot be used on that.
            }
            else if (patient is BaseCreature && ((BaseCreature)patient).IsAnimatedDead)
            {
                healer.SendLocalizedMessage(500951); // You cannot heal that.
            }
            else if (!patient.Poisoned && patient.Hits == patient.HitsMax && !BleedAttack.IsBleeding(patient) && !isDeadPet)
            {
                healer.SendLocalizedMessage(500955); // That being is not damaged!
            }
            else if (!patient.Alive && (patient.Map == null || !patient.Map.CanFit(patient.Location, 16, false, false)))
            {
                healer.SendLocalizedMessage(501042); // Target cannot be resurrected at that location.
            }
            else if (healer.CanBeBeneficial(patient, true, true))
            {
                healer.DoBeneficial(patient);

                BandageContext context = GetContext(healer);

                if (context != null)
                {
                    context.StopHeal();
                }

                TimeSpan delay = GetDelay(healer, patient);

                if (patient is PlayerMobile)
                    BuffInfo.AddBuff(healer, new BuffInfo(BuffIcon.Healing, 1002082, 1151400, delay, healer, string.Format("{0}", patient.Name)));
                else
                    BuffInfo.AddBuff(healer, new BuffInfo(BuffIcon.Veterinary, 1002167, 1151400, delay, healer, string.Format("{0}", patient.Name)));

                context = new BandageContext(healer, patient, delay, enhanced);

                m_Table[healer] = context;

                if (healer != patient)
                {
                    patient.SendLocalizedMessage(1008078, false, healer.Name); //  : Attempting to heal you.
                }

                healer.SendLocalizedMessage(500956); // You begin applying the bandages.

                if (healer.NetState != null && healer.NetState.IsEnhancedClient)
                {
                    healer.NetState.Send(new BandageTimerPacket((int)delay.TotalSeconds));
                }

                return context;
            }

            return null;
        }

        public static TimeSpan GetDelay(Mobile healer, Mobile patient)
        {
            return GetDelay(healer, patient, !patient.Alive || patient.IsDeadBondedPet);
        }

        public static TimeSpan GetDelay(Mobile healer, Mobile patient, bool dead)
        {
            return GetDelay(healer, patient, dead, GetPrimarySkill(healer, patient));
        }

        public static TimeSpan GetDelay(Mobile healer, Mobile patient, bool dead, SkillName skill)
        {
            int dex = healer.Dex;

            double seconds;

            if (healer == patient)
            {
                seconds = Math.Min(8, Math.Ceiling(11.0 - dex / 20));
                seconds = Math.Max(seconds, 4);
            }
            else if (skill == SkillName.Veterinary)
            {
                seconds = 2.0;
            }
            else
            {
                seconds = Math.Ceiling((double)4 - dex / 60);
                seconds = Math.Max(seconds, 2);
            }

            return TimeSpan.FromSeconds(seconds);
        }
    }

    public sealed class BandageTimerPacket : Packet
    {
        public BandageTimerPacket(int duration)
            : base(0xBF)
        {
            EnsureCapacity(15);

            m_Stream.Write((short)0x31);
            m_Stream.Write((short)0x01);

            m_Stream.Write(0xE21);
            m_Stream.Write(duration);
        }
    }
}
