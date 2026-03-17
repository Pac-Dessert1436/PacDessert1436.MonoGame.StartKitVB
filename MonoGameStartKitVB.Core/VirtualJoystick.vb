Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

''' <summary>
''' Represents a virtual joystick used for player input in the game.
''' Renders two images: one for the base, one for the knob.
''' </summary>
Public NotInheritable Class VirtualJoystick
    ' Required fields
    Private ReadOnly _baseTexture As Texture2D
    Private ReadOnly _knobTexture As Texture2D
    Public Property Position As Vector2 ' Center of the joystick base
    Public Property Value As Vector2    ' (-1 to 1 range X/Y, thumb offset)

    Public Sub New(baseTexture As Texture2D, knobTexture As Texture2D, center As Vector2)
        _baseTexture = baseTexture
        _knobTexture = knobTexture
        Position = center
        Value = Vector2.Zero
    End Sub

    Public Sub Update(touchPoint As Vector2?, maxRadius As Single)
        If touchPoint.HasValue Then
            Dim delta = touchPoint.Value - Position
            If delta.Length() > maxRadius Then
                delta.Normalize()
                delta *= maxRadius
            End If
            Value = delta / maxRadius ' Returns -1 to 1 scale
        Else
            Value = Vector2.Zero
        End If
    End Sub

    Public Sub Draw(spriteBatch As SpriteBatch)
        spriteBatch.Draw(_baseTexture, Position - New Vector2(_baseTexture.Width, _baseTexture.Height) / 2, Color.White)
        Dim knobPos = Position + Value * (_baseTexture.Width / 2F)
        spriteBatch.Draw(_knobTexture, knobPos - New Vector2(_knobTexture.Width, _knobTexture.Height) / 2, Color.White)
    End Sub
End Class