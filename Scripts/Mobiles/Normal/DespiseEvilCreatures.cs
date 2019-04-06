using Server;
using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Spells;

namespace Server.Engines.Despise
{
    public class Phantom : DespiseCreature
    {
        [Constructable]
        public Phantom() : this(1)
        {
        }

        [Constructable]
        public Phantom(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
        {
            Name = "Phantom";
            Body = 0xFC;
            BaseSoundID = 0x482;
            Hue = 2671;

            Fame = GetFame;
            Karma = GetKarmaEvil;

            Power = powerLevel;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(65, 75); } }
        public override int DexStart { get { return Utility.RandomMinMax(100, 110); } }
        public override int IntStart { get { return Utility.RandomMinMax(100, 110); } }

        public override Mobile GetBardTarget(bool creaturesOnly = false)
        {
            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, RangePerception);

            Mobile closest = null;
            int range = 0;

            foreach (Mobile m in eable)
            {
                if (CanDoTarget(m))
                {
                    int dist = (int)GetDistanceToSqrt(m);

                    if (closest == null || dist < range)
                    {
                        closest = m;
                        range = dist;
                    }
                }
            }
            eable.Free();

            return closest;
        }

        private bool CanDoTarget(Mobile m)
        {
            int discordanceEffect = 0;

            if (!CanBeHarmful(m, false) || Server.SkillHandlers.Discordance.GetEffect(m, ref discordanceEffect))
                return false;

            if ((m is DespiseCreature && ((DespiseCreature)m).Alignment != Alignment.Neutral && ((DespiseCreature)m).Alignment != this.Alignment) || m is DespiseBoss)
                return true;

            return m is PlayerMobile && !this.Controlled && ((m.Karma < 0 && this.Alignment == Alignment.Good) || (m.Karma > 0 && this.Alignment == Alignment.Evil));
        }

        public Phantom(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Naba : DespiseCreature
    {
        [Constructable]
        public Naba() : this(1)
        {
        }

        [Constructable]
        public Naba(int powerLevel) : base(AIType.AI_Mage, FightMode.Good)
        {
            Name = "Naba";
            Body = 0x88;
            BaseSoundID = 639;
            Hue = 2707;

            Fame = GetFame;
            Karma = GetKarmaEvil;
            Power = powerLevel;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMageAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(65, 80); } }
        public override int DexStart { get { return Utility.RandomMinMax(70, 80); } }
        public override int IntStart { get { return Utility.RandomMinMax(110, 150); } }

        public Naba(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Darkmane : DespiseCreature
    {
        [Constructable]
        public Darkmane() : this(1)
        {
        }

        [Constructable]
        public Darkmane(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
        {
            Name = "Darkmane";
            Body = 0xCC;
            Hue = 1910;
            BaseSoundID = 0xA8;

            Fame = GetFame;
            Karma = GetKarmaEvil;
            Power = powerLevel;

            SetWeaponAbility(WeaponAbility.ArmorIgnore);
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(80, 100); } }
        public override int DexStart { get { return Utility.RandomMinMax(110, 115); } }
        public override int IntStart { get { return Utility.RandomMinMax(100, 115); } }

        public override bool RaiseDamage { get { return true; } }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public Darkmane(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if (v == 0)
            {
                SetWeaponAbility(WeaponAbility.ArmorIgnore);
            }
        }
    }

    public class Skeletrex : DespiseCreature
    {
        [Constructable]
        public Skeletrex() : this(1)
        {
        }

        [Constructable]
        public Skeletrex(int powerLevel) : base(AIType.AI_Archer, FightMode.Good)
        {
            Name = "Skeletrex";
            Body = 147;
            BaseSoundID = 451;
            Hue = 2075;

            SetSkill(SkillName.Archery, SkillStart);

            Fame = GetFame;
            Karma = GetKarmaEvil;

            AddItem(new Bow());
            PackItem(new Arrow(Utility.RandomMinMax(5, 10))); // OSI it is different: in a sub backpack, this is probably just a limitation of their engine
            Power = powerLevel;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(40, 55); } }
        public override int DexStart { get { return Utility.RandomMinMax(160, 180); } }
        public override int IntStart { get { return Utility.RandomMinMax(110, 120); } }
        
        public override bool RaiseDamage { get { return true; } }

        public Skeletrex(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Hellion : DespiseCreature
    {
        [Constructable]
        public Hellion() : this(1)
        {
        }

        [Constructable]
        public Hellion(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
        {
            Name = "Hellion";
            Body = 4;
            BaseSoundID = 0x174;
            Hue = 2671;

            Fame = GetFame;
            Karma = GetKarmaEvil;
            Power = powerLevel;

            SetWeaponAbility(WeaponAbility.CrushingBlow);
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Bear; } }
        public override int MinDamMax { get { return 15; } }
        public override int MaxDamMax { get { return 26; } }
        public override bool RaiseDamage { get { return true; } }

        public override int StrStart { get { return Utility.RandomMinMax(150, 175); } }
        public override int DexStart { get { return Utility.RandomMinMax(90, 105); } }
        public override int IntStart { get { return Utility.RandomMinMax(30, 40); } }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public Hellion(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if (v == 0)
            {
                SetWeaponAbility(WeaponAbility.CrushingBlow);
            }
        }
    }

    public class Echidnite : DespiseCreature
    {
        [Constructable]
        public Echidnite() : this(1)
        {
        }

        [Constructable]
        public Echidnite(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
        {
            Name = "Echidnite";
            Body = 250;
            BaseSoundID = 0x52A;
            Hue = 2671;

            Fame = GetFame;
            Karma = GetKarmaEvil;
            Power = powerLevel;

            SetWeaponAbility(WeaponAbility.ConcussionBlow);
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        
        public override bool RaiseDamage { get { return true; } }

        public override int StrStart { get { return Utility.RandomMinMax(150, 175); } }
        public override int DexStart { get { return Utility.RandomMinMax(120, 130); } }
        public override int IntStart { get { return Utility.RandomMinMax(50, 60); } }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public Echidnite(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if (v == 0)
            {
                SetWeaponAbility(WeaponAbility.ConcussionBlow);
            }
        }
    }

    [TypeAlias("Server.Engines.Despise.BerlingBlades")]
    public class BirlingBlades : DespiseCreature
    {
        [Constructable]
        public BirlingBlades() : this(1)
        {
        }

        [Constructable]
        public BirlingBlades(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
        {
            Name = "Birling Blades";
            Body = 574;
            BaseSoundID = 224;
            Hue = 2672;

            Fame = GetFame;
            Karma = GetKarmaEvil;
            Power = powerLevel;

            SetWeaponAbility(WeaponAbility.DoubleStrike);
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        
        public override bool RaiseDamage { get { return true; } }

        public override int StrStart { get { return Utility.RandomMinMax(100, 120); } }
        public override int DexStart { get { return Utility.RandomMinMax(140, 155); } }
        public override int IntStart { get { return Utility.RandomMinMax(30, 50); } }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public override int GetAngerSound()
        {
            return 0x23A;
        }

        public override int GetAttackSound()
        {
            return 0x3B8;
        }

        public override int GetHurtSound()
        {
            return 0x23A;
        }

        public BirlingBlades(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if (v == 0)
            {
                SetWeaponAbility(WeaponAbility.DoubleStrike);
            }
        }
    }

    public class Prometheoid : DespiseCreature
    {
        private DateTime m_NextHeal;
        private readonly double HealThreshold = 0.5;

        public virtual int MinHeal { get { return Math.Max(10, Power * 3); } }
        public virtual int MaxHeal { get { return Math.Max(25, Power * 5); } }

        [Constructable]
        public Prometheoid() : this(1)
        {
        }

        [Constructable]
        public Prometheoid(int powerLevel) : base(AIType.AI_Melee, FightMode.Good)
        {
            Name = "Prometheoid";
            Body = 305;
            BaseSoundID = 224;
            Hue = 2671;

            Fame = GetFame;
            Karma = GetKarmaEvil;

            m_NextHeal = DateTime.UtcNow;
            Power = powerLevel;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(85, 100); } }
        public override int DexStart { get { return Utility.RandomMinMax(110, 125); } }
        public override int IntStart { get { return Utility.RandomMinMax(130, 150); } }

        public override void OnThink()
        {
            base.OnThink();

            if (m_NextHeal < DateTime.UtcNow && this.Map != null && this.Map != Map.Internal)
            {
                List<Mobile> eligables = new List<Mobile>();
                IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 8);

                foreach (Mobile m in eable)
                {
                    if (m.Alive && m.Hits <= (int)((double)m.HitsMax * HealThreshold) && CanDoHeal(m))
                        eligables.Add(m);
                }

                if (eligables.Count > 0)
                {
                    Mobile m = eligables[Utility.Random(eligables.Count)];

                    Direction = GetDirectionTo(m);

                    SpellHelper.Heal(Utility.RandomMinMax(MinHeal, MaxHeal), m, this);
                    m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                    m.PlaySound(0x202);

                    int nextHeal = Utility.RandomMinMax(20 - Power, 30 - Power);
                    m_NextHeal = DateTime.UtcNow + TimeSpan.FromSeconds(nextHeal);
                    return;
                }

                m_NextHeal = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            }
        }

        private bool CanDoHeal(Mobile toHeal)
        {
            if (toHeal is DespiseCreature && ((DespiseCreature)toHeal).Alignment == Alignment)
                return true;

            return toHeal is PlayerMobile && ((toHeal.Karma < 0 && Alignment == Alignment.Evil) || (toHeal.Karma > 0 && Alignment == Alignment.Good));
        }

        public Prometheoid(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            m_NextHeal = DateTime.UtcNow;
        }
    }
}
