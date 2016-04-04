using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests
{
    public sealed class HeritagePacket : Packet
    {
        public static readonly Packet Close = Packet.SetStatic(new HeritagePacket(false, 0xFF));
        public HeritagePacket(bool female, short type)
            : base(0xBF)
        {
            this.EnsureCapacity(7);
			
            this.m_Stream.Write((short)0x2A);
            this.m_Stream.Write((byte)(female ? 1 : 0));
            this.m_Stream.Write((byte)type);
        }
    }

    public class PacketOverrides
    {
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Override));
        }

        public static void Override()
        {
            PacketHandlers.RegisterEncoded(0x32, true, new OnEncodedPacketReceive(QuestButton)); 
            PacketHandlers.RegisterExtended(0x2A, true, new OnPacketReceive(HeritageTransform)); 
        }

        public static void QuestButton(NetState state, IEntity e, EncodedReader reader) 
        {
            if (state.Mobile is PlayerMobile)
            {
                PlayerMobile from = (PlayerMobile)state.Mobile;
				
                from.CloseGump(typeof(MondainQuestGump));
                from.SendGump(new MondainQuestGump(from));
            }
        }

        public static void HeritageTransform(NetState state, PacketReader reader)
        {
            Mobile m = state.Mobile;
			
            if (reader.Size == 5)
            {
                m.SendLocalizedMessage(1073645); // You may try this again later...	
				
                HeritageQuester.RemovePending(m);
            }
            else if (reader.Size == 15)
            { 
                if (HeritageQuester.IsPending(m))
                {
                    HeritageQuester quester = HeritageQuester.Pending(m);
				
                    m.Race = quester.Race;
					
                    m.Hue = reader.ReadUInt16();
                    m.HairItemID = reader.ReadUInt16();
                    m.HairHue = reader.ReadUInt16();
                    m.FacialHairItemID = reader.ReadUInt16();
                    m.FacialHairHue = reader.ReadUInt16();
				
                    HeritageQuester.RemovePending(m);
                    quester.CheckCompleted(m, true); // removes done quests
					
                    if (m.Race == Race.Elf)
                        m.SendLocalizedMessage(1073653); // You are now fully initiated into the Elven culture.
                    else if (m.Race == Race.Human)						
                        m.SendLocalizedMessage(1073654); // You are now fully human.
                }
            }
        }
    }
}