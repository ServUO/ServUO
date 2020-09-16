using Server.Engines.Craft;
using System;

namespace Server.Items
{
    [Flipable(0x457E, 0x457F)]
    public class GargishLeatherWingArmor : BaseArmor, IRangeDamage
    {
        private AosElementAttributes m_AosElementDamages;

        [CommandProperty(AccessLevel.GameMaster)]
        public AosElementAttributes AosElementDamages { get { return m_AosElementDamages; } set { } }

        [Constructable]
        public GargishLeatherWingArmor()
            : base(0x457E)
        {
            Weight = 2.0;
            Layer = Layer.Cloak;

            m_AosElementDamages = new AosElementAttributes(this);
        }

        public GargishLeatherWingArmor(Serial serial)
            : base(serial)
        {
        }

        public override int PhysicalResistance => 0;
        public override int FireResistance => 0;
        public override int ColdResistance => 0;
        public override int PoisonResistance => 0;
        public override int EnergyResistance => 0;

        public override int StrReq => 10;
        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;
        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;

        public override int GetLuckBonus() { return 0; }

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);

            PlayerConstructed = true;

            CraftContext context = craftSystem.GetContext(from);

            Hue = CraftResources.GetHue(Resource);

            return quality;
        }

        public virtual void AlterRangedDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct)
        {
            fire = m_AosElementDamages.Fire;
            cold = m_AosElementDamages.Cold;
            pois = m_AosElementDamages.Poison;
            nrgy = m_AosElementDamages.Energy;
            chaos = m_AosElementDamages.Chaos;
            direct = m_AosElementDamages.Direct;

            phys = 100 - fire - cold - pois - nrgy - chaos - direct;
        }

        public override void AddDamageTypeProperty(ObjectPropertyList list)
        {
            int phys, fire, cold, pois, nrgy, chaos, direct;
            phys = fire = cold = pois = nrgy = chaos = direct = 0;

            AlterRangedDamage(ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct);

            if (phys != 0 && phys != 100)
                list.Add(1060403, phys.ToString()); // physical damage ~1_val~%

            if (fire != 0)
                list.Add(1060405, fire.ToString()); // fire damage ~1_val~%

            if (cold != 0)
                list.Add(1060404, cold.ToString()); // cold damage ~1_val~%

            if (pois != 0)
                list.Add(1060406, pois.ToString()); // poison damage ~1_val~%

            if (nrgy != 0)
                list.Add(1060407, nrgy.ToString()); // energy damage ~1_val

            if (chaos != 0)
                list.Add(1072846, chaos.ToString()); // chaos damage ~1_val~%

            if (direct != 0)
                list.Add(1079978, direct.ToString()); // Direct Damage: ~1_PERCENT~%
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            if (m_AosElementDamages.IsEmpty)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(1);
                m_AosElementDamages.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        if (reader.ReadInt() == 1)
                        {
                            m_AosElementDamages = new AosElementAttributes(this, reader);
                        }
                        else
                        {
                            m_AosElementDamages = new AosElementAttributes(this);
                        }
                    }
                    break;
                case 0:
                    {
                        m_AosElementDamages = new AosElementAttributes(this);
                    }
                    break;
            }
        }
    }
}
