using UnityEngine;

namespace CombatSandbox.Demo;

public sealed class ModuleHud : MonoBehaviour
{
	private BattleDirector director;

	private GUIStyle style;

	private GUIStyle title;

	private void Start()
	{
		director = GetComponent<BattleDirector>();
	}

	private void OnGUI()
	{
		if (!director || !director.Possessed || GameMenu.Blocking || TreatmentScreen.Active)
		{
			return;
		}
		ArenaTarget body = director.Possessed.Body;
		Vehicle vehicle = (body ? body.Vehicle : null);
		if (!vehicle || !body.Seated)
		{
			return;
		}
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.label)
			{
				fontSize = 14,
				fontStyle = FontStyle.Bold
			};
			title = new GUIStyle(style)
			{
				fontSize = 16
			};
		}
		float num = Mathf.Clamp(Screen.height / 1080f, 0.7f, 2f);
		style.fontSize = Mathf.RoundToInt(14f * num);
		title.fontSize = Mathf.RoundToInt(16f * num);
		float num2 = 22f * num;
		float num3 = 270f * num;
		float num4 = (float)Screen.width - num3 - 20f * num;
		float num5 = 90f * num;
		int count = vehicle.Modules.Count;
		GUI.color = new Color(0f, 0f, 0f, 0.55f);
		GUI.DrawTexture(new Rect(num4 - 8f * num, num5 - 6f * num, num3 + 16f * num, num2 * (float)(count + 1) + 12f * num), Texture2D.whiteTexture);
		GUI.color = new Color(0.95f, 0.7f, 0.25f);
		GUI.Label(new Rect(num4, num5, num3, num2), Loc.T("MODULES") + "  ·  " + Loc.T(vehicle.Kind), title);
		for (int i = 0; i < count; i++)
		{
			Vehicle.Module module = vehicle.Modules[i];
			float num6 = vehicle.ModuleState(module);
			string text = ((num6 <= 0f) ? "DESTROYED" : ((num6 < 0.65f) ? "DAMAGED" : "OK"));
			GUI.color = ((num6 <= 0f) ? new Color(0.92f, 0.27f, 0.22f) : ((num6 < 0.65f) ? new Color(0.95f, 0.75f, 0.25f) : new Color(0.5f, 0.88f, 0.5f)));
			float num7 = num5 + num2 * (float)(i + 1);
			GUI.Label(new Rect(num4, num7, num3 * 0.62f, num2), Loc.T(module.Name), style);
			GUI.Label(new Rect(num4 + num3 * 0.62f, num7, num3 * 0.38f, num2), Loc.T(text), style);
		}
		GUI.color = Color.white;
	}
}
