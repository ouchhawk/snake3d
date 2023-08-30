using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerControllerMenu : MonoBehaviour
{
    public GameManager gameManager;

    [SerializeField]
    private InputAction mouseClick;

    // Start is called before the first frame update
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

    private void MousePressed(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Minigame");
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




}
