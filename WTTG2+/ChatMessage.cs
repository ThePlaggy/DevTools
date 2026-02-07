public class ChatMessage
{
	public string Username;

	public string Content;

	public string Buffer;

	public override string ToString()
	{
		return Username + ": " + Content;
	}
}
