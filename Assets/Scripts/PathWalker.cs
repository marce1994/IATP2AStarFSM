using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PathWalker : MonoBehaviour
{
    public PathWalker()
    {
        _path = new Vector3[] { };
        _currentPathPos = 0;
    }

    private Vector3[] _path;
    private int _currentPathPos;
    private bool _calculating_path = false;
    
    public int WALK_SPEED = 10;

    public Vector3[] WalkPath
    {
        set
        {
            _path = value;
            _currentPathPos = 0;
        }
        get
        {
            return _path!= null? _path : new Vector3[] { };
        }
    }

    public Vector3 NextPosition
    {
        get
        {
            if (!WalkPath.Any())
                return Vector3.right;
            if (transform.position == WalkPath.Last())
                return WalkPath.Last() + Vector3.right;
            if (transform.position == WalkPath[_currentPathPos])
                return WalkPath[_currentPathPos + 1];

            return WalkPath[_currentPathPos];
        }
    }

    public bool Ended
    {
        get
        {
            if (_calculating_path)
                return false;

            return WalkPath.Any()? transform.position == _path.Last() : true;
        }
    }

    private Color debugPathColor;
    public async Task WalkTo(Vector3 position)
    {
        _calculating_path = true;
        debugPathColor = Random.ColorHSV();
        debugPathColor.a = 1f;

        Vector3[] path = await PathFinderManager.Instance.FindPath(transform.position, position);

        if (path == null)
        {
            Debug.Log("No hay un camino para llegar al destino");
            _calculating_path = false;
            return;
        }

        WalkPath = path;

        _calculating_path = false;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < WalkPath.Length; i++)
        {
            Gizmos.color = debugPathColor;
            Gizmos.DrawLine(from: WalkPath[i], to: WalkPath[Mathf.Min(i + 1, WalkPath.Length - 1)]);
        }
    }

    private void LateUpdate()
    {
        if (!WalkPath.Any() || transform.position == WalkPath.Last()) return;

        if (transform.position != WalkPath[_currentPathPos])
        {
            if (_currentPathPos == 0)
                transform.position = WalkPath[_currentPathPos];

            Vector3 pos = Vector3.MoveTowards(transform.position, WalkPath[_currentPathPos], Time.deltaTime * WALK_SPEED);
            transform.position = pos;
        }
        else _currentPathPos = (_currentPathPos + 1) % WalkPath.Length;
    }
}
