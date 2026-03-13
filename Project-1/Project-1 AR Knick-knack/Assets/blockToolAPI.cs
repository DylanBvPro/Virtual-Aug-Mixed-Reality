using UnityEngine;

public class blockToolAPI : MonoBehaviour
{
	[Header("Camera / Lens")]
	[Tooltip("Assign your VR lens/camera transform. If empty, Camera.main will be used.")]
	[SerializeField] private Transform vrLens;
	[SerializeField] private bool useMainCameraIfLensMissing = true;

	[Header("Update Settings")]
	[SerializeField] private bool runEveryFrame = true;
	[SerializeField] private float checkInterval = 0.1f;
	[SerializeField] private bool logWhenSideChanges = true;

	[Header("Output (1=Front, 2=Right, 3=Back, 4=Left, 5=Top, 6=Bottom)")]
	[SerializeField] private int currentSideOutput = 0;

	[Header("Manual Control Targets")]
	[SerializeField] private WeatherAPI weatherAPI;
	[SerializeField] private TimeAPI timeAPI;
	[SerializeField] private dayNightAPI dayNightApi;

	private int lastSideOutput = -1;
	private float timer;
	private bool warnedMissingLens;

	private void Start()
	{
		AutoAssignTargets();
		UpdateSideOutput();
	}

	private void AutoAssignTargets()
	{
		if (weatherAPI == null)
		{
			weatherAPI = FindFirstObjectByType<WeatherAPI>();
		}

		if (timeAPI == null)
		{
			timeAPI = FindFirstObjectByType<TimeAPI>();
		}

		if (dayNightApi == null)
		{
			dayNightApi = FindFirstObjectByType<dayNightAPI>();
		}
	}

	private void Update()
	{
		if (runEveryFrame)
		{
			UpdateSideOutput();
			return;
		}

		timer += Time.deltaTime;
		if (timer >= checkInterval)
		{
			timer = 0f;
			UpdateSideOutput();
		}
	}

	public int GetCurrentSideOutput()
	{
		return currentSideOutput;
	}

	public void UpdateSideOutput()
	{
		int detectedSide = DetermineDominantSide();
		if (detectedSide <= 0)
		{
			return;
		}

		currentSideOutput = detectedSide;

		if (currentSideOutput != lastSideOutput)
		{
			ApplyManualPreset(currentSideOutput);

			if (logWhenSideChanges)
			{
				Debug.Log($"{name}: side output = {currentSideOutput} ({GetSideName(currentSideOutput)})");
			}

			lastSideOutput = currentSideOutput;
		}
	}

	private int DetermineDominantSide()
	{
		Transform lens = ResolveLens();
		if (lens == null)
		{
			if (!warnedMissingLens)
			{
				Debug.LogWarning("blockToolAPI could not find a VR lens/camera. Assign vrLens or tag your camera as MainCamera.");
				warnedMissingLens = true;
			}
			return -1;
		}

		Vector3 toLens = (lens.position - transform.position).normalized;

		Vector3[] faceNormals =
		{
			transform.forward,
			transform.right,
			-transform.forward,
			-transform.right,
			transform.up,
			-transform.up
		};

		float bestDot = float.NegativeInfinity;
		int bestIndex = 1;

		for (int i = 0; i < faceNormals.Length; i++)
		{
			float dot = Vector3.Dot(faceNormals[i], toLens);
			if (dot > bestDot)
			{
				bestDot = dot;
				bestIndex = i + 1;
			}
		}

		return bestIndex;
	}

	private Transform ResolveLens()
	{
		if (vrLens != null)
		{
			return vrLens;
		}

		if (useMainCameraIfLensMissing && Camera.main != null)
		{
			return Camera.main.transform;
		}

		if (Camera.allCamerasCount > 0 && Camera.allCameras[0] != null)
		{
			return Camera.allCameras[0].transform;
		}

		return null;
	}

	private void ApplyManualPreset(int side)
	{
		switch (side)
		{
			case 1:
				if (timeAPI != null) timeAPI.SetDayMode();
				if (dayNightApi != null) dayNightApi.SetDayMode();
				break;

			case 2:
				if (weatherAPI != null) weatherAPI.SetManualWeather("snow");
				break;

			case 3:
				if (timeAPI != null) timeAPI.SetNightModeHour24();
				if (dayNightApi != null) dayNightApi.SetNightMode();
				break;

			case 4:
				if (weatherAPI != null) weatherAPI.SetManualWeather("cloudy");
				break;

			case 5:
				if (weatherAPI != null) weatherAPI.ClearManualWeather();
				if (timeAPI != null) timeAPI.ClearManualTime();
				if (dayNightApi != null) dayNightApi.ClearManualOverride();
				break;

			case 6:
				if (weatherAPI != null) weatherAPI.SetManualWeather("rain");
				break;
		}
	}

	private string GetSideName(int side)
	{
		switch (side)
		{
			case 1: return "Front";
			case 2: return "Right";
			case 3: return "Back";
			case 4: return "Left";
			case 5: return "Top";
			case 6: return "Bottom";
			default: return "Unknown";
		}
	}
}
