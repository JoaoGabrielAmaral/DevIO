namespace DevIO.Api.Extensions
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int ExpireIn { get; set; }
        public string Emitter { get; set; }
        public string ValidAt { get; set; }
    }
}
