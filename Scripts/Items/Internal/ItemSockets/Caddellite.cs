using Server.Engines.Craft;
using Server.Engines.Harvest;
using Server.Engines.Khaldun;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class Caddellite : ItemSocket
    {
        public override void GetProperties(ObjectPropertyList list)
        {
            if (Owner != null && !(Owner is ICombatEquipment) && !(Owner is Spellbook))
            {
                list.Add(1158662); // Caddellite Infused
            }
        }

        public static void Initialize()
        {
            EventSink.Login += OnLogin;
        }

        public static bool CheckDamage(Mobile from, Server.DamageType type)
        {
            if (from is BaseCreature)
            {
                if (KhaldunTastyTreat.UnderInfluence((BaseCreature)from))
                {
                    return true;
                }

                return false;
            }
            else if (from.Player)
            {
                Item damager;

                switch (type)
                {
                    case Server.DamageType.Melee:
                    case Server.DamageType.Ranged:
                        damager = from.FindItemOnLayer(Layer.OneHanded);

                        if (damager == null || !damager.HasSocket<Caddellite>())
                        {
                            damager = from.FindItemOnLayer(Layer.TwoHanded);
                        }

                        return damager != null && damager.HasSocket<Caddellite>() && damager is BaseWeapon;
                    default:
                        damager = from.FindItemOnLayer(Layer.OneHanded);

                        if (damager != null && damager.HasSocket<Caddellite>() && damager is Spellbook)
                        {
                            return true;
                        }

                        damager = from.FindItemOnLayer(Layer.Neck);

                        if (damager == null || !damager.HasSocket<Caddellite>())
                        {
                            damager = from.FindItemOnLayer(Layer.Helm);
                        }

                        return damager != null && damager.HasSocket<Caddellite>();
                }
            }

            return false;
        }

        public static bool IsCaddellite(Mobile from, Item item)
        {
            return TreasuresOfKhaldunEvent.Instance.Running && item is ICaddelliteTool && SpellHelper.IsAnyT2A(from.Map, from.Location);
        }

        public static void OnHarvest(Mobile from, Item tool, HarvestSystem system, Item resource)
        {
            if (IsCaddellite(from, tool))
            {
                if (resource != null)
                {
                    resource.AttachSocket(new Caddellite());
                }

                if (0.005 > Utility.RandomDouble())
                {
                    if (from != null)
                    {
                        if (system == Fishing.System)
                        {
                            from.SendLocalizedMessage(1158664); // You discover a meteorite entangled in your line!
                        }
                        else if (system == Mining.System)
                        {
                            from.SendLocalizedMessage(1158663); // You discover a meteorite in the dirt!
                        }
                        else if (system == Lumberjacking.System)
                        {
                            from.SendLocalizedMessage(1158665); // You discover a meteorite in the tree!
                        }

                        from.AddToBackpack(new Meteorite());
                    }
                }
            }
        }

        private static Rectangle2D _CraftRec = new Rectangle2D(6017, 3743, 8, 8);
        private static Rectangle2D _CookingRec = new Rectangle2D(6002, 3779, 7, 2);
        private static Rectangle2D _ScribeRec = new Rectangle2D(5999, 3771, 5, 2);

        public static void CheckWaterSource(Mobile from, BaseBeverage beverage, Item item)
        {
            if (from.Region.IsPartOf<KhaldunCampRegion>())
            {
                beverage.Content = BeverageType.Water;
                beverage.Poison = null;
                beverage.Poisoner = null;

                beverage.Quantity = beverage.MaxQuantity;

                from.SendLocalizedMessage(1010089); // You fill the container with water.

                beverage.AttachSocket(new Caddellite());
            }
        }

        public static void TryInfuse(Mobile from, Item item, CraftSystem system)
        {
            Rectangle2D rec;

            if (system == DefCooking.CraftSystem || system == DefAlchemy.CraftSystem)
            {
                rec = _CookingRec;
            }
            else if (system == DefInscription.CraftSystem)
            {
                rec = _ScribeRec;
            }
            else
            {
                rec = _CraftRec;
            }

            if (rec.Contains(from))
            {
                item.AttachSocket(new Caddellite());
            }
        }

        public static void UpdateBuff(Mobile m)
        {
            BaseCreature pet = KhaldunTastyTreat.GetPetUnderEffects(m);
            Caddellite equipped = null;
            Item item = m.FindItemOnLayer(Layer.TwoHanded);

            if (item == null)
            {
                item = m.FindItemOnLayer(Layer.OneHanded);
            }

            if (item != null)
            {
                equipped = item.GetSocket<Caddellite>();
            }

            if (equipped == null && pet == null)
            {
                BuffInfo.RemoveBuff(m, BuffIcon.CaddelliteInfused);
            }
            else
            {
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.CaddelliteInfused, 1158662, 1158677,
                    string.Format("{0}\t{1}\t{2}\t{3}", equipped != null && equipped.Owner is BaseWeapon ? "100" : "0", equipped != null && equipped.Owner is Spellbook ? "100" : "0", pet != null ? pet.Name : "", pet != null ? "100" : "0")));
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                UpdateBuff(pm);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(Item owner, GenericReader reader)
        {
            base.Deserialize(owner, reader);
            reader.ReadInt(); // version
        }
    }
}
