﻿using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Microsoft.Maui.Platform
{
	public static class SearchBarExtensions
	{
		private static readonly string[] _backgroundColorKeys =
		{
			"TextControlBackground",
			"TextControlBackgroundPointerOver",
			"TextControlBackgroundFocused",
			"TextControlBackgroundDisabled"
		};

		public static void UpdateBackground(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			UpdateColors(platformControl.Resources, _backgroundColorKeys, searchBar.Background?.ToPlatform());
		}

		public static void UpdateIsEnabled(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			platformControl.IsEnabled = searchBar.IsEnabled;
		}

		public static void UpdateCharacterSpacing(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			platformControl.CharacterSpacing = searchBar.CharacterSpacing.ToEm();
		}

		public static void UpdatePlaceholder(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			platformControl.PlaceholderText = searchBar.Placeholder ?? string.Empty;
		}

		private static readonly string[] _placeholderForegroundColorKeys =
		{
			"TextControlPlaceholderForeground",
			"TextControlPlaceholderForegroundPointerOver",
			"TextControlPlaceholderForegroundFocused",
			"TextControlPlaceholderForegroundDisabled"
		};

		public static void UpdatePlaceholderColor(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			UpdateColors(platformControl.Resources, _placeholderForegroundColorKeys,
				searchBar.PlaceholderColor?.ToPlatform());
		}

		public static void UpdateText(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			platformControl.Text = searchBar.Text;
		}

		private static readonly string[] _foregroundColorKeys =
		{
			"TextControlForeground",
			"TextControlForegroundPointerOver",
			"TextControlForegroundFocused",
			"TextControlForegroundDisabled"
		};

		public static void UpdateTextColor(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			UpdateColors(platformControl.Resources, _foregroundColorKeys, searchBar.TextColor?.ToPlatform());
		}

		private static void UpdateColors(ResourceDictionary resource, string[] keys, Brush? brush)
		{
			if (brush is null)
			{
				resource.RemoveKeys(keys);
			}
			else
			{
				resource.SetValueForAllKey(keys, brush);
			}
		}

		public static void UpdateFont(this AutoSuggestBox platformControl, ISearchBar searchBar, IFontManager fontManager) =>
			platformControl.UpdateFont(searchBar.Font, fontManager);

		public static void UpdateHorizontalTextAlignment(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			platformControl.HorizontalContentAlignment = searchBar.HorizontalTextAlignment.ToPlatformHorizontalAlignment();
		}

		public static void UpdateVerticalTextAlignment(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			platformControl.VerticalContentAlignment = searchBar.VerticalTextAlignment.ToPlatformVerticalAlignment();
		}

		public static void UpdateMaxLength(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			var currentControlText = platformControl.Text;

			if (currentControlText.Length > searchBar.MaxLength)
				platformControl.Text = currentControlText.Substring(0, searchBar.MaxLength);
		}
		
		public static void UpdateIsReadOnly(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			platformControl.IsEnabled = searchBar.IsReadOnly;
		}

		public static void UpdateIsTextPredictionEnabled(this AutoSuggestBox platformControl, ISearchBar searchBar)
		{
			// AutoSuggestBox does not support this property
		}
	}
}
