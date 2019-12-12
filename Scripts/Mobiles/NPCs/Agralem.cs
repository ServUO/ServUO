using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class IntoTheVoidQuest : BaseQuest
    {
        public IntoTheVoidQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(BaseVoidCreature), "Void Daemons", 10));

            AddReward(new BaseReward(typeof(AbyssReaver), 1112694)); // Abyss Reaver
        }

        public override bool DoneOnce
        {
            get
            {
                return true;
            }
        }
        public override object Title
        {
            get
            {
                return 1112687;
            }
        }
        public override object Description
        {
            get
            {
                return 1112690;
            }
        }
        public override object Refuse
        {
            get
            {
                return 1112691;
            }
        }
        public override object Uncomplete
        {
            get
            {
                return 1112692;
            }
        }
        public override object Complete
        {
            get
            {
                return 1112693;
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

    public class Agralem : MondainQuester
    {
        [Constructable]
        public Agralem()
            : base("Agralem", "the Bladeweaver")
        {
            SetSkill(SkillName.Anatomy, 65.0, 90.0);
            SetSkill(SkillName.MagicResist, 65.0, 90.0);
            SetSkill(SkillName.Tactics, 65.0, 90.0);
            SetSkill(SkillName.Throwing, 65.0, 90.0);
        }

        public Agralem(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1112688); // Daemons from the void! They must be vanquished!
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(IntoTheVoidQuest)
                };
            }
        }
        public override void InitBody()
        {
            InitStats(100, 100, 25);
            
            CantWalk = true;
            Race = Race.Gargoyle;

            Hue = 34536;
            HairItemID = 0x425D;
            HairHue = 0x31D;
        }

        public override void InitOutfit()
        {
            AddItem(new Cyclone());
            AddItem(new GargishLeatherKilt(2305));
            AddItem(new GargishLeatherChest(2305));
            AddItem(new GargishLeatherArms(2305));
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
