namespace AbstractSyntax;

/// <summary>
/// This is just the default "Dictionary" class extended with some additional functionalities.
/// </summary>
public class CustomDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull
{
	/// <summary>
	/// This is exactly the same as the default "Add" method for Dictionaries,
	/// except this throws an exception with a custom error message.
	/// </summary>
	public new void Add(TKey key, TValue value)
	{
		try
		{
			base.Add(key, value);
		}
		catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
		{
			throw new Exception($"More than one {typeof(TValue).Name} is using the ID '{key}'");
		}
	}
}

