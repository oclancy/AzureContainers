namespace Firmus.Common
{
    public class RedisOptions
    {
        public string IpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 6379;

        public override string ToString()
        {
            return $"{IpAddress}:{Port}";
        }
    }
}