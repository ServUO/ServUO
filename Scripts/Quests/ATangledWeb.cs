using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class ATangledWeb : BaseQuest
    {
        public ATangledWeb()
            : base()
        {
            AddObjective(new BloodCreaturesObjective(typeof(IBloodCreature), "blood creatures", 12));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /*A Tangled Web*/
        public override object Title => 1095032;
        /*Kill Bloodworms and Blood Elementals to fill Jaacar's barrel.
        Return to Jaacar with the filled barrel for your reward
        Will friend help Jaacar with small errand for big friend?  
        Jaacar need big barrel full of blood.  Can friend do that?  
        Best place to get blood is blood elementals and bloodworms nearby.  
        If you do, Jaacar give to you special present!  More special than favorite recipe!*/
        public override object Description => 1095034;
        /*Filling barrel not gross!  Filling barrel helps friend!  You think and then come back and help.  Yes, friend is big help!*/
        public override object Refuse => 1095035;
        /*Jaacar need barrel filled all the way to the top!  Good friend, go fill the barrel for Jaacar.*/
        public override object Uncomplete => 1095036;
        public override void OnCompleted()
        {
            Owner.SendLocalizedMessage(1095038, null, 0x23); // Jaacar's barrel is completely full. Return to Jaacar for your reward.							
            Owner.PlaySound(CompleteSound);
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

        private class BloodCreaturesObjective : SlayObjective
        {
            public BloodCreaturesObjective(Type creature, string name, int amount)
                : base(creature, name, amount)
            {
            }

            public override void OnKill(Mobile killed)
            {
                base.OnKill(killed);

                if (!Completed)
                    Quest.Owner.SendLocalizedMessage(1095037); // Blood from the creature goes into Jaacar’s barrel.
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
}