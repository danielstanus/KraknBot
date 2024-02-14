namespace KraknBot.Models;

public class NPCItem
{
    public bool Active { get; set; }
    public string Name { get; set; }
    public int AmmoIndex { get; set; } // Index in ammoOptions array
}