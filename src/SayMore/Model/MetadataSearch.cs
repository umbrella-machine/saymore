using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using SayMore.Model.Files;

namespace SayMore.Model
{
	public class MetadataSearch
	{
		
		private readonly ProjectContext _projectContext;

		public MetadataSearch(ProjectContext projectContext)
		{
			_projectContext = projectContext;
		}

		public bool SearchSession(string query)
		{
			System.Diagnostics.Debug.WriteLine("Searching for " + query);
			var allSessions = _projectContext.Project.GetAllSessions(CancellationToken.None);

			foreach (var session in allSessions)
			{
				foreach (var componentFile in session.GetComponentFiles())
				{
					var SessionSearchableTags = new HashSet<string>
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
						"location_continent",
						"location_country",
						"location_region",
						"location_address",
						"sub-genre",
						"name"
					};
					/*switch ()
					{
						case SessionFileType.Session:
						
						case SessionFileType.Eaf:
						if (SearchEaf(query, file))
						{
							return true;
						}
						break;
					case SessionFileType.Meta:
						var searchableTags = 
						break;
				}*/
					var fields = componentFile.MetaDataFieldValues;
					foreach (var field in fields)
					{
						if (SessionSearchableTags.Contains(field.FieldId.ToLower()) && String.Equals(field.Value?.ToString(), query, StringComparison.OrdinalIgnoreCase))
						{
							Console.WriteLine("Found {0}", query);
							return true;
						}
					}
				}
			}
		return false;
		}
	}
}
