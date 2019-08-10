using System;

public static class Extensions
{
    public static T RandomItem<T>(this T[] array, Random random)
    {
        if(random == null)
            return array[UnityEngine.Random.Range(0, array.Length)];
        else
            return array[random.Next(0, array.Length)];
    }
}
