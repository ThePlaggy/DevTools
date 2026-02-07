using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace ZenFulcrum.EmbeddedBrowser;

internal static class JSONParser
{
	private const int TOKEN_NONE = 0;

	private const int TOKEN_CURLY_OPEN = 1;

	private const int TOKEN_CURLY_CLOSE = 2;

	private const int TOKEN_SQUARED_OPEN = 3;

	private const int TOKEN_SQUARED_CLOSE = 4;

	private const int TOKEN_COLON = 5;

	private const int TOKEN_COMMA = 6;

	private const int TOKEN_STRING = 7;

	private const int TOKEN_NUMBER = 8;

	private const int TOKEN_TRUE = 9;

	private const int TOKEN_FALSE = 10;

	private const int TOKEN_NULL = 11;

	private const int BUILDER_CAPACITY = 2000;

	private static readonly char[] EscapeTable;

	private static readonly char[] EscapeCharacters;

	static JSONParser()
	{
		EscapeCharacters = new char[7] { '"', '\\', '\b', '\f', '\n', '\r', '\t' };
		EscapeTable = new char[93];
		EscapeTable[34] = '"';
		EscapeTable[92] = '\\';
		EscapeTable[8] = 'b';
		EscapeTable[12] = 'f';
		EscapeTable[10] = 'n';
		EscapeTable[13] = 'r';
		EscapeTable[9] = 't';
	}

	public static JSONNode Parse(string json)
	{
		if (TryDeserializeObject(json, out var obj))
		{
			return obj;
		}
		throw new SerializationException("Invalid JSON string");
	}

	public static bool TryDeserializeObject(string json, out JSONNode obj)
	{
		bool success = true;
		if (json != null)
		{
			char[] json2 = json.ToCharArray();
			int index = 0;
			obj = ParseValue(json2, ref index, ref success);
		}
		else
		{
			obj = null;
		}
		return success;
	}

	public static string EscapeToJavascriptString(string jsonString)
	{
		if (string.IsNullOrEmpty(jsonString))
		{
			return jsonString;
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		while (num < jsonString.Length)
		{
			char c = jsonString[num++];
			if (c == '\\')
			{
				int num2 = jsonString.Length - num;
				if (num2 >= 2)
				{
					switch (jsonString[num])
					{
					case '\\':
						stringBuilder.Append('\\');
						num++;
						break;
					case '"':
						stringBuilder.Append("\"");
						num++;
						break;
					case 't':
						stringBuilder.Append('\t');
						num++;
						break;
					case 'b':
						stringBuilder.Append('\b');
						num++;
						break;
					case 'n':
						stringBuilder.Append('\n');
						num++;
						break;
					case 'r':
						stringBuilder.Append('\r');
						num++;
						break;
					}
				}
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private static JSONNode ParseObject(char[] json, ref int index, ref bool success)
	{
		JSONNode jSONNode = new JSONNode(JSONNode.NodeType.Object);
		NextToken(json, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (LookAhead(json, index))
			{
			case 0:
				success = false;
				return null;
			case 6:
				NextToken(json, ref index);
				continue;
			case 2:
				NextToken(json, ref index);
				return jSONNode;
			}
			string k = ParseString(json, ref index, ref success);
			if (!success)
			{
				success = false;
				return null;
			}
			int num = NextToken(json, ref index);
			if (num != 5)
			{
				success = false;
				return null;
			}
			JSONNode value = ParseValue(json, ref index, ref success);
			if (!success)
			{
				success = false;
				return null;
			}
			jSONNode[k] = value;
		}
		return jSONNode;
	}

	private static JSONNode ParseArray(char[] json, ref int index, ref bool success)
	{
		JSONNode jSONNode = new JSONNode(JSONNode.NodeType.Array);
		NextToken(json, ref index);
		bool flag = false;
		while (!flag)
		{
			switch (LookAhead(json, index))
			{
			case 0:
				success = false;
				return null;
			case 6:
				NextToken(json, ref index);
				continue;
			case 4:
				break;
			default:
			{
				JSONNode item = ParseValue(json, ref index, ref success);
				if (!success)
				{
					return null;
				}
				jSONNode.Add(item);
				continue;
			}
			}
			NextToken(json, ref index);
			break;
		}
		return jSONNode;
	}

	private static JSONNode ParseValue(char[] json, ref int index, ref bool success)
	{
		switch (LookAhead(json, index))
		{
		case 1:
			return ParseObject(json, ref index, ref success);
		case 3:
			return ParseArray(json, ref index, ref success);
		case 7:
			return ParseString(json, ref index, ref success);
		case 8:
			return ParseNumber(json, ref index, ref success);
		case 9:
			NextToken(json, ref index);
			return true;
		case 10:
			NextToken(json, ref index);
			return false;
		case 11:
			NextToken(json, ref index);
			return JSONNode.NullNode;
		default:
			success = false;
			return JSONNode.InvalidNode;
		}
	}

	private static JSONNode ParseString(char[] json, ref int index, ref bool success)
	{
		StringBuilder stringBuilder = new StringBuilder(2000);
		EatWhitespace(json, ref index);
		char c = json[index++];
		bool flag = false;
		while (!flag && index != json.Length)
		{
			c = json[index++];
			switch (c)
			{
			case '"':
				flag = true;
				break;
			case '\\':
			{
				if (index == json.Length)
				{
					break;
				}
				switch (json[index++])
				{
				case '"':
					stringBuilder.Append('"');
					continue;
				case '\\':
					stringBuilder.Append('\\');
					continue;
				case '/':
					stringBuilder.Append('/');
					continue;
				case 'b':
					stringBuilder.Append('\b');
					continue;
				case 'f':
					stringBuilder.Append('\f');
					continue;
				case 'n':
					stringBuilder.Append('\n');
					continue;
				case 'r':
					stringBuilder.Append('\r');
					continue;
				case 't':
					stringBuilder.Append('\t');
					continue;
				case 'u':
					break;
				default:
					continue;
				}
				int num = json.Length - index;
				if (num < 4)
				{
					break;
				}
				if (!(success = uint.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result)))
				{
					return string.Empty;
				}
				if (55296 <= result && result <= 56319)
				{
					index += 4;
					num = json.Length - index;
					if (num < 6 || !(new string(json, index, 2) == "\\u") || !uint.TryParse(new string(json, index + 2, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result2) || 56320 > result2 || result2 > 57343)
					{
						success = false;
						return string.Empty;
					}
					stringBuilder.Append((char)result);
					stringBuilder.Append((char)result2);
					index += 6;
				}
				else
				{
					stringBuilder.Append(ConvertFromUtf32((int)result));
					index += 4;
				}
				continue;
			}
			default:
				stringBuilder.Append(c);
				continue;
			}
			break;
		}
		if (!flag)
		{
			success = false;
			return null;
		}
		return stringBuilder.ToString();
	}

	private static string ConvertFromUtf32(int utf32)
	{
		if (utf32 < 0 || utf32 > 1114111)
		{
			throw new ArgumentOutOfRangeException("utf32", "The argument must be from 0 to 0x10FFFF.");
		}
		if (55296 <= utf32 && utf32 <= 57343)
		{
			throw new ArgumentOutOfRangeException("utf32", "The argument must not be in surrogate pair range.");
		}
		if (utf32 < 65536)
		{
			return new string((char)utf32, 1);
		}
		utf32 -= 65536;
		return new string(new char[2]
		{
			(char)((utf32 >> 10) + 55296),
			(char)(utf32 % 1024 + 56320)
		});
	}

	private static JSONNode ParseNumber(char[] json, ref int index, ref bool success)
	{
		EatWhitespace(json, ref index);
		int lastIndexOfNumber = GetLastIndexOfNumber(json, index);
		int length = lastIndexOfNumber - index + 1;
		string text = new string(json, index, length);
		JSONNode result2;
		if (text.IndexOf(".", StringComparison.OrdinalIgnoreCase) != -1 || text.IndexOf("e", StringComparison.OrdinalIgnoreCase) != -1)
		{
			success = double.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out var result);
			result2 = result;
		}
		else
		{
			success = long.TryParse(new string(json, index, length), NumberStyles.Any, CultureInfo.InvariantCulture, out var result3);
			result2 = result3;
		}
		index = lastIndexOfNumber + 1;
		return result2;
	}

	private static int GetLastIndexOfNumber(char[] json, int index)
	{
		int i;
		for (i = index; i < json.Length && "0123456789+-.eE".IndexOf(json[i]) != -1; i++)
		{
		}
		return i - 1;
	}

	private static void EatWhitespace(char[] json, ref int index)
	{
		while (index < json.Length && " \t\n\r\b\f".IndexOf(json[index]) != -1)
		{
			index++;
		}
	}

	private static int LookAhead(char[] json, int index)
	{
		int index2 = index;
		return NextToken(json, ref index2);
	}

	private static int NextToken(char[] json, ref int index)
	{
		EatWhitespace(json, ref index);
		if (index == json.Length)
		{
			return 0;
		}
		char c = json[index];
		index++;
		switch (c)
		{
		case ',':
			return 6;
		case '-':
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
			return 8;
		default:
			switch (c)
			{
			case '[':
				return 3;
			default:
				switch (c)
				{
				case '{':
					return 1;
				default:
				{
					if (c == '"')
					{
						return 7;
					}
					index--;
					int num = json.Length - index;
					if (num >= 5 && json[index] == 'f' && json[index + 1] == 'a' && json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
					{
						index += 5;
						return 10;
					}
					if (num >= 4 && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' && json[index + 3] == 'e')
					{
						index += 4;
						return 9;
					}
					if (num >= 4 && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' && json[index + 3] == 'l')
					{
						index += 4;
						return 11;
					}
					return 0;
				}
				case '}':
					return 2;
				}
			case ']':
				return 4;
			}
		case ':':
			return 5;
		}
	}

	public static string Serialize(JSONNode node)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!SerializeValue(node, stringBuilder))
		{
			throw new SerializationException("Failed to serialize JSON");
		}
		return stringBuilder.ToString();
	}

	private static bool SerializeValue(JSONNode value, StringBuilder builder)
	{
		bool result = true;
		switch (value.Type)
		{
		case JSONNode.NodeType.Invalid:
			throw new SerializationException("Cannot serialize invalid JSONNode");
		case JSONNode.NodeType.String:
			result = SerializeString(value, builder);
			break;
		case JSONNode.NodeType.Number:
			result = SerializeNumber(value, builder);
			break;
		case JSONNode.NodeType.Object:
		{
			Dictionary<string, JSONNode> dictionary = value;
			result = SerializeObject(dictionary.Keys, dictionary.Values, builder);
			break;
		}
		case JSONNode.NodeType.Array:
			result = SerializeArray((List<JSONNode>)value, builder);
			break;
		case JSONNode.NodeType.Bool:
			builder.Append((!value) ? "false" : "true");
			break;
		case JSONNode.NodeType.Null:
			builder.Append("null");
			break;
		default:
			throw new SerializationException("Unknown JSONNode type");
		}
		return result;
	}

	private static bool SerializeObject(IEnumerable<string> keys, IEnumerable<JSONNode> values, StringBuilder builder)
	{
		builder.Append("{");
		IEnumerator<string> enumerator = keys.GetEnumerator();
		IEnumerator<JSONNode> enumerator2 = values.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext() && enumerator2.MoveNext())
		{
			string current = enumerator.Current;
			JSONNode current2 = enumerator2.Current;
			if (!flag)
			{
				builder.Append(",");
			}
			string text = current;
			if (text != null)
			{
				SerializeString(text, builder);
			}
			else if (!SerializeValue(current2, builder))
			{
				return false;
			}
			builder.Append(":");
			if (!SerializeValue(current2, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	private static bool SerializeArray(IEnumerable<JSONNode> anArray, StringBuilder builder)
	{
		builder.Append("[");
		bool flag = true;
		foreach (JSONNode item in anArray)
		{
			if (!flag)
			{
				builder.Append(",");
			}
			if (!SerializeValue(item, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("]");
		return true;
	}

	private static bool SerializeString(string aString, StringBuilder builder)
	{
		if (aString.IndexOfAny(EscapeCharacters) == -1)
		{
			builder.Append('"');
			builder.Append(aString);
			builder.Append('"');
			return true;
		}
		builder.Append('"');
		int num = 0;
		char[] array = aString.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (c >= EscapeTable.Length || EscapeTable[(uint)c] == '\0')
			{
				num++;
				continue;
			}
			if (num > 0)
			{
				builder.Append(array, i - num, num);
				num = 0;
			}
			builder.Append('\\');
			builder.Append(EscapeTable[(uint)c]);
		}
		if (num > 0)
		{
			builder.Append(array, array.Length - num, num);
		}
		builder.Append('"');
		return true;
	}

	private static bool SerializeNumber(double number, StringBuilder builder)
	{
		builder.Append(Convert.ToDouble(number, CultureInfo.InvariantCulture).ToString("r", CultureInfo.InvariantCulture));
		return true;
	}
}
