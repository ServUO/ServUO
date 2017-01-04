using Server;
using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Multis;

namespace Server.Items
{
    public class RuinedShipPlans : Item
	{
		public enum PlanType
		{
			One = 1,
			Two,
			Three,
			Four,
			Five,
			Six,
			Seven,
			Eight
		}

		private PlanType m_PlanType;
		public PlanType Type { get { return m_PlanType; } set { m_PlanType = value; } }

        private List<PlanType> m_Joined = new List<PlanType>();
        public List<PlanType> Joined { get { return m_Joined; } }

        public override int LabelNumber { get { return 1116784; } }

		[Constructable]
		public RuinedShipPlans() : this((PlanType)Utility.RandomMinMax(1, 8))
        {
        }
	
		[Constructable]
		public RuinedShipPlans(PlanType type) : base(5360)
		{
			m_PlanType = type;
            m_Joined.Add(type);
		}

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1116747); //Orc Ship

            if(m_Joined.Count == 1)
                list.Add(1116776, String.Format("{0}\t{1}", ((int)m_PlanType).ToString(), "8")); //Part ~1_val~ of ~2_val~
            else
                list.Add(1116777, String.Format("{0}\t{1}", m_Joined.Count.ToString(), "8")); //Parts ~1_val~ of ~2_val~

            if (m_Joined.Count > 1)
            {
                m_Joined.Sort();

                string str = "Contains Parts ";
                foreach (int i in m_Joined)
                    str += (i.ToString() + ", ");

                list.Add(1114057, str);
            }
        }

        public bool TryCombine(Mobile from, RuinedShipPlans plans)
        {
            if (m_Joined.Contains(plans.Type))
            {
                from.SendLocalizedMessage(1116787); //This part is already attached.
                return false;
            }

            plans.Delete();
            from.PlaySound(0x249);

            foreach (PlanType type in plans.Joined)
            {
                if(!m_Joined.Contains(type))
                    m_Joined.Add(type);
            }

            InvalidateProperties();

            if (m_Joined.Count == 8)
            {
                from.AddToBackpack(new OrcishGalleonDeed());
                from.SendLocalizedMessage(1116788); //You have completed a deed for an Orc Ship!

                this.Delete();
            }
            else
                from.Target = new InternalTarget(this);
            return true;
        }

		public override void OnDoubleClick(Mobile from)
		{
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
                from.SendLocalizedMessage(1116785); //Target another piece of ship plans to combine.
            }
		}

        private class InternalTarget : Target
        {
            private RuinedShipPlans m_Plans;

            public InternalTarget(RuinedShipPlans plans) : base(-1, false, TargetFlags.None)
            {
                m_Plans = plans;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Item && !((Item)targeted).IsChildOf(from.Backpack))
                    from.SendMessage("That must be in your pack to combine.");
                else if (targeted is RuinedShipPlans)
                {
                    RuinedShipPlans toAttach = (RuinedShipPlans)targeted;
                    m_Plans.TryCombine(from, toAttach);
                }
                else
                    from.SendLocalizedMessage(1116786); //These do not fit together.
            }
        }
		
		public RuinedShipPlans(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{	
			base.Serialize(writer);
			writer.Write((int)0);	
			writer.Write((int)m_PlanType);

            writer.Write(m_Joined.Count);
            for (int i = 0; i < m_Joined.Count; i++)
                writer.Write((int)m_Joined[i]);
		}

        public override void Deserialize(GenericReader reader)
		{
		 	base.Deserialize(reader);
			int version = reader.ReadInt();
			m_PlanType = (PlanType)reader.ReadInt();

            int count = reader.ReadInt();
            for(int i = 0; i < count; i++)
                m_Joined.Add((PlanType)reader.ReadInt());
		}
    }
}