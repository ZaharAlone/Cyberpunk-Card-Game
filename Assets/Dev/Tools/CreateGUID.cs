namespace CyberNet.Tools
{
    public static class CreateGUID
    {
        public static string Create()
        {
            return System.Guid.NewGuid().ToString();
        }
    }
}