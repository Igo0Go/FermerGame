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
        Messenger.AddListener(GameEvent.HIT, OnHit);
        Messenger<int>.AddListener(GameEvent.ENEMY_HIT, OnChangeScore);
        Messenger<int>.AddListener(GameEvent.CHANGE_SPRINT_COUNT, OnChangeSprint);
        Messenger<int>.AddListener(GameEvent.WEAPON_ARE_CHANGED, OnChangeWeapon);
        Messenger<int>.AddListener(GameEvent.AMMO_ARE_CHANGED, OnChangeAmmo);
        Messenger<float>.AddListener(GameEvent.CHANGE_HEALTH, OnChangeHealth);
        Messenger<float>.AddListener(GameEvent.CHANGE_MAX_HEALTH, OnChangeMaxHealth);
        
        Messenger<int>.AddListener(GameEvent.TAKE_BONUS_JUMP, OnTakeBonusJump);
        Messenger<int>.AddListener(GameEvent.TAKE_BONUS_SPEED, OnTakeBonusSpeed);
        Messenger<int>.AddListener(GameEvent.TAKE_BONUS_DAMAGE, OnTakeBonusDamage);
        Messenger<int>.AddListener(GameEvent.TAKE_BONUS_INVULNERABLE, OnTakeBonusInvulrable);
        Messenger.AddListener(GameEvent.PLAYER_DEAD, OnPlayerDead);
        Messenger<int>.AddListener(GameEvent.NEXT_WAVE, OnNextWave);
        Messenger<int>.AddListener(GameEvent.DAMAGE_MARKER_ACTIVATE, OnDamageMarkerActivate);
        Messenger<Vector3>.AddListener(GameEvent.START_SPRINT, EnebleSprintEffect);
        Messenger.AddListener(GameEvent.STOP_SPRINT, DisableAllSprintEffects);
        Messenger.AddListener(GameEvent.ENEMY_DEAD, OnEnemyDead);
        Messenger.AddListener(GameEvent.START_FINAL_LOADING, StartBlackPanelCoroutine);

        //Messenger.AddListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }
    void OnDestroy()
    {
        Messenger.RemoveListener(GameEvent.HIT, OnHit);
        Messenger<int>.RemoveListener(GameEvent.ENEMY_HIT, OnChangeScore);
        Messenger<int>.RemoveListener(GameEvent.CHANGE_SPRINT_COUNT, OnChangeSprint);
        Messenger<float>.RemoveListener(GameEvent.CHANGE_HEALTH, OnChangeHealth);
        Messenger<float>.RemoveListener(GameEvent.CHANGE_MAX_HEALTH, OnChangeMaxHealth);

        Messenger<int>.RemoveListener(GameEvent.TAKE_BONUS_JUMP, OnTakeBonusJump);
        Messenger<int>.RemoveListener(GameEvent.TAKE_BONUS_SPEED, OnTakeBonusJump);
        Messenger<int>.RemoveListener(GameEvent.TAKE_BONUS_DAMAGE, OnTakeBonusJump);
        Messenger<int>.RemoveListener(GameEvent.TAKE_BONUS_INVULNERABLE, OnTakeBonusJump);
        Messenger<int>.RemoveListener(GameEvent.NEXT_WAVE, OnNextWave);
        Messenger<int>.RemoveListener(GameEvent.DAMAGE_MARKER_ACTIVATE, OnDamageMarkerActivate);

        Messenger<Vector3>.RemoveListener(GameEvent.START_SPRINT, EnebleSprintEffect);
        Messenger.RemoveListener(GameEvent.STOP_SPRINT, DisableAllSprintEffects);

        Messenger.RemoveListener(GameEvent.ENEMY_DEAD, OnEnemyDead);
        Messenger.RemoveListener(GameEvent.START_FINAL_LOADING, StartBlackPanelCoroutine);
        //Messenger.RemoveListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }

   private void Start()
    {
        DontDestroyOnLoad(gameObject);

        #region Буду благодарен, если посмотрите эту часть

        /*
        
        Изначально для перезапуска я планировал просто перезагружать сцену. Однако обнаружилась пролема - ссылки на UI элементы были равны 'null после перезагрузки'.
        При использвании сцены загрузки как буфера, результат тот же. Естественно, всё дебажил. Ниже код. Я пытался подгружать все объекты вручную, поскольку знаю строение
        префаба. Ни один из этих способов не помог. Вообще создалось впечатление, что ссылки обнуляются уже после того, как отработает Start(). Возможно из-за использования
        Messanger. Как будто не вызывается OnDestroy и ссылки на Messanger не удаляются, из-за чего удаление элементов происходит некорреткно. Но в таком случае, почему только
        интерфейс?
         
        В итоге мне пришлось помечать игрока и интерфейс как DontDestroyOnLoad(). Вся сцена перезагружаюется, а игрок и интерфейс просто вручную переводились в изначальное
        состояние. Я считаю, что это - костыль. Однако я общался с другими людьми, которые занимаются Unity профессионально, и мне сказали, что такой ход вполне рабочее решение.
         
        Если не затруднит и сталкивались с таким случаем, дайте, пожалуйста, комментарий по этому случаю. Заранее благодарю.

         */

        //GameObject bufer;
        //if(FindInChildrenByName("SettingsPanel", transform, out bufer))
        //{
        //    settingsPanel = bufer;
        //}
        //if (FindInChildrenByName("ScoreLabel", transform, out bufer))
        //{
        //    scoreLabel = bufer.GetComponent<Text>();
        //}
        //if (FindInChildrenByName("MusicSlider", transform, out bufer))
        //{
        //    musicSlider = bufer.GetComponent<Slider>();
        //}
        //if (FindInChildrenByName("SoundsSlider", transform, out bufer))
        //{
        //    soundsSlider = bufer.GetComponent<Slider>();
        //}
        //if (FindInChildrenByName("HealthSlider", transform, out bufer))
        //{
        //    healthSlider = bufer.GetComponent<Slider>();
        //}
        //if(FindInChildrenByName("HitMarker", transform, out bufer))
        //{
        //    hitMarker = bufer;
        //}
        //for (int i = 0; i < 3; i++)
        //{
        //    if (FindInChildrenByName("SprintLight" + (i+1), transform, out bufer))
        //    {
        //        sprintLights[i] = bufer;
        //    }
        //}
        //for (int i = 0; i < 4; i++)
        //{
        //    if (FindInChildrenByName("WeaponIkon" + (i + 1), transform, out bufer))
        //    {
        //        weaponImages[i] = bufer;
        //    }
        //}
        //if (FindInChildrenByName("AmmoText", transform, out bufer))
        //{
        //    ammoText = bufer.GetComponent<Text>();
        //}
        //if (FindInChildrenByName("FinalPanel", transform, out bufer))
        //{
        //    finalPanel = bufer;
        //}
        //if (FindInChildrenByName("FinalScoreValueText", transform, out bufer))
        //{
        //    finalScoreValueText = bufer.GetComponent<Text>();
        //}

        #endregion

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
        //oper = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        //oper.allowSceneActivation = false;
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
        Messenger<bool>.Broadcast(GameEvent.PAUSE, true);
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
        Messenger<float>.Broadcast(GameEvent.MUSIC_CHANGED, musicVolumeSlider.value);
    }
    public void OnSoundsValueChanged()
    {
        PlayerPrefs.SetFloat("Sounds", soundsVolumeSlider.value);
        Messenger<float>.Broadcast(GameEvent.SOUNDS_CHANGED, soundsVolumeSlider.value);
    }
    public void OnVoiceValueChanged()
    {
        PlayerPrefs.SetFloat("Voices", voiceVolumeSlider.value);
        Messenger<float>.Broadcast(GameEvent.VOICE_CHANGED, voiceVolumeSlider.value);
    }
    public void OnMouseValueChanged()
    {
        Messenger<float>.Broadcast(GameEvent.MOUSE_CHANGED, mouseSlider.value);
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
        Invoke("ReturnHitMarker", 0.5f);
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
        bool inMenu = !settingsPanel.activeSelf;
        settingsPanel.SetActive(inMenu);
        Cursor.visible = inMenu;
        Cursor.lockState = inMenu ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale = inMenu ? 0 : 1;
        Messenger<bool>.Broadcast(GameEvent.PAUSE, inMenu);
    }
    public void Restart()
    {
        OnExit();
        Invoke("RestartScene", 1);
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
        Messenger.Broadcast(GameEvent.EXIT_LEVEL);

        //settingsPanel.SetActive(false);
        //settingsPanel.SetActive(false);
        //hitMarker.SetActive(false);
        //jumpBonusSlider.value = speedBonusSlider.value = damageBonusSlider.value = invilnvurableBonusSlider.value = 0;
        //jumpBonusSlider.gameObject.SetActive(false);
        //speedBonusSlider.gameObject.SetActive(false);
        //damageBonusSlider.gameObject.SetActive(false);
        //invilnvurableBonusSlider.gameObject.SetActive(false);
        
      //  Messenger.Broadcast(GameEvent.EXIT_LEVEL);
    }

    private void CheckSliders()
    {
        if(jumpBonusSlider.gameObject.activeSelf)
        {
            if(jumpBonusSlider.value - Time.deltaTime <= 0)
            {
                jumpBonusSlider.value = 0;
                Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_JUMP, 1);
            }
            else
            {
                jumpBonusSlider.value -= Time.deltaTime;
            }
        }
        if (speedBonusSlider.gameObject.activeSelf)
        {
            if (speedBonusSlider.value - Time.deltaTime <= 0)
            {
                speedBonusSlider.value = 0;
                Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_SPEED, 1);
            }
            else
            {
                speedBonusSlider.value -= Time.deltaTime;
            }
        }
        if (damageBonusSlider.gameObject.activeSelf)
        {
            if (damageBonusSlider.value - Time.deltaTime <= 0)
            {
                damageBonusSlider.value = 0;
                Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_DAMAGE, 1);
            }
            else
            {
                damageBonusSlider.value -= Time.deltaTime;
            }
        }
        if (invilnvurableBonusSlider.gameObject.activeSelf)
        {
            if (invilnvurableBonusSlider.value - Time.deltaTime <= 0)
            {
                invilnvurableBonusSlider.value = 0;
                Messenger<int>.Broadcast(GameEvent.TAKE_BONUS_INVULNERABLE, 1);
            }
            else
            {
                invilnvurableBonusSlider.value -= Time.deltaTime;
            }
        }
    }

    private void ReturnHitMarker()
    {
        hitMarker.SetActive(false);
    }
    private void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
