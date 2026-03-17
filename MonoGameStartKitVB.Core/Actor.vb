Imports Microsoft.Xna.Framework

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

    Public Function GetBounds() As Rectangle
        Return New Rectangle(CInt(Position.X - Size / 2), CInt(Position.Y - Size / 2), Size, Size)
    End Function

    Public NotInheritable Class Player
        Inherits Actor
        Public Property Score As Integer = 0
        Public Property SeedsCollected As Integer = 0
        Public Property IsAlive As Boolean = True

        Public Sub New(position As Vector2)
            MyBase.New(position, PLAYER_SIZE, Color.Yellow)
        End Sub
    End Class

    Public NotInheritable Class Enemy
        Inherits Actor
        Public Property Direction As Vector2
        Private random As New Random()

        Public Sub New(position As Vector2)
            MyBase.New(position, ENEMY_SIZE, Color.Red)
            SetRandomDirection()
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
    End Class

    Public NotInheritable Class Seed
        Inherits Actor
        Public Property IsConsumable As Boolean = True

        Public Sub New(position As Vector2)
            MyBase.New(position, SEED_SIZE, Color.Green)
        End Sub
    End Class

    Public NotInheritable Class Tree
        Inherits Actor
        Public Property GrowthStage As Integer = 0

        Public Sub New(position As Vector2)
            MyBase.New(position, 40, Color.ForestGreen)
        End Sub
    End Class
End Class