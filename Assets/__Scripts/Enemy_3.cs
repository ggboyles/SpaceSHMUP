using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_3 : Enemy
{
    [Header("Enemy_3 Inscribed Fields")]
    public float lifeTime = 5;
    public Vector2 midpointYRange = new Vector2(1.5f, 3);
    [Tooltip("If true, the Bezier points & paths are drawn in the Scene pane")]
    public bool drawDebugInfo = true;
    [Header("Enemy_3 Private Fields")]
    [SerializeField]
    private Vector3[] points; // the three points for the Bezier curve
    [SerializeField]
    private float birthTime;

    void Start()
    {
        points = new Vector3[3]; // initialize points 

        // start position already been set by Main.SpawnEnemy()
        points[0] = pos;

        // set xMin and xMax the same way Main.SpawnEnemy() does
        float xMin = -bndCheck.camWidth + bndCheck.radius;
        float xMax = bndCheck.camWidth - bndCheck.radius;

        // pick a random middle position in the bottom half of the screen
        points[1] = Vector3.zero;
        points[1].x = Random.Range(xMin, xMax);
        float midYMult = Random.Range(midpointYRange[0], midpointYRange[1]);
        points[1].y = -bndCheck.camHeight * midYMult;

        // pick a random final position above the top of the screen
        points[2] = Vector3.zero;
        points[2].y = pos.y;
        points[2].x = Random.Range(xMin, xMax);

        // set the birthTime to the current time
        birthTime = Time.time;

        if(drawDebugInfo) DrawDebug();
    }

    public override void Move()
    {
        // bezier curves work based on a u value between 0 and 1
        float u = (Time.time - birthTime) / lifeTime;

        if(u > 1)
        {
            Destroy(this.gameObject); // enemy_3 has finished its life
            return;
        }

        transform.rotation = Quaternion.Euler(u * 180, 0, 0);

        // interpolate the three Bezier curve points
        u = u - 0.1f * Mathf.Sin(u * Mathf.PI * 2);
        pos = Utlis.Bezier(u, points);
    }

    void DrawDebug()
    {
        // draw the three points
        Debug.DrawLine(points[0], points[1], Color.cyan, lifeTime);
        Debug.DrawLine(points[1], points[2], Color.yellow, lifeTime);

        // draw the Bezier curve
        float numSections = 20;
        Vector3 prevPoint = points[0];
        Color col;
        Vector3 pt;
        for(int i = 0; i < numSections; i++)
        {
            float u = i / numSections;
            pt = Utlis.Bezier(u, points);
            col = Color.Lerp(Color.cyan, Color.yellow, u);
            Debug.DrawLine(prevPoint, pt, col, lifeTime);
            prevPoint = pt;
        }
    }
}
