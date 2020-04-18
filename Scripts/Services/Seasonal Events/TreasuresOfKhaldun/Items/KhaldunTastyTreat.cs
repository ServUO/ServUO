using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class KhaldunTastyTreat : Item
    {
        public const int Duration = 1; // hours

        public override int LabelNumber => 1158680;  // khaldun tasty treat

        [Constructable]
        public KhaldunTastyTreat()
            : this(1)
        {
        }

        public KhaldunTastyTreat(int amount)
            : base(2424)
        {
            Stackable = true;
            Amount = amount;
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            TryFeed(from, target);

            return false;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.BeginTarget(2, false, TargetFlags.Beneficial, (from, targeted) =>
                {
                    if (targeted is Mobile)
                    {
                        TryFeed(from, (Mobile)targeted);
                    }
                });
            }
        }

        private void TryFeed(Mobile from, Mobile target)
        {
            if (target is BaseCreature && !((BaseCreature)target).IsDeadBondedPet && ((BaseCreature)target).ControlMaster == from)
            {
                BaseCreature bc = (BaseCreature)target;

                if (UnderInfluence(bc))
                {
                    from.SendLocalizedMessage(1113051); //Your pet is still enjoying the last tasty treat!
                }
                else
                {
                    DoEffects(bc.ControlMaster, bc);
                }
            }
        }

        public bool DoEffects(Mobile owner, BaseCreature bc)
        {
            owner.SendLocalizedMessage(1158685); // Your pet is now Caddellite infused by this treat.

            bc.PlaySound(0x1EA);
            bc.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);

            bc.Loyalty = BaseCreature.MaxLoyalty;

            if (Table == null)
            {
                Table = new Dictionary<BaseCreature, DateTime>();
            }

            Table.Add(bc, DateTime.UtcNow + TimeSpan.FromHours(1));
            Timer.DelayCall(TimeSpan.FromHours(Duration), RemoveInfluence, bc);

            Caddellite.UpdateBuff(owner);

            Consume();
            return true;
        }

        public static Dictionary<BaseCreature, DateTime> Table { get; set; }

        public static void Initialize()
        {
            EventSink.Login += OnLogin;
        }

        public static bool UnderInfluence(BaseCreature bc)
        {
            return Table != null && Table.ContainsKey(bc);
        }

        public static void RemoveInfluence(BaseCreature bc)
        {
            if (Table != null && Table.ContainsKey(bc))
            {
                Table.Remove(bc);

                if (bc.ControlMaster != null)
                {
                    bc.ControlMaster.SendLocalizedMessage(1158687); // Your pet is no longer Caddellite infused.

                    Caddellite.UpdateBuff(bc.ControlMaster);
                }

                if (Table.Count == 0)
                {
                    Table = null;
                }
            }
        }

        public static BaseCreature GetPetUnderEffects(Mobile m)
        {
            if (m is PlayerMobile)
            {
                PlayerMobile pm = m as PlayerMobile;

                foreach (BaseCreature pet in pm.AllFollowers.OfType<BaseCreature>())
                {
                    if (UnderInfluence(pet))
                    {
                        return pet;
                    }
                }
            }

            return null;
        }

        public static void Save(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Table == null ? 0 : Table.Count);

            if (Table != null)
            {
                foreach (KeyValuePair<BaseCreature, DateTime> kpv in Table)
                {
                    writer.Write(kpv.Key);
                    writer.Write(kpv.Value);
                }
            }
        }

        public static void Load(GenericReader reader)
        {
            reader.ReadInt(); // version

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                BaseCreature bc = reader.ReadMobile() as BaseCreature;
                DateTime dt = reader.ReadDateTime();

                if (bc != null && dt > DateTime.UtcNow)
                {
                    if (Table == null)
                        Table = new Dictionary<BaseCreature, DateTime>();

                    Table[bc] = dt;

                    Timer.DelayCall(dt - DateTime.UtcNow, RemoveInfluence, bc);
                }
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                Timer.DelayCall(() => Caddellite.UpdateBuff(pm));
            }
        }

        public KhaldunTastyTreat(Serial serial)
            : base(serial)
        {
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
