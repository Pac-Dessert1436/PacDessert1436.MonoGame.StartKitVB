Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

Public NotInheritable Class GameManager
    Private ReadOnly _random As New Random

    Public Property Player As Actor.Player
    Public Property Maze As MazeTile(,)
    Public ReadOnly Seeds As New List(Of Actor.Seed)
    Public ReadOnly Enemies As New List(Of Actor.Enemy)
    Public ReadOnly Pesticides As New List(Of Point)

    Public Property GameState As GameState = GameState.Title
    Private _previousGameState As GameState = GameState.Title
    
    Public Property CurrentLevel As Integer = 1
    Public Property HighScore As Integer = 0
    Public Property GetReadyTimer As Single = 0.0F
    Public Property IsGetReadyActive As Boolean = False
    Public Property PesticideActive As Boolean = False
    Public Property PesticideTimer As Single = 0.0F

    Private _previousKeyboardState As KeyboardState

    Public Sub New()
        InitializeGame()
    End Sub
    
    Public Sub InitializeGame()
        Player = New Actor.Player(PlayerStartingPoint)
        CurrentLevel = 1
        InitializeLevel()
    End Sub

    Public Sub InitializeLevel()
        Maze = CreateMazeLayout(CurrentLevel)
        Seeds.Clear()
        Enemies.Clear()
        Pesticides.Clear()
        
        ParseMaze()
        
        Player.ResetPosition()
        GetReadyTimer = GET_READY_DURATION
        IsGetReadyActive = True
        PesticideActive = False
        PesticideTimer = 0.0F
        
        ScheduleEvent_LevelChanged(CurrentLevel)
        ScheduleEvent_GetReadyMessage()
    End Sub

    Private Sub ParseMaze()
        For x As Integer = 0 To MAZE_WIDTH - 1
            For y As Integer = 0 To MAZE_HEIGHT - 1
                Dim tile = Maze(x, y)
                Dim pos = New Point(x, y)
                
                Select Case tile
                    Case MazeTile.Collectible
                        Seeds.Add(New Actor.Seed(pos, SeedType.Acorn))
                        Maze(x, y) = MazeTile.Walkable
                        
                    Case MazeTile.Pesticide
                        Pesticides.Add(pos)
                        Maze(x, y) = MazeTile.Walkable
                End Select
            Next y
        Next x
        
        SpawnEnemies()
    End Sub

    Private Sub SpawnEnemies()
        Dim enemyType = GetEnemyTypeForLevel(CurrentLevel)
        Dim enemyCount = Math.Min(4 + CurrentLevel, 8)
        Dim spawnPoints = New List(Of Point)
        
        For x As Integer = 0 To MAZE_WIDTH - 1
            For y As Integer = 0 To MAZE_HEIGHT - 1
                If Maze(x, y) = MazeTile.Walkable AndAlso
                   ManhattanDistance(New Point(x, y), PlayerStartingPoint) > 5 Then
                    spawnPoints.Add(New Point(x, y))
                End If
            Next y
        Next x
        
        For i As Integer = 0 To Math.Min(enemyCount - 1, spawnPoints.Count - 1)
            Dim index = _random.Next(spawnPoints.Count)
            Dim pos = spawnPoints(index)
            spawnPoints.RemoveAt(index)
            Enemies.Add(New Actor.Enemy(pos, enemyType))
        Next i
    End Sub

    Public Sub Update(deltaTime As Single)
        HandleGameStateTransitions()

        If GameState = GameState.Playing Then
            If IsGetReadyActive Then
                GetReadyTimer -= deltaTime
                If GetReadyTimer <= 0 Then
                    IsGetReadyActive = False
                    GetReadyTimer = 0
                End If
                Return
            End If

            Player.Update(deltaTime)

            For Each enemy In Enemies
                enemy.Update(deltaTime)
            Next

            If PesticideActive Then
                PesticideTimer -= deltaTime
                If PesticideTimer <= 0 Then
                    PesticideActive = False
                    For Each enemy In Enemies
                        enemy.IsVulnerable = False
                    Next
                End If
            End If

            CheckCollisions()
            CheckLevelComplete()
        End If
    End Sub

    Private Sub HandleGameStateTransitions()
        If _previousGameState <> GameState Then
            Dim oldState = _previousGameState
            _previousGameState = GameState
            
            Select Case GameState
                Case GameState.Playing
                    If oldState = GameState.Title Then
                        ScheduleEvent_GameStart()
                        ScheduleEvent_GameStateChanged(GameState)
                    ElseIf oldState = GameState.GameOver Then
                        InitializeGame()
                        ScheduleEvent_GameStart()
                        ScheduleEvent_GameStateChanged(GameState)
                    ElseIf oldState = GameState.LevelCleared Then
                        CurrentLevel += 1
                        InitializeLevel()
                        ScheduleEvent_NextLevel()
                        ScheduleEvent_GameStateChanged(GameState)
                    End If
                    
                Case GameState.GameOver
                    If Player.Score > HighScore Then
                        HighScore = Player.Score
                        ScheduleEvent_HighScoreChanged(HighScore)
                    End If
                    ScheduleEvent_GameStateChanged(GameState)
                    
                Case GameState.LevelCleared
                    ScheduleEvent_GameStateChanged(GameState)
                    
                Case GameState.Title
                    ScheduleEvent_GameStateChanged(GameState)
                    
                Case GameState.Paused
                    ScheduleEvent_GameStateChanged(GameState)
            End Select
        End If
    End Sub

    Private Sub CheckCollisions()
        Dim playerGridPos = Player.GridPosition
        
        For Each seed In Seeds.Where(Function(s) s.IsActive).ToList()
            If seed.GridPosition = playerGridPos Then
                seed.IsActive = False
                Player.CollectSeed(seed.SeedType)
            End If
        Next

        For Each pesticidePos In Pesticides.ToList()
            If pesticidePos = playerGridPos Then
                Pesticides.Remove(pesticidePos)
                Player.CollectPesticide()
                ActivatePesticide()
            End If
        Next

        For Each enemy In Enemies.Where(Function(e) e.IsActive AndAlso Not e.IsRespawning).ToList()
            If enemy.GridPosition = playerGridPos Then
                If enemy.IsVulnerable AndAlso enemy.GracePeriodTimer <= 0 Then
                    enemy.Die()
                    Player.KillEnemy()
                ElseIf enemy.GracePeriodTimer <= 0 AndAlso Not Player.IsInDeathAnimation Then
                    Player.LoseLife()
                    If Player.Lives <= 0 Then
                        GameState = GameState.GameOver
                    Else
                        ResetPositionsAfterDeath()
                    End If
                    Exit For
                End If
            End If
        Next
    End Sub

    Private Sub ActivatePesticide()
        PesticideActive = True
        PesticideTimer = VULNERABLE_DURATION
        
        For Each enemy In Enemies
            If enemy.IsActive AndAlso Not enemy.IsRespawning Then
                enemy.MakeVulnerable()
            End If
        Next
    End Sub

    Private Sub ResetPositionsAfterDeath()
        Player.ResetPosition()
        
        For Each enemy In Enemies
            If enemy.IsActive AndAlso Not enemy.IsRespawning Then
                enemy.SetRandomDirection()
            End If
        Next
    End Sub

    Private Sub CheckLevelComplete()
        If Not Seeds.Any(Function(s) s.IsActive) Then
            GameState = GameState.LevelCleared
            ScheduleEvent_LevelCleared()
        End If
    End Sub
    
    Public Sub HandleInput()
        Dim keyboardState = Keyboard.GetState()
        
        Select Case GameState
            Case GameState.Title
                If keyboardState.IsKeyDown(Keys.Enter) AndAlso Not _previousKeyboardState.IsKeyDown(Keys.Enter) Then
                    GameState = GameState.Playing
                End If
                
            Case GameState.GameOver
                If keyboardState.IsKeyDown(Keys.Enter) AndAlso Not _previousKeyboardState.IsKeyDown(Keys.Enter) Then
                    GameState = GameState.Playing
                End If
                
            Case GameState.Playing
                If keyboardState.IsKeyDown(Keys.P) OrElse keyboardState.IsKeyDown(Keys.Escape) Then
                    GameState = GameState.Paused
                End If
                
            Case GameState.Paused
                If keyboardState.IsKeyDown(Keys.P) OrElse keyboardState.IsKeyDown(Keys.Escape) OrElse
                   keyboardState.IsKeyDown(Keys.Enter) Then
                    GameState = GameState.Playing
                End If
        End Select
        
        _previousKeyboardState = keyboardState
    End Sub

    Public Function GetSeedTypeForCurrentLevel() As SeedType
        Select Case (CurrentLevel - 1) Mod 6
            Case 0, 3
                Return SeedType.Acorn
            Case 1, 4
                Return SeedType.Berry
            Case 2, 5
                Return SeedType.Nut
            Case Else
                Return SeedType.Acorn
        End Select
    End Function
End Class