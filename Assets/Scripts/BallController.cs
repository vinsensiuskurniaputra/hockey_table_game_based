using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallController : MonoBehaviour
{
    public float waktuPermainan = 60f;
    TextMeshProUGUI teksTimer;

    private float sisaWaktu;
    private bool permainanBerjalan = true;

    int scoreP1;
    int scoreP2;
    AudioSource audio;
    public AudioClip hitSound;

    public int force;
    Rigidbody2D rigid;
    TextMeshProUGUI scoreUIP1;
    TextMeshProUGUI scoreUIP2;

    GameObject panelSelesai;
    TextMeshProUGUI txPemenang;
    // Use this for initialization 
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        Vector2 arah = new Vector2(2, 0).normalized;
        rigid.AddForce(arah * force);
        scoreP1 = 0;
        scoreP2 = 0;
        scoreUIP1 = GameObject.Find("Score1").GetComponent<TextMeshProUGUI>();
        scoreUIP2 = GameObject.Find("Score2").GetComponent<TextMeshProUGUI>();
        teksTimer = GameObject.Find("TeksTimer").GetComponent<TextMeshProUGUI>();

        panelSelesai = GameObject.Find("PanelSelesai");
        panelSelesai.SetActive(false);

        sisaWaktu = waktuPermainan;
    }
    // Update is called once per frame 
    void Update()
    {
        if (!permainanBerjalan) return;

        sisaWaktu -= Time.deltaTime;
        if (sisaWaktu <= 0)
        {
            sisaWaktu = 0;
            permainanBerjalan = false;
            GameOver();
        }

        // Tampilkan waktu ke UI
        if (teksTimer != null)
        {
            teksTimer.text = Mathf.CeilToInt(sisaWaktu).ToString(); // Tampilkan angka bulat
        }
    }
    private void OnCollisionEnter2D(Collision2D coll)
    {
        audio.PlayOneShot(hitSound);
        if (coll.gameObject.name == "TepiKanan")
        {
            scoreP1 += 1;
            TampilkanScore();
            if (scoreP1 == 5)
            {
                panelSelesai.SetActive(true);
                txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
                txPemenang.text = "Player Biru Pemenang!";
                Destroy(gameObject);
                return;
            }
            ResetBall();
            Vector2 arah = new Vector2(2, 0).normalized;
            rigid.AddForce(arah * force);
        }
        if (coll.gameObject.name == "TepiKiri")
        {
            scoreP2 += 1;
            TampilkanScore();
            if(scoreP2 == 5) {
                panelSelesai.SetActive(true);
                txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
                txPemenang.text = "Player Hijau Pemenang!";
                Destroy(gameObject);
                return;
            }
            ResetBall();
            Vector2 arah = new Vector2(-2, 0).normalized;
            rigid.AddForce(arah * force);
        }
        if (coll.gameObject.name == "Pemukul1" || coll.gameObject.name == "Pemukul2")
        {
            float sudut = (transform.position.y - coll.transform.position.y) * 5f;
            Vector2 arah = new Vector2(rigid.linearVelocity.x, sudut).normalized;
            rigid.linearVelocity = new Vector2(0, 0);
            rigid.AddForce(arah * force * 2);
        }
    }
    void ResetBall()
    {
        transform.localPosition = new Vector2(0, 0);
        rigid.linearVelocity = new Vector2(0, 0);
    }
    void TampilkanScore()
    {
        Debug.Log("Score P1: " + scoreP1 + " Score P2: " + scoreP2);
        scoreUIP1.text = scoreP1 + "";
        scoreUIP2.text = scoreP2 + "";
    }

    void GameOver()
    {
        if (scoreP1 > scoreP2)
        {
            panelSelesai.SetActive(true);
            txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
            txPemenang.text = "Player Biru Pemenang!";
            Destroy(gameObject);
            return;
        }

        if (scoreP2 > scoreP1)
        {
            panelSelesai.SetActive(true);
            txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
            txPemenang.text = "Player Hijau Pemenang!";
            Destroy(gameObject);
            return;
        }

        if (scoreP2 == scoreP1)
        {
            panelSelesai.SetActive(true);
            txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
            txPemenang.text = "Draw!";
            Destroy(gameObject);
            return;
        }
    }
}
