using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;

    public event Action OnShowDialog;
    public event Action OnHideDialog;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    Dialog dialog;
    int currentLine = 0;
    bool isTyping;
    Coroutine typingCoroutine;
    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        
        this.dialog = dialog;
        dialogBox.SetActive(true);
        typingCoroutine = StartCoroutine(TypeDialog(dialog.Lines[0]));
    }
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                // Nếu đang gõ, kết thúc quá trình gõ và hiển thị đầy đủ dòng
                StopCoroutine(typingCoroutine);
                dialogText.text = dialog.Lines[currentLine];
                isTyping = false;
            }
            else
            {
                // Chuyển sang dòng tiếp theo
                ++currentLine;
                if (currentLine < dialog.Lines.Count)
                {
                    typingCoroutine = StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
                }
                else
                {
                    dialogBox.SetActive(false);
                    currentLine = 0;
                    OnHideDialog?.Invoke();
                }
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        isTyping = false;
    }
}
