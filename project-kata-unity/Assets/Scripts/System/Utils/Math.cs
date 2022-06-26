using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly.Utils
{
    public static class Math
    {
        public static bool IsZero(float f) => Mathf.Abs(f) < Mathf.Epsilon;
        public static bool IsNotZero(float f) => !IsZero(f);
    }
}
