using Server.Spells;
using Server.Spells.Seventh;
using System;

namespace Server.Items
{
    public class PendantOfKhalAnkur : GargishNecklace
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1158731;  // Pendant of Khal Ankur

        [CommandProperty(AccessLevel.GameMaster)]
        public int ChargeTime { get; set; }

        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set
            {
                m_Charges = value;

                if (m_Charges == 0)
                    StartTimer();

                InvalidateProperties();
            }
        }

        [Constructable]
        public PendantOfKhalAnkur()
            : base(0xA1C9)
        {
            Weight = 1;
            Charges = 1;

            Attributes.BonusHits = 10;
            Attributes.BonusMana = 15;
            Attributes.EnhancePotions = 35;
            Attributes.LowerManaCost = 10;
            ArmorAttributes.MageArmor = 1;

            AttachSocket(new Caddellite());
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Parent != from)
            {
                from.SendLocalizedMessage(1116250); // That must be equipped before you can use it.
            }
            else if (ChargeTime > 0)
            {
                from.SendLocalizedMessage(1074882, ChargeTime.ToString()); // You must wait ~1_val~ seconds for this to recharge.
            }
            else if (SpellHelper.CheckTown(from.Location, from))
            {
                MeteorSwarmSpell spell = new MeteorSwarmSpell(from, null, this);
                spell.Cast();
            }
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (Charges == 0)
                StartTimer();
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (Charges == 0)
                StopTimer();
        }

        public override int BasePhysicalResistance => 15;
        public override int BaseFireResistance => 15;
        public override int BaseColdResistance => 15;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 15;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            list.Add(1158732, Charges.ToString()); // Meteor Breath Charges: ~1_VAL~
            list.Add(1158662); // Caddellite Infused
        }

        public PendantOfKhalAnkur(Serial serial)
            : base(serial)
        {
        }

        private Timer m_Timer;

        public virtual void StartTimer()
        {
            ChargeTime = 300;

            if (m_Timer == null || !m_Timer.Running)
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Slice);
        }

        public virtual void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public virtual void Slice()
        {
            if (ChargeTime > 0)
                ChargeTime--;
            else
            {
                ChargeTime = 0;
                Charges = 1;

                StopTimer();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(m_Charges);
            writer.Write(ChargeTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Charges = reader.ReadInt();
            ChargeTime = reader.ReadInt();

            if (Parent != null && Parent is Mobile && ChargeTime > 0)
                m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), Slice);

            if (version == 0)
            {
                AttachSocket(new Caddellite());
            }
        }
    }
}
