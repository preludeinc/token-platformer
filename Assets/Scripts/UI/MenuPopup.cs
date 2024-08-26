using UnityEngine;

public class MenuPopup : BasePopup
{
    [SerializeField] private UIController uiController;
    override public void Open()
    {
        base.Open();
    }

    public void OnExitGameButton()
    {
        Debug.Log("exit game");
        Application.Quit();
    }

    public void OnReturnToGameButton()
    {
        Debug.Log("return to game");
        base.Close();
    }
}