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
        private DateTime m_NextFireRing;
        [Constructable]
        public Changeling()
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = this.DefaultName;
            this.Body = 264;
            this.Hue = this.DefaultHue;

            this.SetStr(36, 105);
            this.SetDex(212, 262);
            this.SetInt(317, 399);

            this.SetHits(201, 211);
            this.SetStam(212, 262);
            this.SetMana(317, 399);

            this.SetDamage(9, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 81, 90);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 49);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 43, 50);

            this.SetSkill(SkillName.Wrestling, 10.4, 12.5);
            this.SetSkill(SkillName.Tactics, 101.1, 108.3);
            this.SetSkill(SkillName.MagicResist, 121.6, 132.2);
            this.SetSkill(SkillName.Magery, 91.6, 99.5);
            this.SetSkill(SkillName.EvalInt, 91.5, 98.8);
            this.SetSkill(SkillName.Meditation, 91.7, 98.5);

            this.Fame = 15000;
            this.Karma = -15000;

            this.PackItem(new Arrow(35));
            this.PackItem(new Bolt(25));
            this.PackGem(2);

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Changeling(Serial serial)
            : base(serial)
        {
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
                return (this.m_MorphedInto != null);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile MorphedInto
        {
            get
            {
                return this.m_MorphedInto;
            }
            set
            {
                if (value == this)
                    value = null;

                if (this.m_MorphedInto != value)
                {
                    this.Revert();

                    if (value != null)
                    {
                        this.Morph(value);
                        this.m_LastMorph = DateTime.UtcNow;
                    }

                    this.m_MorphedInto = value;
                    this.Delta(MobileDelta.Noto);
                }
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosRich, 3);
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

            if (this.Combatant != null)
            {
                if (this.m_NextFireRing <= DateTime.UtcNow && Utility.RandomDouble() < 0.02)
                {
                    this.FireRing();
                    this.m_NextFireRing = DateTime.UtcNow + TimeSpan.FromMinutes(2);
                }

                if (this.Combatant.Player && this.m_MorphedInto != this.Combatant && Utility.RandomDouble() < 0.05)
                    this.MorphedInto = this.Combatant;
            }
        }

        public override bool CheckIdle()
        {
            bool idle = base.CheckIdle();

            if (idle && this.m_MorphedInto != null && DateTime.UtcNow - this.m_LastMorph > TimeSpan.FromSeconds(30))
                this.MorphedInto = null;

            return idle;
        }

        public void DeleteClonedItems()
        {
            for (int i = this.Items.Count - 1; i >= 0; --i)
            {
                Item item = this.Items[i];

                if (item is ClonedItem)
                    item.Delete();
            }

            if (this.Backpack != null)
            {
                for (int i = this.Backpack.Items.Count - 1; i >= 0; --i)
                {
                    Item item = this.Backpack.Items[i];

                    if (item is ClonedItem)
                        item.Delete();
                }
            }
        }

        public override void OnAfterDelete()
        {
            this.DeleteClonedItems();

            base.OnAfterDelete();
        }

        public override void ClearHands()
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((this.m_MorphedInto != null));
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
            this.Revert();
        }

        protected virtual void FireRing()
        {
            this.FireEffects(0x3E27, m_FireNorth);
            this.FireEffects(0x3E31, m_FireEast);
        }

        protected virtual void Morph(Mobile m)
        {
            this.Body = m.Body;
            this.Hue = m.Hue;
            this.Female = m.Female;
            this.Name = m.Name;
            this.NameHue = m.NameHue;
            this.Title = m.Title;
            this.Kills = m.Kills;
            this.HairItemID = m.HairItemID;
            this.HairHue = m.HairHue;
            this.FacialHairItemID = m.FacialHairItemID;
            this.FacialHairHue = m.FacialHairHue;

            // TODO: Skills?

            foreach (Item item in m.Items)
            {
                if (item.Layer != Layer.Backpack && item.Layer != Layer.Mount && item.Layer != Layer.Bank)
                    this.AddItem(new ClonedItem(item)); // TODO: Clone weapon/armor attributes
            }

            this.PlaySound(0x511);
            this.FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);
        }

        protected virtual void Revert()
        {
            this.Body = 264;
            this.Hue = (this.IsParagon && this.DefaultHue == 0) ? Paragon.Hue : this.DefaultHue;
            this.Female = false;
            this.Name = this.DefaultName;
            this.NameHue = -1;
            this.Title = null;
            this.Kills = 0;
            this.HairItemID = 0;
            this.HairHue = 0;
            this.FacialHairItemID = 0;
            this.FacialHairHue = 0;

            this.DeleteClonedItems();

            this.PlaySound(0x511);
            this.FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);
        }

        private void FireEffects(int itemID, int[] offsets)
        {
            for (int i = 0; i < offsets.Length; i += 2)
            {
                Point3D p = this.Location;

                p.X += offsets[i];
                p.Y += offsets[i + 1];

                if (SpellHelper.AdjustField(ref p, this.Map, 12, false))
                    Effects.SendLocationEffect(p, this.Map, itemID, 50);
            }
        }

        private class ClonedItem : Item
        {
            public ClonedItem(Item item)
                : base(item.ItemID)
            {
                this.Name = item.Name;
                this.Weight = item.Weight;
                this.Hue = item.Hue;
                this.Layer = item.Layer;
                this.Movable = false;
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