using Ast.NodeArchetypes;
using Ast.Nodes;
using Runtime.Objects;

namespace Ast.Tables;

public class RuntimeVariableTable
{
	private Dictionary<(Type, string), RuntimeObject> _variables = new();

	public RuntimeVariableTable Clone()
	{
		return new()
		{
			_variables = new(this._variables)
		};
	}

	public T Get<T>(string id) where T : RuntimeObject
	{
		T result = (T)_variables[(typeof(T), id)];

		if (result == null)
		{
			throw new Exception($"Internal error: Cannot get variable of type '{typeof(T)}' and id '{id}' from variable table because the variable does not exist.");
		}

		return result;
	}

	public bool TryGet<T>(string id, out T value) where T : RuntimeObject
	{
		if (_variables.TryGetValue((typeof(T), id), out RuntimeObject? output))
		{
			value = (T)output;
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	public void Upsert(RuntimeObject value, VariableNode symbolNode)
	{
		_variables[(value.GetType(), symbolNode.Id)] = value;
	}

}