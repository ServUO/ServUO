using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Zosilem : MondainQuester
    {
        [Constructable]
        public Zosilem()
            : base("Zosilem", "the Alchemist")
        {
        }

        public Zosilem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[]
                {
                    typeof(DabblingontheDarkSide)
                };
            }
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = true;
            this.Race = Race.Gargoyle;
            this.Hue = 0x86E1;
            this.HairItemID = 0x42AB;
            this.HairHue = 0x385;
            this.Body = 666;
            this.Blessed = true;
        }

        public override void InitOutfit()
        {
            this.AddItem(new FemaleGargishClothLegs(0x70F));
            this.AddItem(new FemaleGargishClothKilt(0x742));
            this.AddItem(new FemaleGargishClothChest(0x4C3));
            this.AddItem(new FemaleGargishClothArms(0x738));
        }

        private static Type[][] m_PileTypes = new Type[][]
            {
                new Type[] {typeof( DullCopperIngot ),  typeof( PileOfInspectedDullCopperIngots )},
                new Type[] {typeof( ShadowIronIngot ),  typeof( PileOfInspectedShadowIronIngots )},
                new Type[] {typeof( BronzeIngot ),      typeof( PileOfInspectedBronzeIngots )},
                new Type[] {typeof( GoldIngot ),        typeof( PileOfInspectedGoldIngots )},
                new Type[] {typeof( AgapiteIngot ),     typeof( PileOfInspectedAgapiteIngots )},
                new Type[] {typeof( VeriteIngot ),      typeof( PileOfInspectedVeriteIngots )},
                new Type[] {typeof( ValoriteIngot ),    typeof( PileOfInspectedValoriteIngots )}
            };

        private static object[][] m_KegTypes = new object[][]
            {
                new object[] {PotionEffect.RefreshTotal,  typeof( InspectedKegOfTotalRefreshmentPotions )},
                new object[] {PotionEffect.PoisonGreater, typeof( InspectedKegOfGreaterPoisonPotions )}
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

        private Type GetKegType(PotionEffect effect)
        {
            for (int i = 0; i < m_KegTypes.Length; i++)
            {
                object[] pair = m_KegTypes[i];

                if (effect == (PotionEffect)pair[0])
                    return (Type)pair[1];
            }

            return null;
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            Type inspectedType = null;
            bool success = false;

            inspectedType = GetPileType(item);

            if (inspectedType != null)
            {
                if (item.Amount > NeededIngots)
                    SayTo(from, 1113037); // That's too many.
                else if (item.Amount < NeededIngots)
                    SayTo(from, 1113036); // That's not enough.
                else
                    success = true;
            }
            else if (item is PotionKeg)
            {
                PotionKeg keg = (PotionKeg)item;

                inspectedType = GetKegType(keg.Type);

                if (inspectedType == null)
                    SayTo(from, 1113039); // It is the wrong type.
                else if (keg.Held < 100)
                    SayTo(from, 1113038); // It is not full.
                else
                    success = true;
            }
            else
            {
                SayTo(from, 1113035); // Oooh, shiney. I have no use for this, though.
            }

            if (success)
            {
                SayTo(from, 1113040); // Good. I can use this.

                from.AddToBackpack(Activator.CreateInstance(inspectedType) as Item);
                from.SendLocalizedMessage(1113041); // Now mark the inspected item as a quest item to turn it in.
            }

            return success;
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