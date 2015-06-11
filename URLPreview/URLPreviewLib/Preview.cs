namespace URLPreviewLib
{
    public class Preview
    {
		private string strTitle;
		private string strDescription;
		private string strImageUrl;

		public string Title
		{
			get { return this.strTitle; }
			set { this.strTitle = value; }
		}

		public string Description
		{
			get { return this.strDescription; }
			set { this.strDescription = value; }
		}

		public string ImageUrl
		{
			get { return this.strImageUrl; }
			set { this.strImageUrl = value; }
		}
    }
}
