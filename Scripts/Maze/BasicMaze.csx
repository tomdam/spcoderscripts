byte N = 1;
byte S = 2;
byte E = 4;
byte W = 8;

var SIDES    = new Dictionary<string, int>() {{ "N", 1 }, { "S", 2 }, { "E", 4 }, { "W", 8 }};
var DX       = new Dictionary<string, int>() {{ "E", 1 }, { "W", -1 }, { "N", 0 }, { "S", 0 } };
var DY       = new Dictionary<string, int>() {{ "E", 0 }, { "W", 0 }, { "N", -1 }, { "S", 1 }};
var OPPOSITE = new Dictionary<string, int>() {{ "E", W }, { "W", E }, { "N", S }, { "S", N }};

Random r = new Random();
void RandomizeDirections(string[] array) 
{    
    for (var i = array.Length - 1; i > 0; i--) {
        int j       = r.Next(i + 1);
        string temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}

void MazeGenerationPassage(int cx, int cy, int[,] grid)
{
    var directions = new string[] {"N", "S", "E", "W"};
    RandomizeDirections(directions);
    
    for(var i = 0; i < directions.Length; i++)
    {
        var direction = directions[i];
        int nx       = cx + DX[direction]; 
        int ny       = cy + DY[direction];
        if ((ny >= 0 && ny < grid.GetLength(0)) && (nx >= 0 && nx < grid.GetLength(1)) && grid[ny,nx] == 0)
        {
          grid[cy,cx] |= SIDES[direction];          
          grid[ny,nx] |= OPPOSITE[direction];
          MazeGenerationPassage(nx, ny, grid);
        }
    }
}

string PrintMaze(int[,] grid, int startRow, int finishRow)
{
    StringBuilder sb = new StringBuilder();
    sb.Append(" ");
    for(var y = 0; y < (grid.GetLength(1) * 2 - 1); y++)
    {
        sb.Append("_");
    }
    sb.AppendLine();
    for(var y = 0; y < grid.GetLength(0); y++)
    {
        if (y != startRow) sb.Append("|"); else sb.Append(" ");
        
        for(var x = 0; x < grid.GetLength(1); x++)
        {
            if (((grid[y,x] & S) != 0))
            {
                sb.Append(" ");
            }
            else
            {
                sb.Append("_");
            }
            
            if ((grid[y,x] & E) != 0)
            {
                sb.Append((((grid[y,x] | grid[y,x+1]) & S) != 0) ? " " : "_");                
            }                
            else
            {
                if (y == finishRow && x == grid.GetLength(0) - 1) sb.Append(" "); else sb.Append("|"); 
            }
        }        
        sb.AppendLine();
    }
    return sb.ToString();
}

int height = 20;
int width  = 20;

int[,] grid    = new int[height, width];
MazeGenerationPassage(0, 0, grid);

int startRow  = r.Next(height);
int finishRow = r.Next(height);

string maze = PrintMaze(grid, startRow, finishRow);
logger.Log("\n"+maze);

