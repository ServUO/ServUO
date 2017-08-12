using System;
using Server;
using Server.Items;
using Server.Gumps;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Points;
using Server.Mobiles;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class Hal : BaseTurnInMobile
    {
        public override int TitleLocalization { get { return 1154520; } } // Click a minor artifact to turn in for reward points.
        public override int CancelLocalization { get { return 1156903; } }	// Bring me items of the Kotl and I will reward you with valuable items.
        public override int TurnInLocalization { get { return 1155592; } } // Turn In Artifacts of the Kotl
        

        [Constructable]
        public Hal() : base("the Researcher")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = "Hal";

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x190;
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt(), 1255);
            SetWearable(new JinBaori(), 2722);
            SetWearable(new Kilt(), 2012);
            SetWearable(new ThighBoots(), 1908);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1157038); // Artifacts of the Kotl City Trader
        }

        private DateTime _NextSpeak;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (_NextSpeak < DateTime.UtcNow)
            {
                SayTo(m, 1156903); // Bring me items of the Kotl and I will reward you with valuable items.
                _NextSpeak = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            }
        }

        public override void AwardPoints(PlayerMobile pm, int amount)
        {
            PointsSystem.TreasuresOfKotlCity.AwardPoints(pm, amount);
        }

        public override bool IsRedeemableItem(Item item)
        {
            if (item is BaseWeapon && ((BaseWeapon)item).ReforgedSuffix == ReforgedSuffix.Kotl)
                return true;
            if (item is BaseArmor && ((BaseArmor)item).ReforgedSuffix == ReforgedSuffix.Kotl)
                return true;
            if (item is BaseJewel && ((BaseJewel)item).ReforgedSuffix == ReforgedSuffix.Kotl)
                return true;
            if (item is BaseClothing && ((BaseClothing)item).ReforgedSuffix == ReforgedSuffix.Kotl)
                return true;

            return false;
        }

        public override void SendRewardGump(Mobile m)
        {
            if (m.Player && m.CheckAlive())
                m.SendGump(new KotlCityRewardGump(this, m as PlayerMobile));
        }

        public Hal(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}