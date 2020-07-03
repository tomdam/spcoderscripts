//Implementation of the Recursive backtracking algorithm for maze generation in C#.
//Parts of the alghoritm taken from http://weblog.jamisbuck.org/2011/2/7/maze-generation-algorithm-recap.html (Ruby implementation of maze generation)

//declaring "directions" (North, South, East and West) variables that we will use later during the printing
byte N = 1;
byte S = 2;
byte E = 4;
byte W = 8;

//declaring utility dictionaries used during the maze generation.
//regular sides
var SIDES    = new Dictionary<char, byte>() {{ 'N', 1 }, { 'S', 2 }, { 'E', 4 }, { 'W', 8 }};
//dictionary that contains values that help us go horizontally (east => + 1; west => - 1) during the random generation
var DX       = new Dictionary<char, sbyte>() {{ 'E', 1 }, { 'W', -1 }, { 'N', 0 }, { 'S', 0 } };
//dictionary that contains values that help us go vertically (south => + 1; north => - 1) during the random generation
var DY       = new Dictionary<char, sbyte>() {{ 'E', 0 }, { 'W', 0 }, { 'N', -1 }, { 'S', 1 }};
//oppisite sides
var OPPOSITE = new Dictionary<char, byte>() {{ 'E', W }, { 'W', E }, { 'N', S }, { 'S', N }};

//declare random variable
Random r = new Random();
//Method used for shuffling the directions array
void RandomizeDirections(char[] array) 
{    
    for (var i = array.Length - 1; i > 0; i--) {
        int j       = r.Next(i + 1);
        char temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}

//Recirsive method that contains the main generation logic
//It takes coordinates of current cell and the grid
void MazeGenerationPassage(byte cx, byte cy, byte[,] grid)
{
    //generate the array of all 4 directions
    var directions = new char[] {'N', 'S', 'E', 'W'};
    //shuffle the array
    //based on the result of this shuffling will the alghoritm choose the next cells to visit
    RandomizeDirections(directions);
    
    //go through the array of directions
    for(var i = 0; i < directions.Length; i++)
    {
        //get the current direction
        var direction = directions[i];
        //calculate the next x value (add the current x value to the value from DX, so nx can go 1 cell left or right from cx, or remain the same)
        byte nx       = (byte)(cx + DX[direction]);         
        //calculate the next y value (add the current y value to the value from DY, so nx can go 1 cell up or down from cy, or remain the same)
        byte ny       = (byte)(cy + DY[direction]);
        //check if nx and ny coordinates are inside the grid and also check if the cell with those coordinates is still unvisited (has value 0)
        if ((ny >= 0 && ny < grid.GetLength(0)) && (nx >= 0 && nx < grid.GetLength(1)) && grid[ny,nx] == 0)
        {
            //change the value of current cell by removing the borders between it and the cell next to it in the "direction" direction 
            //(the one that we are currently handling in for loop)          
            grid[cy,cx] |= SIDES[direction];
            //change the value of ny,nx cell by removing the borders between it and the current cell in the "opposite direction" direction 
            //(opposite to the one that we are currently handling in for loop)
            grid[ny,nx] |= OPPOSITE[direction];
            //Recursively call the same method for the next cell (nx,ny)
            MazeGenerationPassage(nx, ny, grid);
        }
    }
}
//Method that generates the string maze, with walls, from the matrix
string PrintMaze(byte[,] grid, byte startRow, byte finishRow)
{
    StringBuilder sb = new StringBuilder();
    sb.Append(" ");
    //draw top walls
    for(var y = 0; y < (grid.GetLength(1) * 2 - 1); y++)
    {
        sb.Append("_");
    }
    sb.AppendLine();
    for(var y = 0; y < grid.GetLength(0); y++)
    {
        //draw left walls, except for the starting row which is empty
        if (y != startRow) sb.Append("|"); else sb.Append(" ");
        
        for(var x = 0; x < grid.GetLength(1); x++)
        {
            //draw south (down) wall 
            if (((grid[y,x] & S) != 0))
            {
                sb.Append(" ");
            }
            else
            {
                sb.Append("_");
            }
            
            //draw east (right) wall 
            if ((grid[y,x] & E) != 0)
            {
                //if the east wall is missing, but the south wall in the next field exists
                //than draw the south wall "_"
                sb.Append((((grid[y,x] | grid[y,x+1]) & S) != 0) ? " " : "_");
            }                
            else
            {
                //draw east (right) wall except for the end right on finish row
                if (y == finishRow && x == grid.GetLength(0) - 1) sb.Append(" "); else sb.Append("|"); 
            }
        }        
        sb.AppendLine();
    }
    return sb.ToString();
}

//declare height of the grid
byte height = 10;
//declare width of the grid
byte width = 10;

//declare the matrix that will contain the data
//every cell of the matrix contains the byte whose bits hold the value for the walls between that cell and 4 cells around it
byte[,] grid    = new byte[height, width];

//call the function for maze generation, starting from top left cell (0,0)
MazeGenerationPassage(0, 0, grid);

//generate random start wall
byte startRow  = (byte)r.Next(height);
//generate random end wall
byte finishRow = (byte)r.Next(height);

//generate the string from the maze
string maze = PrintMaze(grid, startRow, finishRow);
//show the maze in SPCoder logger window
logger.Log("\n"+maze, false);

//This method prints the values from the maze
//it can be used to better understand how the algorighm works
//For example, if the value in the cell is 10, that means that the cell has the top and right walls
//because the bits at North (1) and East (4) walls are 0, and bits at South (2) and West (8) are 1
//so, 8+2 = 10
string PrintMazeValues(byte[,] grid)
{
    StringBuilder sb = new StringBuilder();
    
    for(var i = 0; i < grid.GetLength(0); i++)
    {
        for(var j = 0; j < grid.GetLength(1); j++)
        {
            sb.Append(String.Format("{0, 4}", grid[i,j]));
        }
        sb.AppendLine();
    }
    return sb.ToString();
}

//Execute the next line to print the values of the maze
//logger.Log("\n"+PrintMazeValues(grid), false);
