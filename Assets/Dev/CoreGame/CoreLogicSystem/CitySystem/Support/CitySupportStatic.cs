using System.Collections.Generic;
using ModulesFramework.Data.Enumerators;
using UnityEngine;
namespace CyberNet.Core.City
{
    public static class CitySupportStatic
    {
        public static Vector3 SelectPosition(BoxCollider collider, Vector3 positions, List<Vector3> positionsOtherItem)
        {
            var y = positions.y;
            var x = collider.size.x / 2;
            var z = collider.size.z / 2;
            var newPos = new Vector3();

            var noDouble = false;
            while (!noDouble)
            {
                newPos = new Vector3(Random.Range(-x, x), y, Random.Range(-z, z));
                newPos.x += collider.center.x + positions.x;
                newPos.z += collider.center.z + positions.z;

                noDouble = CheckDistanceObject(newPos, positionsOtherItem);
            }
            return newPos;
        }

        //check the distance between other objects so as not to plant plants too close
        private static bool CheckDistanceObject(Vector3 positions, List<Vector3> positionsOtherItem)
        {
            if (positionsOtherItem.Count == 0)
                return true;

            var result = true;
            foreach (var item in positionsOtherItem)
                if (Vector3.Distance(item, positions) < 1)
                    result = false;
            return result;
        }
    }
}