using Server;
using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Spells;

namespace Server.Engines.Despise
{
    public class Silenii : DespiseCreature
    {
        [Constructable]
        public Silenii() : this(1)
        {
        }

        [Constructable]
        public Silenii(int powerLevel) : base(AIType.AI_Melee, FightMode.Evil)
        {
            Name = "Silenii";
            Body = 0x10F;
            BaseSoundID = 0x585;

            Fame = GetFame;
            Karma = GetKarmaGood;

            Power = powerLevel;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(65, 75); } }
        public override int DexStart { get { return Utility.RandomMinMax(100, 110); } }
        public override int IntStart { get { return Utility.RandomMinMax(100, 110); } }

        public override bool CanDiscord { get { return true; } }
        public override TimeSpan DiscordInterval { get { return TimeSpan.FromSeconds(45); } }

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

        public Silenii(Serial serial) : base(serial)
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

    public class ForestNymph : DespiseCreature
    {
        [Constructable]
        public ForestNymph() : this(1)
        {
        }

        [Constructable]
        public ForestNymph(int powerLevel) : base(AIType.AI_Mage, FightMode.Evil)
        {
            Name = "Forest Nymph";
            Body = 266;
            BaseSoundID = 0x467;

            Fame = GetFame;
            Karma = GetKarmaGood;
            Power = powerLevel;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMageAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(65, 80); } }
        public override int DexStart { get { return Utility.RandomMinMax(70, 80); } }
        public override int IntStart { get { return Utility.RandomMinMax(110, 150); } }

        public ForestNymph(Serial serial) : base(serial)
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

    public class DespiseUnicorn : DespiseCreature
    {
        [Constructable]
        public DespiseUnicorn() : this(1)
        {
        }

        [Constructable]
        public DespiseUnicorn(int powerLevel) : base(AIType.AI_Melee, FightMode.Evil)
        {
            Name = "Unicorn";
            Body = 0x7A;
            BaseSoundID = 0x4BC;

            Fame = GetFame;
            Karma = GetKarmaGood;
            Power = powerLevel;

            SetWeaponAbility(WeaponAbility.ArmorIgnore);
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(80, 100); } }
        public override int DexStart { get { return Utility.RandomMinMax(110, 115); } }
        public override int IntStart { get { return Utility.RandomMinMax(100, 115); } }

        public override bool RaiseDamage { get { return true; } }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public DespiseUnicorn(Serial serial) : base(serial)
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

    [TypeAlias("Server.Engines.Despise.Sagittari")]
    public class Sagittarri : DespiseCreature
    {
        [Constructable]
        public Sagittarri() : this(1)
        {
        }

        [Constructable]
        public Sagittarri(int powerLevel) : base(AIType.AI_Archer, FightMode.Evil)
        {
            Name = "Sagittarri";
            Body = 101;
            BaseSoundID = 679;

            SetSkill(SkillName.Archery, SkillStart);

            Fame = GetFame;
            Karma = GetKarmaGood;

            AddItem(new Bow());
            PackItem(new Arrow(Utility.RandomMinMax(5, 10)));
            Power = powerLevel;

            RangeFight = 8;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override int StrStart { get { return Utility.RandomMinMax(40, 55); } }
        public override int DexStart { get { return Utility.RandomMinMax(160, 180); } }
        public override int IntStart { get { return Utility.RandomMinMax(110, 120); } }
        
        public override bool RaiseDamage { get { return true; } }

        public Sagittarri(Serial serial)
            : base(serial)
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

    public class Ursadane : DespiseCreature
    {
        [Constructable]
        public Ursadane() : this(1)
        {
        }

        [Constructable]
        public Ursadane(int powerLevel) : base(AIType.AI_Melee, FightMode.Evil)
        {
            Name = "Ursadane";
            Body = 212;
            BaseSoundID = 0xA3;

            Fame = GetFame;
            Karma = GetKarmaGood;
            Power = powerLevel;

            SetWeaponAbility(WeaponAbility.CrushingBlow);
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Bear; } }

        public override bool RaiseDamage { get { return true; } }

        public override int StrStart { get { return Utility.RandomMinMax(150, 175); } }
        public override int DexStart { get { return Utility.RandomMinMax(90, 105); } }
        public override int IntStart { get { return Utility.RandomMinMax(30, 40); } }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public Ursadane(Serial serial) : base(serial)
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

    public class DivineGuardian : DespiseCreature
    {
        [Constructable]
        public DivineGuardian() : this(1)
        {
        }

        [Constructable]
        public DivineGuardian(int powerLevel) : base(AIType.AI_Melee, FightMode.Evil)
        {
            Name = "Divine Guardian";
            Body = 123;
            BaseSoundID = 0x2F7;

            Fame = GetFame;
            Karma = GetKarmaGood;
            Power = powerLevel;

            SetWeaponAbility(WeaponAbility.ConcussionBlow);
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }

        public override bool RaiseDamage { get { return true; } }

        public override int StrStart { get { return Utility.RandomMinMax(150, 175); } }
        public override int DexStart { get { return Utility.RandomMinMax(120, 130); } }
        public override int IntStart { get { return Utility.RandomMinMax(50, 60); } }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public DivineGuardian(Serial serial) : base(serial)
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
                SetWeaponAbility(WeaponAbility.ConcussionBlow);
            }
        }
    }

    public class Dendrite : DespiseCreature
    {
        [Constructable]
        public Dendrite() : this(1)
        {
        }

        [Constructable]
        public Dendrite(int powerLevel) : base(AIType.AI_Melee, FightMode.Evil)
        {
            Name = "Dendrite";
            Body = 301;

            Fame = GetFame;
            Karma = GetKarmaGood;
            Power = powerLevel;

            SetWeaponAbility(WeaponAbility.DoubleStrike);
        }

        public override int GetIdleSound()
        {
            return 443;
        }

        public override int GetDeathSound()
        {
            return 31;
        }

        public override int GetAttackSound()
        {
            return 672;
        }

        protected override BaseAI ForcedAI { get { return new DespiseMeleeAI(this); } }

        public override bool RaiseDamage { get { return true; } }

        public override int StrStart { get { return Utility.RandomMinMax(100, 120); } }
        public override int DexStart { get { return Utility.RandomMinMax(140, 155); } }
        public override int IntStart { get { return Utility.RandomMinMax(30, 50); } }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public Dendrite(Serial serial) : base(serial)
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

    public class Fairy : DespiseCreature
    {
        private DateTime m_NextHeal;
        private readonly double HealThreshold = 0.60;

        public virtual int MinHeal { get { return Math.Max(10, Power * 3); } }
        public virtual int MaxHeal { get { return Math.Max(25, Power * 5); } }

        [Constructable]
        public Fairy() : this(1)
        {
        }

        [Constructable]
        public Fairy(int powerLevel) : base(AIType.AI_Melee, FightMode.Evil)
        {
            Name = "Fairy";
            Body = 0x108;
            BaseSoundID = 0x467;

            Fame = GetFame;
            Karma = GetKarmaGood;

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

        public Fairy(Serial serial) : base(serial)
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
