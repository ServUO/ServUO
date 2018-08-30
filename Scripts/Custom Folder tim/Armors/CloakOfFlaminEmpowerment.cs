using System;

namespace Server.Items
{
    [Flipable(0x2B04, 0x2B05)]
    public class CloakOfEmpowerment : BaseCloak
    {
        private OnEquipTimer m_Timer;

        public override int BasePhysicalResistance { get { return 20; } }
        public override int BaseFireResistance { get { return -14; } }
        public override int BaseColdResistance { get { return 17; } }
        public override int BasePoisonResistance { get { return 12; } }
        public override int BaseEnergyResistance { get { return 19; } }
        [Constructable]
        public CloakOfEmpowerment()
            : base(0x2B04)
        {
            Name = "Cloak of flaming Empowerment";
            Weight = 4.0;
            Hue = 1161;
            Attributes.EnhancePotions = 20;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 55;
            Attributes.BonusDex = 25;
            Attributes.CastRecovery = 6;
            Attributes.CastSpeed = 3;
        }

        public override bool OnEquip(Mobile from)
        {
            this.Movable = false;
            return Validate(from) && base.OnEquip(from);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Validate(Parent as Mobile))
                base.OnSingleClick(from);
        }

        public bool Validate(Mobile m)
        {
            if (m == null || !m.Player)
                return true;
            {
                m_Timer = new OnEquipTimer(this, m);
                m_Timer.Start();
                m.SendMessage("The great power of the cloak binds on you");
                m.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                m.PlaySound(0x205);
            }

            return true;
        }

        public CloakOfEmpowerment(Serial serial)
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

        private class OnEquipTimer : Timer
        {
            private Item m_Item;
            private Mobile m;

            public OnEquipTimer(Item item, Mobile from)
                : base(TimeSpan.FromSeconds(Utility.RandomMinMax(60, 80)))
            {
                Priority = TimerPriority.FiftyMS;

                m_Item = item;
                m = from;
            }

            protected override void OnTick()
            {
                if (m_Item.Deleted)
                    return;

                Map map = m_Item.Map;
                if (map == null)
                    return;

                m.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                m.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
                Effects.PlaySound(m_Item.Location, m_Item.Map, 0x307);
                m.PlaySound(0x208);
                AOS.Damage(m, 100, 0, 100, 0, 0, 0);
                m.SendMessage("The cloak destroyed itself after charging to much magical power");
                m_Item.Delete();
            }
        }
    }
}