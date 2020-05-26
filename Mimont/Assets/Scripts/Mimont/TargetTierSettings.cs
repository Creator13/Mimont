using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mimont {
[CreateAssetMenu(fileName = "TargetSettings", menuName = "Mimont/TargetSettings", order = 0)]
public class TargetTierSettings : ScriptableObject {
    [SerializeField] public List<TargetTier> tiers = new List<TargetTier>();
}
}
