using UnityEngine;
using System.Collections;
using System;

public class OthelloTableView : MonoBehaviour
{
    [SerializeField]
    private GameObject _cell = null;
    public GameObject Cell
    {
        get
        {
            if (_cell != null) { return _cell; }
            _cell = Resources.Load<GameObject>("Cell/Cell");
            return _cell;
        }
    }

    [SerializeField]
    private GameObject _stone = null;
    public GameObject Stone
    {
        get
        {
            if (_stone != null) { return _stone; }
            _stone = Resources.Load<GameObject>("Stone/Stone");
            return _stone;
        }
    }

    [SerializeField]
    private Material _normalMaterial = null;
    public Material NormalMaterial
    {
        get
        {
            if (_normalMaterial != null) { return _normalMaterial; }
            _normalMaterial = Resources.Load<Material>("Cell/Normal");
            return _normalMaterial;
        }
    }

    [SerializeField]
    private Material _selectedMaterial = null;
    public Material SelectedMaterial
    {
        get
        {
            if (_selectedMaterial != null) { return _selectedMaterial; }
            _selectedMaterial = Resources.Load<Material>("Cell/Selected");
            return _selectedMaterial;
        }
    }

    [SerializeField]
    private Material _placeableMaterial = null;
    public Material PlaceableMaterial
    {
        get
        {
            if (_placeableMaterial != null) { return _placeableMaterial; }
            _placeableMaterial = Resources.Load<Material>("Cell/Placeable");
            return _placeableMaterial;
        }
    }

    private const int Rows = 8;
    private const int Columns = 8;
    private readonly GameObject[,] _cells = new GameObject[Rows, Columns];
    private readonly GameObject[,] _stones = new GameObject[Rows, Columns];

    void Start()
    {
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                _cells[r, c] = CreateCell(r, c);
            }
        }
        UpdateCellState(3, 3, CellState.Black);
        UpdateCellState(3, 4, CellState.White);
        UpdateCellState(4, 3, CellState.White);
        UpdateCellState(4, 4, CellState.Black);

        UpdateSelectedCell(0, 0);
        UpdatePlaceableCells();
    }

    private void UpdatePlaceableCells()
    {
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                var renderer = _cells[r, c].GetComponent<Renderer>();
                renderer.material = IsPlaceable(r, c, _currentPlayer) ?
                    PlaceableMaterial : NormalMaterial;
            }
        }
    }

    private GameObject CreateGameObject(GameObject origin, int r, int c)
    {
        var gameObject = Instantiate(origin);
        gameObject.name = origin.name + "(" + r + "," + c + ")";
        gameObject.transform.Translate(1.05F * c, 0, -1.05F * r);
        gameObject.transform.parent = base.gameObject.transform;
        return gameObject;
    }
    private GameObject CreateCell(int r, int c)
    {
        return CreateGameObject(Cell, r, c);
    }
    private GameObject CreateStone(int r, int c)
    {
        return CreateGameObject(Stone, r, c);
    }

    private int _selectedRow = 0;
    public int SelectedRow
    {
        get { return _selectedRow; }
        set
        {
            if (value < 0) { _selectedRow = 0; }
            else if(value >= Rows) { _selectedRow = Rows - 1; }
            else { _selectedRow = value; }
        }
    }
    private int _selectedColumn = 0;

    public int SelectedColumn
    {
        get { return _selectedColumn; }
        set
        {
            if (value < 0) { _selectedColumn = 0; }
            else if (value >= Columns) { _selectedColumn = Columns - 1; }
            else { _selectedColumn = value; }
        }
    }

    public GameObject SelectedCell
    {
        get { return _cells[SelectedRow, SelectedColumn]; }
    }
    
    private Player _currentPlayer = Player.Black;

    private Player GetOtherPlayer(Player player)
    {
        return player == Player.Black ? Player.White : Player.Black;
    }
    private CellState ToCellState(Player player)
    {
        return player == Player.Black ? CellState.Black : CellState.White;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            { UpdateSelectedCell(SelectedRow - 1, SelectedColumn); }
        if (Input.GetKeyDown(KeyCode.DownArrow))
            { UpdateSelectedCell(SelectedRow + 1, SelectedColumn); }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            { UpdateSelectedCell(SelectedRow, SelectedColumn - 1); }
        if (Input.GetKeyDown(KeyCode.RightArrow))
            { UpdateSelectedCell(SelectedRow, SelectedColumn + 1); }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (IsPlaceable(SelectedRow, SelectedColumn, _currentPlayer))
            {
                UpdateCellState(
                    SelectedRow, SelectedColumn,
                    ToCellState(_currentPlayer));
                _currentPlayer = GetOtherPlayer(_currentPlayer);
                UpdatePlaceableCells();
            }
        }
    }

    private bool IsPlaceable(int row, int column, Player player)
    {
        var state = GetCellState(row, column);
        if (state != CellState.None) { return false; }
        
        return
            IsPlaceableByDirection(row, column, -1, -1, player) || //左上
            IsPlaceableByDirection(row, column, -1, 0, player) || //上
            IsPlaceableByDirection(row, column, -1, 1, player) || //右上
            IsPlaceableByDirection(row, column, 0, -1, player) || //左
            IsPlaceableByDirection(row, column, 0, 1, player) || //右
            IsPlaceableByDirection(row, column, 1, -1, player) || //左下
            IsPlaceableByDirection(row, column, 1, 0, player) || //下
            IsPlaceableByDirection(row, column, 1, 1, player); //右;
    }

    private bool IsPlaceableByDirection(int row, int column, int dr, int dc, Player player)
    {
        var other = ToCellState(GetOtherPlayer(player));

        var nr = row + dr;
        var nc = column + dc;
        if (GetCellState(nr, nc) != other) { return false; }

        for (
            int ir = nr + dr, ic = nc + dc;
            ir >= 0 && ic >= 0 && ir < Rows && ic < Columns;
            ir += dr, ic += dc)
        {
            var s = GetCellState(ir, ic);
            if (s != other) { return s != CellState.None; }
        }

        return false;
    }

    private CellState GetCellState(int row, int column)
    {
        if (row < 0 || row >= Rows || column < 0 || column >= Columns)
        {
            return CellState.None;
        }

        var stone = _stones[row, column];
        return stone == null ? CellState.None
            : stone.transform.localRotation == Quaternion.identity
                ? CellState.Black
            : CellState.White;
    }

    private void UpdateCellState(int row, int column, CellState cellState)
    {
        var stone = _stones[row, column];
        if (stone != null && cellState == CellState.None)
        {
            Destroy(stone);
            return;
        }
        if (stone == null)
        {
            stone = CreateStone(row, column);
            _stones[row, column] = stone;
        }
        stone.transform.rotation =
            cellState == CellState.Black
                ? Quaternion.identity
                : Quaternion.Euler(180, 0, 0);
    }

    private void UpdateSelectedCell(int row, int column)
    {
        SelectedCell.GetComponent<Renderer>().material
            = IsPlaceable(SelectedRow, SelectedColumn, _currentPlayer) ?
                PlaceableMaterial : NormalMaterial;
        SelectedRow = row;
        SelectedColumn = column;
        SelectedCell.GetComponent<Renderer>().material = SelectedMaterial;
    }


}
