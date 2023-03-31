using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField]
    private float mouseDragPhysicsSpeed = 10;
    [SerializeField]
    private float mouseDragSpeed = 10f;

    private Camera mainCamera;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    public GameObject sphere;
    public GameManager gameManager;
    public Snake _snake;


    private void Awake()
    {
        mainCamera = Camera.main;
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
                Vector3 direction = new Vector3(ray.GetPoint(initialDistance).x - clickedObject.transform.position.x - mouseOffset,0, 0f);
                rb.velocity = direction * mouseDragPhysicsSpeed;
                //rb.velocity = (direction.x * mouseDragPhysicsSpeed);
                //transform.Translate(Vector3.right * Time.deltaTime * 100f);
                yield return waitForFixedUpdate;
            }
            else
            {
                clickedObject.transform.position += new Vector3(
                     ray.GetPoint(Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position)).x - clickedObject.transform.position.x - mouseOffset, 0,0);
                yield return null;
            }
        }
        rb.velocity = new Vector3(0,0,0);
    }
}
