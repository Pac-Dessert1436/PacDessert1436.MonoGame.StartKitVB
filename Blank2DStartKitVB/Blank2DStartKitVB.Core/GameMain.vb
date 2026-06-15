Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Public NotInheritable Class GameMain
    Inherits Game

    Private _graphics As GraphicsDeviceManager
    Private _spriteBatch As SpriteBatch

    Public Sub New()
        _graphics = New GraphicsDeviceManager(Me)
        Content.RootDirectory = "Content"
        IsMouseVisible = True
    End Sub

    Protected Overrides Sub Initialize()
        _graphics.PreferredBackBufferWidth = 800
        _graphics.PreferredBackBufferHeight = 600
        
        #If ANDROID Then
            _graphics.IsFullScreen = True
        #Else
            _graphics.IsFullScreen = False
        #End If
        
        _graphics.ApplyChanges()

        MyBase.Initialize()
    End Sub

    Protected Overrides Sub LoadContent()
        _spriteBatch = New SpriteBatch(GraphicsDevice)

        ' TODO: use this.Content to load your game content here
    End Sub

    Protected Overrides Sub Update(gameTime As GameTime)
        If GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed OrElse
           Keyboard.GetState().IsKeyDown(Keys.Escape) Then [Exit]()

        ' TODO: Add your update logic here

        MyBase.Update(gameTime)
    End Sub

    Protected Overrides Sub Draw(gameTime As GameTime)
        GraphicsDevice.Clear(Color.CornflowerBlue)

        ' TODO: Add your drawing code here

        MyBase.Draw(gameTime)
    End Sub
End Class