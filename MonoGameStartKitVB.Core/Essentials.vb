Imports Microsoft.Xna.Framework
Imports System.Runtime.CompilerServices

''' <summary>
''' Contains essential constants, functions, enums, etc. used throughout the game.
''' </summary>
Public Module Essentials
#Region "Constants"
    ' Aspect Ratio: 9:16
    Public Const SCREEN_WIDTH As Integer = 540
    Public Const SCREEN_HEIGHT As Integer = 960

    Public Const COLLISION_THRESHOLD As Single = 0.8F
    Public Const GRID_WIDTH As Integer = 25
    Public Const GRID_HEIGHT As Integer = 35
    Public Const CELL_SIZE As Integer = 24
    Public Const PLAYER_SPEED As Single = 200.0F
    Public Const ENEMY_SPEED As Single = 100.0F

    Public ReadOnly Property PlayerStartingPoint As New Point(GRID_WIDTH \ 2, GRID_HEIGHT - 2)
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
        Enemy = 6
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

    Public Enum TreeType As Integer
        Pine = 0
        Fruit = 1
        Oak = 2
    End Enum

    Public Enum SeedType As Integer
        Acorn = 0
        Berry = 1
        Nut = 2
    End Enum

    Public Enum EnemyType As Integer
        Beetle = 0
        Caterpillar = 1
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
    ''' Creates a maze layout with everything needed, using a specific algorithm.
    ''' </summary>
    ''' <param name="level">The current level number.</param>
    ''' <returns>A complete maze layout.</returns>
    Public Function CreateMazeLayout(level As Integer) As MazeTile(,)
        Dim maze(GRID_WIDTH - 1, GRID_HEIGHT - 1) As MazeTile
        Dim maxRowIndex = GRID_WIDTH - 1
        Dim maxColIndex = GRID_HEIGHT - 1
        Dim saplingCount = 0

        ' Create a Pac-Man style maze with:
        '   - Fences (5) around the perimeter
        '   - Walkable tiles (0) forming continuous paths
        '   - Saplings (3) in a checkerboard pattern
        '   - No dead ends or enclosed spaces

        For i As Integer = 0 To maxRowIndex
            For j As Integer = 0 To maxColIndex
                ' Fences around the perimeter
                If i = 0 OrElse i = maxRowIndex OrElse j = 0 OrElse j = maxColIndex Then
                    maze(i, j) = MazeTile.Fence
                    Continue For
                End If

                ' Second and second-to-last rows are all walkable (border paths)
                If i = 1 OrElse i = maxRowIndex - 1 Then
                    maze(i, j) = MazeTile.Walkable
                    Continue For
                End If
                ' First and last columns of inner area are walkable (border paths)
                If j = 1 OrElse j = maxColIndex - 1 Then
                    maze(i, j) = MazeTile.Walkable
                    Continue For
                End If

                If i Mod 2 = 0 AndAlso j Mod 2 = 1 Then
                    maze(i, j) = MazeTile.Sapling
                    saplingCount += 1
                Else
                    maze(i, j) = MazeTile.Walkable
                End If
            Next j
        Next i

        Dim enemyCount = Math.Clamp(level, 3, 8)
        Dim rnd = Random.Shared
        Dim pesticideCount = 3

        Dim ManhattanDistance =
            Function(pt1 As Point, pt2 As Point) As Integer
                Return Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y)
            End Function

        Dim playerStart = PlayerStartingPoint
        Dim walkableTiles As New List(Of Point)
        For i As Integer = 0 To maxRowIndex
            For j As Integer = 0 To maxColIndex
                If maze(i, j) = MazeTile.Walkable AndAlso
                    Not (i = playerStart.X AndAlso j = playerStart.Y) Then
                    walkableTiles.Add(New Point(i, j))
                End If
            Next j
        Next i

        Dim enemyTiles = From wt In walkableTiles
                         Where ManhattanDistance(wt, playerStart) > 5
                         Order By rnd.Next() Take enemyCount
        For Each tile In enemyTiles
            maze(tile.X, tile.Y) = MazeTile.Enemy
        Next tile

        Dim farTiles = From wt In walkableTiles
                       Where ManhattanDistance(wt, playerStart) > 5
                       Order By rnd.Next() Take pesticideCount
        For Each tile In farTiles
            maze(tile.X, tile.Y) = MazeTile.Pesticide
        Next tile

        Return maze
    End Function
#End Region
End Module