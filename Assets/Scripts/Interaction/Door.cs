using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Door : Interactable
{
    public RequirementList openRequirements;

    public bool unlocked
    {
        get
        {
            return openRequirements == null || openRequirements.completed;
        }
    }

    //UnityEvents triggering chain events
    public UnityEvent<bool> OnOpen;

    public UnityEvent<bool> OnUnlock;

    [SerializeField]
    private bool open;

    //Reference to animator
    public Animator[] animators;

    //Start is called before first update.
    private void Start()
    {
        openRequirements.Start();

        openRequirements.onCompleted += UpdateLocked;

        //SetOpen(open);
        UpdateLocked (unlocked);
    }

    //Called by UnityEvents to chage hover info
    public void SetInfo(string info)
    {
        hoverInfoText = info;
    }

    //Attempt to toggle open/close if not locked
    public override InteractionInfo Interact(Player player)
    {
        Task incompleteTask = openRequirements.GetIncompleteTask();

        if (!unlocked) return InteractionInfo.Fail(incompleteTask.description);

        SetOpen(!open);

        return InteractionInfo.Success();
    }

    //Main opening/closing door function
    public void SetOpen(bool open)
    {
        this.open = open;
        foreach (Animator animator in animators)
        {
            animator.SetBool("open", open);
        }

        if (OnOpen != null) OnOpen.Invoke(open);
    }

    //Call UnLock event when all requirements are met
    private void UpdateLocked(bool unlocked)
    {
        if (OnUnlock != null) OnUnlock.Invoke(unlocked);
		
		if(unlocked) outline.SetColorMode(Outline.ColorMode.Normal);
		else outline.SetColorMode(Outline.ColorMode.Problem);
    }

    //Allow for unityEvents to set task completed
    public void CompleteOpenRequirement(string name, bool complete)
    {
        openRequirements.SetTaskCompleted (name, complete);
    }

    public string autoCompleteTaskName;

    public void CompleteOpenRequirement(bool complete)
    {
        openRequirements.SetTaskCompleted (autoCompleteTaskName, complete);
    }

    public void CompleteOpenRequirement(string name)
    {
        openRequirements.SetTaskCompleted(name, true);
    }

    public void OpenAfter(float time)
    {
        StartCoroutine(SetOpenAfter(time, true));
    }

    private IEnumerator SetOpenAfter(float time, bool open)
    {
        yield return new WaitForSeconds(time);
        SetOpen (open);
    }
}
