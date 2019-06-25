using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip blueSkill;
    public AudioClip blueSkillEnd;
    public AudioClip redSkill;
    public AudioClip blackSkill;
    public AudioClip yellowSkill;
    public AudioClip whiteSkill;

    public AudioClip bigSkeletonDefeat;
    public AudioClip skeletonDefeat;
    public AudioClip ghostDefeat;
    public AudioClip batDefeat;

    public AudioClip matchThree;
    public AudioClip takeDamage;
    public AudioClip slide;

    private float defaultPitch = 1f;
    private Board board;

    public AudioSource NormalAudioSource;
    public AudioSource MatchAudioSource;
    public AudioSource EnemyAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

        blueSkill = Resources.Load<AudioClip>("Time_Freeze");
        blueSkillEnd = Resources.Load<AudioClip>("Ice_Shatter");
        redSkill = Resources.Load<AudioClip>("Comet_Storm");
        blackSkill = Resources.Load<AudioClip>("Multiply_Wounds");
        yellowSkill = Resources.Load<AudioClip>("Invigorate");
        whiteSkill = Resources.Load<AudioClip>("Restore");

        bigSkeletonDefeat = Resources.Load<AudioClip>("Big_Skeleton_Defeated");
        skeletonDefeat = Resources.Load<AudioClip>("Skeleton_Defeated");
        ghostDefeat = Resources.Load<AudioClip>("Ghost_Defeated");
        batDefeat = Resources.Load<AudioClip>("Bat_Defeated");

        matchThree = Resources.Load<AudioClip>("Match");
        takeDamage = Resources.Load<AudioClip>("Attack_Damage");
        slide = Resources.Load<AudioClip>("Slide");

        AudioSource[] audioSources = GetComponents<AudioSource>();
        NormalAudioSource = audioSources[0];
        MatchAudioSource = audioSources[1];
        EnemyAudioSource = audioSources[2];
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlaySound(string clip)
    {
        switch (clip)
        {
            case "Time_Freeze":
                NormalAudioSource.PlayOneShot(blueSkill);
                break;
            case "Ice_Shatter":
                NormalAudioSource.PlayOneShot(blueSkillEnd);
                break;
            case "Comet_Storm":
                NormalAudioSource.PlayOneShot(redSkill);
                break;
            case "Multiply_Wounds":
                NormalAudioSource.PlayOneShot(blackSkill);
                break;
            case "Invigorate":
                NormalAudioSource.PlayOneShot(yellowSkill);
                break;
            case "Restore":
                NormalAudioSource.PlayOneShot(whiteSkill);
                break;
            case "Big_Skeleton_Defeated":
                EnemyAudioSource.PlayOneShot(bigSkeletonDefeat);
                break;
            case "Skeleton_Defeated":
                EnemyAudioSource.PlayOneShot(skeletonDefeat);
                break;
            case "Ghost_Defeated":
                EnemyAudioSource.PlayOneShot(ghostDefeat);
                break;
            case "Bat_Defeated":
                EnemyAudioSource.PlayOneShot(batDefeat);
                break;
            case "Match":
                MatchAudioSource.pitch = defaultPitch + (float)board.multiplier / 10;
                MatchAudioSource.PlayOneShot(matchThree);
                break;
            case "Attack_Damage":
                NormalAudioSource.PlayOneShot(takeDamage);
                break;
            case "Slide":
                NormalAudioSource.PlayOneShot(slide);
                break;
        }
    }
}
