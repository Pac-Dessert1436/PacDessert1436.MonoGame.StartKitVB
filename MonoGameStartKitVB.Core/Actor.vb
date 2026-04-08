Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

''' <summary>
''' Base class for all game actors with common properties and behaviors.
''' </summary>
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

    ''' <summary>
    ''' Gets the bounding rectangle for collision detection.
    ''' </summary>
    Public Overridable Function GetBounds() As Rectangle
        Return New Rectangle(
            CInt(Position.X - Size / 2), 
            CInt(Position.Y - Size / 2), 
            Size, 
            Size
        )
    End Function

    ''' <summary>
    ''' Updates the actor's state.
    ''' </summary>
    Public Overridable Sub Update(deltaTime As Single)
        ' Base implementation does nothing
    End Sub

''' <summary>
''' Represents the player character in the game.
''' </summary>
Public NotInheritable Class Player
    Inherits Actor

    Public Const IMAGE_PATH As String = "Images/player"
    Public Property Score As Integer = 0
    Public Property SeedsCollected As Integer = 0
    Public Property IsAlive As Boolean = True
    Public Property Speed As Single = PLAYER_SPEED

    Public Sub New(position As Vector2)
        MyBase.New(position, PLAYER_SIZE, Color.Yellow)
    End Sub

    ''' <summary>
    ''' Updates the player's position based on input.
    ''' </summary>
    Public Overrides Sub Update(deltaTime As Single)
        If Not IsAlive Then Return

        Dim keyboardState = Keyboard.GetState()
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
            Position += movement * Speed * deltaTime
            Position = New Vector2(
                Math.Max(Size / 2.0F, Math.Min(SCREEN_WIDTH - Size / 2.0F, Position.X)),
                Math.Max(Size / 2.0F, Math.Min(SCREEN_HEIGHT - Size / 2.0F, Position.Y))
            )
        End If
    End Sub

    ''' <summary>
    ''' Collects a seed and updates score.
    ''' </summary>
    Public Sub CollectSeed(seed As Seed)
        Score += 10
        SeedsCollected += 1
        ScheduleEvent_PlayerScoreChanged(Score)
        ScheduleEvent_SeedCollected(seed)
    End Sub

    ''' <summary>
    ''' Plants a tree and resets seed count.
    ''' </summary>
    Public Sub PlantTree()
        SeedsCollected = 0
    End Sub

    ''' <summary>
    ''' Handles player death.
    ''' </summary>
    Public Sub Die()
        IsAlive = False
        ScheduleEvent_PlayerDied()
    End Sub
End Class

''' <summary>
''' Represents enemy characters in the game.
''' </summary>
Public NotInheritable Class Enemy
    Inherits Actor

    Public Const IMAGE_PATH As String = "Images/beetle"
    Public Property Direction As Vector2
    Public Property Speed As Single = ENEMY_SPEED
    Private ReadOnly random As New Random()

    Public Sub New(position As Vector2)
        MyBase.New(position, ENEMY_SIZE, Color.Red)
        SetRandomDirection()
    End Sub

    ''' <summary>
    ''' Updates the enemy's position and handles boundary collision.
    ''' </summary>
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

    ''' <summary>
    ''' Sets a random direction for the enemy.
    ''' </summary>
    Public Sub SetRandomDirection()
        Dim directions As Vector2() = {
            New Vector2(1, 0),
            New Vector2(-1, 0),
            New Vector2(0, 1),
            New Vector2(0, -1)
        }
        Direction = directions(random.Next(directions.Length))
    End Sub

    ''' <summary>
    ''' Handles enemy death.
    ''' </summary>
    Public Sub Die()
        IsActive = False
        ScheduleEvent_EnemyKilled(Me)
    End Sub
End Class

''' <summary>
''' Represents collectible seeds in the game.
''' </summary>
Public NotInheritable Class Seed
    Inherits Actor

    Public Const IMAGE_PATH As String = "Images/acorn_seed"
    Public Property IsConsumable As Boolean = True

    Public Sub New(position As Vector2)
        MyBase.New(position, SEED_SIZE, Color.Green)
    End Sub
End Class

''' <summary>
''' Represents trees that can be planted in the game.
''' </summary>
Public NotInheritable Class Tree
    Inherits Actor
    Public Property GrowthStage As Integer = 0

    Public Sub New(position As Vector2)
        MyBase.New(position, 40, Color.ForestGreen)
    End Sub

    ''' <summary>
    ''' Updates the tree's growth stage.
    ''' </summary>
    Public Overrides Sub Update(deltaTime As Single)
        ' Trees could grow over time in future versions
    End Sub
End Class
End Class