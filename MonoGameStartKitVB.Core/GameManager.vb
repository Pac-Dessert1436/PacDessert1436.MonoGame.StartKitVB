Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

Public NotInheritable Class GameManager
    Private ReadOnly _random As New Random

    Public Property Player As Actor.Player
    Public ReadOnly Seeds As New List(Of Actor.Seed)
    Public ReadOnly Enemies As New List(Of Actor.Enemy)
    Public ReadOnly Trees As New List(Of Actor.Tree)
    Public ReadOnly Pesticides As New List(Of Actor.Pesticide)

    Public Property GameState As GameState = GameState.Title
    Private _previousGameState As GameState = GameState.Title
    
    Public Const SEEDS_TO_SPAWN As Integer = 20
    Public Const ENEMIES_TO_SPAWN As Integer = 8
    Public Const PESTICIDES_TO_SPAWN As Integer = 3
    
    Public Sub New()
        InitializeGame()
    End Sub
    
    Public Sub InitializeGame()
        Player = New Actor.Player(New Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2)) With {
            .IsAlive = True,
            .Score = 0,
            .SeedsCollected = 0
        }
        Seeds.Clear()
        Enemies.Clear()
        Trees.Clear()
        Pesticides.Clear()
        SpawnSeeds(SEEDS_TO_SPAWN)
        SpawnEnemies(ENEMIES_TO_SPAWN)
        SpawnPesticides(PESTICIDES_TO_SPAWN)
    End Sub

    Public Sub Update(deltaTime As Single)
        HandleGameStateTransitions()

        If GameState = GameState.Playing Then
            Player.Update(deltaTime)

            For Each enemy In Enemies
                enemy.Update(deltaTime)
            Next

            For Each tree In Trees
                tree.Update(deltaTime)
            Next

            CheckCollisions()
        End If
    End Sub

    Private Sub HandleGameStateTransitions()
        If _previousGameState <> GameState Then
            Dim oldState = _previousGameState
            _previousGameState = GameState
            
            Select Case GameState
                Case GameState.Playing
                    If oldState = GameState.Title Then
                        ScheduleEvent_GameStateChanged(GameState)
                    ElseIf oldState = GameState.GameOver Then
                        InitializeGame()
                        ScheduleEvent_GameStateChanged(GameState)
                    End If
                    
                Case GameState.GameOver
                    ScheduleEvent_GameStateChanged(GameState)
                    
                Case GameState.Title
                    ScheduleEvent_GameStateChanged(GameState)
            End Select
        End If
    End Sub

    Private Sub CheckCollisions()
        Dim playerBounds As Rectangle = Player.GetBounds()

        For Each seed In Seeds.Where(Function(s) s.IsActive).ToList()
            If playerBounds.Intersects(seed.GetBounds()) Then
                seed.IsActive = False
                Player.CollectSeed(seed)

                If Player.SeedsCollected >= SEEDS_TO_PLANT_TREE Then
                    PlantTree()
                    Player.PlantTree()
                End If

                If Not Seeds.Any(Function(s) s.IsActive) Then
                    SpawnSeeds(SEEDS_TO_SPAWN)
                End If
            End If
        Next

        For Each pesticide In Pesticides.Where(Function(p) p.IsActive).ToList()
            If playerBounds.Intersects(pesticide.GetBounds()) Then
                pesticide.IsActive = False
                KillAllEnemies()
            End If
        Next

        For Each enemy In Enemies
            If playerBounds.Intersects(enemy.GetBounds()) Then
                Player.Die()
                GameState = GameState.GameOver
                Exit For
            End If
        Next
    End Sub

    Private Sub SpawnSeeds(count As Integer)
        Dim seedTypes As SeedType() = {SeedType.Acorn, SeedType.Berry, SeedType.Nut}
        
        For i As Integer = 0 To count - 1
            Dim position As New Vector2(
                _random.Next(50, SCREEN_WIDTH - 50),
                _random.Next(50, SCREEN_HEIGHT - 50)
            )
            Dim seedType = seedTypes(_random.Next(seedTypes.Length))
            Seeds.Add(New Actor.Seed(position, seedType))
        Next
    End Sub

    Private Sub SpawnEnemies(count As Integer)
        Dim enemyTypes As EnemyType() = {EnemyType.Beetle, EnemyType.Caterpillar}
        
        For i As Integer = 0 To count - 1
            Dim position As New Vector2(
                _random.Next(50, SCREEN_WIDTH - 50),
                _random.Next(50, SCREEN_HEIGHT - 50)
            )
            Dim enemyType = enemyTypes(_random.Next(enemyTypes.Length))
            Enemies.Add(New Actor.Enemy(position, enemyType))
        Next
    End Sub

    Private Sub SpawnPesticides(count As Integer)
        For i As Integer = 0 To count - 1
            Dim position As New Vector2(
                _random.Next(50, SCREEN_WIDTH - 50),
                _random.Next(50, SCREEN_HEIGHT - 50)
            )
            Pesticides.Add(New Actor.Pesticide(position))
        Next
    End Sub

    Private Sub PlantTree()
        Dim treeTypes As TreeType() = {TreeType.Oak, TreeType.Pine, TreeType.Fruit}
        Dim treePosition As New Vector2(
            _random.Next(50, SCREEN_WIDTH - 50),
            _random.Next(50, SCREEN_HEIGHT - 50)
        )
        Dim tree As New Actor.Tree(treePosition, treeTypes(_random.Next(treeTypes.Length)))
        Trees.Add(tree)
        ScheduleEvent_TreePlanted(tree)
    End Sub

    Private Sub KillAllEnemies()
        For Each enemy In Enemies.ToList()
            enemy.Die()
        Next
        Enemies.Clear()
        SpawnEnemies(ENEMIES_TO_SPAWN)
    End Sub
    
    Public Sub HandleInput()
        Dim keyboardState = Keyboard.GetState()
        
        Select Case GameState
            Case GameState.Title
                If keyboardState.IsKeyDown(Keys.Enter) Then
                    GameState = GameState.Playing
                End If
                
            Case GameState.GameOver
                If keyboardState.IsKeyDown(Keys.Enter) Then
                    GameState = GameState.Playing
                End If
        End Select
    End Sub
End Class