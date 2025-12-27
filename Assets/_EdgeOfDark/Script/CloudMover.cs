using UnityEngine;

public class CloudMover : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveDirection = Vector3.right;
    public float speed = 0.5f;

    [Header("Loop Settings")]
    public float loopDistance = 200f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);

        float distance = Vector3.Distance(startPosition, transform.position);
        if (distance >= loopDistance)
        {
            transform.position = startPosition;
        }
    }
}
