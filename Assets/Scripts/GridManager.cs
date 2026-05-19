using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int gridSize = 4;
    public GameObject cellPrefab;
    public GameObject[] iconPrefabs;
    public Transform gridParent;
    public float cellSpacing = 10f;

    private Cell[,] gridCells;
    private Cell firstSelectedCell;
    private Cell secondSelectedCell;
    private List<Cell> highlightedCells = new List<Cell>();

    public delegate void TwoCellsSelectedHandler(Cell cell1, Cell cell2);
    public event TwoCellsSelectedHandler OnTwoCellsSelected;

    void Start()
    {
        if (!ValidatePrefabs())
        {
            Debug.LogError("GridManager: 预制体验证失败，请检查配置！");
            return;
        }

        InitializeGrid();
        RandomFillIcons();
        OnTwoCellsSelected += HandleMatchCheck;
    }

    bool ValidatePrefabs()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("GridManager: Cell预制体未赋值！");
            return false;
        }

        if (iconPrefabs == null || iconPrefabs.Length == 0)
        {
            Debug.LogError("GridManager: 图标预制体数组为空！");
            return false;
        }

        for (int i = 0; i < iconPrefabs.Length; i++)
        {
            if (iconPrefabs[i] == null)
            {
                Debug.LogError($"GridManager: 图标预制体[{i}]为空！");
                return false;
            }
        }

        if (gridParent == null)
        {
            Debug.LogError("GridManager: GridParent未赋值！");
            return false;
        }

        return true;
    }

    void InitializeGrid()
    {
        gridCells = new Cell[gridSize, gridSize];
        
        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridSize;
            gridLayout.spacing = new Vector2(cellSpacing, cellSpacing);
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
        }

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                GameObject cellObj = Instantiate(cellPrefab, gridParent);
                cellObj.name = $"Cell_{row}_{col}";
                
                Cell cell = cellObj.GetComponent<Cell>();
                if (cell != null)
                {
                    cell.row = row;
                    cell.col = col;
                    cell.OnCellClicked += HandleCellClicked;
                    gridCells[row, col] = cell;
                }
                else
                {
                    Debug.LogError($"GridManager: Cell预制体缺少Cell组件！ {cellObj.name}");
                }
            }
        }
    }

    void RandomFillIcons()
    {
        ClearAllSelections();
        
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Cell cell = gridCells[row, col];
                if (cell != null)
                {
                    cell.ClearIcon();
                    
                    int randomIndex = Random.Range(0, iconPrefabs.Length);
                    if (iconPrefabs[randomIndex] != null)
                    {
                        GameObject iconObj = Instantiate(iconPrefabs[randomIndex]);
                        Icon icon = iconObj.GetComponent<Icon>();
                        
                        if (icon != null)
                        {
                            cell.SetIcon(icon);
                        }
                        else
                        {
                            Debug.LogError($"GridManager: 图标预制体缺少Icon组件！ index:{randomIndex}");
                            Destroy(iconObj);
                        }
                    }
                }
            }
        }
    }

    void HandleCellClicked(Cell clickedCell)
    {
        if (clickedCell == null)
        {
            Debug.LogWarning("GridManager: 点击的单元格为空！");
            return;
        }

        if (clickedCell.currentIcon == null)
        {
            Debug.Log("GridManager: 该单元格没有图标，无法选中！");
            return;
        }

        if (firstSelectedCell == null)
        {
            SelectFirstCell(clickedCell);
        }
        else if (clickedCell == firstSelectedCell)
        {
            DeselectFirstCell();
        }
        else if (secondSelectedCell == null)
        {
            if (IsValidAndAdjacent(firstSelectedCell, clickedCell))
            {
                SelectSecondCell(clickedCell);
            }
            else
            {
                SelectFirstCell(clickedCell);
            }
        }
        else
        {
            ClearAllSelections();
            SelectFirstCell(clickedCell);
        }
    }

    void SelectFirstCell(Cell cell)
    {
        if (cell == null || cell.currentIcon == null) return;
        
        ClearAllSelections();
        firstSelectedCell = cell;
        firstSelectedCell.SetSelected(true);
        HighlightAdjacentCells(cell);
    }

    void DeselectFirstCell()
    {
        if (firstSelectedCell != null)
        {
            firstSelectedCell.SetSelected(false);
            firstSelectedCell = null;
        }
        ClearHighlights();
    }

    void SelectSecondCell(Cell cell)
    {
        if (cell == null || cell.currentIcon == null) return;
        
        secondSelectedCell = cell;
        secondSelectedCell.SetSelected(true);
        ClearHighlights();
        
        OnTwoCellsSelected?.Invoke(firstSelectedCell, secondSelectedCell);
    }

    public void ClearAllSelections()
    {
        if (firstSelectedCell != null)
        {
            firstSelectedCell.SetSelected(false);
            firstSelectedCell = null;
        }
        
        if (secondSelectedCell != null)
        {
            secondSelectedCell.SetSelected(false);
            secondSelectedCell = null;
        }
        
        ClearHighlights();
    }

    void HighlightAdjacentCells(Cell centerCell)
    {
        if (centerCell == null) return;
        
        ClearHighlights();
        
        int[] rowOffsets = { -1, 1, 0, 0 };
        int[] colOffsets = { 0, 0, -1, 1 };
        
        for (int i = 0; i < 4; i++)
        {
            int newRow = centerCell.row + rowOffsets[i];
            int newCol = centerCell.col + colOffsets[i];
            
            if (IsValidCell(newRow, newCol))
            {
                Cell cell = gridCells[newRow, newCol];
                if (cell != null && cell.currentIcon != null)
                {
                    cell.SetHighlighted(true);
                    highlightedCells.Add(cell);
                }
            }
        }
    }

    void ClearHighlights()
    {
        foreach (Cell cell in highlightedCells)
        {
            if (cell != null)
            {
                cell.SetHighlighted(false);
            }
        }
        highlightedCells.Clear();
    }

    bool IsValidAndAdjacent(Cell cell1, Cell cell2)
    {
        if (cell1 == null || cell2 == null)
        {
            return false;
        }

        if (cell1.currentIcon == null || cell2.currentIcon == null)
        {
            return false;
        }

        return IsAdjacent(cell1, cell2);
    }

    bool IsAdjacent(Cell cell1, Cell cell2)
    {
        if (cell1 == null || cell2 == null) return false;
        
        int rowDiff = Mathf.Abs(cell1.row - cell2.row);
        int colDiff = Mathf.Abs(cell1.col - cell2.col);
        
        return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
    }

    bool IsValidCell(int row, int col)
    {
        return row >= 0 && row < gridSize && col >= 0 && col < gridSize;
    }

    void HandleMatchCheck(Cell cell1, Cell cell2)
    {
        if (cell1 == null || cell2 == null)
        {
            Debug.LogWarning("GridManager: 选中的单元格为空，无法进行匹配检查！");
            ClearAllSelections();
            return;
        }

        if (cell1.currentIcon == null || cell2.currentIcon == null)
        {
            Debug.LogWarning("GridManager: 选中的单元格没有图标，无法进行匹配检查！");
            ClearAllSelections();
            return;
        }

        if (IsSameIconType(cell1, cell2))
        {
            Debug.Log($"匹配成功！消除两个 {cell1.currentIcon.iconType} 图标");
            RemoveMatchedIcons(cell1, cell2);
            
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddMatchScore();
            }
            
            ProcessColumnFall(cell1.col);
            ProcessColumnFall(cell2.col);
        }
        else
        {
            Debug.Log($"匹配失败！{cell1.currentIcon.iconType} ≠ {cell2.currentIcon.iconType}");
        }
        
        ClearAllSelections();
    }

    void ProcessColumnFall(int col)
    {
        if (!IsValidCell(0, col)) return;

        Debug.Log($"处理第 {col} 列的图标下落");

        for (int row = gridSize - 1; row >= 0; row--)
        {
            Cell currentCell = gridCells[row, col];
            
            if (currentCell != null && currentCell.currentIcon == null)
            {
                Cell upperCell = FindUpperCellWithIcon(row, col);
                
                if (upperCell != null)
                {
                    MoveIconDown(upperCell, currentCell);
                }
                else
                {
                    SpawnNewIconAtTop(row, col);
                }
            }
        }
    }

    Cell FindUpperCellWithIcon(int startRow, int col)
    {
        for (int row = startRow - 1; row >= 0; row--)
        {
            Cell cell = gridCells[row, col];
            if (cell != null && cell.currentIcon != null)
            {
                return cell;
            }
        }
        return null;
    }

    void MoveIconDown(Cell fromCell, Cell toCell)
    {
        if (fromCell == null || toCell == null) return;
        if (fromCell.currentIcon == null) return;

        Icon icon = fromCell.currentIcon;
        fromCell.currentIcon = null;
        toCell.SetIcon(icon);
        
        Debug.Log($"图标从 ({fromCell.row}, {fromCell.col}) 下落到 ({toCell.row}, {toCell.col})");
    }

    void SpawnNewIconAtTop(int row, int col)
    {
        if (!IsValidCell(row, col)) return;

        Cell cell = gridCells[row, col];
        if (cell == null) return;

        int randomIndex = Random.Range(0, iconPrefabs.Length);
        if (iconPrefabs[randomIndex] != null)
        {
            GameObject iconObj = Instantiate(iconPrefabs[randomIndex]);
            Icon icon = iconObj.GetComponent<Icon>();
            
            if (icon != null)
            {
                cell.SetIcon(icon);
                Debug.Log($"在 ({row}, {col}) 生成新图标: {icon.iconType}");
            }
            else
            {
                Destroy(iconObj);
            }
        }
    }

    bool IsSameIconType(Cell cell1, Cell cell2)
    {
        if (cell1 == null || cell2 == null)
        {
            return false;
        }

        if (cell1.currentIcon == null || cell2.currentIcon == null)
        {
            return false;
        }

        return cell1.currentIcon.iconType == cell2.currentIcon.iconType;
    }

    void RemoveMatchedIcons(Cell cell1, Cell cell2)
    {
        if (cell1 != null)
        {
            cell1.ClearIcon();
        }
        if (cell2 != null)
        {
            cell2.ClearIcon();
        }
    }

    public void RefreshGrid()
    {
        RandomFillIcons();
    }

    public Cell GetFirstSelectedCell()
    {
        return firstSelectedCell;
    }

    public Cell GetSecondSelectedCell()
    {
        return secondSelectedCell;
    }

    void OnDestroy()
    {
        OnTwoCellsSelected -= HandleMatchCheck;
        
        if (gridCells != null)
        {
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    Cell cell = gridCells[row, col];
                    if (cell != null)
                    {
                        cell.OnCellClicked -= HandleCellClicked;
                    }
                }
            }
        }
    }
}
