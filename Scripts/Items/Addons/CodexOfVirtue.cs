namespace Server.Items
{
    public class CodexOfVirtueAddon : BaseAddon
    {
        [Constructable]
        public CodexOfVirtueAddon()
        {
            AddComponent(new AddonComponent(16637), 3, 3, 0);
            AddComponent(new AddonComponent(16636), 2, 3, 0);
            AddComponent(new AddonComponent(16635), 1, 3, 0);
            AddComponent(new AddonComponent(16634), 0, 3, 0);
            AddComponent(new AddonComponent(16633), -1, 3, 0);
            AddComponent(new AddonComponent(16632), -2, 3, 0);
            AddComponent(new AddonComponent(16631), 3, 2, 0);
            AddComponent(new AddonComponent(16630), 2, 2, 0);
            AddComponent(new AddonComponent(16629), 1, 2, 0);
            AddComponent(new AddonComponent(16628), 0, 2, 0);
            AddComponent(new AddonComponent(16627), -1, 2, 0);
            AddComponent(new AddonComponent(16626), -2, 2, 0);
            AddComponent(new AddonComponent(16625), 3, 1, 0);
            AddComponent(new AddonComponent(16624), 2, 1, 0);
            AddComponent(new AddonComponent(16623), 1, 1, 0);
            AddComponent(new AddonComponent(16622), 0, 1, 0);
            AddComponent(new AddonComponent(16621), -1, 1, 0);
            AddComponent(new AddonComponent(16620), -2, 1, 0);
            AddComponent(new AddonComponent(16619), 3, 0, 0);
            AddComponent(new AddonComponent(16618), 2, 0, 0);
            AddComponent(new AddonComponent(16617), 1, 0, 0);
            AddComponent(new AddonComponent(16616), 0, 0, 0);
            AddComponent(new AddonComponent(16615), -1, 0, 0);
            AddComponent(new AddonComponent(16614), -2, 0, 0);
            AddComponent(new AddonComponent(16613), 3, -1, 0);
            AddComponent(new AddonComponent(16612), 2, -1, 0);
            AddComponent(new AddonComponent(16611), 1, -1, 0);
            AddComponent(new AddonComponent(16610), 0, -1, 0);
            AddComponent(new AddonComponent(16609), -1, -1, 0);
            AddComponent(new AddonComponent(16608), -2, -1, 0);
            AddComponent(new AddonComponent(16607), 3, -2, 0);
            AddComponent(new AddonComponent(16606), 2, -2, 0);
            AddComponent(new AddonComponent(16605), 1, -2, 0);
            AddComponent(new AddonComponent(16604), 0, -2, 0);
            AddComponent(new AddonComponent(16603), -1, -2, 0);
            AddComponent(new AddonComponent(16602), -2, -2, 0);
        }

        public CodexOfVirtueAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new CodexOfVirtueDeed();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CodexOfVirtueDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1113919; // a Codex of Virtue deed

        [Constructable]
        public CodexOfVirtueDeed()
        {
        }

        public CodexOfVirtueDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new CodexOfVirtueAddon();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
