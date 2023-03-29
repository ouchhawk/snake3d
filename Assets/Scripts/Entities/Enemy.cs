using UnityEngine;
using Color = UnityEngine.Color;

public abstract class Enemy : MonoBehaviour
{

    [SerializeField] private GameObject shapeObject;
    [SerializeField] private TMPro.TextMeshProUGUI label;

    private GameManager _gameManager;
    private int size;

    public int Size { get => size; set => size = value; }
    private Color Color { get; set; }


    public Enemy(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public abstract void SetColor(Color color);
    public abstract void DecreaseSize();
    public void UpdateLabel()
    {
        label.text = Size.ToString();
    }
}
