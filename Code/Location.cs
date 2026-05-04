

public class Location
{
	private string _fileName = "";
	private int _line = -1;
	private int _column = -1;

	public Location() { }

	public Location(string fileName, int line, int column)
	{
		_fileName = fileName;
		_line = line;
		_column = column;
	}

	public Location Clone()
	{
		return new()
		{
			_fileName = this._fileName,
			_line = this._line,
			_column = this._column,
		};
	}

	public override string ToString()
	{
		return $"File: '{_fileName}'. Line: '{_line}'. Column: '{_column}'";
	}
}