using Server.Engines.Plants;
using Server.Network;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class Hoe : BaseAxe, IUsesRemaining
    {
        public override int LabelNumber => 1150482;  // hoe

        [Constructable]
        public Hoe()
            : base(0xE86)
        {
            Hue = 2524;
            Weight = 11.0;
            UsesRemaining = 50;
            ShowUsesRemaining = true;
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Disarm;
        public override int StrengthReq => 50;
        public override int MinDamage => 12;
        public override int MaxDamage => 16;
        public override float Speed => 3.00f;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;

        public override WeaponAnimation DefAnimation => WeaponAnimation.Slash1H;

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private readonly Hoe m_Hoe;

            public InternalTarget(Hoe hoe)
                : base(2, true, TargetFlags.None)
            {
                m_Hoe = hoe;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!MaginciaPlantSystem.Enabled)
                {
                    from.SendMessage("Magincia plant placement is currently disabled.");
                    return;
                }

                Map map = from.Map;

                if (targeted is LandTarget && map != null)
                {
                    LandTarget lt = (LandTarget)targeted;
                    Region r = Region.Find(lt.Location, map);

                    if (r != null && r.IsPartOf("Magincia") && (lt.Name == "dirt" || lt.Name == "grass"))
                    {
                        if (MaginciaPlantSystem.CanAddPlant(from, lt.Location))
                        {
                            if (!MaginciaPlantSystem.CheckDelay(from))
                            {
                                return;
                            }
                            else if (from.Mounted || from.Flying)
                            {
                                from.SendLocalizedMessage(501864); // You can't mine while riding.
                            }
                            else if (from.IsBodyMod && !from.Body.IsHuman)
                            {
                                from.SendLocalizedMessage(501865); // You can't mine while polymorphed.
                            }
                            else
                            {
                                m_Hoe.UsesRemaining--;

                                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1150492); // You till a small area to plant.                                
                                from.Animate(AnimationType.Attack, 3);

                                MaginciaPlantItem dirt = new MaginciaPlantItem
                                {
                                    Owner = from
                                };
                                dirt.StartTimer();

                                MaginciaPlantSystem.OnPlantPlanted(from, from.Map);

                                Timer.DelayCall(TimeSpan.FromSeconds(.7), new TimerStateCallback(MoveItem_Callback), new object[] { dirt, lt.Location, map });
                            }
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(1150457); // The ground here is not good for gardening.
                    }
                }
            }

            private void MoveItem_Callback(object o)
            {
                object[] objs = o as object[];

                if (objs != null)
                {
                    Item dirt = objs[0] as Item;
                    Point3D p = (Point3D)objs[1];
                    Map map = objs[2] as Map;

                    if (dirt != null)
                        dirt.MoveToWorld(p, map);
                }
            }
        }

        public Hoe(Serial serial)
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
