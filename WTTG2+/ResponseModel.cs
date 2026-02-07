using System;

[Serializable]
public class ResponseModel
{
	public bool Passed;

	public bool Success;

	public string Message;

	public string Data;
}
[Serializable]
public class ResponseModel<T>
{
	public bool Passed;

	public bool Success;

	public string Message;

	public T Data;
}
