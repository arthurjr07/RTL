using System.Text.Json;
using System.Text.Json.Serialization;

namespace RTL.TvMaze.Scraper.Infrastructure.Utilities
{
    public class DateTimeConverter : JsonConverter<DateTime?>
	{
		private readonly string Format;
		public DateTimeConverter(string format)
		{
			Format = format;
		}
		public override void Write(Utf8JsonWriter writer, DateTime? date, JsonSerializerOptions options)
		{
			writer.WriteStringValue(date.Value.ToString(Format));
		}
		public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var value = reader.GetString();
			if (!string.IsNullOrWhiteSpace(value))
			{
				return DateTime.ParseExact(value, Format, null);
			}
			return null;
		}
	}
}
