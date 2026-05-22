Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Media
Imports Microsoft.Xna.Framework.Content

''' <summary>
''' Manages all audio playback including background music and sound effects.
''' </summary>
Public NotInheritable Class SoundManager
    Implements IDisposable

    Private ReadOnly _content As ContentManager

    ' Sound effects
    Private _sndSeedPacket As SoundEffect
    Private _sndPesticide As SoundEffect
    Private _sndEnemyKilled As SoundEffect
    Private _sndEnemyReappear As SoundEffect
    Private _sndGameStart As SoundEffect
    Private _sndAtNextLevel As SoundEffect
    Private _sndGameOver As SoundEffect
    Private _sndLifeLost As SoundEffect

    ' Background music
    Private _bgmMainTheme As Song
    Private _isMusicPlaying As Boolean = False
    Private disposedValue As Boolean

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
        _sndPesticide = _content.Load(Of SoundEffect)("Sounds/pesticide")
        _sndEnemyKilled = _content.Load(Of SoundEffect)("Sounds/enemy_killed")
        _sndEnemyReappear = _content.Load(Of SoundEffect)("Sounds/enemy_reappear")
        _sndGameStart = _content.Load(Of SoundEffect)("Sounds/game_start")
        _sndAtNextLevel = _content.Load(Of SoundEffect)("Sounds/at_next_level")
        _sndGameOver = _content.Load(Of SoundEffect)("Sounds/game_over")
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
        AddHandler PesticideCollected, AddressOf OnPesticideCollected
        AddHandler EnemyKilled, AddressOf OnEnemyKilled
        AddHandler EnemyRespawned, AddressOf OnEnemyRespawned
        AddHandler PlayerDied, AddressOf OnPlayerDied
        AddHandler LifeLost, AddressOf OnLifeLost
        AddHandler LevelCleared, AddressOf OnLevelCleared
        AddHandler GetReadyMessage, AddressOf OnGetReadyMessage
        AddHandler GameStart, AddressOf OnGameStart
        AddHandler NextLevel, AddressOf OnNextLevel
    End Sub

    ''' <summary>
    ''' Handles game state changes for audio.
    ''' </summary>
    Private Sub OnGameStateChanged(newState As GameState)
        Select Case newState
            Case GameState.Playing
                PlayBackgroundMusic()

            Case GameState.GameOver
                _sndGameOver.Play()
                StopBackgroundMusic()

            Case GameState.Title
                StopBackgroundMusic()
                
            Case GameState.Paused
                MediaPlayer.Pause()
                
            Case GameState.LevelCleared
                ' Continue playing music
        End Select
    End Sub

    ''' <summary>
    ''' Handles get ready message for audio.
    ''' </summary>
    Private Sub OnGetReadyMessage()
        If MediaPlayer.State = MediaState.Paused Then
            MediaPlayer.Resume()
        End If
    End Sub

    ''' <summary>
    ''' Handles game start sound.
    ''' </summary>
    Private Sub OnGameStart()
        _sndGameStart.Play()
    End Sub

    ''' <summary>
    ''' Handles next level sound.
    ''' </summary>
    Private Sub OnNextLevel()
        _sndAtNextLevel.Play()
    End Sub

    ''' <summary>
    ''' Handles seed collection sound.
    ''' </summary>
    Private Sub OnSeedCollected(seed As Actor.Seed)
        _sndSeedPacket.Play()
    End Sub

    ''' <summary>
    ''' Handles pesticide collection sound.
    ''' </summary>
    Private Sub OnPesticideCollected()
        _sndPesticide.Play()
    End Sub

    ''' <summary>
    ''' Handles enemy killed sound.
    ''' </summary>
    Private Sub OnEnemyKilled(enemy As Actor.Enemy)
        _sndEnemyKilled.Play()
    End Sub

    ''' <summary>
    ''' Handles enemy respawned sound.
    ''' </summary>
    Private Sub OnEnemyRespawned(enemy As Actor.Enemy)
        _sndEnemyReappear.Play()
    End Sub

    ''' <summary>
    ''' Handles player death sound.
    ''' </summary>
    Private Sub OnPlayerDied()
        _sndGameOver.Play()
        StopBackgroundMusic()
    End Sub

    ''' <summary>
    ''' Handles life lost sound.
    ''' </summary>
    Private Sub OnLifeLost()
        _sndLifeLost.Play()
    End Sub

    ''' <summary>
    ''' Handles level cleared sound.
    ''' </summary>
    Private Sub OnLevelCleared()
        _sndAtNextLevel.Play()
    End Sub

    ''' <summary>
    ''' Starts playing background music.
    ''' </summary>
    Private Sub PlayBackgroundMusic()
        If Not _isMusicPlaying Then
            MediaPlayer.Play(_bgmMainTheme)
            MediaPlayer.IsRepeating = True
            _isMusicPlaying = True
        ElseIf MediaPlayer.State = MediaState.Paused Then
            MediaPlayer.Resume()
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
    Private Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                RemoveHandler GameStateChanged, AddressOf OnGameStateChanged
                RemoveHandler SeedCollected, AddressOf OnSeedCollected
                RemoveHandler PesticideCollected, AddressOf OnPesticideCollected
                RemoveHandler EnemyKilled, AddressOf OnEnemyKilled
                RemoveHandler EnemyRespawned, AddressOf OnEnemyRespawned
                RemoveHandler PlayerDied, AddressOf OnPlayerDied
                RemoveHandler LifeLost, AddressOf OnLifeLost
                RemoveHandler LevelCleared, AddressOf OnLevelCleared
                RemoveHandler GetReadyMessage, AddressOf OnGetReadyMessage
                RemoveHandler GameStart, AddressOf OnGameStart
                RemoveHandler NextLevel, AddressOf OnNextLevel

                StopBackgroundMusic()
            End If

            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class