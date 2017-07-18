#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is released under the MIT License.
		See file License.txt for details.
	</License>
	<CreationDate>2017-07-17 17:06:34Z</CreationDate>
</File>
*/
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using FontAwesome.WPF;
using MarkdownMonster;
using MarkdownMonster.AddIns;

namespace TocAddin
{
	public class TocAddin : MarkdownMonster.AddIns.MarkdownMonsterAddin
	{
		const string AddinTitle = "TOC Generator";
		public override void OnApplicationStart()
		{
			base.OnApplicationStart();

			// Id - should match output folder name
			Id = "TocAddin";

			// a descriptive name - shows up on labels and tooltips for components
			Name = AddinTitle;


			// by passing in the add in you automatically
			// hook up OnExecute/OnExecuteConfiguration/OnCanExecute
			var menuItem = new AddInMenuItem(this)
			{
				Caption = "Generate TOC",

				// if an icon is specified it shows on the toolbar
				// if not the add-in only shows in the add-ins menu
				FontawesomeIcon = FontAwesomeIcon.Book
			};

			// if you don't want to display config or main menu item clear handler
			menuItem.ExecuteConfiguration = null;

			// Must add the menu to the collection to display menu and toolbar items            
			this.MenuItems.Add(menuItem);
		}

		public override void OnExecute(object sender)
		{
			try
			{
				ExecuteCore();
			}
			catch (Exception ex)
			{
				MessageBox.Show("An error occurred creating table of contents. " + ex.Message, AddinTitle,
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		const string tocBeginIndicator = "[//]: # (TOC Begin)";

		const string tocEndIndicator = "[//]: # (TOC End)";

		void ExecuteCore()
		{
			if (!mmApp.Configuration.MarkdownOptions.AutoLinks)
			{
				var result = MessageBox.Show("AutoLinks must be enabled in Markdown Monster "
											+ "for table of contents links to function. "
											+ "Is it okay to enable AutoLinks?", "Enable AutoLinks?",
					MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

				if (result == MessageBoxResult.Cancel)
				{
					return;
				}

				if (result == MessageBoxResult.Yes)
				{
					mmApp.Configuration.MarkdownOptions.AutoLinks = true;
				}
			}

			MarkdownDocumentEditor editor = GetMarkdownEditor();
			int caretLineNumber = editor.GetLineNumber();

			var markdown = GetMarkdown();
			string[] lines = markdown.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

			int maxDepth = TocAddinConfiguration.Current.MaxDepth;

			if (maxDepth < 0)
			{
				maxDepth = 3;
			}

			string[] prefixes = new string[maxDepth];
			string hashString = string.Empty;
			for (int i = 0; i < maxDepth; i++)
			{
				hashString = hashString + "#";
				prefixes[i] = hashString + " ";
			}

			var headings = new List<Heading>();

			int beginTocLineIndex = caretLineNumber;
			int endTocLineIndex = caretLineNumber;
			bool hasBeginIndicator = false;
			bool hasEndIndicator = false;

			int lineIndex = 0;
			
			foreach (var line in lines)
			{
				if (line.Contains(tocBeginIndicator))
				{
					hasBeginIndicator = true;
					beginTocLineIndex = lineIndex;
				}
				else if (line.Contains(tocEndIndicator))
				{
					hasEndIndicator = true;
					endTocLineIndex = lineIndex;
				}

				lineIndex++;
			}

			bool replaceToc = hasBeginIndicator && hasEndIndicator && beginTocLineIndex <= endTocLineIndex;

			int firstDepth = -1;
			lineIndex = -1;

			/* Locate headings */
			foreach (var line in lines)
			{
				lineIndex++;

				if (replaceToc && lineIndex < endTocLineIndex)
				{
					continue;
				}

				if (!replaceToc && lineIndex < caretLineNumber)
				{
					continue;
				}
				
				var trimmed = line.TrimStart();
				for (int i = prefixes.Length - 1; i >= 0; i--)
				{
					var prefix = prefixes[i];
					if (trimmed.StartsWith(prefix))
					{
						string text = trimmed.Substring(i + 2);
						string anchorId = "#" + text.Replace(" ", "-").ToLower();

						if (firstDepth == -1)
						{
							firstDepth = i;
						}

						/* The depth is calculated relative the the first depth encountered. */
						int depth = i - firstDepth;				

						headings.Add(new Heading(depth, text, anchorId));
					}
				}
			}

			/* Create an array of tabs that is used to prepend generated toc links. */
			string[] tabArray = new string[maxDepth];
			string tabString = string.Empty;
			for (int i = 0; i < maxDepth; i++)
			{
				tabArray[i] = tabString;
				tabString = tabString + "\t";
			}

			/* Create the TOC */
			StringBuilder sb = new StringBuilder();

			bool first = true;

			sb.AppendLine(tocBeginIndicator);
			foreach (Heading heading in headings)
			{		
				string tabs = tabArray[heading.Depth];
				sb.AppendLine($"{tabs}* [{heading.Text}]({heading.Href})");
			}

			sb.AppendLine();
			sb.AppendLine(tocEndIndicator);

			string tocText = sb.ToString();

			/* If there isn't an existing TOC, set the current selection to the TOC. */
			if (!replaceToc)
			{
				SetSelection(tocText);
			}
			else
			{
				int indexOfStart = markdown.IndexOf(tocBeginIndicator, StringComparison.Ordinal);
				int indexOfEnd = markdown.IndexOf(tocEndIndicator, StringComparison.Ordinal);
				if (indexOfStart < 0 || indexOfEnd < 0 || indexOfStart > indexOfEnd)
				{
					/* The TOC indicators are out of whack. */
					SetSelection(tocText);
				}
				else
				{
					/* There's an existing TOC, so replace it with the new TOC.
					 * There's probably a better way to replace a portion of the document. 
					 * The AceEditor looks able to do that. But, I'm concerned that interacting with it directly 
					 * may polute the state of the MarkdownDocumentEditor. 
					 * Rick, please let me know if there's an API for this. */
					string beforeToc = markdown.Substring(0, indexOfStart);
					string afterToc = markdown.Substring(indexOfEnd + tocEndIndicator.Length);
					string newMarkdown = beforeToc + tocText + afterToc;

					SetMarkdown(newMarkdown);
				}
			}
		}

		//		public override void OnExecuteConfiguration(object sender)
		//		{
		//			MessageBox.Show("Configuration for our sample Addin", "Markdown Addin Sample",
		//							MessageBoxButton.OK, MessageBoxImage.Information);
		//		}

		public override bool OnCanExecute(object sender)
		{
			return true;
		}

		class Heading
		{
			public int Depth { get; }
			public string Text { get; }

			public string Href { get; }

			public Heading(int depth, string text, string href)
			{
				Depth = depth;
				Text = text;
				Href = href;
			}
		}
	}
}
