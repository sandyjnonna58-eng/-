using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace CombatSandbox.Demo;

public static class Loc
{
	private static Dictionary<string, string> exact;

	private static Dictionary<string, string> fragments;

	private static Regex fragment;

	private static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

	private static readonly Regex split = new Regex("(<[^>]*>|\\n)");

	public static bool Russian => GameSettings.Language == 0;

	public static string T(string s)
	{
		if (!Russian || string.IsNullOrEmpty(s))
		{
			return s;
		}
		if (cache.TryGetValue(s, out var value))
		{
			return value;
		}
		if (exact == null)
		{
			Prepare();
		}
		value = Translate(s);
		if (cache.Count > 6000)
		{
			cache.Clear();
		}
		cache[s] = value;
		return value;
	}

	private static string Translate(string s)
	{
		if (HasCyrillic(s))
		{
			return s;
		}
		if (s.IndexOf('<') < 0 && s.IndexOf('\n') < 0)
		{
			return Piece(s);
		}
		string[] array = split.Split(s);
		StringBuilder stringBuilder = new StringBuilder(s.Length + 16);
		foreach (string text in array)
		{
			if (text.Length == 0)
			{
				continue;
			}
			if (text == "\n" || (text[0] == '<' && text[text.Length - 1] == '>'))
			{
				stringBuilder.Append(text);
			}
			else
			{
				stringBuilder.Append(Piece(text));
			}
		}
		return stringBuilder.ToString();
	}

	private static string Piece(string p)
	{
		int i = 0;
		int num = p.Length;
		while (i < num && char.IsWhiteSpace(p[i]))
		{
			i++;
		}
		while (num > i && char.IsWhiteSpace(p[num - 1]))
		{
			num--;
		}
		if (i >= num)
		{
			return p;
		}
		string text = p.Substring(i, num - i);
		if (!exact.TryGetValue(text, out var value))
		{
			if (!HasLetter(text))
			{
				return p;
			}
			value = fragment.Replace(text, (Match m) => fragments[m.Value]);
		}
		return p.Substring(0, i) + value + p.Substring(num);
	}

	private static bool HasLetter(string s)
	{
		foreach (char c in s)
		{
			if (char.IsLetter(c))
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasCyrillic(string s)
	{
		foreach (char c in s)
		{
			if (c >= 'Ѐ' && c <= 'ӿ')
			{
				return true;
			}
		}
		return false;
	}

	private static void Prepare()
	{
		exact = new Dictionary<string, string>();
		foreach (var (text, value) in LocRu.Pairs())
		{
			string text2 = text.Trim();
			if (text2.Length != 0)
			{
				exact[text2] = value.Trim();
			}
		}
		foreach (KeyValuePair<string, string> item in exact.ToList())
		{
			string text3 = item.Key.ToUpperInvariant();
			if (text3 != item.Key && !exact.ContainsKey(text3))
			{
				exact[text3] = item.Value.ToUpperInvariant();
			}
		}
		fragments = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> item2 in exact)
		{
			if (item2.Key.Length >= 2 && HasLetter(item2.Key))
			{
				fragments[item2.Key] = item2.Value;
			}
		}
		string text4 = string.Join("|", from k in fragments.Keys
			orderby k.Length descending
			select Regex.Escape(k));
		fragment = new Regex("(?<![\\p{L}\\p{Nd}])(?:" + text4 + ")(?![\\p{L}\\p{Nd}])", RegexOptions.CultureInvariant);
	}
}

public sealed class LocRunner : MonoBehaviour
{
	private static LocRunner instance;

	private Text[] texts = new Text[0];

	private float nextScan;

	private readonly Dictionary<Text, string> shown = new Dictionary<Text, string>();

	private readonly Dictionary<Text, string> original = new Dictionary<Text, string>();

	private readonly Dictionary<Text, bool> skip = new Dictionary<Text, bool>();

	public static void Ensure()
	{
		if (!instance && Application.isPlaying)
		{
			GameObject obj = new GameObject("Localisation");
			Object.DontDestroyOnLoad(obj);
			instance = obj.AddComponent<LocRunner>();
		}
	}

	private void OnEnable()
	{
		Canvas.willRenderCanvases += Apply;
	}

	private void OnDisable()
	{
		Canvas.willRenderCanvases -= Apply;
	}

	private void LateUpdate()
	{
		Apply();
	}

	private void Apply()
	{
		if (Time.unscaledTime >= nextScan)
		{
			nextScan = Time.unscaledTime + 0.2f;
			texts = Object.FindObjectsByType<Text>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			if (shown.Count > 3000)
			{
				shown.Clear();
				original.Clear();
				skip.Clear();
			}
		}
		bool russian = Loc.Russian;
		Text[] array = texts;
		foreach (Text text in array)
		{
			if (!text || !text.isActiveAndEnabled)
			{
				continue;
			}
			string text2 = text.text;
			string value;
			bool flag = shown.TryGetValue(text, out value) && (object)value == text2;
			if (russian)
			{
				if (flag || Skipped(text))
				{
					continue;
				}
				string text3 = Loc.T(text2);
				if ((object)text3 != text2 && text3 != text2)
				{
					original[text] = text2;
					text.text = text3;
					shown[text] = text.text;
				}
				else
				{
					shown[text] = text2;
				}
			}
			else if (value != null)
			{
				if (value == text2 && original.TryGetValue(text, out var value2))
				{
					text.text = value2;
				}
				shown.Remove(text);
				original.Remove(text);
			}
		}
	}

	private bool Skipped(Text t)
	{
		if (skip.TryGetValue(t, out var value))
		{
			return value;
		}
		InputField componentInParent = t.GetComponentInParent<InputField>(includeInactive: true);
		value = (bool)componentInParent && componentInParent.textComponent == t;
		skip[t] = value;
		return value;
	}
}
