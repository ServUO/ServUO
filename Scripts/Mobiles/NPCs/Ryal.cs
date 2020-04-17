using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class DaemonicPrismQuest : BaseQuest
    {
        public DaemonicPrismQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(CrystalDaemon), "crystal daemons", 3, "Prism of Light"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Daemonic Prism */
        public override object Title => 1073053;
        /* Good, you're here.  The presence of a twisted creature deep under the earth near Nu'Jelm has 
        corrupted the natural growth of crystals in that region.  They've become infused with the 
        twisting energy - they've come to a sort of life.  This is an abomination that festers within 
        Sosaria. You must eradicate the crystal daemons. */
        public override object Description => 1074668;
        /* These abominations must not be permitted to fester! */
        public override object Refuse => 1074671;
        /* You must not waste time. Do not suffer these crystalline abominations to live. */
        public override object Uncomplete => 1074672;
        /* You have done well.  Enjoy this reward. */
        public override object Complete => 1074673;
        public override bool CanOffer()
        {
            return MondainsLegacy.PrismOfLight;
        }

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

    public class HowManyHeadsQuest : BaseQuest
    {
        public HowManyHeadsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(CrystalHydra), "crystal hydras", 3, "Prism of Light"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* How Many Heads? */
        public override object Title => 1073050;
        /* Good, you're here.  The presence of a twisted creature deep under the earth near Nu'Jelm has 
        corrupted the natural growth of crystals in that region.  They've become infused with the twisting 
        energy - they've come to a sort of life.  This is an abomination that festers within Sosaria. You 
        must eradicate the crystal hydras. */
        public override object Description => 1074674;
        /* These abominations must not be permitted to fester! */
        public override object Refuse => 1074671;
        /* You must not waste time. Do not suffer these crystalline abominations to live. */
        public override object Uncomplete => 1074672;
        /* You have done well.  Enjoy this reward. */
        public override object Complete => 1074673;
        public override bool CanOffer()
        {
            return MondainsLegacy.PrismOfLight;
        }

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

    public class GlassyFoeQuest : BaseQuest
    {
        public GlassyFoeQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(CrystalLatticeSeeker), "crystal lattice seekers", 5, "Prism of Light"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Glassy Foe */
        public override object Title => 1073055;
        /* Good, you're here.  The presence of a twisted creature deep under the earth near Nu'Jelm has 
        corrupted the natural growth of crystals in that region.  They've become infused with the twisting 
        energy - they've come to a sort of life.  This is an abomination that festers within Sosaria.  You 
        must eradicate the crystal lattice seekers. */
        public override object Description => 1074669;
        /* These abominations must not be permitted to fester! */
        public override object Refuse => 1074671;
        /* You must not waste time. Do not suffer these crystalline abominations to live. */
        public override object Uncomplete => 1074672;
        /* You have done well.  Enjoy this reward. */
        public override object Complete => 1074673;
        public override bool CanOffer()
        {
            return MondainsLegacy.PrismOfLight;
        }

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

    public class HailstormQuest : BaseQuest
    {
        public HailstormQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(CrystalVortex), "crystal vortices", 8, "Prism of Light"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Hailstorm */
        public override object Title => 1073057;
        /* Good, you're here.  The presence of a twisted creature deep under the earth near Nu'Jelm has corrupted 
        the natural growth of crystals in that region.  They've become infused with the twisting energy - they've 
        come to a sort of life.  This is an abomination that festers within Sosaria.  You must eradicate the 
        crystal vortices. */
        public override object Description => 1074670;
        /* These abominations must not be permitted to fester! */
        public override object Refuse => 1074671;
        /* You must not waste time. Do not suffer these crystalline abominations to live. */
        public override object Uncomplete => 1074672;
        /* You have done well.  Enjoy this reward. */
        public override object Complete => 1074673;
        public override bool CanOffer()
        {
            return MondainsLegacy.PrismOfLight;
        }

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

    public class Ryal : MondainQuester
    {
        [Constructable]
        public Ryal()
            : base("Lorekeeper Ryal", "the keeper of tradition")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Ryal(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(DaemonicPrismQuest),
                    typeof(HowManyHeadsQuest),
                    typeof(GlassyFoeQuest),
                    typeof(HailstormQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x82FE;
            HairItemID = 0x2FC2;
            HairHue = 0x324;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x1BB));
            AddItem(new Cloak(0x219));
            AddItem(new LeafTonlet());
            AddItem(new GnarledStaff());
        }

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