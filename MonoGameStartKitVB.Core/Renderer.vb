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
    Private _iconSpriteSheet As Texture2D
    Private _titleCard As Texture2D
    Private _joystickBase As Texture2D
    Private _joystickKnob As Texture2D
    Private _pauseButton As Texture2D
    Private _generalButton As Texture2D

    Private _playerAnimations As Dictionary(Of Direction, Animation)
    Private _playerDeathAnimation As Animation
    Private _enemyAnimations As Dictionary(Of (EnemyType, Direction), Animation)
    Private _joystick As VirtualJoystick

    Public Sub New(graphicsDevice As GraphicsDevice, content As ContentManager)
        _graphicsDevice = graphicsDevice
        _spriteBatch = New SpriteBatch(graphicsDevice)
        _content = content

        _pixelTexture = New Texture2D(graphicsDevice, 1, 1)
        _pixelTexture.SetData({Color.White})
        _renderTarget = New RenderTarget2D(
            graphicsDevice, MAZE_WIDTH * CELL_SIZE, MAZE_HEIGHT * CELL_SIZE
        )

        LoadContent()
    End Sub

    ''' <summary>
    ''' Returns an array of 1-based indices from start to stop, inclusive.
    ''' </summary>
    ''' <param name="start">The 1-based start index.</param>
    ''' <param name="stop">The 1-based stop index.</param>
    ''' <returns>An array of 1-based indices from start to stop, inclusive.</returns>
    Private Shared Function OneBasedIndices(start As Integer, [stop] As Integer) As Integer()
        Return Enumerable.Range(start - 1, [stop] - start + 1).ToArray()
    End Function

    Private Sub LoadContent()
        _playerSpriteSheet = New SpriteSheet(_content, "Images/player_sheet", CELL_SIZE, CELL_SIZE)
        _enemySpriteSheet = New SpriteSheet(_content, "Images/enemy_sheet", CELL_SIZE, CELL_SIZE)
        _objectSpriteSheet = New SpriteSheet(_content, "Images/object_sheet", CELL_SIZE, CELL_SIZE)

        _playerAnimations = New Dictionary(Of Direction, Animation) From {
            {Direction.Left, New Animation(_playerSpriteSheet, OneBasedIndices(1, 2), 0.1F)},
            {Direction.Right, New Animation(_playerSpriteSheet, OneBasedIndices(3, 4), 0.1F)},
            {Direction.Up, New Animation(_playerSpriteSheet, OneBasedIndices(5, 6), 0.1F)},
            {Direction.Down, New Animation(_playerSpriteSheet, OneBasedIndices(7, 8), 0.1F)}
        }
        _playerDeathAnimation = New Animation(_playerSpriteSheet, OneBasedIndices(9, 16), 0.1F)

        _enemyAnimations = New Dictionary(Of (EnemyType, Direction), Animation) From {
            {(EnemyType.Beetle, Direction.Left), New Animation(_enemySpriteSheet, OneBasedIndices(1, 2), 0.1F)},
            {(EnemyType.Beetle, Direction.Right), New Animation(_enemySpriteSheet, OneBasedIndices(3, 4), 0.1F)},
            {(EnemyType.Beetle, Direction.Up), New Animation(_enemySpriteSheet, OneBasedIndices(5, 6), 0.1F)},
            {(EnemyType.Beetle, Direction.Down), New Animation(_enemySpriteSheet, OneBasedIndices(7, 8), 0.1F)},
            {(EnemyType.Caterpillar, Direction.Left), New Animation(_enemySpriteSheet, OneBasedIndices(9, 10), 0.1F)},
            {(EnemyType.Caterpillar, Direction.Right), New Animation(_enemySpriteSheet, OneBasedIndices(11, 12), 0.1F)},
            {(EnemyType.Caterpillar, Direction.Up), New Animation(_enemySpriteSheet, OneBasedIndices(13, 14), 0.1F)},
            {(EnemyType.Caterpillar, Direction.Down), New Animation(_enemySpriteSheet, OneBasedIndices(15, 16), 0.1F)}
        }

        _iconSpriteSheet = _content.Load(Of Texture2D)("Images/icon_sheet")
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
                DrawTitleScreen()

            Case GameState.Playing, GameState.Paused
                DrawGameArea(gameManager, deltaTime)
                DrawHUD(gameManager)
                DrawJoystick()
                DrawPauseButton()

                If gameState = GameState.Paused Then
                    DrawPauseOverlay()
                End If

            Case GameState.GameOver
                DrawGameArea(gameManager, deltaTime)
                DrawHUD(gameManager)
                DrawJoystick()
                DrawPauseButton()
                DrawGameOverScreen(gameManager)

            Case GameState.LevelCleared
                DrawGameArea(gameManager, deltaTime)
                DrawHUD(gameManager)
                DrawJoystick()
                DrawPauseButton()
                DrawLevelClearedScreen(gameManager)
        End Select

        _spriteBatch.End()
    End Sub

    Private Sub DrawTitleScreen()
        Dim titleRect As New Rectangle(
            (SCREEN_WIDTH - _titleCard.Width * 2) \ 2,
            100,
            _titleCard.Width * 2,
            _titleCard.Height * 2
        )
        _spriteBatch.Draw(_titleCard, titleRect, Color.White)

        DrawButton("START", SCREEN_WIDTH \ 2, 400, True)
        DrawButton("EXIT", SCREEN_WIDTH \ 2, 520, False)
    End Sub

    Private Sub DrawButton(text As String, centerX As Integer, y As Integer, isStart As Boolean)
        Dim buttonRect As New Rectangle(
            centerX - _generalButton.Width,
            y,
            _generalButton.Width * 2,
            _generalButton.Height * 2
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
            centerX - textSize.X / 2.0F,
            y + _generalButton.Height - textSize.Y / 2.0F
        )
        _spriteBatch.DrawString(
            _gameFont,
            text,
            textPos,
            Color.Wheat
        )
    End Sub

    Private Sub DrawHUD(gameManager As GameManager)
        DrawText($"1UP: {gameManager.Player.Score:D5}", New Vector2(10, 10), Color.White)
        DrawText($"HI: {gameManager.HighScore:D5}", New Vector2(SCREEN_WIDTH \ 2 + 10, 10), Color.White)

        Dim lifeIconRect As New Rectangle(0, 0, ICON_SIZE, ICON_SIZE)
        For i As Integer = 0 To gameManager.Player.Lives - 1
            _spriteBatch.Draw(
                _iconSpriteSheet,
                New Rectangle(10 + i * (ICON_SIZE * 2 + 4), 30, ICON_SIZE * 2, ICON_SIZE * 2),
                lifeIconRect,
                Color.White
            )
        Next

        Dim seedType = SeedTypeForLevel(gameManager.CurrentLevel)
        Dim seedIconIndex = GetSeedIconIndex(seedType)
        Dim seedIconRect As New Rectangle(seedIconIndex * ICON_SIZE, 0, ICON_SIZE, ICON_SIZE)

        Dim seasonText = $"SEASON {gameManager.CurrentLevel}"
        Dim seasonSize = _gameFont.MeasureString(seasonText)
        Dim seasonX = SCREEN_WIDTH \ 2 + 10
        _spriteBatch.DrawString(_gameFont, seasonText, New Vector2(seasonX, 30), Color.White)
        _spriteBatch.Draw(_iconSpriteSheet, New Rectangle(seasonX + CInt(seasonSize.X) + 8, 30, ICON_SIZE * 2, ICON_SIZE * 2), seedIconRect, Color.White)
    End Sub

    Private Shared Function GetSeedIconIndex(seedType As SeedType) As Integer
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

        DrawMaze(gameManager.Maze, gameManager.CurrentLevel)
        DrawSeeds(gameManager.Seeds)
        DrawPesticides(gameManager.Pesticides)
        DrawPlayer(gameManager.Player, deltaTime)
        DrawEnemies(gameManager.Enemies, deltaTime)

        If gameManager.IsGetReadyActive Then DrawGetReadyMessage()
        _spriteBatch.End()
        _graphicsDevice.SetRenderTarget(Nothing)

        Const HUD_HEIGHT As Integer = 150
        Dim renderRect As New Rectangle(
            0,
            (SCREEN_HEIGHT - HUD_HEIGHT - MAZE_HEIGHT * CELL_SIZE) \ 2,
            SCREEN_WIDTH,
            SCREEN_WIDTH * _renderTarget.Height \ _renderTarget.Width
        )

        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)
        _spriteBatch.Draw(_renderTarget, renderRect, Color.White)
        _spriteBatch.End()
        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)
    End Sub

    Private Sub DrawMaze(maze As MazeTile(,), currentLevel As Integer)
        For x As Integer = 0 To MAZE_WIDTH - 1
            For y As Integer = 0 To MAZE_HEIGHT - 1
                If maze(x, y) = MazeTile.Fence Then
                    Dim rect As New Rectangle(x * CELL_SIZE, y * CELL_SIZE, CELL_SIZE, CELL_SIZE)
                    DrawFence(rect)
                ElseIf maze(x, y) = MazeTile.Sapling OrElse maze(x, y) = MazeTile.Tree Then
                    DrawVegetation(x, y, maze(x, y), currentLevel)
                End If
            Next y
        Next x
    End Sub

    Private Sub DrawVegetation(x As Integer, y As Integer, tileType As MazeTile, Optional currentLevel As Integer = 1)
        Dim frameIndex As Integer
        If tileType = MazeTile.Sapling Then
            frameIndex = 7
        Else
            Dim seedType As SeedType
            Select Case (currentLevel - 1) Mod 6
                Case 0, 3
                    seedType = SeedType.Acorn
                Case 1, 4
                    seedType = SeedType.Berry
                Case 2, 5
                    seedType = SeedType.Nut
                Case Else
                    seedType = SeedType.Acorn
            End Select
            Select Case seedType
                Case SeedType.Acorn
                    frameIndex = 4
                Case SeedType.Berry
                    frameIndex = 5
                Case SeedType.Nut
                    frameIndex = 6
                Case Else
                    frameIndex = 4
            End Select
        End If

        Dim scale = CSng(CELL_SIZE / _objectSpriteSheet.FrameWidth)
        Dim drawPos As New Vector2(x * CELL_SIZE, y * CELL_SIZE)
        _objectSpriteSheet.DrawFrame(_spriteBatch, frameIndex, drawPos, scale, Color.White)
    End Sub

    Private Sub DrawFence(rect As Rectangle)
        Dim scale = CSng(CELL_SIZE / _objectSpriteSheet.FrameWidth)
        _objectSpriteSheet.DrawFrame(_spriteBatch, 0, New Vector2(rect.X, rect.Y), scale, Color.White)
    End Sub

    Private Sub DrawSeeds(seeds As List(Of Actor.Seed))
        For Each seed In seeds.Where(Function(s) s.IsActive)
            Dim scale = CSng(CELL_SIZE / _objectSpriteSheet.FrameWidth)
            Dim frameIndex = GetSeedFrameIndex(seed.SeedType)
            Dim gridPos As New Point(seed.GridPosition.X, seed.GridPosition.Y)
            Dim drawPos As New Vector2(gridPos.X * CELL_SIZE, gridPos.Y * CELL_SIZE)
            _objectSpriteSheet.DrawFrame(_spriteBatch, frameIndex, drawPos, scale, Color.White)
        Next
    End Sub

    Private Shared Function GetSeedFrameIndex(seedType As SeedType) As Integer
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

    Private Sub DrawPesticides(pesticides As List(Of Point))
        For Each item In pesticides
            Dim scale = CSng(CELL_SIZE / _objectSpriteSheet.FrameWidth)
            Dim gridPos As New Point(item.X, item.Y)
            Dim drawPos As New Vector2(gridPos.X * CELL_SIZE, gridPos.Y * CELL_SIZE)
            _objectSpriteSheet.DrawFrame(_spriteBatch, 8, drawPos, scale, Color.White)
        Next
    End Sub

    Private Sub DrawPlayer(player As Actor.Player, deltaTime As Single)
        If Not player.IsAlive Then Exit Sub
        Dim animation As Animation, frameIndex As Integer

        If player.IsInDeathAnimation Then
            animation = _playerDeathAnimation
            animation.Update(deltaTime)
            frameIndex = animation.CurrentFrameIndex
        Else
            animation = _playerAnimations(player.CurrentDirection)
            If player.IsMoving Then animation.Update(deltaTime) Else animation.Reset()
            frameIndex = animation.CurrentFrameIndex
        End If

        Dim scale As Single = CSng(PLAYER_SIZE / animation.SpriteSheet.FrameWidth)
        Dim drawPos As New Vector2(
            player.PixelPosition.X - PLAYER_SIZE \ 2,
            player.PixelPosition.Y - PLAYER_SIZE \ 2
        )

        ' Draw actual player
        animation.SpriteSheet.DrawFrame(_spriteBatch, frameIndex, drawPos, scale, Color.White)
    End Sub

    Private Sub DrawEnemies(enemies As List(Of Actor.Enemy), deltaTime As Single)
        For Each enemy In From e In enemies Where e.IsActive
            Dim direction = enemy.Direction
            Dim key = (enemy.EnemyType, direction)
            Dim animation = _enemyAnimations(key)
            animation.Update(deltaTime)
            Dim frameIndex = animation.CurrentFrameIndex

            Dim scale = CSng(ENEMY_SIZE / animation.SpriteSheet.FrameWidth)
            Dim drawPos As New Vector2(
                enemy.PixelPosition.X - ENEMY_SIZE \ 2,
                enemy.PixelPosition.Y - ENEMY_SIZE \ 2
            )

            Dim enemyColor = Color.White
            If enemy.IsVulnerable Then
                enemyColor = Color.LightBlue
            ElseIf enemy.GracePeriodTimer > 0 Then
                enemyColor = New Color(200, 200, 255, 128)
            End If

            animation.SpriteSheet.DrawFrame(_spriteBatch, frameIndex, drawPos, scale, enemyColor)
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

    Private Sub DrawJoystick()
        Dim keyboardState = Keyboard.GetState()
        Dim touchCollection = Touch.TouchPanel.GetState()
        Dim mouseState = Mouse.GetState()
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

        Dim maxRadius = CSng(_joystickBase.Width)
        For Each touchLoc In touchCollection
            If touchLoc.State = Touch.TouchLocationState.Pressed Then
                Dim delta = touchLoc.Position - _joystick.Position
                If delta.Length() <= maxRadius * 2 Then
                    If delta.Length() > maxRadius Then
                        delta.Normalize()
                        delta *= maxRadius
                    End If
                    joystickValue = delta / maxRadius
                End If
            End If
        Next

        If mouseState.LeftButton = ButtonState.Pressed Then
            Dim mousePos = New Vector2(mouseState.X, mouseState.Y)
            Dim delta = mousePos - _joystick.Position
            If delta.Length() <= maxRadius * 2 Then
                If delta.Length() > maxRadius Then
                    delta.Normalize()
                    delta *= maxRadius
                End If
                joystickValue = delta / maxRadius
            End If
        End If

        _joystick.Value = joystickValue
        _joystick.Draw(_spriteBatch)
    End Sub

    Private Sub DrawPauseButton()
        Dim buttonRect As New Rectangle(
            10, SCREEN_HEIGHT - 100, _pauseButton.Width * 2, _pauseButton.Height * 2
        )
        _spriteBatch.Draw(_pauseButton, buttonRect, Color.White)
    End Sub

    Private Sub DrawPauseOverlay()
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))

        Dim text As String = "PAUSED"
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