using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIDispetcher : MonoBehaviour
{
    public ReplicDispether replicDispether;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject hitMarker;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private List<GameObject> sprintCells;
    [SerializeField] private List<GameObject> weaponImages;

    [SerializeField] private Text scoreLabel;
    [SerializeField] private Text ammoText;
    [SerializeField] private Text scoreMultiplicatorText;
    [SerializeField] private Text waveCounterText;
    [SerializeField] private Text finalScoreValueText;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundsVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;
    [SerializeField] private Slider mouseSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider jumpBonusSlider;
    [SerializeField] private Slider speedBonusSlider;
    [SerializeField] private Slider damageBonusSlider;
    [SerializeField] private Slider invilnvurableBonusSlider;

    [SerializeField] private GameObject musicPalyerPanel;
    [SerializeField] private Text musicClipName;
    [SerializeField] private Text musicClipNameForLoading;

    [SerializeField] private GameObject bonusPanel;
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private GameObject WavePanel;


    [SerializeField]
    [Tooltip("0 - влево, 1 - вправо")]
    private List<GameObject> sprintMarkers;

    [SerializeField] private Animator scoreMultiplicatorAnim;
    [SerializeField] private List<Animator> damageMarkersAnimators;
    [SerializeField] private Animator damagePanelAnim;

    [SerializeField, Range(1,120)] private float jumpBonusTime = 60;
    [SerializeField, Range(1, 120)] private float speedBonusTime = 60;
    [SerializeField, Range(1, 120)] private float damageBonusTime = 60;
    [SerializeField, Range(1, 120)] private float invilnirableBonusTime = 60;

    [SerializeField] private Image blackPanel;

    private bool opportunityToShowSettings = true;
    private int score;
    private float scoreReturnTime;

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && opportunityToShowSettings)
            SettingsPanelToggle();

        CheckSliders();
        scoreReturnTime -= Time.deltaTime;
        if(scoreReturnTime<= 0)
        {
            scoreReturnTime = 0;
            scoreMultiplicatorText.text = string.Empty;
            PlayerBonusStat.scoreMultiplicator = 1;
        }

        for (int i = 0; i < damageMarkersAnimators.Count; i++)
        {
            damageMarkersAnimators[i].SetFloat("ActiveMarker", 0, Time.deltaTime * 10, Time.deltaTime);
        }
    }

    public void Setup()
    {
        GameController.HIT.AddListener(OnHit);
        GameController.ENEMY_HIT.AddListener(OnChangeScore);
        GameController.ENEMY_DEAD.AddListener(OnEnemyDead);

        GameController.START_SPRINT.AddListener(EnebleSprintEffect);
        GameController.STOP_SPRINT.AddListener(DisableAllSprintEffects);
        GameController.CHANGE_SPRINT_COUNT.AddListener(OnChangeSprint);

        GameController.CHANGE_MAX_HEALTH.AddListener(OnChangeMaxHealth);
        GameController.CHANGE_HEALTH.AddListener(OnChangeHealth);
        GameController.PLAYER_DEAD.AddListener(OnPlayerDead);
        GameController.DAMAGE_MARKER_ACTIVATE.AddListener(OnDamageMarkerActivate);

        GameController.WEAPON_ARE_CHANGED.AddListener(OnChangeWeapon);
        GameController.AMMO_ARE_CHANGED.AddListener(OnChangeAmmo);

        GameController.TAKE_BONUS_JUMP.AddListener(OnTakeBonusJump);
        GameController.TAKE_BONUS_SPEED.AddListener(OnTakeBonusSpeed);
        GameController.TAKE_BONUS_DAMAGE.AddListener(OnTakeBonusDamage);
        GameController.TAKE_BONUS_INVULNERABLE.AddListener(OnTakeBonusInvulrable);

        GameController.NEXT_WAVE.AddListener(OnNextWave);
        GameController.START_FINAL_LOADING.AddListener(StartBlackPanelCoroutine);

        GameController.PLAYER_MUSIC_CHANGED.AddListener(OnChangeMusicClip);
        GameController.PLAYER_MUSIC_LOAD_CLIP_COMPLETED.AddListener(OnClipLoaded);
        GameController.PLAYER_MUSIC_LOADED.AddListener(OnAllMusicLoaded);

        blackPanel.gameObject.SetActive(true);
        blackPanel.color = Color.black;

        settingsPanel.SetActive(true);

        SettingsPanelToggle();
        score = 0;
        OnChangeScore(0);
        hitMarker.SetActive(false);
        jumpBonusSlider.value = speedBonusSlider.value = damageBonusSlider.value = invilnvurableBonusSlider.value = 0;
        finalPanel.SetActive(false);
        healthSlider.value = healthSlider.maxValue;
        waveCounterText.text = string.Empty;
        mouseSlider.value = GameController.MouseSensivity;
        musicVolumeSlider.value = GameController.MusicVolume;
        soundsVolumeSlider.value = GameController.SoundsVolume;
        voiceVolumeSlider.value = GameController.VoicesVolume;

        DisableAllSprintEffects();

        OnMusicValueChanged();
        OnMouseValueChanged();
        OnSoundsValueChanged();
        OnVoiceValueChanged();

        opportunityToShowSettings = true;

        ConsoleEventCenter.ShowConsoleChanged.AddListener(SetPauseValue);
        musicPalyerPanel.SetActive(false);
    }

    private void StartBlackPanelCoroutine()
    {
        Destroy(gameObject, 7.5f);
        StartCoroutine(BlackPanelCoroutine(true));
    }

    private IEnumerator BlackPanelCoroutine(bool toBlack)
    {
        Color alpha = new Color(0,0,0,0);
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime / 5;
            if (toBlack)
            {
                blackPanel.color = Color.Lerp(alpha, Color.black, t);
            }
            else
            {
                blackPanel.color = Color.Lerp(Color.black, alpha, t);
            }
            yield return null;
        }
        blackPanel.color = toBlack ? Color.black : alpha;
    }

    private void OnNextWave(int number)
    {
        WavePanel.SetActive(true);
        waveCounterText.text = "ВОЛНА " + (number + 1);
    }
    private void OnEnemyDead()
    {
        if(scoreReturnTime > 0 && PlayerBonusStat.scoreMultiplicator < 10)
        {
            PlayerBonusStat.scoreMultiplicator++;
            scoreMultiplicatorText.text = "X" + PlayerBonusStat.scoreMultiplicator;
            scoreMultiplicatorText.color = Color.Lerp(Color.green, Color.red, 0.1f * PlayerBonusStat.scoreMultiplicator);
            scoreMultiplicatorAnim.SetTrigger("ChangeScoreMultiplicator");
        }
        scoreReturnTime = 5;
    }
    private void OnPlayerDead()
    {
        opportunityToShowSettings = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        GameController.PAUSE.Invoke(true);
        finalPanel.SetActive(true);
        finalScoreValueText.text = score.ToString();
    }

    private void OnDamageMarkerActivate(int number)
    {
        damageMarkersAnimators[number].SetFloat("ActiveMarker", 4);
    }
    private void OnTakeBonusJump(int value)
    {
        if(value == 1)
        {
            jumpBonusSlider.gameObject.SetActive(false);
        }
        else
        {
            bonusPanel.SetActive(true);
            jumpBonusSlider.gameObject.SetActive(true);
            jumpBonusSlider.maxValue = jumpBonusTime;
            jumpBonusSlider.value = jumpBonusTime;
        }
    }
    private void OnTakeBonusSpeed(int value)
    {
        if (value == 1)
        {
            speedBonusSlider.gameObject.SetActive(false);
        }
        else
        {
            bonusPanel.SetActive(true);
            speedBonusSlider.gameObject.SetActive(true);
            speedBonusSlider.maxValue = speedBonusTime;
            speedBonusSlider.value = speedBonusTime;
        }
    }
    private void OnTakeBonusDamage(int value)
    {
        if (value == 1)
        {
            damageBonusSlider.gameObject.SetActive(false);
        }
        else
        {
            bonusPanel.SetActive(true);
            damageBonusSlider.gameObject.SetActive(true);
            damageBonusSlider.maxValue = damageBonusTime;
            damageBonusSlider.value = damageBonusTime;
        }
    }
    private void OnTakeBonusInvulrable(int value)
    {
        if (value == 1)
        {
            invilnvurableBonusSlider.gameObject.SetActive(false);
            damagePanelAnim.SetBool("NoDamaged", false);
        }
        else
        {
            bonusPanel.SetActive(true);
            damagePanelAnim.SetBool("NoDamaged", true);
            invilnvurableBonusSlider.gameObject.SetActive(true);
            invilnvurableBonusSlider.maxValue = invilnirableBonusTime;
            invilnvurableBonusSlider.value = invilnirableBonusTime;
        }
    }

    private void OnChangeWeapon(int index)
    {
        weaponPanel.SetActive(true);
        foreach (var item in weaponImages)
        {
            item.SetActive(false);
        }
        if(index >= 0)
            weaponImages[index].SetActive(true);
        else
        {
            ammoText.text = string.Empty;
        }
    }
    private void OnChangeAmmo(int count)
    {
        ammoText.text = "X " + count;
    }
    private void OnChangeHealth(float value)
    {
        float old = healthSlider.value;
        healthSlider.value = value;
        if(healthSlider.value > old)
        {
           damagePanelAnim.SetTrigger("Buf");
        }
        else
        {
           damagePanelAnim.SetTrigger("Damage");
        }
    }
    private void OnChangeMaxHealth(float value)
    {
        healthSlider.maxValue = value;
    }
    public void OnMusicValueChanged()
    {
        GameController.MusicVolume = musicVolumeSlider.value;
        GameController.MUSIC_CHANGED.Invoke(musicVolumeSlider.value);
    }
    public void OnSoundsValueChanged()
    {
        GameController.SoundsVolume = soundsVolumeSlider.value;
        GameController.SOUNDS_CHANGED.Invoke(soundsVolumeSlider.value);
    }
    public void OnVoiceValueChanged()
    {
        GameController.VoicesVolume = voiceVolumeSlider.value;
        GameController.VOICE_CHANGED.Invoke(voiceVolumeSlider.value);
    }
    public void OnMouseValueChanged()
    {
        GameController.MouseSensivity = mouseSlider.value;
        GameController.MOUSE_CHANGED.Invoke(mouseSlider.value);
    }
    private void OnChangeScore(int value)
    {
        score += value * PlayerBonusStat.scoreMultiplicator;
        scoreLabel.text = "Счёт: " + score;
    }
    private void OnChangeSprint(int value)
    {
        for (int i = 0; i < sprintCells.Count; i++)
        {
            sprintCells[i].SetActive(i < value);
        }
    }
    private void OnHit()
    {
        hitMarker.SetActive(true);
        Invoke(nameof(ReturnHitMarker), 0.5f);
    }

    private void OnChangeMusicClip(string clipName)
    {
        if(string.IsNullOrEmpty(clipName))
        {
            musicPalyerPanel.SetActive(false);
        }
        else
        {
            musicPalyerPanel.SetActive(true);
            musicClipName.text = clipName;
        }
    }
    private void OnClipLoaded(string clipName)
    {
        musicClipNameForLoading.text = "загружено: " + clipName;
    }
    private void OnAllMusicLoaded()
    {
        WavePanel.SetActive(false);
        weaponPanel.SetActive(false);
        bonusPanel.SetActive(false);
        musicClipNameForLoading.gameObject.SetActive(false);
        StartCoroutine(BlackPanelCoroutine(false));
    }

    private void EnebleSprintEffect(Vector3 direction)
    {
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            if(direction.x > 0)
            {
                sprintMarkers[1].SetActive(true);
            }
            else
            {
                sprintMarkers[0].SetActive(true);
            }
        }
        else
        {
            sprintMarkers[0].SetActive(true);
            sprintMarkers[1].SetActive(true);
        }
    }
    private void DisableAllSprintEffects()
    {
        foreach (var item in sprintMarkers)
        {
            item.SetActive(false);
        }
    }

    public void SettingsPanelToggle()
    {
        if (ConsoleEventCenter.ShowConsole)
            return;

        bool inMenu = !settingsPanel.activeSelf;
        settingsPanel.SetActive(inMenu);
        SetPauseValue(inMenu);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    private bool FindInChildrenByName(string name, Transform root, out GameObject obj)
    {
        obj = null;
        for (int i = 0; i < root.childCount; i++)
        {
            obj = root.GetChild(i).gameObject;
            if (obj.name.Equals(name))
            {
                return true;
            }
            else if (FindInChildrenByName(name, obj.transform, out obj))
                return true;
        }
        return false;
    }

    private void CheckSliders()
    {
        if(jumpBonusSlider.gameObject.activeSelf)
        {
            if(jumpBonusSlider.value - Time.deltaTime <= 0)
            {
                jumpBonusSlider.value = 0;
                GameController.TAKE_BONUS_JUMP.Invoke(1);
            }
            else if(PlayerBonusStat.bonusPack[BonusType.Jump] < 3)
            {
                jumpBonusSlider.value -= Time.deltaTime;
            }
        }
        if (speedBonusSlider.gameObject.activeSelf)
        {
            if (speedBonusSlider.value - Time.deltaTime <= 0)
            {
                speedBonusSlider.value = 0;
                GameController.TAKE_BONUS_SPEED.Invoke(1);
            }
            else if (PlayerBonusStat.bonusPack[BonusType.Speed] < 3)
            {
                speedBonusSlider.value -= Time.deltaTime;
            }
        }
        if (damageBonusSlider.gameObject.activeSelf)
        {
            if (damageBonusSlider.value - Time.deltaTime <= 0)
            {
                damageBonusSlider.value = 0;
                GameController.TAKE_BONUS_DAMAGE.Invoke(1);
            }
            else if (PlayerBonusStat.bonusPack[BonusType.Damage] < 3)
            {
                damageBonusSlider.value -= Time.deltaTime;
            }
        }
        if (invilnvurableBonusSlider.gameObject.activeSelf)
        {
            if (invilnvurableBonusSlider.value - Time.deltaTime <= 0)
            {
                invilnvurableBonusSlider.value = 0;
                GameController.TAKE_BONUS_INVULNERABLE.Invoke(1);
            }
            else if (PlayerBonusStat.bonusPack[BonusType.Invulnerable] < 3)
            {
                invilnvurableBonusSlider.value -= Time.deltaTime;
            }
        }
    }

    private void SetPauseValue(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = value ? 0 : 1;
        GameController.PAUSE.Invoke(value);
    }

    private void ReturnHitMarker()
    {
        hitMarker.SetActive(false);
    }
}
