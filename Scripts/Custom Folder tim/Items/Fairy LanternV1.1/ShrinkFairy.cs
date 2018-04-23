using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server {
    public class FairyShrinkFunctions {
        public static bool FairyShrink (Mobile from, object targ, bool restricted) {
            String errorString = null;
            int errorLocalizedMessage = 0;
            if (from == targ)
                errorString = "You can't shrink yourself!";
            else if (!(targ is BaseCreature))
                errorString = "You can't shrink that!";
            else {
                BaseCreature t = (BaseCreature)targ;

                #region Shrink Restrictions
                if (t.IsDeadPet) {
                    errorString = "You cannot shrink dead animals.";
                } else if (!restricted) {
                    //Don't check anything if not a restricted Shrink
                } else if (t.Summoned) {
                    errorString = "You cannot shrink summoned creatures.";
                } else if (t.Combatant != null && t.InRange(t.Combatant, 12) && t.Map == t.Combatant.Map) {
                    errorString = "You cannot shrink your fairy while it is in combat.";
                } else if (!(t.Controlled && t.ControlMaster == from)) {
                    errorLocalizedMessage = 1042562;	//That is not your pet!
                    errorString = "That is not your pet!";
                } else if ((t is PackLlama || t is PackHorse || t is Beetle) && (t.Backpack != null && t.Backpack.Items.Count > 0)) {
                    errorLocalizedMessage = 1042563; //Unload the pet first
                    errorString = "Unload the pet first";
                }
                #endregion

                if ((errorString == null) && (errorLocalizedMessage == 0)) {
                    //If no errors, proceed.

                    FairyShrinkItem shrunkenPet = new FairyShrinkItem(t);
                    shrunkenPet.Layer = Layer.TwoHanded;

                    if (from != null) {
                        from.SendMessage("You shrink the pet");
                        if (!from.AddToBackpack(shrunkenPet)) {
                            shrunkenPet.MoveToWorld(new Point3D(from.X, from.Y, from.Z), from.Map);
                            from.SendMessage("Your backpack is full so the shrunken pet falls to the ground");
                        }
                    } else {
                        shrunkenPet.MoveToWorld(new Point3D(t.X, t.Y, t.Z), t.Map);  // place shrunken pet at current location
                    }

                    SendAway(t);

                    if (from != null)
                        shrunkenPet.BondOwner = from;


                    return true;

                }
            }

            if (from != null) //if From is NOT null, send error Message.
			{

                if (errorLocalizedMessage != 0) {
                    from.SendLocalizedMessage(errorLocalizedMessage);
                } else {
                    from.SendMessage(errorString);
                }
            }
            return false;

        }

        public static bool FairyShrink (Mobile from, object targ) {
            return FairyShrink(from, targ, true);
        }

        public static bool FairyShrink (object targ) {
            return FairyShrink(null, targ, false);
        }

        private static void SendAway (BaseCreature b) {

            b.Controlled = true;	//To make it so It won't still be a part of a spawner. 
            b.SetControlMaster(null);
            b.SummonMaster = null;
            b.Internalize();

            b.OwnerAbandonTime = DateTime.MinValue;

            b.IsStabled = true;
        }
    }
}