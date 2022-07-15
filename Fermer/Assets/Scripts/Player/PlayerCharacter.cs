using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerCharacter : AliveController
{
    [SerializeField] private List<AudioClip> damageClips;
    [SerializeField] private AudioClip sprintClip;

    private AudioSource source;
    private bool opportunityToDead;

    private void Start()
    {
        GameController.TAKE_BONUS_INVULNERABLE.AddListener(OnTakeBonusInvulnerable);
        GameController.SPRINT_ACTION.AddListener(PlaySprintSound);
        GameController.START_FINAL_LOADING.AddListener(SetUpToFinalLoading);
        opportunityToDead = true;
        source = GetComponent<AudioSource>();
        Setup();
    }

    public void Setup()
    {
        Health = maxHealth;
        GameController.CHANGE_HEALTH.Invoke(maxHealth);
    }

    public override void GetDamage(int damage)
    {
        if(PlayerBonusStat.bonusPack[BonusType.Invulnerable]==1 && opportunityToDead)
        {
            source.PlayOneShot(damageClips[Random.Range(0, damageClips.Count)]);
            base.GetDamage(damage);
            GameController.CHANGE_HEALTH.Invoke(Health);
        }
    }
    public void RestoreHealth(int hp)
    {
        Health += hp;
        GameController.CHANGE_HEALTH.Invoke(Health);
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

        GameController.DAMAGE_MARKER_ACTIVATE.Invoke(result);
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
        GameController.PLAYER_DEAD.Invoke();
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
