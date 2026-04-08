Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

''' <summary>
''' Manages the core game logic, including actors, collisions, and game state.
''' </summary>
Public NotInheritable Class GameManager
    Private ReadOnly _random As New Random

    ' Game objects
    Public Property Player As Actor.Player
    Public ReadOnly Seeds As New List(Of Actor.Seed)
    Public ReadOnly Enemies As New List(Of Actor.Enemy)
    Public ReadOnly Trees As New List(Of Actor.Tree)

    ' Game state
    Public Property GameState As GameState = GameState.Title
    Private _previousGameState As GameState = GameState.Title
    
    ' Game constants
    Public Const SEEDS_TO_SPAWN As Integer = 20
    Public Const ENEMIES_TO_SPAWN As Integer = 8
    
    Public Sub New()
        InitializeGame()
    End Sub
    
    ''' <summary>
    ''' Initializes the game with starting conditions.
    ''' </summary>
    Public Sub InitializeGame()
        Player = New Actor.Player(New Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2)) With {
            .IsAlive = True,  ' Ensure player is alive on restart
            .Score = 0,       ' Reset score
            .SeedsCollected = 0  ' Reset seed count
        }
        Seeds.Clear()
        Enemies.Clear()
        Trees.Clear()
        SpawnSeeds(SEEDS_TO_SPAWN)
        SpawnEnemies(ENEMIES_TO_SPAWN)
    End Sub

    ''' <summary>
    ''' Updates all game objects and handles game logic.
    ''' </summary>
    Public Sub Update(deltaTime As Single)
        ' Update game state first
        HandleGameStateTransitions()

        ' Only update game objects when playing
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

    ''' <summary>
    ''' Handles transitions between game states.
    ''' </summary>
    Private Sub HandleGameStateTransitions()
        If _previousGameState <> GameState Then
            ' Store the previous state before changing
            Dim oldState = _previousGameState
            _previousGameState = GameState
            
            Select Case GameState
                Case GameState.Playing
                    If oldState = GameState.Title Then
                        ' Game starting from title
                        ScheduleEvent_GameStateChanged(GameState)
                    ElseIf oldState = GameState.GameOver Then
                        ' Restarting game from game over
                        InitializeGame()
                        ScheduleEvent_GameStateChanged(GameState)
                    End If
                    
                Case GameState.GameOver
                    ' Game over - always trigger the event
                    ScheduleEvent_GameStateChanged(GameState)
                    
                Case GameState.Title
                    ' Returning to title
                    ScheduleEvent_GameStateChanged(GameState)
            End Select
        End If
    End Sub

    ''' <summary>
    ''' Checks for collisions between game objects.
    ''' </summary>
    Private Sub CheckCollisions()
        Dim playerBounds As Rectangle = Player.GetBounds()

        ' Check seed collisions
        For Each seed In Seeds.Where(Function(s) s.IsActive).ToList()
            If playerBounds.Intersects(seed.GetBounds()) Then
                seed.IsActive = False
                Player.CollectSeed(seed)

                ' Check if player can plant a tree
                If Player.SeedsCollected >= SEEDS_TO_PLANT_TREE Then
                    PlantTree()
                    Player.PlantTree()
                End If

                ' Respawn seeds if all are collected
                If Not Seeds.Any(Function(s) s.IsActive) Then
                    SpawnSeeds(SEEDS_TO_SPAWN)
                End If
            End If
        Next

        ' Check enemy collisions
        For Each enemy In Enemies
            If playerBounds.Intersects(enemy.GetBounds()) Then
                Player.Die()
                GameState = GameState.GameOver
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Spawns seeds at random positions.
    ''' </summary>
    Private Sub SpawnSeeds(count As Integer)
        For i As Integer = 0 To count - 1
            Dim position As New Vector2(
                _random.Next(50, SCREEN_WIDTH - 50),
                _random.Next(50, SCREEN_HEIGHT - 50)
            )
            Seeds.Add(New Actor.Seed(position))
        Next
    End Sub

    ''' <summary>
    ''' Spawns enemies at random positions.
    ''' </summary>
    Private Sub SpawnEnemies(count As Integer)
        For i As Integer = 0 To count - 1
            Dim position As New Vector2(
                _random.Next(50, SCREEN_WIDTH - 50),
                _random.Next(50, SCREEN_HEIGHT - 50)
            )
            Enemies.Add(New Actor.Enemy(position))
        Next
    End Sub

    ''' <summary>
    ''' Plants a tree at a random position.
    ''' </summary>
    Private Sub PlantTree()
        Dim treePosition As New Vector2(
            _random.Next(50, SCREEN_WIDTH - 50),
            _random.Next(50, SCREEN_HEIGHT - 50)
        )
        Dim tree As New Actor.Tree(treePosition)
        Trees.Add(tree)
        ScheduleEvent_TreePlanted(tree)
    End Sub
    
    ''' <summary>
    ''' Handles input for game state transitions.
    ''' </summary>
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