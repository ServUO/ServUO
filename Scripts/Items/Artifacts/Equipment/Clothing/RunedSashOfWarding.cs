using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Engines.Craft;

namespace Server.Items
{
    public enum WardingEffect
    {
        WeaponDamage,
        SpellDamage
    }

    [Alterable(typeof(DefTailoring), typeof(GargishRunedSashOfWarding))]
    public class RunedSashOfWarding : BodySash
    {
        public override bool IsArtifact { get { return true; } }
        public static Dictionary<Mobile, WardingEffect> Table { get { return m_Table; } }
        private static Dictionary<Mobile, WardingEffect> m_Table = new Dictionary<Mobile, WardingEffect>();

        public override int LabelNumber { get { return 1116231; } }

        private int m_Charges;
        private WardingEffect m_Ward;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return m_Charges; } set { m_Charges = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public WardingEffect Ward { get { return m_Ward; } set { m_Ward = value; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public RunedSashOfWarding()
        {
            Hue = 1157;
            m_Charges = 50;

            if (Utility.RandomBool())
                m_Ward = WardingEffect.WeaponDamage;
            else
                m_Ward = WardingEffect.SpellDamage;

            Attributes.Brittle = 1;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Charges <= 0)
                from.SendLocalizedMessage(501259); //This magic item is out of charges.
            if (IsUnderEffects(from, m_Ward))
                from.SendLocalizedMessage(502173); //You are already under a similar effect.
            else if (from.FindItemOnLayer(Layer.MiddleTorso) == this)
            {
                Charges--;

                m_Table[from] = m_Ward;
                Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerStateCallback(RemoveEffects), from);

                from.FixedParticles(14154, 1, 30, 9964, 3, 3, EffectLayer.Waist);

                for (int i = -1; i <= 1; i++)
                {
                    IEntity entity = new Entity(Serial.Zero, new Point3D(from.X + i, from.Y, from.Z), from.Map);
                    IEntity to = new Entity(Serial.Zero, new Point3D(from.X + i, from.Y, from.Z + 50), from.Map);
                    Effects.SendMovingParticles(entity, to, 7956, 1, 0, false, false, 33, 3, 9501, 1, 0, EffectLayer.Head, 0x100);
                }

                from.PlaySound(0x5C1);
                from.SendLocalizedMessage(1116243); //The runes glow and a magical warding forms around your body.

                BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.Warding, 1151395, 1151396, m_Ward == WardingEffect.WeaponDamage ? 1116172 : 1116173));
            }
            else
            {
                from.SendLocalizedMessage(1112886); //You must be wearing this item in order to use it.
            }
        }


        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060741, m_Charges.ToString());

            switch (m_Ward)
            {
                case WardingEffect.WeaponDamage:
                    list.Add(1116172);
                    break;
                case WardingEffect.SpellDamage:
                    list.Add(1116173);
                    break;
            }

            list.Add(1060639, "{0}\t{1}", this.HitPoints, this.MaxHitPoints); // durability ~1_val~ / ~2_val~
        }

        public static bool IsUnderEffects(Mobile from, WardingEffect type)
        {
            return m_Table.ContainsKey(from) && m_Table[from] == type;
        }

        public static void RemoveEffects(object obj)
        {
            Mobile from = (Mobile)obj;

            if (m_Table.ContainsKey(from))
                m_Table.Remove(from);

            from.SendLocalizedMessage(1116244); //The magical ward around you dissipates.

            BuffInfo.RemoveBuff(from, BuffIcon.Warding);
        }

        public RunedSashOfWarding(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Charges);
            writer.Write((int)m_Ward);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Charges = reader.ReadInt();
            m_Ward = (WardingEffect)reader.ReadInt();
        }
    }

    public class GargishRunedSashOfWarding : RunedSashOfWarding
    {
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishRunedSashOfWarding()
        {
            ItemID = 0x46B4;
        }

        public GargishRunedSashOfWarding(Serial serial)
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