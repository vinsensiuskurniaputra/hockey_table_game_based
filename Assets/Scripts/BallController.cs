using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement; // Tambahkan ini untuk menggunakan SceneManager

public class BallController : MonoBehaviour
{
    public float waktuPermainan = 60f;
    TextMeshProUGUI teksTimer;

    private float sisaWaktu;
    private bool permainanBerjalan = true;
    private bool isPaused = false;

    int scoreP1;
    int scoreP2;
    AudioSource audio;
    public AudioClip hitSound;
    public AudioClip losingSfx;
    public AudioClip winningSfx; // Tambahkan variable untuk winning sound effect

    public int force;
    Rigidbody2D rigid;
    TextMeshProUGUI scoreUIP1;
    TextMeshProUGUI scoreUIP2;

    GameObject panelSelesai;
    GameObject panelPause; // Panel untuk menu pause
    TextMeshProUGUI txPemenang;

    // Tambahkan referensi ke AudioSource musik latar
    private AudioSource bgmAudioSource;
    [Range(0.0f, 1.0f)]
    public float gameOverMusicVolume = 0.2f; // Volume musik saat game over (0.0-1.0)
    [Range(0.0f, 1.0f)]
    public float losingSfxMusicVolume = 0.05f; // Volume musik saat losing sfx dimainkan (hampir mute)
    [Range(0.0f, 1.0f)]
    public float losingSfxVolume = 1.0f;     // Volume SFX saat kalah (0.0-1.0)
    
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
        
        // Mencari dan menyembunyikan panel pause
        panelPause = GameObject.Find("PanelPause");
        if (panelPause != null)
            panelPause.SetActive(false);
        else
            Debug.LogError("PanelPause tidak ditemukan! Buat panel UI dengan nama 'PanelPause'");

        sisaWaktu = waktuPermainan;

        // Temukan BGM AudioSource (asumsi ini ada di GameObject bernama "BGM" atau pada Camera)
        bgmAudioSource = GameObject.Find("BGM")?.GetComponent<AudioSource>();
        if (bgmAudioSource == null)
            bgmAudioSource = Camera.main?.GetComponent<AudioSource>();
            
        if (bgmAudioSource == null)
            Debug.LogWarning("BGM AudioSource tidak ditemukan. Pastikan ada GameObject dengan nama 'BGM' yang memiliki AudioSource atau AudioSource ada di Main Camera.");
    }
    
    // Update is called once per frame 
    void Update()
    {
        // Deteksi input ESC untuk pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
        
        if (!permainanBerjalan || isPaused) return;

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

    // Fungsi untuk pause game
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0; // Menghentikan waktu permainan
        if (panelPause != null)
            panelPause.SetActive(true);
    }
    
    // Fungsi untuk melanjutkan game
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // Mengembalikan waktu permainan
        if (panelPause != null)
            panelPause.SetActive(false);
    }
    
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (isPaused) return; // Jangan proses collision saat pause
        
        audio.PlayOneShot(hitSound);
        if (coll.gameObject.name == "TepiKanan")
        {
            scoreP1 += 1;
            TampilkanScore();
            if (scoreP1 == 5)
            {
                // Pause game saat mencapai score 5
                Time.timeScale = 0;
                permainanBerjalan = false;
                
                panelSelesai.SetActive(true);
                txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
                txPemenang.text = "Tim Merah Pemenang!";
                
                // Tambahkan winning sound effect dan volume adjustment
                if (winningSfx != null)
                {
                    Debug.Log("Playing winning sound");
                    
                    // Set volume untuk winning SFX
                    audio.volume = losingSfxVolume; // Bisa gunakan volume yang sama
                    audio.PlayOneShot(winningSfx);
                    
                    // Kecilkan musik utama saat winning sfx dimainkan
                    if (bgmAudioSource != null)
                    {
                        StartCoroutine(FadeMusicVolume(bgmAudioSource.volume, losingSfxMusicVolume, 0.5f));
                    }
                    
                    // Delay penghancuran objek
                    StartCoroutine(DestroyAfterDelay(6f));
                }
                else
                {
                    // Kecilkan musik saat game over biasa
                    AdjustMusicVolumeForGameOver();
                    Destroy(gameObject);
                }
                
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
                // Pause game saat mencapai score 5
                Time.timeScale = 0;
                permainanBerjalan = false;
                
                panelSelesai.SetActive(true);
                txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
                txPemenang.text = "Tim Biru Pemenang!";
                if (losingSfx != null) {
                    Debug.Log("Playing losing sound");
                    
                    // Suarakan efek suara kalah dengan volume besar
                    audio.volume = losingSfxVolume;
                    audio.PlayOneShot(losingSfx);
                    
                    // Kecilkan musik utama drastis saat losing sfx dimainkan
                    if (bgmAudioSource != null)
                    {
                        StartCoroutine(FadeMusicVolume(bgmAudioSource.volume, losingSfxMusicVolume, 0.5f));
                    }
                    
                    // Mulai coroutine untuk delay 6 detik
                    StartCoroutine(DestroyAfterDelay(6f));
                } else {
                    // Kecilkan musik saat game over biasa
                    AdjustMusicVolumeForGameOver();
                    Debug.Log("No losing sound clip assigned");
                    Destroy(gameObject);
                }
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
    
    // Coroutine untuk delay penghancuran objek
    IEnumerator DestroyAfterDelay(float delay)
    {
        // Gunakan WaitForSecondsRealtime bukan WaitForSeconds karena Time.timeScale = 0
        yield return new WaitForSecondsRealtime(delay);
        Destroy(gameObject);
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
        // Pause game saat panel selesai muncul
        Time.timeScale = 0;
        permainanBerjalan = false;
        
        if (scoreP1 > scoreP2)
        {
            panelSelesai.SetActive(true);
            txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
            txPemenang.text = "Tim Merah Pemenang!";
            
            // Tambahkan winning sound effect dan volume adjustment
            if (winningSfx != null)
            {
                // Set volume untuk winning SFX
                audio.volume = losingSfxVolume;
                audio.PlayOneShot(winningSfx);
                
                // Kecilkan musik utama saat winning sfx dimainkan
                if (bgmAudioSource != null)
                {
                    StartCoroutine(FadeMusicVolume(bgmAudioSource.volume, losingSfxMusicVolume, 0.5f));
                }
                
                // Delay penghancuran objek
                StartCoroutine(DestroyAfterDelay(6f));
            }
            else
            {
                // Kecilkan musik saat game over biasa
                AdjustMusicVolumeForGameOver();
                Destroy(gameObject);
            }
            
            return;
        }

        if (scoreP2 > scoreP1)
        {
            panelSelesai.SetActive(true);
            txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
            txPemenang.text = "Tim Biru Pemenang!";
            if (losingSfx != null)
            {
                // Atur volume efek suara kalah menjadi besar
                audio.volume = losingSfxVolume;
                audio.PlayOneShot(losingSfx);
                
                // Kecilkan musik utama drastis saat losing sfx dimainkan
                if (bgmAudioSource != null)
                {
                    StartCoroutine(FadeMusicVolume(bgmAudioSource.volume, losingSfxMusicVolume, 0.5f));
                }
                
                // Mulai coroutine untuk delay 6 detik
                StartCoroutine(DestroyAfterDelay(6f));
            }
            else
            {
                // Kecilkan musik saat game over biasa
                AdjustMusicVolumeForGameOver();
                Destroy(gameObject);
            }
            return;
        }

        if (scoreP2 == scoreP1)
        {
            panelSelesai.SetActive(true);
            txPemenang = GameObject.Find("Pemenang").GetComponent<TextMeshProUGUI>();
            txPemenang.text = "Draw!";
            // Kecilkan musik saat game over biasa
            AdjustMusicVolumeForGameOver();
            Destroy(gameObject);
            return;
        }
    }

    // Tambahkan fungsi untuk menyesuaikan volume musik
    void AdjustMusicVolumeForGameOver()
    {
        if (bgmAudioSource != null)
        {
            // Simpan volume awal
            float originalVolume = bgmAudioSource.volume;
            
            // Kurangi volume musik latar secara bertahap
            StartCoroutine(FadeMusicVolume(originalVolume, gameOverMusicVolume, 1.0f));
        }
        else
        {
            Debug.LogWarning("BGM AudioSource tidak ditemukan, tidak dapat menyesuaikan volume");
        }
    }
    
    // Coroutine untuk fade out volume musik
    IEnumerator FadeMusicVolume(float startVolume, float targetVolume, float duration)
    {
        float timeElapsed = 0;
        
        while (timeElapsed < duration)
        {
            bgmAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / duration);
            timeElapsed += Time.unscaledDeltaTime; // Gunakan unscaledDeltaTime karena Time.timeScale = 0
            yield return null;
        }
        
        bgmAudioSource.volume = targetVolume;
    }

    // Tambahkan fungsi untuk restart game
    public void RestartGame()
    {
        // Reset time scale ke normal
        Time.timeScale = 1;
        
        // Restart scene yang aktif saat ini
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
