Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Media
Imports Microsoft.Xna.Framework.Content

''' <summary>
''' Manages all audio playback including background music and sound effects.
''' </summary>
Public NotInheritable Class SoundManager
    Private ReadOnly _content As ContentManager
    
    ' Sound effects
    Private _sndSeedPacket As SoundEffect
    Private _sndGameStart As SoundEffect
    Private _sndGameOver As SoundEffect
    Private _sndLevelCleared As SoundEffect
    Private _sndLifeLost As SoundEffect
    
    ' Background music
    Private _bgmMainTheme As Song
    Private _isMusicPlaying As Boolean = False
    
    Public Sub New(content As ContentManager)
        _content = content
        LoadSounds()
        SetupEventHandlers()
    End Sub
    
    ''' <summary>
    ''' Loads all sound assets.
    ''' </summary>
    Private Sub LoadSounds()
        ' Load sound effects
        _sndSeedPacket = _content.Load(Of SoundEffect)("Sounds/seed_packet")
        _sndGameStart = _content.Load(Of SoundEffect)("Sounds/game_start")
        _sndGameOver = _content.Load(Of SoundEffect)("Sounds/game_over")
        _sndLevelCleared = _content.Load(Of SoundEffect)("Sounds/level_cleared")
        _sndLifeLost = _content.Load(Of SoundEffect)("Sounds/life_lost")
        
        ' Load background music
        _bgmMainTheme = _content.Load(Of Song)("Sounds/BGM/main_theme")
    End Sub
    
    ''' <summary>
    ''' Sets up event handlers for game events.
    ''' </summary>
    Private Sub SetupEventHandlers()
        AddHandler GameStateChanged, AddressOf OnGameStateChanged
        AddHandler SeedCollected, AddressOf OnSeedCollected
        AddHandler TreePlanted, AddressOf OnTreePlanted
        AddHandler PlayerDied, AddressOf OnPlayerDied
    End Sub
    
    ''' <summary>
    ''' Handles game state changes for audio.
    ''' </summary>
    Private Sub OnGameStateChanged(newState As GameState)
        Select Case newState
            Case GameState.Playing
                _sndGameStart.Play()
                PlayBackgroundMusic()
                
            Case GameState.GameOver
                _sndGameOver.Play()
                StopBackgroundMusic()
                
            Case GameState.Title
                StopBackgroundMusic()
        End Select
    End Sub

    ''' <summary>
    ''' Handles seed collection sound.
    ''' </summary>
    Private Sub OnSeedCollected(seed As Actor.Seed)
        _sndSeedPacket.Play()
    End Sub

    ''' <summary>
    ''' Handles tree planting sound.
    ''' </summary>
    Private Sub OnTreePlanted(tree As Actor.Tree)
        _sndLevelCleared.Play()
    End Sub

    ''' <summary>
    ''' Handles player death sound.
    ''' </summary>
    Private Sub OnPlayerDied()
        _sndLifeLost.Play()
    End Sub
    
    ''' <summary>
    ''' Starts playing background music.
    ''' </summary>
    Private Sub PlayBackgroundMusic()
        If Not _isMusicPlaying Then
            MediaPlayer.Play(_bgmMainTheme)
            MediaPlayer.IsRepeating = True
            _isMusicPlaying = True
        End If
    End Sub
    
    ''' <summary>
    ''' Stops background music.
    ''' </summary>
    Private Sub StopBackgroundMusic()
        If _isMusicPlaying Then
            MediaPlayer.Stop()
            _isMusicPlaying = False
        End If
    End Sub
    
    ''' <summary>
    ''' Disposes of sound resources.
    ''' </summary>
    Public Sub Dispose()
        RemoveHandler GameStateChanged, AddressOf OnGameStateChanged
        RemoveHandler SeedCollected, AddressOf OnSeedCollected
        RemoveHandler TreePlanted, AddressOf OnTreePlanted
        RemoveHandler PlayerDied, AddressOf OnPlayerDied
        
        StopBackgroundMusic()
    End Sub
End Class