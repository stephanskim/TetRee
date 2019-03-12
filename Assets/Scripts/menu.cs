using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour {

	public void playAgain() {
        SceneManager.LoadScene("lvlselect");
    }

    public void playLvl(int lvl) {
        
        SceneManager.LoadScene("level");
        Game.currentLevel = lvl;
    }

    public void controls() {
        SceneManager.LoadScene("Controls");
    }

    public void quit() {
        Application.Quit();
    }

    public void credits() {
        SceneManager.LoadScene("Credits");
    }

    public void startScreen() {
        SceneManager.LoadScene("Start");
    }

    public void selectMusic(int y) {
        Game.songChoice = y;
    }
}
