using System;
using System.Collections.Generic;
using Server;

namespace Server.Items
{
    public class StoneSkinLotion : BalmOrLotion
    {
        public override int LabelNumber { get { return 1094944; } } // Stone Skin Lotion
        public override int ApplyMessage { get { return 1095143; } } // You apply the ointment and suddenly feel less vulnerable!

        public override void AddBuff(Mobile m, TimeSpan duration)
        {
            ResistanceMod[] mods = new ResistanceMod[]
                {
                    new ResistanceMod( ResistanceType.Physical, 30 ),
                    new ResistanceMod( ResistanceType.Fire, -5 ),
                    new ResistanceMod( ResistanceType.Cold, -5 )
                };

            for (int i = 0; i < mods.Length; ++i)
                m.AddResistanceMod(mods[i]);

            m_Table[m] = mods;
        }

        public override void RemoveBuff(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                ResistanceMod[] mods = m_Table[m];

                for (int i = 0; i < mods.Length; ++i)
                    m.RemoveResistanceMod(mods[i]);
            }
        }

        private static Dictionary<Mobile, ResistanceMod[]> m_Table = new Dictionary<Mobile, ResistanceMod[]>();

        [Constructable]
        public StoneSkinLotion()
            : base(0xEFC)
        {
            Hue = 0x487;

            // TODO: Fix Hue, ItemID
        }

        public StoneSkinLotion(Serial serial)
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