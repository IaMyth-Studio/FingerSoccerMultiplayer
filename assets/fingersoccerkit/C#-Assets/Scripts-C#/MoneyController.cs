using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MoneyController : MonoBehaviour
{
	//**************************************************************************//
	// Main CoinPack Purchase Controller.
	// Este script gestiona todos los eventos de toque en los paquetes de monedas.
	// Puedes integrar fácilmente tu propio sistema de IAB personalizado para ofrecer
	// opciones de IAP atractivas para el jugador.
	//**************************************************************************//

	private float buttonAnimationSpeed = 9;   // Velocidad del efecto de animación al tocar un botón
	private bool canTap = true;               // Bandera para prevenir toques dobles
	public AudioClip coinsCheckout;           // Sonido para una compra exitosa
	public AudioClip tapSfx;                  // Sonido para una acción de toque

	// Referencias a GameObjects
	public GameObject playerMoney;            // Objeto de texto 3D de la UI para mostrar el dinero del jugador
	private int availableMoney;               // Cantidad de dinero disponible para el jugador

	public GameObject[] eng_text = new GameObject[10];  // GameObjects de texto en inglés
	public GameObject[] arab_text = new GameObject[10]; // GameObjects de texto en árabe

	//*****************************************************************************
	// Init. Actualiza los textos 3D con valores guardados obtenidos de PlayerPrefs.
	//*****************************************************************************
	void Awake()
	{
		availableMoney = PlayerPrefs.GetInt("PlayerMoney");
		playerMoney.GetComponent<TextMesh>().text = "" + availableMoney;
	}

	//-------------------------------------------------------------------

	//*****************************************************************************
	// FSM (Finite State Machine)
	//*****************************************************************************
	void Update()
	{
		HandleLanguage();
		if (canTap)
		{
			StartCoroutine(TapManager());
		}
	}

	//*****************************************************************************
	// Manejo de opciones de idioma
	//*****************************************************************************
	void HandleLanguage()
	{
		if (pubR.language_option == 0)
		{
			for (int i = 0; i < eng_text.Length; i++)
			{
				eng_text[i].SetActive(true);
				arab_text[i].SetActive(false);
			}
		}
		else
		{
			for (int i = 0; i < eng_text.Length; i++)
			{
				arab_text[i].SetActive(true);
				eng_text[i].SetActive(false);
			}
		}
	}

	//*****************************************************************************
	// Monitorea los toques del jugador en los botones del menú.
	// Detecta tanto toques como clics y puede ser usado con el editor, dispositivos portátiles y 
	// otras plataformas.
	//*****************************************************************************
	private RaycastHit hitInfo;
	private Ray ray;

	IEnumerator TapManager()
	{
		if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)
			ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
		else if (Input.GetMouseButtonUp(0))
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		else
			yield break;

		if (Physics.Raycast(ray, out hitInfo))
		{
			GameObject objectHit = hitInfo.transform.gameObject;
			switch (objectHit.name)
			{
			case "coinPack_1":
				StartCoroutine(AnimateButton(objectHit));
				// BuyProduct("com.200coins"); // Eliminado
				playSfx(coinsCheckout);
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				yield return new WaitForSeconds(1.5f);
				break;

			case "coinPack_2":
				StartCoroutine(AnimateButton(objectHit));
				// BuyProduct("com.500coins"); // Eliminado
				playSfx(coinsCheckout);
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				yield return new WaitForSeconds(1.5f);
				break;

			case "coinPack_3":
				StartCoroutine(AnimateButton(objectHit));
				// BuyProduct("com.2500coins"); // Eliminado
				playSfx(coinsCheckout);
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				yield return new WaitForSeconds(1.5f);
				break;

			case "Btn-Back":
				StartCoroutine(AnimateButton(objectHit));
				playSfx(tapSfx);
				yield return new WaitForSeconds(1.0f);
				SceneManager.LoadScene("Scene-Menu");
				break;
			}
		}
	}

	//*****************************************************************************
	// Animar botón escalando hacia arriba/abajo.
	//*****************************************************************************
	IEnumerator AnimateButton(GameObject _btn)
	{
		canTap = false;

		Vector3 startingScale = _btn.transform.localScale;
		Vector3 destinationScale = startingScale * 1.1f;

		float t = 0.0f;
		while (t <= 1.0f)
		{
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = Vector3.Lerp(startingScale, destinationScale, Mathf.SmoothStep(0.0f, 1.0f, t));
			yield return null;
		}

		t = 0.0f;
		while (t <= 1.0f)
		{
			t += Time.deltaTime * buttonAnimationSpeed;
			_btn.transform.localScale = Vector3.Lerp(destinationScale, startingScale, Mathf.SmoothStep(0.0f, 1.0f, t));
			yield return null;
		}

		canTap = true;
	}

	//*****************************************************************************
	// Reproducir sonido
	//*****************************************************************************
	void playSfx(AudioClip _clip)
	{
		AudioSource audioSource = GetComponent<AudioSource>();
		if (audioSource != null)
		{
			audioSource.clip = _clip;
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}
		else
		{
			Debug.LogWarning("AudioSource no encontrado en el GameObject.");
		}
	}
}
