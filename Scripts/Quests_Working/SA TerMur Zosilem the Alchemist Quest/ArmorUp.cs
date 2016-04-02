using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class ArmorUp : BaseQuest
    { 
        public ArmorUp()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(BouraSkin), "BouraSkin", 5, 0x11f4));
            this.AddObjective(new ObtainObjective(typeof(LeatherWolfSkin), "Leather Wolf Skin", 10, 0x3189));
            this.AddObjective(new ObtainObjective(typeof(UndamagedIronBeetleScale), "Undamaged IronBeetle Scale", 10, 0x5742));
						
            this.AddReward(new BaseReward(typeof(VialofArmorEssence), 1, "Vial Of Armor Essence"));
        }

        public override Type NextQuest
        {
            get
            {
                return typeof(ToTurnBaseMetalIntoVerite);
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromHours(2);
            }
        }
        public override object Title
        {
            get
            {
                return "Armor Up";
            }
        }
        public override object Description
        {
            get
            {
                return 1112971;
            }
        }
        public override object Refuse
        {
            get
            {
                return "You are Scared from this Task !! Muahahah";
            }
        }
        public override object Uncomplete
        {
            get
            {
                return "I am sorry that you have not accepted!";
            }
        }
        public override object Complete
        {
            get
            {
                return 1112974;
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