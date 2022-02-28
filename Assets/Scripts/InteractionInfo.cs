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
	public static InteractionInfo Fail(string message)
	{
		InteractionInfo info = new InteractionInfo();

		info.success = false;
		info.info = message;

		return info;
	}
}