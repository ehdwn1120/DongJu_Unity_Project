using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Pathfinder : MonoBehaviour
{
    public enum Options
    {
        FixedPoints,
        BFS,
        DFS
    }
    [SerializeField] private Options _option;

    public struct Coord
    {
        public static Coord Zero = new Coord(0, 0);
        public static Coord Error = new Coord(-1, -1);

        public int Y, X;
        public Coord(int y, int x)
        {
            Y = y;
            X = x;
        }

        public static bool operator ==(Coord op1, Coord op2)
            => (op1.X == op2.X) && (op1.Y == op2.Y);

        public static bool operator !=(Coord op1, Coord op2)
            => !(op1 == op2);
    }

    // 맵 기본 단위
    private struct MapNode
    {
        public Transform Point;
        public Coord Coord;
        public NodeTypes Type;
    }

    public enum NodeTypes
    {
        None,
        Path,
        Obstacle
    }

    private static MapNode[,] _map;
    private static bool[,] _visited;
    private static int[,] _direction = new int[2, 4]
    {
        {-1, 0, 1, 0 },
        {0, -1, 0, 1},
    };

    private static Transform _leftBottom;
    private static Transform _rightTop;
    private static float _nodeTerm = 1.0f; // 노드 실제 간격
    private static float _width => _rightTop.position.x - _leftBottom.position.x;
    private static float _height => _rightTop.position.z - _leftBottom.position.z;



    private static List<Transform> _path;
    private static List<Transform> _path_DFS;
    private static List<Transform> _path_BFS;
    private static List<List<Transform>> _pathList_BFS;
    public static void SetUpMap()
    {
        Transform obstaclesParent = GameObject.Find("Nodes").transform;
        Transform[] obstacles = new Transform[obstaclesParent.childCount];
        for (int i = 0; i < obstaclesParent.childCount; i++)
            obstacles[i] = obstaclesParent.GetChild(i);

        Transform pathsParent = GameObject.Find("Paths").transform;
        Transform[] paths = new Transform[pathsParent.childCount];
        for (int i = 0; i < pathsParent.childCount; i++)
            paths[i] = pathsParent.GetChild(i);

        SetUpMap(paths.ToList(), obstacles.ToList());
    }


    public bool TryFindOptimizedPath(Transform start, Transform end, out List<Transform> optimizedPath)
    {
        bool found = false;
        optimizedPath = null;

        _visited = new bool[_map.GetLength(0), _map.GetLength(1)];

        switch (_option)
        {
            case Options.FixedPoints:
                {
                    found = SetPathWithFixedPathPoints(start, end);
                    if (found)
                        optimizedPath = _path;
                }
                break;
            case Options.BFS:
                {
                    found = BFS(start: TransformToCoord(start),
                                end: TransformToCoord(end));

                    if (found)
                        optimizedPath = _path_BFS;
                }                
                break;
            case Options.DFS:
                {
                    found = DFS(start: TransformToCoord(start),
                                end: TransformToCoord(end));
                    if (found)
                        optimizedPath = _path_DFS;
                }
                break;
            default:
                break;
        }


        return found;
    }

    private static void SetUpMap(List<Transform> paths, List<Transform> obstacles)
    {
        List<Transform> nodes = new List<Transform>();
        nodes.AddRange(paths);
        nodes.AddRange(obstacles);

        IOrderedEnumerable<Transform> nodesFiltered = nodes.OrderBy(node => node.position.x + node.position.z);

        _leftBottom = nodesFiltered.First();
        _rightTop = nodesFiltered.Last();

        _map = new MapNode[(int)(_height / _nodeTerm) + 1, (int)(_width / _nodeTerm) + 1];
        _visited = new bool[(int)(_height / _nodeTerm) + 1, (int)(_width / _nodeTerm) + 1];

        Coord tmpCoord;
        foreach (Transform path in paths)
        {
            tmpCoord = TransformToCoord(path);
            _map[tmpCoord.Y, tmpCoord.X] = new MapNode()
            {
                Point = path,
                Coord = tmpCoord,
                Type = NodeTypes.Path
            };
        }

        foreach (Transform obstacle in obstacles)
        {
            tmpCoord = TransformToCoord(obstacle);
            _map[tmpCoord.Y, tmpCoord.X] = new MapNode()
            {
                Point = obstacle,
                Coord = tmpCoord,
                Type = NodeTypes.Obstacle
            };
        }
    }
    private static bool SetPathWithFixedPathPoints(Transform start, Transform end)
    {
        foreach (Paths.Path path in Paths.Instance.GetPaths())
        {
            if (path.GetPathPoints()[0] == start)
            {
                _path = new List<Transform>(path.GetPathPoints());
                return true;
            }
        }
        return false;
    }

    private static bool DFS(Coord start, Coord end)
    {
        _path_DFS = new List<Transform>();
        bool isFound = DFSLoop(start, end);
        if (isFound)
        {
            _path_DFS.Add(GetTransform(start));
            _path_DFS.Reverse();
            _path_DFS.Add(GetTransform(end));
        }
        return isFound;
    }


    private static bool DFSLoop(Coord start, Coord end)
    {
        bool isFound = false;
        _visited[start.Y, start.X] = true;
        //Debug.Log($"DFS ing... {start.X}, {start.Y}");

        Coord next;
        for (int i = 0; i < _direction.GetLength(1); i++)
        {
            next.Y = start.Y + _direction[0, i];
            next.X = start.X + _direction[1, i];

            // 탐색 위치가 맵을 벗어나는지
            if ((next.Y < 0 || next.Y >= _map.GetLength(0)) ||
                (next.X < 0 || next.X >= _map.GetLength(1)))
                continue;

            // 탐색 위치가 장애물일 경우
            if (_map[next.Y, next.X].Type == NodeTypes.Obstacle)
                continue;

            // 방문 여부
            if (_visited[next.Y, next.X] == true)
                continue;

            // 도착 여부
            if (next == end)
            {
                return true;
            }
            else
            {
                isFound = DFSLoop(next, end);
                if (isFound)
                {
                    _path_DFS.Add(GetTransform(next));
                    break;
                }
            }
        }
        return isFound;
    }


    private static bool BFS(Coord start, Coord end)
    {
        _pathList_BFS = new List<List<Transform>>(); // <-- 이거 추가해주삼요..

        bool isFinished = false;
        List<KeyValuePair<Coord, Coord>> parents = new List<KeyValuePair<Coord, Coord>>(); // 탐색한 자식노드 - 부모노드 쌍
        Queue<Coord> queue = new Queue<Coord>(); // 탐색 하려는 노드들
        queue.Enqueue(start);
        _visited[start.Y, start.X] = true;

        // 탐색 하려는 노드가 존재하는 동안 반복
        while (queue.Count > 0)
        {
            Coord parent = queue.Dequeue();

            for (int i = 0; i < _direction.GetLength(1); i++)
            {
                Coord next = new Coord(parent.Y + _direction[0, i], parent.X + _direction[1, i]);

                // 탐색 위치가 맵을 벗어나는지
                if (next.Y < 0 || next.Y >= _map.GetLength(0) ||
                    next.X < 0 || next.X >= _map.GetLength(1))
                    continue;

                // 탐색 위치가 길이 아닐  경우
                if (_map[next.Y, next.X].Type != NodeTypes.Path)
                    continue;

                // 방문 여부
                if (_visited[next.Y, next.X])
                    continue;

                // 방문
                parents.Add(new KeyValuePair<Coord, Coord>(parent, next));
                _visited[next.Y, next.X] = true;

                // 도착 여부
                if (next == end)
                {
                    isFinished = true;
                    _pathList_BFS.Add(CalcPath(parents));
                }
                else
                {
                    queue.Enqueue(next);
                }
            }
        }

        _path_BFS = _pathList_BFS.OrderBy(path => path.Count).First();// list 들 중에서 가장 요소 갯수가 적은 list 를 반환
        return isFinished;
    }

    private static List<Transform> CalcPath(List<KeyValuePair<Coord, Coord>> parents)
    {
        List<Transform> path = new List<Transform>();
        Coord coord = parents.Last().Value; // 젤 마지막 자식 노드
        path.Add(GetTransform(coord));

        int index = parents.Count - 1; // 젤 마지막 인덱스
        while (index > 0 &&
               parents[index].Key != parents.First().Key)
        {
            path.Add(GetTransform(parents[index].Key)); // 부모 노드 추가
            index = parents.FindLastIndex(pair => pair.Value == parents[index].Key); // 현재 부모노드를 자식노드로 가지는 조부모노드의 인덱스 찾기
        }
        path.Add(GetTransform(parents.First().Key)); // 젤 처음 노드 추가
        path.Reverse(); // 경로를 반대로 저장했으니 뒤집어줌

        return path;
    }



    private static Coord TransformToCoord(Transform target)
    {
        return new Coord(Mathf.RoundToInt((target.position.z - _leftBottom.position.z) / _nodeTerm),
                         Mathf.RoundToInt((target.position.x - _leftBottom.position.x) / _nodeTerm));
    }

    private static Transform GetTransform(Coord coord)
    {
        return _map[coord.Y, coord.X].Point;
    }
}
