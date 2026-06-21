using System;
using UnityEngine;

namespace TacticalPrototype.Settings
{
    [CreateAssetMenu(menuName = "Tactical Prototype/Unit Visual Catalog")]
    public sealed class UnitVisualCatalog : ScriptableObject
    {
        [SerializeField] private UnitVisualCatalogEntry[] entries;

        public GameObject GetVisualPrefab(Team team, UnitType unitType)
        {
            if (entries == null)
            {
                return null;
            }

            foreach (UnitVisualCatalogEntry entry in entries)
            {
                if (entry.Team == team && entry.UnitType == unitType)
                {
                    return entry.Prefab;
                }
            }

            return null;
        }

        public void SetEntries(UnitVisualCatalogEntry[] value)
        {
            entries = value;
        }
    }

    [Serializable]
    public struct UnitVisualCatalogEntry
    {
        public Team Team;
        public UnitType UnitType;
        public GameObject Prefab;

        public UnitVisualCatalogEntry(Team team, UnitType unitType, GameObject prefab)
        {
            Team = team;
            UnitType = unitType;
            Prefab = prefab;
        }
    }
}
