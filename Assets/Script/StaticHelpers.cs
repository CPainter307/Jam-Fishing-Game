using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticHelpers
{
    public static float[] Shift(this float[] myArray)
    {
        float[] tArray = new float[myArray.Length];
        float v = myArray[0];
        System.Array.Copy(myArray, 1, tArray, 0, myArray.Length - 1);
        tArray[tArray.Length - 1] = v;
        return tArray;
    }

    public static Mesh[] ShiftMesh(this Mesh[] myArray)
    {
        Mesh[] tArray = new Mesh[myArray.Length];
        Mesh v = myArray[0];
        System.Array.Copy(myArray, 1, tArray, 0, myArray.Length - 1);
        tArray[tArray.Length - 1] = v;
        return tArray;
    }

    public static GameObject[] ShiftGO(this GameObject[] myArray)
    {
        GameObject[] tArray = new GameObject[myArray.Length];
        GameObject v = myArray[0];
        System.Array.Copy(myArray, 1, tArray, 0, myArray.Length - 1);
        tArray[tArray.Length - 1] = v;
        return tArray;
    }
}
