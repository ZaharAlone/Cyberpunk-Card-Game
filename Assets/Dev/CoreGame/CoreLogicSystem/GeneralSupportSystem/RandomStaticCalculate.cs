using System;
using System.Collections.Generic;
using Random = System.Random;

public static class RandomStaticCalculate
{
    public static Random RandomTime()
    {
        var seed = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var random = new Random(seed);
        return random;
    }

    public static Random RandomShift(int value)
    {
        var seed = (int)new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        seed += value;
        var random = new Random(seed);
        return random;
    }
}