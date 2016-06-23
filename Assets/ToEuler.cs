using UnityEngine;
using System.Collections;

public class ToEuler {

    public static void toEuler(Vector3 axis, float angle, Vector3 euler)
    {
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);
        float t = 1 - c;
        //  if axis is not already normalised then uncomment this
        // double magnitude = Math.sqrt(x*x + y*y + z*z);
        // if (magnitude==0) throw error;
        // x /= magnitude;
        // y /= magnitude;
        // z /= magnitude;
        if ((axis.x * axis.y * t + axis.z * s) > 0.998)
        { // north pole singularity detected
            euler.x = 2 * Mathf.Atan2(axis.x * Mathf.Sin(angle / 2), Mathf.Cos(angle / 2));
            euler.y = Mathf.PI / 2;
            euler.z = 0;
            return;
        }
        if ((axis.x * axis.y * t + axis.z * s) < -0.998)
        { // south pole singularity detected
            euler.x = -2 * Mathf.Atan2(axis.x * Mathf.Sin(angle / 2), Mathf.Cos(angle / 2));
            euler.y = -Mathf.PI / 2;
            euler.z = 0;
            return;
        }
        euler.x = Mathf.Atan2(axis.y * s - axis.x * axis.z * t, 1 - (axis.y * axis.y + axis.z * axis.z) * t);
        euler.y = Mathf.Asin(axis.x * axis.y * t + axis.z * s);
        euler.z = Mathf.Atan2(axis.x * s - axis.y * axis.z * t, 1 - (axis.x * axis.x + axis.z * axis.z) * t);
    }
}
