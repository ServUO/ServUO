using System;
using System.Collections.Generic;

namespace Server.Mobiles 
{ 
    public class Shipwright : BaseVendor 
    { 
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Shipwright()
            : base("the shipwright")
        { 
            this.SetSkill(SkillName.Carpentry, 60.0, 83.0);
            this.SetSkill(SkillName.Macing, 36.0, 68.0);
        }

        public Shipwright(Serial serial)
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
            this.m_SBInfos.Add(new SBShipwright()); 
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.SmithHammer());
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