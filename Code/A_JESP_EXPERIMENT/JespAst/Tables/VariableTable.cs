namespace JespAst.Tables;

public class VariableTable
{
	// TOOD: Add exception messages and handling

	private Dictionary<(Type, string), object> _variables = new();

	public VariableTable Clone()
	{
		return new()
		{
			_variables = new(this._variables)
		};
	}

	public T Get<T>(string id)
	{
		return (T)_variables[(typeof(T), id)];
	}

	public bool TryGet<T>(string id, out T value)
	{
		if (_variables.TryGetValue((typeof(T), id), out object? output))
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

	public void Upsert(object value, string id)
	{
		_variables[(value.GetType(), id)] = value;
	}

}