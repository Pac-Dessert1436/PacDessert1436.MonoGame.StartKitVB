Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

Public MustInherit Class Actor
    Public Property GridPosition As Point
    Public Property PixelPosition As Vector2
    Public Property Size As Integer
    Public Property IsActive As Boolean = True

    Protected Const SNAP_THRESHOLD As Integer = 5

    Public Sub New(gridPosition As Point, size As Integer)
        Me.GridPosition = gridPosition
        PixelPosition = New Vector2(
            gridPosition.X * CELL_SIZE + CELL_SIZE \ 2,
            gridPosition.Y * CELL_SIZE + CELL_SIZE \ 2
        )
        Me.Size = size
    End Sub

    Public Overridable Function GetBounds() As Rectangle
        Return New Rectangle(
            CInt(PixelPosition.X - Size / 2),
            CInt(PixelPosition.Y - Size / 2),
            Size,
            Size
        )
    End Function

    Public Overridable Sub Update(deltaTime As Single, Optional maze As MazeTile(,) = Nothing)
    End Sub

    Public Shared ReadOnly Property ValidDirections(currDir As Direction) As Direction()
        Get
            Dim directions As Direction() = {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right
            }
            Return Aggregate dir As Direction In directions
                   Where dir <> OppositeDirection(currDir) Into ToArray()
        End Get
    End Property

    Public Shared ReadOnly Property OppositeDirection(currDir As Direction) As Direction
        Get
            Select Case currDir
                Case Direction.Up
                    Return Direction.Down
                Case Direction.Down
                    Return Direction.Up
                Case Direction.Left
                    Return Direction.Right
                Case Direction.Right
                    Return Direction.Left
                Case Else
                    Return Direction.Down
            End Select
        End Get
    End Property

    Public NotInheritable Class Player
        Inherits Actor

        Public Const IMAGE_PATH As String = "Images/player_sheet"
        Public Property Score As Integer = 0
        Public Property Lives As Integer = STARTING_LIVES
        Public Property HasReceivedBonusLife As Boolean = False
        Public Property IsAlive As Boolean = True
        Public Property Speed As Single = PLAYER_SPEED
        Public Property CurrentDirection As Direction = Direction.Right
        Public Property NextDirection As Direction = Direction.Right
        Public Property IsMoving As Boolean = False
        Public Property IsInDeathAnimation As Boolean = False
        Public Property DeathAnimationTimer As Single = 0.0F

        Private Shared _joystickBaseWidth As Integer = 64

        Public Shared Property JoystickBaseWidth As Integer
            Get
                Return _joystickBaseWidth
            End Get
            Set(value As Integer)
                _joystickBaseWidth = value
            End Set
        End Property

        Public Sub New(gridPosition As Point)
            MyBase.New(gridPosition, PLAYER_SIZE)
        End Sub

        Public Overrides Sub Update(deltaTime As Single, Optional maze As MazeTile(,) = Nothing)
            If Not IsAlive Then Exit Sub

            If IsInDeathAnimation Then
                DeathAnimationTimer += deltaTime
                If DeathAnimationTimer >= DEATH_ANIMATION_DURATION Then
                    CompleteDeathAnimation()
                End If
                Return
            End If

            Dim keyboardState = Keyboard.GetState()
            Dim touchCollection = Touch.TouchPanel.GetState()
            Dim mouseState = Mouse.GetState()
            Dim gamePadState = GamePad.GetState(PlayerIndex.One)
            IsMoving = False

            If keyboardState.IsKeyDown(Keys.Left) OrElse keyboardState.IsKeyDown(Keys.A) Then
                NextDirection = Direction.Left
                IsMoving = True
            End If
            If keyboardState.IsKeyDown(Keys.Right) OrElse keyboardState.IsKeyDown(Keys.D) Then
                NextDirection = Direction.Right
                IsMoving = True
            End If
            If keyboardState.IsKeyDown(Keys.Up) OrElse keyboardState.IsKeyDown(Keys.W) Then
                NextDirection = Direction.Up
                IsMoving = True
            End If
            If keyboardState.IsKeyDown(Keys.Down) OrElse keyboardState.IsKeyDown(Keys.S) Then
                NextDirection = Direction.Down
                IsMoving = True
            End If

            Dim leftThumbStick = gamePadState.ThumbSticks.Left
            If Math.Abs(leftThumbStick.X) > 0.2F OrElse Math.Abs(leftThumbStick.Y) > 0.2F Then
                If Math.Abs(leftThumbStick.X) > Math.Abs(leftThumbStick.Y) Then
                    NextDirection = If(leftThumbStick.X < 0, Direction.Left, Direction.Right)
                Else
                    NextDirection = If(leftThumbStick.Y < 0, Direction.Up, Direction.Down)
                End If
                IsMoving = True
            End If

            If gamePadState.DPad.Left = ButtonState.Pressed Then
                NextDirection = Direction.Left
                IsMoving = True
            End If
            If gamePadState.DPad.Right = ButtonState.Pressed Then
                NextDirection = Direction.Right
                IsMoving = True
            End If
            If gamePadState.DPad.Up = ButtonState.Pressed Then
                NextDirection = Direction.Up
                IsMoving = True
            End If
            If gamePadState.DPad.Down = ButtonState.Pressed Then
                NextDirection = Direction.Down
                IsMoving = True
            End If

            Dim joystickCenter = New Vector2(
                Renderer.ActualScreenWidth / 2.0F,
                Renderer.ActualScreenHeight - _joystickBaseWidth * Renderer.ScreenScale * 2 - 10.0F
            )
            Dim joystickRadius = _joystickBaseWidth * Renderer.ScreenScale * 2

            Dim buttonScale = Renderer.ScreenScale * 2
            Dim pauseButtonRect = New Rectangle(
                CInt(10 * Renderer.ScreenScale),
                CInt(Renderer.ActualScreenHeight - Renderer.PauseButtonHeight * buttonScale - 10),
                CInt(Renderer.PauseButtonWidth * buttonScale),
                CInt(Renderer.PauseButtonHeight * buttonScale)
            )

            For Each touchLoc In touchCollection
                If touchLoc.State = Touch.TouchLocationState.Pressed OrElse
                   touchLoc.State = Touch.TouchLocationState.Moved Then
                    Dim touchPos = touchLoc.Position
                    If Not pauseButtonRect.Contains(CInt(touchPos.X), CInt(touchPos.Y)) Then
                        Dim delta = touchPos - joystickCenter
                        If delta.Length() <= joystickRadius * 2 Then
                            HandleJoystickInput(delta)
                        End If
                    End If
                End If
            Next

            If mouseState.LeftButton = ButtonState.Pressed Then
                Dim mousePos = New Vector2(mouseState.X, mouseState.Y)
                If Not pauseButtonRect.Contains(CInt(mousePos.X), CInt(mousePos.Y)) Then
                    Dim delta = mousePos - joystickCenter
                    If delta.Length() <= joystickRadius * 2 Then
                        HandleJoystickInput(delta)
                    End If
                End If
            End If

            If IsMoving Then
                CurrentDirection = NextDirection
                Dim newPosition = PixelPosition + CurrentDirection.ToVector2() * Speed * deltaTime

                If IsValidPosition(newPosition, maze) Then
                    PixelPosition = newPosition
                    GridPosition = New Point(
                        CInt(PixelPosition.X / CELL_SIZE),
                        CInt(PixelPosition.Y / CELL_SIZE)
                    )

                    Dim cellCenterX = GridPosition.X * CELL_SIZE + CELL_SIZE \ 2
                    Dim cellCenterY = GridPosition.Y * CELL_SIZE + CELL_SIZE \ 2
                    Dim distToCenterX = Math.Abs(PixelPosition.X - cellCenterX)
                    Dim distToCenterY = Math.Abs(PixelPosition.Y - cellCenterY)

                    Dim tempPixelPosition = PixelPosition
                    If CurrentDirection = Direction.Left OrElse CurrentDirection = Direction.Right Then
                        If distToCenterY < SNAP_THRESHOLD Then tempPixelPosition.Y = cellCenterY
                    Else
                        If distToCenterX < SNAP_THRESHOLD Then tempPixelPosition.X = cellCenterX
                    End If
                    PixelPosition = tempPixelPosition
                End If
            End If
        End Sub

        Private Sub HandleJoystickInput(delta As Vector2)
            If Math.Abs(delta.X) > Math.Abs(delta.Y) Then
                If delta.X < 0 Then
                    NextDirection = Direction.Left
                Else
                    NextDirection = Direction.Right
                End If
            Else
                If delta.Y < 0 Then
                    NextDirection = Direction.Up
                Else
                    NextDirection = Direction.Down
                End If
            End If
            IsMoving = True
        End Sub

        Private Function IsValidPosition(newPosition As Vector2, maze As MazeTile(,)) As Boolean
            Dim bounds = New Rectangle(
                CInt(newPosition.X - Size / 2),
                CInt(newPosition.Y - Size / 2),
                Size,
                Size
            )

            Dim startTileX = Math.Max(0, bounds.Left \ CELL_SIZE)
            Dim endTileX = Math.Min(MAZE_WIDTH - 1, bounds.Right \ CELL_SIZE)
            Dim startTileY = Math.Max(0, bounds.Top \ CELL_SIZE)
            Dim endTileY = Math.Min(MAZE_HEIGHT - 1, bounds.Bottom \ CELL_SIZE)

            For tileX = startTileX To endTileX
                For tileY = startTileY To endTileY
                    Dim tile As MazeTile = maze(tileX, tileY)
                    If tile = MazeTile.Fence OrElse
                       tile = MazeTile.Sapling OrElse
                       tile = MazeTile.Tree Then Return False
                Next tileY
            Next tileX

            Return True
        End Function

        Public Sub CollectSeed(seedType As SeedType)
            Score += SEED_POINTS
            CheckBonusLife()
            ScheduleEvent_PlayerScoreChanged(Score)
            ScheduleEvent_SeedCollected(New Actor.Seed(GridPosition, seedType))
        End Sub

        Public Sub CheckBonusLife()
            If Not HasReceivedBonusLife AndAlso Score >= BONUS_LIFE_AT Then
                Lives += 1
                HasReceivedBonusLife = True
                ScheduleEvent_LivesChanged(Lives)
            End If
        End Sub

        Public Sub CollectPesticide()
            Score += PESTICIDE_POINTS
            CheckBonusLife()
            ScheduleEvent_PlayerScoreChanged(Score)
            ScheduleEvent_PesticideCollected()
        End Sub

        Public Sub KillEnemy()
            Score += ENEMY_POINTS
            CheckBonusLife()
            ScheduleEvent_PlayerScoreChanged(Score)
        End Sub

        Public Sub LoseLife()
            Lives -= 1
            ScheduleEvent_LivesChanged(Lives)
            ScheduleEvent_LifeLost()

            If Lives <= 0 Then
                IsAlive = False
                ScheduleEvent_GameHasEnded()
            Else
                IsInDeathAnimation = True
            End If
        End Sub

        Public Sub CompleteDeathAnimation()
            IsInDeathAnimation = False
            DeathAnimationTimer = 0.0F
            ResetPosition()
            ScheduleEvent_DeathAnimationComplete()
        End Sub

        Public Sub ResetPosition()
            GridPosition = PlayerStartingPoint
            PixelPosition = New Vector2(
                GridPosition.X * CELL_SIZE + CELL_SIZE \ 2,
                GridPosition.Y * CELL_SIZE + CELL_SIZE \ 2
            )
            CurrentDirection = Direction.Right
            NextDirection = Direction.Right
        End Sub

        Public Sub ResetBonusLifeFlag()
            HasReceivedBonusLife = False
        End Sub
    End Class

    Public NotInheritable Class Enemy
        Inherits Actor

        Public Const IMAGE_PATH As String = "Images/enemy_sheet"
        Public Property Direction As Direction
        Public Property Speed As Single = ENEMY_SPEED
        Public Property IsVulnerable As Boolean = False
        Public Property VulnerableTimer As Single = 0.0F
        Public Property EnemyType As EnemyType = EnemyType.Beetle
        Public Property IsRespawning As Boolean = False
        Public Property RespawnTimer As Single = 0.0F
        Public Property GracePeriodTimer As Single = 0.0F
        Public Property SpawnPoint As Point

        Private ReadOnly random As New Random
        Private _previousDirection As Direction

        Public Sub New(gridPosition As Point, Optional enemyType As EnemyType = EnemyType.Beetle)
            MyBase.New(gridPosition, ENEMY_SIZE)
            Me.EnemyType = enemyType
            Me.SpawnPoint = gridPosition
            SetRandomDirection()
            _previousDirection = Direction
            IsActive = True
        End Sub

        Public Overrides Sub Update(deltaTime As Single, Optional maze As MazeTile(,) = Nothing)
            If IsRespawning Then
                RespawnTimer += deltaTime
                If RespawnTimer >= ENEMY_RESPAWN_TIME Then
                    IsRespawning = False
                    RespawnTimer = 0.0F
                    IsActive = True
                    GracePeriodTimer = ENEMY_GRACE_PERIOD
                    ScheduleEvent_EnemyRespawned(Me)
                End If
                Return
            End If

            If Not IsActive Then Return

            If GracePeriodTimer > 0 Then
                GracePeriodTimer -= deltaTime
                If GracePeriodTimer < 0 Then GracePeriodTimer = 0
            End If

            If IsVulnerable Then
                VulnerableTimer -= deltaTime
                If VulnerableTimer <= 0 Then
                    IsVulnerable = False
                    VulnerableTimer = 0
                End If
            End If

            Dim movement = Direction.ToVector2()
            Dim newPosition = PixelPosition + movement * Speed * deltaTime

            If maze IsNot Nothing AndAlso IsValidPosition(newPosition, maze) Then
                PixelPosition = newPosition
                GridPosition = New Point(
                    CInt(PixelPosition.X / CELL_SIZE),
                    CInt(PixelPosition.Y / CELL_SIZE)
                )

                Dim cellCenterX = GridPosition.X * CELL_SIZE + CELL_SIZE \ 2
                Dim cellCenterY = GridPosition.Y * CELL_SIZE + CELL_SIZE \ 2
                Dim distToCenterX = Math.Abs(PixelPosition.X - cellCenterX)
                Dim distToCenterY = Math.Abs(PixelPosition.Y - cellCenterY)

                Dim tempPixelPosition = PixelPosition
                If Direction = Direction.Left OrElse Direction = Direction.Right Then
                    If distToCenterY < SNAP_THRESHOLD Then tempPixelPosition.Y = cellCenterY
                Else
                    If distToCenterX < SNAP_THRESHOLD Then tempPixelPosition.X = cellCenterX
                End If
                PixelPosition = tempPixelPosition
            Else
                ChangeDirection()
            End If

            If random.Next(0, 100) < 3 Then ChangeDirection()
            _previousDirection = Direction
        End Sub

        Private Function IsValidPosition(newPosition As Vector2, maze As MazeTile(,)) As Boolean
            Dim bounds = New Rectangle(
                CInt(newPosition.X - Size / 2),
                CInt(newPosition.Y - Size / 2),
                Size,
                Size
            )

            Dim startTileX = Math.Max(0, bounds.Left \ CELL_SIZE)
            Dim endTileX = Math.Min(MAZE_WIDTH - 1, bounds.Right \ CELL_SIZE)
            Dim startTileY = Math.Max(0, bounds.Top \ CELL_SIZE)
            Dim endTileY = Math.Min(MAZE_HEIGHT - 1, bounds.Bottom \ CELL_SIZE)

            For tileX = startTileX To endTileX
                For tileY = startTileY To endTileY
                    Dim tile = maze(tileX, tileY)
                    If tile = MazeTile.Fence OrElse tile = MazeTile.Sapling OrElse tile = MazeTile.Tree Then
                        Return False
                    End If
                Next
            Next

            Return True
        End Function

        Public Sub SetRandomDirection()
            Dim validDirections = Actor.ValidDirections(_previousDirection)

            If validDirections.Length > 0 Then
                Direction = validDirections(random.Next(validDirections.Length))
            Else
                Dim directions As Direction() = {
                    Direction.Up,
                    Direction.Down,
                    Direction.Left,
                    Direction.Right
                }
                Direction = directions(random.Next(directions.Length))
            End If
        End Sub

        Private Sub ChangeDirection()
            Dim directions As Direction() = {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right
            }

            Dim validDirections = Actor.ValidDirections(Direction)
            If validDirections.Length > 0 Then
                Direction = validDirections(random.Next(validDirections.Length))
            End If
        End Sub

        Public Sub MakeVulnerable()
            IsVulnerable = True
            VulnerableTimer = VULNERABLE_DURATION
        End Sub

        Public Sub Die()
            IsActive = False
            IsRespawning = True
            RespawnTimer = 0.0F
            GridPosition = SpawnPoint
            PixelPosition = New Vector2(
                SpawnPoint.X * CELL_SIZE + CELL_SIZE \ 2,
                SpawnPoint.Y * CELL_SIZE + CELL_SIZE \ 2
            )
            ScheduleEvent_EnemyKilled(Me)
        End Sub

        Public Sub RespawnAt(newPosition As Point)
            GridPosition = newPosition
            PixelPosition = New Vector2(
                newPosition.X * CELL_SIZE + CELL_SIZE \ 2,
                newPosition.Y * CELL_SIZE + CELL_SIZE \ 2
            )
            IsActive = True
            IsRespawning = False
            IsVulnerable = False
            GracePeriodTimer = ENEMY_GRACE_PERIOD
            SetRandomDirection()
        End Sub
    End Class

    Public NotInheritable Class Seed
        Inherits Actor

        Public Const IMAGE_PATH As String = "Images/object_sheet"
        Public Property SeedType As SeedType = SeedType.Acorn

        Public Sub New(gridPosition As Point, Optional seedType As SeedType = SeedType.Acorn)
            MyBase.New(gridPosition, SEED_SIZE)
            Me.SeedType = seedType
        End Sub
    End Class
End Class