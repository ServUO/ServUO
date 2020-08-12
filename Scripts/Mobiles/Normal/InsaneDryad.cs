using Server.Items;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a dryad's corpse")]
    public class MLDryad : BaseCreature
    {
        public override bool InitialInnocent => true;
        public static TimeSpan PeaceDuration => TimeSpan.FromSeconds(20);

        [Constructable]
        public MLDryad()
            : base(AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            Name = "a dryad";
            Body = 266;
            BaseSoundID = 0x57B;

            SetStr(132, 149);
            SetDex(152, 168);
            SetInt(251, 280);

            SetHits(304, 321);

            SetDamage(11, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 40, 45);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.Meditation, 80.0, 90.0);
            SetSkill(SkillName.EvalInt, 70.0, 80.0);
            SetSkill(SkillName.Magery, 70.0, 80.0);
            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 70.0, 80.0);
            SetSkill(SkillName.Wrestling, 70.0, 80.0);

            Fame = 5000;
            Karma = 5000;
        }

        public MLDryad(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.ArcanistScrolls, 0, 1);
            AddLoot(LootPack.PeculiarSeed1);
        }

        public override int Meat => 1;

        public override void OnThink()
        {
            base.OnThink();

            AreaPeace();
            AreaUndress();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Peaced != null)
            {
                var peaced = Peaced.Keys.ToList();

                for (int i = 0; i < peaced.Count; i++)
                {
                    var pm = peaced[i] as PlayerMobile;

                    if (pm != null)
                    {
                        pm.PeacedUntil = DateTime.UtcNow;
                    }

                    RemoveTimer(peaced[i]);
                }

                ColUtility.Free(peaced);
            }
        }

        #region Area Peace
        public Dictionary<Mobile, Timer> Peaced { get; set; }

        private DateTime m_NextPeace;

        public void AreaPeace()
        {
            if (Combatant == null || Deleted || !Alive || m_NextPeace > DateTime.UtcNow || 0.1 < Utility.RandomDouble())
                return;

            IPooledEnumerable eable = GetMobilesInRange(RangePerception);

            foreach (var p in eable.OfType<PlayerMobile>())
            {
                if (IsValidTarget(p))
                {
                    AddPeaceEffects(p);
                }
            }

            eable.Free();

            m_NextPeace = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            PlaySound(0x1D3);
        }

        public void AddPeaceEffects(PlayerMobile p)
        {
            p.SendLocalizedMessage(1072065); // You gaze upon the dryad's beauty, and forget to continue battling!
            p.FixedParticles(0x376A, 1, 20, 0x7F5, EffectLayer.Waist);

            p.Warmode = false;
            p.Combatant = null;

            if (Peaced == null)
            {
                Peaced = new Dictionary<Mobile, Timer>();
            }

            if (Peaced.ContainsKey(p))
            {
                Peaced[p].Stop();
            }

            p.PeacedUntil = DateTime.UtcNow + PeaceDuration;
            Peaced[p] = Timer.DelayCall(PeaceDuration, RemoveTimer, p);
        }

        public bool IsValidTarget(PlayerMobile m)
        {
            return m.PeacedUntil < DateTime.UtcNow && !m.Hidden && CanBeHarmful(m);
        }

        public void RemoveTimer(Mobile m)
        {
            if (Peaced != null && Peaced.ContainsKey(m))
            {
                Peaced.Remove(m);

                if (Peaced.Count == 0)
                {
                    Peaced = null;
                }
            }
        }
        #endregion

        #region Undress
        private DateTime m_NextUndress;

        public void AreaUndress()
        {
            if (Combatant == null || Deleted || !Alive || m_NextUndress > DateTime.UtcNow || 0.05 < Utility.RandomDouble())
                return;

            IPooledEnumerable eable = GetMobilesInRange(RangePerception);

            foreach (Mobile m in eable)
            {
                if (m != null && m.Player && !m.Female && !m.Hidden && m.IsPlayer() && CanBeHarmful(m))
                {
                    UndressItem(m, Layer.OuterTorso);
                    UndressItem(m, Layer.InnerTorso);
                    UndressItem(m, Layer.MiddleTorso);
                    UndressItem(m, Layer.Pants);
                    UndressItem(m, Layer.Shirt);

                    m.SendLocalizedMessage(1072197); // The dryad's beauty makes your blood race. Your clothing is too confining.
                }
            }
            eable.Free();

            m_NextUndress = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        }

        public void UndressItem(Mobile m, Layer layer)
        {
            Item item = m.FindItemOnLayer(layer);

            if (item != null && item.Movable)
                m.PlaceInBackpack(item);
        }

        #endregion

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
    }

    [CorpseName("an insane dryad corpse")]
    public class InsaneDryad : MLDryad
    {
        public override bool InitialInnocent => false;

        [Constructable]
        public InsaneDryad()
            : base()
        {
            Name = "an insane dryad";
            Hue = 0x487;

            FightMode = FightMode.Closest;

            Fame = 7000;
            Karma = -7000;
        }

        public InsaneDryad(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new ParrotItem());
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
    }
}
