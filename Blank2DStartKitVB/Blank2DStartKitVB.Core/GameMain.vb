Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Input.Touch

''' <summary>
''' Main game class for the blank MonoGame template.
''' Provides a clean starting point for your own MonoGame projects.
''' </summary>
Public NotInheritable Class GameMain
    Inherits Game

    Private ReadOnly _graphics As GraphicsDeviceManager
    Private _spriteBatch As SpriteBatch
    Private _font As SpriteFont
    Private _screen As RenderTarget2D
    Private _counter As Integer
    Private _prevMouseState As MouseState
    Private _prevTouchState As TouchCollection

    Public Const SCREEN_WIDTH As Integer = 800
    Public Const SCREEN_HEIGHT As Integer = 600

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

        ' Initialize input states for click/tap detection
        _prevMouseState = Mouse.GetState()
        _prevTouchState = TouchPanel.GetState()
    End Sub

    Protected Overrides Sub LoadContent()
        _spriteBatch = New SpriteBatch(GraphicsDevice)
        _font = Content.Load(Of SpriteFont)("Fonts/GameFont")

        ' Create render target for fixed-resolution rendering (letterboxing)
        _screen = New RenderTarget2D(GraphicsDevice, SCREEN_WIDTH, SCREEN_HEIGHT)
    End Sub

    Protected Overrides Sub Update(gameTime As GameTime)
        If GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed OrElse
           Keyboard.GetState().IsKeyDown(Keys.Escape) Then [Exit]()

        ' Handle input to increment counter
        Dim inputDetected As Boolean = False

        ' Check mouse input (desktop)
        Dim mouseState = Mouse.GetState()
        If mouseState.LeftButton = ButtonState.Pressed AndAlso
            _prevMouseState.LeftButton = ButtonState.Released Then inputDetected = True

        ' Check touch input (mobile)
        If OperatingSystem.IsAndroid() OrElse OperatingSystem.IsIOS() Then
            Dim touchState = TouchPanel.GetState()
            If touchState.Count > 0 AndAlso
                touchState(0).State = TouchLocationState.Pressed Then inputDetected = True
        End If

        ' Increment counter if input detected
        If inputDetected Then _counter += 1

        ' Update previous input states
        _prevMouseState = mouseState

        MyBase.Update(gameTime)
    End Sub

    Protected Overrides Sub Draw(gameTime As GameTime)
        ' Draw all content to the fixed-resolution render target first
        GraphicsDevice.SetRenderTarget(_screen)
        GraphicsDevice.Clear(Color.CornflowerBlue)

        _spriteBatch.Begin()
        ' Draw counter message at the top center
        Dim message = $"Welcome to MonoGame! Counter = {_counter}."
        Dim textSize As Vector2 = _font.MeasureString(message)
        Dim textX As Single = (SCREEN_WIDTH - textSize.X) / 2.0F
        Dim textY As Single = (SCREEN_HEIGHT \ 2 - textSize.Y) - 30 ' Position above center
        _spriteBatch.DrawString(_font, message, New Vector2(textX, textY), Color.White)

        ' Draw instruction message at the bottom center
        message = "Tap screen to increase counter!"
        textSize = _font.MeasureString(message)
        textX = (SCREEN_WIDTH - textSize.X) / 2.0F
        textY = (SCREEN_HEIGHT / 2) + 30 ' Position below center
        _spriteBatch.DrawString(_font, message, New Vector2(textX, textY), Color.White)
        _spriteBatch.End()

        ' Switch back to drawing to the screen
        GraphicsDevice.SetRenderTarget(Nothing)
        GraphicsDevice.Clear(Color.Black) ' Black background for letterbox bars

        ' Calculate letterboxing scale to maintain aspect ratio
        Dim viewport = GraphicsDevice.Viewport
        Dim targetAspectRatio = CSng(SCREEN_WIDTH) / SCREEN_HEIGHT
        Dim viewportAspectRatio = CSng(viewport.Width) / viewport.Height

        Dim scaleX As Single, scaleY As Single
        If viewportAspectRatio > targetAspectRatio Then
            ' Screen is wider than target: scale by height, add horizontal letterbox
            scaleY = CSng(viewport.Height) / SCREEN_HEIGHT
            scaleX = scaleY
        Else
            ' Screen is taller than target: scale by width, add vertical letterbox
            scaleX = CSng(viewport.Width) / SCREEN_WIDTH
            scaleY = scaleX
        End If

        ' Calculate destination rectangle for the render target
        Dim scaledWidth = SCREEN_WIDTH * scaleX
        Dim scaledHeight = SCREEN_HEIGHT * scaleY
        Dim destX = (viewport.Width - scaledWidth) / 2.0F
        Dim destY = (viewport.Height - scaledHeight) / 2.0F
        Dim destRect = New Rectangle(CInt(destX), CInt(destY), CInt(scaledWidth), CInt(scaledHeight))

        ' Draw the render target to the screen with letterboxing
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
        _spriteBatch.Draw(_screen, destRect, Color.White)
        _spriteBatch.End()

        MyBase.Draw(gameTime)
    End Sub
End Class