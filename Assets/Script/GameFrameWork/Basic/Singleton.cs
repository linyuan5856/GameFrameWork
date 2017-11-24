using UnityEngine;
using System.Collections;

namespace GFW
{
    public class Singleton<CLASS_NAME> where CLASS_NAME : new()
    {
        private static CLASS_NAME __instance = default(CLASS_NAME);

        public static CLASS_NAME Instance
        {
            get
            {
                if (__instance == null)
                {
                    __instance = new CLASS_NAME();
                }
                return __instance;
            }
        }
    }
}