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
    ''' <summary>
    ''' Center of the joystick base.
    ''' </summary>
    Public Property Position As Vector2
    ''' <summary>
    ''' Current value of the joystick.
    ''' </summary>
    Public Property Value As Vector2

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
        Dim baseRect = New Rectangle(
            CInt(Position.X - _baseTexture.Width),
            CInt(Position.Y - _baseTexture.Height),
            _baseTexture.Width * 2,
            _baseTexture.Height * 2
        )
        spriteBatch.Draw(_baseTexture, baseRect, Color.White)
        
        Dim knobPos = Position + Value * (_baseTexture.Width)
        Dim knobRect = New Rectangle(
            CInt(knobPos.X - _knobTexture.Width),
            CInt(knobPos.Y - _knobTexture.Height),
            _knobTexture.Width * 2,
            _knobTexture.Height * 2
        )
        spriteBatch.Draw(_knobTexture, knobRect, Color.White)
    End Sub
End Class