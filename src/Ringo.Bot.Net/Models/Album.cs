namespace RingoBotNet.Models
{
    public class Album : Item
    {
        public Album() { }

        public Artist[] Artists { get; set; }
    }
}
