using System;
using Server.Items;

namespace Server.Spells.First
{
    public class CreateFoodSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Create Food", "In Mani Ylem",
            224,
            9011,
            Reagent.Garlic,
            Reagent.Ginseng,
            Reagent.MandrakeRoot);
        private static readonly FoodInfo[] m_Food = new FoodInfo[]
        {
            new FoodInfo(typeof(Grapes), "a grape bunch"),
            new FoodInfo(typeof(Ham), "a ham"),
            new FoodInfo(typeof(CheeseWedge), "a wedge of cheese"),
            new FoodInfo(typeof(Muffins), "muffins"),
            new FoodInfo(typeof(FishSteak), "a fish steak"),
            new FoodInfo(typeof(Ribs), "cut of ribs"),
            new FoodInfo(typeof(CookedBird), "a cooked bird"),
            new FoodInfo(typeof(Sausage), "sausage"),
            new FoodInfo(typeof(Apple), "an apple"),
            new FoodInfo(typeof(Peach), "a peach")
        };
        public CreateFoodSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.First;
            }
        }
        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                FoodInfo foodInfo = m_Food[Utility.Random(m_Food.Length)];
                Item food = foodInfo.Create();

                if (food != null)
                {
                    this.Caster.AddToBackpack(food);

                    // You magically create food in your backpack:
                    this.Caster.SendLocalizedMessage(1042695, true, " " + foodInfo.Name);

                    this.Caster.FixedParticles(0, 10, 5, 2003, EffectLayer.RightHand);
                    this.Caster.PlaySound(0x1E2);
                }
            }

            this.FinishSequence();
        }
    }

    public class FoodInfo
    {
        private Type m_Type;
        private string m_Name;
        public FoodInfo(Type type, string name)
        {
            this.m_Type = type;
            this.m_Name = name;
        }

        public Type Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        public Item Create()
        {
            Item item;

            try
            {
                item = (Item)Activator.CreateInstance(this.m_Type);
            }
            catch
            {
                item = null;
            }

            return item;
        }
    }
}