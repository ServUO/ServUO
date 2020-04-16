using Server.Engines.InstancedPeerless;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class RejoinInstanceGump : Gump
    {
        private readonly PeerlessInstance m_Instance;

        public RejoinInstanceGump(PeerlessInstance instance, int titleCliloc, int msgCliloc)
            : base(340, 340)
        {
            /* Not sure if the gump structure is the same as OSI, but this is better than nothing */

            m_Instance = instance;

            AddPage(0);

            AddBackground(0, 0, 291, 99, 0x13BE);
            AddImageTiled(5, 6, 280, 20, 0xA40);

            AddHtmlLocalized(9, 8, 280, 20, titleCliloc, 0x7FFF, false, false);

            AddImageTiled(5, 31, 280, 40, 0xA40);

            AddHtmlLocalized(9, 35, 272, 40, msgCliloc, 0x7FFF, false, false);

            AddButton(215, 73, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(250, 75, 65, 20, 1006044, 0x7FFF, false, false); // OK

            AddButton(5, 73, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 75, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (info.ButtonID == 1)
            {
                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                Effects.PlaySound(from.Location, from.Map, 0x1FE);

                BaseCreature.TeleportPets(from, m_Instance.EntranceLocation, m_Instance.Map);
                from.MoveToWorld(m_Instance.EntranceLocation, m_Instance.Map);

                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                Effects.PlaySound(from.Location, from.Map, 0x1FE);

                m_Instance.AddFighter(from);
            }
        }
    }
}