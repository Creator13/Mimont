using System;
using UnityEngine;

namespace Mimont.Gameplay {
public interface IPlayer {
    event Action<Vector3> RingCreated;
    event Action RingReleased;

    void StartOtherRing(Vector3 position);
    void ReleaseOtherRing();

    void AddTarget(Vector3 position, int tierIndex);
}
}
