using UnityEngine;

public class PaddleAi : MonoBehaviour
{
    public Transform bola;
    public float kecepatan = 5f;

    public float batasAtas = 3.45f;
    public float batasBawah = -3.45f;
    public float batasKiri = 0f;
    public float batasKanan = 7.5f;

    void Update()
    {
        if (bola == null) return;

        Rigidbody2D bolaRb = bola.GetComponent<Rigidbody2D>();

        Vector2 target = bola.position;
        Vector2 current = transform.position;

        if (bolaRb != null && bolaRb.linearVelocity.x < 0) return;
        float gerakY = 0f;
        float gerakX = 0f;

        if (Mathf.Abs(target.y - current.y) > 0.1f)
        {
            gerakY = Mathf.Sign(target.y - current.y) * kecepatan * Time.deltaTime;
        }

        if (Mathf.Abs(target.x - current.x) > 0.1f)
        {
            gerakX = Mathf.Sign(target.x - current.x) * kecepatan * Time.deltaTime;
        }

        float nextY = Mathf.Clamp(current.y + gerakY, batasBawah, batasAtas);
        float nextX = Mathf.Clamp(current.x + gerakX, batasKiri, batasKanan);

        transform.position = new Vector2(nextX, nextY);
    }
}
