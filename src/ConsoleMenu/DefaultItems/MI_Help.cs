﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConsoleMenu.DefaultItems
{
	public class MI_Help : CMenuItem
	{
		private readonly CMenu _Menu;

		public MI_Help (CMenu menu)
			: base ("help")
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			_Menu = menu;

			HelpText = ""
				+ "help [command]\n"
				+ "Displays a help text for the specified command, or\n"
				+ "Displays a list of all available commands.";
		}

		public override void Execute (string arg)
		{
			DisplayHelp (arg, _Menu, false);
		}

		private void DisplayHelp (string arg, CMenuItem context, bool isInner)
		{
			if (arg == null) {
				throw new ArgumentNullException ("arg");
			}
			if (context == null) {
				throw new ArgumentNullException ("context");
			}

			if (string.IsNullOrEmpty (arg)) {
				if (!DisplayItemHelp (context, !context.Any ())) {
					DisplayAvailableCommands (context, isInner);
				}
				return;
			}

			var cmd = arg;
			var inner = context.GetMenuItem (ref cmd, out arg, true, false, false);
			if (inner != null) {
				DisplayHelp (arg, inner, true);
			}
		}

		private bool DisplayItemHelp (CMenuItem item, bool force)
		{
			if (item == null) {
				throw new ArgumentNullException ("item");
			}

			if (item.HelpText == null) {
				if (force) {
					OnWriteLine ("No help available for " + item.Selector);
				}
				return false;
			}
			else {
				OnWriteLine (item.HelpText);
				return true;
			}
		}

		private void DisplayAvailableCommands (CMenuItem menu, bool inner)
		{
			if (menu == null) {
				throw new ArgumentNullException ("menu");
			}

			if (!inner) {
				OnWriteLine ("Available commands:");
			}
			var abbreviations = menu
				.CommandAbbreviations ()
				.OrderBy (it => it.Key)
				.ToArray();
			var longestAbbreviation = abbreviations.Max (a => a.Value?.Length) ?? 0;
			foreach (var ab in abbreviations) {
				if (ab.Value == null) {
					OnWrite (new string (' ', longestAbbreviation + 3));
				}
				else {
					OnWrite (ab.Value.PadRight (longestAbbreviation) + " | ");
				}
				OnWriteLine (ab.Key);
			}
			if (!inner) {
				OnWriteLine ("Type \"help <command>\" for individual command help.");
			}
		}
	}
}
