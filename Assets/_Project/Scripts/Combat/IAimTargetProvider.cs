using UnityEngine;

namespace TopdownSurvival.Combat
{
    public interface IAimTargetProvider
    {
        Transform Current { get; }

        Vector3 AimDirection { get; }

        bool TryGetTarget(out Transform target);
    }
}
