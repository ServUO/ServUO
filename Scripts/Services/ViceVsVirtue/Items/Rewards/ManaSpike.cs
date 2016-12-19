using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
	public class ManaSpike : Item
	{
        public override int LabelNumber { get { return 1155508; } }

        [Constructable]
		public ManaSpike() : base(2308)
		{
		}
		
		public override void OnDoubleClick(Mobile m)
		{
			if(IsChildOf(m.Backpack))
			{
				VvVBattle battle = ViceVsVirtueSystem.Instance.Battle;
				
				if(!ViceVsVirtueSystem.IsVvV(m))
				{
					m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
				}
                else if (battle == null || battle.Region == null || !battle.OnGoing || !battle.IsInActiveBattle(m))
				{
					m.SendLocalizedMessage(1155406); // This item can only be used in an active VvV battle region!
				}
				else if(battle.NextManaSpike > DateTime.UtcNow)
				{
					m.SendLocalizedMessage(1155497); // The ground is too charged to use a mana spike!
				}
				else if (m.Mana < 50)
				{
                    m.SendLocalizedMessage(1155509); // You lack the mana required to use this.
				}
				else
				{
                    m.FixedParticles(0x37C4, 1, 8, 9916, 39, 3, EffectLayer.CenterFeet);
                    m.FixedParticles(0x37C4, 1, 8, 9502, 39, 4, EffectLayer.CenterFeet);
                    m.PlaySound(0x210);
					m.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1155499, m.NetState); // *You drive the spike into the ground!*
					
					Timer.DelayCall(TimeSpan.FromMilliseconds(250), () =>
					{
						if (m.Mana < 50) // Another mana check!
						{
                            m.SendLocalizedMessage(1155509); // You lack the mana required to use this.
							return;
						}

                        m.Mana = 0;

						battle.NextManaSpike = DateTime.UtcNow + TimeSpan.FromMinutes(5);
						battle.ManaSpikeEndEffects = DateTime.UtcNow + TimeSpan.FromMinutes(2); // TODO: Duration?

                        foreach (Mobile mobile in battle.Region.GetEnumeratedMobiles())
						{
                            if (ViceVsVirtueSystem.IsEnemy(mobile, m))
                            {
                                mobile.RevealingAction();

                                mobile.BoltEffect(0);
                                AOS.Damage(mobile, m, Utility.RandomMinMax(50, 75), 0, 0, 0, 0, 100);

                                mobile.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1155498, mobile.NetState); // *Your body convulses as energy surges through it!*
                            }
						}

                        Delete();
					});
				}
			}
			else
			{
				m.SendLocalizedMessage(1042004); // That must be in your pack for you to use it.
			}
		}

        public static bool UnderEffects(Mobile m)
        {
            VvVBattle battle = ViceVsVirtueSystem.Instance.Battle;

            if (battle != null && battle.OnGoing && battle.ManaSpikeEndEffects != DateTime.MinValue && battle.ManaSpikeEndEffects > DateTime.UtcNow)
            {
                m.PrivateOverheadMessage(Network.MessageType.Regular, 1154, 1155500, m.NetState); // *Your body is too charged with electrical energy to hide!*
                return true;
            }

            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1154937); // vvv item
        }

		public ManaSpike(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}