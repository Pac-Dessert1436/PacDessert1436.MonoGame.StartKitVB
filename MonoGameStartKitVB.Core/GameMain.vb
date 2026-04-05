Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Media

Public NotInheritable Class GameMain
    Inherits Game

    Private ReadOnly graphics As GraphicsDeviceManager
    Private spriteBatch As SpriteBatch
    Private pixelTexture As Texture2D

    ' Image textures
    Private playerTexture As Texture2D
    Private seedTexture As Texture2D
    Private enemyTexture As Texture2D
    Private treeTexture As Texture2D

    Private player As Actor.Player
    Private ReadOnly seeds As New List(Of Actor.Seed)
    Private ReadOnly enemies As New List(Of Actor.Enemy)
    Private ReadOnly trees As New List(Of Actor.Tree)
    Private ReadOnly random As New Random

    ' Sound effects and music
    'Private bgmMainTheme As Song  ' will be played looping
    Private sndSeedPacket As SoundEffect
    Private sndGameStart As SoundEffect
    Private sndGameOver As SoundEffect
    'Private sndEnemyKilled As SoundEffect
    'Private sndLifeLost As SoundEffect
    Private sndLevelCleared As SoundEffect
    'Private sndProceeding As SoundEffect
    'Private sndPesticide As SoundEffect

    ' Sound state tracking
    Private isMusicPlaying As Boolean = False

    Private _gameState As GameState = GameState.Title
    Private Const SEEDS_TO_SPAWN As Integer = 20
    Private Const ENEMIES_TO_SPAWN As Integer = 8

    Public Sub New()
        graphics = New GraphicsDeviceManager(Me) With {
            .PreferredBackBufferWidth = SCREEN_WIDTH,
            .PreferredBackBufferHeight = SCREEN_HEIGHT
        }
        graphics.ApplyChanges()
        Content.RootDirectory = "Content"
        IsMouseVisible = False
    End Sub

    Private Sub OnGameStateChanged(newState As GameState)
        ' Handle sound effects for state transitions
        Select Case newState
            Case GameState.Title
                sndGameStart.Play()
            Case GameState.GameOver
                sndGameOver.Play()
                '    'StopBackgroundMusic()
                'Case GameState.Title
                '    'StopBackgroundMusic()
        End Select

        _gameState = newState
    End Sub

    Protected Overrides Sub Initialize()
        AddHandler GameStateChanged, AddressOf OnGameStateChanged
        InitializeGame()
        MyBase.Initialize()
    End Sub

    Protected Overrides Sub LoadContent()
        spriteBatch = New SpriteBatch(GraphicsDevice)
        pixelTexture = New Texture2D(GraphicsDevice, 1, 1)
        pixelTexture.SetData(New Color() {Color.White})

        ' Load image textures
        playerTexture = Content.Load(Of Texture2D)("Images/player")
        seedTexture = Content.Load(Of Texture2D)("Images/acorn_packet")
        enemyTexture = Content.Load(Of Texture2D)("Images/beetle")
        treeTexture = Content.Load(Of Texture2D)("Images/oak_tree")

        ' Load music and sound effects
        'bgmMainTheme = Content.Load(Of Song)("Sounds/BGM/main_theme")
        sndSeedPacket = Content.Load(Of SoundEffect)("Sounds/seed_packet")
        sndGameStart = Content.Load(Of SoundEffect)("Sounds/game_start")
        sndGameOver = Content.Load(Of SoundEffect)("Sounds/game_over")
        'sndEnemyKilled = Content.Load(Of SoundEffect)("Sounds/enemy_killed")
        'sndLifeLost = Content.Load(Of SoundEffect)("Sounds/life_lost")
        sndLevelCleared = Content.Load(Of SoundEffect)("Sounds/level_cleared")
        'sndProceeding = Content.Load(Of SoundEffect)("Sounds/proceeding")
        'sndPesticide = Content.Load(Of SoundEffect)("Sounds/pesticide")

        RaiseEvent_GameStateChanged(GameState.Title)
    End Sub

    Private Sub InitializeGame()
        player = New Actor.Player(New Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2))
        seeds.Clear()
        enemies.Clear()
        trees.Clear()
        SpawnSeeds(SEEDS_TO_SPAWN)
        SpawnEnemies(ENEMIES_TO_SPAWN)
    End Sub

    Private Sub SpawnSeeds(count As Integer)
        For i As Integer = 0 To count - 1
            Dim position As New Vector2(
                random.Next(50, SCREEN_WIDTH - 50),
                random.Next(50, SCREEN_HEIGHT - 50)
            )
            seeds.Add(New Actor.Seed(position))
        Next
    End Sub

    Private Sub SpawnEnemies(count As Integer)
        For i As Integer = 0 To count - 1
            Dim position As New Vector2(
                random.Next(50, SCREEN_WIDTH - 50),
                random.Next(50, SCREEN_HEIGHT - 50)
            )
            enemies.Add(New Actor.Enemy(position))
        Next
    End Sub

    Protected Overrides Sub Update(gameTime As GameTime)
        If GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed OrElse
           Keyboard.GetState().IsKeyDown(Keys.Escape) Then [Exit]()

        Dim deltaTime As Single = CSng(gameTime.ElapsedGameTime.TotalSeconds)

        Select Case _gameState
            Case GameState.Title
                If Keyboard.GetState().IsKeyDown(Keys.Enter) Then
                    ScheduleEvent_GameStateChanged(GameState.Playing)
                End If

            Case GameState.Playing
                UpdatePlayer(deltaTime)
                UpdateEnemies(deltaTime)
                CheckCollisions()

                If Not player.IsAlive Then
                    ScheduleEvent_GameStateChanged(GameState.GameOver)
                End If

            Case GameState.GameOver
                If Keyboard.GetState().IsKeyDown(Keys.Enter) Then
                    InitializeGame()
                    ScheduleEvent_GameStateChanged(GameState.Playing)
                End If
        End Select

        MyBase.Update(gameTime)
    End Sub

    Private Sub UpdatePlayer(deltaTime As Single)
        Dim keyboardState As KeyboardState = Keyboard.GetState()
        Dim movement As Vector2 = Vector2.Zero
        If keyboardState.IsKeyDown(Keys.Left) OrElse keyboardState.IsKeyDown(Keys.A) Then
            movement.X -= 1
        End If
        If keyboardState.IsKeyDown(Keys.Right) OrElse keyboardState.IsKeyDown(Keys.D) Then
            movement.X += 1
        End If
        If keyboardState.IsKeyDown(Keys.Up) OrElse keyboardState.IsKeyDown(Keys.W) Then
            movement.Y -= 1
        End If
        If keyboardState.IsKeyDown(Keys.Down) OrElse keyboardState.IsKeyDown(Keys.S) Then
            movement.Y += 1
        End If

        If movement <> Vector2.Zero Then
            movement.Normalize()
            player.Position += movement * PLAYER_SPEED * deltaTime

            Dim newX = Math.Max(player.Size / 2.0F, Math.Min(SCREEN_WIDTH - player.Size / 2.0F, player.Position.X))
            Dim newY = Math.Max(player.Size / 2.0F, Math.Min(SCREEN_HEIGHT - player.Size / 2.0F, player.Position.Y))
            player.Position = New Vector2(newX, newY)
        End If
    End Sub

    Private Sub UpdateEnemies(deltaTime As Single)
        For Each enemy In enemies
            enemy.Position += enemy.Direction * ENEMY_SPEED * deltaTime

            Dim newDir As Vector2 = enemy.Direction
            Dim newPos As Vector2 = enemy.Position
            If enemy.Position.X <= enemy.Size / 2 OrElse enemy.Position.X >= SCREEN_WIDTH - enemy.Size / 2 Then
                newDir.X *= -1
                newPos.X = Math.Max(enemy.Size / 2.0F, Math.Min(SCREEN_WIDTH - enemy.Size / 2.0F, enemy.Position.X))
            End If
            If enemy.Position.Y <= enemy.Size / 2 OrElse enemy.Position.Y >= SCREEN_HEIGHT - enemy.Size / 2 Then
                newDir.Y *= -1
                newPos.Y = Math.Max(enemy.Size / 2.0F, Math.Min(SCREEN_HEIGHT - enemy.Size / 2.0F, enemy.Position.Y))
            End If

            If random.Next(0, 100) < 1 Then enemy.SetRandomDirection()
            enemy.Direction = newDir
            enemy.Position = newPos
        Next
    End Sub

    Private Sub CheckCollisions()
        Dim playerBounds As Rectangle = player.GetBounds()

        For Each seed In seeds.Where(Function(s) s.IsActive).ToList()
            If playerBounds.Intersects(seed.GetBounds()) Then
                seed.IsActive = False
                player.Score += 10
                player.SeedsCollected += 1
                sndSeedPacket.Play()  ' Play seed collection sound

                If player.SeedsCollected >= SEEDS_TO_PLANT_TREE Then
                    PlantTree()
                    player.SeedsCollected = 0
                    sndLevelCleared.Play()  ' Play tree planting sound
                End If

                If Not Aggregate s In seeds Into Any(s.IsActive) Then SpawnSeeds(SEEDS_TO_SPAWN)
            End If
        Next

        For Each enemy In enemies
            If playerBounds.Intersects(enemy.GetBounds()) Then
                player.IsAlive = False
                'sndLifeLost.Play()  
            End If
        Next enemy
    End Sub

    Private Sub PlantTree()
        Dim treePosition As New Vector2(
            random.Next(50, SCREEN_WIDTH - 50),
            random.Next(50, SCREEN_HEIGHT - 50)
        )
        trees.Add(New Actor.Tree(treePosition))
    End Sub

    'Private Sub PlayBackgroundMusic()
    '    If Not isMusicPlaying Then
    '        MediaPlayer.Play(bgmMainTheme)
    '        MediaPlayer.IsRepeating = True
    '        isMusicPlaying = True
    '    End If
    'End Sub

    'Private Sub StopBackgroundMusic()
    '    If isMusicPlaying Then
    '        MediaPlayer.Stop()
    '        isMusicPlaying = False
    '    End If
    'End Sub

    Protected Overrides Sub Draw(gameTime As GameTime)
        GraphicsDevice.Clear(Color.Black)
        RaiseScheduledEvents()  ' Required for scheduled events
        spriteBatch.Begin(samplerState:=SamplerState.PointClamp)

        Select Case _gameState
            Case GameState.Title
                DrawTitleScreen()

            Case GameState.Playing
                DrawGame()

            Case GameState.GameOver
                DrawGame()
                DrawGameOverScreen()
        End Select

        spriteBatch.End()
        MyBase.Draw(gameTime)
    End Sub

    Private Sub DrawTitleScreen()
        DrawCenteredText("DEVOUR-MAN", -50, Color.Yellow, 2.0F)
        DrawCenteredText("Eat seeds & plant a forest!", 20, Color.Green)
        DrawCenteredText("Press ENTER to start", 80, Color.White)
        DrawCenteredText("Arrow Keys or WASD to move", 120, Color.Gray)
    End Sub

    Private Sub DrawGame()
        DrawPlayer()
        DrawSeeds()
        DrawEnemies()
        DrawTrees()
        DrawHUD()
    End Sub

    Private Sub DrawPlayer()
        Dim origin As New Vector2(playerTexture.Width / 2.0F, playerTexture.Height / 2.0F)
        Dim scale As Single = player.Size / MathF.Max(playerTexture.Width, playerTexture.Height)
        spriteBatch.Draw(playerTexture, player.Position, Nothing, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
    End Sub

    Private Sub DrawSeeds()
        For Each seed In seeds.Where(Function(s) s.IsActive)
            Dim origin As New Vector2(seedTexture.Width / 2.0F, seedTexture.Height / 2.0F)
            Dim scale As Single = seed.Size / MathF.Max(seedTexture.Width, seedTexture.Height)
            spriteBatch.Draw(seedTexture, seed.Position, Nothing, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    Private Sub DrawEnemies()
        For Each enemy In enemies
            Dim origin As New Vector2(enemyTexture.Width / 2.0F, enemyTexture.Height / 2.0F)
            Dim scale As Single = enemy.Size / MathF.Max(enemyTexture.Width, enemyTexture.Height)
            spriteBatch.Draw(enemyTexture, enemy.Position, Nothing, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    Private Sub DrawTrees()
        For Each tree In trees
            Dim origin As New Vector2(treeTexture.Width / 2.0F, treeTexture.Height)
            Dim scale As Single = tree.Size / MathF.Max(treeTexture.Width, treeTexture.Height)
            spriteBatch.Draw(treeTexture, tree.Position, Nothing, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    Private Sub DrawHUD()
        DrawText($"Score: {player.Score}", New Vector2(10, 10), Color.White)
        DrawText($"Seeds: {player.SeedsCollected}/{SEEDS_TO_PLANT_TREE}", New Vector2(10, 35), Color.Green)
        DrawText($"Trees: {trees.Count}", New Vector2(10, 60), Color.ForestGreen)
    End Sub

    Private Sub DrawGameOverScreen()
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        spriteBatch.Draw(pixelTexture, overlayRect, New Color(0, 0, 0, 150))
        DrawCenteredText("GAME OVER", -30, Color.Red, 2.0F)
        DrawCenteredText($"Final Score: {player.Score}", 20, Color.White)
        DrawCenteredText($"Trees Planted: {trees.Count}", 50, Color.ForestGreen)
        DrawCenteredText("Press ENTER to restart", 100, Color.White)
    End Sub

    Private Sub DrawCircle(center As Vector2, radius As Single, color As Color)
        Dim diameter As Integer = CInt(radius * 2)
        Dim rect As New Rectangle(CInt(center.X - radius), CInt(center.Y - radius), diameter, diameter)
        spriteBatch.Draw(pixelTexture, rect, color)
    End Sub

    Private Sub DrawText(text As String, position As Vector2, color As Color)
        spriteBatch.DrawString(Content.Load(Of SpriteFont)("Fonts/GameFont"), text, position, color)
    End Sub

    Private Sub DrawCenteredText(text As String, yOffset As Integer, color As Color, Optional scale As Single = 1.0F)
        Dim font As SpriteFont = Content.Load(Of SpriteFont)("Fonts/GameFont")
        Dim textSize As Vector2 = font.MeasureString(text) * scale
        Dim position As New Vector2(
            (SCREEN_WIDTH - textSize.X) / 2,
            (SCREEN_HEIGHT - textSize.Y) / 2 + yOffset
        )
        spriteBatch.DrawString(font, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0)
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        ' Critical: Remove all event handlers before disposing
        RemoveHandler GameStateChanged, AddressOf OnGameStateChanged

        If disposing Then
            ' TODO: Dispose of managed resources
        End If

        MyBase.Dispose(disposing)
    End Sub
End Class