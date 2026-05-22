Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.Input

Public NotInheritable Class Renderer
    Implements IDisposable

    Private ReadOnly _spriteBatch As SpriteBatch
    Private ReadOnly _content As ContentManager
    Private ReadOnly _graphicsDevice As GraphicsDevice
    Private ReadOnly _pixelTexture As Texture2D
    Private ReadOnly _renderTarget As RenderTarget2D

    Private _gameFont As SpriteFont
    Private disposedValue As Boolean

    Private _playerSpriteSheet As SpriteSheet
    Private _enemySpriteSheet As SpriteSheet
    Private _objectSpriteSheet As SpriteSheet
    Private _iconSheet As Texture2D
    Private _titleCard As Texture2D
    Private _joystickBase As Texture2D
    Private _joystickKnob As Texture2D
    Private _pauseButton As Texture2D
    Private _generalButton As Texture2D

    Private _playerAnimations As Dictionary(Of Direction, Animation)
    Private _playerDeathAnimation As Animation
    Private _enemyAnimations As Dictionary(Of Tuple(Of EnemyType, Direction), Animation)
    Private _enemyVulnerableAnimations As Dictionary(Of Tuple(Of EnemyType, Direction), Animation)

    Private ReadOnly _hudOffset As Integer = 60
    Private _mazeOffsetX As Integer
    Private _mazeOffsetY As Integer
    Private _joystick As VirtualJoystick

    Public Enum ObjectType
        Fence
        AcornSeed
        BerrySeed
        NutSeed
        Pesticide
    End Enum

    Public Sub New(graphicsDevice As GraphicsDevice, content As ContentManager)
        _graphicsDevice = graphicsDevice
        _spriteBatch = New SpriteBatch(graphicsDevice)
        _content = content

        _pixelTexture = New Texture2D(graphicsDevice, 1, 1)
        _pixelTexture.SetData(New Color() {Color.White})

        _renderTarget = New RenderTarget2D(graphicsDevice, MAZE_WIDTH * CELL_SIZE, MAZE_HEIGHT * CELL_SIZE)

        CalculateMazeOffset()
        LoadContent()
    End Sub

    Private Sub CalculateMazeOffset()
        _mazeOffsetX = (SCREEN_WIDTH - MAZE_WIDTH * CELL_SIZE) \ 2
        _mazeOffsetY = _hudOffset + (SCREEN_HEIGHT - _hudOffset - MAZE_HEIGHT * CELL_SIZE - 150) \ 2
    End Sub

    <CodeAnalysis.SuppressMessage("Performance", "CA1861")>
    Private Sub LoadContent()
        _playerSpriteSheet = New SpriteSheet(_content, "Images/player_sheet", 64, 64)
        _enemySpriteSheet = New SpriteSheet(_content, "Images/enemy_sheet", 64, 64)
        _objectSpriteSheet = New SpriteSheet(_content, "Images/object_sheet", 64, 64)

        _playerAnimations = New Dictionary(Of Direction, Animation) From {
            {Direction.Left, New Animation(_playerSpriteSheet, {1, 2}, 0.1F)},
            {Direction.Right, New Animation(_playerSpriteSheet, {3, 4}, 0.1F)},
            {Direction.Up, New Animation(_playerSpriteSheet, {5, 6}, 0.1F)},
            {Direction.Down, New Animation(_playerSpriteSheet, {7, 8}, 0.1F)}
        }
        _playerDeathAnimation = New Animation(_playerSpriteSheet, Enumerable.Range(9, 8).ToArray(), 0.1F)

        _enemyAnimations = New Dictionary(Of Tuple(Of EnemyType, Direction), Animation) From {
            {Tuple.Create(EnemyType.Beetle, Direction.Left), New Animation(_enemySpriteSheet, {1, 2}, 0.1F)},
            {Tuple.Create(EnemyType.Beetle, Direction.Right), New Animation(_enemySpriteSheet, {3, 4}, 0.1F)},
            {Tuple.Create(EnemyType.Beetle, Direction.Up), New Animation(_enemySpriteSheet, {5, 6}, 0.1F)},
            {Tuple.Create(EnemyType.Beetle, Direction.Down), New Animation(_enemySpriteSheet, {7, 8}, 0.1F)},
            {Tuple.Create(EnemyType.Caterpillar, Direction.Left), New Animation(_enemySpriteSheet, {9, 10}, 0.1F)},
            {Tuple.Create(EnemyType.Caterpillar, Direction.Right), New Animation(_enemySpriteSheet, {11, 12}, 0.1F)},
            {Tuple.Create(EnemyType.Caterpillar, Direction.Up), New Animation(_enemySpriteSheet, {13, 14}, 0.1F)},
            {Tuple.Create(EnemyType.Caterpillar, Direction.Down), New Animation(_enemySpriteSheet, {15, 16}, 0.1F)}
        }

        _enemyVulnerableAnimations = New Dictionary(Of Tuple(Of EnemyType, Direction), Animation) From {
            {Tuple.Create(EnemyType.Beetle, Direction.Left), New Animation(_enemySpriteSheet, {17, 18}, 0.2F)},
            {Tuple.Create(EnemyType.Beetle, Direction.Right), New Animation(_enemySpriteSheet, {19, 20}, 0.2F)},
            {Tuple.Create(EnemyType.Beetle, Direction.Up), New Animation(_enemySpriteSheet, {21, 22}, 0.2F)},
            {Tuple.Create(EnemyType.Beetle, Direction.Down), New Animation(_enemySpriteSheet, {23, 24}, 0.2F)},
            {Tuple.Create(EnemyType.Caterpillar, Direction.Left), New Animation(_enemySpriteSheet, {25, 26}, 0.2F)},
            {Tuple.Create(EnemyType.Caterpillar, Direction.Right), New Animation(_enemySpriteSheet, {27, 28}, 0.2F)},
            {Tuple.Create(EnemyType.Caterpillar, Direction.Up), New Animation(_enemySpriteSheet, {29, 30}, 0.2F)},
            {Tuple.Create(EnemyType.Caterpillar, Direction.Down), New Animation(_enemySpriteSheet, {31, 32}, 0.2F)}
        }

        _iconSheet = _content.Load(Of Texture2D)("Images/icon_sheet")
        _titleCard = _content.Load(Of Texture2D)("Images/title_card")
        _joystickBase = _content.Load(Of Texture2D)("Images/joystick_base")
        _joystickKnob = _content.Load(Of Texture2D)("Images/joystick_knob")
        _pauseButton = _content.Load(Of Texture2D)("Images/pause_button")
        _generalButton = _content.Load(Of Texture2D)("Images/general_button")

        _gameFont = _content.Load(Of SpriteFont)("Fonts/GameFont")

        _joystick = New VirtualJoystick(_joystickBase, _joystickKnob, New Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT - 100))
    End Sub

    Public Sub Render(gameManager As GameManager, gameState As GameState, deltaTime As Single)
        _graphicsDevice.Clear(Color.Black)
        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)

        Select Case gameState
            Case GameState.Title
                DrawTitleScreen(gameManager)

            Case GameState.Playing, GameState.Paused
                DrawHUD(gameManager)
                DrawGameArea(gameManager, deltaTime)
                DrawJoystick(gameManager)
                DrawPauseButton()

                If gameState = GameState.Paused Then
                    DrawPauseOverlay()
                End If

            Case GameState.GameOver
                DrawHUD(gameManager)
                DrawGameArea(gameManager, deltaTime)
                DrawJoystick(gameManager)
                DrawPauseButton()
                DrawGameOverScreen(gameManager)

            Case GameState.LevelCleared
                DrawHUD(gameManager)
                DrawGameArea(gameManager, deltaTime)
                DrawJoystick(gameManager)
                DrawPauseButton()
                DrawLevelClearedScreen(gameManager)
        End Select

        _spriteBatch.End()
    End Sub

    Private Sub DrawTitleScreen(gameManager As GameManager)
        Dim titleRect As New Rectangle(
            (SCREEN_WIDTH - _titleCard.Width) \ 2,
            100,
            _titleCard.Width,
            _titleCard.Height
        )
        _spriteBatch.Draw(_titleCard, titleRect, Color.White)

        DrawButton("START", SCREEN_WIDTH \ 2, 400, True)
        DrawButton("EXIT", SCREEN_WIDTH \ 2, 480, False)
    End Sub

    Private Sub DrawButton(text As String, centerX As Integer, y As Integer, isStart As Boolean)
        Dim buttonRect As New Rectangle(
            centerX - _generalButton.Width \ 2,
            y,
            _generalButton.Width,
            _generalButton.Height
        )

        Dim keyboardState = Keyboard.GetState()
        Dim isPressed As Boolean

        If isStart Then
            isPressed = keyboardState.IsKeyDown(Keys.Enter)
        Else
            isPressed = keyboardState.IsKeyDown(Keys.Escape)
        End If

        Dim buttonColor = If(isPressed, Color.LightGray, Color.White)

        _spriteBatch.Draw(_generalButton, buttonRect, buttonColor)

        Dim textSize As Vector2 = _gameFont.MeasureString(text)
        Dim textPos As New Vector2(
            centerX - textSize.X / 2F,
            y + _generalButton.Height / 2F - textSize.Y / 2F
        )
        _spriteBatch.DrawString(_gameFont, text, textPos, Color.Black)
    End Sub

    Private Sub DrawHUD(gameManager As GameManager)
        DrawText($"1UP: {gameManager.Player.Score:D6}", New Vector2(10, 10), Color.White)
        DrawText($"HI: {gameManager.HighScore:D6}", New Vector2(SCREEN_WIDTH - 150, 10), Color.White)

        Dim lifeIconRect = New Rectangle(0, 0, ICON_SIZE, ICON_SIZE)
        For i As Integer = 0 To gameManager.Player.Lives - 1
            _spriteBatch.Draw(_iconSheet, New Vector2(10 + i * (ICON_SIZE + 4), 35), lifeIconRect, Color.White)
        Next

        Dim seedType = gameManager.GetSeedTypeForCurrentLevel()
        Dim seedIconIndex = GetSeedIconIndex(seedType)
        Dim seedIconRect = New Rectangle(seedIconIndex * ICON_SIZE, 0, ICON_SIZE, ICON_SIZE)

        Dim seasonText = $"SEASON {gameManager.CurrentLevel}"
        Dim seasonSize = _gameFont.MeasureString(seasonText)
        Dim seasonX = SCREEN_WIDTH - seasonSize.X - ICON_SIZE - 10
        _spriteBatch.DrawString(_gameFont, seasonText, New Vector2(seasonX, 35), Color.White)
        _spriteBatch.Draw(_iconSheet, New Vector2(seasonX + seasonSize.X + 4, 35), seedIconRect, Color.White)
    End Sub

    Private Function GetSeedIconIndex(seedType As SeedType) As Integer
        Select Case seedType
            Case SeedType.Acorn
                Return 1
            Case SeedType.Berry
                Return 2
            Case SeedType.Nut
                Return 3
            Case Else
                Return 1
        End Select
    End Function

    Private Sub DrawGameArea(gameManager As GameManager, deltaTime As Single)
        _spriteBatch.End()
        _graphicsDevice.SetRenderTarget(_renderTarget)
        _graphicsDevice.Clear(Color.Black)
        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)

        DrawMaze(gameManager.Maze)
        DrawSeeds(gameManager.Seeds)
        DrawPesticides(gameManager.Pesticides)
        DrawPlayer(gameManager.Player, deltaTime)
        DrawEnemies(gameManager.Enemies, deltaTime)

        If gameManager.IsGetReadyActive Then
            DrawGetReadyMessage()
        End If

        _spriteBatch.End()
        _graphicsDevice.SetRenderTarget(Nothing)

        Dim renderRect As New Rectangle(_mazeOffsetX, _mazeOffsetY, _renderTarget.Width, _renderTarget.Height)
        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)
        _spriteBatch.Draw(_renderTarget, renderRect, Color.White)
        _spriteBatch.End()
        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)
    End Sub

    Private Sub DrawMaze(maze As MazeTile(,))
        For x As Integer = 0 To MAZE_WIDTH - 1
            For y As Integer = 0 To MAZE_HEIGHT - 1
                If maze(x, y) = MazeTile.Fence Then
                    Dim rect As New Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE)
                    _spriteBatch.Draw(_pixelTexture, rect, Color.Blue)
                End If
            Next y
        Next x
    End Sub

    Private Sub DrawSeeds(seeds As List(Of Actor.Seed))
        For Each seed In seeds.Where(Function(s) s.IsActive)
            Dim color = GetSeedColor(seed.SeedType)
            Dim rect As New Rectangle(
                seed.GridPosition.X * CELL_SIZE + CELL_SIZE \ 2 - SEED_SIZE \ 2,
                seed.GridPosition.Y * CELL_SIZE + CELL_SIZE \ 2 - SEED_SIZE \ 2,
                SEED_SIZE,
                SEED_SIZE
            )
            _spriteBatch.Draw(_pixelTexture, rect, color)
        Next
    End Sub

    Private Function GetSeedColor(seedType As SeedType) As Color
        Select Case seedType
            Case SeedType.Acorn
                Return Color.Brown
            Case SeedType.Berry
                Return Color.Red
            Case SeedType.Nut
                Return Color.SandyBrown
            Case Else
                Return Color.Brown
        End Select
    End Function

    Private Sub DrawPesticides(pesticides As List(Of Point))
        For Each pos In pesticides
            Dim rect As New Rectangle(
                pos.X * CELL_SIZE + CELL_SIZE \ 2 - SEED_SIZE \ 2,
                pos.Y * CELL_SIZE + CELL_SIZE \ 2 - SEED_SIZE \ 2,
                SEED_SIZE,
                SEED_SIZE
            )
            _spriteBatch.Draw(_pixelTexture, rect, Color.Cyan)
        Next
    End Sub

    Private Sub DrawPlayer(player As Actor.Player, deltaTime As Single)
        If Not player.IsAlive Then Return

        Dim animation As Animation
        Dim frameRect As Rectangle

        If player.IsInDeathAnimation Then
            animation = _playerDeathAnimation
            animation.Update(deltaTime)
            frameRect = animation.CurrentFrame
        Else
            animation = _playerAnimations(player.CurrentDirection)
            If player.IsMoving Then
                animation.Update(deltaTime)
            Else
                animation.Reset()
            End If
            frameRect = animation.CurrentFrame
        End If

        Dim drawPos = New Vector2(
            player.PixelPosition.X - _mazeOffsetX,
            player.PixelPosition.Y - _mazeOffsetY
        )

        Dim origin As New Vector2(frameRect.Width / 2.0F, frameRect.Height / 2.0F)
        Dim scale As Single = PLAYER_SIZE / MathF.Max(frameRect.Width, frameRect.Height)
        _spriteBatch.Draw(animation.SpriteSheet.Texture, drawPos, frameRect, Color.Yellow, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
    End Sub

    Private Sub DrawEnemies(enemies As List(Of Actor.Enemy), deltaTime As Single)
        For Each enemy In enemies.Where(Function(e) e.IsActive)
            Dim direction = enemy.Direction
            Dim key = Tuple.Create(enemy.EnemyType, direction)

            Dim animation As Animation
            If enemy.IsVulnerable Then
                animation = _enemyVulnerableAnimations(key)
            Else
                animation = _enemyAnimations(key)
            End If

            animation.Update(deltaTime)
            Dim frameRect = animation.CurrentFrame

            Dim drawPos = New Vector2(
                enemy.PixelPosition.X - _mazeOffsetX,
                enemy.PixelPosition.Y - _mazeOffsetY
            )

            Dim origin As New Vector2(frameRect.Width / 2.0F, frameRect.Height / 2.0F)
            Dim scale As Single = ENEMY_SIZE / MathF.Max(frameRect.Width, frameRect.Height)

            Dim enemyColor = Color.White
            If enemy.IsVulnerable Then
                enemyColor = Color.LightBlue
            ElseIf enemy.GracePeriodTimer > 0 Then
                enemyColor = New Color(200, 200, 255, 128)
            End If

            _spriteBatch.Draw(
                animation.SpriteSheet.Texture,
                drawPos,
                frameRect,
                enemyColor,
                0.0F,
                origin,
                scale,
                SpriteEffects.None,
                0.0F)
        Next
    End Sub

    Private Sub DrawGetReadyMessage()
        Dim text = "GET READY!"
        Dim textSize As Vector2 = _gameFont.MeasureString(text)
        Dim position As New Vector2(
            (_renderTarget.Width - textSize.X) / 2,
            (_renderTarget.Height - textSize.Y) / 2
        )

        _spriteBatch.DrawString(_gameFont, text, position, Color.Yellow)
    End Sub

    Private Sub DrawJoystick(gameManager As GameManager)
        Dim keyboardState = Keyboard.GetState()
        Dim joystickValue = Vector2.Zero

        If keyboardState.IsKeyDown(Keys.Left) OrElse keyboardState.IsKeyDown(Keys.A) Then
            joystickValue.X = -1
        End If
        If keyboardState.IsKeyDown(Keys.Right) OrElse keyboardState.IsKeyDown(Keys.D) Then
            joystickValue.X = 1
        End If
        If keyboardState.IsKeyDown(Keys.Up) OrElse keyboardState.IsKeyDown(Keys.W) Then
            joystickValue.Y = -1
        End If
        If keyboardState.IsKeyDown(Keys.Down) OrElse keyboardState.IsKeyDown(Keys.S) Then
            joystickValue.Y = 1
        End If

        _joystick.Value = joystickValue
        _joystick.Draw(_spriteBatch)
    End Sub

    Private Sub DrawPauseButton()
        Dim buttonRect As New Rectangle(10, SCREEN_HEIGHT - 60, _pauseButton.Width, _pauseButton.Height)
        _spriteBatch.Draw(_pauseButton, buttonRect, Color.White)
    End Sub

    Private Sub DrawPauseOverlay()
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))

        Dim text = "PAUSED"
        Dim textSize As Vector2 = _gameFont.MeasureString(text)
        Dim position As New Vector2(
            (SCREEN_WIDTH - textSize.X) / 2,
            (SCREEN_HEIGHT - textSize.Y) / 2
        )
        _spriteBatch.DrawString(_gameFont, text, position, Color.White)
    End Sub

    Private Sub DrawGameOverScreen(gameManager As GameManager)
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))

        DrawCenteredText("GAME OVER", -50, Color.Red, 2.0F)
        DrawCenteredText($"Final Score: {gameManager.Player.Score}", 10, Color.White)
        DrawCenteredText($"High Score: {gameManager.HighScore}", 40, Color.Yellow)
        DrawCenteredText($"Seasons Cleared: {gameManager.CurrentLevel - 1}", 70, Color.Green)
        DrawCenteredText("Press ENTER to restart", 120, Color.White)
    End Sub

    Private Sub DrawLevelClearedScreen(gameManager As GameManager)
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))

        DrawCenteredText($"SEASON {gameManager.CurrentLevel} CLEARED!", -30, Color.Green, 1.5F)
        DrawCenteredText($"Score: {gameManager.Player.Score}", 20, Color.White)
        DrawCenteredText("Get ready for next season...", 60, Color.Yellow)
    End Sub

    Private Sub DrawText(text As String, position As Vector2, color As Color)
        _spriteBatch.DrawString(_gameFont, text, position, color)
    End Sub

    Private Sub DrawCenteredText(text As String, yOffset As Integer, color As Color, Optional scale As Single = 1.0F)
        Dim textSize As Vector2 = _gameFont.MeasureString(text) * scale
        Dim position As New Vector2(
            (SCREEN_WIDTH - textSize.X) / 2,
            (SCREEN_HEIGHT - textSize.Y) / 2 + yOffset
        )
        _spriteBatch.DrawString(_gameFont, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0)
    End Sub

    Private Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                _spriteBatch?.Dispose()
                _pixelTexture?.Dispose()
                _renderTarget?.Dispose()
            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class