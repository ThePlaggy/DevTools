using System;
using System.Collections.Generic;

namespace ZenFulcrum.EmbeddedBrowser;

public class JSONNode
{
	public enum NodeType
	{
		Invalid,
		String,
		Number,
		Object,
		Array,
		Bool,
		Null
	}

	public static readonly JSONNode InvalidNode = new JSONNode(NodeType.Invalid);

	public static readonly JSONNode NullNode = new JSONNode();

	private List<JSONNode> _arrayValue;

	private bool _boolValue;

	private double _numberValue;

	private Dictionary<string, JSONNode> _objectValue;

	private string _stringValue;

	public NodeType _type;

	public NodeType Type => _type;

	public bool IsValid => _type != NodeType.Invalid;

	public JSONNode this[string k]
	{
		get
		{
			if (_type == NodeType.Object && _objectValue.TryGetValue(k, out var value))
			{
				return value;
			}
			return InvalidNode;
		}
		set
		{
			if (_type != NodeType.Object)
			{
				throw new InvalidJSONNodeException();
			}
			if (value._type == NodeType.Invalid)
			{
				_objectValue.Remove(k);
			}
			else
			{
				_objectValue[k] = value;
			}
		}
	}

	public JSONNode this[int idx]
	{
		get
		{
			if (_type == NodeType.Array && idx >= 0 && idx < _arrayValue.Count)
			{
				return _arrayValue[idx];
			}
			return InvalidNode;
		}
		set
		{
			if (_type != NodeType.Array)
			{
				throw new InvalidJSONNodeException();
			}
			if (idx == -1)
			{
				if (value._type == NodeType.Invalid)
				{
					_arrayValue.RemoveAt(_arrayValue.Count - 1);
				}
				else
				{
					_arrayValue.Add(value);
				}
			}
			else if (value._type == NodeType.Invalid)
			{
				_arrayValue.RemoveAt(idx);
			}
			else
			{
				_arrayValue[idx] = value;
			}
		}
	}

	public int Count => _type switch
	{
		NodeType.Array => _arrayValue.Count, 
		NodeType.Object => _objectValue.Count, 
		_ => 0, 
	};

	public bool IsNull => _type == NodeType.Null;

	public object Value
	{
		get
		{
			switch (_type)
			{
			case NodeType.Invalid:
				Check();
				return null;
			case NodeType.String:
				return _stringValue;
			case NodeType.Number:
				return _numberValue;
			case NodeType.Object:
				return _objectValue;
			case NodeType.Array:
				return _arrayValue;
			case NodeType.Bool:
				return _boolValue;
			case NodeType.Null:
				return null;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	public string AsJSON => JSONParser.Serialize(this);

	public JSONNode(NodeType type = NodeType.Null)
	{
		_type = type;
		switch (type)
		{
		case NodeType.Object:
			_objectValue = new Dictionary<string, JSONNode>();
			break;
		case NodeType.Array:
			_arrayValue = new List<JSONNode>();
			break;
		}
	}

	public JSONNode(string value)
	{
		_type = NodeType.String;
		_stringValue = value;
	}

	public JSONNode(double value)
	{
		_type = NodeType.Number;
		_numberValue = value;
	}

	public JSONNode(Dictionary<string, JSONNode> value)
	{
		_type = NodeType.Object;
		_objectValue = value;
	}

	public JSONNode(List<JSONNode> value)
	{
		_type = NodeType.Array;
		_arrayValue = value;
	}

	public JSONNode(bool value)
	{
		_type = NodeType.Bool;
		_boolValue = value;
	}

	public static JSONNode Parse(string json)
	{
		return JSONParser.Parse(json);
	}

	public JSONNode Check()
	{
		if (_type == NodeType.Invalid)
		{
			throw new InvalidJSONNodeException();
		}
		return this;
	}

	public static implicit operator string(JSONNode n)
	{
		return (n._type != NodeType.String) ? null : n._stringValue;
	}

	public static implicit operator JSONNode(string v)
	{
		return new JSONNode(v);
	}

	public static implicit operator int(JSONNode n)
	{
		return (n._type == NodeType.Number) ? ((int)n._numberValue) : 0;
	}

	public static implicit operator JSONNode(int v)
	{
		return new JSONNode(v);
	}

	public static implicit operator float(JSONNode n)
	{
		return (n._type != NodeType.Number) ? 0f : ((float)n._numberValue);
	}

	public static implicit operator JSONNode(float v)
	{
		return new JSONNode(v);
	}

	public static implicit operator double(JSONNode n)
	{
		return (n._type != NodeType.Number) ? 0.0 : n._numberValue;
	}

	public static implicit operator JSONNode(double v)
	{
		return new JSONNode(v);
	}

	public static implicit operator Dictionary<string, JSONNode>(JSONNode n)
	{
		return (n._type != NodeType.Object) ? null : n._objectValue;
	}

	public static implicit operator List<JSONNode>(JSONNode n)
	{
		return (n._type != NodeType.Array) ? null : n._arrayValue;
	}

	public void Add(JSONNode item)
	{
		if (_type != NodeType.Array)
		{
			throw new InvalidJSONNodeException();
		}
		_arrayValue.Add(item);
	}

	public static implicit operator bool(JSONNode n)
	{
		return n._type == NodeType.Bool && n._boolValue;
	}

	public static implicit operator JSONNode(bool v)
	{
		return new JSONNode(v);
	}
}
