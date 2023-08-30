using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField]
    private float mouseDragPhysicsSpeed = 10;
    [SerializeField]
    private float mouseDragSpeed = 10f;
    private Mouse mouse;

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
        mouseClick.performed += OnAnyKey;
    }

    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.performed -= OnAnyKey;
        mouseClick.Disable();
    }

    private void OnAnyKey(InputAction.CallbackContext context)
    {
        if (gameManager.IsGameOver)
        {
            SessionInformation.ResetLevel();
            SessionInformation.ResetSnakeLength();
            SessionInformation.ResetScore();
            gameManager.IsGameOver = false;
            gameManager.IsStageClear = false;
            SceneManager.LoadScene("Minigame");
        }
        else if (gameManager.IsStageClear)
        {
            SceneManager.LoadScene("Minigame");
        }

    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Left mouse button clicked");
            // Do something in response to the left mouse button click
        }
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
                rb.velocity = direction * mouseDragPhysicsSpeed;
                //rb.velocity = (direction.x * mouseDragPhysicsSpeed);
                //transform.Translate(Vector3.right * Time.deltaTime * 100f);
                yield return waitForFixedUpdate;
            }
            else
            {
                clickedObject.transform.position += new Vector3(
                     ray.GetPoint(Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position)).x - clickedObject.transform.position.x - mouseOffset, 0, 0);
                yield return null;
            }
        }
        rb.velocity = new Vector3(0, 0, 0);
    }
}
