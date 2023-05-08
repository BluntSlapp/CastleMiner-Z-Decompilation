using System.Collections.Generic;

namespace DNA.Data
{
	public class SpreadSheet<T>
	{
		public class Page
		{
			private T[,] Cells = new T[0, 0];

			public T this[int row, int column]
			{
				get
				{
					return Cells[row, column];
				}
				set
				{
					Cells[row, column] = value;
				}
			}

			public int RowCount
			{
				get
				{
					return Cells.GetLength(0);
				}
			}

			public int ColumnCount
			{
				get
				{
					return Cells.GetLength(1);
				}
			}

			public Page()
			{
			}

			public Page(int rows, int cols)
			{
				Init(rows, cols);
			}

			public void Init(int rows, int columns)
			{
				Cells = new T[rows, columns];
			}
		}

		public List<Page> Pages = new List<Page>();
	}
}
