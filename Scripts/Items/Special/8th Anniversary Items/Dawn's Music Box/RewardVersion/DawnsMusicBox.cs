using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items.MusicBox
{
    [Flipable(0x2AF9, 0x2AFD)]
    public class DawnsMusicBox : Item, ISecurable, IRewardItem
    {
        public static readonly int MusicRange = 10;
        private List<MusicName> m_Tracks;
        private Timer m_PlayingTimer;
        private MusicName m_ActualSong;
        private SecureLevel m_Level;
        private bool m_IsRewardItem;
        [Constructable]
        public DawnsMusicBox()
            : base(0x2AF9)
        {
            this.Weight = 1.0;
            
            this.m_Tracks = new List<MusicName>();
            this.m_ActualSong = MusicName.Invalid;
            
            while (this.Tracks.Count < 4)
                this.AddSong(TrackInfo.RandomSong(TrackRarity.Common));
        }

        public DawnsMusicBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075198;
            }
        }// Dawn’s Music Box
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Developer)]
        public bool IsPlaying
        {
            get
            {
                return this.m_PlayingTimer != null;
            }
        }
        public List<MusicName> Tracks
        {
            get
            {
                return this.m_Tracks;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Developer)]
        public MusicName ActualSong
        {
            get
            {
                return this.m_ActualSong;
            }
            set
            {
                this.m_ActualSong = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            int commonSongs = 0;
            int unCommonSongs = 0;
            int rareSongs = 0;
			
            for (int i = 0; i < this.m_Tracks.Count; i++)
            {
                TrackInfo ti = TrackInfo.GetInfo(this.m_Tracks[i]);
                switch( ti.Rarity )
                {
                    case TrackRarity.Common:
                        commonSongs++;
                        break;
                    case TrackRarity.UnCommon:
                        unCommonSongs++;
                        break;
                    case TrackRarity.Rare:
                        rareSongs++;
                        break;
                }
            }
			
            if (commonSongs > 0)
                list.Add(1075234, commonSongs.ToString()); // ~1_NUMBER~ Common Tracks
            if (unCommonSongs > 0)
                list.Add(1075235, unCommonSongs.ToString()); // ~1_NUMBER~ Uncommon Tracks
            if (rareSongs > 0)
                list.Add(1075236, rareSongs.ToString()); // ~1_NUMBER~ Rare Tracks
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            
            SetSecureLevelEntry.AddTo(from, this, list); // Set secure level
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Tracks.Count < 1)
            {
                from.SendMessage("This music box is empty.");
            }
            else if (this.IsOwner(from))
            {
                if (!this.IsLockedDown)
                    from.SendLocalizedMessage(502692); // This must be in a house and be locked down to work.
                else
                {
                    if (from.HasGump(typeof(MusicGump)))
                        from.CloseGump(typeof(MusicGump));
					
                    from.SendGump(new MusicGump(this));
                }
            }
            else
            {
                from.SendLocalizedMessage(502691); // You must be the owner to use this.
            }
        }

        public bool AddSong(MusicName song)
        {
            if (this.m_Tracks.Contains(song))
            {
                return false;
            }
            else
            {
                this.m_Tracks.Add(song);
                return true;
            }
        }

        public void Animate()
        {
            switch( this.ItemID )
            {
                case 0x2AF9:
                    this.ItemID = 0x2AFB;
                    break;
                    //            	case 0x2AFA:	ItemID = 0x2AFB; break;
                case 0x2AFB:
                    this.ItemID = 0x2AFC;
                    break;
                case 0x2AFC:
                    this.ItemID = 0x2AF9;
                    break;
                case 0x2AFD:
                    this.ItemID = 0x2AFF;
                    break;
                    //            	case 0x2AFE:	ItemID = 0x2AFF; break;
                case 0x2AFF:
                    this.ItemID = 0x2B00;
                    break;
                case 0x2B00:
                    this.ItemID = 0x2AFD;
                    break;
            }
        }

        public bool IsOwner(Mobile mob)
        {
            if (mob.AccessLevel >= AccessLevel.GameMaster)
                return true;
			
            BaseHouse house = BaseHouse.FindHouseAt(this);

            return (house != null && house.IsOwner(mob));
        }

        public void ToggleMusic(Mobile m, bool play)
        {
            if (this.m_ActualSong != MusicName.Invalid && m.NetState != null)
            {
                m.Send(PlayMusic.InvalidInstance); // Stop actual music
            	
                if (play)
                    m.Send(PlayMusic.GetInstance(this.m_ActualSong));
            }
        }

        public void TogglePlaying(bool hasToStart)
        {
            this.ToggleTimer(hasToStart);
        	
            string message = hasToStart ? "* The musix box starts playing a song *" : "* The musix box stops *";
        	
            this.PublicOverheadMessage(MessageType.Regular, 0x5D, true, message);
            this.StopBoxesInRange();
            Map boxMap = this.Map;
	
            if (boxMap != Map.Internal)
            {
                Point3D boxLoc = this.Location;
                IPooledEnumerable mobsEable = boxMap.GetMobilesInRange(boxLoc, MusicRange);
	        	
                foreach (Mobile m in mobsEable)
                {
                    if (m is Mobiles.PlayerMobile)
                        this.ToggleMusic(m, hasToStart);
                }
	            
                mobsEable.Free();
            }
        }

        public void ToggleTimer(bool hasToStart)
        {
            if (this.IsPlaying && !hasToStart)
            {
                if (this.m_PlayingTimer != null && this.m_PlayingTimer.Running)	// remove correctly the timer...
                    this.m_PlayingTimer.Stop();
                this.m_PlayingTimer = null;
            }
            else if (!this.IsPlaying && hasToStart)
            {
                TrackInfo ti = TrackInfo.GetInfo(this.m_ActualSong);

                this.m_PlayingTimer = new PlayingTimer((double)ti.Duration, this);	// add a new timer
                this.m_PlayingTimer.Start();         		
            }
        }

        public void StopBoxesInRange()
        {
            Map boxMap = this.Map;

            if (boxMap != Map.Internal)
            {
                Point3D boxLoc = this.Location;
                IPooledEnumerable itemsEable = boxMap.GetItemsInRange(boxLoc, MusicRange);

                foreach (Item i in itemsEable)
                {
                    if (i is DawnsMusicBox && i != this)
                    {
                        DawnsMusicBox mb = (DawnsMusicBox)i;
                        if (mb.IsPlaying)
                        {
                            mb.ToggleTimer(false);
                            mb.PublicOverheadMessage(MessageType.Regular, 0x5D, true, "* The musix box stops *");
                        }
                    }
                }

                itemsEable.Free();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            
            writer.Write((int)0); // version
            
            writer.Write(this.m_Tracks.Count);
            
            for (int i = 0; i < this.m_Tracks.Count; i++)
                writer.Write((int)this.m_Tracks[i]);
            	
            writer.Write((int)this.m_Level);
            writer.Write(this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            
            int version = reader.ReadInt();
            
            switch( version )
            {
                case 0:
                    {
                        if (this.m_Tracks == null)
                            this.m_Tracks = new List<MusicName>();
        			
                        int numSongs = reader.ReadInt();
                        for (int i = 0; i < numSongs; i++)
                            this.m_Tracks.Add((MusicName)reader.ReadInt());
            		
                        this.m_Level = (SecureLevel)reader.ReadInt();
                        this.m_IsRewardItem = reader.ReadBool();
                    
                        this.ToggleTimer(false);
                    
                        break;
                    }
            }
        }

        internal class PlayingTimer : Timer
        {
            private readonly DawnsMusicBox m_Box;
            private readonly DateTime m_Until;
            public PlayingTimer(double duration, DawnsMusicBox box)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Box = box;
                this.m_Until = DateTime.UtcNow + TimeSpan.FromSeconds(duration);
                
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (DateTime.UtcNow > this.m_Until)
                {
                    if (this.m_Box != null && !this.m_Box.Deleted)
                        this.m_Box.TogglePlaying(false);
                    else
                        this.Stop();
                }
                else if (this.m_Box != null && !this.m_Box.Deleted)
                    this.m_Box.Animate();
            }
        }

        private class MusicGump : Gump
        {
            private static readonly int m_Fields = 9;
            private static readonly int m_HueTit = 32767;
            private static readonly int m_HueEnt = 32767;
            private static readonly int m_DeltaBut = 2;
            private static readonly int m_FieldsDist = 25;
            private readonly DawnsMusicBox m_Box;
            private readonly List<int> m_Songs;
            private readonly bool m_HasStopSongEntry;
            private int m_Page;
            public MusicGump(DawnsMusicBox box)
                : this(box, null, 1)
            { 
            }

            public MusicGump(DawnsMusicBox box, List<int> songs, int page)
                : base(50, 50)
            {
                this.Closable = false;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.m_Box = box;
                this.m_Songs = songs;
                this.m_Page = page;
					
                this.m_HasStopSongEntry = this.m_Box.IsPlaying;
				
                if (this.m_Songs == null)
                    this.m_Songs = BuildList(box, this.m_HasStopSongEntry);

                this.Initialize();
            }

            public void Initialize()
            {
                this.AddPage(0);
				
                this.AddBackground(0, 0, 275, 325, 9200);
	            
                this.AddImageTiled(10, 10, 255, 25, 2624); 
                this.AddImageTiled(10, 45, 255, 240, 2624); 
                this.AddImageTiled(40, 295, 225, 20, 2624);
	            
                this.AddButton(10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0); 
                this.AddHtmlLocalized(45, 295, 75, 20, 1011012, m_HueTit, false, false); // CANCEL
	
                this.AddAlphaRegion(10, 10, 255, 285);
                this.AddAlphaRegion(40, 295, 225, 20);
	            
                this.AddHtmlLocalized(14, 12, 255, 25, 1075130, m_HueTit, false, false); // Choose a track to play
	            
                if (this.m_Page > 1)
                    this.AddButton(225, 297, 5603, 5607, 200, GumpButtonType.Reply, 0); // Previous page
	
                if (this.m_Page < Math.Ceiling(this.m_Songs.Count / (double)m_Fields))
                    this.AddButton(245, 297, 5601, 5605, 300, GumpButtonType.Reply, 0); // Next Page
				
                int IndMax = (this.m_Page * m_Fields) - 1;
                int IndMin = (this.m_Page * m_Fields) - m_Fields;
                int IndTemp = 0;

                for (int i = 0; i < this.m_Songs.Count; i++)
                {
                    if (i >= IndMin && i <= IndMax)
                    {
                        this.AddHtmlLocalized(35, 52 + (IndTemp * m_FieldsDist), 225, 20, this.m_Songs[i], m_HueEnt, false, false);
                        this.AddButton(15, 52 + m_DeltaBut + (IndTemp * m_FieldsDist), 1209, 1210, i + 1, GumpButtonType.Reply, 0);
                        IndTemp++;
                    }
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                if (info.ButtonID == 0)
                    return;
                else if (info.ButtonID == 200) // Previous page
                {
                    this.m_Page--;
                    from.SendGump(new MusicGump(this.m_Box, this.m_Songs, this.m_Page));
                }
                else if (info.ButtonID == 300)  // Next Page
                {
                    this.m_Page++;
                    from.SendGump(new MusicGump(this.m_Box, this.m_Songs, this.m_Page));
                }
                else if (this.m_HasStopSongEntry && info.ButtonID == this.m_Songs.Count)
                {
                    this.m_Box.TogglePlaying(false);
                }
                else
                {
                    TrackInfo ti = TrackInfo.GetInfo(this.m_Songs[info.ButtonID - 1]);
                    this.m_Box.ActualSong = ti.Name;
                    this.m_Box.TogglePlaying(true);
                }
            }

            private static List<int> BuildList(DawnsMusicBox box, bool hasStopSongEntry)
            {
                List<int> list = new List<int>();
				
                for (int i = 0; i < box.Tracks.Count; i++)
                {
                    TrackInfo ti = TrackInfo.GetInfo(box.Tracks[i]);
                    list.Add(ti.Label);
                }
				
                if (hasStopSongEntry)
                    list.Add(1075207); // Stop Song
				
                return list;
            }
        }
    }
}