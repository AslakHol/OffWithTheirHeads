using UnityEngine;

public class DebugUIText
{
	public GUIText GuiText { get; private set; }

	public DebugUIText()
	{
		GuiText = new GameObject("zzDebugUIText", typeof(GUIText)).GetComponent<GUIText>();
		GuiText.transform.position = new Vector3(0.01f, 0.99f);
	}

	public void SetText(string format, params object[] args)
	{
		GuiText.text = string.Format(format, args);
	}
}
