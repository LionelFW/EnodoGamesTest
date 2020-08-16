using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Linq;

namespace Utils
{
    public static class Archetypes
    {
        public static EntityArchetype waypointArchetype;
        public static EntityArchetype vehicleArchetype;
        public static EntityArchetype spawnArchetype;
    }

    public static class UniqueID
    {
        public static List<int> takenIds;

        public static int GenerateNewId()
        {
            bool idIsUnique = false;
            int newId = -1;
            do
            {
                newId = Random.Range(1, int.MaxValue);
                if (takenIds.Count(x => x == newId) == 0) idIsUnique = true;
                takenIds.Add(newId);

            } while (!idIsUnique && newId < 0);
            return newId;
        }
    }
}
