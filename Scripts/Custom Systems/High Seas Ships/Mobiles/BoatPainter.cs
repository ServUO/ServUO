using System;
using System.Collections.Generic;

namespace Server.Mobiles 
{ 
    public class BoatPainter : BaseVendor 
    { 
        private readonly List<SBInfo> _sBInfos = new List<SBInfo>();
        [Constructable]
        public BoatPainter()
            : base("the Boat Painter")
        { 
            SetSkill(SkillName.Carpentry, 60.0, 83.0);
            SetSkill(SkillName.Macing, 36.0, 68.0);
        }

        public BoatPainter(Serial serial)
            : base(serial)
        { 
        }

        protected override List<SBInfo> SBInfos
        {
            get
            {
                return _sBInfos;
            }
        }
        public override void InitSBInfo() 
        { 
            _sBInfos.Add(new SBBoatPainter()); 
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new Server.Items.SmithHammer());
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