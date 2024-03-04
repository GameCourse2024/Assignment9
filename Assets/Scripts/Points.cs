using Fusion;
using UnityEngine;

public class Points : NetworkBehaviour
{
    [SerializeField] NumberField PointsDisplay;

    [Networked(OnChanged = nameof(NetworkedPointsChanged))]
    public int NetworkedPoints { get; set; } = 0;

    private static void NetworkedPointsChanged(Changed<Points> changed)
    {
        Debug.Log($"Points changed to: {changed.Behaviour.NetworkedPoints}");
        changed.Behaviour.PointsDisplay.SetNumber(changed.Behaviour.NetworkedPoints);
    }

    public void AddPoints(int pointsToAdd)
    {
        Debug.Log("Received points on StateAuthority, modifying Networked variable");
        NetworkedPoints += pointsToAdd;
    }
}
