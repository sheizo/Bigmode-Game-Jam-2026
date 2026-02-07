using UnityEngine;

public class RotateWithAxisLock : MonoBehaviour
{
    public Vector3 wobbleAmount = new Vector3(20f, 20f, 0f); // max degrees per axis
    public float speed = 2f; // how fast it wobbles
    public bool lockX, lockY, lockZ; // axis locks

    private Vector3 startRotation;

    void Start()
    {
        startRotation = transform.eulerAngles;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * speed); // goes -1 to 1

        Vector3 wobble = new Vector3(
            lockX ? 0 : wobbleAmount.x * t,
            lockY ? 0 : wobbleAmount.y * t,
            lockZ ? 0 : wobbleAmount.z * t
        );

        transform.eulerAngles = startRotation + wobble;
    }
}
