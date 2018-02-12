using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Factions
{
    public class ShrineGem : Item, IFactionItem
    {
        public static void Initialize()
        {
            EventSink.PlayerDeath += OnPlayerDeath;
        }

        public static void OnPlayerDeath(PlayerDeathEventArgs e)
        {
            if (e.Mobile is PlayerMobile && e.Mobile.Backpack != null)
            {
                var state = PlayerState.Find(e.Mobile);

                if(state != null)
                {
                    ShrineGem gem = null;

                    foreach (var item in e.Mobile.Backpack.Items)
                    {
                        if (item is ShrineGem && ((ShrineGem)item).FactionItemState.Faction == state.Faction)
                        {
                            gem = (ShrineGem)item;
                            break;
                        }
                    }
                    Console.WriteLine("Gem: {0}", gem);
                    if (gem != null)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(2.5), () =>
                            {
                                BaseGump.SendGump(new ConfirmCallbackGump((PlayerMobile)e.Mobile, 1094715, 1094716, gem, null,
                                    (m, obj) =>
                                    {
                                        ShrineGem g = obj as ShrineGem;

                                        if (g != null && !g.Deleted && g.IsChildOf(m.Backpack))
                                        {
                                            Point3D p = _ShrineLocs[Utility.Random(_ShrineLocs.Length)];

                                            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                                                {
                                                    m.PlaySound(0x1FC);
                                                    m.MoveToWorld(p, m.Map);
                                                    m.PlaySound(0x1FC);

                                                    g.Delete();
                                                });
                                        }
                                    }));
                            });
                    }
                }
            }
        }

        private static readonly Point3D[] _ShrineLocs = new Point3D[]
        {
            new Point3D(1470, 843, 0),
            new Point3D(1857, 865, -1),
            new Point3D(4220, 563, 36),
            new Point3D(1732, 3528, 0),
            new Point3D(1300, 644, 8),
            new Point3D(3355, 302, 9),
            new Point3D(1606, 2490, 5),
            new Point3D(2500, 3931, 3),
            new Point3D(4264, 3707, 0)
        };

        public override int LabelNumber { get { return 1094711; } } // Shrine Gem

        #region Factions
        private FactionItem m_FactionState;

        public FactionItem FactionItemState
        {
            get { return m_FactionState; }
            set
            {
                m_FactionState = value;

                Hue = m_FactionState != null ? m_FactionState.Faction.Definition.HuePrimary : 0;
            }
        }
        #endregion

        public ShrineGem()
            : base(0x1EA7)
        {
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            FactionEquipment.AddFactionProperties(this, list);
        }

        public ShrineGem(Serial serial)
            : base(serial)
        {
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