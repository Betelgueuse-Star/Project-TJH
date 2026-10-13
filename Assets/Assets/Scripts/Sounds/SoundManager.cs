using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }

    public event EventHandler OnVolumeChanged;

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    [SerializeField] private AudioClip buttonClickSound;

    private float volume = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one SoundManager instance");
        }
        Instance = this;

        //aqui a gente pega oq foi salva, o 1f e o default value, caso nada foi salvo ainda
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);

    }
    private void Start()
    {
        //DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        //DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        //CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        //Player.Instance.OnPickedSomething += Instance_OnPickedSomething;
        //BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        //TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }
    //private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    //{
    //    TrashCounter trashCounter = sender as TrashCounter;
    //    PlaySoundArray(audioClipRefsSO.trash, trashCounter.transform.position);
    //}

    //private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
    //{
    //    BaseCounter baseCounter = sender as BaseCounter;
    //    PlaySoundArray(audioClipRefsSO.objectDrop, baseCounter.transform.position);
    //}

    //private void Instance_OnPickedSomething(object sender, System.EventArgs e)
    //{
    //    PlaySoundArray(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
    //}

    //private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e) //sender e quem chamou o evento, nesse caso o CuttingCounter
    //{
    //    CuttingCounter cuttingCounter = sender as CuttingCounter;
    //    PlaySoundArray(audioClipRefsSO.chops, cuttingCounter.transform.position);
    //}

    //private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    //{
    //    DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
    //    PlaySoundArray(audioClipRefsSO.deliveryFail, deliveryCounter.transform.position);
    //}

    //private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e)
    //{
    //    DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
    //    PlaySoundArray(audioClipRefsSO.deliverySucces, deliveryCounter.transform.position);//usa a posica do delivery counter para tocar o som
    //}

    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplayer = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplayer * volume); //playclipatpoint pede um clip, a posicao que vai ser tocado, e o volume
    }

    private void PlaySoundArray(AudioClip[] audioClipArray, Vector3 position, float volumeMultiplayer = 1f)
    {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volumeMultiplayer * volume); //pega um clip aleatorio do array e toca ele
        //multiplica o volume que recebe pelo volume daqui
    }

    public void PlayFootstepSound(Vector3 position, float volumeMultiplayer)
    {
        // PlaySoundArray(audioClipRefsSO.footstep, position, volumeMultiplayer * volume);
    }

    public void PlayCountdownSound()
    {
        //PlaySoundArray(audioClipRefsSO.warning, Vector3.zero, 1 * volume);
    }
    public void PlayOnSelectedSound()
    {
        PlaySound(buttonClickSound, Vector3.zero, 1 * volume);
    }

    public void ChangeVolume(float soundEffectVolume)
    {
        soundEffectVolume = Mathf.Round(soundEffectVolume * 10f) / 10f; //arredonda o float

        volume = soundEffectVolume;

        OnVolumeChanged?.Invoke(this, EventArgs.Empty); //troca o volume do fogao

        //pede a string que ele vai armazenar, e oque vai armazenar
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume); //player prefs armazena dados, unity normalmente salva automaticamente
        // um dos problemas e quando por algum motivo unity crasha entre setfloat e quando realmente salve, por isso podemos salvar manualmente

        PlayerPrefs.Save(); //salva
    }

    public float GetVolume()
    {
        return volume;
    }
}
