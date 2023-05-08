using System;
using System.Collections.Generic;

namespace DNA.Data.Units
{
	public abstract class Unit
	{
		private string[] _abbrevs;

		private string _name;

		protected static Dictionary<string, Unit> _lookupTable = new Dictionary<string, Unit>();

		public string[] Abbrevations
		{
			get
			{
				return _abbrevs;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		protected abstract object SetUnit(double value);

		public static void RegisterUnit(Unit unit)
		{
			string[] abbrevations = unit.Abbrevations;
			foreach (string key in abbrevations)
			{
				if (_lookupTable.ContainsKey(key))
				{
					throw new ArgumentException("Unit Already Registered");
				}
				_lookupTable[key] = unit;
			}
		}

		public static Unit ParseUnit(string unitStr)
		{
			return _lookupTable[unitStr];
		}

		public static object Convert(double val, string unit)
		{
			return Convert(val, ParseUnit(unit));
		}

		public static object Convert(double val, Unit unit)
		{
			return unit.SetUnit(val);
		}

		public override string ToString()
		{
			return _name;
		}

		public Unit(string[] abbrivations, string name)
		{
			_abbrevs = abbrivations;
			_name = name;
		}
	}
}
