using System;
using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField]
    private float mouseDragPhysicsSpeed = 75f;
    [SerializeField]
    private float mouseDragSpeed = 10f;
    private Vector3 speed = new Vector3(0f, 0f, 30f);
    private float snakeSpeed = 5f;

    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    public GameObject sphere;
    public GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        mainCamera =Camera.main;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
        gameManager.snakeHead.transform.GetComponent<Rigidbody>().velocity = speed;
        moveNodes();
    }

    private void Start()
    {
        sphere = gameManager.nodeList[0];
    }

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
    }

    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }


    private void MousePressed(InputAction.CallbackContext context)
    {
        StartCoroutine(DragUpdate(sphere));
    }

    private IEnumerator DragUpdate(GameObject clickedObject)
    {
        Ray ray2 = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);
        float mouseOffset = ray2.GetPoint(initialDistance).x - clickedObject.transform.position.x;

        clickedObject.TryGetComponent<Rigidbody>(out var rb);
        while (mouseClick.ReadValue<float>() != 0)
        {
            
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (rb != null)
            {
                Vector3 direction = new Vector3(ray.GetPoint(initialDistance).x - clickedObject.transform.position.x - mouseOffset, 0, 0f);
                //rb.velocity = direction * mouseDragPhysicsSpeed;
                speed.x = (direction * mouseDragPhysicsSpeed).x;

               
                yield return waitForFixedUpdate;
            }
            else
            {
                //rb.velocity = new Vector3(0f, 0f, 30f);
                //speed.x = 0f;

                clickedObject.transform.position += new Vector3(
                     ray.GetPoint(Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position)).x - clickedObject.transform.position.x - mouseOffset, 0,0);
                yield return null;
            }
            //moveNodes();
            Debug.Log("X: " + speed.x + ", Z: " + speed.z +", Mag: " + speed.magnitude );
            speed.x = 0f;
        }

        
        

    }

    private void moveNodes()
    {
        gameManager.nodeList[0].transform.position = gameManager.snakeHeadPosition.transform.position;
        //gameManager.snakeHeadPosition.transform.position += new Vector3(0f,0f,5f) * Time.deltaTime;

        int nc = gameManager.nodeList.Count;
        for (int i = 0; i <nc-1; i++)
        {
            //Vector3 newVector = new Vector3(gameManager.nodeList[i].transform.position.x - gameManager.nodeList[i+1].transform.position.x,
            //    gameManager.nodeList[i].transform.position.y - gameManager.nodeList[i+1].transform.position.y,
            //    gameManager.nodeList[i].transform.position.z - gameManager.nodeList[i+1].transform.position.z);

            Vector3 newVector = new Vector3(gameManager.nodeList[i].transform.position.x - gameManager.nodeList[i].GetComponent<Node>().nextTarget.x,
                gameManager.nodeList[i].transform.position.y - gameManager.nodeList[i].GetComponent<Node>().nextTarget.y,
                gameManager.nodeList[i].transform.position.z - gameManager.nodeList[i].GetComponent<Node>().nextTarget.z);

            //newVector = new Vector3(newVector.x, newVector.y, newVector.z);
            //Debug.Log(newVector.magnitude);
            if (newVector.magnitude > 2.5f && speed.magnitude > 2.5f )
            {
                gameManager.nodeList[i + 1].transform.Translate( newVector.normalized * (speed.magnitude) * Time.deltaTime, Space.World);
            }
            else
            {
                gameManager.nodeList[i + 1].transform.Translate(new Vector3(0f, 0f, 2.5f) * Time.deltaTime, Space.World);
            }
        }
        
        //gameManager.nodeList[0].GetComponent<SnakeAnimation>().Animation();
    }
}
