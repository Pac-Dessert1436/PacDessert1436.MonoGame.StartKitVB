Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Content

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

        frameWidth = frameWidth
        frameHeight = frameHeight

        _columns = If(_texture IsNot Nothing AndAlso _texture.Width > 0, _texture.Width \ frameWidth, 1)
        _rows = If(_texture IsNot Nothing AndAlso _texture.Height > 0, _texture.Height \ frameHeight, 1)
        FrameCount = _columns * _rows
    End Sub

    Public ReadOnly Property FrameRectangle(frameIndex As Integer) As Rectangle
        Get
            If _texture Is Nothing Then
                Return New Rectangle(0, 0, FrameWidth, FrameHeight)
            End If

            Dim clampedIndex = Math.Max(0, Math.Min(frameIndex, FrameCount - 1))
            Dim row = clampedIndex \ _columns
            Dim col = clampedIndex Mod _columns

            Return New Rectangle(col * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight)
        End Get
    End Property

    Public ReadOnly Property Texture As Texture2D
        Get
            Return _texture
        End Get
    End Property
End Class

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
            Return _spriteSheet.FrameRectangle(_frameIndices(_currentFrameIndex))
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
End Class