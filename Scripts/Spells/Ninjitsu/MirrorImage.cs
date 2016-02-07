using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;

namespace Server.Spells.Ninjitsu
{
    public class MirrorImage : NinjaSpell
    {
        private static readonly Dictionary<Mobile, int> m_CloneCount = new Dictionary<Mobile, int>();
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Mirror Image", null,
            -1,
            9002);
        public MirrorImage(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return Core.ML ? 20.0 : 40.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }
        public override bool BlockedByAnimalForm
        {
            get
            {
                return false;
            }
        }
        public static bool HasClone(Mobile m)
        {
            return m_CloneCount.ContainsKey(m);
        }

        public static void AddClone(Mobile m)
        {
            if (m == null)
                return;

            if (m_CloneCount.ContainsKey(m))
                m_CloneCount[m]++;
            else
                m_CloneCount[m] = 1;
        }

        public static void RemoveClone(Mobile m)
        {
            if (m == null)
                return;

            if (m_CloneCount.ContainsKey(m))
            {
                m_CloneCount[m]--;

                if (m_CloneCount[m] == 0)
                    m_CloneCount.Remove(m);
            }
        }

        public override bool CheckCast()
        {
            if (this.Caster.Mounted)
            {
                this.Caster.SendLocalizedMessage(1063132); // You cannot use this ability while mounted.
                return false;
            }
            else if ((this.Caster.Followers + 1) > this.Caster.FollowersMax)
            {
                this.Caster.SendLocalizedMessage(1063133); // You cannot summon a mirror image because you have too many followers.
                return false;
            }
            else if (TransformationSpellHelper.UnderTransformation(this.Caster, typeof(HorrificBeastSpell)))
            {
                this.Caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
                return false;
            }

            return base.CheckCast();
        }

        public override bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
        {
            return false;
        }

        public override void OnBeginCast()
        {
            base.OnBeginCast();

            this.Caster.SendLocalizedMessage(1063134); // You begin to summon a mirror image of yourself.
        }

        public override void OnCast()
        {
            if (this.Caster.Mounted)
            {
                this.Caster.SendLocalizedMessage(1063132); // You cannot use this ability while mounted.
            }
            else if ((this.Caster.Followers + 1) > this.Caster.FollowersMax)
            {
                this.Caster.SendLocalizedMessage(1063133); // You cannot summon a mirror image because you have too many followers.
            }
            else if (TransformationSpellHelper.UnderTransformation(this.Caster, typeof(HorrificBeastSpell)))
            {
                this.Caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
            }
            else if (this.CheckSequence())
            {
                this.Caster.FixedParticles(0x376A, 1, 14, 0x13B5, EffectLayer.Waist);
                this.Caster.PlaySound(0x511);

                new Clone(this.Caster).MoveToWorld(this.Caster.Location, this.Caster.Map);
            }

            this.FinishSequence();
        }
    }
}

namespace Server.Mobiles
{
    public class Clone : BaseCreature
    {
        private Mobile m_Caster;
        public Clone(Mobile caster)
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            this.m_Caster = caster;

            this.Body = caster.Body;

            this.Hue = caster.Hue;
            this.Female = caster.Female;

            this.Name = caster.Name;
            this.NameHue = caster.NameHue;

            this.Title = caster.Title;
            this.Kills = caster.Kills;

            this.HairItemID = caster.HairItemID;
            this.HairHue = caster.HairHue;

            this.FacialHairItemID = caster.FacialHairItemID;
            this.FacialHairHue = caster.FacialHairHue;

            for (int i = 0; i < caster.Skills.Length; ++i)
            {
                this.Skills[i].Base = caster.Skills[i].Base;
                this.Skills[i].Cap = caster.Skills[i].Cap;
            }

            for (int i = 0; i < caster.Items.Count; i++)
            {
                this.AddItem(this.CloneItem(caster.Items[i]));
            }

            this.Warmode = true;

            this.Summoned = true;
            this.SummonMaster = caster;

            this.ControlOrder = OrderType.Follow;
            this.ControlTarget = caster;

            TimeSpan duration = TimeSpan.FromSeconds(30 + caster.Skills.Ninjitsu.Fixed / 40);

            new UnsummonTimer(caster, this, duration).Start();
            this.SummonEnd = DateTime.UtcNow + duration;

            MirrorImage.AddClone(this.m_Caster);
        }

        public Clone(Serial serial)
            : base(serial)
        {
        }

        public override bool DeleteCorpseOnDeath
        {
            get
            {
                return true;
            }
        }
        public override bool IsDispellable
        {
            get
            {
                return false;
            }
        }
        public override bool Commandable
        {
            get
            {
                return false;
            }
        }
        protected override BaseAI ForcedAI
        {
            get
            {
                return new CloneAI(this);
            }
        }
        public override bool IsHumanInTown()
        {
            return false;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            this.Delete();
        }

        public override void OnDelete()
        {
            Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 10, 15, 5042);

            base.OnDelete();
        }

        public override void OnAfterDelete()
        {
            MirrorImage.RemoveClone(this.m_Caster);
            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_Caster);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Caster = reader.ReadMobile();

            MirrorImage.AddClone(this.m_Caster);
        }

        private Item CloneItem(Item item)
        {
            Item newItem = new Item(item.ItemID);
            newItem.Hue = item.Hue;
            newItem.Layer = item.Layer;

            return newItem;
        }
    }
}

namespace Server.Mobiles
{
    public class CloneAI : BaseAI
    {
        public CloneAI(Clone m)
            : base(m)
        {
            m.CurrentSpeed = m.ActiveSpeed;
        }

        public override bool CanDetectHidden
        {
            get
            {
                return false;
            }
        }
        public override bool Think()
        {
            // Clones only follow their owners
            Mobile master = this.m_Mobile.SummonMaster;

            if (master != null && master.Map == this.m_Mobile.Map && master.InRange(this.m_Mobile, this.m_Mobile.RangePerception))
            {
                int iCurrDist = (int)this.m_Mobile.GetDistanceToSqrt(master);
                bool bRun = (iCurrDist > 5);

                this.WalkMobileRange(master, 2, bRun, 0, 1);
            }
            else
                this.WalkRandom(2, 2, 1);

            return true;
        }
    }
}