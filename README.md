# TOC Generator for Markdown Monster

This project is an addin for [Markdown Monster](https://markdownmonster.west-wind.com/). The addin generator a table of contents for a markdown document.

When installed, a book icon is located in Markdown Monster's toolbar.
When you click the button, the addin generates (or regenerates) a TOC.

The addin reads each line of your markdown file, looking for headings. It then generates the TOC based on the depth of each heading.

The addin places the TOC between 'hidden' markers, like so:
[//]: # (TOC Begin)
TOC is placed here.
[//]: # (TOC End)

By placing the TOC between the markers, it is able to regenerate, without the user manually replacing the TOC.

When first generating a TOC, the TOC is placed at the cursors current line.

> **NOTE:** Previous headings, which reside before the TOC, are ignored.

Markdown Monster's *AutoLinks* setting needs to be enabled. If the setting is not enabled, the addin enables it for you.

By default, the addin uses a maximum depth of 3 when generating the TOC. That means  
`### A Heading`  
is picked up. But,  
`#### A Heading`  
is not.

To change the maximum depth, set the MaxDepth setting in the addin's configuration file.

> **TIP:** If the TOC links are not scrolling for you, remove the *base* element from the *head* element of the page. 

