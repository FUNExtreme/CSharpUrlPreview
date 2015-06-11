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
		/// The tags we'll be searching for
		/// IMPORTANT: order by length
		/// </summary>
		private static String[] strSearchTags = 
		{
			"title",
			"head",
			"meta",
			"img"
		};

		/// <summary>
		/// The length of the longest tag we'll be searching for
		/// </summary>
		private static int iLongestSearchTag = strSearchTags[0].Length;

		/// <summary>
		/// The length of the shortest tag we'll be searching for
		/// </summary>
		private static int iShortestSearchTag = strSearchTags[strSearchTags.Length - 1].Length;

		/// <summary>
		/// The amount of tags
		/// </summary>
		private static int iSearchTagsAmount = strSearchTags.Length;

		/// <summary>
		/// Byte enum of all the states the parser can have
		/// </summary>
		private enum ParserState : byte
		{
			None,
			TagOpen,
			TagName,
			TagProperties,
			String,
			Comment,
		}

		/// <summary>
		/// Generates a preview from the given url
		/// </summary>
		/// <param name="strUrl">The url to generate a preview for</param>
		/// <returns>A preview object, null if invalid url</returns>
		public static Preview CreatePreview(string strUrl)
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
				Preview result = null;

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
				for(int x = 0; x < 10000; x++)
				{
					result = ParseResponseString(str);
				}
				swWatch.Stop();

				Console.WriteLine("Parse (average of 10000): " + (swWatch.ElapsedMilliseconds / 10000).ToString());
				return result;
			}

			return null;
		}

		/// <summary>
		/// Parses the given response stream, only handles whatever is needed for previews.
		/// The following code is not written for readability, but for speed. So nothing is split up into seperate functions, function calls are slow
		/// </summary>
		/// <param name="sResponse">The stream to read from</param>
		private static Preview ParseResponseStream(Stream oResponse)
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
									// We can skip this whole block if the tag is being ignored
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

		private static Preview ParseResponseString(String str)
		{
			bool conditionMetTitle = false;
			bool conditionMetImage = false;
			bool conditionMetDescription = false;

			string title = String.Empty;
			string imageUrl = String.Empty;
			string description = String.Empty;

			int strLength = str.Length;
			//try 
			//{
				bool ignoreTag = false;
				ParserState previousState = ParserState.None;
				ParserState currentState = ParserState.None;
				char[] previousChar = { 'a', 'a' };
				char currentChar = 'a';
				string currentTag = "";
				string currentTagProperties = "";

				for(int x = 0; x < strLength; x ++)
				{
					previousChar[1] = previousChar[0];
					previousChar[0] = currentChar;
					currentChar = (char)str[x];

					if(currentState == ParserState.Comment)
					{
						if(currentChar == '>' && previousChar[0] == '-' && previousChar[1] == '-')
							currentState = previousState;
						
						continue; // Current character is inside a command or the last character of a comment, do nothing
					}
					else if(currentState == ParserState.String)
					{
						if(currentChar == '"')
							currentState = previousState;

						continue; // Current character is inside a string or the last character of a string, do nothing
					}
					else if(ignoreTag)
					{
						if(currentChar != '<')
							continue; // We're ignoring this tag until we find a new opening char, do nothing

						// New tag, let it flow through and continue with the code
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
								break;
							default:
								// We can skip this whole block if the tag is being ignored
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
			//}

			return null;
		}

		private static bool IsWantedTag(string strTag)
		{
			//Console.WriteLine(strTag);

			// Only check tags that have the minimum required length
			if(strTag.Length > iShortestSearchTag)
			{
				for(int x = 0; x < iSearchTagsAmount; x++)
				{
					// Do a length tag, this is way faster than a normal compare
					if(strSearchTags[x].Length == strTag.Length)
					{
						// Do a proper compare, Ordinal is the fastest
						if(string.Equals(strSearchTags[x], strTag, StringComparison.OrdinalIgnoreCase))
						{
							// We've got a hit, stop comparing
							//Console.WriteLine("Tag: " + strTag);
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
