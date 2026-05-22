Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Input

''' <summary>
''' Main game class that coordinates all game systems using proper object-oriented patterns.
''' </summary>
Public NotInheritable Class GameMain
    Inherits Game

    Private ReadOnly _graphics As GraphicsDeviceManager
    
    ' Game systems
    Private _gameManager As GameManager
    Private _renderer As Renderer
    Private _soundManager As SoundManager

    Public Sub New()
        _graphics = New GraphicsDeviceManager(Me) With {
            .PreferredBackBufferWidth = SCREEN_WIDTH,
            .PreferredBackBufferHeight = SCREEN_HEIGHT
        }
        _graphics.ApplyChanges()
        Content.RootDirectory = "Content"
        IsMouseVisible = False
    End Sub

    Protected Overrides Sub Initialize()
        ' Initialize game systems
        _gameManager = New GameManager
        _renderer = New Renderer(GraphicsDevice, Content)
        _soundManager = New SoundManager(Content)

        MyBase.Initialize()
    End Sub

    Protected Overrides Sub Update(gameTime As GameTime)
        ' Handle exit condition
        If GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed OrElse
           Keyboard.GetState().IsKeyDown(Keys.Escape) Then [Exit]()

        ' Handle input for game state transitions
        _gameManager.HandleInput()

        ' Update game logic
        Dim deltaTime As Single = CSng(gameTime.ElapsedGameTime.TotalSeconds)
        _gameManager.Update(deltaTime)
        MyBase.Update(gameTime)
    End Sub

    Protected Overrides Sub Draw(gameTime As GameTime)
        Dim deltaTime As Single = CSng(gameTime.ElapsedGameTime.TotalSeconds)
        _renderer.Render(_gameManager, _gameManager.GameState, deltaTime)
        RaiseScheduledEvents()
        MyBase.Draw(gameTime)
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            ' Dispose of all game systems
            _soundManager?.Dispose()
            _renderer?.Dispose()
        End If

        MyBase.Dispose(disposing)
    End Sub
End Class