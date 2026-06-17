Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

''' <summary>
''' Main game class for the blank MonoGame template.
''' Provides a clean starting point for your own MonoGame projects.
''' </summary>
Public NotInheritable Class GameMain
    Inherits Game

    Private ReadOnly _graphics As GraphicsDeviceManager
    Private _spriteBatch As SpriteBatch
    Private _font As SpriteFont

    Private Const SCREEN_WIDTH As Integer = 800
    Private Const SCREEN_HEIGHT As Integer = 600

    Public Sub New()
        _graphics = New GraphicsDeviceManager(Me)
        Content.RootDirectory = "Content"
        IsMouseVisible = True
    End Sub

    Protected Overrides Sub Initialize()
        If OperatingSystem.IsAndroid() Then
            _graphics.IsFullScreen = True
            With GraphicsDevice.PresentationParameters
                GraphicsDevice.Viewport = 
                    New Viewport(0, 0, .BackBufferWidth, .BackBufferHeight) _
                    With {.MinDepth = 0.0F, .MaxDepth = 1.0F}
            End With
        Else
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT
            _graphics.IsFullScreen = False
        End If
        _graphics.ApplyChanges()

        MyBase.Initialize()
    End Sub

    Protected Overrides Sub LoadContent()
        _spriteBatch = New SpriteBatch(GraphicsDevice)
        _font = Content.Load(Of SpriteFont)("Fonts/GameFont")
    End Sub

    Protected Overrides Sub Update(gameTime As GameTime)
        If GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed OrElse
           Keyboard.GetState().IsKeyDown(Keys.Escape) Then [Exit]()

        ' TODO: Add your game logic here

        MyBase.Update(gameTime)
    End Sub

    Protected Overrides Sub Draw(gameTime As GameTime)
        GraphicsDevice.Clear(Color.CornflowerBlue)

        _spriteBatch.Begin()
        Const MESSAGE = "Welcome to MonoGame! Press ESC to quit."
        Dim textSize As Vector2 = _font.MeasureString(MESSAGE)
        Dim textX As Single = (SCREEN_WIDTH - textSize.X) / 2.0F
        Dim textY As Single = (SCREEN_HEIGHT - textSize.Y) / 2.0F
        _spriteBatch.DrawString(_font, MESSAGE, New Vector2(textX, textY), Color.White)
        _spriteBatch.End()

        MyBase.Draw(gameTime)
    End Sub
End Class