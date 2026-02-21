using UnityEngine;

public class PlayerVisualForms : MonoBehaviour
{
    [SerializeField] private GameObject[] forms;
    [SerializeField] private int currentFormIndex = 0;

    private void Awake()
    {
        SetForm(currentFormIndex);
    }

    public void SetForm(int index)
    {
        if (forms == null || forms.Length == 0) return;

        index = Mathf.Clamp(index, 0, forms.Length - 1);
        currentFormIndex = index;

        for (int i = 0; i < forms.Length; i++)
            if (forms[i] != null)
                forms[i].SetActive(i == currentFormIndex);
    }
    public Animator GetCurrentAnimator()
    {
        if (forms == null) return null;

        for (int i = 0; i < forms.Length; i++)
        {
            if (forms[i] != null && forms[i].activeInHierarchy)
                return forms[i].GetComponentInChildren<Animator>();
        }

        return null;
    }
}