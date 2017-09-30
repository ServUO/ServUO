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
            switch (Utility.Random(4))
            {
                case 0:
                    this.AddObjective(new SlayObjective(typeof(Anzuanord),"Anzuanord" , 10));
                    break;
                case 1:
                    this.AddObjective(new SlayObjective(typeof(Vasanord), "Vasanord", 10));
                    break;
                case 2:
                    this.AddObjective(new SlayObjective(typeof(UsagralemBallem), "Usagralem Ballem ", 10));
                    break;
                case 3:
                    this.AddObjective(new SlayObjective(typeof(Anlorzen), "Anlorzen", 10));
                    break;
            }

            this.AddReward(new BaseReward(typeof(AbyssReaver), 1112694));/////Abyss Reaver
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
            Female = false;
            CantWalk = true;
            Race = Race.Gargoyle;

            Hue = 0x86ED;
            HairItemID = 0x425D;
            HairHue = 0x31D;
        }

        public override void InitOutfit()
        {
            AddItem(new Cyclone());
            AddItem(new GargishLeatherKilt(0x901));
            AddItem(new GargishLeatherChest(0x901));
            AddItem(new GargishLeatherArms(0x901));
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