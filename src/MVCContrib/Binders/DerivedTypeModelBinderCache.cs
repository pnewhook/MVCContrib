using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using MvcContrib.Attributes;

namespace MvcContrib.Binders
{
	/// <summary>
	/// This cache is used to both improve performance of the derived type model binder
	/// on cases where a binding type has already been identified.
	/// </summary>
	public static class DerivedTypeModelBinderCache
	{
		private static readonly ThreadSafeDictionary<Type, IEnumerable<Type>> TypeCache =
			new ThreadSafeDictionary<Type, IEnumerable<Type>>();

		private static readonly ConcurrentDictionary<string, Type> _hashKeyDictionary =
			new ConcurrentDictionary<string, Type>();

		private static string _typeStampFieldName = "_xTypeStampx_";

		public static string TypeStampFieldName
		{
			get { return _typeStampFieldName; } 
			set { _typeStampFieldName = value; }
		}

		/// <summary>
		/// Registers the attached set of derived types by the indicated base type
		/// </summary>
		/// <param name="baseType">base type that will be encountered by the binder where an alternate value should be used</param>
		/// <param name="derivedTypes">an enumerable set of types to be considered for binding</param>
		public static bool RegisterDerivedTypes(Type baseType, IEnumerable<Type> derivedTypes)
		{
			try
			{
				// register the types based on the base type
				TypeCache.Add(baseType, derivedTypes);
			}
			catch (ArgumentException)
			{
				return false;
			}

			foreach (var item in derivedTypes)
			{
				// this step is needed to make sure the closure behaves properly
				var currentItem = item;

				_hashKeyDictionary.AddOrUpdate(EncryptStringToBase64(currentItem.FullName), (name) => currentItem,
				                               (name, itemValue) => currentItem);
			}
			// register the base type with the derived type modelbinder for binding purposes
			ModelBinders.Binders.Add(baseType, new DerivedTypeModelBinder());

			return true;

		}

		private static readonly byte[] _encryptionSalt = new byte[]
                                                          {
                                                              0xc5, 0x10, 0x53, 0xe3, 0xc4, 0x17, 0x47, 0xc9,
                                                              0x85, 0x5d, 0xf1, 0x62, 0x73, 0x94, 0x12, 0x9e
                                                          };

		private static string EncryptStringToBase64(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			var passwordData = Encoding.UTF8.GetBytes(value);

			var hashAlgorithm = new HMACSHA256(_encryptionSalt);

			var hashBytes = hashAlgorithm.ComputeHash(passwordData);

			return Convert.ToBase64String(hashBytes);
		}

		public static Type GetDerivedType(string typeValue)
		{
			Type type;
			return _hashKeyDictionary.TryGetValue(typeValue, out type) ? type : null;
		}

		/// <summary>
		/// removes all items from the cache
		/// </summary>
		public static void Reset()
		{
			// first, remove all type registrations in Mvc's binder registry
			foreach (var type in TypeCache)
				ModelBinders.Binders.Remove(type.Key);

			_hashKeyDictionary.Clear();

			// clear the cache
			TypeCache.Clear();
		}

		/// <summary>
		/// Gets a hashed name for the given type and also checks to see if the type has been registered.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public static string GetTypeName(Type type)
		{
			var name = EncryptStringToBase64(type.FullName);
			
			if( !_hashKeyDictionary.ContainsKey( name))
				throw new InvalidOperationException(string.Format("Type {0} has not been registered with the DerivedTypeModelBinderCache", type.Name));

			return name;
		}
	}
}


