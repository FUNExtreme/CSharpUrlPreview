using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mime;

namespace URLPreviewLib
{
	public static class URLPreviewGenerator
	{
		/// <summary>
		/// A struct representing a tag to search for
		/// </summary>
		private struct SearchTag
		{
			public string tag;
			public ParserState parseMethod;
		}

		/// <summary>
		/// The tags we'll be searching for
		/// IMPORTANT: order by length, longest first
		/// IMPORTANT: if you add new searchtags you still need to handle them yourself, there is too much difference between tags to create a method that works for all (CTRL+F for ContentParser)
		/// </summary>
		private static SearchTag[] searchTags = 
		{
			new SearchTag {tag = "title", parseMethod = ParserState.Content},
			new SearchTag {tag = "meta", parseMethod = ParserState.TagAttributes},
			new SearchTag {tag = "img", parseMethod = ParserState.TagAttributes}
		};

		/// <summary>
		/// The length of the longest tag we'll be searching for
		/// </summary>
		private static int iLongestSearchTag = searchTags[0].tag.Length;

		/// <summary>
		/// The length of the shortest tag we'll be searching for
		/// </summary>
		private static int iShortestSearchTag = searchTags[searchTags.Length - 1].tag.Length;

		/// <summary>
		/// The amount of tags
		/// </summary>
		private static int iSearchTagsAmount = searchTags.Length;

		/// <summary>
		/// Byte enum of all the states the parser can have
		/// </summary>
		private enum ParserState : byte
		{
			None,
			TagOpen,
			TagName,
			TagAttributes,
			Content,
			String,
			Comment
		}

		/// <summary>
		/// Byte enum identifying the current location of the parser
		/// </summary>
		private enum ParserLocation : byte
		{
			HTML,
			Comment,
			String,
		}

		/// <summary>
		/// Generates a preview from the given url
		/// </summary>
		/// <param name="strUrl">The url to generate a preview for</param>
		/// <returns>A preview object, null if invalid url</returns>
		public static URLPreview CreatePreview(string strUrl)
		{
			// Prepare a web request
			WebRequest request = WebRequest.Create(strUrl);
			// Get the web response
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			// If the response type is not text/html we have no idea what to do with it
			// TODO: Properly handle some other types? Display the type (video, audio, ..)?
			if(response.ContentType.Contains("text/html"))
			{
				Stream responseStream = response.GetResponseStream();
				URLPreview result = null;

				/*
				Stream[] streams = new Stream[10000];
				for(int x = 0; x < 10000; x ++)
				{
					responseStream.CopyTo(streams[x]);
				}

				Stopwatch swWatch = Stopwatch.StartNew();
				for(int x = 0; x < 10000; x ++)
				{
					result = ParseResponseStream(streams[x]);
				}
				 */

				Stopwatch swWatch = Stopwatch.StartNew();
				string str = "";
				using(StreamReader sr = new StreamReader(responseStream))
					str = sr.ReadToEnd();

				swWatch.Stop();
				Console.WriteLine("Read from stream: " + swWatch.ElapsedMilliseconds.ToString());

				swWatch = Stopwatch.StartNew();
				//for(int x = 0; x < 10000; x++)
				{
					result = ParseResponseString(str);
				}
				swWatch.Stop();

				Console.WriteLine("Parse (average of 10000): " + (swWatch.ElapsedMilliseconds /*/ 10000*/).ToString());
				return result;
			}

			return null;
		}

		#region ParseResponseStream UNUSED UNTIL I OPTIMISE ParseResponseString
		/*
		/// <summary>
		/// Parses the given response stream, only handles whatever is needed for previews.
		/// The following code is not written for readability, but for speed. So nothing is split up into seperate functions, function calls are slow
		/// </summary>
		/// <param name="sResponse">The stream to read from</param>
		private static URLPreview ParseResponseStream(Stream oResponse)
		{
			bool conditionMetTitle = false;
			bool conditionMetImage = false;
			bool conditionMetDescription = false;

			string title = String.Empty;
			string imageUrl = String.Empty;
			string description = String.Empty;

			using(StreamReader sr = new StreamReader(oResponse))
			{
				//try 
				//{
					bool ignoreTag = false;
					ParserState previousState = ParserState.None;
					ParserState currentState = ParserState.None;
					char[] previousChar = {'a', 'a'};
					char currentChar = 'a';
					string currentTag = "";
					string currentTagProperties = "";

					while(!sr.EndOfStream)
					{
						previousChar[1] = previousChar[0];
						previousChar[0] = currentChar;
						currentChar = (char)sr.Read();

						if(currentState == ParserState.Comment)
						{
							if(currentChar == '>' && previousChar[0] == '-' && previousChar[1] == '-')
								currentState = previousState;
						}
						else if(currentState == ParserState.String)
						{
							if(currentChar == '"')
								currentState = previousState;
						}
						else if(ignoreTag)
						{
							if(currentChar == '<')
								ignoreTag = false;
						}

						if(!ignoreTag)
						{
							switch(currentChar)
							{
								case '<':
									currentState = ParserState.TagOpen;
									currentTag = "";
									currentTagProperties = "";
									break;
								case '!':
									if(currentState == ParserState.TagOpen)
										previousState = currentState;
										currentState = ParserState.Comment;
									break;
								case '"':
									currentState = ParserState.String;
									break;
								case '/':
									if(currentState == ParserState.TagOpen)
										ignoreTag = true;
									break;
								case '>':
									currentState = ParserState.None;
									ignoreTag = !IsWantedTag(currentTag);
									if(currentTag.Length == 0)
										Console.WriteLine("Empty tag 2");
									break;
								default:
									if(currentState == ParserState.TagOpen)
									{
										if(!Char.IsWhiteSpace(currentChar))
											currentState = ParserState.TagName;
									}

									if(currentState == ParserState.TagName)
									{
										if(!Char.IsWhiteSpace(currentChar))
										{
											if(currentTag.Length == iLongestSearchTag)
												ignoreTag = true; // We no longer want this tag
											else
												currentTag += currentChar;												
										}
										else
										{
											ignoreTag = !IsWantedTag(currentTag);
										}
									}
									break;
							}
						}
					}
				//}
				//catch
				//{
					// Woopsie daisy, looks like we won't have a preview here either
					// Add appropriate log message
				//	return null; 
				//}	
			}

			return null;
		}
		 */
		#endregion

		/// <summary>
		/// Parses a given string, attempts to extract useful data for a URL Preview
		/// </summary>
		/// <param name="str">The string to extract data from</param>
		/// <returns>A URLPreview or null</returns>
		private static URLPreview ParseResponseString(String str)
		{
			URLPreview preview = new URLPreview();

			bool conditionMetTitle = false;
			bool conditionMetImage = false;

			string title = String.Empty;
			string imageUrl = String.Empty;
			string description = String.Empty;

			int strLength = str.Length;
			//try 
			//{
				bool ignoreTag = false;
				int tagIndex = -1;
				ParserState currentState = ParserState.None;
				ParserLocation currentLocation = ParserLocation.HTML;
				char previousChar = 'a';
				char currentChar = 'a';
				string currentTag = "";
				string currentTagContent = "";

				for(int x = 0; x < strLength; x++)
				{
					previousChar = currentChar;
					currentChar = (char)str[x];

					// There is a very specific order here
					// 1. Check if we are in a comment (comments are always ignored)
					// 2. Check if we are in a string, if we are check if the string doesn't end here
					//		2.5 Check if we're parsing content, if we are we want this string data
					// 3. Check if we are ignoring a tag, if we are we check if the tag wasn't closed
					// 4. If we aren't ignoring a tag, we handle the rest

					// Check if we are still in a comment or string
					if(currentLocation == ParserLocation.Comment)
					{
						if(currentChar == '>' && previousChar == '-')
							currentLocation = ParserLocation.HTML;

						continue; // Current character is inside a command or the last character of a comment, do nothing
					}
					else if(currentLocation == ParserLocation.String)
					{
						// If we're parsing tag content we want the string data
						if(currentState == ParserState.TagAttributes || currentState == ParserState.Content)
							currentTagContent += currentChar;

						// Check if the string ends here
						if(currentChar == '"')
							currentLocation = ParserLocation.HTML;

						continue; // Current character is inside a string or the last character of a string, do nothing
					}
					else if(ignoreTag)
					{
						if(currentChar != '<')
							continue; // We're ignoring this tag until we find a new opening char, do nothing
						
						// New tag, let it flow through and continue with the code
						ignoreTag = false;
					}

					// If we aren't ignoring this tag we continue whatever we want to continue
					if(!ignoreTag)
					{
						switch(currentChar)
						{
							case '<':
								if(currentState == ParserState.Content)
								{
									ContentParser(preview, currentTag, currentTagContent);
									//Console.WriteLine("Tag: " + currentTag + ", Content: " + currentTagContent);
								}

								currentState = ParserState.TagOpen;
								currentTag = "";
								currentTagContent = "";
								break;
							case '>':
								if(currentState == ParserState.TagAttributes)
								{
									ContentParser(preview, currentTag, currentTagContent);
									currentState = ParserState.None;
									//Console.WriteLine("Tag: " + currentTag + ", Attributes: " + currentTagContent);
								}
								else
								{
									tagIndex = IsWantedTag(currentTag);
									if(tagIndex == -1)
									{
										currentState = ParserState.None;
										ignoreTag = true;
									}	
									else
										currentState = searchTags[tagIndex].parseMethod;
								}
								break;
							case '-':
								// There are tags like <!DOCTYPE, so we can't just check for !
								if(previousChar == '!')
								{
									if(currentState == ParserState.TagOpen)
									{
										currentState = ParserState.TagName;
										currentLocation = ParserLocation.Comment;
									}
								}
								break;
							case '"':
								currentLocation = ParserLocation.String;

								if(currentState == ParserState.TagAttributes || currentState == ParserState.Content)
									currentTagContent += '"';
								break;
							case '/':
								if(currentState == ParserState.TagOpen)
									ignoreTag = true;
								break;
							default:
								// If we're parsing tag content we want these characters
								if(currentState == ParserState.TagAttributes || currentState == ParserState.Content)
									currentTagContent += currentChar;
								else
								{
									// If the parser is at a tag open '<' and the next character is not a whitespace, then we're reading the tag name
									if(currentState == ParserState.TagOpen)
									{
										if(!Char.IsWhiteSpace(currentChar))
											currentState = ParserState.TagName;
									}

									// If we're at the tag name we want to store that in a variable
									if(currentState == ParserState.TagName)
									{
										if(!Char.IsWhiteSpace(currentChar))
										{
											if(currentTag.Length == iLongestSearchTag)
												ignoreTag = true; // We no longer want this tag
											else
												currentTag += currentChar;
										}
										else
										{
											tagIndex = IsWantedTag(currentTag);
											if(tagIndex == -1)
												ignoreTag = true;
											else
												currentState = searchTags[tagIndex].parseMethod;
										}
									}
								}
								break;
						}
					}
					


					if(preview.PreferredTitle && preview.PreferredImageUrl && preview.PreferredDescription)
					{
						return preview;
					}
				}
			
			//}
			//catch
			//{
			// Woopsie daisy, looks like we won't have a preview here either
			// Add appropriate log message
			//	return null; 
			//}	

			return preview;
		}

		/// <summary>
		/// Checks if the tag is one we want
		/// </summary>
		/// <param name="strTag">The tag to check</param>
		/// <returns></returns>
		private static int IsWantedTag(string strTag)
		{
			// Only check tags that have the minimum required length
			if(strTag.Length > iShortestSearchTag)
			{
				for(int x = 0; x < iSearchTagsAmount; x++)
				{
					// Do a length tag, this is way faster than a normal compare
					if(searchTags[x].tag.Length == strTag.Length)
					{
						// Do a proper compare, Ordinal is the fastest
						if(string.Equals(searchTags[x].tag, strTag, StringComparison.OrdinalIgnoreCase))
						{
							// We've got a hit, stop comparing
							//Console.WriteLine("Wanted Tag: " + strTag);
							return x;
						}
					}
				}
			}
			return -1;
		}

		private static void ContentParser(URLPreview destinationPreview, string tag, string strContent)
		{
			int tagLength = tag.Length;
			
			// Title, Description and ImageUrl in meta og tags
			if(tagLength == "meta".Length)
			{
				int indexOfOg = strContent.IndexOf("og:")+3;
				if(indexOfOg != -1)
				{
					int indexOfContentValueStart = strContent.IndexOf("content")+9;
					int contentValueLength = (strContent.IndexOf("\"", indexOfContentValueStart) - indexOfContentValueStart);
					if(indexOfContentValueStart != -1)
					{ 
						int indexOfProperty = strContent.IndexOf("title", indexOfOg, 5);
						if(indexOfProperty != -1)
						{
							destinationPreview.Title = strContent.Substring(indexOfContentValueStart, contentValueLength);
							destinationPreview.PreferredTitle = true;
							return;
						}

						indexOfProperty = strContent.IndexOf("description", indexOfOg, 11);
						if(indexOfProperty != -1)
						{
							destinationPreview.Description = strContent.Substring(indexOfContentValueStart, contentValueLength);
							destinationPreview.PreferredDescription = true;
							return;
						}

						indexOfProperty = strContent.IndexOf("image", indexOfOg, 5);
						if(indexOfProperty != -1)
						{
							destinationPreview.ImageUrl = strContent.Substring(indexOfContentValueStart, contentValueLength);
							destinationPreview.PreferredImageUrl = true;
							return;
						}
					}
				}
			}
			// Title
			else if(tagLength == "title".Length)
			{
				destinationPreview.Title = strContent;
			}
			// ImageUrl
			else if(tagLength == "img".Length)
			{
				int indexOfSrc = strContent.IndexOf("src");
				if(indexOfSrc != -1)
				{
					int indexOfContentValueStart = indexOfSrc + 5;
					int contentValueLength = (strContent.IndexOf("\"", indexOfContentValueStart) - indexOfContentValueStart);
					if(indexOfContentValueStart != -1)
					{
						destinationPreview.ImageUrl = strContent.Substring(indexOfContentValueStart, contentValueLength);
						return;
					}
				}
			}
		}
	}
}
