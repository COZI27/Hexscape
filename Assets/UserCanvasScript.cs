using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserCanvasScript : MonoBehaviour
{ 
    [SerializeField]
    private GameObject responseObject;
    private Animator responseAnimator;
    [SerializeField]
    private Text placeholderText;

    [SerializeField]
    private GameObject inputNameObject;
    private Animator inputNameAnimator;

    [SerializeField]
    private GameObject inputPassObject;
    private Animator inputPassAnimator;


    // Start is called before the first frame update
    void Start()
    {
        if (responseObject != null)
        {
            responseAnimator = responseObject.GetComponent<Animator>();
            //placeholderText = responseObject.transform.GetChild(0).Find("Placeholder").GetComponent<Text>();
        }
        if (inputNameObject != null) inputNameAnimator = inputNameObject.GetComponent<Animator>();
        if (inputPassObject != null) inputPassAnimator = inputPassObject.GetComponent<Animator>();
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DisiplayResponseMessage(string message, float duration = 3.0f)
    {
        StartCoroutine(DisplayResponseMessage(message, duration));
    }

    public void HideResponseField()
    {
        responseAnimator.SetTrigger("Hide");
    }

    public void HideInputFields()
    {
        StartCoroutine(HideFields());
    }

    public void ShowInputFields()
    {
        StartCoroutine(ShowFields());
    }


    private IEnumerator DisplayResponseMessage(string message, float duration = 3.0f)
    {
        if (responseObject != null)
        { 
       
            string previousMessage = placeholderText.text;
            placeholderText.text = message;


            responseAnimator.SetBool("doFlash", true);

            yield return new WaitForSeconds(duration);

            responseAnimator.SetBool("doFlash", false);

            responseAnimator.SetTrigger("Show");

            placeholderText.text = previousMessage;
        }
    }

    private IEnumerator HideFields()
    {
        inputNameAnimator.SetTrigger("Hide");
        inputPassAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(0.1f);
    }

    private IEnumerator ShowFields()
    {
        inputNameAnimator.SetTrigger("Show");
        inputPassAnimator.SetTrigger("Show");
        yield return new WaitForSeconds(0.1f);
    }



}
