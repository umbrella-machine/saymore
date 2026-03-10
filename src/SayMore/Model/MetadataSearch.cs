using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SayMore.Model
{
	internal class MetadataSearch
	{
		public static bool SearchXMLFile(string query, string filePath, HashSet<string> searchableTags)
		{
			var sessionSearchableTags = new HashSet<string>
			{
				"genre",
				"title",
				"setting",
				"participants",
				"situation",
				"synopsis",
				"location",
				"access",
				"notes",  // This serves both for the session notes and for the participant notes
                "Location_Continent",
				"Location_Country",
				"Location_Region",
				"Location_Address",
				"Sub-Genre",
				"name"
			};

			var eafSearchableTags = new HashSet<string>
			{
				"ANNOTATION_VALUE"
			};

			var metaSearchableTags = new HashSet<string>
			{
				"notes",  // This serves both for the session notes and for the participant notes
                "name",
				"Microphone",
				"Device",
				"participants"
			};

			using var reader = XmlReader.Create(filePath);

			bool isCustomField = false;

			while (reader.Read())
			{

				if (reader.NodeType == XmlNodeType.Element)
				{

					if (reader.Name == "CustomFields")
					{
						isCustomField = true;
						continue;
					}

					if ((eafSearchableTags.Contains(reader.Name) || isCustomField) && !reader.IsEmptyElement)
					{
						Console.WriteLine("Searching " + reader.Name);
						string value = reader.ReadElementContentAsString();
						if (value.Contains(query, StringComparison.OrdinalIgnoreCase))
						{
							Console.WriteLine("FOUND!");
							return true;
						}
					}

				}

				else if (reader.NodeType == XmlNodeType.Element &&
					reader.Name == "CustomFields")
				{
					isCustomField = false;
				}

				else
				{
					Console.WriteLine("Not searching " + reader.Name);
				}
			}

			Console.WriteLine("NOT FOUND :(");
			return false;
		}
	}
}
