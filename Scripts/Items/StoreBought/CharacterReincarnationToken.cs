namespace Server.Items
{
    public class CharacterReincarnationToken : PromotionalToken
    {
        public override TextDefinition ItemName => 1156612;  // Character Reincarnation
        public override TextDefinition ItemReceiveMessage => null;
        public override TextDefinition ItemGumpName => "<center>Character Reincarnation</center>";

        public override bool PlaceInBank => false;

        [Constructable]
        public CharacterReincarnationToken()
        {
            LootType = LootType.Blessed;
        }

        public override Item CreateItemFor(Mobile from)
        {
            Bag bag = new Bag();

            bag.DropItem(new GenderChangeToken());
            bag.DropItem(new NameChangeToken());
            bag.DropItem(new RaceChangeToken());

            return bag;
        }

        public CharacterReincarnationToken(Serial serial)
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
        }
    }
}
