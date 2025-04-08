using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PaddleController : MonoBehaviour
{
    [Header("General Settings")]
    public float batasAtas;
    public float batasBawah;
    public float batasTengah;
    public float batasBelakang;
    public float kecepatan;

    [Header("Input Settings")]
    public string horizontal;
    public string vertical;

    void Update()
    {
        float gerakX = Input.GetAxis(horizontal) * kecepatan * Time.deltaTime;
        float gerakY = Input.GetAxis(vertical) * kecepatan * Time.deltaTime;

        Vector2 nextPos = new Vector2(transform.position.x + gerakX, transform.position.y + gerakY);

        if (nextPos.x < Mathf.Min(batasTengah, batasBelakang) || nextPos.x > Mathf.Max(batasTengah, batasBelakang))
        {
            gerakX = 0;
        }
        if (nextPos.y < batasBawah || nextPos.y > batasAtas)
        {
            gerakY = 0;
        }
        transform.Translate(gerakX, gerakY, 0);
    }
}
