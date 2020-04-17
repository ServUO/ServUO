using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class TastyTreat : Item
    {
        public static Dictionary<BaseCreature, DateTime> m_Table = new Dictionary<BaseCreature, DateTime>();

        public override int LabelNumber => 1112774;

        public virtual double Bonus => 0.05;
        public virtual TimeSpan Duration => TimeSpan.FromMinutes(20);
        public virtual TimeSpan CoolDown => TimeSpan.FromMinutes(2);
        public virtual int DamageBonus => 0;

        [Constructable]
        public TastyTreat()
            : this(1)
        {
        }

        [Constructable]
        public TastyTreat(int amount) : base(2424)
        {
            Stackable = true;
            Amount = amount;
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1113213); //* For Pets Only *

            if (Bonus == 0.10)
                list.Add(1113215); //Stats Increased by 10%
            else if (Bonus == 0.15)
                list.Add(1113216); //Stats Increased by 15%
            else
                list.Add(1113214); //Stats Increased by 5%

            list.Add(1113212, Duration.TotalMinutes.ToString()); //Duration: ~1_val~ minutes
            list.Add(1113218, CoolDown.TotalMinutes.ToString()); //Cooldown: ~1_val~ minutes
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
                    if (m_Table[bc] + (CoolDown + Duration) < DateTime.UtcNow)
                    {
                        from.SendLocalizedMessage(1113049); //Your pet is still recovering from the last tasty treat.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1113051); //Your pet is still enjoying the last tasty treat!
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
            string modName = Serial.ToString();

            bc.AddStatMod(new StatMod(StatType.Str, bc.Serial + "Str", (int)(bc.RawStr * Bonus), Duration));
            bc.AddStatMod(new StatMod(StatType.Int, bc.Serial + "Int", (int)(bc.RawInt * Bonus), Duration));
            bc.AddStatMod(new StatMod(StatType.Dex, bc.Serial + "Dex", (int)(bc.RawDex * Bonus), Duration));

            bc.PlaySound(0x1EA);
            bc.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);

            bc.TempDamageBonus = DamageBonus;
            bc.Loyalty = BaseCreature.MaxLoyalty;

            m_Table.Add(bc, DateTime.UtcNow);
            Timer.DelayCall(Duration + CoolDown, new TimerStateCallback(RemoveInfluence), bc);

            Consume();
            return true;
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

            bc.TempDamageBonus = 0;
        }

        public TastyTreat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                m_InheritsItem = true;

                reader.ReadBool();
            }
        }

        #region Old Item Serialization Vars
        /* DO NOT USE! Only used in serialization of tasty treats that originally derived from Item */
        private bool m_InheritsItem;

        protected bool InheritsItem => m_InheritsItem;
        #endregion
    }
}