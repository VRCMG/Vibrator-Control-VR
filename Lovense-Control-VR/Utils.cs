using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lovense_Control_VR
{
    class Utils
    {
        public static int GetLovenseLevel(float value, float min, float max)
        {
            if (value == 0) return 0;
            //its hard to reach a vcalue of 1 on the upper limit. the the lower limit is easier reachable
            float newValue = Lerp(min, max, 0, 20, value);
            int intValue = (int)Math.Round(newValue);

            if (intValue < 0) intValue = 0;
            if (intValue > 20) intValue = 20;

            return intValue;
        }

        //https://forum.unity.com/threads/mapping-or-scaling-values-to-a-new-range.180090/
        public static float Lerp(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
        {

            float OldRange = (OldMax - OldMin);
            float NewRange = (NewMax - NewMin);
            float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

            return (NewValue);
        }
    }
}
