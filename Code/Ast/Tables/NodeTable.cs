using Ast.Nodes;

namespace Ast.Tables;

public class NodeTable
{
	private Dictionary<Type, Node> _nodes = new();

	public NodeTable Clone()
	{
		return new()
		{
			_nodes = new(this._nodes)
		};
	}

	public T Get<T>() where T : Node
	{
		T result = (T)_nodes[typeof(T)];

		if (result == null)
		{
			throw new Exception($"Internal error: Cannot get node of type '{typeof(T)}' from node table because the node does not exist.");
		}

		return result;
	}

	/// <summary>
	/// "Upsert" means "update or insert", so this either
	/// 1) adds a new entry, if no entry with the inputted key exists, or
	/// 2) overwrites an old entry, if an entry with the inputted key already exists.
	/// </summary>
	public void Upsert(Node node)
	{
		_nodes[node.GetType()] = node;
	}
}