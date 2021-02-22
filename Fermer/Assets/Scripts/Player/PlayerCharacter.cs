using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerCharacter : AliveController
{
    [SerializeField] private List<AudioClip> damageClips;
    [SerializeField] private AudioClip sprintClip;

    private AudioSource source;
    private bool opportunityToDead;

    private void Awake()
    {
        Messenger<int>.AddListener(GameEvent.TAKE_BONUS_INVULNERABLE, OnTakeBonusInvulnerable);
        Messenger.AddListener(GameEvent.SPRINT_ACTION, PlaySprintSound);
        Messenger.AddListener(GameEvent.START_FINAL_LOADING, SetUpToFinalLoading);
        opportunityToDead = true;
        //   Messenger.AddListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }
    private void OnDestroy()
    {
        Messenger<int>.RemoveListener(GameEvent.TAKE_BONUS_INVULNERABLE, OnTakeBonusInvulnerable);
        Messenger.RemoveListener(GameEvent.SPRINT_ACTION, PlaySprintSound);
        Messenger.RemoveListener(GameEvent.START_FINAL_LOADING, SetUpToFinalLoading);
        //    Messenger.AddListener(GameEvent.EXIT_LEVEL, OnDestroy);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        source = GetComponent<AudioSource>();
        Setup();
      //  Messenger<float>.Broadcast(GameEvent.CHANGE_MAX_HEALTH, maxHealth);
    }

    public void Setup()
    {
        Health = maxHealth;
        Messenger<float>.Broadcast(GameEvent.CHANGE_HEALTH, maxHealth);
    }

    public override void GetDamage(int damage)
    {
        if(PlayerBonusStat.bonusPack[BonusType.Invulnerable]==1 && opportunityToDead)
        {
            source.PlayOneShot(damageClips[Random.Range(0, damageClips.Count)]);
            base.GetDamage(damage);
            Messenger<float>.Broadcast(GameEvent.CHANGE_HEALTH, Health);
        }
    }
    public void RestoreHealth(int hp)
    {
        Health += hp;
        Messenger<float>.Broadcast(GameEvent.CHANGE_HEALTH, Health);
    }
   
    public void OnTakeDamageFromDirection(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        dir.y = 0;
        dir = dir.normalized;
        Vector3 myForwardGlobal = transform.forward;
        myForwardGlobal.y = 0;
        float degrees = Mathf.Sign(Vector3.Cross(myForwardGlobal, dir).y) * Vector3.Angle(myForwardGlobal, dir);

        int result = 0;

        if (degrees > -22.5f && degrees <= 22.5f)
            result = 0;
        else if (degrees > 22.5 && degrees < 67.5f)
            result = 1;
        else if (degrees > 67.5f && degrees < 112.5f)
            result = 2;
        else if (degrees > 112.5f && degrees < 157.5)
            result = 3;
        else if (degrees < -157.5f || degrees >= 157.5f)
            result = 4;
        else if (degrees >= -157.5f && degrees < -112.5f)
            result = 5;
        else if (degrees >= -112.5f && degrees < -67.5f)
            result = 6;
        else if (degrees >= -67.5f && degrees <= -22.5f)
            result = 7;

        Messenger<int>.Broadcast(GameEvent.DAMAGE_MARKER_ACTIVATE, result);
    }

    private void OnTakeBonusInvulnerable(int value)
    {
        PlayerBonusStat.bonusPack[BonusType.Invulnerable] = value;
    }

    private void PlaySprintSound()
    {
        source.PlayOneShot(sprintClip);
    }

    public override void Death()
    {
        Messenger.Broadcast(GameEvent.PLAYER_DEAD);
    }

    private void SetUpToFinalLoading()
    {
        Destroy(gameObject, 7);
        opportunityToDead = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            ExplosionZone zone = other.GetComponent<ExplosionZone>();
            if (zone != null)
            {
                OnTakeDamageFromDirection(other.transform.position);
                GetDamage(zone.damage);
            }
        }
        else if (other.CompareTag("LizerSword"))
        {
            OnTakeDamageFromDirection(other.transform.position);
            GetDamage(10);
        }
        else if(other.CompareTag("Finish"))
        {
            other.GetComponent<SceneController>().OnPlayerEntered();
        }
        else if (other.CompareTag("ReplicPoint"))
        {
            other.GetComponent<ReplicPointScript>().PlayReplicas();
        }
        else if (other.CompareTag("Info"))
        {
            other.GetComponent<InfoPanel>().SetState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Info"))
        {
            other.GetComponent<InfoPanel>().SetState(false);
        }
    }
}
