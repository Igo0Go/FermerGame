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

    void Awake()
    {
        GameController.HIT.AddListener(OnHit);
        GameController.ENEMY_HIT.AddListener(OnChangeScore);
        GameController.CHANGE_SPRINT_COUNT.AddListener(OnChangeSprint);
        GameController.WEAPON_ARE_CHANGED.AddListener(OnChangeWeapon);
        GameController.AMMO_ARE_CHANGED.AddListener(OnChangeAmmo);
        GameController.CHANGE_HEALTH.AddListener(OnChangeHealth);
        GameController.CHANGE_MAX_HEALTH.AddListener(OnChangeMaxHealth);
        GameController.TAKE_BONUS_JUMP.AddListener(OnTakeBonusJump);
        GameController.TAKE_BONUS_SPEED.AddListener(OnTakeBonusSpeed);
        GameController.TAKE_BONUS_DAMAGE.AddListener(OnTakeBonusDamage);
        GameController.TAKE_BONUS_INVULNERABLE.AddListener(OnTakeBonusInvulrable);
        GameController.PLAYER_DEAD.AddListener(OnPlayerDead);
        GameController.NEXT_WAVE.AddListener(OnNextWave);
        GameController.DAMAGE_MARKER_ACTIVATE.AddListener(OnDamageMarkerActivate);
        GameController.START_SPRINT.AddListener(EnebleSprintEffect);
        GameController.STOP_SPRINT.AddListener(DisableAllSprintEffects);
        GameController.ENEMY_DEAD.AddListener(OnEnemyDead);
        GameController.START_FINAL_LOADING.AddListener(StartBlackPanelCoroutine);
    }

   private void Start()
    {
        DontDestroyOnLoad(gameObject);

        Setup();
    }

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
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("Mouse", 0.5f);
        PlayerPrefs.SetFloat("Music", 0.25f);
        PlayerPrefs.SetFloat("Sounds", 0.15f);
        PlayerPrefs.SetFloat("Voices", 1);

        settingsPanel.SetActive(true);
        OnLoad();
        OnMusicValueChanged();
        OnSoundsValueChanged();
        SettingsPanelToggle();
        score = 0;
        OnChangeScore(0);
        hitMarker.SetActive(false);
        jumpBonusSlider.value = speedBonusSlider.value = damageBonusSlider.value = invilnvurableBonusSlider.value = 0;
        finalPanel.SetActive(false);
        healthSlider.value = healthSlider.maxValue;
        waveCounterText.text = string.Empty;
        mouseSlider.value = PlayerPrefs.GetFloat("Mouse", 0.5f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("Music", 0.25f);
        soundsVolumeSlider.value = PlayerPrefs.GetFloat("Sounds", 0.25f);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat("Voices", 1);

        DisableAllSprintEffects();

        OnMusicValueChanged();
        OnMouseValueChanged();
        OnSoundsValueChanged();
        OnVoiceValueChanged();

        opportunityToShowSettings = true;

        ConsoleEventCenter.ShowConsoleChanged.AddListener(SetPauseValue);
    }

    private void StartBlackPanelCoroutine()
    {
        Destroy(gameObject, 7);
        blackPanel.gameObject.SetActive(true);
        StartCoroutine(BlackPanelCoroutine());
    }

    private IEnumerator BlackPanelCoroutine()
    {
        Color bufer = blackPanel.color;
        float t = 0;
        while(blackPanel.color.a < 255)
        {
            t += Time.deltaTime / 5;
            blackPanel.color = Color.Lerp(bufer, Color.black, t);
            yield return null;
        }
    }

    private void OnNextWave(int number)
    {
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
            damagePanelAnim.SetBool("NoDamaged", true);
            invilnvurableBonusSlider.gameObject.SetActive(true);
            invilnvurableBonusSlider.maxValue = invilnirableBonusTime;
            invilnvurableBonusSlider.value = invilnirableBonusTime;
        }
    }

    private void OnChangeWeapon(int index)
    {
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
        PlayerPrefs.SetFloat("Music", musicVolumeSlider.value);
        GameController.MUSIC_CHANGED.Invoke(musicVolumeSlider.value);
    }
    public void OnSoundsValueChanged()
    {
        PlayerPrefs.SetFloat("Sounds", soundsVolumeSlider.value);
        GameController.SOUNDS_CHANGED.Invoke(soundsVolumeSlider.value);
    }
    public void OnVoiceValueChanged()
    {
        PlayerPrefs.SetFloat("Voices", voiceVolumeSlider.value);
        GameController.VOICE_CHANGED.Invoke(voiceVolumeSlider.value);
    }
    public void OnMouseValueChanged()
    {
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
        OnExit();
        Invoke(nameof(RestartScene), 1);
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
    
    private void OnLoad()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("Music", 0.5f);
        soundsVolumeSlider.value = PlayerPrefs.GetFloat("Sounds", 0.5f);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat("Voices", 1);
    }
    private void OnExit()
    {
        GameController.EXIT_LEVEL.Invoke();
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
    private void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
