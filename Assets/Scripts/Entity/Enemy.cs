using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

public abstract class Enemy : MonoBehaviour
{

    [SerializeField] 
    protected GameObject shapeObject;
    [SerializeField]
    private TMPro.TextMeshProUGUI label;
    private GameManager gameManager;
    private int size;
    private Color color;

    public TextMeshProUGUI Label { get => label; set => label = value; }
    public GameManager GameManager { get => gameManager; set => gameManager = value; }
    public int Size { get => size; set => size = value; }
    public Color Color { get => color; set => color = value; }

    public abstract void UpdateColor();
    public abstract void DecreaseSize();

    private void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void UpdateLabel()
    {
        Label.text = Size.ToString();
    }
}
