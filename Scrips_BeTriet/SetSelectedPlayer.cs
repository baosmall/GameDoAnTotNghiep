using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseCharacter : MonoBehaviour
{
    public int selectedPlayerIndex = 0; // Chỉ số nhân vật được chọn

    public void SetSelectedPlayer(int playerIndex)
    {
        selectedPlayerIndex = playerIndex;
        PlayerPrefs.SetInt("SelectedPlayer", selectedPlayerIndex); // Lưu chỉ số nhân vật
        SceneManager.LoadScene("Scene3");
    }
}