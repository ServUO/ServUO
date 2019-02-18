using System;
using System.Collections.Generic;

using Server;
using Server.Prompts;
using Server.Mobiles;
using Server.Items;
using Server.SkillHandlers;
using Server.Network;
using Server.Engines.Quests;

namespace Server.Engines.Khaldun
{
    public class RolledParchment : Item
    {
        public override int LabelNumber { get { return 1158578; } } // rolled parchment

        public int Page { get; set; }

        public RolledParchment(int page)
            : base(0x2831)
        {
            Page = page;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(Page);
        }

        public RolledParchment(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(Page);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            Page = reader.ReadInt();
        }
    }
}
