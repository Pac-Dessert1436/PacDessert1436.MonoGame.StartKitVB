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

    Private _renderTarget As RenderTarget2D
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

    Private Shared _screenScale As Single = 1.0F
    Private Shared _screenOffset As Vector2 = Vector2.Zero
    Private Const HUD_HEIGHT As Integer = 100

    Public Shared ReadOnly Property ScreenScale As Single
        Get
            Return _screenScale
        End Get
    End Property

    Public Shared ReadOnly Property ScreenOffset As Vector2
        Get
            Return _screenOffset
        End Get
    End Property

    Private Shared _actualScreenWidth As Integer = SCREEN_WIDTH
    Private Shared _actualScreenHeight As Integer = SCREEN_HEIGHT
    Private Shared _pauseButtonWidth As Integer = 0
    Private Shared _pauseButtonHeight As Integer = 0

    Public Shared ReadOnly Property ActualScreenWidth As Integer
        Get
            Return _actualScreenWidth
        End Get
    End Property

    Public Shared ReadOnly Property ActualScreenHeight As Integer
        Get
            Return _actualScreenHeight
        End Get
    End Property

    Public Shared Property PauseButtonWidth As Integer
        Get
            Return _pauseButtonWidth
        End Get
        Set(value As Integer)
            _pauseButtonWidth = value
        End Set
    End Property

    Public Shared Property PauseButtonHeight As Integer
        Get
            Return _pauseButtonHeight
        End Get
        Set(value As Integer)
            _pauseButtonHeight = value
        End Set
    End Property

    Public Sub New(graphicsDevice As GraphicsDevice, content As ContentManager)
        _graphicsDevice = graphicsDevice
        _spriteBatch = New SpriteBatch(graphicsDevice)
        _content = content

        _pixelTexture = New Texture2D(graphicsDevice, 1, 1)
        _pixelTexture.SetData({Color.White})

        UpdateScreenScale()
        CreateRenderTarget()
        LoadContent()
    End Sub

    Private Sub UpdateScreenScale()
        Dim newScreenWidth = _graphicsDevice.PresentationParameters.BackBufferWidth
        Dim newScreenHeight = _graphicsDevice.PresentationParameters.BackBufferHeight

        If newScreenWidth <> _actualScreenWidth OrElse newScreenHeight <> _actualScreenHeight Then
            _actualScreenWidth = newScreenWidth
            _actualScreenHeight = newScreenHeight

            Dim scaleX = CSng(_actualScreenWidth) / SCREEN_WIDTH
            Dim scaleY = CSng(_actualScreenHeight) / SCREEN_HEIGHT
            _screenScale = Math.Min(scaleX, scaleY)

            _screenOffset.X = (_actualScreenWidth - SCREEN_WIDTH * _screenScale) / 2
            _screenOffset.Y = (_actualScreenHeight - SCREEN_HEIGHT * _screenScale) / 2

            CreateRenderTarget()
        End If
    End Sub

    Private Sub CreateRenderTarget()
        _renderTarget?.Dispose()
        _renderTarget = New RenderTarget2D(_graphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT)
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

        Actor.Player.JoystickBaseWidth = _joystickBase.Width
        PauseButtonWidth = _pauseButton.Width
        PauseButtonHeight = _pauseButton.Height
        _joystick = New VirtualJoystick(_joystickBase, _joystickKnob, New Vector2(SCREEN_WIDTH / 2, SCREEN_HEIGHT - 100))
    End Sub

    Public Sub Render(gameManager As GameManager, gameState As GameState, deltaTime As Single)
        UpdateScreenScale()

        _graphicsDevice.SetRenderTarget(_renderTarget)
        _graphicsDevice.Clear(Color.LightSeaGreen)
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
                DrawLevelClearedScreen()
        End Select

        _spriteBatch.End()
        _graphicsDevice.SetRenderTarget(Nothing)
        _graphicsDevice.Clear(Color.Black)

        Dim renderWidth = CInt(SCREEN_WIDTH * _screenScale)
        Dim renderHeight = CInt(SCREEN_HEIGHT * _screenScale)
        Dim renderRect As New Rectangle(
            CInt(_screenOffset.X), CInt(_screenOffset.Y), renderWidth, renderHeight
        )
        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)
        _spriteBatch.Draw(_renderTarget, renderRect, Color.White)
        _spriteBatch.End()
    End Sub

    Private Sub DrawTitleScreen()
        Dim titleRect As New Rectangle(
            CInt((SCREEN_WIDTH - _titleCard.Width * 2) / 2),
            100,
            _titleCard.Width * 2,
            _titleCard.Height * 2
        )
        _spriteBatch.Draw(_titleCard, titleRect, Color.White)

        DrawButton("START", CInt(SCREEN_WIDTH / 2), 400, 1.0F)
        DrawButton("EXIT", CInt(SCREEN_WIDTH / 2), 520, 1.0F)

        Static instructions As String() = {
            "--- HOW TO PLAY ---",
            "Move with joystick/keyboard",
            "Collect seeds to plant trees",
            "Pesticide weakens enemies",
            "Survive as long as possible"
        }

        Dim bonusLifeStr = $"BONUS LIFE AT {BONUS_LIFE_AT} PTS."
        _spriteBatch.DrawString(_gameFont,
            bonusLifeStr,
            New Vector2((SCREEN_WIDTH - _gameFont.MeasureString(bonusLifeStr).X) / 2, 300),
            Color.MintCream
        )
        Dim instrY As Integer = 700
        For Each instr As String In instructions
            Dim instructionPos As New Vector2(
                (SCREEN_WIDTH - _gameFont.MeasureString(instr).X) / 2,
                instrY
            )
            _spriteBatch.DrawString(_gameFont, instr, instructionPos, Color.White)
            instrY += 50
        Next
    End Sub

    Private Sub DrawButton(text As String, centerX As Integer, y As Integer, Optional scale As Single = 1.0F)
        Dim buttonWidth = CInt(_generalButton.Width * 2 * scale)
        Dim buttonHeight = CInt(_generalButton.Height * 2 * scale)
        Dim buttonRect As New Rectangle(
            centerX - buttonWidth \ 2,
            y,
            buttonWidth,
            buttonHeight
        )

        Dim keyboardState = Keyboard.GetState()
        Dim isPressed = keyboardState.IsKeyDown(Keys.Enter)

        Dim buttonColor = If(isPressed, Color.LightGray, Color.White)
        _spriteBatch.Draw(_generalButton, buttonRect, buttonColor)

        Dim textSize As Vector2 = _gameFont.MeasureString(text) * scale
        Dim textPos As New Vector2(
            centerX - textSize.X / 2.0F,
            y + buttonHeight / 2.0F - textSize.Y / 2.0F
        )
        _spriteBatch.DrawString(
            _gameFont,
            text,
            textPos,
            Color.Wheat,
            0,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0
        )
    End Sub

    Private Sub DrawHUD(gameManager As GameManager)
        With gameManager
            DrawText($"1UP { .Player.Score,6}", New Vector2(10, 10), Color.MintCream, 1.0F)
            DrawText(
                If(.Player.Score > .HighScore, "-NEW BEST-", $"HI. { .HighScore,6}"),
                New Vector2(SCREEN_WIDTH \ 2 + 10, 10),
                Color.MintCream, 1.0F)
        End With

        Dim lifeIconRect As New Rectangle(0, 0, ICON_SIZE, ICON_SIZE)
        For i As Integer = 0 To gameManager.Player.Lives - 1
            _spriteBatch.Draw(
                _iconSpriteSheet,
                New Rectangle(
                    10 + i * (ICON_SIZE * 2 + 4),
                    35,
                    ICON_SIZE * 2,
                    ICON_SIZE * 2
                ),
                lifeIconRect,
                Color.White
            )
        Next

        Dim seedType = SeedTypeForLevel(gameManager.CurrentLevel)
        Dim seedIconIndex = GetSeedIconIndex(seedType)
        Dim seedIconRect As New Rectangle(seedIconIndex * ICON_SIZE, 0, ICON_SIZE, ICON_SIZE)

        Dim seasonText = $"SEASON {gameManager.CurrentLevel,2}"
        Dim seasonSize = _gameFont.MeasureString(seasonText)
        Dim seasonX = SCREEN_WIDTH \ 2 - 50
        Dim seasonY = 45
        _spriteBatch.DrawString(_gameFont, seasonText, New Vector2(seasonX, seasonY), Color.White)
        _spriteBatch.Draw(
            _iconSpriteSheet,
            New Rectangle(
                CInt(seasonX + seasonSize.X + 8),
                seasonY,
                ICON_SIZE * 2,
                ICON_SIZE * 2
            ),
            seedIconRect,
            Color.White
        )

        If gameManager.IsGetReadyActive Then
            _spriteBatch.DrawString(_gameFont, "GET READY!", New Vector2(10, 70), Color.Yellow)
        End If
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
        DrawMaze(gameManager.Maze, gameManager.CurrentLevel)
        DrawSpawnPoints(gameManager.Enemies)
        DrawSeeds(gameManager.Seeds)
        DrawPesticides(gameManager.Pesticides)
        DrawPlayer(gameManager.Player, deltaTime)
        DrawEnemies(gameManager.Enemies, deltaTime)
    End Sub

    Private Sub DrawSpawnPoints(enemies As List(Of Actor.Enemy))
        Dim renderScale = CSng(SCREEN_WIDTH) / (MAZE_WIDTH * CELL_SIZE)
        
        For Each enemy In enemies
            Dim rect As New Rectangle(
                CInt(enemy.SpawnPoint.X * CELL_SIZE * renderScale),
                CInt(enemy.SpawnPoint.Y * CELL_SIZE * renderScale + HUD_HEIGHT),
                CInt(CELL_SIZE * renderScale),
                CInt(CELL_SIZE * renderScale)
            )
            
            _spriteBatch.Draw(_pixelTexture, rect, Color.Red * 0.5F)
            
            Dim borderThickness = CInt(2 * renderScale)
            If borderThickness < 1 Then borderThickness = 1
            
            _spriteBatch.Draw(_pixelTexture, New Rectangle(rect.X, rect.Y, rect.Width, borderThickness), Color.Red)
            _spriteBatch.Draw(_pixelTexture, New Rectangle(rect.X, rect.Y, borderThickness, rect.Height), Color.Red)
            _spriteBatch.Draw(_pixelTexture, New Rectangle(rect.X, rect.Y + rect.Height - borderThickness, rect.Width, borderThickness), Color.Red)
            _spriteBatch.Draw(_pixelTexture, New Rectangle(rect.X + rect.Width - borderThickness, rect.Y, borderThickness, rect.Height), Color.Red)
        Next
    End Sub

    Private Sub DrawMaze(maze As MazeTile(,), currentLevel As Integer)
        Dim scale = CSng(SCREEN_WIDTH) / (MAZE_WIDTH * CELL_SIZE)

        For x As Integer = 0 To MAZE_WIDTH - 1
            For y As Integer = 0 To MAZE_HEIGHT - 1
                If maze(x, y) = MazeTile.Fence Then
                    Dim rect As New Rectangle(
                        CInt(x * CELL_SIZE * scale),
                        CInt(y * CELL_SIZE * scale + HUD_HEIGHT),
                        CInt(CELL_SIZE * scale),
                        CInt(CELL_SIZE * scale)
                    )
                    DrawFence(rect, scale)
                ElseIf maze(x, y) = MazeTile.Sapling OrElse maze(x, y) = MazeTile.Tree Then
                    DrawVegetation(x, y, maze(x, y), currentLevel, scale, HUD_HEIGHT)
                End If
            Next y
        Next x
    End Sub

    Private Sub DrawVegetation(x As Integer, y As Integer, tileType As MazeTile, Optional currentLevel As Integer = 1, Optional scale As Single = 1.0F, Optional yOffset As Integer = 0)
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

        Dim cellScale = CSng(CELL_SIZE / _objectSpriteSheet.FrameWidth) * scale
        Dim drawPos As New Vector2(x * CELL_SIZE * scale, y * CELL_SIZE * scale + yOffset)
        _objectSpriteSheet.DrawFrame(_spriteBatch, frameIndex, drawPos, cellScale, Color.White)
    End Sub

    Private Sub DrawFence(rect As Rectangle, Optional scale As Single = 1.0F)
        Dim cellScale = CSng(CELL_SIZE / _objectSpriteSheet.FrameWidth) * scale
        _objectSpriteSheet.DrawFrame(_spriteBatch, 0, New Vector2(rect.X, rect.Y), cellScale, Color.White)
    End Sub

    Private Sub DrawSeeds(seeds As List(Of Actor.Seed))
        Dim renderScale = CSng(SCREEN_WIDTH) / (MAZE_WIDTH * CELL_SIZE)
        For Each seed In From s In seeds Where s.IsActive
            Dim scale = CSng(CELL_SIZE / _objectSpriteSheet.FrameWidth) * renderScale
            Dim frameIndex = GetSeedFrameIndex(seed.SeedType)
            Dim gridPos As New Point(seed.GridPosition.X, seed.GridPosition.Y)
            Dim drawPos As New Vector2(gridPos.X * CELL_SIZE * renderScale, gridPos.Y * CELL_SIZE * renderScale + HUD_HEIGHT)
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
        Dim renderScale = CSng(SCREEN_WIDTH) / (MAZE_WIDTH * CELL_SIZE)
        For Each item In pesticides
            Dim scale = CSng(CELL_SIZE / _objectSpriteSheet.FrameWidth) * renderScale
            Dim gridPos As New Point(item.X, item.Y)
            Dim drawPos As New Vector2(gridPos.X * CELL_SIZE * renderScale, gridPos.Y * CELL_SIZE * renderScale + HUD_HEIGHT)
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

        Dim renderScale = CSng(SCREEN_WIDTH) / (MAZE_WIDTH * CELL_SIZE)
        Dim scale As Single = CSng(PLAYER_SIZE / animation.SpriteSheet.FrameWidth) * renderScale
        Dim drawPos As New Vector2(
            player.PixelPosition.X * renderScale - PLAYER_SIZE * renderScale / 2,
            player.PixelPosition.Y * renderScale - PLAYER_SIZE * renderScale / 2 + HUD_HEIGHT
        )

        ' Draw actual player
        animation.SpriteSheet.DrawFrame(_spriteBatch, frameIndex, drawPos, scale, Color.White)
    End Sub

    Private Sub DrawEnemies(enemies As List(Of Actor.Enemy), deltaTime As Single)
        Dim renderScale = CSng(SCREEN_WIDTH) / (MAZE_WIDTH * CELL_SIZE)

        For Each enemy In From e In enemies Where e.IsActive AndAlso Not e.IsVulnerable
            Dim direction = enemy.Direction
            Dim key = (enemy.EnemyType, direction)
            Dim animation = _enemyAnimations(key)
            animation.Update(deltaTime)
            Dim frameIndex = animation.CurrentFrameIndex

            Dim scale = CSng(ENEMY_SIZE / animation.SpriteSheet.FrameWidth) * renderScale
            Dim drawPos As New Vector2(
                enemy.PixelPosition.X * renderScale - ENEMY_SIZE * renderScale / 2,
                enemy.PixelPosition.Y * renderScale - ENEMY_SIZE * renderScale / 2 + HUD_HEIGHT
            )

            Dim enemyColor = If(enemy.GracePeriodTimer > 0, New Color(200, 200, 255, 128), Color.White)
            animation.SpriteSheet.DrawFrame(_spriteBatch, frameIndex, drawPos, scale, enemyColor)
        Next

        Dim frightenedEnemies = From e In enemies Where e.IsActive AndAlso e.IsVulnerable
        If frightenedEnemies.Any() Then
            Dim blinkValue = Math.Abs(Math.Sin(Date.Now.TimeOfDay.TotalSeconds * 8.0))
            Dim frightenedColor As Color
            If blinkValue > 0.5 Then
                frightenedColor = Color.White
            Else
                frightenedColor = Color.Blue
            End If

            For Each enemy In frightenedEnemies
                Dim direction = enemy.Direction
                Dim key = (enemy.EnemyType, direction)
                Dim animation = _enemyAnimations(key)
                animation.Update(deltaTime)
                Dim frameIndex = animation.CurrentFrameIndex

                Dim scale = CSng(ENEMY_SIZE / animation.SpriteSheet.FrameWidth) * renderScale
                Dim drawPos As New Vector2(
                    enemy.PixelPosition.X * renderScale - ENEMY_SIZE * renderScale / 2,
                    enemy.PixelPosition.Y * renderScale - ENEMY_SIZE * renderScale / 2 + HUD_HEIGHT
                )

                animation.SpriteSheet.DrawFrame(_spriteBatch, frameIndex, drawPos, scale, frightenedColor)
            Next
        End If
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

        Dim joystickCenter = New Vector2(
            SCREEN_WIDTH / 2.0F,
            SCREEN_HEIGHT - _joystickBase.Height * 2 - 10
        )
        Dim maxRadius = CSng(_joystickBase.Width * 2)

        For Each touchLoc In touchCollection
            If touchLoc.State = Touch.TouchLocationState.Pressed OrElse
               touchLoc.State = Touch.TouchLocationState.Moved Then
                Dim screenPos = touchLoc.Position
                Dim renderPos = New Vector2(
                    (screenPos.X - _screenOffset.X) / _screenScale,
                    (screenPos.Y - _screenOffset.Y) / _screenScale
                )
                Dim delta = renderPos - joystickCenter
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
            Dim screenPos = New Vector2(mouseState.X, mouseState.Y)
            Dim renderPos = New Vector2(
                (screenPos.X - _screenOffset.X) / _screenScale,
                (screenPos.Y - _screenOffset.Y) / _screenScale
            )
            Dim delta = renderPos - joystickCenter
            If delta.Length() <= maxRadius * 2 Then
                If delta.Length() > maxRadius Then
                    delta.Normalize()
                    delta *= maxRadius
                End If
                joystickValue = delta / maxRadius
            End If
        End If

        _joystick.Position = joystickCenter
        _joystick.Value = joystickValue
        _joystick.Draw(_spriteBatch, 1.0F)
    End Sub

    Private Sub DrawPauseButton()
        Dim scale = 2.0F
        Dim buttonRect As New Rectangle(
            10,
            CInt(SCREEN_HEIGHT - _pauseButton.Height * scale - 10),
            CInt(_pauseButton.Width * scale),
            CInt(_pauseButton.Height * scale)
        )
        _spriteBatch.Draw(_pauseButton, buttonRect, Color.White)
    End Sub

    Private Sub DrawPauseOverlay()
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))

        DrawCenteredText("PAUSED", -80, Color.White, 2.0F)
        DrawButton("RESUME", CInt(SCREEN_WIDTH / 2), CInt(SCREEN_HEIGHT / 2), 1.0F)
        DrawButton("MENU", CInt(SCREEN_WIDTH / 2), CInt(SCREEN_HEIGHT / 2) + 120, 1.0F)
    End Sub

    Private Sub DrawGameOverScreen(gameManager As GameManager)
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))

        DrawCenteredText("GAME OVER", -100, Color.White, 2.0F)
        DrawCenteredText($"Final Score: {gameManager.Player.Score}", -50, Color.White, 1.0F)
        DrawCenteredText($"Highest Score: {gameManager.HighScore}", -20, Color.White, 1.0F)

        DrawButton("RETRY", CInt(SCREEN_WIDTH / 2), CInt(SCREEN_HEIGHT / 2) + 50, 1.0F)
        DrawButton("MENU", CInt(SCREEN_WIDTH / 2), CInt(SCREEN_HEIGHT / 2) + 170, 1.0F)
    End Sub

    Private Sub DrawLevelClearedScreen()
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))

        DrawCenteredText($"SEASON CLEARED", -100, Color.White, 1.5F)
        DrawCenteredText($"Let's move on to the next!", -50, Color.White, 1.0F)
    End Sub

    Private Sub DrawText(text As String, position As Vector2, color As Color, Optional scale As Single = 1.0F)
        _spriteBatch.DrawString(_gameFont, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0)
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