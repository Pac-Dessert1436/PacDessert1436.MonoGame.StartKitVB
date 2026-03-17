''' <summary>
''' Contains essential constants, functions, enums, etc. used throughout the game.
''' </summary>
Public Module Essentials
    Public Const SCREEN_WIDTH As Integer = 800
    Public Const SCREEN_HEIGHT As Integer = 600

    Public Enum GameState As Integer
        Title = 0
        Playing = 1
        Paused = 2
        GameOver = 3
    End Enum

    Public Const PLAYER_SIZE As Integer = 30
    Public Const PLAYER_SPEED As Single = 200.0F
    Public Const SEED_SIZE As Integer = 15
    Public Const ENEMY_SIZE As Integer = 25
    Public Const ENEMY_SPEED As Single = 100.0F
    Public Const SEEDS_TO_PLANT_TREE As Integer = 15

#Region "Events"
    Public Event GameStateChanged(newState As GameState)
#End Region
End Module