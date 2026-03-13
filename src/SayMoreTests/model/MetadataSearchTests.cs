using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using SIL.TestUtilities;
using SayMore.Model;

namespace SayMoreTests.model
{
	[TestFixture]
	public class MetadataSearchTests
	{
		private TemporaryFolder _tempFolder;

		[SetUp]
		public void Setup()
		{
			_tempFolder = new TemporaryFolder("metadataSearchTest");
		}

		[TearDown]
		public void TearDown()
		{
			_tempFolder.Dispose();
		}

		/// ------------------------------------------------------------------------------------
		///<summary>
		///helper method to create temporary XML files
		///</summary>
		/// ------------------------------------------------------------------------------------
		private string CreateTestXmlFile(string xmlContent)
		{
			var filePath = Path.Combine(_tempFolder.Path, "data.xml");
			File.WriteAllText(filePath, xmlContent);
			return filePath;
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void SearchXMLFile_QueryFoundInSearchableTag_ReturnsTrue()
		{
			var xml = @"<Session>
				<title>My Session</title>
				<synopsis>This is about apples and oranges</synopsis>
			</Session>";

			var filePath = CreateTestXmlFile(xml);
			var searchableTags = new HashSet<string> { "title", "synopsis" };

			Assert.IsTrue(MetadataSearch.SearchXMLFile("apples", filePath, searchableTags));
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void SearchXMLFile_QueryNotFound_ReturnsFalse()
		{
			var xml = @"<Session>
				<title>My Session</title>
				<synopsis>This is about oranges</synopsis>
			</Session>";

			var filePath = CreateTestXmlFile(xml);
			var searchableTags = new HashSet<string> { "title", "synopsis" };

			Assert.IsFalse(MetadataSearch.SearchXMLFile("apples", filePath, searchableTags));
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void SearchXMLFile_QueryInNonSearchableTag_ReturnsFalse()
		{
			var xml = @"<Session>
				<title>My Session</title>
				<notes>This contains apples</notes>
			</Session>";

			var filePath = CreateTestXmlFile(xml);
			var searchableTags = new HashSet<string> { "title" };

			Assert.IsFalse(MetadataSearch.SearchXMLFile("apples", filePath, searchableTags));
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void SearchXMLFile_QueryIgnoresCase_ReturnsTrue()
		{
			var xml = @"<Session>
				<title>APPLES in uppercase</title>
			</Session>";

			var filePath = CreateTestXmlFile(xml);
			var searchableTags = new HashSet<string> { "title" };

			Assert.IsTrue(MetadataSearch.SearchXMLFile("apples", filePath, searchableTags));
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void SearchXMLFile_EmptyQuery_ReturnsFalse()
		{
			var xml = @"<Session>
				<title>My Session</title>
			</Session>";

			var filePath = CreateTestXmlFile(xml);
			var searchableTags = new HashSet<string> { "title" };

			Assert.IsFalse(MetadataSearch.SearchXMLFile("", filePath, searchableTags));
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void SearchXMLFile_EmptySearchableTags_ReturnsFalse()
		{
			var xml = @"<Session>
				<title>apples</title>
			</Session>";

			var filePath = CreateTestXmlFile(xml);
			var searchableTags = new HashSet<string>();

			Assert.IsFalse(MetadataSearch.SearchXMLFile("apples", filePath, searchableTags));
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void SearchXMLFile_FileNotFound_ReturnsFalse()
		{
			var searchableTags = new HashSet<string> { "title" };

			Assert.IsFalse(MetadataSearch.SearchXMLFile("query", "/nonexistent/path.xml", searchableTags));
		}

		/// ------------------------------------------------------------------------------------
		[Test]
		public void SearchXMLFile_QueryPartialMatch_ReturnsTrue()
		{
			var xml = @"<Session>
				<title>strawberries</title>
			</Session>";

			var filePath = CreateTestXmlFile(xml);
			var searchableTags = new HashSet<string> { "title" };

			Assert.IsTrue(MetadataSearch.SearchXMLFile("berr", filePath, searchableTags));
		}

		
		
		

	}
}
