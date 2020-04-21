using Server.Targeting;

namespace Server.Items.MusicBox
{
    [Flipable(0x1053, 0x1054)]
    public class MusicBoxGears : Item
    {
        private readonly MusicName m_Music;
        [Constructable]
        public MusicBoxGears()
            : this(TrackInfo.RandomSong())
        {
        }

        [Constructable]
        public MusicBoxGears(MusicName music)
            : base(0x1053)
        {
            m_Music = music;
            Weight = 1.0;
        }

        public MusicBoxGears(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MusicName Music => m_Music;
        public static MusicBoxGears RandomMusixBoxGears(TrackRarity rarity)
        {
            return new MusicBoxGears(TrackInfo.RandomSong(rarity));
        }

        public static MusicBoxGears RandomMusixBoxGears()
        {
            return new MusicBoxGears(TrackInfo.RandomSong());
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            TrackInfo ti = TrackInfo.GetInfo(m_Music);
            switch (ti.Rarity)
            {
                case TrackRarity.Common:
                    list.Add(1075204);
                    break; // Gear for Dawn's Music Box (Common)
                case TrackRarity.UnCommon:
                    list.Add(1075205);
                    break; // Gear for Dawn's Music Box (Uncommon)
                case TrackRarity.Rare:
                    list.Add(1075206);
                    break; // Gear for Dawn's Music Box (Rare)
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            TrackInfo ti = TrackInfo.GetInfo(m_Music);
            list.Add(ti.Label);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.BeginTarget(3, false, TargetFlags.None, OnTarget);
                from.SendMessage("Select a Dawn's music box to add this gears to.");
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public virtual void OnTarget(Mobile from, object obj)
        {
            if (Deleted)
                return;

            DawnsMusicBox mb = obj as DawnsMusicBox;

            if (mb == null)
            {
                from.SendMessage("That is not a Dawn's music box.");
            }
            else
            {
                if (mb.AddSong(m_Music))
                {
                    from.SendMessage("You have added this gear to the music box.");
                    Delete();
                }
                else
                    from.SendMessage("This gear is already present in this box.");
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write((int)m_Music);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            MusicName m_Music = (MusicName)reader.ReadInt();
        }
    }
}
