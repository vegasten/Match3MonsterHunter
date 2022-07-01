using System.Collections.Generic;

public static class LootMaster
{
    private enum Loot
    {
        BasicScroll
    };

    public static List<ILoot> GetLootForVictory()
    {
        return GetRandomLoot();
    }    

    private static List<ILoot> GetRandomLoot()
    {
        var loot = new List<ILoot>();

        loot.Add(new BasicScroll());

        return loot;
    }
}
