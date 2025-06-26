using System;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Space space;
    public Vector3 Eulars = new Vector3(0, -180, 0);

    private void LateUpdate()
    {
        transform.Rotate(Eulars * Time.deltaTime, space);
    }
}