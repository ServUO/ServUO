using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class EodonPotionContext
    {
        public PotionEffect Type => Potion.PotionEffect;
        public DateTime StartTime { get; set; }
        public DateTime Expires { get; set; }
        public EodonianPotion Potion { get; set; }

        public Timer Timer { get; set; }

        public EodonPotionContext(EodonianPotion potion)
        {
            Potion = potion;
            Expires = DateTime.UtcNow + potion.Cooldown;
            StartTime = DateTime.UtcNow;
        }

        public void OnTick(Mobile m)
        {
            if (DateTime.UtcNow >= Expires)
            {
                Potion.EndEffects(m);
            }
            else
                Potion.OnTick(m);
        }

        public void StopTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }
    }

    public class EodonianPotion : BasePotion
    {
        public static Dictionary<Mobile, List<EodonPotionContext>> Contexts { get; set; }
        public static Timer Timer { get; set; }

        public virtual TimeSpan Cooldown => TimeSpan.FromMinutes(20);

        public override int LabelNumber
        {
            get
            {
                switch (PotionEffect)
                {
                    default:
                    case PotionEffect.Barrab: return 1156724;
                    case PotionEffect.Jukari: return 1156726;
                    case PotionEffect.Kurak: return 1156728;
                    case PotionEffect.Barako: return 1156729;
                    case PotionEffect.Urali: return 1156734;
                    case PotionEffect.Sakkhra: return 1156732;
                }
            }
        }

        public static void Initialize()
        {
            EventSink.PlayerDeath += EventSink_PlayerDeath;
        }

        public EodonianPotion(int id, PotionEffect effect)
            : base(id, effect)
        {
        }

        public override void Drink(Mobile m)
        {
            if (CanDoEffects(m))
            {
                m.FixedEffect(0x375A, 10, 15);
                m.PlaySound(0x1E7);

                DoEffects(m);
                PlayDrinkEffect(m);

                Consume();
            }
        }

        public virtual void OnTick(Mobile m)
        {
        }

        public virtual bool CanDoEffects(Mobile m)
        {
            if (IsUnderEffects(m, PotionEffect))
            {
                m.SendLocalizedMessage(502173); // You are already under a similar effect.
                return false;
            }

            return true;
        }

        public virtual void DoEffects(Mobile m)
        {
            if (Contexts == null)
                Contexts = new Dictionary<Mobile, List<EodonPotionContext>>();

            if (!Contexts.ContainsKey(m) || Contexts[m] == null)
                Contexts[m] = new List<EodonPotionContext>();

            AddBuff(m);

            Contexts[m].Add(new EodonPotionContext(this));
            BeginTimer();
        }

        public virtual void EndEffects(Mobile m)
        {
            RemoveContext(m);
        }

        public void RemoveContext(Mobile m)
        {
            EodonPotionContext context = GetContext(m, PotionEffect);

            if (context != null)
                RemoveContext(m, context);
        }

        public virtual void AddBuff(Mobile m)
        {
            switch (PotionEffect)
            {
                case PotionEffect.Barrab:
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.BarrabHemolymphConcentrate, LabelNumber, 1156738, "100\t10\t10\t5")); break;
                case PotionEffect.Jukari:
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.JukariBurnPoiltice, LabelNumber, 1156739, "10\t10\t10\t5")); break;
                case PotionEffect.Kurak:
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.KurakAmbushersEssence, LabelNumber, 1156740, "200")); break;
                case PotionEffect.Barako:
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.BarakoDraftOfMight, LabelNumber, 1156741, "10\t10\t5\t10")); break;
                case PotionEffect.Urali:
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.UraliTranceTonic, LabelNumber, 1156742, "10\t10\t10\t5")); break;
                case PotionEffect.Sakkhra:
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.SakkhraProphylaxis, LabelNumber, 1156743, "10\t10\t5\t10")); break;
            }
        }

        public virtual void RemoveBuff(Mobile m)
        {
            switch (PotionEffect)
            {
                case PotionEffect.Barrab:
                    BuffInfo.RemoveBuff(m, BuffIcon.BarrabHemolymphConcentrate); break;
                case PotionEffect.Jukari:
                    BuffInfo.RemoveBuff(m, BuffIcon.JukariBurnPoiltice); break;
                case PotionEffect.Kurak:
                    BuffInfo.RemoveBuff(m, BuffIcon.KurakAmbushersEssence); break;
                case PotionEffect.Barako:
                    BuffInfo.RemoveBuff(m, BuffIcon.BarakoDraftOfMight); break;
                case PotionEffect.Urali:
                    BuffInfo.RemoveBuff(m, BuffIcon.UraliTranceTonic); break;
                case PotionEffect.Sakkhra:
                    BuffInfo.RemoveBuff(m, BuffIcon.SakkhraProphylaxis); break;
            }
        }

        public static bool IsUnderEffects(Mobile m, PotionEffect effect)
        {
            return GetContext(m, effect) != null;
        }

        public static void RemoveEffects(Mobile m, PotionEffect effect)
        {
            RemoveContext(m, GetContext(m, effect));
        }

        public static EodonPotionContext GetContext(Mobile m, PotionEffect effect)
        {
            if (m == null)
                return null;

            if (Contexts == null || !Contexts.ContainsKey(m) || Contexts[m] == null)
                return null;

            return Contexts[m].FirstOrDefault(c => c.Type == effect);
        }

        public static void RemoveContext(Mobile m, PotionEffect effect)
        {
            RemoveContext(m, GetContext(m, effect));
        }

        public static void RemoveContext(Mobile m, EodonPotionContext context)
        {
            if (context == null)
                return;

            if (context.Potion != null)
                context.Potion.RemoveBuff(m);

            if (Contexts.ContainsKey(m))
            {
                if (Contexts[m] != null && Contexts[m].Contains(context))
                {
                    Contexts[m].Remove(context);
                }

                if (Contexts[m] == null || Contexts[m].Count == 0)
                    Contexts.Remove(m);

                if (Contexts.Count == 0)
                    EndTimer();

                m.Delta(MobileDelta.WeaponDamage);
            }
        }

        public static void BeginTimer()
        {
            if (Timer == null || !Timer.Running)
            {
                Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
                Timer.Start();
            }
        }

        public static void EndTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        public static void OnTick()
        {
            if (Contexts == null)
                EndTimer();
            else
            {
                Dictionary<Mobile, List<EodonPotionContext>> dictionary = new Dictionary<Mobile, List<EodonPotionContext>>(Contexts);

                foreach (KeyValuePair<Mobile, List<EodonPotionContext>> kvp in dictionary)
                {
                    List<EodonPotionContext> contexts = new List<EodonPotionContext>(kvp.Value);

                    foreach (EodonPotionContext context in contexts)
                    {
                        context.OnTick(kvp.Key);
                    }

                    ColUtility.Free(contexts);
                }

                dictionary.Clear();
            }
        }

        public static void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            if (Contexts != null)
            {
                if (e.Mobile != null && Contexts.ContainsKey(e.Mobile))
                    Contexts.Remove(e.Mobile);

                if (Contexts.Count == 0)
                    EndTimer();
            }
        }

        public EodonianPotion(Serial serial)
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

    public class BarrabHemolymphConcentrate : EodonianPotion
    {
        [Constructable]
        public BarrabHemolymphConcentrate() : this(1) { }

        [Constructable]
        public BarrabHemolymphConcentrate(int amount)
            : base(3846, PotionEffect.Barrab)
        {
            Hue = 1272;
            Stackable = true;
            Amount = amount;
        }

        public override bool CanDoEffects(Mobile m)
        {
            if (MortalStrike.IsWounded(m))
            {
                m.SendLocalizedMessage(1156869); // You may not use this with a mortal wound!
            }
            else if (m.Poison != null)
            {
                m.SendLocalizedMessage(1156868); // You may not use this while poisoned!
            }
            else if (Spells.Bushido.Confidence.IsConfident(m))
            {
                m.SendLocalizedMessage(1156873); // You may not use this while under the effects of confidence!
            }

            return base.CanDoEffects(m);
        }

        public static int GetHitBuff(Mobile m)
        {
            EodonPotionContext c = GetContext(m, PotionEffect.Barrab);

            if (c != null && c.StartTime + TimeSpan.FromMinutes(5) > DateTime.UtcNow)
                return 10;

            return 0;
        }

        public static int HPRegenBonus(Mobile m)
        {
            EodonPotionContext c = GetContext(m, PotionEffect.Barrab);

            if (c != null && c.StartTime + TimeSpan.FromMinutes(10) > DateTime.UtcNow)
                return 100;

            return 0;
        }

        public BarrabHemolymphConcentrate(Serial serial)
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

    public class JukariBurnPoiltice : EodonianPotion
    {
        [Constructable]
        public JukariBurnPoiltice() : this(1) { }

        [Constructable]
        public JukariBurnPoiltice(int amount)
            : base(3846, PotionEffect.Jukari)
        {
            Hue = 2727;
            Stackable = true;
            Amount = amount;
        }

        public override void DoEffects(Mobile m)
        {
            base.DoEffects(m);

            ResistanceMod mod = new ResistanceMod(ResistanceType.Fire, 10);
            m.AddResistanceMod(mod);

            Timer.DelayCall(TimeSpan.FromMinutes(10), () => m.RemoveResistanceMod(mod));
        }

        public static int GetStamBuff(Mobile m)
        {
            EodonPotionContext c = GetContext(m, PotionEffect.Jukari);

            if (c != null && c.StartTime + TimeSpan.FromMinutes(5) > DateTime.UtcNow)
                return 10;

            return 0;
        }

        public JukariBurnPoiltice(Serial serial)
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

    public class KurakAmbushersEssence : EodonianPotion
    {
        public override TimeSpan Cooldown => TimeSpan.FromMinutes(10);

        [Constructable]
        public KurakAmbushersEssence() : this(1) { }

        [Constructable]
        public KurakAmbushersEssence(int amount)
            : base(3846, PotionEffect.Kurak)
        {
            Hue = 1260;
            Stackable = true;
            Amount = amount;
        }

        public KurakAmbushersEssence(Serial serial)
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

    public class BarakoDraftOfMight : EodonianPotion
    {
        [Constructable]
        public BarakoDraftOfMight() : this(1) { }

        [Constructable]
        public BarakoDraftOfMight(int amount)
            : base(3846, PotionEffect.Barako)
        {
            Hue = 1072;
            Stackable = true;
            Amount = amount;
        }

        public override void DoEffects(Mobile m)
        {
            base.DoEffects(m);

            ResistanceMod mod1 = new ResistanceMod(ResistanceType.Physical, 10);
            m.AddResistanceMod(mod1);

            Timer.DelayCall(TimeSpan.FromMinutes(10), () => m.RemoveResistanceMod(mod1));

            ResistanceMod mod2 = new ResistanceMod(ResistanceType.Cold, 5);
            m.AddResistanceMod(mod1);

            Timer.DelayCall(TimeSpan.FromMinutes(10), () => m.RemoveResistanceMod(mod2));
        }

        public BarakoDraftOfMight(Serial serial)
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

    public class UraliTranceTonic : EodonianPotion
    {
        [Constructable]
        public UraliTranceTonic() : this(1) { }

        [Constructable]
        public UraliTranceTonic(int amount)
            : base(3846, PotionEffect.Urali)
        {
            Hue = 1098;
            Stackable = true;
            Amount = amount;
        }

        public override void DoEffects(Mobile m)
        {
            base.DoEffects(m);

            //TODO: Message?
        }

        public override void OnTick(Mobile m)
        {
            EodonPotionContext context = GetContext(m, PotionEffect);

            if (context != null && context.StartTime + TimeSpan.FromMinutes(10) > DateTime.UtcNow)
            {
                m.Mana += 10;
                //TODO: Message?
            }
        }

        public static int GetManaBuff(Mobile m)
        {
            EodonPotionContext c = GetContext(m, PotionEffect.Urali);

            if (c != null && c.StartTime + TimeSpan.FromMinutes(5) > DateTime.UtcNow)
                return 10;

            return 0;
        }

        public UraliTranceTonic(Serial serial)
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

    public class SakkhraProphylaxisPotion : EodonianPotion
    {
        [Constructable]
        public SakkhraProphylaxisPotion() : this(1) { }

        [Constructable]
        public SakkhraProphylaxisPotion(int amount)
            : base(3846, PotionEffect.Sakkhra)
        {
            Hue = 2531;
            Stackable = true;
            Amount = amount;
        }

        public override void DoEffects(Mobile m)
        {
            base.DoEffects(m);

            ResistanceMod mod1 = new ResistanceMod(ResistanceType.Poison, 10);
            m.AddResistanceMod(mod1);

            Timer.DelayCall(TimeSpan.FromMinutes(10), () => m.RemoveResistanceMod(mod1));

            ResistanceMod mod2 = new ResistanceMod(ResistanceType.Energy, 5);
            m.AddResistanceMod(mod2);

            Timer.DelayCall(TimeSpan.FromMinutes(10), () => m.RemoveResistanceMod(mod2));
        }

        public SakkhraProphylaxisPotion(Serial serial)
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

    // resources
    public class MyrmidexEggsac : Item, ICommodity
    {
        public override int LabelNumber => 1156725;  // Myrmidex Eggsac

        [Constructable]
        public MyrmidexEggsac() : this(1) { }

        [Constructable]
        public MyrmidexEggsac(int amount)
            : base(10248)
        {
            Hue = 1272;
            Stackable = true;
            Amount = amount;
        }

        public MyrmidexEggsac(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class LavaBerry : Item
    {
        // TODO: Harvested near Jukari Village
        public override int LabelNumber => 1156727;  // Lava Berry

        [Constructable]
        public LavaBerry() : this(1) { }

        [Constructable]
        public LavaBerry(int amount)
            : base(22326)
        {
            Hue = 1955;
            Stackable = true;
            Weight = 1.0;
            Amount = amount;
        }

        public LavaBerry(Serial serial)
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

    public class LavaBerryBush : Item
    {
        // TODO: Harvested near Jukari Village
        public override int LabelNumber => 1156735;  // Lava Berry Bush

        [Constructable]
        public LavaBerryBush()
            : base(Utility.RandomBool() ? 0xDC4 : 0xDC5)
        {
            Hue = 2075;
            Movable = false;
            Weight = 0.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                LavaBerry berry = new LavaBerry(1);
                from.AddToBackpack(berry);
                from.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1156736, "#1156727", from.NetState);

                Delete();
            }
            else
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public LavaBerryBush(Serial serial)
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

    public class PerfectBanana : Item
    {
        public override int LabelNumber => 1156730;  // Perfect Bananas

        [Constructable]
        public PerfectBanana() : this(1) { }

        [Constructable]
        public PerfectBanana(int amount)
            : base(5922)
        {
            Hue = 1119;
            Stackable = true;
            Amount = amount;
        }

        public PerfectBanana(Serial serial)
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

    public class RiverMossDecorate : Item
    {
        // TODO: Harvested near Urali Village
        public override int LabelNumber => 1156731;  // River Moss

        [Constructable]
        public RiverMossDecorate()
            : base(3378)
        {
            Hue = 1272;
            Movable = false;
            Weight = 0.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                RiverMoss rm = new RiverMoss(1);
                from.AddToBackpack(rm);
                from.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1156736, "#1156731", from.NetState);

                Delete();
            }
            else
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public RiverMossDecorate(Serial serial)
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

    public class RiverMoss : Item, ICommodity
    {
        // TODO: Harvested near Urali Village
        public override int LabelNumber => 1156731;  // River Moss

        [Constructable]
        public RiverMoss() : this(1) { }

        [Constructable]
        public RiverMoss(int amount)
            : base(22333)
        {
            Hue = 1272;
            Stackable = true;
            Amount = amount;
        }

        public RiverMoss(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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

    public class BlueCorn : EarOfCorn
    {
        // TODO: Harvestable from Sakkhra corn fields
        public override int LabelNumber => 1156733;  // Blue Corn

        [Constructable]
        public BlueCorn() : this(1) { }

        [Constructable]
        public BlueCorn(int amount)
        {
            Hue = 1284;
            Amount = amount;
        }

        public BlueCorn(Serial serial)
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

    public class CornStalk : Item
    {
        public override int LabelNumber => 1035639;  // corn stalk
        private int m_Used;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Used
        {
            get { return m_Used; }
            set { m_Used = value; }
        }

        [Constructable]
        public CornStalk()
            : base(3197)
        {
            m_Used = Utility.RandomMinMax(1, 4);
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 3))
            {
                Item corn;

                if (Utility.RandomDouble() < 0.70)
                {
                    corn = new EarOfCorn(1);
                    from.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1156736, "#1156737", from.NetState);
                }
                else
                {
                    corn = new BlueCorn(1);
                    from.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1156736, "#1156733", from.NetState);
                }

                from.AddToBackpack(corn);

                if (Used > 1)
                {
                    Used--;
                }
                else
                {
                    Delete();
                }
            }
            else
            {
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public CornStalk(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Used);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Used = reader.ReadInt();
        }
    }

    public class MoonstoneCrystalShard : Item
    {
        public override int LabelNumber => 1124142;  // Moonstone Crystal Shards

        [Constructable]
        public MoonstoneCrystalShard() : this(1) { }

        [Constructable]
        public MoonstoneCrystalShard(int amount)
            : base(40118)
        {
            Stackable = true;
            Amount = amount;
        }

        public MoonstoneCrystalShard(Serial serial)
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
