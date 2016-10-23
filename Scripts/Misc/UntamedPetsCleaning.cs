using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class UntamedPetsCleaning
    {
        #region Untamed pets cleaning

        public static void Initialize()
        {
            CleanUntamedPets();
            Timer.DelayCall(TimeSpan.FromHours(12.0), TimeSpan.FromHours(12.0), new TimerCallback(CleanUntamedPets));
        }

        private static void CleanUntamedPets()
        {
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is BaseCreature)
                {
                    BaseCreature bc = m as BaseCreature;

                    if (bc.RemoveOnSave && !bc.Controlled && bc.ControlMaster == null)
                        list.Add(bc);
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Delete();
            }
        }
        #endregion
    }
}
