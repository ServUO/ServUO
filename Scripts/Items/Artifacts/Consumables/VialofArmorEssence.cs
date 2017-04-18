using System;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public class VialofArmorEssence : Item
    {
        public static Dictionary<BaseCreature, DateTime> m_Table = new Dictionary<BaseCreature, DateTime>();

        public virtual int Bonus { get { return 10; } }
        public virtual TimeSpan Duration { get { return TimeSpan.FromMinutes(10); } }
        public virtual TimeSpan CoolDown { get { return TimeSpan.FromMinutes(120); } }

        [Constructable]
        public VialofArmorEssence()
            : base(0x5722)
        {
        }

        public VialofArmorEssence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113018;
            }
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            TryFeed(from, target);

            return false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.BeginTarget(2, false, TargetFlags.Beneficial, (from, targeted) =>
                {
                    if (targeted is Mobile)
                    {
                        TryFeed(from, (Mobile)targeted);
                    }
                });
            }
        }

        private void TryFeed(Mobile from, Mobile target)
        {
            if (target is BaseCreature && !((BaseCreature)target).IsDeadBondedPet)
            {
                BaseCreature bc = (BaseCreature)target;

                if (UnderInfluence(bc))
                {
                    if (m_Table[bc] + (CoolDown + Duration) < DateTime.Now)
                    {
                        from.SendLocalizedMessage(1113076); //Your pet is still recovering from the last armor essence it consumed.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1113075); //Your pet is still under the effect of armor essence.
                    }
                }
                else if (bc.ControlMaster == from)
                {
                    from.SendLocalizedMessage(1113050); //Your pet looks much happier.
                    DoEffects(bc);
                }
            }
        }

        public bool DoEffects(BaseCreature bc)
        {
            bc.PlaySound(0x1EA);
            bc.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);

            m_Table.Add(bc, DateTime.Now);
            Timer.DelayCall(Duration + CoolDown, new TimerStateCallback(RemoveInfluence), bc);

            bc.TempDamageAbsorb = Bonus;
            bc.Loyalty = BaseCreature.MaxLoyalty;

            Consume();
            return true;
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1113213); //* For Pets Only *
            list.Add(1113219); //Stats Increased by 10%

            list.Add(1113212, Duration.TotalMinutes.ToString()); //Duration: ~1_val~ minutes
            list.Add(1113218, CoolDown.TotalMinutes.ToString()); //Cooldown: ~1_val~ minutes
        }

        public static bool UnderInfluence(BaseCreature bc)
        {
            return m_Table.ContainsKey(bc);
        }

        public static void RemoveInfluence(object obj)
        {
            BaseCreature bc = (BaseCreature)obj;

            if (m_Table.ContainsKey(bc))
                m_Table.Remove(bc);

            bc.TempDamageAbsorb = 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                reader.ReadBool();
        }
    }
}