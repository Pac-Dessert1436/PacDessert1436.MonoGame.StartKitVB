Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Content

''' <summary>
''' Handles all rendering operations for the game.
''' </summary>
Public NotInheritable Class Renderer
    Private ReadOnly _spriteBatch As SpriteBatch
    Private ReadOnly _content As ContentManager
    Private ReadOnly _pixelTexture As Texture2D
    
    ' Textures
    Private _playerTexture As Texture2D
    Private _seedTexture As Texture2D
    Private _enemyTexture As Texture2D
    Private _treeTexture As Texture2D
    
    ' Font
    Private _gameFont As SpriteFont
    
    Public Sub New(graphicsDevice As GraphicsDevice, content As ContentManager)
        _spriteBatch = New SpriteBatch(graphicsDevice)
        _content = content
        
        ' Create pixel texture for simple shapes
        _pixelTexture = New Texture2D(graphicsDevice, 1, 1)
        _pixelTexture.SetData(New Color() {Color.White})
        
        LoadContent()
    End Sub
    
    ''' <summary>
    ''' Loads all graphical assets.
    ''' </summary>
    Private Sub LoadContent()
        ' Load image textures
        _playerTexture = _content.Load(Of Texture2D)("Images/player")
        _seedTexture = _content.Load(Of Texture2D)("Images/acorn_packet")
        _enemyTexture = _content.Load(Of Texture2D)("Images/beetle")
        _treeTexture = _content.Load(Of Texture2D)("Images/oak_tree")
        
        ' Load font
        _gameFont = _content.Load(Of SpriteFont)("Fonts/GameFont")
    End Sub
    
    ''' <summary>
    ''' Renders the entire game based on the current game state.
    ''' </summary>
    Public Sub Render(gameManager As GameManager, gameState As GameState)
        _spriteBatch.GraphicsDevice.Clear(Color.Black)
        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)
        
        Select Case gameState
            Case GameState.Title
                DrawTitleScreen()
                
            Case GameState.Playing
                DrawGame(gameManager)
                
            Case GameState.GameOver
                DrawGame(gameManager)
                DrawGameOverScreen(gameManager)
        End Select
        
        _spriteBatch.End()
    End Sub
    
    ''' <summary>
    ''' Draws the title screen.
    ''' </summary>
    Private Sub DrawTitleScreen()
        DrawCenteredText("DEVOUR-MAN", -50, Color.Yellow, 2.0F)
        DrawCenteredText("Eat seeds & plant a forest!", 20, Color.Green)
        DrawCenteredText("Press ENTER to start", 80, Color.White)
        DrawCenteredText("Arrow Keys or WASD to move", 120, Color.Gray)
    End Sub
    
    ''' <summary>
    ''' Draws the main game scene.
    ''' </summary>
    Private Sub DrawGame(gameManager As GameManager)
        DrawPlayer(gameManager.Player)
        DrawSeeds(gameManager.Seeds)
        DrawEnemies(gameManager.Enemies)
        DrawTrees(gameManager.Trees)
        DrawHUD(gameManager.Player, gameManager.Trees.Count)
    End Sub
    
    ''' <summary>
    ''' Draws the game over screen overlay.
    ''' </summary>
    Private Sub DrawGameOverScreen(gameManager As GameManager)
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))
        
        DrawCenteredText("GAME OVER", -30, Color.Red, 2.0F)
        DrawCenteredText($"Final Score: {gameManager.Player.Score}", 20, Color.White)
        DrawCenteredText($"Trees Planted: {gameManager.Trees.Count}", 50, Color.ForestGreen)
        DrawCenteredText("Press ENTER to restart", 100, Color.White)
    End Sub

    ''' <summary>
    ''' Draws the player character.
    ''' </summary>
    Private Sub DrawPlayer(player As Actor.Player)
        Dim origin As New Vector2(_playerTexture.Width / 2.0F, _playerTexture.Height / 2.0F)
        Dim scale As Single = player.Size / MathF.Max(_playerTexture.Width, _playerTexture.Height)
        _spriteBatch.Draw(_playerTexture, player.Position, Nothing, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
    End Sub

    ''' <summary>
    ''' Draws all active seeds.
    ''' </summary>
    Private Sub DrawSeeds(seeds As List(Of Actor.Seed))
        For Each seed In From s In seeds Where s.IsActive
            Dim origin As New Vector2(_seedTexture.Width / 2.0F, _seedTexture.Height / 2.0F)
            Dim scale As Single = seed.Size / MathF.Max(_seedTexture.Width, _seedTexture.Height)
            _spriteBatch.Draw(_seedTexture, seed.Position, Nothing, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    ''' <summary>
    ''' Draws all enemies.
    ''' </summary>
    Private Sub DrawEnemies(enemies As List(Of Actor.Enemy))
        For Each enemy In enemies
            Dim origin As New Vector2(_enemyTexture.Width / 2.0F, _enemyTexture.Height / 2.0F)
            Dim scale As Single = enemy.Size / MathF.Max(_enemyTexture.Width, _enemyTexture.Height)
            _spriteBatch.Draw(_enemyTexture, enemy.Position, Nothing, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    ''' <summary>
    ''' Draws all trees.
    ''' </summary>
    Private Sub DrawTrees(trees As List(Of Actor.Tree))
        For Each tree In trees
            Dim origin As New Vector2(_treeTexture.Width / 2.0F, _treeTexture.Height)
            Dim scale As Single = tree.Size / MathF.Max(_treeTexture.Width, _treeTexture.Height)
            _spriteBatch.Draw(_treeTexture, tree.Position, Nothing, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    ''' <summary>
    ''' Draws the heads-up display.
    ''' </summary>
    Private Sub DrawHUD(player As Actor.Player, treeCount As Integer)
        DrawText($"Score: {player.Score}", New Vector2(10, 10), Color.White)
        DrawText($"Seeds: {player.SeedsCollected}/{SEEDS_TO_PLANT_TREE}", New Vector2(10, 35), Color.Green)
        DrawText($"Trees: {treeCount}", New Vector2(10, 60), Color.ForestGreen)
    End Sub

    ''' <summary>
    ''' Draws text at a specific position.
    ''' </summary>
    Private Sub DrawText(text As String, position As Vector2, color As Color)
        _spriteBatch.DrawString(_gameFont, text, position, color)
    End Sub
    
    ''' <summary>
    ''' Draws centered text with optional vertical offset and scale.
    ''' </summary>
    Private Sub DrawCenteredText(text As String, yOffset As Integer, color As Color, Optional scale As Single = 1.0F)
        Dim textSize As Vector2 = _gameFont.MeasureString(text) * scale
        Dim position As New Vector2(
            (SCREEN_WIDTH - textSize.X) / 2,
            (SCREEN_HEIGHT - textSize.Y) / 2 + yOffset
        )
        _spriteBatch.DrawString(_gameFont, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0)
    End Sub
    
    ''' <summary>
    ''' Disposes of graphical resources.
    ''' </summary>
    Public Sub Dispose()
        _spriteBatch?.Dispose()
        _pixelTexture?.Dispose()
    End Sub
End Class