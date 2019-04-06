using System;
using Server;

namespace Server.Items
{
    public class SmugglersToolBox : Item
    {
        private int _UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining { get { return _UsesRemaining; } set { _UsesRemaining = value; InvalidateProperties(); } }

        public DateTime NextRecharge { get; set; }

        public override int LabelNumber { get { return 1071520; } } // Smuggler's Tool Box

        [Constructable] 
        public SmugglersToolBox()
           : base(0x1EB6)
        {
            Hue = 953;
            UsesRemaining = 10;

            NextRecharge = DateTime.UtcNow;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack) && _UsesRemaining > 0)
            {
                var lockpick = new Lockpick(Utility.RandomMinMax(5, 12));

                if (m.Backpack == null || !m.Backpack.TryDropItem(m, lockpick, false))
                {
                    m.SendLocalizedMessage(1077971); // Make room in your backpack first!
                    lockpick.Delete();
                }
                else
                {
                    m.SendLocalizedMessage(1071526); // You take some lockpicks from the tool box.
                    UsesRemaining--;
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060584, _UsesRemaining.ToString());
        }

        public SmugglersToolBox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(_UsesRemaining);
            writer.Write(NextRecharge);

            if (NextRecharge < DateTime.UtcNow)
            {
                UsesRemaining = Math.Min(20, UsesRemaining + 1);
                NextRecharge = DateTime.UtcNow + TimeSpan.FromHours(24);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _UsesRemaining = reader.ReadInt();
            NextRecharge = reader.ReadDateTime();
        }
    }
}
