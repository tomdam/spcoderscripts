byte N = 1;
byte S = 2;
byte E = 4;
byte W = 8;

var SIDES    = new Dictionary<char, byte>() {{ 'N', 1 }, { 'S', 2 }, { 'E', 4 }, { 'W', 8 }};
var DX       = new Dictionary<char, sbyte>() {{ 'E', 1 }, { 'W', -1 }, { 'N', 0 }, { 'S', 0 } };
var DY       = new Dictionary<char, sbyte>() {{ 'E', 0 }, { 'W', 0 }, { 'N', -1 }, { 'S', 1 }};
var OPPOSITE = new Dictionary<char, byte>() {{ 'E', W }, { 'W', E }, { 'N', S }, { 'S', N }};

Random r = new Random();
void RandomizeDirections(char[] array) {
    
    for (var i = array.Length - 1; i > 0; i--) {
        int j       = r.Next(i + 1);
        char temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}

void MazeGenerationPassage(byte cx, byte cy, byte[,] grid)
{
    var directions = new char[] {'N', 'S', 'E', 'W'};
    RandomizeDirections(directions);
    
    for(var i = 0; i < directions.Length; i++)
    {
        var direction = directions[i];
        byte nx       = (byte)(cx + DX[direction]); 
        byte ny       = (byte)(cy + DY[direction]);
        if ((ny >= 0 && ny < grid.GetLength(0)) && (nx >= 0 && nx < grid.GetLength(1)) && grid[ny,nx] == 0)
        {
          grid[cy,cx] |= SIDES[direction];          
          grid[ny,nx] |= OPPOSITE[direction];
          MazeGenerationPassage(nx, ny, grid);
        }
    }
}

string PrintMaze(byte[,] grid, byte startRow, byte finishRow)
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

byte height = 20;
byte width  = 20;

byte[,] grid    = new byte[height, width];
MazeGenerationPassage(0, 0, grid);

byte startRow  = (byte)r.Next(height);
byte finishRow = (byte)r.Next(height);

string maze = PrintMaze(grid, startRow, finishRow);
logger.Log("\n"+maze);

