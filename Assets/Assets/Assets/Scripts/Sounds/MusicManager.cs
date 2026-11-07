using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips;

    // [SerializeField] private TextMeshProUGUI currentMusicText; // Exibe nome da musica atual

    public static MusicManager Instance { get; private set; }
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume"; // Chave para armazenar o volume da música no PlayerPrefs.

    private AudioSource audioSource;
    private AudioClip currentMusic;

    private List<int> playlist = new List<int>(); // Lista de índices da PlayList
    private int playlistIndex = 0; // Index da PlayList

    private float volume = .2f; // default volume

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Verifica se já existe uma instância do MusicManager
        if (Instance != null) 
        {
            Debug.LogError("There is more than one Music instance");
        }

        Instance = this;

        // Pega o volume salvo no PlayerPrefs, ou usar o valor padrão de 0.2f se não houver valor salvo.
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .2f); 
        audioSource.volume = volume;

        CreatePlaylist(); // Cria
        ShufflePlaylist(); // Embaralha
        PlayCurrentMusic(); // Toca
    }

    private void Update()
    {
        // Se não existe música carregada, não faz nada.
        if (audioSource.clip == null) return;

        // Verifica se a música atual terminou de tocar.
        if (audioSource.time >= audioSource.clip.length) 
        {
            PlayNextMusic();
        }
    }

    private void CreatePlaylist()
    {
        // Cria a lista Playlist com os índices das músicas disponíveis no array audioClips.
        for (int i = 0; i < audioClips.Length; i++) 
        {
            playlist.Add(i);
        }
    }

    private void ShufflePlaylist()
    {
        // Embaralha a lista Playlist usando o algoritmo de Fisher-Yates Shuffle

        for (int i = playlist.Count - 1; i > 0; i--)
        {
            // Escolhe uma posição aleatória entre 0 e i.
            int randomIndex = Random.Range(0, i + 1);

            // Guarda temporariamente o valor atual.
            int temp = playlist[i];

            // Troca os valores de posição.
            playlist[i] = playlist[randomIndex];
            playlist[randomIndex] = temp;
        }
    }

    private void PlayCurrentMusic()
    {
        int musicIndex = playlist[playlistIndex]; // Pega o indice 0

        currentMusic = audioClips[musicIndex]; // Seta a musica atual com o musicIndex
        audioSource.clip = currentMusic; // Seta o clip do audioSource com a musica atual

        audioSource.Play();

        // currentMusicText.text = currentMusic.name; // Texto
    }

    private void PlayNextMusic()
    {
        playlistIndex++; // Avança para o próximo índice da playlist.

        // Se chegamos ao final da playlist, reiniciamos do início e embaralhamos novamente.
        if (playlistIndex >= playlist.Count) 
        {
            playlistIndex = 0;
            ShufflePlaylist();
        }

        PlayCurrentMusic(); // Toca a Musica
    }
    public void SkipMusic()
    {
        audioSource.Stop();
        PlayNextMusic();
    }

    public void ChangeVolume(float musicVolume)
    {
        musicVolume = Mathf.Round(musicVolume * 10f) / 10f; // arredonda o float
        volume = musicVolume;
        audioSource.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume); // armazena dados
        PlayerPrefs.Save(); // salva
    }

    public float GetVolume()
    {
        return volume;
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }
}