using UnityEngine;

public class ErrorReporter
{
	private bool hasErroredOnce = false;

	public void ErrorOnce(string message, object context = null)
	{
		if (!hasErroredOnce)
		{
			hasErroredOnce = true;
			throw new DebugUtils.RuntimeError(message);
		}
	}
}
