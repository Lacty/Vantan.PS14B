using UnityEngine;
using System.Collections;

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

    private const int Rows = 8;
    private const int Columns = 8;
    private readonly GameObject[,] _cells = new GameObject[Rows, Columns];

    void Start()
    {
        for (var r = 0; r < Rows; r++)
        {
            for (var c = 0; c < Columns; c++)
            {
                _cells[r, c] = CreateCell(r, c);
            }
        }
        UpdateSelectedCell(0, 0);
    }

    private GameObject CreateCell(int r, int c)
    {
        var cell = Instantiate(Cell);
        cell.name = Cell.name + "(" + r + "," + c + ")";
        cell.transform.Translate(1.05F * c, 0, -1.05F * r);
        cell.transform.parent = gameObject.transform;
        return cell;
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
    }

    private void UpdateSelectedCell(int row, int column)
    {
        SelectedCell.GetComponent<Renderer>()
            .material = NormalMaterial;

        SelectedRow = row;
        SelectedColumn = column;

        SelectedCell.GetComponent<Renderer>()
            .material = SelectedMaterial;
    }
}
