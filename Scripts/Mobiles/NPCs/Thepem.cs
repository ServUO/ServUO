using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Thepem : MondainQuester
    {
        [Constructable]
        public Thepem()
            : base("Thepem", "the Apprentice")
        {
        }

        public Thepem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[]
                {
                    typeof(AllThatGlitters)
                };
            }
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = true;
            this.Race = Race.Gargoyle;

            this.Hue = 0x86EA;
            this.HairItemID = 0x4273;
            this.HairHue = 0x323;
            this.Body = 666;
            this.Blessed = true;
        }

        public override void InitOutfit()
        {
            this.AddItem(new FemaleGargishClothLegs(0x736));
            this.AddItem(new FemaleGargishClothKilt(0x73D));
            this.AddItem(new FemaleGargishClothChest(0x38B));
            this.AddItem(new FemaleGargishClothArms(0x711));
        }

        private static Type[][] m_PileTypes = new Type[][]
            {
                new Type[] {typeof( DullCopperIngot ),  typeof( PileOfInspectedDullCopperIngots )},
                new Type[] {typeof( ShadowIronIngot ),  typeof( PileOfInspectedShadowIronIngots )},
                new Type[] {typeof( CopperIngot ),      typeof( PileOfInspectedCopperIngots )},
                new Type[] {typeof( BronzeIngot ),      typeof( PileOfInspectedBronzeIngots )},
                new Type[] {typeof( GoldIngot ),        typeof( PileOfInspectedGoldIngots )},
                new Type[] {typeof( AgapiteIngot ),     typeof( PileOfInspectedAgapiteIngots )}
            };

        private const int NeededIngots = 20;

        private Type GetPileType(Item item)
        {
            Type itemType = item.GetType();

            for (int i = 0; i < m_PileTypes.Length; i++)
            {
                Type[] pair = m_PileTypes[i];

                if (itemType == pair[0])
                    return pair[1];
            }

            return null;
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            Type pileType = GetPileType(item);

            if (pileType != null)
            {
                if (item.Amount > NeededIngots)
                {
                    SayTo(from, 1113037); // That's too many.
                    return false;
                }
                else if (item.Amount < NeededIngots)
                {
                    SayTo(from, 1113036); // That's not enough.
                    return false;
                }
                else
                {
                    SayTo(from, 1113040); // Good. I can use this.

                    from.AddToBackpack(Activator.CreateInstance(pileType) as Item);
                    from.SendLocalizedMessage(1113041); // Now mark the inspected item as a quest item to turn it in.					

                    return true;
                }
            }
            else
            {
                SayTo(from, 1113035); // Oooh, shiney. I have no use for this, though.
                return false;
            }
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
}