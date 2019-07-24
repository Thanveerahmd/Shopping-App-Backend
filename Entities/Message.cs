namespace pro.backend.Entities
{
    public class Message
    {
        public string to { get; set; }
        public Notification notification { get; set; }
        public object data { get; set; }
    }
}