using System;
using System.Collections.Generic;

namespace Server.Mobiles 
{ 
    public class TavernKeeper : BaseVendor 
    { 
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public TavernKeeper()
            : base("the tavern keeper")
        { 
        }

        public TavernKeeper(Serial serial)
            : base(serial)
        { 
        }

        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo() 
        { 
            this.m_SBInfos.Add(new SBTavernKeeper()); 
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.HalfApron());
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