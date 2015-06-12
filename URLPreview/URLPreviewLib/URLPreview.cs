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
		/// True if og:title was found
		/// </summary>
		private bool bPreferredTitle;

		/// <summary>
		/// True if og:description was found
		/// </summary>
		private bool bPreferredDescription;

		/// <summary>
		/// True if og:image was found
		/// </summary>
		private bool bPreferredImageUrl;

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

		/// <summary>
		/// Get/Set if the preferred title was found
		/// </summary>
		public bool PreferredTitle
		{
			get { return this.bPreferredTitle; }
			set { this.bPreferredTitle = value; }
		}

		/// <summary>
		/// Get/Set if the preferred description was found
		/// </summary>
		public bool PreferredDescription
		{
			get { return this.bPreferredDescription; }
			set { this.bPreferredDescription = value; }
		}

		/// <summary>
		/// Get/Set if the preferred image url was found
		/// </summary>
		public bool PreferredImageUrl
		{
			get { return this.bPreferredImageUrl; }
			set { this.bPreferredImageUrl = value; }
		}
    }
}
