using UnityEngine;

public class DrawStainGizmos : MonoBehaviour
{
    public float radius = 0.3f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.hotPink;

        foreach (Transform child in transform)
        {
            Gizmos.DrawSphere(new Vector3(child.position.x, child.position.y + 1, child.position.z), radius);
        }
    }
}
