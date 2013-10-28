using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;

namespace Server.Mobiles
{
    public abstract class BaseHealer : BaseVendor
    {
        private static readonly TimeSpan ResurrectDelay = TimeSpan.FromSeconds(2.0);
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        private DateTime m_NextResurrect;
        public BaseHealer()
            : base(null)
        {
            if (!this.IsInvulnerable)
            {
                this.AI = AIType.AI_Mage;
                this.ActiveSpeed = 0.2;
                this.PassiveSpeed = 0.8;
                this.RangePerception = BaseCreature.DefaultRangePerception;
                this.FightMode = FightMode.Aggressor;
            }

            this.SpeechHue = 0;

            this.SetStr(304, 400);
            this.SetDex(102, 150);
            this.SetInt(204, 300);

            this.SetDamage(10, 23);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 75.0, 97.5);
            this.SetSkill(SkillName.EvalInt, 82.0, 100.0);
            this.SetSkill(SkillName.Healing, 75.0, 97.5);
            this.SetSkill(SkillName.Magery, 82.0, 100.0);
            this.SetSkill(SkillName.MagicResist, 82.0, 100.0);
            this.SetSkill(SkillName.Tactics, 82.0, 100.0);

            this.Fame = 1000;
            this.Karma = 10000;

            this.PackItem(new Bandage(Utility.RandomMinMax(5, 10)));
            this.PackItem(new HealPotion());
            this.PackItem(new CurePotion());
        }

        public BaseHealer(Serial serial)
            : base(serial)
        {
        }

        public override bool IsActiveVendor
        {
            get
            {
                return false;
            }
        }
        public override bool IsInvulnerable
        {
            get
            {
                return false;
            }
        }
        public override VendorShoeType ShoeType
        {
            get
            {
                return VendorShoeType.Sandals;
            }
        }
        public virtual bool HealsYoungPlayers
        {
            get
            {
                return true;
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        {
        }

        public virtual int GetRobeColor()
        {
            return Utility.RandomYellowHue();
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Robe(this.GetRobeColor()));
        }

        public virtual bool CheckResurrect(Mobile m)
        {
            return true;
        }

        public virtual void OfferResurrection(Mobile m)
        {
            this.Direction = this.GetDirectionTo(m);

            m.PlaySound(0x1F2);
            m.FixedEffect(0x376A, 10, 16);

            m.CloseGump(typeof(ResurrectGump));
            m.SendGump(new ResurrectGump(m, ResurrectMessage.Healer));
        }

        public virtual void OfferHeal(PlayerMobile m)
        {
            this.Direction = this.GetDirectionTo(m);

            if (m.CheckYoungHealTime())
            {
                this.Say(501229); // You look like you need some healing my child.

                m.PlaySound(0x1F2);
                m.FixedEffect(0x376A, 9, 32);

                m.Hits = m.HitsMax;
            }
            else
            {
                this.Say(501228); // I can do no more for you at this time.
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!m.Frozen && DateTime.UtcNow >= this.m_NextResurrect && this.InRange(m, 4) && !this.InRange(oldLocation, 4) && this.InLOS(m))
            {
                if (!m.Alive)
                {
                    this.m_NextResurrect = DateTime.UtcNow + ResurrectDelay;

                    if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
                    {
                        m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                    }
                    else if (this.CheckResurrect(m))
                    {
                        this.OfferResurrection(m);
                    }
                }
                else if (this.HealsYoungPlayers && m.Hits < m.HitsMax && m is PlayerMobile && ((PlayerMobile)m).Young)
                {
                    this.OfferHeal((PlayerMobile)m);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            if (!this.IsInvulnerable)
            {
                this.AI = AIType.AI_Mage;
                this.ActiveSpeed = 0.2;
                this.PassiveSpeed = 0.8;
                this.RangePerception = BaseCreature.DefaultRangePerception;
                this.FightMode = FightMode.Aggressor;
            }
        }
    }
}