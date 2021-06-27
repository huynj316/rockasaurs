using UnityEngine;

public class PhysicsController : MonoBehaviour
{

    public float ShakeForceMultiplier;
    public Rigidbody[] ShakingRigidbodies;

    public void ShakeRigidbodies(Vector3 deviceAcceleration)
    {
        foreach (var rigidbody in ShakingRigidbodies)
        {
            rigidbody.AddForce(deviceAcceleration * ShakeForceMultiplier, ForceMode.Impulse);
        }
    }
}