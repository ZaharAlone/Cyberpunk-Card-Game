namespace CyberNet.Tools
{
    public static class ConverterValue
    {
        public static int ConvertToInt(float value)
        {
            return (int)(value * 100);
        }
        
        public static float ConvertToFloat(int value)
        {
            return (float)value / 100f;
        }
    }
}