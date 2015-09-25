using Server;
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Despise
{
	public class DespiseAnkh : BaseAddon
	{
		private Alignment m_Alignment;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Alignment Alignment { get { return m_Alignment; } set { m_Alignment = value; } }
		
		public DespiseAnkh(Alignment alignment)
		{
			m_Alignment = alignment;

            switch (alignment)
            {
                default:
                case Alignment.Good:
                    AddComponent(new AddonComponent(4), 0, 0, 0);
                    AddComponent(new AddonComponent(5), +1, 0, 0);
                    break;
                case Alignment.Evil:
                    AddComponent(new AddonComponent(2), 0, 0, 0);
                    AddComponent(new AddonComponent(3), 0, -1, 0);
                    break;
            }
		}

        public override void OnComponentUsed(AddonComponent c, Mobile from)
		{
			if(from.InRange(c.Location, 3) && from.Backpack != null)
			{
				foreach(WispOrb orb in WispOrb.Orbs)
				{
					if(orb.Owner == from)
					{
						LabelTo(from, 1153357); // Thou can guide but one of us.
						return;
					}
				}
				
				Alignment alignment = Alignment.Neutral;
				
				if(from.Karma > 0 && m_Alignment == Alignment.Good)
					alignment = Alignment.Good;
				else if (from.Karma < 0 && m_Alignment == Alignment.Evil)
					alignment = Alignment.Evil;
					
				if(alignment != Alignment.Neutral)
				{
                    WispOrb orb = new WispOrb(from, alignment);
					from.Backpack.DropItem(orb);

                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), new TimerStateCallback(SendMessage_Callback), new object[] { orb, from } );
				}
				else
					LabelTo(from, 1153350); // Thy spirit be not compatible with our goals!
			}
		}

        private void SendMessage_Callback(object o)
        {
            object[] obj = o as object[];

            WispOrb orb = obj[0] as WispOrb;
            Mobile from = obj[1] as Mobile;

            if(orb != null && orb.IsChildOf(from.Backpack))
                orb.LabelTo(from, 1153355); // I will follow thy guidance.
        }
		
		public DespiseAnkh(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			writer.Write((int)m_Alignment);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			m_Alignment = (Alignment)reader.ReadInt();
		}
	}
}