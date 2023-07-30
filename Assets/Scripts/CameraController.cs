using UnityEngine;
public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(offset.x, offset.y, player.transform.position.z + offset.z);
    }
}