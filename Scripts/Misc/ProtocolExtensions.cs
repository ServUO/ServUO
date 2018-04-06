using System;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Network;

namespace Server.Misc
{
    public class ProtocolExtensions
    {
        private static readonly PacketHandler[] m_Handlers = new PacketHandler[0x100];
        public static void Initialize()
        {
            PacketHandlers.Register(0xF0, 0, false, new OnPacketReceive(DecodeBundledPacket));

            Register(0x00, true, new OnPacketReceive(QueryPartyLocations));
            Register(0x01, true, new OnPacketReceive(QueryGuildsLocations));
        }

        public static void QueryPartyLocations(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            Party party = Party.Get(from);

            if (party != null)
            {
                AckPartyLocations ack = new AckPartyLocations(from, party);

                if (ack.UnderlyingStream.Length > 8)
                    state.Send(ack);
            }
        }

        private static void QueryGuildsLocations(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            Guild guild = from.Guild as Guild;

            if (guild != null)
            {
                bool locations = pvSrc.ReadByte() != 0;

                AckGuildsLocations packet = new AckGuildsLocations(from, guild, locations);

                if (packet.UnderlyingStream.Length > (locations ? 9 : 5))
                    state.Send(packet);
            }
            else
                state.Send(new AckGuildsLocations());
        }

        public static void Register(int packetID, bool ingame, OnPacketReceive onReceive)
        {
            m_Handlers[packetID] = new PacketHandler(packetID, 0, ingame, onReceive);
        }

        public static PacketHandler GetHandler(int packetID)
        {
            if (packetID >= 0 && packetID < m_Handlers.Length)
                return m_Handlers[packetID];

            return null;
        }

        public static void DecodeBundledPacket(NetState state, PacketReader pvSrc)
        {
            int packetID = pvSrc.ReadByte();

            PacketHandler ph = GetHandler(packetID);

            if (ph != null)
            {
                if (ph.Ingame && state.Mobile == null)
                {
                    Utility.PushColor(ConsoleColor.DarkRed);
                    Console.WriteLine("Client: {0}: Sent ingame packet (0xF0x{1:X2}) before having been attached to a mobile", state, packetID);
                    Utility.PopColor();
                    state.Dispose();
                }
                else if (ph.Ingame && state.Mobile.Deleted)
                {
                    state.Dispose();
                }
                else
                {
                    ph.OnReceive(state, pvSrc);
                }
            }
        }
    }

    public abstract class ProtocolExtension : Packet
    {
        public ProtocolExtension(int packetID, int capacity)
            : base(0xF0)
        {
            this.EnsureCapacity(4 + capacity);

            this.m_Stream.Write((byte)packetID);
        }
    }

    public class AckPartyLocations : ProtocolExtension
    {
        public AckPartyLocations(Mobile from, Party party)
            : base(0x01, ((party.Members.Count - 1) * 9) + 4)
        {
            for (int i = 0; i < party.Members.Count; ++i)
            {
                PartyMemberInfo pmi = (PartyMemberInfo)party.Members[i];

                if (pmi == null || pmi.Mobile == from)
                    continue;

                Mobile mob = pmi.Mobile;

                if (Utility.InUpdateRange(from, mob) && from.CanSee(mob))
                    continue;

                this.m_Stream.Write((int)mob.Serial);
                this.m_Stream.Write((short)mob.X);
                this.m_Stream.Write((short)mob.Y);
                this.m_Stream.Write((byte)(mob.Map == null ? 0 : mob.Map.MapID));
            }

            this.m_Stream.Write((int)0);
        }
    }

    public class AckGuildsLocations : ProtocolExtension
    {
        public AckGuildsLocations() : base(0x02, 5)
        {
            m_Stream.Write((byte)0);
            m_Stream.Write((int)0);
        }

        public AckGuildsLocations(Mobile from, Guild guild, bool locations) : base(0x02, ((guild.Members.Count - 1) * (locations ? 10 : 4)) + 5)
        {
            m_Stream.Write((byte)(locations ? 1 : 0));

            for (int i = 0; i < guild.Members.Count; ++i)
            {
                Mobile mob = guild.Members[i];

                if (mob == null || mob == from || mob.NetState == null)
                    continue;

                if (locations && Utility.InUpdateRange(from, mob) && from.CanSee(mob))
                    continue;

                m_Stream.Write((int)mob.Serial);

                if (locations)
                {
                    m_Stream.Write((short)mob.X);
                    m_Stream.Write((short)mob.Y);
                    m_Stream.Write((byte)(mob.Map == null ? 0 : mob.Map.MapID));

                    if (mob.Alive)
                        m_Stream.Write((byte)(mob.Hits / Math.Max(mob.HitsMax, 1.0) * 100));
                    else
                        m_Stream.Write((byte)0);
                }
            }

            m_Stream.Write((int)0);
        }
    }
}