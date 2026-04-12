using Ast.Nodes;

namespace Ast.Tables;

public class SemanticSymbolTable
{
	/// <summary>
	/// Key: node type and id.
	/// Value: scope depth.
	/// </summary>
	private Dictionary<(Type, string), int> _symbols = new();

	public SemanticSymbolTable Clone()
	{
		return new()
		{
			_symbols = new(this._symbols)
		};
	}

	public bool Contains(Type type, string id)
	{
		return _symbols.ContainsKey((type, id));
	}

	public void Add(VariableNode symbolNode)
	{
		if (_symbols.TryGetValue((symbolNode.GetType(), symbolNode.Id), out int oldScope))
		{
			if (symbolNode.ScopeDepth <= oldScope)
			{
				Annotator.AddError(symbolNode, $"ID: '{symbolNode.Id}'. Scope depth: '{symbolNode.ScopeDepth}'. Double declaration within the same scope level.");
			}
		}

		_symbols[(symbolNode.GetType(), symbolNode.Id)] = symbolNode.ScopeDepth;
	}

}