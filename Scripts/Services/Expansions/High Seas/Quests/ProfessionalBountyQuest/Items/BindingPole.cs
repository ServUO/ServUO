using Server;
using System;
using Server.Engines.Quests;
using Server.Multis;

namespace Server.Items
{
    public class BindingPole : Item
	{
		private BaseQuest m_Quest;
        private BaseGalleon m_Galleon;

		public BaseQuest Quest 
        { 
            get { return m_Quest; } 
            set { m_Quest = value; } 
        }

        public BaseGalleon Galleon
        {
            get { return m_Galleon; }
            set { m_Galleon = value; }
        }

        public override int LabelNumber { get { return 1116718; } }

		public override void Delete()
		{
            base.Delete();

			if (m_Quest != null)
				m_Quest.OnResign(false);

            if (m_Galleon != null)
                m_Galleon.Pole = null;
		}

		public BindingPole(BaseQuest quest) : base(5696)
		{
			m_Quest = quest;
			Movable = false;
		}

		public BindingPole(Serial serial) : base(serial)
		{
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
	}
}