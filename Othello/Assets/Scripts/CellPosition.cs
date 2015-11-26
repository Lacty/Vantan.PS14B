struct CellPosition
{
    public int Row { get; set; }
    public int Column { get; set; }

    public CellPosition(int row, int column) : this()
    {
        Row = row;
        Column = column;
    }
}