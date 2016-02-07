using System;

namespace Server.Mobiles
{
    public class ServantOfSemidar : BaseCreature
    {
        [Constructable]
        public ServantOfSemidar()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            this.Name = "a servant of Semidar";
            this.Body = 0x26;
        }

        public ServantOfSemidar(Serial serial)
            : base(serial)
        {
        }

        public override bool DisallowAllMoves
        {
            get
            {
                return true;
            }
        }
        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public override bool CanBeDamaged()
        {
            return false;
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1005494); // enslaved
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}