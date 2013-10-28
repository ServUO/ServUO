using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a satyr's corpse")]
    public class Satyr : BaseCreature
    {
        [Constructable]
        public Satyr()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a satyr";
            this.Body = 271;
            this.BaseSoundID = 0x586;

            this.SetStr(177, 195);
            this.SetDex(251, 269);
            this.SetInt(153, 170);

            this.SetHits(350, 400);

            this.SetDamage(13, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 60);
            this.SetResistance(ResistanceType.Fire, 25, 35);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 55.0, 65.0);
            this.SetSkill(SkillName.Tactics, 80.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.0, 100.0);

            this.Fame = 5000;
            this.Karma = 0;

            this.VirtualArmor = 28; // Don't know what it should be

            this.PackArcanceScroll(0.05);
        }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.MlRich);
        }

        public override void OnThink()
        {
            base.OnThink();

            this.Peace(this.Combatant);
            this.Undress(this.Combatant);
            this.Suppress(this.Combatant);
            this.Provoke(this.Combatant);
        }

        #region Peace
        private DateTime m_NextPeace;

        public void Peace(Mobile target)
        {
            if (target == null || this.Deleted || !this.Alive || this.m_NextPeace > DateTime.UtcNow || 0.1 < Utility.RandomDouble())
                return;

            PlayerMobile p = target as PlayerMobile;

            if (p != null && p.PeacedUntil < DateTime.UtcNow && !p.Hidden && this.CanBeHarmful(p))
            {
                p.PeacedUntil = DateTime.UtcNow + TimeSpan.FromMinutes(1);
                p.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!
                p.FixedParticles(0x376A, 1, 32, 0x15BD, EffectLayer.Waist);
                p.Combatant = null;

                this.PlaySound(0x58D);
            }

            this.m_NextPeace = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        }

        #endregion

        #region Suppress
        private static readonly Dictionary<Mobile, Timer> m_Suppressed = new Dictionary<Mobile, Timer>();
        private DateTime m_NextSuppress;

        public void Suppress(Mobile target)
        {
            if (target == null || m_Suppressed.ContainsKey(target) || this.Deleted || !this.Alive || this.m_NextSuppress > DateTime.UtcNow || 0.1 < Utility.RandomDouble())
                return;

            TimeSpan delay = TimeSpan.FromSeconds(Utility.RandomMinMax(20, 80));

            if (!target.Hidden && this.CanBeHarmful(target))
            {
                target.SendLocalizedMessage(1072061); // You hear jarring music, suppressing your strength.

                for (int i = 0; i < target.Skills.Length; i++)
                {
                    Skill s = target.Skills[i];

                    target.AddSkillMod(new TimedSkillMod(s.SkillName, true, s.Base * -0.28, delay));
                }

                int count = (int)Math.Round(delay.TotalSeconds / 1.25);
                Timer timer = new AnimateTimer(target, count);
                m_Suppressed.Add(target, timer);
                timer.Start();

                this.PlaySound(0x58C);
            }

            this.m_NextSuppress = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        }

        public static void SuppressRemove(Mobile target)
        {
            if (target != null && m_Suppressed.ContainsKey(target))
            {
                Timer timer = m_Suppressed[target];

                if (timer != null || timer.Running)
                    timer.Stop();

                m_Suppressed.Remove(target);
            }
        }

        private class AnimateTimer : Timer
        {
            private readonly Mobile m_Owner;
            private int m_Count;

            public AnimateTimer(Mobile owner, int count)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.25))
            {
                this.m_Owner = owner;
                this.m_Count = count;
            }

            protected override void OnTick()
            {
                if (this.m_Owner.Deleted || !this.m_Owner.Alive || this.m_Count-- < 0)
                {
                    SuppressRemove(this.m_Owner);
                }
                else
                    this.m_Owner.FixedParticles(0x376A, 1, 32, 0x15BD, EffectLayer.Waist);
            }
        }
        #endregion

        #region Undress
        private DateTime m_NextUndress;

        public void Undress(Mobile target)
        {
            if (target == null || this.Deleted || !this.Alive || this.m_NextUndress > DateTime.UtcNow || 0.005 < Utility.RandomDouble())
                return;

            if (target.Player && target.Female && !target.Hidden && this.CanBeHarmful(target))
            {
                this.UndressItem(target, Layer.OuterTorso);
                this.UndressItem(target, Layer.InnerTorso);
                this.UndressItem(target, Layer.MiddleTorso);
                this.UndressItem(target, Layer.Pants);
                this.UndressItem(target, Layer.Shirt);

                target.SendLocalizedMessage(1072196); // The satyr's music makes your blood race. Your clothing is too confining.
            }

            this.m_NextUndress = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        }

        public void UndressItem(Mobile m, Layer layer)
        {
            Item item = m.FindItemOnLayer(layer);

            if (item != null && item.Movable)
                m.PlaceInBackpack(item);
        }

        #endregion

        #region Provoke
        private DateTime m_NextProvoke;

        public void Provoke(Mobile target)
        {
            if (target == null || this.Deleted || !this.Alive || this.m_NextProvoke > DateTime.UtcNow || 0.05 < Utility.RandomDouble())
                return;

            foreach (Mobile m in this.GetMobilesInRange(this.RangePerception))
            {
                if (m is BaseCreature)
                {
                    BaseCreature c = (BaseCreature)m;

                    if (c == this || c == target || c.Unprovokable || c.IsParagon || c.BardProvoked || c.IsStaff() || !c.CanBeHarmful(target))
                        continue;

                    c.Provoke(this, target, true);

                    if (target.Player)
                        target.SendLocalizedMessage(1072062); // You hear angry music, and start to fight.

                    this.PlaySound(0x58A);
                    break;
                }
            }

            this.m_NextProvoke = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        }

        #endregion

        public override int Meat
        {
            get
            {
                return 1;
            }
        }

        public Satyr(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}