using MarkdownMonster.AddIns;

namespace TocAddin
{
	public class TocAddinConfiguration : BaseAddinConfiguration<TocAddinConfiguration>
	{
		public TocAddinConfiguration()
		{
			// uses this file for storing settings in `%appdata%\Markdown Monster`
			// to persist settings call `TocAddinConfiguration.Current.Write()`
			// at any time or when the addin is shut down
			ConfigurationFilename = "TocAddin.json";
		}

		// Add properties for any configuration setting you want to persist and reload
		// you can access this object as 
		//     TocAddinConfiguration.Current.PropertyName

		public int MaxDepth { get; set; } = 3;
	}
}