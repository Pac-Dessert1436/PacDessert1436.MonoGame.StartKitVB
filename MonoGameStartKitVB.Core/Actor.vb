Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

Public MustInherit Class Actor
    Public Property Position As Vector2
    Public Property Size As Integer
    Public Property Color As Color
    Public Property IsActive As Boolean = True

    Public Sub New(position As Vector2, size As Integer, color As Color)
        Me.Position = position
        Me.Size = size
        Me.Color = color
    End Sub

    Public Overridable Function GetBounds() As Rectangle
        Return New Rectangle(
            CInt(Position.X - Size / 2),
            CInt(Position.Y - Size / 2),
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
        Public Property SeedsCollected As Integer = 0
        Public Property IsAlive As Boolean = True
        Public Property Speed As Single = PLAYER_SPEED
        Public Property CurrentDirection As Direction = Direction.Right
        Public Property IsMoving As Boolean = False

        Public Sub New(position As Vector2)
            MyBase.New(position, PLAYER_SIZE, Color.Yellow)
        End Sub

        Public Overrides Sub Update(deltaTime As Single)
            If Not IsAlive Then Return

            Dim keyboardState = Keyboard.GetState()
            Dim movement As Vector2 = Vector2.Zero
            IsMoving = False

            If keyboardState.IsKeyDown(Keys.Left) OrElse keyboardState.IsKeyDown(Keys.A) Then
                movement.X -= 1
                CurrentDirection = Direction.Left
                IsMoving = True
            End If
            If keyboardState.IsKeyDown(Keys.Right) OrElse keyboardState.IsKeyDown(Keys.D) Then
                movement.X += 1
                CurrentDirection = Direction.Right
                IsMoving = True
            End If
            If keyboardState.IsKeyDown(Keys.Up) OrElse keyboardState.IsKeyDown(Keys.W) Then
                movement.Y -= 1
                CurrentDirection = Direction.Up
                IsMoving = True
            End If
            If keyboardState.IsKeyDown(Keys.Down) OrElse keyboardState.IsKeyDown(Keys.S) Then
                movement.Y += 1
                CurrentDirection = Direction.Down
                IsMoving = True
            End If

            If movement <> Vector2.Zero Then
                movement.Normalize()
                Position += movement * Speed * deltaTime
                Position = New Vector2(
                    Math.Max(Size / 2.0F, Math.Min(SCREEN_WIDTH - Size / 2.0F, Position.X)),
                    Math.Max(Size / 2.0F, Math.Min(SCREEN_HEIGHT - Size / 2.0F, Position.Y))
                )
            End If
        End Sub

        Public Sub CollectSeed(seed As Seed)
            Score += 10
            SeedsCollected += 1
            ScheduleEvent_PlayerScoreChanged(Score)
            ScheduleEvent_SeedCollected(seed)
        End Sub

        Public Sub PlantTree()
            SeedsCollected = 0
        End Sub

        Public Sub Die()
            IsAlive = False
            ScheduleEvent_PlayerDied()
        End Sub
    End Class

    Public NotInheritable Class Enemy
        Inherits Actor

        Public Const IMAGE_PATH As String = "Images/enemy_sheet"
        Public Property Direction As Vector2
        Public Property Speed As Single = ENEMY_SPEED
        Public Property IsVulnerable As Boolean = False
        Public Property EnemyType As EnemyType = EnemyType.Beetle
        Private ReadOnly random As New Random

        Public Sub New(position As Vector2, Optional enemyType As EnemyType = EnemyType.Beetle)
            MyBase.New(position, ENEMY_SIZE, Color.Red)
            Me.EnemyType = enemyType
            SetRandomDirection()
        End Sub

        Public Overrides Sub Update(deltaTime As Single)
            Position += Direction * Speed * deltaTime

            Dim newDir As Vector2 = Direction
            Dim newPos As Vector2 = Position

            If Position.X <= Size / 2 OrElse Position.X >= SCREEN_WIDTH - Size / 2 Then
                newDir.X *= -1
                newPos.X = Math.Max(Size / 2.0F, Math.Min(SCREEN_WIDTH - Size / 2.0F, Position.X))
            End If
            If Position.Y <= Size / 2 OrElse Position.Y >= SCREEN_HEIGHT - Size / 2 Then
                newDir.Y *= -1
                newPos.Y = Math.Max(Size / 2.0F, Math.Min(SCREEN_HEIGHT - Size / 2.0F, Position.Y))
            End If

            If random.Next(0, 100) < 1 Then SetRandomDirection()
            Direction = newDir
            Position = newPos
        End Sub

        Public Sub SetRandomDirection()
            Dim directions As Vector2() = {
                New Vector2(1, 0),
                New Vector2(-1, 0),
                New Vector2(0, 1),
                New Vector2(0, -1)
            }
            Direction = directions(random.Next(directions.Length))
        End Sub

        Public Sub Die()
            IsActive = False
            ScheduleEvent_EnemyKilled(Me)
        End Sub
    End Class

    Public Enum EnemyType
        Beetle
        Caterpillar
    End Enum

    Public NotInheritable Class Seed
        Inherits Actor

        Public Const IMAGE_PATH As String = "Images/object_sheet"
        Public Property IsConsumable As Boolean = True
        Public Property SeedType As SeedType = SeedType.Acorn

        Public Sub New(position As Vector2, Optional seedType As SeedType = SeedType.Acorn)
            MyBase.New(position, SEED_SIZE, Color.Green)
            Me.SeedType = seedType
        End Sub
    End Class

    Public Enum SeedType
        Acorn
        Berry
        Nut
    End Enum

    Public NotInheritable Class Tree
        Inherits Actor
        Public Property GrowthStage As Integer = 0
        Public Property TreeType As TreeType = TreeType.Oak

        Public Sub New(position As Vector2, Optional treeType As TreeType = TreeType.Oak)
            MyBase.New(position, 40, Color.ForestGreen)
            Me.TreeType = treeType
        End Sub

        Public Overrides Sub Update(deltaTime As Single)
        End Sub
    End Class

    Public Enum TreeType
        Pine
        Fruit
        Oak
    End Enum

    Public NotInheritable Class Pesticide
        Inherits Actor

        Public Const IMAGE_PATH As String = "Images/object_sheet"

        Public Sub New(position As Vector2)
            MyBase.New(position, 30, Color.Blue)
        End Sub
    End Class
End Class