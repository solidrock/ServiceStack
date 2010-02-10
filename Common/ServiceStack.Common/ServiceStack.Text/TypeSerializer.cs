using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ServiceStack.Text.Jsv;

namespace ServiceStack.Text
{
	/// <summary>
	/// Creates an instance of a Type from a string value
	/// </summary>
	public static class TypeSerializer
	{
		public const char MapStartChar = '{';
		public const char MapKeySeperator = ':';
		public const char ItemSeperator = ',';
		public const char MapEndChar = '}';
		public const string MapNullValue = "\"\"";

		public const char ListStartChar = '[';
		public const char ListEndChar = ']';

		public const char QuoteChar = '"';
		public const string QuoteString = "\"";
		public const string DoubleQuoteString = "\"\"";
		public const string ItemSeperatorString = ",";
		public const string MapKeySeperatorString = ":";

		public static readonly char[] CsvChars = new[] { ItemSeperator, QuoteChar };
		public static readonly char[] EscapeChars = new[] { QuoteChar, ItemSeperator, MapStartChar, MapEndChar, ListStartChar, ListEndChar, };

		/// <summary>
		/// Determines whether the specified type is convertible from string.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type is convertible from string; otherwise, <c>false</c>.
		/// </returns>
		public static bool CanCreateFromString(Type type)
		{
			return JsvReader.GetParseFn(type) != null;
		}

		/// <summary>
		/// Parses the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static T DeserializeFromString<T>(string value)
		{
			if (string.IsNullOrEmpty(value)) return default(T);
			return (T) JsvReader<T>.Parse(value);
		}

		public static T DeserializeFromReader<T>(TextReader reader)
		{
			return DeserializeFromString<T>(reader.ReadToEnd());
		}

		/// <summary>
		/// Parses the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static object DeserializeFromString(string value, Type type)
		{
			return value == null 
			       	? null 
			       	: JsvReader.GetParseFn(type)(value);
		}

		public static object DeserializeFromReader(TextReader reader, Type type)
		{
			return DeserializeFromString(reader.ReadToEnd(), type);
		}

		public static string SerializeToString<T>(T value)
		{
			if (value == null) return null;
			if (typeof(T) == typeof(string)) return value as string;

			var sb = new StringBuilder(4096);
			using (var writer = new StringWriter(sb))
			{
				JsvWriter<T>.WriteObject(writer, value);
			}
			return sb.ToString();
		}

		public static void SerializeToWriter<T>(T value, TextWriter writer)
		{
			if (value == null) return;
			if (typeof(T) == typeof(string))
			{
				writer.Write(value);
				return;
			}

			JsvWriter<T>.WriteObject(writer, value);
		}

		public static string SerializeToCsv<T>(IEnumerable<T> records)
		{
			var sb = new StringBuilder();
			using (var writer = new StringWriter(sb))
			{
				writer.WriteCsv(records);
				return sb.ToString();
			}
		}
	}
}