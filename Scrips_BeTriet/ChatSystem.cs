using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatSystem : NetworkBehaviour
{
    public TextMeshProUGUI TextMess;
    public TMP_InputField InputMess;
    public GameObject Gui;
    public override void Spawned()
    {
        // chay khi nhan vat spam
        //var id = Runner.LocalPlayer.PlayerId;
        //RpcNotify(id);
        TextMess = GameObject.Find("Text Mess").GetComponent<TextMeshProUGUI>();
        InputMess = GameObject.Find("Input Mess").GetComponent<TMP_InputField>();
        Gui = GameObject.Find("Gui");
        Gui.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(SendMess);
    }
    public void SendMess()
    {
        var message = InputMess.text;
        if (string.IsNullOrWhiteSpace(message)) return;
        var id = Runner.LocalPlayer.PlayerId;
        var text = $"player {id}: {message}";
        RpcChat(text);
        InputMess.text = "";
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcChat(string msg)
    {
        TextMess.text += msg + "\n";
    }
}
