using UnityEngine;
using System.Collections.Generic;

public static class RandomDropPositionHelper {
    public static List<Vector3> GetRandomDropPositions(Vector3 center, float radius, int count, float minDistance) {
        List<Vector3> positions = new();
        for (int i = 0; i < count; i++) {
            Vector3 randomPosition;
            bool validPosition;
            int attempts = 0;

            do {
                randomPosition = center + new Vector3(
                    Random.Range(-radius, radius),
                    0,
                    Random.Range(-radius, radius)
                );

                validPosition = true;
                foreach (var pos in positions) {
                    if (Vector3.Distance(randomPosition, pos) < minDistance) {
                        validPosition = false;
                        break;
                    }
                }
                attempts++;
            } while (!validPosition && attempts < 10);

            positions.Add(randomPosition);
        }
        return positions;
    }
}
