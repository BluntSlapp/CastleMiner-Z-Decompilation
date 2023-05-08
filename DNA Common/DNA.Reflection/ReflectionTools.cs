using System;
using System.Collections.Generic;
using System.Reflection;
using DNA.Collections;

namespace DNA.Reflection
{
	public static class ReflectionTools
	{
		private static Dictionary<Assembly, Dictionary<Assembly, int>> _assemblies = new Dictionary<Assembly, Dictionary<Assembly, int>>();

		public static int TypeNameComparison(Type a, Type b)
		{
			return string.Compare(a.FullName, b.FullName);
		}

		public static bool ImplementsInterface(this Type type, Type interfaceType)
		{
			Type[] interfaces = type.GetInterfaces();
			Type[] array = interfaces;
			foreach (Type type2 in array)
			{
				if (type2 == interfaceType)
				{
					return true;
				}
			}
			return false;
		}

		public static string GetCompanyName(this Assembly assembly)
		{
			object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
			if (customAttributes.Length == 0)
			{
				return "My Company Name Here";
			}
			AssemblyCompanyAttribute assemblyCompanyAttribute = (AssemblyCompanyAttribute)customAttributes[0];
			if (assemblyCompanyAttribute.Company == null || assemblyCompanyAttribute.Company == "")
			{
				return "My Company Name Here";
			}
			return assemblyCompanyAttribute.Company;
		}

		public static string GetCompanyName(this Type type)
		{
			return type.Assembly.GetCompanyName();
		}

		public static string GetCSharpName(this Type t)
		{
			if (t == typeof(void))
			{
				return "void";
			}
			if (t == typeof(int))
			{
				return "int";
			}
			if (t == typeof(bool))
			{
				return "bool";
			}
			if (t == typeof(byte))
			{
				return "byte";
			}
			if (t == typeof(sbyte))
			{
				return "sbyte";
			}
			if (t == typeof(char))
			{
				return "char";
			}
			if (t == typeof(decimal))
			{
				return "decimal";
			}
			if (t == typeof(float))
			{
				return "float";
			}
			if (t == typeof(uint))
			{
				return "uint";
			}
			if (t == typeof(long))
			{
				return "long";
			}
			if (t == typeof(object))
			{
				return "object";
			}
			if (t == typeof(short))
			{
				return "short";
			}
			if (t == typeof(ushort))
			{
				return "ushort";
			}
			if (t == typeof(string))
			{
				return "string";
			}
			return t.Name;
		}

		public static T GetAttribute<T>(this Type type, bool inherit)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(T), inherit);
			if (customAttributes.Length == 0)
			{
				return default(T);
			}
			return (T)customAttributes[0];
		}

		public static Assembly[] GetAssemblies()
		{
			Assembly[] array = new Assembly[_assemblies.Count];
			_assemblies.Keys.CopyTo(array, 0);
			return array;
		}

		public static void RegisterAssembly(Assembly callingAssembly, Assembly assembly)
		{
			Dictionary<Assembly, int> value = null;
			if (!_assemblies.TryGetValue(callingAssembly, out value))
			{
				value = new Dictionary<Assembly, int>();
				_assemblies[callingAssembly] = value;
			}
			value[assembly] = 0;
			if (!_assemblies.TryGetValue(assembly, out value))
			{
				value = new Dictionary<Assembly, int>();
				_assemblies[assembly] = value;
			}
		}

		public static Type[] GetTypes(Filter<Type> filter)
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			Assembly[] assemblies = GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] array = null;
				try
				{
					array = assembly.GetTypes();
				}
				catch
				{
					continue;
				}
				foreach (Type type in array)
				{
					if (filter != null && !filter(type))
					{
						continue;
					}
					Type value;
					if (dictionary.TryGetValue(type.FullName, out value))
					{
						if (type.Assembly.GetName().Version > value.Assembly.GetName().Version)
						{
							dictionary[type.FullName] = type;
						}
					}
					else
					{
						dictionary[type.FullName] = type;
					}
				}
			}
			Type[] array2 = new Type[dictionary.Values.Count];
			dictionary.Values.CopyTo(array2, 0);
			Array.Sort(array2, TypeNameComparison);
			return array2;
		}

		public static Type[] GetTypes()
		{
			return GetTypes(null);
		}
	}
}
