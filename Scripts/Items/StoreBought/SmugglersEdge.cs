using System;
using Server;

namespace Server.Items
{
    public class SmugglersEdge : ButcherKnife
    {
        public override int LabelNumber { get { return 1071499; } } // Smuggler's Edge

        [Constructable]
        public SmugglersEdge()
        {
        }

        public SmugglersEdge(Serial serial)
            : base(serial)
        {
        }

        public override void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            base.OnHit(attacker, damageable, damageBonus);

            if (damageable is BaseCreature)
            {
                if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
                {
                    attacker.SendLocalizedMessage(1071501); // Your left hand must be free to steal an item from the creature.
                }
                else
                {
                    Server.Engines.CreatureStealing.HandleSteal((BaseCreature)damageable, attacker, true);
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1071500); // Monster Stealing
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}