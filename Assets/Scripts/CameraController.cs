using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (transform && player)
        {
            transform.position = new Vector3(offset.x, offset.y, player.transform.position.z + offset.z);
        }
        
    }
}
