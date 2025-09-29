using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 6;
    public int gridHeight = 5;
    public int colorCount = 5;
    public float cellSize = 1f;
    
    [Header("References")]
    public GameObject blockPrefab;
    public Transform gridParent;
    
    private Block[,] grid;
    private bool isProcessing = false;
    
        private void Start()
    {
        GenerateGrid();
    }
    
    public void GenerateGrid()
    {
        // Clear existing grid
        if (grid != null)
        {
            foreach (var block in grid)
            {
                if (block != null) Destroy(block.gameObject);
            }
        }
        
        grid = new Block[gridWidth, gridHeight];
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                CreateBlock(x, y);
            }
        }
    }
    
    private void CreateBlock(int x, int y)
    {
        Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
        GameObject blockObj = Instantiate(blockPrefab, position, Quaternion.identity, gridParent);
        
        Block block = blockObj.GetComponent<Block>();
        int colorIndex = Random.Range(0, colorCount);
        
        block.Initialize(colorIndex, new Vector2Int(x, y), this);
        grid[x, y] = block;
    }
    
    public void OnBlockSelected(Block selectedBlock)
    {
        if (isProcessing) return;
        
        List<Block> connectedBlocks = FindConnectedBlocks(selectedBlock);
        
        if (connectedBlocks.Count > 0)
        {
            StartCoroutine(ProcessBlockCollection(connectedBlocks));
        }
    }
    
    private List<Block> FindConnectedBlocks(Block startBlock)
    {
        List<Block> connected = new List<Block>();
        HashSet<Block> visited = new HashSet<Block>();
        Queue<Block> toVisit = new Queue<Block>();
        
        int targetColor = startBlock.ColorIndex;
        
        toVisit.Enqueue(startBlock);
        visited.Add(startBlock);
        
        while (toVisit.Count > 0)
        {
            Block current = toVisit.Dequeue();
            connected.Add(current);
            
            // Check adjacent blocks
            CheckAdjacentBlock(current.GridPosition + Vector2Int.up, targetColor, visited, toVisit);
            CheckAdjacentBlock(current.GridPosition + Vector2Int.down, targetColor, visited, toVisit);
            CheckAdjacentBlock(current.GridPosition + Vector2Int.left, targetColor, visited, toVisit);
            CheckAdjacentBlock(current.GridPosition + Vector2Int.right, targetColor, visited, toVisit);
        }
        
        return connected;
    }
    
    private void CheckAdjacentBlock(Vector2Int position, int targetColor, HashSet<Block> visited, Queue<Block> toVisit)
    {
        if (IsPositionValid(position) && grid[position.x, position.y] != null)
        {
            Block adjacent = grid[position.x, position.y];
            if (adjacent.ColorIndex == targetColor && !visited.Contains(adjacent))
            {
                visited.Add(adjacent);
                toVisit.Enqueue(adjacent);
            }
        }
    }
    
    private IEnumerator ProcessBlockCollection(List<Block> blocksToRemove)
    {
        isProcessing = true;
        
        // Use one move
        GameManager.Instance.UseMove();
        
        // Calculate score (1 for 1 block, 2 for 2, etc.)
        int score = blocksToRemove.Count;
        GameManager.Instance.AddScore(score);
        
        // Remove blocks
        foreach (Block block in blocksToRemove)
        {
            Vector2Int pos = block.GridPosition;
            grid[pos.x, pos.y] = null;
            Destroy(block.gameObject);
        }
        
        yield return new WaitForSeconds(1f);
        
        // Make blocks fall
        ApplyGravity();
        
        // Fill empty spaces
        yield return StartCoroutine(FillEmptySpaces());
        
        isProcessing = false;
    }
    
    private void ApplyGravity()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    // Look for the first block above this position
                    for (int aboveY = y + 1; aboveY < gridHeight; aboveY++)
                    {
                        if (grid[x, aboveY] != null)
                        {
                            // Move block down
                            grid[x, y] = grid[x, aboveY];
                            grid[x, aboveY] = null;
                            
                            // Update block's position and grid position
                            grid[x, y].GridPosition = new Vector2Int(x, y);
                            grid[x, y].transform.position = new Vector3(x * cellSize, y * cellSize, 0);
                            break;
                        }
                    }
                }
            }
        }
    }
    
    private IEnumerator FillEmptySpaces()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    CreateBlock(x, y);
                    yield return new WaitForSeconds(0.1f); 
                }
            }
        }
    }
    
    private bool IsPositionValid(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth && position.y >= 0 && position.y < gridHeight;
    }
}

