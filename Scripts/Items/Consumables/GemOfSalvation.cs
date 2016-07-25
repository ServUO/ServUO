using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class GemOfSalvation : Item
    {
        public static readonly TimeSpan Cooldown = TimeSpan.FromHours(6.0);

        public override int LabelNumber { get { return 1094939; } } // Gem of Salvation

        [Constructable]
        public GemOfSalvation()
            : base(0x1F13)
        {
            this.LootType = LootType.Blessed;

            this.Weight = 1.0;
            this.Hue = 0x11E;
        }

        public void Use(PlayerMobile pm)
        {
            if (DateTime.UtcNow < pm.NextGemOfSalvationUse)
            {
                TimeSpan left = pm.NextGemOfSalvationUse - DateTime.UtcNow;

                if (left >= TimeSpan.FromMinutes(1.0))
                    pm.SendLocalizedMessage(1095131, ((left.Hours * 60) + left.Minutes).ToString()); // Your spirit lacks cohesion. You must wait ~1_minutes~ minutes before invoking the power of a Gem of Salvation.
                else
                    pm.SendLocalizedMessage(1095130, left.Seconds.ToString()); // Your spirit lacks cohesion. You must wait ~1_seconds~ seconds before invoking the power of a Gem of Salvation.
            }
            else
            {
                pm.CloseGump(typeof(ResurrectGump));
                pm.SendGump(new GemResurrectGump(pm, this));
            }
        }

        public GemOfSalvation(Serial serial)
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

    public class GemResurrectGump : ResurrectGump
    {
        private GemOfSalvation m_Gem;
        private PlayerMobile m_Mobile;

        public GemResurrectGump(PlayerMobile pm, GemOfSalvation gem)
            : base(pm, ResurrectMessage.GemOfSalvation)
        {
            m_Gem = gem;
            m_Mobile = pm;
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            m_Mobile.CloseGump(typeof(ResurrectGump));

            if (info.ButtonID == 2 && !m_Gem.Deleted && m_Gem.IsChildOf(m_Mobile.Backpack))
            {
                if (m_Mobile.Map == null || !m_Mobile.Map.CanFit(m_Mobile.Location, 16, false, false))
                {
                    m_Mobile.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                    return;
                }

                m_Mobile.PlaySound(0x214);
                m_Mobile.Resurrect();

                m_Mobile.SendLocalizedMessage(1095132); // The gem infuses you with its power and is destroyed in the process.

                m_Gem.Delete();

                m_Mobile.NextGemOfSalvationUse = DateTime.UtcNow + GemOfSalvation.Cooldown;
            }
        }
    }
}