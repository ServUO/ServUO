using System;
using Server.Multis;

namespace Server.Items
{
    public class SerpentPillar : Item
    {
        private bool m_Active;
        private string m_Word;
        private Rectangle2D m_Destination;
        [Constructable]
        public SerpentPillar()
            : this(null, new Rectangle2D(), false)
        {
        }

        public SerpentPillar(string word, Rectangle2D destination)
            : this(word, destination, true)
        {
        }

        public SerpentPillar(string word, Rectangle2D destination, bool active)
            : base(0x233F)
        {
            this.Movable = false;

            this.m_Active = active;
            this.m_Word = word;
            this.m_Destination = destination;
        }

        public SerpentPillar(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return this.m_Active;
            }
            set
            {
                this.m_Active = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Word
        {
            get
            {
                return this.m_Word;
            }
            set
            {
                this.m_Word = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D Destination
        {
            get
            {
                return this.m_Destination;
            }
            set
            {
                this.m_Destination = value;
            }
        }
        public override bool HandlesOnSpeech
        {
            get
            {
                return true;
            }
        }
        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (!e.Handled && from.InRange(this, 10) && e.Speech.ToLower() == this.Word)
            {
                BaseBoat boat = BaseBoat.FindBoatAt(from, from.Map);

                if (boat == null)
                    return;

                if (!this.Active)
                {
                    if (boat.TillerMan != null)
                        boat.TillerMan.Say(502507); // Ar, Legend has it that these pillars are inactive! No man knows how it might be undone!

                    return;
                }

                Map map = from.Map;

                for (int i = 0; i < 5; i++) // Try 5 times
                {
                    int x = Utility.Random(this.Destination.X, this.Destination.Width);
                    int y = Utility.Random(this.Destination.Y, this.Destination.Height);
                    int z = map.GetAverageZ(x, y);

                    Point3D dest = new Point3D(x, y, z);

                    if (boat.CanFit(dest, map, boat.ItemID))
                    {
                        int xOffset = x - boat.X;
                        int yOffset = y - boat.Y;
                        int zOffset = z - boat.Z;

                        boat.Teleport(xOffset, yOffset, zOffset);

                        return;
                    }
                }

                if (boat.TillerMan != null)
                    boat.TillerMan.Say(502508); // Ar, I refuse to take that matey through here!
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((bool)this.m_Active);
            writer.Write((string)this.m_Word);
            writer.Write((Rectangle2D)this.m_Destination);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Active = reader.ReadBool();
            this.m_Word = reader.ReadString();
            this.m_Destination = reader.ReadRect2D();
        }
    }
}