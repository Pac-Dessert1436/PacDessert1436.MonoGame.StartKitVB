Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Content

Public NotInheritable Class Renderer
    Implements IDisposable

    Private ReadOnly _spriteBatch As SpriteBatch
    Private ReadOnly _content As ContentManager
    Private ReadOnly _pixelTexture As Texture2D

    Private _gameFont As SpriteFont
    Private disposedValue As Boolean

    Private _playerSpriteSheet As SpriteSheet
    Private _enemySpriteSheet As SpriteSheet
    Private _objectSpriteSheet As SpriteSheet

    Private _playerAnimations As Dictionary(Of Direction, Animation)
    Private _playerDeathAnimation As Animation
    Private _enemyAnimations As Dictionary(Of Tuple(Of Actor.EnemyType, Direction), Animation)
    Private _objectFrames As Dictionary(Of ObjectType, Rectangle)

    Public Enum ObjectType
        Fence
        AcornSeed
        BerrySeed
        NutSeed
        PineTree
        FruitTree
        OakTree
        Sapling
        Pesticide
    End Enum

    Public Sub New(graphicsDevice As GraphicsDevice, content As ContentManager)
        _spriteBatch = New SpriteBatch(graphicsDevice)
        _content = content

        _pixelTexture = New Texture2D(graphicsDevice, 1, 1)
        _pixelTexture.SetData(New Color() {Color.White})

        LoadContent()
    End Sub

    <CodeAnalysis.SuppressMessage("Performance", "CA1861")>
    Private Sub LoadContent()
        _playerSpriteSheet = New SpriteSheet(_content, "Images/player_sheet", 64, 64)
        _enemySpriteSheet = New SpriteSheet(_content, "Images/enemy_sheet", 64, 64)
        _objectSpriteSheet = New SpriteSheet(_content, "Images/object_sheet", 64, 64)

        _playerAnimations = New Dictionary(Of Direction, Animation) From {
            {Direction.Left, New Animation(_playerSpriteSheet, {1, 2}, 0.1F)},
            {Direction.Right, New Animation(_playerSpriteSheet, {3, 4}, 0.1F)},
            {Direction.Up, New Animation(_playerSpriteSheet, {5, 6}, 0.1F)},
            {Direction.Down, New Animation(_playerSpriteSheet, {7, 8}, 0.1F)}
        }
        _playerDeathAnimation = New Animation(_playerSpriteSheet, Enumerable.Range(9, 8).ToArray(), 0.1F)
        _enemyAnimations = New Dictionary(Of Tuple(Of Actor.EnemyType, Direction), Animation) From {
            {Tuple.Create(Actor.EnemyType.Beetle, Direction.Left), New Animation(_enemySpriteSheet, {1, 2}, 0.1F)},
            {Tuple.Create(Actor.EnemyType.Beetle, Direction.Right), New Animation(_enemySpriteSheet, {3, 4}, 0.1F)},
            {Tuple.Create(Actor.EnemyType.Beetle, Direction.Up), New Animation(_enemySpriteSheet, {5, 6}, 0.1F)},
            {Tuple.Create(Actor.EnemyType.Beetle, Direction.Down), New Animation(_enemySpriteSheet, {7, 8}, 0.1F)},
            {Tuple.Create(Actor.EnemyType.Caterpillar, Direction.Left), New Animation(_enemySpriteSheet, {9, 10}, 0.1F)},
            {Tuple.Create(Actor.EnemyType.Caterpillar, Direction.Right), New Animation(_enemySpriteSheet, {11, 12}, 0.1F)},
            {Tuple.Create(Actor.EnemyType.Caterpillar, Direction.Up), New Animation(_enemySpriteSheet, {13, 14}, 0.1F)},
            {Tuple.Create(Actor.EnemyType.Caterpillar, Direction.Down), New Animation(_enemySpriteSheet, {15, 16}, 0.1F)}
        }

        _objectFrames = New Dictionary(Of ObjectType, Rectangle) From {
            {ObjectType.Fence, _objectSpriteSheet.FrameRectangle(1)},
            {ObjectType.AcornSeed, _objectSpriteSheet.FrameRectangle(2)},
            {ObjectType.BerrySeed, _objectSpriteSheet.FrameRectangle(3)},
            {ObjectType.NutSeed, _objectSpriteSheet.FrameRectangle(4)},
            {ObjectType.PineTree, _objectSpriteSheet.FrameRectangle(5)},
            {ObjectType.FruitTree, _objectSpriteSheet.FrameRectangle(6)},
            {ObjectType.OakTree, _objectSpriteSheet.FrameRectangle(7)},
            {ObjectType.Sapling, _objectSpriteSheet.FrameRectangle(8)},
            {ObjectType.Pesticide, _objectSpriteSheet.FrameRectangle(9)}
        }

        _gameFont = _content.Load(Of SpriteFont)("Fonts/GameFont")
    End Sub

    Public Sub Render(gameManager As GameManager, gameState As GameState, deltaTime As Single)
        _spriteBatch.GraphicsDevice.Clear(Color.Black)
        _spriteBatch.Begin(samplerState:=SamplerState.PointClamp)

        Select Case gameState
            Case GameState.Title
                DrawTitleScreen()

            Case GameState.Playing
                DrawGame(gameManager, deltaTime)

            Case GameState.GameOver
                DrawGame(gameManager, deltaTime)
                DrawGameOverScreen(gameManager)
        End Select

        _spriteBatch.End()
    End Sub

    Private Sub DrawTitleScreen()
        DrawCenteredText("DEVOUR-MAN", -50, Color.Yellow, 2.0F)
        DrawCenteredText("Eat seeds & plant a forest!", 20, Color.Green)
        DrawCenteredText("Press ENTER to start", 80, Color.White)
        DrawCenteredText("Arrow Keys or WASD to move", 120, Color.Gray)
    End Sub

    Private Sub DrawGame(gameManager As GameManager, deltaTime As Single)
        With gameManager
            DrawPlayer(.Player, deltaTime)
            DrawSeeds(.Seeds)
            DrawPesticides(.Pesticides)
            DrawEnemies(.Enemies, deltaTime)
            DrawTrees(.Trees)
            DrawHUD(.Player, .Trees.Count)
        End With
    End Sub

    Private Sub DrawGameOverScreen(gameManager As GameManager)
        Dim overlayRect As New Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT)
        _spriteBatch.Draw(_pixelTexture, overlayRect, New Color(0, 0, 0, 150))

        DrawCenteredText("GAME OVER", -30, Color.Red, 2.0F)
        DrawCenteredText($"Final Score: {gameManager.Player.Score}", 20, Color.White)
        DrawCenteredText($"Trees Planted: {gameManager.Trees.Count}", 50, Color.ForestGreen)
        DrawCenteredText("Press ENTER to restart", 100, Color.White)
    End Sub

    Private Sub DrawPlayer(player As Actor.Player, deltaTime As Single)
        Dim animation As Animation
        Dim frameRect As Rectangle

        If player.IsAlive Then
            animation = _playerAnimations(player.CurrentDirection)
            If player.IsMoving Then
                animation.Update(deltaTime)
            Else
                animation.Reset()
            End If
            frameRect = animation.CurrentFrame
        Else
            animation = _playerDeathAnimation
            animation.Update(deltaTime)
            frameRect = animation.CurrentFrame
        End If

        Dim origin As New Vector2(frameRect.Width / 2.0F, frameRect.Height / 2.0F)
        Dim scale As Single = player.Size / MathF.Max(frameRect.Width, frameRect.Height)
        _spriteBatch.Draw(animation.SpriteSheet.Texture, player.Position, frameRect, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
    End Sub

    Private Sub DrawSeeds(seeds As List(Of Actor.Seed))
        For Each seed In From s In seeds Where s.IsActive
            Dim frameRect As Rectangle = GetSeedFrame(seed.SeedType)
            Dim origin As New Vector2(frameRect.Width / 2.0F, frameRect.Height / 2.0F)
            Dim scale As Single = seed.Size / MathF.Max(frameRect.Width, frameRect.Height)
            _spriteBatch.Draw(_objectSpriteSheet.Texture, seed.Position, frameRect, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    Private Sub DrawPesticides(pesticides As List(Of Actor.Pesticide))
        For Each pesticide In From p In pesticides Where p.IsActive
            Dim frameRect As Rectangle = _objectFrames(ObjectType.Pesticide)
            Dim origin As New Vector2(frameRect.Width / 2.0F, frameRect.Height / 2.0F)
            Dim scale As Single = pesticide.Size / MathF.Max(frameRect.Width, frameRect.Height)
            _spriteBatch.Draw(_objectSpriteSheet.Texture, pesticide.Position, frameRect, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    Private Function GetSeedFrame(seedType As Actor.SeedType) As Rectangle
        Select Case seedType
            Case Actor.SeedType.Acorn
                Return _objectFrames(ObjectType.AcornSeed)
            Case Actor.SeedType.Berry
                Return _objectFrames(ObjectType.BerrySeed)
            Case Actor.SeedType.Nut
                Return _objectFrames(ObjectType.NutSeed)
            Case Else
                Return _objectFrames(ObjectType.AcornSeed)
        End Select
    End Function

    Private Sub DrawEnemies(enemies As List(Of Actor.Enemy), deltaTime As Single)
        For Each enemy In enemies
            Dim direction = VectorToDirection(enemy.Direction)
            Dim key = Tuple.Create(enemy.EnemyType, direction)
            Dim animation = _enemyAnimations(key)
            animation.Update(deltaTime)
            Dim frameRect = animation.CurrentFrame

            Dim origin As New Vector2(frameRect.Width / 2.0F, frameRect.Height / 2.0F)
            Dim scale As Single = enemy.Size / MathF.Max(frameRect.Width, frameRect.Height)
            _spriteBatch.Draw(animation.SpriteSheet.Texture, enemy.Position, frameRect, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    Private Function VectorToDirection(vec As Vector2) As Direction
        If vec.X < 0 Then Return Direction.Left
        If vec.X > 0 Then Return Direction.Right
        If vec.Y < 0 Then Return Direction.Up
        Return Direction.Down
    End Function

    Private Sub DrawTrees(trees As List(Of Actor.Tree))
        For Each tree In trees
            Dim frameRect As Rectangle = GetTreeFrame(tree.TreeType)
            Dim origin As New Vector2(frameRect.Width / 2.0F, frameRect.Height)
            Dim scale As Single = tree.Size / MathF.Max(frameRect.Width, frameRect.Height)
            _spriteBatch.Draw(_objectSpriteSheet.Texture, tree.Position, frameRect, Color.White, 0.0F, origin, scale, SpriteEffects.None, 0.0F)
        Next
    End Sub

    Private Function GetTreeFrame(treeType As Actor.TreeType) As Rectangle
        Select Case treeType
            Case Actor.TreeType.Pine
                Return _objectFrames(ObjectType.PineTree)
            Case Actor.TreeType.Fruit
                Return _objectFrames(ObjectType.FruitTree)
            Case Actor.TreeType.Oak
                Return _objectFrames(ObjectType.OakTree)
            Case Else
                Return _objectFrames(ObjectType.OakTree)
        End Select
    End Function

    Private Sub DrawHUD(player As Actor.Player, treeCount As Integer)
        DrawText($"Score: {player.Score}", New Vector2(10, 10), Color.White)
        DrawText($"Seeds: {player.SeedsCollected}/{SEEDS_TO_PLANT_TREE}", New Vector2(10, 35), Color.Green)
        DrawText($"Trees: {treeCount}", New Vector2(10, 60), Color.ForestGreen)
    End Sub

    Private Sub DrawText(text As String, position As Vector2, color As Color)
        _spriteBatch.DrawString(_gameFont, text, position, color)
    End Sub

    Private Sub DrawCenteredText(text As String, yOffset As Integer, color As Color, Optional scale As Single = 1.0F)
        Dim textSize As Vector2 = _gameFont.MeasureString(text) * scale
        Dim position As New Vector2(
            (SCREEN_WIDTH - textSize.X) / 2,
            (SCREEN_HEIGHT - textSize.Y) / 2 + yOffset
        )
        _spriteBatch.DrawString(_gameFont, text, position, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0)
    End Sub

    Private Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                _spriteBatch?.Dispose()
                _pixelTexture?.Dispose()
            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class