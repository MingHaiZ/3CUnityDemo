using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 3f;
    public WayPointManager wayPoints { get; protected set; }


    protected virtual void Awake()
    {
        tag = GameTag.Platform;
        wayPoints = GetComponent<WayPointManager>();
    }

    protected virtual void Update()
    {
        var position = transform.position;
        var target = wayPoints.current.position;
        position = Vector3.MoveTowards(position, target, speed * Time.deltaTime);
        transform.position = position;

        if (Vector3.Distance(transform.position, target) == 0)
        {
            wayPoints.Next();
        }
    }
}