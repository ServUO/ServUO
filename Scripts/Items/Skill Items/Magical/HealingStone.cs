using System;

namespace Server.Items
{
    public class HealingStone : Item
    {
        private readonly Mobile m_Caster;
        private readonly int m_Amount;
        [Constructable]
        public HealingStone(Mobile caster, int amount)
            : base(0x4078)
        {
            this.m_Caster = caster;
            this.m_Amount = amount;
        }

        public HealingStone(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(502138); // That is too far away for you to use
                return;
            }
            else if (from != this.m_Caster)
            {
                // from.SendLocalizedMessage( ); // 
                return;
            }

            BaseWeapon weapon = from.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;

            if (weapon == null)
                weapon = from.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            if (weapon != null)
            {
                from.SendLocalizedMessage(1080116); // You must have a free hand to use a Healing Stone.
            }
            else if (from.BeginAction(typeof(BaseHealPotion)))
            {
                from.Heal(Utility.RandomMinMax(BasePotion.Scale(from, 13), BasePotion.Scale(from, 16)));
                this.Consume();
                Timer.DelayCall(TimeSpan.FromSeconds(8.0), new TimerStateCallback(ReleaseHealLock), from);
            }
            else
                from.SendLocalizedMessage(1095172); // You must wait a few seconds before using another Healing Stone.
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            this.Delete();
            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private static void ReleaseHealLock(object state)
        {
            ((Mobile)state).EndAction(typeof(BaseHealPotion));
        }
    }
}