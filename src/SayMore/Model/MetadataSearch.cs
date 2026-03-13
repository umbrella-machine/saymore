using SayMore.Model.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace SayMore.Model
{
	public class MetadataSearch
	{
		
		private readonly ProjectContext _projectContext;

		public MetadataSearch(ProjectContext projectContext)
		{
			_projectContext = projectContext;
		}

		private static readonly HashSet<string> sessionFileSearchableTags = new HashSet<string>
		{
			"genre",
			"title",
			"setting",
			"participants",
			"situation",
			"synopsis",
			"location",
			"access",
			"notes",  // This serves both for the general session notes and for the specific participant notes
			"location_continent",
			"location_country",
			"location_region",
			"location_address",
			"sub-genre",
			"name"
		};

		private static readonly HashSet<string> annotationFileSearchableTags = new HashSet<string>
		{
			"annotation_value"
		};

		private static readonly HashSet<string> otherFileSearchableTags = new HashSet<string>
		{
			"notes",  // This serves both for the general session notes and for the specific participant notes
			"name",
			"microphone",
			"device",
			"participants",
			"annotation_value",
			"recordist",
			"speaker"
		};

		public IEnumerable<string> SearchSessions(string query)
		{
			System.Diagnostics.Debug.WriteLine("Searching for " + query);
			var allSessions = _projectContext.Project.GetAllSessions(CancellationToken.None);

			foreach (var session in allSessions)
			{
				bool found = false;
				foreach (var componentFile in session.GetComponentFiles())
				{
					HashSet<string> searchableTags;
					if (componentFile.FileType is SessionFileType)
					{
						searchableTags = new HashSet<string>(sessionFileSearchableTags.Union(GetCustomFieldIds(componentFile)));
					}
					else if (componentFile is AnnotationComponentFile)
					{
						/*System.Diagnostics.Debug.WriteLine("ANNOTATION");
						var oralTags = componentFile.MetaDataFieldValues;
						foreach (var field in oralTags)
							System.Diagnostics.Debug.WriteLine(field);*/
						searchableTags = annotationFileSearchableTags;
					}
					else if (componentFile is OralAnnotationComponentFile)
					{
						/*System.Diagnostics.Debug.WriteLine("ORAL");
						var oralTags = componentFile.MetaDataFieldValues;
						foreach (var field in oralTags)
							System.Diagnostics.Debug.WriteLine(field);*/
						searchableTags = annotationFileSearchableTags;
					}
					else
					{
						searchableTags = new HashSet<string>(otherFileSearchableTags.Union(GetCustomFieldIds(componentFile)));
					}

					var fields = componentFile.MetaDataFieldValues;

					foreach (var field in fields)
					{
						if (searchableTags.Contains(field.FieldId?.ToLowerInvariant()) && 
							(field.Value?.ToString() ?? string.Empty).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							System.Diagnostics.Debug.WriteLine("Found " + query);
							found = true;
							yield return session.Id;
							break;
						}
					}
					if (found) break;
				}
			}
		}
		private HashSet<string> GetCustomFieldIds(ComponentFile file)
		{
			HashSet<string> customFields = new HashSet<string>();

			if (file is ProjectElementComponentFile pef)
			{
				System.Diagnostics.Debug.WriteLine("Is pef");
				customFields = new HashSet<string>(
					pef.GetCustomFields()
						.Select(f => f.FieldId?.ToLowerInvariant())
						.Where(id => !string.IsNullOrEmpty(id))
				);
			}
			foreach (var thing in customFields)
			{
				System.Diagnostics.Debug.WriteLine(thing);
			}
			
			return customFields;
		}
	}
}
