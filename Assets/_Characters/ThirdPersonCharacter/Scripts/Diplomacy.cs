using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Diplomacy
{
    [SerializeField]
    private List<Faction> factions = new List<Faction>();

    public void AddFaction(string name)
    {
        Faction newFaction = new Faction
        {
            id = factions.Count + 1,
            name = name,
            trustValue = new List<int>()
        };

        factions.Add(newFaction);

        for (int i = 0; i < factions.Count; i++)
        {
            // add trust value of new faction to all existing factions
            factions[i].trustValue.Add(50);
            // add defauld trust values to new faction
            if (i > 0)
            {
                factions[factions.Count - 1].trustValue.Add(50);
            }
        }
    }

    public Faction GetfactionById(int id)
    {
        return factions[id];
    }

    public Faction GetfactionByName(string name)
    {
        return factions.Find(faction => faction.name == name);

        //return new Faction();
    }

    public int GetTrustValue(int ownId, int otherId)
    {
        if (ownId >= 0 && ownId < factions.Count && otherId >= 0 && otherId < factions.Count)
        {
            Faction ownfaction = GetfactionById(ownId);
            return ownfaction.trustValue[otherId];
        }
        return -1;
    }

    [System.Serializable]
    public struct Faction
    {
        // place info like banner, field of work etc. here
        public int id;
        public string name;
        public List<int> trustValue;
    }
}

