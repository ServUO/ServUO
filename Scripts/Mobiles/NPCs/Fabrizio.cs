using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class GentleBladeQuest : BaseQuest
    {
        public Dagger Dagger { get; set; }

        public GentleBladeQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Aminia), "warewolf", 1, 10800));

            AddReward(new BaseReward(1075363)); // Misericord
        }

        public override TimeSpan RestartDelay => TimeSpan.FromMinutes(3);
        /* Gentle Blade */
        public override object Title => 1075361;
        /* I came to this place looking for a cure for my wife. But I’m getting ahead of myself -- my wife was attacked by a 
        werewolf, and survived. Now she has become a werewolf herself. My research has turned up nothing that would cure her 
        affliction. *Sob* She begged me to end her suffering, but I cannot. She has removed herself to a remote part of Ice 
        Island so that she does not endanger others. If I give you the means, will you go there, find her, and give her the 
        mercy of a clean death?  */
        public override object Description => 1075362;
        /* I understand. I am no warrior, either. I suppose I shall have to wait here until one comes along. */
        public override object Refuse => 1075364;
        /* My wife is hiding out in a cave on the north end of Ice Island. You will not be able to harm here, even with the 
        weapon I gave you, until night falls and she transforms into a wolf. */
        public override object Uncomplete => 1075365;
        /* Thank you my friend . . . I know she is at peace, now. Here, keep the weapon. Most of its power is expended, but 
        it remains somewhat potent against wolf-kind. */
        public override object Complete => 1075366;
        public override void OnAccept()
        {
            Dagger = new Dagger
            {
                QuestItem = true
            };
            Dagger.WeaponAttributes.UseBestSkill = 1;

            if (Owner.PlaceInBackpack(Dagger))
            {
                base.OnAccept();
            }
            else
            {
                Dagger.Delete();
                Owner.SendLocalizedMessage(1075574); // Could not create all the necessary items. Your quest has not advanced.
            }
        }

        public override void GiveRewards()
        {
            base.GiveRewards();

            if (Dagger != null && !Dagger.Deleted && Dagger.RootParent == Owner)
            {
                Dagger.Name = "Misericord";
                Dagger.WeaponAttributes.UseBestSkill = 0;
                Dagger.QuestItem = false;
                Dagger.Slayer3 = TalismanSlayerName.Wolf;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
            writer.WriteItem(Dagger);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version > 0)
            {
                Dagger = reader.ReadItem<Dagger>();
            }
        }
    }

    public class Fabrizio : MondainQuester
    {
        [Constructable]
        public Fabrizio()
            : base("Fabrizio", "the master weaponsmith")
        {
        }

        public Fabrizio(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(GentleBladeQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x840E;
            HairItemID = 0x203D;
            HairHue = 0x1;
            FacialHairItemID = 0x203F;
            FacialHairHue = 0x1;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(0x753));
            AddItem(new LongPants(0x59C));
            AddItem(new HalfApron(0x8FD));
            AddItem(new Tunic(0x58F));
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
