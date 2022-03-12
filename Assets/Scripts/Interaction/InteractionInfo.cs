public class InteractionInfo
{
	public bool success;
	public string info;

	public static InteractionInfo Success()
	{
		InteractionInfo info = new InteractionInfo();

		info.success = true;

		return info;
	}
	public static InteractionInfo Fail(string error)
	{
		InteractionInfo info = new InteractionInfo();

		info.success = false;
		info.info = error;

		return info;
	}
}