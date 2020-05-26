using UnityEngine;

namespace Mimont {
public interface ISphere {
    float Radius { get; }
    Vector3 Position { get; }
}

public static class SphereExtensions {
    public static bool IsInside(this ISphere self, ISphere sphere) {
        var distance = Vector3.Distance(self.Position, sphere.Position);
        return distance + self.Radius < sphere.Radius;
    }
}
}
