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

    void Start()
    {
        for (var r = 0; r < 8; r++)
        {
            for (var c = 0; c < 8; c++)
            {
                var cell = Instantiate(Cell);
                cell.name = Cell.name + "(" + r + "," + c + ")";
                cell.transform.Translate(1.05F * c, 0, -1.05F * r);
                cell.transform.parent = gameObject.transform;
            }
        }
    }
}
