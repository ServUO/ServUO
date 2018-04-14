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
            if (Caster.Mounted)
            {
                Caster.SendLocalizedMessage(1063132); // You cannot use this ability while mounted.
                return false;
            }
            else if ((Caster.Followers + 1) > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1063133); // You cannot summon a mirror image because you have too many followers.
                return false;
            }
            else if (TransformationSpellHelper.UnderTransformation(Caster, typeof(HorrificBeastSpell)))
            {
                Caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
                return false;
            }
            else if (Caster.Flying)
            {
                Caster.SendLocalizedMessage(1113415); // You cannot use this ability while flying.
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

            Caster.SendLocalizedMessage(1063134); // You begin to summon a mirror image of yourself.
        }

        public override void OnCast()
        {
            if (Caster.Mounted)
            {
                Caster.SendLocalizedMessage(1063132); // You cannot use this ability while mounted.
            }
            else if ((Caster.Followers + 1) > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1063133); // You cannot summon a mirror image because you have too many followers.
            }
            else if (TransformationSpellHelper.UnderTransformation(Caster, typeof(HorrificBeastSpell)))
            {
                Caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
            }
            else if (CheckSequence())
            {
                Caster.FixedParticles(0x376A, 1, 14, 0x13B5, EffectLayer.Waist);
                Caster.PlaySound(0x511);

                new Clone(Caster).MoveToWorld(Caster.Location, Caster.Map);
            }

            FinishSequence();
        }

        public static Clone GetDeflect(Mobile attacker, Mobile defender)
        {
            Clone clone = null;

            if (HasClone(defender) && (defender.Skills.Ninjitsu.Value / 150.0) > Utility.RandomDouble())
            {
                IPooledEnumerable eable = defender.GetMobilesInRange(4);

                foreach (Mobile m in eable)
                {
                    clone = m as Clone;

                    if (clone != null && clone.Summoned && clone.SummonMaster == defender)
                    {
                        attacker.SendLocalizedMessage(1063141); // Your attack has been diverted to a nearby mirror image of your target!
                        defender.SendLocalizedMessage(1063140); // You manage to divert the attack onto one of your nearby mirror images.
                        break;
                    }
                }

                eable.Free();
            }

            return clone;
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
            m_Caster = caster;

            Body = caster.Body;

            Hue = caster.Hue;
            Female = caster.Female;

            Name = caster.Name;
            NameHue = caster.NameHue;

            Title = caster.Title;
            Kills = caster.Kills;

            HairItemID = caster.HairItemID;
            HairHue = caster.HairHue;

            FacialHairItemID = caster.FacialHairItemID;
            FacialHairHue = caster.FacialHairHue;

            for (int i = 0; i < caster.Skills.Length; ++i)
            {
                Skills[i].Base = caster.Skills[i].Base;
                Skills[i].Cap = caster.Skills[i].Cap;
            }

            for (int i = 0; i < caster.Items.Count; i++)
            {
                AddItem(CloneItem(caster.Items[i]));
            }

            Warmode = true;

            Summoned = true;
            SummonMaster = caster;

            ControlOrder = OrderType.Follow;
            ControlTarget = caster;

            TimeSpan duration = TimeSpan.FromSeconds(30 + caster.Skills.Ninjitsu.Fixed / 40);

            new UnsummonTimer(caster, this, duration).Start();
            SummonEnd = DateTime.UtcNow + duration;

            MirrorImage.AddClone(m_Caster);

            IgnoreMobiles = true;
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

        public override bool CanDetectHidden { get { return false; } }

        public override bool IsHumanInTown()
        {
            return false;
        }

        public override bool OnMoveOver(Mobile m)
        {
            return true;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            Delete();
        }

        public override void OnDelete()
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 15, 5042);

            base.OnDelete();
        }

        public override void OnAfterDelete()
        {
            MirrorImage.RemoveClone(m_Caster);
            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_Caster);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Caster = reader.ReadMobile();

            MirrorImage.AddClone(m_Caster);
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

        public override bool Think()
        {
            // Clones only follow their owners
            Mobile master = m_Mobile.SummonMaster;

            if (master != null && master.Map == m_Mobile.Map && master.InRange(m_Mobile, m_Mobile.RangePerception))
            {
                int iCurrDist = (int)m_Mobile.GetDistanceToSqrt(master);
                bool bRun = (iCurrDist > 5);

                WalkMobileRange(master, 2, bRun, 0, 1);
            }
            else
                WalkRandom(2, 2, 1);

            return true;
        }
    }
}