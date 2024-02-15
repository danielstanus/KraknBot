namespace KraknBot.Models;

public class NPC
{
    public string Name { get; set; }
    public int Id { get; set; }
}

public class NPCItem
{
    public bool Active { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public int AmmoID { get; set; }
}