using UnityEngine;

public class FollowRigidbody : MonoBehaviour
{
    public Rigidbody thisBody, targetBody;

    private void FixedUpdate()
    {
        thisBody.Move(targetBody.position, targetBody.rotation);
    }
}