using Server;
using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Factions;

namespace Server.Engines.VvV
{
    public enum PotionType
    {
        None = 0x0,
        AntiParalysis = 0x1,
        Supernova = 0x2,
        StatLossRemoval = 0x4,
        GreaterStamina = 0x8,
    }

    public class VvVPotionKeg : Item
    {
        private PotionType _PotionType;
        private int _Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public PotionType PotionType { get { return _PotionType; } set { _PotionType = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return _Charges; } set { _Charges = value; if (_Charges <= 0) Delete(); else InvalidateProperties(); } }

        public override double DefaultWeight
        {
            get
            {
                return 10 + _Charges * 1.8;
            }
        }

        [Constructable]
        public VvVPotionKeg(PotionType type)
            : base(6870)
        {
            PotionType = type;
            Charges = 10;

            switch (type)
            {
                default:
                case PotionType.AntiParalysis: Hue = 2543; break;
                case PotionType.Supernova: Hue = 13; break;
                case PotionType.StatLossRemoval: Hue = 2500; break;
                case PotionType.GreaterStamina: Hue = 437; break;
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            string str;

            switch (_PotionType)
            {
                default:
                case PotionType.AntiParalysis: str = "#1155543"; break;
                case PotionType.Supernova: str = "#1094718"; break;
                case PotionType.StatLossRemoval: str = "#1155541"; break;
                case PotionType.GreaterStamina: str = "#1094764"; break;
            }

            list.Add(1155535, str); // A Batch of ~1_ITEMS~
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                if (!ViceVsVirtueSystem.IsVvV(m))
                {
                    m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                }
                else
                {
                    Item potion = null;

                    switch (_PotionType)
                    {
                        case PotionType.AntiParalysis: potion = new AntiParalysisPotion(); break;
                        case PotionType.Supernova: potion = new SupernovaPotion(); break;
                        case PotionType.StatLossRemoval: potion = new StatLossRemovalPotion(); break;
                        case PotionType.GreaterStamina: potion = new GreaterStaminaPotion(); break;
                    }

                    if (potion != null)
                    {
                        m.SendLocalizedMessage(502242); // You pour some of the keg's contents into an empty bottle...

                        if (m.Backpack == null || !m.Backpack.TryDropItem(m, potion, false))
                        {
                            m.SendLocalizedMessage(1155570); // Your backpack could not hold the item.  Free up some space and try again.
                            potion.Delete();
                        }
                        else
                        {
                            m.SendLocalizedMessage(502243); // ...and place it into your backpack.
                            m.PlaySound(0x240);

                            Charges--;
                        }
                    }
                }
            }
            else
            {
                m.SendLocalizedMessage(1042004); // That must be in your pack for you to use it
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            
            list.Add(1155569, Charges.ToString()); // Potions: ~1_val~
            list.Add(1154937); // VvV Item
        }

        public VvVPotionKeg(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)PotionType);
            writer.Write(Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            PotionType = (PotionType)reader.ReadInt();
            Charges = reader.ReadInt();
        }
    }

    public abstract class VvVPotion : Item, IFactionItem
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

        public virtual TimeSpan CooldownDuration { get { return TimeSpan.MinValue; } }
        public virtual PotionType CooldownType { get { return PotionType.None; } }

        public static Dictionary<Mobile, Dictionary<PotionType, DateTime>> _Cooldown = new Dictionary<Mobile, Dictionary<PotionType, DateTime>>();

        public static void RemoveFromCooldown(Mobile m, PotionType type)
        {
            if (_Cooldown.ContainsKey(m) && _Cooldown[m].ContainsKey(type))
            {
                _Cooldown[m].Remove(type);

                if (_Cooldown[m].Count == 0)
                    _Cooldown.Remove(m);
            }
        }

        public static void CheckCooldown()
        {
            List<Mobile> toRemove = new List<Mobile>();

            foreach (KeyValuePair<Mobile, Dictionary<PotionType, DateTime>> kvp in _Cooldown)
            {
                List<PotionType> removeTypes = new List<PotionType>();

                foreach (KeyValuePair<PotionType, DateTime> values in kvp.Value)
                {
                    if (values.Value < DateTime.UtcNow)
                        removeTypes.Add(values.Key);
                }

                foreach (PotionType t in removeTypes)
                    kvp.Value.Remove(t);

                if (kvp.Value.Count == 0)
                    toRemove.Add(kvp.Key);

                removeTypes.Clear();
                removeTypes.TrimExcess();
            }

            foreach (Mobile mob in toRemove)
                _Cooldown.Remove(mob);

            toRemove.Clear();
            toRemove.TrimExcess();
        }

        public override int LabelNumber
        {
            get
            {
                switch (CooldownType)
                {
                    case PotionType.AntiParalysis: return 1155543;
                    case PotionType.Supernova: return 1094718;
                    case PotionType.StatLossRemoval: return 1155541;
                    case PotionType.GreaterStamina: return 1094764;
                }

                return base.LabelNumber;
            }
        }

        public VvVPotion()
            : base(3849)
        {
            Stackable = true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!FactionEquipment.AddFactionProperties(this, list))
            {
                list.Add(1154937); // VvV Item
            }
        }

        public bool IsInCooldown(Mobile m, ref DateTime dt)
        {
            if (_Cooldown.ContainsKey(m))
            {
                if (_Cooldown[m].ContainsKey(PotionType.GreaterStamina))
                {
                    dt = _Cooldown[m][PotionType.GreaterStamina];

                    if (dt < DateTime.UtcNow)
                    {
                        RemoveFromCooldown(m, PotionType.GreaterStamina);
                        return false;
                    }

                    return true;
                }

                if (_Cooldown[m].ContainsKey(CooldownType))
                {
                    dt = _Cooldown[m][CooldownType];

                    if (dt < DateTime.UtcNow)
                    {
                        RemoveFromCooldown(m, CooldownType);
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public void AddToCooldown(Mobile m)
        {
            if (!_Cooldown.ContainsKey(m))
                _Cooldown[m] = new Dictionary<PotionType, DateTime>();

            _Cooldown[m][CooldownType] = DateTime.UtcNow + CooldownDuration;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!Movable)
                return;

            if (IsChildOf(m.Backpack))
            {
                DateTime dt = DateTime.UtcNow;

                if (ViceVsVirtueSystem.Enabled && !ViceVsVirtueSystem.IsVvV(m))
                {
                    m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                }
                else if (Server.Factions.Settings.Enabled && !FactionEquipment.CanUse(this, m))
                {
                }
                else if (!BasePotion.HasFreeHand(m))
                {
                    m.SendLocalizedMessage(502172); // You must have a free hand to drink a potion.
                }
                else if (IsInCooldown(m, ref dt))
                {
                    TimeSpan left = dt - DateTime.UtcNow;

                    if (left.TotalMinutes > 2)
                    {
                        m.SendLocalizedMessage(1114110, ((int)left.TotalMinutes).ToString()); // You must wait ~1_minutes~ minutes before using another one of these.
                    }
                    else
                    {
                        m.SendLocalizedMessage(1114109, ((int)left.TotalSeconds).ToString()); // You must wait ~1_seconds~ seconds before using another one of these.
                    }
                }
                else if (CheckUse(m))
                {
                    UseEffects(m);
                    Use(m);
                }
            }
            else
            {
                m.SendLocalizedMessage(1042004); // That must be in your pack for you to use it
            }
        }

        public virtual bool CheckUse(Mobile m)
        {
            return true;
        }

        public abstract void Use(Mobile m);

        public virtual void UseEffects(Mobile m)
        {
            m.RevealingAction();
            m.PlaySound(0x2D6);

            if (m.Body.IsHuman && !m.Mounted)
                m.Animate(34, 5, 1, true, false, 0);

            if (CooldownDuration != TimeSpan.MinValue)
            {
                AddToCooldown(m);
            }

            Timer.DelayCall<Mobile>(TimeSpan.FromMilliseconds(500), DrinkEffects, m);
        }

        public virtual void DrinkEffects(Mobile m)
        {
        }

        public VvVPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class AntiParalysisPotion : VvVPotion
    {
        public override PotionType CooldownType { get { return PotionType.AntiParalysis; } }

        [Constructable]
        public AntiParalysisPotion()
        {
            Hue = 2543;
        }

        public override bool CheckUse(Mobile m)
        {
            if (!m.Paralyzed)
            {
                m.SendLocalizedMessage(1155544); // You are not currently paralyzed
                return false;
            }

            return true;
        }

        public override void Use(Mobile m)
        {
            m.Paralyzed = false;
            m.Stam /= 2;

            Consume();
        }

        public override void DrinkEffects(Mobile m)
        {
            m.FixedEffect(0x375A, 10, 15);
            m.PlaySound(0x1E7);
        }

        public AntiParalysisPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SupernovaPotion : VvVPotion
    {
        public override TimeSpan CooldownDuration { get { return TimeSpan.FromMinutes(2); } }
        public override PotionType CooldownType { get { return PotionType.Supernova; } }

        [Constructable]
        public SupernovaPotion()
        {
            Hue = 13;
        }

        public override void Use(Mobile m)
        {
            Effects.SendMovingEffect(m, new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z + 25), m.Map), this.ItemID, 3, 0, false, false, this.Hue, 0);
            
            int count = 5;

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    m.PlaySound(0x1DD);

                    for (int i = 0; i < count; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromMilliseconds(i * 170), index =>
                            {
                                Server.Misc.Geometry.Circle2D(m.Location, m.Map, index, (pnt, map) =>
                                {
                                    Effects.SendLocationEffect(pnt, map, 0x3709, 30, 10, 0, 5);
                                });
                            }, i);
                    }
                });

            Timer.DelayCall(TimeSpan.FromMilliseconds(170 * count), () =>
                {
                    IPooledEnumerable eable = m.Map.GetMobilesInRange(m.Location, count);

                    foreach (Mobile mob in eable)
                    {
                        if (mob != m && Server.Spells.SpellHelper.ValidIndirectTarget(m, mob) && m.CanBeHarmful(mob, false))
                        {
                            m.DoHarmful(mob);
                            AOS.Damage(mob, m, Utility.RandomMinMax(40, 60), 0, 100, 0, 0, 0);
                        }
                    }

                    eable.Free();
                });

            if (m.AccessLevel == AccessLevel.Player)
                Consume();
        }

        public override void UseEffects(Mobile m)
        {
            AddToCooldown(m);
        }

        public SupernovaPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class StatLossRemovalPotion : VvVPotion
    {
        public override TimeSpan CooldownDuration { get { return TimeSpan.FromMinutes(20); } }
        public override PotionType CooldownType { get { return PotionType.StatLossRemoval; } }

        [Constructable]
        public StatLossRemovalPotion()
        {
            Hue = 2500;
        }

        public override bool CheckUse(Mobile m)
        {
            if (!Server.Factions.Faction.InSkillLoss(m))
            {
                m.SendLocalizedMessage(1155542); // You are not currently under the effects of stat loss.
                return false;
            }

            return true;
        }

        public override void Use(Mobile m)
        {
            m.SendLocalizedMessage(1155540); // You feel the effects of your stat loss fade.
            Server.Factions.Faction.ClearSkillLoss(m);

            Consume();
        }

        public override void DrinkEffects(Mobile m)
        {
            m.PlaySound(0xF6);
            m.PlaySound(0x1F7);
            m.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);
        }

        public StatLossRemovalPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GreaterStaminaPotion : VvVPotion
    {
        public override TimeSpan CooldownDuration { get { return TimeSpan.FromSeconds(10); } }
        public override PotionType CooldownType { get { return PotionType.GreaterStamina; } }

        [Constructable]
        public GreaterStaminaPotion()
        {
            Hue = 437;
        }

        public override bool CheckUse(Mobile m)
        {
            //TODO: Message?  Stam check?
            return true;
        }

        public override void Use(Mobile m)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), 10, () =>
            {
                int gain = Utility.RandomMinMax(10, 13);

                if (m.Stam + gain > m.StamMax)
                    gain = m.StamMax - m.Stam;

                m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                m.Stam += gain;
            });

            Consume();
        }

        public override void DrinkEffects(Mobile m)
        {
            m.FixedEffect(0x375A, 10, 15);
            m.PlaySound(0x1E7);
        }

        public GreaterStaminaPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}