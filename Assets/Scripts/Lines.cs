using System;

public delegate void ShowBox(int x, int y, int ball);
public delegate void PlayCut();
public class Lines 
{
    public const int SIZE = 9;
    private const int ADD_BALLS = 3;
    private const int BALLS = 7;
    ShowBox showBox;
    PlayCut playCut;

    private int[,] _map;
    private bool[,] _mark;
    private bool[,] _used;

    private int _fromX, _fromY;
    private bool _isBallSelected;

    private Random random = new Random();

    public Lines(ShowBox showBox, PlayCut playCut)
    {
        this.showBox = showBox;
        this.playCut = playCut;
        _map = new int[SIZE, SIZE];
    }
    public void Start()
    {
        _isBallSelected = false;
        ClearMap();
        AddRandomBalls();
    }

    public void Click(int x, int y)
    {
        if (IsGameOver())
        {
            Start();
        } else
        {
            if (_map[x, y] > 0)
            {
                TakeBall(x, y);
            }
            else
            {
                MoveBall(x, y);
            }
        }
        
    }

    private void TakeBall(int x, int y)
    {
        _fromX = x;
        _fromY = y;
        _isBallSelected = true;
    }

    private void MoveBall(int x, int y)
    {
        if (!_isBallSelected) return;
        if (!CanMove(x, y)) return;
        SetMap(x, y, _map[_fromX, _fromY]);
        SetMap(_fromX, _fromY, 0);
        _isBallSelected = false;
        if (!CutLines())
        {
            AddRandomBalls();
            CutLines();
        }
    }

    private bool CutLines()
    {
        int balls = 0;
        _mark = new bool[SIZE, SIZE];

        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                balls += CutLine(x, y, 1, 0);
                balls += CutLine(x, y, 0, 1);
                balls += CutLine(x, y, 1, 1);
                balls += CutLine(x, y, -1, 1);
            }
        }
        if (balls > 0)
        {
            playCut();
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    if (_mark[x, y])
                    {
                        SetMap(x, y, 0);
                    }
                }
            }
            return true;
        }
        return false;
    }

    private int CutLine(int x0, int y0, int sx, int sy)
    {
        int ball = _map[x0, y0];
        if (ball == 0) return 0;
        int count = 0;
        for (int x = x0, y = y0; GetMap(x, y) == ball; x += sx, y += sy)
        {
            count++;
        }
        if (count < 5)
        {
            return 0;
        }
        for (int x = x0, y = y0; GetMap(x, y) == ball; x += sx, y += sy)
        {
            _mark[x, y] = true;
        }
        return count;
    }

    private bool OnMap(int x, int y)
    {
        return x >= 0 && x < SIZE && y >= 0 && y < SIZE;
    }

    private int GetMap(int x, int y)
    {
        if (!OnMap(x, y)) return 0;
        return _map[x, y];
    }
    

    private bool CanMove(int toX, int toY)
    {
        _used = new bool[SIZE, SIZE];
        Walk(_fromX, _fromY, true);
        return _used[toX, toY];
    }

    private bool IsGameOver ()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if (_map[x, y] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void Walk(int x, int y, bool start = false)
    {
        if (!start)
        {
            if (!OnMap(x, y)) return;
            if (_map[x, y] > 0) return;
            if (_used[x, y]) return;
        }
        _used[x, y] = true;
        Walk(x + 1, y);
        Walk(x - 1, y);
        Walk(x, y + 1);
        Walk(x, y - 1);
    }

    private void ClearMap()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                SetMap(x, y, 0);
            }
        }
    }
    private void SetMap(int x, int y, int value)
    {
        _map[x, y] = value;
        showBox(x, y, value);
    }

    private void AddRandomBalls()
    {
        for (int i = 0; i < ADD_BALLS; i++)
        {
            AddRandomBall();
        }
    }

    private void AddRandomBall()
    {
        int x, y;
        int loop = SIZE * SIZE;
        do
        {
            x = random.Next(SIZE);
            y = random.Next(SIZE);
            if (--loop <= 0) return;
        }
        while (_map[x, y] > 0);
        int ball = 1 + random.Next(BALLS - 1);
        SetMap(x, y, ball);
    }
}
