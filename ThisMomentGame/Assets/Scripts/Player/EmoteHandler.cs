using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EmoteEnum { Red, Green, Blue, Yellow }

public class EmoteHandler : MonoBehaviour
{
    PlayerActions.EmotesActions emoteControls;

    [SerializeField] Animator emoteAnimator;

    public GameObject emoteTarget;

    // Start is called before the first frame update
    void Start()
    {
        emoteControls = GetComponent<PlayerMovement>().actions.Emotes;

        emoteControls.Red.performed += ctx => DoEmote(EmoteEnum.Red);
        emoteControls.Green.performed += ctx => DoEmote(EmoteEnum.Green);
        emoteControls.Blue.performed += ctx => DoEmote(EmoteEnum.Blue);
        emoteControls.Yellow.performed += ctx => DoEmote(EmoteEnum.Yellow);
    }

    void DoEmote(EmoteEnum emote)
    {
        

        //SetAnimValue("Red", false);
        //SetAnimValue("Green", false);
        //SetAnimValue("Blue", false);
        //SetAnimValue("Yellow", false);

        switch (emote)
        {
            case EmoteEnum.Red:
                emoteAnimator.SetTrigger("Red");
                break;
            case EmoteEnum.Green:
                emoteAnimator.SetTrigger("Green");
                break;
            case EmoteEnum.Blue:
                emoteAnimator.SetTrigger("Blue");
                break;
            case EmoteEnum.Yellow:
                emoteAnimator.SetTrigger("Yellow");
                break;
        }

        if(emoteTarget != null)
        {
            emoteTarget.GetComponent<ConnectionManager>().TryEmote(emote);
        }
    }

    public void FinishEmote()
    {
        SetAnimValue("Red", false);
        SetAnimValue("Green", false);
        SetAnimValue("Blue", false);
        SetAnimValue("Yellow", false);
    }

    void SetAnimValue(string name, bool value)
    {


        emoteAnimator.SetBool(name, value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
