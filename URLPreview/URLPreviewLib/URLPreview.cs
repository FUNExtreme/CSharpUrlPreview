namespace URLPreviewLib
{
    public class URLPreview
    {
		/// <summary>
		/// The title of page
		/// </summary>
		private string strTitle;

		/// <summary>
		/// The description of the page (optional)
		/// </summary>
		private string strDescription;

		/// <summary>
		/// The url to the image we'll use for the preview
		/// </summary>
		private string strImageUrl;

		/// <summary>
		/// Get/Set the page title
		/// </summary>
		public string Title
		{
			get { return this.strTitle; }
			set { this.strTitle = value; }
		}

		/// <summary>
		/// Get/Set the page description
		/// </summary>
		public string Description
		{
			get { return this.strDescription; }
			set { this.strDescription = value; }
		}

		/// <summary>
		/// Get/Set the url of the image for this preview
		/// </summary>
		public string ImageUrl
		{
			get { return this.strImageUrl; }
			set { this.strImageUrl = value; }
		}
    }
}
