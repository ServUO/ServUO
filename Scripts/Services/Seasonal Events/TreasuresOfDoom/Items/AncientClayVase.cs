using Server.Engines.TreasuresOfDoom;
using Server.Network;
using Server.SkillHandlers;

namespace Server.Items
{
    public class AncientClayVase : Item, ICarvable
    {
        public override int LabelNumber => 1155625;  // Ancient Clay Vase
        public bool DoomEvent { get; set; }

        [Constructable]
        public AncientClayVase()
            : this(false)
        {
        }

        public AncientClayVase(bool doom)
            : base(0x42B3)
        {
            DoomEvent = doom;
            Hue = 2676;
            LootType = LootType.Blessed;
        }

        public bool Carve(Mobile from, Item item)
        {
            if (IsChildOf(from.Backpack))
            {
                from.PlaySound(Utility.Random(0x3E, 4));
                from.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1155629, from.NetState); // *The vase shatters as you cut away the sealing wax!"

                Delete();

                if (0.5 > Utility.RandomDouble())
                {
                    from.AddToBackpack(new AncientParchment());
                }

                return true;
            }

            return false;
        }

        public static void Initialize()
        {
            Stealing.ItemStolen += OnStolen;
        }

        public static void OnStolen(ItemStolenEventArgs e)
        {
            if (e.Item is AncientClayVase)
            {
                e.Mobile.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1155626, e.Mobile.NetState); // *It appears to be an ancient vase. The top is sealed with some kind of wax. A bladed item would perhaps be useful...*
            }
        }

        public AncientClayVase(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(DoomEvent);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            DoomEvent = reader.ReadBool();

            if (DoomEvent)
            {
                VaseSpawner.AddToSpawner(this);
            }
        }
    }
}