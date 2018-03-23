using System;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a changeling corpse")]
    public class Changeling : BaseCreature
    {
        private static readonly int[] m_FireNorth = new int[]
        {
            -1, -1,
            1, -1,
            -1, 2,
            1, 2
        };
        private static readonly int[] m_FireEast = new int[]
        {
            -1, 0,
            2, 0
        };

        private Mobile m_MorphedInto;
        private DateTime m_LastMorph;

        [Constructable]
        public Changeling()
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = DefaultName;
            Body = 264;
            Hue = DefaultHue;

            SetStr(36, 105);
            SetDex(212, 262);
            SetInt(317, 399);

            SetHits(201, 211);
            SetStam(212, 262);
            SetMana(317, 399);

            SetDamage(9, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 81, 90);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 49);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 43, 50);

            SetSkill(SkillName.Wrestling, 10.4, 12.5);
            SetSkill(SkillName.Tactics, 101.1, 108.3);
            SetSkill(SkillName.MagicResist, 121.6, 132.2);
            SetSkill(SkillName.Magery, 91.6, 99.5);
            SetSkill(SkillName.EvalInt, 91.5, 98.8);
            SetSkill(SkillName.Meditation, 91.7, 98.5);

            SetSkill(SkillName.Spellweaving, 91.6, 99.5);

            Fame = 15000;
            Karma = -15000;

            PackItem(new Arrow(35));
            PackItem(new Bolt(25));
            PackGem(2);

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Changeling(Serial serial)
            : base(serial)
        {
        }

        public override bool IsEnemy(Mobile m)
        {
            if (m is BaseCreature && ((BaseCreature)m).IsMonster && m.Karma > 0)
            {
                return true;
            }

            return base.IsEnemy(m);
        }

        public virtual string DefaultName
        {
            get
            {
                return "a changeling";
            }
        }
        public virtual int DefaultHue
        {
            get
            {
                return 0;
            }
        }

        public override bool UseSmartAI { get { return true; } }

        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override bool InitialInnocent
        {
            get
            {
                return (m_MorphedInto != null);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile MorphedInto
        {
            get
            {
                return m_MorphedInto;
            }
            set
            {
                if (value == this)
                    value = null;

                if (m_MorphedInto != value)
                {
                    Revert();

                    if (value != null)
                    {
                        Morph(value);
                        m_LastMorph = DateTime.UtcNow;
                    }

                    m_MorphedInto = value;
                    Delta(MobileDelta.Noto);
                }
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosRich, 3);
            AddLoot(LootPack.LowScrolls);
            AddLoot(LootPack.MedScrolls);
        }

        public override int GetAngerSound()
        {
            return 0x46E;
        }

        public override int GetIdleSound()
        {
            return 0x470;
        }

        public override int GetAttackSound()
        {
            return 0x46D;
        }

        public override int GetHurtSound()
        {
            return 0x471;
        }

        public override int GetDeathSound()
        {
            return 0x46F;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant is PlayerMobile && m_MorphedInto != Combatant && Utility.RandomDouble() < 0.05)
            {
                MorphedInto = Combatant as Mobile;
            }
        }

        public override bool CheckIdle()
        {
            bool idle = base.CheckIdle();

            if (idle && m_MorphedInto != null && DateTime.UtcNow - m_LastMorph > TimeSpan.FromSeconds(30))
                MorphedInto = null;

            return idle;
        }

        public void DeleteClonedItems()
        {
            for (int i = Items.Count - 1; i >= 0; --i)
            {
                Item item = Items[i];

                if (item is ClonedItem)
                    item.Delete();
            }

            if (Backpack != null)
            {
                for (int i = Backpack.Items.Count - 1; i >= 0; --i)
                {
                    Item item = Backpack.Items[i];

                    if (item is ClonedItem)
                        item.Delete();
                }
            }
        }

        public override void OnAfterDelete()
        {
            DeleteClonedItems();

            base.OnAfterDelete();
        }

        public override void ClearHands()
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((m_MorphedInto != null));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (reader.ReadBool())
                ValidationQueue<Changeling>.Add(this);
        }

        public void Validate()
        {
            Revert();
        }

        protected virtual void Morph(Mobile m)
        {
            Body = m.Body;
            Hue = m.Hue;
            Female = m.Female;
            Name = m.Name;
            NameHue = m.NameHue;
            Title = m.Title;
            Kills = m.Kills;
            HairItemID = m.HairItemID;
            HairHue = m.HairHue;
            FacialHairItemID = m.FacialHairItemID;
            FacialHairHue = m.FacialHairHue;

            // TODO: Skills?

            foreach (Item item in m.Items)
            {
                if (item.Layer != Layer.Backpack && item.Layer != Layer.Mount && item.Layer != Layer.Bank)
                    AddItem(new ClonedItem(item)); // TODO: Clone weapon/armor attributes
            }

            PlaySound(0x511);
            FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);
        }

        protected virtual void Revert()
        {
            Body = 264;
            Hue = (IsParagon && DefaultHue == 0) ? Paragon.Hue : DefaultHue;
            Female = false;
            Name = DefaultName;
            NameHue = -1;
            Title = null;
            Kills = 0;
            HairItemID = 0;
            HairHue = 0;
            FacialHairItemID = 0;
            FacialHairHue = 0;

            DeleteClonedItems();

            PlaySound(0x511);
            FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);
        }

        private void FireEffects(int itemID, int[] offsets)
        {
            for (int i = 0; i < offsets.Length; i += 2)
            {
                Point3D p = Location;

                p.X += offsets[i];
                p.Y += offsets[i + 1];

                if (SpellHelper.AdjustField(ref p, Map, 12, false))
                    Effects.SendLocationEffect(p, Map, itemID, 50);
            }
        }

        private class ClonedItem : Item
        {
            public ClonedItem(Item item)
                : base(item.ItemID)
            {
                Name = item.Name;
                Weight = item.Weight;
                Hue = item.Hue;
                Layer = item.Layer;
                Movable = false;
            }

            public ClonedItem(Serial serial)
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
}