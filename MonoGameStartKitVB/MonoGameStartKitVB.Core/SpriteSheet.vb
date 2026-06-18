Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Content

''' <summary>
''' Represents a sprite sheet with multiple frames.
''' </summary>
Public NotInheritable Class SpriteSheet
    Private ReadOnly _texture As Texture2D
    Public ReadOnly FrameWidth As Integer
    Public ReadOnly FrameHeight As Integer
    Public ReadOnly FrameCount As Integer
    Private ReadOnly _columns As Integer
    Private ReadOnly _rows As Integer

    Public Sub New(content As ContentManager, path As String, frameWidth As Integer, frameHeight As Integer)
        _texture = content.Load(Of Texture2D)(path)

        If frameWidth <= 0 Then
            Throw New ArgumentException("FrameWidth must be greater than 0", NameOf(frameWidth))
        End If
        If frameHeight <= 0 Then
            Throw New ArgumentException("FrameHeight must be greater than 0", NameOf(frameHeight))
        End If

        Me.FrameWidth = frameWidth
        Me.FrameHeight = frameHeight

        ' Calculate columns and rows based on texture dimensions
        ArgumentNullException.ThrowIfNull(_texture)
        _columns = If(_texture.Width > 0, Math.Max(1, _texture.Width \ frameWidth), 1)
        _rows = If(_texture.Height > 0, Math.Max(1, _texture.Height \ frameHeight), 1)
        FrameCount = _columns * _rows
    End Sub

    ''' <summary>
    ''' Gets the rectangle for a specific frame in the sprite sheet.
    ''' </summary>
    ''' <param name="frameIndex">The index of the frame to retrieve.</param>
    ''' <returns>The rectangle defining the frame's position in the sprite sheet.</returns>
    Public Function GetFrameRectangle(frameIndex As Integer) As Rectangle
        If _texture Is Nothing Then Return New Rectangle(0, 0, FrameWidth, FrameHeight)

        ' Clamp the frame index to valid range
        Dim clampedIndex = Math.Max(0, Math.Min(frameIndex, FrameCount - 1))

        ' Calculate the row and column for this frame
        Dim row As Integer = clampedIndex \ _columns
        Dim col As Integer = clampedIndex Mod _columns

        ' Return the rectangle for this frame
        Return New Rectangle(col * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight)
    End Function

    ''' <summary>
    ''' Draws a specific frame from the sprite sheet to the sprite batch.
    ''' </summary>
    ''' <param name="spriteBatch">The SpriteBatch to draw with.</param>
    ''' <param name="frameIndex">The index of the frame to draw.</param>
    ''' <param name="position">The position to draw the sprite at.</param>
    ''' <param name="scale">The scale to apply to the sprite.</param>
    ''' <param name="color">The color to tint the sprite.</param>
    Public Sub DrawFrame(spriteBatch As SpriteBatch, frameIndex As Integer, position As Vector2,
                         Optional scale As Single = 1.0F, Optional color As Color = Nothing)
        If color = Nothing Then color = Color.White

        spriteBatch.Draw(
            _texture,
            position,
            GetFrameRectangle(frameIndex),
            color,
            0.0F,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0.0F
        )
    End Sub

    Public ReadOnly Property Texture As Texture2D
        Get
            Return _texture
        End Get
    End Property
    
    Public ReadOnly Property Columns As Integer
        Get
            Return _columns
        End Get
    End Property
    
    Public ReadOnly Property Rows As Integer
        Get
            Return _rows
        End Get
    End Property
End Class

''' <summary>
''' Represents an animation sequence for a sprite sheet.
''' </summary>
Public NotInheritable Class Animation
    Private ReadOnly _spriteSheet As SpriteSheet
    Private ReadOnly _frameIndices As Integer()
    Private _currentFrameIndex As Integer = 0
    Private _frameTimer As Single = 0
    Public ReadOnly FrameDuration As Single

    Public Sub New(spriteSheet As SpriteSheet, frameIndices As Integer(), frameDuration As Single)
        _spriteSheet = spriteSheet
        _frameIndices = If(frameIndices Is Nothing OrElse frameIndices.Length = 0, {0}, frameIndices)
        Me.FrameDuration = Math.Max(0.001F, frameDuration)
    End Sub

    Public ReadOnly Property CurrentFrame As Rectangle
        Get
            If _frameIndices Is Nothing OrElse _frameIndices.Length = 0 Then
                Return New Rectangle(0, 0, _spriteSheet.FrameWidth, _spriteSheet.FrameHeight)
            End If
            Return _spriteSheet.GetFrameRectangle(_frameIndices(_currentFrameIndex))
        End Get
    End Property

    Public Sub Update(deltaTime As Single)
        If _frameIndices.Length <= 1 Then Exit Sub

        _frameTimer += deltaTime
        If _frameTimer >= FrameDuration Then
            _frameTimer -= FrameDuration
            _currentFrameIndex = (_currentFrameIndex + 1) Mod _frameIndices.Length
        End If
    End Sub

    Public Sub Reset()
        _currentFrameIndex = 0
        _frameTimer = 0
    End Sub

    Public ReadOnly Property SpriteSheet As SpriteSheet
        Get
            Return _spriteSheet
        End Get
    End Property
        
    Public ReadOnly Property CurrentFrameIndex As Integer
        Get
            Return _frameIndices(_currentFrameIndex)
        End Get
    End Property
End Class