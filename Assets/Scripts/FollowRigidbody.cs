using UnityEngine;

public class FollowRigidbody : MonoBehaviour
{
    public Rigidbody thisBody, targetBody;

    private void FixedUpdate()
    {
        thisBody.Move(targetBody.position, targetBody.rotation);
        thisBody.linearVelocity= Vector3.one;
        thisBody.angularVelocity = Vector3.one;
    }
}