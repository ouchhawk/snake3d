using UnityEngine;

public class Node : MonoBehaviour
{
    public Transform nextTarget;
    public Vector3 previousPosition;

    public Transform NextTarget { get => nextTarget; set => nextTarget = value; }

    void Update()
    {

    }
}