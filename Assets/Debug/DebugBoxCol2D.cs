using UnityEngine;

public class DebugBoxCol2D : MonoBehaviour
{
    public GameObject debugPrefab;
    GameObject _debug;
    BoxCollider2D _col;

    private void Update()
    {
        if (!_debug)
            _debug = Instantiate(debugPrefab, transform);

        if (!_col)
            _col = GetComponent<BoxCollider2D>();

        _debug.transform.position = transform.position + (Vector3)_col.offset;
        _debug.transform.localScale = transform.localScale * _col.size;
    }
}
