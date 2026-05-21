Imports Microsoft.Xna.Framework
Imports System.Runtime.CompilerServices

''' <summary>
''' Contains essential constants, functions, enums, etc. used throughout the game.
''' </summary>
Public Module Essentials
#Region "Constants"
    Public Const SCREEN_WIDTH As Integer = 800
    Public Const SCREEN_HEIGHT As Integer = 600

    Public Const COLLISION_THRESHOLD As Single = 0.8F
    Public Const GRID_WIDTH As Integer = 10
    Public Const GRID_HEIGHT As Integer = 10
    Public Const CELL_SIZE As Integer = 24
    Public Const PLAYER_SPEED As Single = 200.0F
    Public Const ENEMY_SPEED As Single = 100.0F
#End Region

#Region "Enums"
    ''' <summary>
    ''' Represents the current state of the game.
    ''' </summary>
    Public Enum GameState As Integer
        Title = 0
        Playing = 1
        Paused = 2
        GameOver = 3
        LevelStart = 4
        LevelCleared = 5
    End Enum

    ''' <summary>
    ''' Represents the type of a tile in the maze.
    ''' </summary>
    Public Enum MazeTile As Integer
        Walkable = 0
        Collectible = 1
        Pesticide = 2
        Sapling = 3
        Tree = 4
        Fence = 5
    End Enum

    ''' <summary>
    ''' Represents the direction of movement.
    ''' </summary>
    Public Enum Direction As Integer
        Up = 0
        Down = 1
        Left = 2
        Right = 3
    End Enum
#End Region

    ' NOTE: These constants are obsolete and should be removed.
    Public Const PLAYER_SIZE As Integer = 60
    Public Const SEED_SIZE As Integer = 30
    Public Const ENEMY_SIZE As Integer = 50
    Public Const SEEDS_TO_PLANT_TREE As Integer = 15

#Region "Events / Signals"
    Public Event GameStateChanged(newState As GameState)
    Public Event PlayerScoreChanged(score As Integer)
    Public Event SeedCollected(seed As Actor.Seed)
    Public Event TreePlanted(tree As Actor.Tree)
    Public Event PlayerDied()
    Public Event EnemyKilled(enemy As Actor.Enemy)
#End Region

#Region "Functions"
    ''' <summary>
    ''' Calculates the Jaccard distance between two rectangles.
    ''' </summary>
    ''' <param name="rectA">The first rectangle.</param>
    ''' <param name="rectB">The second rectangle.</param>
    ''' <returns>The Jaccard distance between the two rectangles.</returns>
    Public Function JaccardDistance(rectA As Rectangle, rectB As Rectangle) As Single
        If rectA.IsEmpty OrElse rectB.IsEmpty Then Return 0F
        Dim overlapArea As Integer
        With Rectangle.Intersect(rectA, rectB)
            overlapArea = If(.IsEmpty, 0, .width * .height)
        End With
        Dim areaA As Integer = rectA.width * rectA.height
        Dim areaB As Integer = rectB.width * rectB.height
        Dim unionArea As Integer = areaA + areaB - overlapArea
        Return 1.0F - If(unionArea <= 0, 0F, CSng(overlapArea / unionArea))
    End Function

    ''' <summary>
    ''' Converts a direction enum to a Vector2.
    ''' </summary>
    ''' <param name="direction">The direction enum to convert.</param>
    ''' <returns>A Vector2 representing the direction.</returns>
    <Extension> Public Function ToVector2(direction As Direction) As Vector2
        Select Case direction
            Case Direction.Up
                Return New Vector2(0, -1)
            Case Direction.Down
                Return New Vector2(0, 1)
            Case Direction.Left
                Return New Vector2(-1, 0)
            Case Direction.Right
                Return New Vector2(1, 0)
        End Select
    End Function

    ''' <summary>
    ''' Creates a maze layout without collectibles, using a specific algorithm.
    ''' </summary>
    ''' <returns>A maze layout without collectibles.</returns>
    Public Function CreateMazeLayoutWithoutCollectibles() As MazeTile(,)
        Dim maze(GRID_WIDTH - 1, GRID_HEIGHT - 1) As MazeTile
        Dim maxRowIndex As Integer = GRID_WIDTH - 1
        Dim maxColIndex As Integer = GRID_HEIGHT - 1

        For i As Integer = 0 To maxRowIndex
            For j As Integer = 0 To maxColIndex
                Dim isFence As Boolean =
                    i = 0 OrElse i = maxRowIndex OrElse j = 0 OrElse j = maxColIndex
                maze(i, j) = If(isFence, MazeTile.Fence, MazeTile.Walkable)
                ' TODO: Add farmland to the maze using the specific algorithm.
            Next j
        Next i
        Return maze
    End Function
#End Region
End Module