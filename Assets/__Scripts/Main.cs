using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static private Main S;

    [Header("Inscribed")]
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2;

    private BoundsCheck bndCheck;

    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();

        // invokes SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
    }

    public void SpawnEnemy()
    {
        // picks a random enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // position enemy above the screen with random x position
        float enemyInset = enemyInsetDefault;
        if(go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        // set the initial position for spawned enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // invoke SpawnEnemy() again
        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
    }

    void DelayedRestart()
    {
        // invokes the Restart() method in gameRestartDelay seconds
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }
}
