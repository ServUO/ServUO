using Server.Engines.Points;
using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.TreasuresOfDoom
{
    public class Carey : BaseTurnInMobile
    {
        public override int TitleLocalization => 1155595;  // Artifacts of Doom
        public override int CancelLocalization => 1155591; 	// Bring me items of Doom and I will reward you with valuable items.
        public override int TurnInLocalization => 1155595;  // Artifacts of Doom

        public static Carey Instance { get; set; }
        public static readonly Point3D SpawnLocation = new Point3D(2373, 1278, -90);

        [Constructable]
        public Carey() : base("the Researcher")
        {
            Instance = this;
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = "Carey";
            Female = true;

            SpeechHue = 0x3B2;
            Hue = Utility.RandomSkinHue();
            Body = 0x191;
        }

        public override void InitOutfit()
        {
            SetWearable(new Robe(), 1364);
            SetWearable(new ThighBoots(), 1908);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1155594); // Artifacts of Doom Trader
        }

        private DateTime _NextSpeak;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (_NextSpeak < DateTime.UtcNow)
            {
                SayTo(m, 1155591); // Bring me items of the Kotl and I will reward you with valuable items.
                _NextSpeak = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            }
        }

        public override void AwardPoints(PlayerMobile pm, Item item, int amount)
        {
            PointsSystem.TreasuresOfDoom.AwardPoints(pm, amount);
        }

        public override bool IsRedeemableItem(Item item)
        {
            if (item is ICombatEquipment && ((ICombatEquipment)item).ReforgedSuffix == ReforgedSuffix.Doom)
                return true;

            return false;
        }

        public override void SendRewardGump(Mobile m)
        {
            if (m.Player && m.CheckAlive())
                m.SendGump(new DoomRewardGump(this, m as PlayerMobile));
        }

        public Carey(Serial serial)
            : base(serial)
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

            Instance = this;
        }
    }
}
