Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

Public MustInherit Class Actor
    Public Property GridPosition As Point
    Public Property PixelPosition As Vector2
    Public Property Size As Integer
    Public Property IsActive As Boolean = True

    Public Sub New(gridPosition As Point, size As Integer)
        Me.GridPosition = gridPosition
        Me.PixelPosition = New Vector2(
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

    Public Overridable Sub Update(deltaTime As Single)
    End Sub

    Public NotInheritable Class Player
        Inherits Actor

        Public Const IMAGE_PATH As String = "Images/player_sheet"
        Public Property Score As Integer = 0
        Public Property Lives As Integer = STARTING_LIVES
        Public Property IsAlive As Boolean = True
        Public Property Speed As Single = PLAYER_SPEED
        Public Property CurrentDirection As Direction = Direction.Right
        Public Property NextDirection As Direction = Direction.Right
        Public Property IsMoving As Boolean = False
        Public Property IsInDeathAnimation As Boolean = False
        Public Property DeathAnimationTimer As Single = 0.0F

        Public Sub New(gridPosition As Point)
            MyBase.New(gridPosition, PLAYER_SIZE)
        End Sub

        Public Overrides Sub Update(deltaTime As Single)
            If Not IsAlive Then Return

            If IsInDeathAnimation Then
                DeathAnimationTimer += deltaTime
                If DeathAnimationTimer >= DEATH_ANIMATION_DURATION Then
                    IsInDeathAnimation = False
                    DeathAnimationTimer = 0.0F
                End If
                Return
            End If

            Dim keyboardState = Keyboard.GetState()
            Dim movement As Vector2 = Vector2.Zero
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

            If IsMoving Then
                CurrentDirection = NextDirection
                movement = CurrentDirection.ToVector2()
                PixelPosition += movement * Speed * deltaTime
                GridPosition = New Point(
                    CInt(PixelPosition.X / CELL_SIZE),
                    CInt(PixelPosition.Y / CELL_SIZE)
                )
            End If
        End Sub

        Public Sub CollectSeed(seedType As SeedType)
            Score += SEED_POINTS
            ScheduleEvent_PlayerScoreChanged(Score)
            ScheduleEvent_SeedCollected(New Actor.Seed(GridPosition, seedType))
        End Sub

        Public Sub CollectPesticide()
            Score += PESTICIDE_POINTS
            ScheduleEvent_PlayerScoreChanged(Score)
            ScheduleEvent_PesticideCollected()
        End Sub

        Public Sub KillEnemy()
            Score += ENEMY_POINTS
            ScheduleEvent_PlayerScoreChanged(Score)
        End Sub

        Public Sub LoseLife()
            Lives -= 1
            ScheduleEvent_LivesChanged(Lives)
            ScheduleEvent_LifeLost()
            
            If Lives <= 0 Then
                IsAlive = False
                ScheduleEvent_PlayerDied()
            Else
                IsInDeathAnimation = True
                ResetPosition()
            End If
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

        Private ReadOnly random As New Random
        Private _previousDirection As Direction

        Public Sub New(gridPosition As Point, Optional enemyType As EnemyType = EnemyType.Beetle)
            MyBase.New(gridPosition, ENEMY_SIZE)
            Me.EnemyType = enemyType
            SetRandomDirection()
            _previousDirection = Direction
            IsActive = True
        End Sub

        Public Overrides Sub Update(deltaTime As Single)
            If Not IsActive Then Return

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
            PixelPosition += movement * Speed * deltaTime

            GridPosition = New Point(
                CInt(PixelPosition.X / CELL_SIZE),
                CInt(PixelPosition.Y / CELL_SIZE)
            )

            If random.Next(0, 100) < 2 Then
                ChangeDirection()
            End If

            _previousDirection = Direction
        End Sub

        Public Sub SetRandomDirection()
            Dim directions As Direction() = {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right
            }
            Dim validDirections = directions.Where(Function(d) d <> GetOppositeDirection(_previousDirection)).ToArray()
            
            If validDirections.Length > 0 Then
                Direction = validDirections(random.Next(validDirections.Length))
            Else
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
            
            Dim validDirections = directions.Where(Function(d) d <> GetOppositeDirection(Direction)).ToArray()
            
            If validDirections.Length > 0 Then
                Direction = validDirections(random.Next(validDirections.Length))
            End If
        End Sub

        Private Function GetOppositeDirection(dir As Direction) As Direction
            Select Case dir
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
        End Function

        Public Sub MakeVulnerable()
            IsVulnerable = True
            VulnerableTimer = VULNERABLE_DURATION
        End Sub

        Public Sub Die()
            IsActive = False
            IsRespawning = True
            RespawnTimer = 0.0F
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