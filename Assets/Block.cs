using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    public int ColorIndex { get; private set; }
    public Vector2Int GridPosition { get; set; }
    
    [SerializeField]private Image image;
    private GridManager gridManager;
    
    
    public void Initialize(int colorIndex, Vector2Int gridPos, GridManager manager)
    {
        ColorIndex = colorIndex;
        GridPosition = gridPos;
        gridManager = manager;
        
        // Set color based on index (you'd assign these in inspector)
        image.sprite = sprites[colorIndex];
    }
    
    private void OnMouseDown()
    {
        if (GameManager.Instance.IsGameActive)
        {
            gridManager.OnBlockSelected(this);
        }
    }
    
    private int GetColorFromIndex(int index)
    {
        int[] colors = {0 ,1, 2, 3, 4};
        return colors[index % colors.Length];
    }
}
