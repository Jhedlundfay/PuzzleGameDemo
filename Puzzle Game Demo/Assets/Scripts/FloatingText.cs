using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Animator animator;
    private Text damageText;

    void Start()
    {
        //AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        //damageText = animator.GetComponent<Text>();
    }

    public void SetText(string text)
    {
        //damageText.text = text;
    }
}
