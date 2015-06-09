using System;
using System.IO;
using System.Net;
using System.Net.Mime;

namespace URLPreviewLib
{
	public static class URLPreviewGenerator
	{
		/// <summary>
		/// The tags we'll be searching for
		/// </summary>
		private static String[] strSearchTags = 
		{
			"meta",
			"title",
			"img",
			"head"
		};

		/// <summary>
		/// The amount of tags
		/// </summary>
		private static int iSearchTagsAmount = strSearchTags.Length;

		/// <summary>
		/// Byte enum of all the states the parser can have
		/// </summary>
		private enum ParserState : byte
		{
			FindTagStart,
			FindTag,
			InComment,
		}

		/// <summary>
		/// Byte enum of the type of tags, either open or close tag
		/// </summary>
		private enum TagType : byte
		{
			Open,
			Close,
			None
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
			if(!response.ContentType.Contains("text/html"))
				return null;

			// Parse the response
			return ParseResponseStream(response.GetResponseStream());	
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
				try 
				{
					ParserState beforeCommentState = ParserState.FindTagStart;
					ParserState currentState = ParserState.FindTagStart;
					TagType currentTagType = TagType.None;
					char[] previousChar = {'a', 'a'};
					char currentChar = 'a';

					while(!sr.EndOfStream)
					{
						previousChar[2] = previousChar[1];
						previousChar[1] = currentChar;
						currentChar = (char)sr.Read();

						if(currentState == ParserState.InComment)
						{
							if(currentChar == '>' && previousChar[1] == '-' && previousChar[2] == '-')
								currentState = beforeCommentState;
						}
						else
						{
							switch(currentChar)
							{
								case '<':
									currentState = ParserState.FindTag;
									currentTagType = TagType.Open;
									break;
								case '>':
									currentState = ParserState.FindTagStart;
									currentTagType = TagType.None;
									break;
								case '!':
									if(currentState == ParserState.FindTag)
										beforeCommentState = currentState;
										currentState = ParserState.InComment;
									break;
								case '/':
									break;
							}
						}
					}
				}
				catch
				{
					// Woopsie daisy, looks like we won't have a preview here either
					// Add appropriate log message
					return null; 
				}	
			}

			return null;
		}
	}
}
